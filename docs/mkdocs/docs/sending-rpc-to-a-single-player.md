# Sending RPC to a Single Player
During your development process you may find that you have to send an RPC to a single player and nobody else. This is especially useful to target players that are just connecting to the server to send them needed data that is not buffered already.

### General Example
```csharp
// targetPlayer is a NetworkingPlayer refence we got from the NetWorker::Players list or anywhere else
// args is an object array

networkObject.SendRpc(targetPlayer, RPC_RPC_NAME, args);
```

### Player Accept (join) Example
```csharp
// Somewhere in your code

if (networker.IsServer)
    networker.playerAccepted += PlayerAcceptedSetup;

// ...

private void PlayerAcceptedSetup(NetworkingPlayer newPlayer)
{
    networkObject.SendRpc(newPlayer, RPC_RPC_NAME, args);
}
```

## Notes
This player targeting with RPCs are done on the server. Clients do not have direct access to each other since all network traffic goes through the server (which makes it [authoritative](authoritative-design)). You can use Receivers.Owner to target the owning player of the network object from any client though.