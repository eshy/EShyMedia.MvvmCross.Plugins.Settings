using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;

namespace EShyMedia.MvvmCross.Plugins.Settings.WindowsPhone
{
    public class Plugin : IMvxPlugin
    {

        public void Load()
        {
            Mvx.RegisterSingleton<ISettings>(new MvxWindowsPhoneSettings());
        }
    }
}
