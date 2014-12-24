using System;
using System.Globalization;
using Foundation;
using Security;

namespace EShyMedia.MvvmCross.Plugins.Settings.Touch
{
    public class MvxTouchSettings : ISettings
    {
        private readonly object locker = new object();

        public T GetValueOrDefault<T>(string key, T defaultValue = default(T), bool roaming = false)
        {
            lock (locker)
            {
                if (NSUserDefaults.StandardUserDefaults[key] == null)
                    return defaultValue;

                Type typeOf = typeof(T);
                if (typeOf.IsGenericType && typeOf.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }
                object value = null;
                var typeCode = Type.GetTypeCode(typeOf);
                var defaults = NSUserDefaults.StandardUserDefaults;
                switch (typeCode)
                {
                    case TypeCode.Decimal:
                        var savedDecimal = defaults.StringForKey(key);
                        value = Convert.ToDecimal(savedDecimal, CultureInfo.InvariantCulture);
                        break;
                    case TypeCode.Boolean:
                        value = defaults.BoolForKey(key);
                        break;
                    case TypeCode.Int64:
                        var savedInt64 = defaults.StringForKey(key);
                        value = Convert.ToInt64(savedInt64, CultureInfo.InvariantCulture);
                        break;
                    case TypeCode.Double:
                        value = defaults.DoubleForKey(key);
                        break;
                    case TypeCode.String:
                        value = defaults.StringForKey(key);
                        break;
                    case TypeCode.Int32:
                        value = (Int32)defaults.IntForKey(key);
                        break;
                    case TypeCode.Single:
                        value = (float)defaults.FloatForKey(key);

                        break;

                    case TypeCode.DateTime:
                        var savedTime = defaults.StringForKey(key);
                        var ticks = string.IsNullOrWhiteSpace(savedTime) ? -1 : Convert.ToInt64(savedTime, CultureInfo.InvariantCulture);
                        if (ticks == -1)
                            value = defaultValue;
                        else
                            value = new DateTime(ticks);
                        break;
                    default:

                        if (defaultValue is Guid)
                        {
                            var outGuid = Guid.Empty;
                            var savedGuid = defaults.StringForKey(key);
                            if (string.IsNullOrWhiteSpace(savedGuid))
                            {
                                value = outGuid;
                            }
                            else
                            {
                                Guid.TryParse(savedGuid, out outGuid);
                                value = outGuid;
                            }
                        }
                        else
                        {
                            throw new ArgumentException(string.Format("Value of type {0} is not supported.", value.GetType().Name));
                        }

                        break;
                }


                return null != value ? (T)value : defaultValue;
            }
        }

        public bool AddOrUpdateValue<T>(string key, T value = default(T), bool roaming = false)
        {
            lock (locker)
            {
                var typeOf = value.GetType();
                if (typeOf.IsGenericType && typeOf.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }
                var typeCode = Type.GetTypeCode(typeOf);
                var defaults = NSUserDefaults.StandardUserDefaults;
                switch (typeCode)
                {
                    case TypeCode.Decimal:
                        defaults.SetString(Convert.ToString(value), key);
                        break;
                    case TypeCode.Boolean:
                        defaults.SetBool(Convert.ToBoolean(value), key);
                        break;
                    case TypeCode.Int64:
                        defaults.SetString(Convert.ToString(value), key);
                        break;
                    case TypeCode.Double:
                        defaults.SetDouble(Convert.ToDouble(value), key);
                        break;
                    case TypeCode.String:
                        defaults.SetString(Convert.ToString(value), key);
                        break;
                    case TypeCode.Int32:
                        defaults.SetInt(Convert.ToInt32(value), key);
                        break;
                    case TypeCode.Single:
                        defaults.SetFloat(Convert.ToSingle(value), key);
                        break;
                    case TypeCode.DateTime:
                        defaults.SetString(Convert.ToString(((DateTime)(object)value).Ticks), key);
                        break;
                    default:
                        throw new ArgumentException(string.Format("Value of type {0} is not supported.",
                            value.GetType().Name));
                }
                try
                {
                    defaults.Synchronize();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
                }
            }


            return true;
        }

        public bool DeleteValue(string key, bool roaming = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key must have a value", "key");
            }

            var defaults = NSUserDefaults.StandardUserDefaults;
            defaults.RemoveObject(key);
            return defaults.Synchronize();
        }

        public bool Contains(string key, bool roaming = false)
        {
            var defaults = NSUserDefaults.StandardUserDefaults;
            try
            {
                var stuff = defaults[key];
                return stuff != null;
            }
            catch
            {
                return false;
            }
        }

        public bool ClearAllValues(bool roaming = false)
        {
            var defaults = NSUserDefaults.StandardUserDefaults;
            defaults.RemovePersistentDomain(NSBundle.MainBundle.BundleIdentifier);
            return defaults.Synchronize();
        }

        public string GetSecuredValue(string key)
        {
            lock (locker)
            {
                var existingRecord = new SecRecord(SecKind.GenericPassword)
                {
                    Account = key,
                    Label = key,
                    Server = NSBundle.MainBundle.BundleIdentifier
                };

                // Locate the entry in the keychain, using the label, service and account information.
                // The result code will tell us the outcome of the operation.
                SecStatusCode resultCode;
                SecKeyChain.QueryAsRecord(existingRecord, out resultCode);

                return resultCode == SecStatusCode.Success ? NSString.FromData(existingRecord.ValueData, NSStringEncoding.UTF8) : null;
            }
        }

        public void AddOrUpdateSecuredValue(string key, string value)
        {
            lock (locker)
            {
                SecKeyChain.Add(new SecRecord(SecKind.GenericPassword)
                {
                    Service = NSBundle.MainBundle.BundleIdentifier,
                    Account = key,
                    ValueData = NSData.FromString(value, NSStringEncoding.UTF8)
                });
            }
        }

        public void RemoveSecuredValue(string key)
        {
            lock (locker)
            {
                var existingRecord = new SecRecord(SecKind.GenericPassword)
                {
                    Account = key,
                    Label = key,
                    Server = NSBundle.MainBundle.BundleIdentifier
                };

                SecKeyChain.Remove(existingRecord);
            }
        }
    }
}