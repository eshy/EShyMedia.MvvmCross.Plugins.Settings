using System;
using System.Globalization;
using Windows.Security.Credentials;
using Windows.Storage;

namespace EShyMedia.MvvmCross.Plugins.Settings.WindowsCommon
{
    public class MvxWindowsCommonSettings : ISettings
    {
        private static ApplicationDataContainer AppSettings
        {
            get { return ApplicationData.Current.LocalSettings; }
        }

        private readonly object _locker = new object();

        public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            object value;
            lock (_locker)
            {
                if (typeof(T) == typeof(decimal))
                {
                    string savedDecimal;
                    // If the key exists, retrieve the value.
                    if (AppSettings.Values.ContainsKey(key))
                    {
                        savedDecimal = (string)AppSettings.Values[key];
                    }
                    // Otherwise, use the default value.
                    else
                    {
                        savedDecimal = defaultValue.ToString();
                    }

                    value = Convert.ToDecimal(savedDecimal, CultureInfo.InvariantCulture);

                    return null != value ? (T)value : defaultValue;
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    string savedTime = null;
                    // If the key exists, retrieve the value.
                    if (AppSettings.Values.ContainsKey(key))
                    {
                        savedTime = (string)AppSettings.Values[key];
                    }

                    var ticks = string.IsNullOrWhiteSpace(savedTime) ? -1 : Convert.ToInt64(savedTime, CultureInfo.InvariantCulture);
                    if (ticks == -1)
                        value = defaultValue;
                    else
                        value = new DateTime(ticks);

                    return null != value ? (T)value : defaultValue;
                }

                // If the key exists, retrieve the value.
                if (AppSettings.Values.ContainsKey(key))
                {
                    value = (T)AppSettings.Values[key];
                }
                // Otherwise, use the default value.
                else
                {
                    value = defaultValue;
                }
            }

            return null != value ? (T)value : defaultValue;
        }

        public bool AddOrUpdateValue(string key, object value)
        {
            bool valueChanged = false;
            lock (_locker)
            {

                if (value is decimal)
                {
                    return AddOrUpdateValue(key, Convert.ToString((decimal)value, CultureInfo.InvariantCulture));
                }
                else if (value is DateTime)
                {
                    return AddOrUpdateValue(key, Convert.ToString(((DateTime)value).Ticks, CultureInfo.InvariantCulture));
                }


                // If the key exists
                if (AppSettings.Values.ContainsKey(key))
                {

                    // If the value has changed
                    if (AppSettings.Values[key] != value)
                    {
                        // Store key new value
                        AppSettings.Values[key] = value;
                        valueChanged = true;
                    }
                }
                // Otherwise create the key.
                else
                {
                    AppSettings.CreateContainer(key, ApplicationDataCreateDisposition.Always);
                    AppSettings.Values[key] = value;
                    valueChanged = true;
                }
            }

            return valueChanged;
        }

        public string GetSecuredValue(string key)
        {
            lock (_locker)
            {
                try
                {
                    var vault = new PasswordVault();
                    return vault.Retrieve(Windows.ApplicationModel.Package.Current.Id.Name, key).Password;
                }
                catch
                {
                    return null;
                }
            }
        }

        public void AddOrUpdateSecuredValue(string key, string value)
        {
            lock (_locker)
            {
                var vault = new PasswordVault();
                vault.Add(new PasswordCredential(Windows.ApplicationModel.Package.Current.Id.Name, key, value));
            }
        }

        public void RemoveSecuredValue(string key)
        {
            lock (_locker)
            {
                var vault = new PasswordVault();
                var passwordCredential = vault.Retrieve(Windows.ApplicationModel.Package.Current.Id.Name, key);
                vault.Remove(passwordCredential);
            }
        }
    }
}