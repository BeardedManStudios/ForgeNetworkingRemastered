# Ban Client Code
Sometimes clients are just annoying and you want your hosts to be able to ban players from joining the server for a given amount of time (or what could be forever). To do this you need to use the reference to your server's `NetWorker`.

```csharp
// The amount of time in minutes that we want to ban the player
int minutesToBan = 60;  // 1 hour

// Consider the player you want to disconnect is the targetPlayer object
// We will want to get the players network id as we are going to do a generic ban
// Note:  If you have the network id from the RPC or any other messaging you can ban from there
ulong playerId = targetPlayer.NetworkId;

// Consider that serverNetworker is the server's networker (this can be NetworkManager.Instance.Networker)
((IServer)serverNetworker).BanPlayer(playerId, minutesToBan)
```