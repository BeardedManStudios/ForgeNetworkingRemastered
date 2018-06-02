# Client Connecting Code
Before you can take full advantage of setting up a client, you will probably want to setup [server hosting code](server-hosting-code) so that the client has something to actually connect to. Below is a sample of how to start a UDP Client and a TCP Client.

#### UDP Client
```csharp
string hostAddress = "127.0.0.1";
ushort port = 15937;
UDPClient client = new UDPClient();
client.Connect(hostAddress, port);
```

#### TCP Client
```csharp
TCPClient client = new TCPClient();
client.Connect(hostAddress, port);
```

## Assigning to Network Manager
**Hard to believe?** Yes that is all the code you need to start up a client, now there is one more important piece of business we must complete. Since this client is most likely going to be the main communication `NetWorker` with the server and the game, you are probably going to want to set it up as the default `NetWorker` for the `NetworkManager` to use. You can do this with the following:

```csharp
// The client object is the same object from the above code samples
NetworkManager.Instance.Initialize(client);
```

By doing this, the passed in client will be the `NetWorker` that is used for all communication managed by the `NetworkManager`. This includes spawning objects, RPC calls on those objects, scene transitions and so forth.
