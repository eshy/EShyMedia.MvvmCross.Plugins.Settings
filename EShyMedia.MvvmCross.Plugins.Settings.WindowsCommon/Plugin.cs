using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace EShyMedia.MvvmCross.Plugins.Settings.WindowsCommon
{
public class Plugin : IMvxPlugin
    {

        public void Load()
        {
            Mvx.RegisterSingleton<ISettings>(new MvxWindowsCommonSettings());
        }
    }
}
