# Temporarily Blocking Connections
Sometimes, when a match starts, you don't want people being able to join the server, even when other players leave. You are able to tell the server to no longer accept connections as seen below:

```csharp
NetWorker server = new UdpServer(32);
server.Connect();

// ...

// Don't allow any more connections to this server, but don't kick any current connections
((IServer)server).StopAcceptingConnections();
```

Obviously we needed to give you a way to be able to start accepting connections again otherwise that would be kinda rude. So we wen't ahead and made the following function for you to begin accepting connections again. *Note that this is a continuation of the above example*

```csharp
// Start allowing for connections to this server again
((IServer)server).StartAcceptingConnections();
```

Lastly, you may have forgotten or don't know if you stopped allowing connections, so we made this handy boolean for you to check against! *Note that this is a continuation of the above example*

```csharp
if (!((IServer)server).AcceptingConnections)
{
    // Connections are currently not allowed for this server
}
```
