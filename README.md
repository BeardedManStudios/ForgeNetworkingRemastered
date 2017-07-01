# Forge Networking Remastered
In short, Forge Networking is a free and open source multiplayer game (multi-user) networking system that has a very good integration with the Unity game engine. You wanna make a multiplayer game or real time multi-user application? This is the library for you.

Welcome to the most innovative networking solution (which works with the Unity game engine). Forge's network bandwith is un-beatable, it's performance out classes other solutions, it's flexibility is in a league of it's own and of course, as always, no CCU limits ever. Forge Networking is an open source networking solution built in C#. It is completely multi-threaded and was designed to work both inside and outside of the Unity Game Engine but mainly in conjunction with Unity. To get started, check out the links listed below in this readme.

## Possibilities
- Unity multiplayer games/applications
- Unity independent applications
- User hosted servers
- Run servers on Windows, OSX, Raspberry Pi, and/or Linux
- Developer hosted servers
- MMO, RTS, FPS, MOBA, you name it, Forge does it
- Master Servers, NAT Hole punching, Web Servers, Cache servers, all the servers!
- Server to server communication, yup
- Spin up servers on demand, check
- Tons of other stuff that I won't list otherwise I will go on forever, yes indeed

If you can name it, Forge most likely can do it :), it is built on some basic principals which makes any idea a possibility.

## TLDR
Forge Networking is a networking solution built with the Unity game engine in mind. However, this is actually the 2nd version of Forge so it has some interesting properties. The first original Forge (classic) was built directly inside of Unity and was very tightly integrated with Unity. Once we learned everything we could from that version we opened up a blank Visual Studio project and began work on Forge Networking remastered. Forge Networking Remastered was completely developed independantly of Unity, it was tested and debugged in a C# project. Once it was working, we made it so that Unity could use it. This means that you can easily create native C# applications which run on Forge to support your games and applications such as Relay servers, NAT hole punching servers, chat servers, master servers, cache servers, websocket servers, you name it! Forge was developed in a way that makes it easy for you to do serialization of data in any way that you want. This allows you to make the security of your project as secure as you want or as fast as you need.

[Original Unity Forum Thread](https://forum.unity3d.com/threads/no-ccu-limit-forge-networking-superpowered-fully-cross-platform.286900/)

## Installation
Unity Package:
1) Download the unitypackage
2) Open the package in Unity
3) Done, you are ready to start working with doc examples

Git Source Code:
1) Clone the source
2) Open the `BeardedManStudios/BeardedManStudios.sln`
3) Build it (See note below if you get errors)
4) Open `Forge Networking Remastered Unity` in Unity
5) Right click on `Bearded Man Studios Inc` and export a unity package
6) Follow the steps above listed under `Unity Package:` starting with step #2

**Note** Sometimes when you try to build from GitHub you will get an error about missing references. To fix this you need to open the Unity project for Forge Networking. Then open any script inside your IDE by double click it. This will force Unity to update the `.csproj` file for the Unity project. After this you can close that instance of the IDE and return to the `BeardedManStudios.sln` and continue back at step 3 above.

## Community
[Official Documentation](http://docs.forgepowered.com/)

[Wiki](https://github.com/BeardedManStudios/ForgeNetworkingRemastered/wiki)

[Discord](https://discord.gg/yzZwEYm) - Join us and the growing community, for talking about Forge Networking as well as just networking in general. Even if you don't exactly use Forge Networking in your project you can get a ton of insight from this community :)

[Generated API](https://forgepowered.com/ForgeNetworkingRemasteredAPI/html/index.html)

[YouTube Jumpstart Videos](https://www.youtube.com/playlist?list=PLm1w78-UUlMIi5Vfwy6ckJQIQMHMT-QS5)

[Getting Started Docs](http://docs.forgepowered.com/getting-started/)

## Unity Package Download
Most Used: [Version 21.5](https://e71dac46a75cac973b88-8a2ab8f09d41afeb61265f61aa50339b.ssl.cf1.rackcdn.com/ForgeNetworking-Release-V21.5.unitypackage)

Most Recent: [Version 23.02](https://e71dac46a75cac973b88-8a2ab8f09d41afeb61265f61aa50339b.ssl.cf1.rackcdn.com/ForgeNetworking-Release-V23.02.unitypackage)

## Ways to support the project
1) Make pull requests :D
2) Any donations are much appreciated <3

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4CXPTUZR3KBLU)

## License (MIT)
Copyright 2017 Bearded Man Studios, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
