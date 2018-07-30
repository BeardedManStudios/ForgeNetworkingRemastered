# Server Hosting Code
So you may have looked at the `MultiplayerMenu.cs` file and noticed there is a lot of amazing useful code samples there, but sometimes all that code can be a bit overwhelming and you just want to know **How do I start a server?**. Below is a sample of how to start a UDP Server and a TCP Server.

#### UDP Server
```csharp
int maxAllowedClients = 32;
UDPServer server = new UDPServer(maxAllowedClients);
server.Connect();
```

#### TCP Server
```csharp
int maxAllowedClients = 32;
TCPServer server = new TCPServer(maxAllowedClients);
server.Connect();
```

It is worth noting that both `UDPServer` and `TCPServer` implement the `IServer` interface and derive from the `NetWorker`. So there are many casting and generic options to choose from. There are arguments that go into the `Connect` methods of these types, but they are for the [Master Server](/MasterServer/quick-start.md) and [Nat Server](/nat-hole-punching.md).

## Assigning to Network Manager
**Hard to believe?**, Yes that is all the code you need to start up a server, now there is one more important piece of business we must complete. Since this server is most likely going to be the main communication `NetWorker` with the clients and the game, you are probably going to want to set it up as the default `NetWorker` for the `NetworkManager` to use. You can do this with the following:

```csharp
// The server object is the same object from the above code samples
NetworkManager.Instance.Initialize(server);
```

By doing this, the passed in server will be the `NetWorker` that is used for all communication managed by the `NetworkManager`. This includes spawning objects, RPC calls on those objects, scene transitions and so forth.
