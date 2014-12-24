using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.Preferences;

namespace EShyMedia.MvvmCross.Plugins.Settings.Droid
{
    public class MvxAndroidSettings : ISettings
    {
        private static ISharedPreferences SharedPreferences { get; set; }

        private static ISharedPreferencesEditor SharedPreferencesEditor { get; set; }

        private static ISharedPreferences SecuredPreferences { get; set; }

        private static ISharedPreferencesEditor SecuredPreferencesEditor { get; set; }

        private readonly object locker = new object();

        public MvxAndroidSettings()
        {
            SharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            SharedPreferencesEditor = SharedPreferences.Edit();

            SecuredPreferences = Application.Context.GetSharedPreferences(Application.Context.PackageName+ ".SecureStorage",
                FileCreationMode.Private);
            SecuredPreferencesEditor = SecuredPreferences.Edit();
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default(T), bool roaming = false)
        {
            lock (locker)
            {
                Type typeOf = typeof (T);
                if (typeOf.IsGenericType && typeOf.GetGenericTypeDefinition() == typeof (Nullable<>))
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }


                object value = null;
                var typeCode = Type.GetTypeCode(typeOf);
                switch (typeCode)
                {
                    case TypeCode.Decimal:
                        value =
                            (decimal)
                                SharedPreferences.GetLong(key,
                                    (long)
                                        Convert.ToDecimal(defaultValue,
                                            CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Boolean:
                        value = SharedPreferences.GetBoolean(key, Convert.ToBoolean(defaultValue));
                        break;
                    case TypeCode.Int64:
                        value =
                            (Int64)
                                SharedPreferences.GetLong(key,
                                    (long)
                                        Convert.ToInt64(defaultValue, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.String:
                        value = SharedPreferences.GetString(key, Convert.ToString(defaultValue));
                        break;
                    case TypeCode.Double:
                        value =
                            (double)
                                SharedPreferences.GetLong(key,
                                    (long)
                                        Convert.ToDouble(defaultValue, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Int32:
                        value = SharedPreferences.GetInt(key,
                            Convert.ToInt32(defaultValue, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Single:
                        value = SharedPreferences.GetFloat(key,
                            Convert.ToSingle(defaultValue, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.DateTime:
                        var ticks = SharedPreferences.GetLong(key, -1);
                        if (ticks == -1)
                            value = defaultValue;
                        else
                            value = new DateTime(ticks);
                        break;
                    default:

                        if (defaultValue is Guid)
                        {
                            var outGuid = Guid.Empty;
                            Guid.TryParse(SharedPreferences.GetString(key, Guid.Empty.ToString()), out outGuid);
                            value = outGuid;
                        }
                        else
                        {
                            throw new ArgumentException(string.Format("Value of type {0} is not supported.",
                                value.GetType().Name));
                        }

                        break;
                }



                return null != value ? (T) value : defaultValue;
            }
        }

        public bool AddOrUpdateValue<T>(string key, T value = default(T), bool roaming = false)
        {
            lock (locker)
            {
                Type typeOf = value.GetType();
                if (typeOf.IsGenericType && typeOf.GetGenericTypeDefinition() == typeof (Nullable<>))
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }
                var typeCode = Type.GetTypeCode(typeOf);
                switch (typeCode)
                {
                    case TypeCode.Decimal:
                        SharedPreferencesEditor.PutLong(key,
                            (long) Convert.ToDecimal(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Boolean:
                        SharedPreferencesEditor.PutBoolean(key, Convert.ToBoolean(value));
                        break;
                    case TypeCode.Int64:
                        SharedPreferencesEditor.PutLong(key,
                            (long) Convert.ToInt64(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.String:
                        SharedPreferencesEditor.PutString(key, Convert.ToString(value));
                        break;
                    case TypeCode.Double:
                        SharedPreferencesEditor.PutLong(key,
                            (long) Convert.ToDouble(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Int32:
                        SharedPreferencesEditor.PutInt(key,
                            Convert.ToInt32(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Single:
                        SharedPreferencesEditor.PutFloat(key,
                            Convert.ToSingle(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.DateTime:
                        SharedPreferencesEditor.PutLong(key, ((DateTime) (object) value).Ticks);
                        break;
                    default:
                        throw new ArgumentException(string.Format("Value of type {0} is not supported.",
                            value.GetType().Name));
                }
            }

            lock (locker)
            {
                SharedPreferencesEditor.Commit();
            }
            return true;
        }

        public bool DeleteValue(string key, bool roaming = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key must have a value", "key");
            }
            SharedPreferencesEditor.Remove(key);
            return SharedPreferencesEditor.Commit();
        }

        public bool Contains(string key, bool roaming = false)
        {
            return SharedPreferences.Contains(key);
        }

        public bool ClearAllValues(bool roaming = false)
        {
            SharedPreferencesEditor.Clear();
            return SharedPreferencesEditor.Commit();
        }

        public string GetSecuredValue(string key)
        {
            lock (locker)
            {
                try
                {
                    return SecuredPreferences.GetString(key, null);
                }
                catch
                {
                    return null;
                }
            }
        }

        public void AddOrUpdateSecuredValue(string key, string value)
        {
            lock (locker)
            {
                SecuredPreferencesEditor.PutString(key, value);
                SecuredPreferencesEditor.Commit();
            }
        }

        public void RemoveSecuredValue(string key)
        {
            if (SecuredPreferences.Contains(key))
            {
                lock (locker)
                {
                    SecuredPreferencesEditor.Remove(key);
                    SecuredPreferencesEditor.Commit();
                }
            }
        }
    }
}