// Helpers/Settings.cs
using MvvmCross.Platform;
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

        public static string GeneralSetting
        {
            get { return AppSettings.GetValueOrDefault(nameof(GeneralSetting), "DefaultValue"); }
            set { AppSettings.AddOrUpdateValue(nameof(GeneralSetting), value); }
        }
    }
}