﻿using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

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
