# Working With Multiple Sockets
Working in Forge Networking you may have found that it is extremely easy to setup a server and then setup a client to communicate with each other. Just in case you have forgotten, lets take a trip down memory lane:

```csharp
ushort myPort = 15937;
UDPServer myServer = new UDPServer(32); // Only allowing 32 connections to this server
myServer.Connect(port: myPort);
```

Looking above you might be thinking "wow, why use any other networking solution" and that is a very good question. Alright, so sure, it is easy to make a server... so how would you go about making 3 servers that run at the same time huh!? The answer is easy, let us do this through example below:

```csharp
UDPServer server1 = new UDPServer(32); // Only allowing 32 connections to this UDP server
server1.Connect();

ushort myPort = 15959;
TCPServer server2 = new TCPServer(364); // Only allowing 64 connections to this TCP server
server2.Connect(myPort);

UDPServer server3 = new UDPServer(1024); // Only allowing 1024 connections to this UDP server
server3.Connect(myPort + 1);
```

And just like that we have 3 servers **server1**, **server2**, and **server3**. These three servers are 3 different servers and manage their own players, network objects, rpcs, and pretty much everything else. Be warned, with great power comes great responsibility! You must always be aware of what networker is being communicated with and also know that the `NetworkManager` can only manage 1 NetWorker for the game. That means that the Instantiate methods and everything else that works with the network on the `NetworkManager` is for that "main" NetWorker. This means you need to do the work of disconnecting these servers when the application exits as well, otherwise the application will hang when the user tries to exit. You are also responsible for managing the references to these NetWorkers as well.

## Matching Players on Separate NetWorkers
What kind of guys would we be if we didn't tell you about the awesome `NetWorker::FindMatchingPlayer` method. Each NetWorker has a method `FindMatchingPlayer` that can be called to find players that match across the different NetWorkers. **Remember** that just because a player is id **22** on NetWorkerA doesn't mean it will be **22** on NetWorkerB.
