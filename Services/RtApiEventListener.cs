using Manlaan.CommanderMarkers.RtApi;
using Manlaan.CommanderMarkers.Utils;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace Manlaan.CommanderMarkers.Services;

public sealed class RtApiEventListener : IDisposable
{
    private readonly RtApiConnection _connection;
    private readonly object _roleSync = new();
    private Thread? _listenerThread;
    private CancellationTokenSource? _cancellation;
    private bool _selfIsCommander;
    private bool _selfIsLieutenant;
    private bool _hasSelfRole;
    private RealTimeDataLayout.GroupTypeValue _groupType = RealTimeDataLayout.GroupTypeValue.None;

    public RtApiEventListener(RtApiConnection connection)
    {
        _connection = connection;
        _connection.ConnectionStateChanged += OnConnectionStateChanged;
    }

    public event EventHandler? SelfRoleChanged;
    public event EventHandler? RoleCleared;

    public bool GrantsCommanderPermissions()
    {
        lock (_roleSync)
        {
            if (!_hasSelfRole)
            {
                return false;
            }

            if (_groupType != RealTimeDataLayout.GroupTypeValue.Squad
                && _groupType != RealTimeDataLayout.GroupTypeValue.RaidSquad)
            {
                return false;
            }

            return _selfIsCommander || _selfIsLieutenant;
        }
    }

    public void Start()
    {
        if (_listenerThread != null)
        {
            return;
        }

        _cancellation = new CancellationTokenSource();
        _listenerThread = new Thread(() => ListenLoop(_cancellation.Token))
        {
            IsBackground = true,
            Name = "CommanderMarkers.RtApiEventListener",
        };
        _listenerThread.Start();
    }

    public void Stop()
    {
        _cancellation?.Cancel();
        _listenerThread?.Join(TimeSpan.FromSeconds(2));
        _listenerThread = null;
        _cancellation?.Dispose();
        _cancellation = null;
        ClearRoleCache(raiseEvent: false);
    }

    public void Dispose()
    {
        _connection.ConnectionStateChanged -= OnConnectionStateChanged;
        Stop();
    }

    private void OnConnectionStateChanged(object? sender, RtApiConnectionState state)
    {
        if (state != RtApiConnectionState.Active)
        {
            ClearRoleCache(raiseEvent: true);
        }
    }

    private void ListenLoop(CancellationToken cancellationToken)
    {
        uint lastConsumedSequence = 0;
        var waitHandles = new WaitHandle[] { cancellationToken.WaitHandle };

        while (!cancellationToken.IsCancellationRequested)
        {
            if (!_connection.EnsureActive())
            {
                lastConsumedSequence = 0;
                Thread.Sleep(500);
                continue;
            }

            var processId = ResolveProcessId();
            if (processId <= 0)
            {
                Thread.Sleep(500);
                continue;
            }

            try
            {
                using var eventSignal = TryOpenEventSignal(processId);
                using var eventsMap = TryOpenEventsMap(processId);
                if (eventSignal == null || eventsMap == null)
                {
                    Thread.Sleep(500);
                    continue;
                }

                using var accessor = eventsMap.CreateViewAccessor(
                    0,
                    RealTimeDataLayout.EventsMapSize,
                    MemoryMappedFileAccess.ReadWrite);

                var handles = eventSignal != null
                    ? new[] { cancellationToken.WaitHandle, eventSignal }
                    : waitHandles;

                while (!cancellationToken.IsCancellationRequested && _connection.IsConnectedForProcess(processId))
                {
                    var signaled = WaitHandle.WaitAny(handles, 500);
                    if (signaled == 0)
                    {
                        break;
                    }

                    if (!_connection.EnsureActive())
                    {
                        break;
                    }

                    UpdateGroupTypeFromConnection();
                    lastConsumedSequence = DrainEvents(accessor, lastConsumedSequence);
                }
            }
            catch (FileNotFoundException)
            {
                Thread.Sleep(500);
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(500);
            }
            catch (IOException)
            {
                Thread.Sleep(500);
            }
        }
    }

