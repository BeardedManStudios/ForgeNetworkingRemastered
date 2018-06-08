# Troubleshooting

## Network Issues
If you are having trouble setting up your network, or you are having trouble connecting your game, please review this page and follow the steps completely. There are many stages of network communication that you can troubleshoot to test your game. In this document we will go over the various steps to troubleshooting so that you can figure out what could be wrong with the setup on your machine, local network, or internet connections.

### The Local Machine
The primary method of testing your game will most likely be on a single machine which is often your development machine. The addresses `127.0.0.1`, `localhost`, `::0`, etc. are all references to the machine you are currently on. These addresses are used for local testing, routing, security, and other probably less common practices.

Steps to test:
1. Turn on "Run in Background" in Unity Player Settings
2. Set the Multiplayer Menu scene as the first scene
3. Use the CubeForge demo scene as the second scene
4. Build your game from Unity
5. Run 1 instance of your build and play the other instance in the Unity Editor
6. Press play in the Editor
7. Host a server in the Editor on port **15937**
8. Run the built instance of the application
9. Connect to IP address **127.0.0.1** and port number **15937** on the built instance

If you can connect, **CONGRATULATIONS**, your game is properly networked and your machine is properly setup to test your game on the local machine. If you are having trouble with this stage, please ensure that you are properly following the steps above and building the instance.

### The Local Area Network (LAN)
If the local machine setup above is working correctly and your local area network (LAN) is not, chances are there is an issue with your network or machine firewall. Follow the above steps except run the instance of the game that was built on another machine on your network. We will refer to the machine that the Unity Editor (server) is on as **Server** and the second machine that is on the network which is running the built instance **Client**.

1. Host the server on **Server**
2. Try to connect using **Client**

If the above steps fail to connect and play the game, you have 1 or more of the following problems with your network setup.

1. Your machine firewall on **Server** is not allowing the connections
2. Your machines are not on the same network
3. You are not supplying the correct IP address or Port number

####### Issue 1 Resolution
First, let's find out if your issue is the one listed as #1. Open your firewall settings on your local machine, locate the Unity Editor in the firewall settings and make sure it is allowing connections. If you are not sure how to check or change your firewall settings, please reference Google to learn the steps for your operating system.

**NOTE**: If you are not hosting in the Unity Editor, these same steps would apply to the build game, you just need to locate the game name in the firewall settings. If it is not found, you need to add the exception to your machine firewall with the same steps as mentioned above.

####### Issue 2 Resolution
For (#2) you are going to have to make sure that the two machines are on the same exact network. You can do this by making sure that the router name (when connecting) is the same for both machines (when connecting via Wi-Fi). If you are using a Ethernet cable, make sure that both cables for both machines are leading into the same router. If you are using Ethernet for one machine and Wi-Fi for another machine, make sure that the Wi-Fi connected machine is connecting to the same router name as the machine that has the Ethernet cord connected to the router.

####### Issue 3 Resolution
When connecting to **Server**, you are required to input the correct IP address. For the **Server** machine, you should be hosting on 0.0.0.0 for this test; if you are not sure what that means, then you are already set to that and do not need to worry about it as you've not set it explicitly yourself.

First, you need to open the network configuration on **Server**. You can do this by opening a `command prompt` or a `terminal` window. The following is the command that you would input onto a `Windows Command Prompt` window to find your local area networks IP address.
```
ipconfig
```

The following is the command that you would input onto a `MacOS/Linux Terminal` window to find your local area networks IP address.
```
ifconfig
```

Locate the LAN that you are connected to and find the IPv4 address that is assigned to the **Server**. This is the IP address that you need to input into your **Client** for the host address. The port number should still be **15937** as mentioned in the previous example.

**NOTE**: If you use machine virtualization (VM or Virtual Machine's) then there may be multiple network IPv4 addresses when you enter the above command. This is also the case if you using both Ethernet and Wi-Fi, so please be sure to select the correct network address. Usually the name showed in the listings is indicative to the connection type.

### The Internet & Cloud Servers
Chances are that if the above are working perfectly, then there are one of two things that could not be working for you.

1) The server machine's firewall is not opened for the application. Please see [the above](#The Local Machine) on how to fix this
or
2) The network firewall settings are not allowing the port

For #2 you will need to review with your cloud hosting provider on how to open the ports for your application/machine. This is done differently through each provider but should be an easy [Google Search](https://google.com/) away from finding out. Let's say that you are hosting through Microsoft Azure, you would search Google as follows:

```
Azure open network ports
```
or
```
Azure open ports and endpoints
```

These queries should produce some fine results. **MAKE SURE** to replace "Azure" with your cloud hosting provider, like "EC2" or "Rackspace".
