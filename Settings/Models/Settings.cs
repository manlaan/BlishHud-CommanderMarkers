using System;
using Blish_HUD.Settings;

namespace Manlaan.CommanderMarkers.Settings.Models;

public record Setting<T>(string Key, T DefaultValue, Func<string>? Name = null, Func<string>? Description = null)
{
    public string Key { get; } = Key;
    public T DefaultValue { get; } = DefaultValue;
    public Func<string>? Name { get; } = Name;
    public Func<string>? Description { get; } = Description;
}


public static class Settings
{
 
}

public static class SettingCollectionExtensions
{
    public static SettingEntry<TEntry> DefineSetting<TEntry>(this SettingCollection collection, Setting<TEntry> setting)
    {
        return collection.DefineSetting(setting.Key, setting.DefaultValue, setting.Name, setting.Description);
    }
    public static SettingEntry<TEntry> DefineSettingRange<TEntry>(this SettingCollection collection, Setting<TEntry> setting,float min, float max)
    {
        var settingEntry = collection.DefineSetting(setting.Key, setting.DefaultValue, setting.Name, setting.Description);
        (settingEntry as SettingEntry<float>).SetRange(min, max);
        return settingEntry;
    }


}