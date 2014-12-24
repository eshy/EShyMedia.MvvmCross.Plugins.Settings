using System;

namespace EShyMedia.MvvmCross.Plugins.Settings
{
    public interface ISettings
    {
        T GetValueOrDefault<T>(string key, T defaultValue  = default(T), bool roaming = false);

        bool AddOrUpdateValue<T>(string key, T value = default(T), bool roaming = false);
        
        bool DeleteValue(string key, bool roaming = false);

        bool Contains(string key, bool roaming = false);

        bool ClearAllValues(bool roaming = false);

        string GetSecuredValue(string key);

        void AddOrUpdateSecuredValue(string key, string value);

        void RemoveSecuredValue(string key);
    }
}