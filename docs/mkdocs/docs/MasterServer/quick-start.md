# Master Server Quick Start
The Master Server is a stand alone server that allows for games to be publicly (or privately with password) listed for the world to join. If you've ever played an online game with friends, chances are that you have had to get their IP address in order to join their hosted games. The Master Server gives your game the ability to have these fancy server listings.

## Getting the Master Server
To start with your Master Server adventures you must first purchase the Independent Master Server. **AHH!!** another purchase? Well yes, Forge Networking sells addons to help support the team to continue developing for Forge Networking. We've been around since 2014 so something has to be working with this model :). Speaking of which, **THANK YOU** sooooo much for supporting Forge Networking, it means a lot to us and the community!

Alright, where were we, ah yes; downloading the Master Server. The first thing you need to do is head over to your [Forge Networking developer profile](http://developers.forgepowered.com/Profile) and scroll down towards the bottom. You will see the addon section with **Unity Independent Master Server** listed right. Once you purchase this addon through PayPal you will immediately have it in your account for download.

## Running the Master Server
At this point we are assuming that you have already purchased the Master Server addon and you are looking to get it up and running. The very first thing you do on any operating system is to head over to your [developer profile](http://developers.forgepowered.com/Profile) and locate the download for the **Unity Independent Master Server**. Upon downloading this file (a .zip file) you will find that within the contents is a README file to get you started and the .exe file to be run.

### Windows
In windows it is very simple to host a Master Server. Just double click the `MasterServer.exe` and you are up and running!

### Other Platforms
On any platform other than Windows, as you may know, you can not run an **.exe** file. However with the power of Mono you will be abel to do such things without any extra coding needed. Please follow the instructions required to install mono onto your respective platform. After this you can open up a terminal window in the location where the **.exe** file was extracted and you can run the program by doing `mono MasterServer.exe`.

### Firewall
So in order for your MasterServer to be effective, you will need to allow the process through your machine firewall (see instructions on how to allow a process on your operating system through the firewall). Once you have allowed the process on your machine through your machine's firewall, it is time for you to allow the port to your machine on the network. Yes, there are technically 2 firewalls that you have to setup to allow an application to be visible to the outside world. Every router is different and so there are many different ways to allow port forwarding to your machine. Please view an online guide on how to set this up for your router make/model. The default port number for the master server is **15940**.

## Using the Master Server
So now that we have the master server up and running on our machine, it is time to give it a test and see what all is needed to use it. Lets begin by making sure that everything is working properly by running the built in testing method for Master Servers that come with Forge Networking.

1. Open your project with Forge Networking
2. Open the demo Multiplayer Menu scene
3. Make sure your build settings have a scene after the Multiplayer Menu scene in it
4. Locate the **Canvas** game object in the hierarchy and select it
5. Locate the **Master Server Host** and **Master Server Port** in the Inspector
![default-master-server-unity](https://raw.githubusercontent.com/BeardedManStudios/ForgeNetworkingRemastered/develop/docs/mkdocs/docs/images/default-master-server-unity.png)
6. Input the local or static IP address of the machine your Master Server is running on
7. Input **15940** as the port number for the Master Server
8. Click the play button in Unity
9. Click this **Host** button to start a game server

If you look at the Master Server command/terminal you should notice that a message appears with your game servers IP address and port number saying that it has been registered. At this point any clients can now connect to your Master Server and get the host listings. If the message did not appear then you've probably not setup the firewall settings correctly on either your machine or your router/network.
