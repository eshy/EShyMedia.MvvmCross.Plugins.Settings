using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace EShyMedia.MvvmCross.Plugins.Settings.Touch
{
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.RegisterSingleton<ISettings>(new MvxTouchSettings());
        }
    }
}
