// Helpers/Settings.cs
using Cirrious.CrossCore;
using EShyMedia.MvvmCross.Plugins.Settings;

namespace $rootnamespace$.Helpers
{
    public static class Settings
    {
        /// <summary>
        /// Simply setup your settings once when it is initialized.
        /// </summary>
        private static ISettings m_Settings;
        private static ISettings AppSettings
        {
            get
            {
                return m_Settings ?? (m_Settings = Mvx.GetSingleton<ISettings>());
            }
        }

#region Setting Constants

		private const string SettingsKey = "settings_key";
		private static string SettingsDefault = string.Empty;

#endregion

	public static string GeneralSettings
        {
            get
            {
				return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }
            set
            {
                //if value has changed then save it!
				AppSettings.AddOrUpdateValue(SettingsKey, value);

            }
        }

    }

}