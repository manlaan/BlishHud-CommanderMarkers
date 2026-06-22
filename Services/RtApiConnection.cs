using Blish_HUD;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.RtApi;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Manlaan.CommanderMarkers.Services;

public sealed class RtApiConnection : IDisposable
{
    private readonly object _sync = new();
    private MemoryMappedFile? _mappedFile;
    private MemoryMappedViewAccessor? _accessor;
    private int _connectedProcessId;
    private RtApiConnectionState _state = RtApiConnectionState.NotDetected;

    public event EventHandler<RtApiConnectionState>? ConnectionStateChanged;

    public RtApiConnectionState State
    {
        get
        {
            lock (_sync)
            {
                return _state;
            }
        }
    }

    public bool IsActive => State == RtApiConnectionState.Active;

    public bool EnsureActive()
    {
        lock (_sync)
        {
            var processId = ResolveProcessId();
            if (processId <= 0)
            {
                DisconnectInternal(RtApiConnectionState.NotDetected);
                return false;
            }

            if (_mappedFile == null || _connectedProcessId != processId)
            {
                DisconnectInternal(_state);
                if (!TryConnect(processId))
                {
                    DisconnectInternal(RtApiConnectionState.NotDetected);
                    return false;
                }
            }

            if (!IsGameBuildActive())
            {
                DisconnectInternal(RtApiConnectionState.Inactive);
                return false;
            }

            SetState(RtApiConnectionState.Active);
            return true;
        }
    }

    public bool TryGetSquadMarkerPosition(int slotIndex, out Vector3 position)
    {
        position = Vector3.Zero;
        if (slotIndex < 0 || slotIndex >= RealTimeDataLayout.SquadMarkerSlotCount)
        {
            return false;
        }

        lock (_sync)
        {
            if (_accessor == null || !IsGameBuildActive())
            {
                return false;
            }

            var baseOffset = RealTimeDataLayout.SquadMarkers + (slotIndex * RealTimeDataLayout.SquadMarkerStride);
            var x = _accessor.ReadSingle(baseOffset);
            var y = _accessor.ReadSingle(baseOffset + 4);
            var z = _accessor.ReadSingle(baseOffset + 8);

            if (!IsSquadMarkerPlaced(x, y, z))
            {
                return false;
            }

            position = RtApiCoordinates.ToGame(x, y, z);
            return true;
        }
    }

    public bool TryImportSquadMarker(int slotIndex, MarkerCoord marker)
    {
        if (!EnsureActive() || !TryGetSquadMarkerPosition(slotIndex, out var position))
        {
            return false;
        }

        marker.icon = slotIndex + 1;
        marker.x = position.X;
        marker.y = position.Y;
        marker.z = position.Z;
        return true;
    }

    public void Dispose()
    {
        lock (_sync)
        {
            DisconnectInternal(RtApiConnectionState.NotDetected);
        }
    }

    private bool TryConnect(int processId)
    {
        try
        {
            _mappedFile = MemoryMappedFile.OpenExisting(RealTimeDataLayout.DataMapName(processId));
            _accessor = _mappedFile.CreateViewAccessor(0, RealTimeDataLayout.RealTimeDataSize, MemoryMappedFileAccess.Read);
            _connectedProcessId = processId;
            return true;
        }
        catch (FileNotFoundException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private bool IsGameBuildActive()
    {
        if (_accessor == null)
        {
            return false;
        }

        return _accessor.ReadUInt32(RealTimeDataLayout.GameBuild) != 0;
    }

    private void DisconnectInternal(RtApiConnectionState nextState)
    {
        _accessor?.Dispose();
        _accessor = null;
        _mappedFile?.Dispose();
        _mappedFile = null;
        _connectedProcessId = 0;
        SetState(nextState);
    }

    private void SetState(RtApiConnectionState nextState)
    {
        if (_state == nextState)
        {
            return;
        }

        _state = nextState;
        ConnectionStateChanged?.Invoke(this, nextState);
    }

    private static int ResolveProcessId()
    {
        try
        {
            var gw2Process = GameService.GameIntegration.Gw2Instance.Gw2Process;
            if (gw2Process != null && !gw2Process.HasExited)
            {
                return gw2Process.Id;
            }
        }
        catch
        {
            // Fall back to process enumeration below.
        }

        return FindProcessIdByName("Gw2-64") ?? FindProcessIdByName("Gw2") ?? 0;
    }

    private static int? FindProcessIdByName(string processName)
    {
        foreach (var process in Process.GetProcessesByName(processName))
        {
            try
            {
                return process.Id;
            }
            finally
            {
                process.Dispose();
            }
        }

        return null;
    }

    private static bool IsSquadMarkerPlaced(float x, float y, float z)
    {
        if (float.IsInfinity(x) || float.IsInfinity(y) || float.IsInfinity(z))
        {
            return false;
        }

        return Math.Abs(x) > RealTimeDataLayout.PlacedMarkerEpsilon
            || Math.Abs(y) > RealTimeDataLayout.PlacedMarkerEpsilon
            || Math.Abs(z) > RealTimeDataLayout.PlacedMarkerEpsilon;
    }
}

internal static class RtApiCoordinates
{
    public static Vector3 ToGame(float x, float elevation, float z)
    {
        return new Vector3(x, z, elevation);
    }
}
