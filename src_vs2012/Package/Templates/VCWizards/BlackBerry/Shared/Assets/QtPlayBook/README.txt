
This is a prototype of Qt4 application running on PlayBook and BB10 devices.

Starting from NDK 2.1+ and continuing in NDK 10.0+ BlackBerry delivers compiled set of Qt libraries
and matching headers. Unfortunately those libraries don't load at all on the device and must be deployed in bar file.

Please install following NuGet packages (NuGet 2.6+ required) to download respective Qt binaries and headers.
The locations of dependant libraries are automatically placed inside bar-descriptor.xml.

Qt-4.8.3 (last one with PlayBook supported):
 * Install-Package codetitans-playbook-qt4-core
 * Install-Package codetitans-playbook-qt4-qml
 * Install-Package codetitans-playbook-qt4-labs

The last one might be required mostly for particles effects in game samples.


More info can be found at: https://github.com/phofman/vs-plugin/wiki/Qt-Project-for-PlayBook

===============
CodeTitans 2015
