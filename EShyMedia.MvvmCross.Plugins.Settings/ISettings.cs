using System;

namespace EShyMedia.MvvmCross.Plugins.Settings
{
    public interface ISettings
    {
        T GetValueOrDefault<T>(string key, T defaultValue  = default(T));

        bool AddOrUpdateValue(string key, Object value);

        string GetSecuredValue(string key);

        void AddOrUpdateSecuredValue(string key, string value);

        void RemoveSecuredValue(string key);
    }
}