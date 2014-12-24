using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace EShyMedia.MvvmCross.Plugins.Settings.Droid
{
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.RegisterSingleton<ISettings>(new MvxAndroidSettings());
        }
    }
}
