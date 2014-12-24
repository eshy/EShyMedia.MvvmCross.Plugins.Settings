#Settings Plug-In for MvvmCross
====================================

This plug-in is based on 

The settings plug-ins:
https://github.com/jamesmontemagno/Mvx.Plugins.Settings
https://github.com/Cheesebaron/Cheesebaron.MvxPlugins

The secured storage plug-in:
https://github.com/ChristianRuiz/MvvmCross-SecureStorage

##Getting Started

###Install from NuGet

```
Install-Package EShyMedia.MvvmCross.Plugins.Settings
```

###Save/Get settings

Use the Settings class generated in the Core project under Helpers to get started.


Use GetValueOrDefault and AddOrUpdateValue to get/save settings.

Use GetSecuredValue and AddOrUpdateSecuredValue to get/save secured strings.


On XAML apps (Windows/Windows Phone 8.1) you can also save roaming settings.

##iOS 
This plug-in supports Xamarin iOS Unified API only (classic API doesn't support Apple's 64-bit requirement)
