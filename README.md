#BlackBerry Native Development plugin for Visual Studio

This project is a full-featured package for Visual Studio enabling BlackBerry native (C/C++/Cascades) development. It configures your favorite IDE in few clicks and stays always up-to-date as installed via [VS Gallery](https://visualstudiogallery.msdn.microsoft.com/site/search?query=blackberry%20qnx&f%5B0%5D.Value=blackberry%20qnx&f%5B0%5D.Type=SearchText&ac=4).

It was once a project owned by BlackBerry itself. However since it was so impressively improved, BlackBerry made an official release [Gold 3.0](http://devblog.blackberry.com/2015/03/more-gold-blackberry-native-plug-in-for-microsoft-visual-studio/) purely basing on my contribution.

##License
All assets in this repository, unless otherwise stated through sub-directory LICENSE or NOTICE files, are subject to the Apache Software License v2.0.

#Key Features

Please visit the [Wiki](https://github.com/phofman/vs-plugin/wiki) for detailed documentation.

###Supported platforms
 * all known and modern BlackBerry devices (BB10 phones and PlayBook) and simulators
 * integrates inside Visual Studio 2010/2012/2013/2015 including Community Edition
 * works on Windows XP/7/8/10

###Delivered via Visual Studio Gallery
There are following releases available for installation:
* for [Visual Studio 2010](https://visualstudiogallery.msdn.microsoft.com/91f71ea5-32eb-41f0-8229-cba59c1bfdca)
* for [Visual Studio 2012](https://visualstudiogallery.msdn.microsoft.com/de47fd53-e7ae-4bd6-a59a-73a6bf8efae3)
* for [Visual Studio 2013](https://visualstudiogallery.msdn.microsoft.com/9a03ae7e-b786-41a7-a63b-dc0d9fe818d2)
* for [Visual Studio 2015](https://visualstudiogallery.msdn.microsoft.com/2b21e9d1-a54b-4d58-878f-2d53604e85b1)

###New Project and New Project-Item
![New Project](/media/info/feature-new-project.png)

![New Project Item](/media/info/feature-new-project-item.png)

The easiest and most natural way of starting development for new platform. Templates for new BlackBerry C/C++ projects are already there and waiting for the job to start.


###Automatic MSBuild update
![Auto MSBuild update](/media/info/feature-auto-msbuild.png)

Validates build environment and might suggest updating MSBuild on fresh install, or when plugin gets updated in the future. No manual copying files around!

###GDB Integration
![GDB](/media/info/feature-gdb.png)

Set a breakpoint, hit F5 and observe, how the application is build, uploaded to the device and stops at requested location. It contains full-blown GDB integration with:
 * breaking at location
 * stepping through the code
 * local variables preview
 * and much more!

###IntelliSense Support
![IntelliSense](/media/info/feature-intellisense.png)

The original headers from active BlackBerry NDK are processed and IntelliSense can serve descriptions to classes and methods, while coding. It works great for C/C++ files, QML is not supported yet.

###QML colorizing Support
![QML Colorizer](/media/info/feature-qml.png)

Not leaving QML too far. There is a basic QML colorizer build-in, to mark words as known types, numbers, strings or comments, while coding.

###Device Browsing Support
![Target Navigator](/media/info/feature-target-navigator.png)

It's possible to navigate over file system remotely, copying files to/from the device and even kill processes, if developing headless applications or cards.

###Settings
![Project Options](/media/info/feature-project-settings.png)

![Global Options](/media/info/feature-options.png)

And almost all aspect of the project can be changed or tuned. Also if you dislike Momentics too, you can directly inside Visual Studio:
 * register as BlackBerry developer
 * download latest BlackBerry NDK
 * generate debug tokens and upload them onto devices
 * and more!

## Reference materials
* [How to Contribute](https://github.com/phofman/vs-plugin/wiki/Contribution)
* Who is behind the project - [CodeTitans.pl](http://www.codetitans.pl)
