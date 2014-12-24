using System;
using Windows.ApplicationModel;
using Windows.Security.Credentials;
using Windows.Storage;

namespace EShyMedia.MvvmCross.Plugins.Settings.WindowsCommon
{
    public class MvxWindowsCommonSettings : ISettings
    {
        private static ApplicationDataContainer LocalSettings
        {
            get { return ApplicationData.Current.LocalSettings; }
        }

        private static ApplicationDataContainer RoamingSettings
        {
            get { return ApplicationData.Current.RoamingSettings; }
        }

        private static T GetValue<T>(ApplicationDataContainer container, string key, T defaultValue = default(T))
        {
            if (container == null) throw new ArgumentNullException("container");

            object value;

            if (container.Values.TryGetValue(key, out value))
                return (T) value;

            return defaultValue;
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default(T), bool roaming = false)
        {
            return GetValue(!roaming ? LocalSettings : RoamingSettings, key, defaultValue);
        }

        private static bool AddOrUpdateValue<T>(ApplicationDataContainer container, string key, T value = default(T))
        {
            if (container == null) throw new ArgumentNullException("container");

            if (container.Values.ContainsKey(key))
            {
                container.Values[key] = value;
                return true;
            }

            container.Values.Add(key, value);
            return true;    
        }

        public bool AddOrUpdateValue<T>(string key, T value = default(T), bool roaming = false)
        {
            return AddOrUpdateValue(!roaming ? LocalSettings : RoamingSettings, key, value);
        }

        private static bool DeleteValue(ApplicationDataContainer container, string key)
        {
            if (container.Values.ContainsKey(key))
            {
                container.Values.Remove(key);
                return true;
            }

            return false;
        }

        public bool DeleteValue(string key, bool roaming = false)
        {
            return DeleteValue(!roaming ? LocalSettings : RoamingSettings, key);
        }

        private static bool Contains(ApplicationDataContainer container, string key)
        {
            return container.Values.ContainsKey(key);
        }

        public bool Contains(string key, bool roaming = false)
        {
            return Contains(!roaming ? LocalSettings : RoamingSettings, key);
        }

        private bool ClearAllValues(ApplicationDataContainer container)
        {
            container.Values.Clear();
            return true;
        }

        public bool ClearAllValues(bool roaming = false)
        {
            return ClearAllValues(!roaming ? LocalSettings : RoamingSettings);
        }      

        public string GetSecuredValue(string key)
        {
            try
            {
                var vault = new PasswordVault();
                return vault.Retrieve(Package.Current.Id.Name, key).Password;
            }
            catch
            {
                return null;
            }
        }

        public void AddOrUpdateSecuredValue(string key, string value)
        {
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(Package.Current.Id.Name, key, value));
        }

        public void RemoveSecuredValue(string key)
        {
            var vault = new PasswordVault();
            var passwordCredential = vault.Retrieve(Package.Current.Id.Name, key);
            vault.Remove(passwordCredential);
        }
    }
}