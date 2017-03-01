# How to Make "Offline" Mode
Something you may be curious about is how you can make your game both single player and multi-player. Your main problem is probably going to be trying to figure out how you can separate all your RPC calls and your offline direct calls. The thing that may suprise you is if we were to say that there is no need for the distinction between "Online" and "Offline" code.

## The Solo Server
So if you think about it, single player could very well be a server that has the following attributes:
1. Does not register with a master server
2. Does not register with a nat traversal server
3. Does not accept any connections (max players is 0)
4. Blocks any other connection attempts

Well this makes sense so far, if a server doesn't accept any clients then technically it is nothing more than an offline game. However, what about the RPC calls and the vairious other networked things like the networkObject? Well, the answer is simple, Forge Networking was built to where the server isolation is offline play. If there are no connections to the server, it will call all RPCs directly without any network layer involved for sending (it doesn't send to itself, just runs the method locally). Below is an example of a server put into an **offline** like state:

```csharp
// Zero connections allowed to this server
NetWorker server = new UdpServer(0);
server.Connect();

// Block all connections to skip the validation phase
((IServer)server).StopAcceptingConnections();
```

## Notes
When you specify receivers that do not include the server, such as `Receivers.Others` then the server will not call the method, even with no other connections.