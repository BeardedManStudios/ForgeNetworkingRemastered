# Server Disconnect Client Code
Sometimes it is necessary to manually disconnect a player from the server (on the server). First you will need to find out which NetworkingPlayer you are wishing to kick on the server. Once you have a reference to that player, you are going to want to call the disconnect on them. To do this you need to use the reference to your server's `NetWorker`.

```csharp
// We are going to force the disconnect of the client
bool forcefully = true;

// Consider the player you want to disconnect is the targetPlayer object
// Also consider that serverNetworker is the server's networker (this can be NetworkManager.Instance.Networker)
((IServer)serverNetworker).Disconnect(targetPlayer, forcefully);
```

That is it, you are ready to start kicking all of the players! :D