    private uint DrainEvents(MemoryMappedViewAccessor accessor, uint lastConsumedSequence)
    {
        var publishSequence = accessor.ReadUInt32(0);
        if (publishSequence == lastConsumedSequence)
        {
            return lastConsumedSequence;
        }

        var slotCount = Math.Max(1, (int)Math.Min(accessor.ReadUInt32(8), RealTimeDataLayout.MaxEventSlots));
        var nextSequence = lastConsumedSequence;

        while (nextSequence < publishSequence)
        {
            nextSequence++;
            var slotIndex = (int)((nextSequence - 1) % slotCount);
            var entryOffset = RealTimeDataLayout.EventsHeaderSize + (slotIndex * RealTimeDataLayout.EventEntrySize);
            var kind = (RealTimeDataLayout.RtApiEventKind)accessor.ReadUInt32(entryOffset);
            HandleEvent(kind, accessor, entryOffset + RealTimeDataLayout.EventKindSize);
        }

        accessor.Write(4, nextSequence);
        return nextSequence;
    }

    private void HandleEvent(RealTimeDataLayout.RtApiEventKind kind, MemoryMappedViewAccessor accessor, int memberOffset)
    {
        var flags = accessor.ReadUInt32(memberOffset + RealTimeDataLayout.GroupMemberFlags);
        var isSelf = (flags & RealTimeDataLayout.FlagIsSelf) != 0;
        if (!isSelf)
        {
            return;
        }

        switch (kind)
        {
            case RealTimeDataLayout.RtApiEventKind.GroupMemberLeft:
                ClearRoleCache(raiseEvent: true);
                break;
            case RealTimeDataLayout.RtApiEventKind.GroupMemberJoined:
            case RealTimeDataLayout.RtApiEventKind.GroupMemberUpdated:
                UpdateSelfRole(
                    (flags & RealTimeDataLayout.FlagIsCommander) != 0,
                    (flags & RealTimeDataLayout.FlagIsLieutenant) != 0);
                break;
        }
    }

    private void UpdateSelfRole(bool isCommander, bool isLieutenant)
    {
        lock (_roleSync)
        {
            var changed = !_hasSelfRole
                || _selfIsCommander != isCommander
                || _selfIsLieutenant != isLieutenant;

            _hasSelfRole = true;
            _selfIsCommander = isCommander;
            _selfIsLieutenant = isLieutenant;

            if (changed)
            {
                GameThreadUtil.Enqueue(() => SelfRoleChanged?.Invoke(this, EventArgs.Empty));
            }
        }
    }

    private void ClearRoleCache(bool raiseEvent)
    {
        lock (_roleSync)
        {
            if (!_hasSelfRole && _groupType == RealTimeDataLayout.GroupTypeValue.None)
            {
                return;
            }

            _hasSelfRole = false;
            _selfIsCommander = false;
            _selfIsLieutenant = false;
            _groupType = RealTimeDataLayout.GroupTypeValue.None;
        }

        if (raiseEvent)
        {
            GameThreadUtil.Enqueue(() => RoleCleared?.Invoke(this, EventArgs.Empty));
        }
    }

    private void UpdateGroupTypeFromConnection()
    {
        if (!_connection.TryGetGroupType(out var groupType))
        {
            return;
        }

        lock (_roleSync)
        {
            _groupType = groupType;
        }
    }

    private static EventWaitHandle? TryOpenEventSignal(int processId)
    {
        try
        {
            return EventWaitHandle.OpenExisting(RealTimeDataLayout.EventSignalName(processId));
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            return null;
        }
    }

    private static MemoryMappedFile? TryOpenEventsMap(int processId)
    {
        try
        {
            return MemoryMappedFile.OpenExisting(RealTimeDataLayout.EventsMapName(processId));
        }
        catch (FileNotFoundException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static int ResolveProcessId()
    {
        try
        {
            var gw2Process = Blish_HUD.GameService.GameIntegration.Gw2Instance.Gw2Process;
            if (gw2Process != null && !gw2Process.HasExited)
            {
                return gw2Process.Id;
            }
        }
        catch
        {
            // Ignore and return zero.
        }

        return 0;
    }
}
