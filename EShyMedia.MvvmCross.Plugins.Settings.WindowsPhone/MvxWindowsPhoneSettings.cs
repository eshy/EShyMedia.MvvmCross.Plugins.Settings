using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;

namespace EShyMedia.MvvmCross.Plugins.Settings.WindowsPhone
{
    public class MvxWindowsPhoneSettings : ISettings
    {
        static IsolatedStorageSettings Settings { get { return IsolatedStorageSettings.ApplicationSettings; } }
        private readonly object _locker = new object();

        public T GetValueOrDefault<T>(string key, T defaultValue = default(T), bool roaming = false)
        {
            T value;
            lock (_locker)
            {
                // If the key exists, retrieve the value.
                if (Settings.Contains(key))
                {
                    value = (T)Settings[key];
                }
                // Otherwise, use the default value.
                else
                {
                    value = defaultValue;
                }
            }

            return value;
        }

        public bool AddOrUpdateValue<T>(string key, T value = default(T), bool roaming = false)
        {
            bool valueChanged = false;

            lock (_locker)
            {
                // If the key exists
                if (Settings.Contains(key))
                {

                    // If the value has changed
                    if (!Settings[key].Equals(value))
                    {
                        // Store key new value
                        Settings[key] = value;
                        valueChanged = true;
                    }
                }
                // Otherwise create the key.
                else
                {
                    Settings.Add(key, value);
                    valueChanged = true;
                }
            }

            if (valueChanged)
            {
                lock (_locker)
                {
                    Settings.Save();
                }
            }

            return valueChanged;
        }

        public bool DeleteValue(string key, bool roaming = false)
        {
            if (!Settings.Contains(key))
            {
                return false;
            }

            Settings.Remove(key);
            Settings.Save();
            return true;
        }

        public bool Contains(string key, bool roaming = false)
        {
            return Settings.Contains(key);
        }

        public bool ClearAllValues(bool roaming = false)
        {
            Settings.Clear();
            Settings.Save();
            return true;
        }

        public string GetSecuredValue(string key)
        {
            lock (_locker)
            {
                try
                {
                    var protectedValueByte = ReadValueFromFile(key);
                    var valueByte = ProtectedData.Unprotect(protectedValueByte, null);
                    return Encoding.UTF8.GetString(valueByte, 0, valueByte.Length);
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
                var valueByte = Encoding.UTF8.GetBytes(value);
                var protectedValueByte = ProtectedData.Protect(valueByte, null);
                WriteValueToFile(key, protectedValueByte);
            }
        }

        public void RemoveSecuredValue(string key)
        {
            lock (_locker)
            {
                var file = IsolatedStorageFile.GetUserStoreForApplication();
                file.DeleteFile(key);
            }
        }

        private void WriteValueToFile(string key, byte[] protectedValueByte)
        {
            var file = IsolatedStorageFile.GetUserStoreForApplication();

            using (var isolatedStorageFileStream = new IsolatedStorageFileStream(key, FileMode.Create, FileAccess.Write, file))
            {
                using (var writer = new StreamWriter(isolatedStorageFileStream).BaseStream)
                {
                    writer.Write(protectedValueByte, 0, protectedValueByte.Length);
                }
            }
        }

        private byte[] ReadValueFromFile(string key)
        {
            var file = IsolatedStorageFile.GetUserStoreForApplication();

            using (var isolatedStorageFileStream = new IsolatedStorageFileStream(key, FileMode.Open, FileAccess.Read, file))
            {
                using (var reader = new StreamReader(isolatedStorageFileStream).BaseStream)
                {
                    var valueArray = new byte[reader.Length];
                    reader.Read(valueArray, 0, valueArray.Length);
                    return valueArray;
                }
            }
        }
    }
}