# User Manual (Forge Networking Remastered)

Welcome to Forge Networking Remastered, we would like to personally thank you for electing to become a part of our journey into Networking systems and Software as a whole. You are here because you have become a premium supporter of the Forge Networking project!

### Here for the [Code API](http://forgepowered.com/ForgeNetworkingRemasteredAPI/html/index.html)?

### Here for the [Video Tutorials](https://www.youtube.com/playlist?list=PLm1w78-UUlMIi5Vfwy6ckJQIQMHMT-QS5)?

## Forge Networking to Remastered Migration Guide

First, we would like to **thank you** so much for supporting Forge Networking over the past two years and we are proud to bring you **Forge Networking Remastered**!

Forge Networking Remastered (FNR) is a re-imagining of networking for Forge. We took everything we learned, all the feedback we could get and we started **from scratch** on FNR. We kept some of the helper classes such as **ObjectMapper** and **BMSByte** as those were optimized classes just for generic network data transfer. Secondly, we started FNR with the thought of **Web Sockets** and WebGL in mind. Though (at the time of this writing) WebGL support is not officially complete, we are working very hard to solidify this platform (mainly in relation to Unity integration and limitations). Thirdly we wanted to completely remove any kind of reflection "magic" that was in the system. In place of reflection we opted for generated class files and code, this makes debugging and tracking execution much easier to maintain and test.

Now for the reveal of the biggest change to Forge Networking, the Network Object. We have replaced the old singular, monolithic **inheritance** network model in place of an **attachment** network model. Now you can write your code as you wish without overly intrusive and complicated code/reflection and have a more solid extendable model. We have also included the new [Network Contract Wizard (NCW)](/NetworkContractWizard/network-contract-wizard-ncw.md) which allows you to easily blueprint and review your network contracts without having to dig through tons of code. We have completely re-imagined networking to be more of a service, attachment, addon, and extension rather than an overly complex integrated model.

Of course you will find many things that you once knew such as [Network Instantiation](/UnityIntegration/network-instantiation.md), [Remote Procedure Calls RPC](/NetworkObject/RemoteProcedureCalls/remote-procedure-calls.md), **MainThreadManager** ([Threading in Unity](/UnityIntegration/threading-in-unity.md)) however you will find that some of the older models (such as **NetSync** ) have been removed. I know, I know, **NetSync** is awesome and it works kinda like UNET's **SyncVar** ; however, we believe we have a much simpler, more controlled, less magical, and more powerful approach to serializing network variables. Since we have abstracted all the network code to be an attachment, you will find a new fancy **network Object** variable that is a part of the class you create. This object will house all of your network variables and allow you to set the sync time as well as access any variable from the network easily. This means you can **choose** when to use the network variable, choices are good, yes?

![smile](https://raw.githubusercontent.com/BeardedManStudios/ForgeNetworkingRemastered/develop/docs/mkdocs/docs/images/smile.png "Smile") Also, the networkObject is powerful, it automatically detects changes to your variable (as a whole) and serializes only that variable as opposed to the whole network class!

So, what are the difference highlights?

* [Instantiation](UnityIntegration/network-instantiation) is done through **NetworkManager.Instance.Instantiate...**
* [Remote procedure calls](NetworkObject/RemoteProcedureCalls/remote-procedure-calls) are done through **networkObject.SendRpc**
* NetSync has become **networkObject.[myVar]**
* PrimarySocket has become **NetworkManager.Instance.Networker** (You can also get the networker from the **networkObject** )
* WriteCustom has become **Binary** (Check out the VOIP module for how to use Binary) Networking and NetworkingManager have been replaced by **NetworkManager**
* So what are some of the new highlights?
* [Network Contract Wizard (NCW)](NetworkContractWizard/network-contract-wizard-ncw)
* Support for [NAT hole punching](nat-hole-punching)
* Master Server has Matchmaking
* "Cherry picked" network variable updating
* [State rewinding](rewinding)
* Network error logging system
* The Network Object

**Notes from Community**  
Do not put NetworkManager into your scene, it will be spawned on its own

Do not wrap your RPC calls in `MainThreadManger.Run` if you have the "Use Main Thread Manager For RPCs" enabled
