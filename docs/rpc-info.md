# RpcInfo

If you have done any RPC calls by now, then you will notice that they all are supplied 1 argument being **RpcArgs**. Inside of this RpcArgs you will find an object named **Info** which is of type **RpcInfo**. Basically, what this contains is the various information about the RPC being called. At the time of this writing there are 2 variables provided, the **TimeStep** and the **SendingPlayer**. The TimeStep is of ulong type and is useful for determining at what timestep (millisecond) when the RPC was called on the sender which is often useful in lockstep systems or detecting cheating. The **SendingPlayer** is of type NetworkingPlayer and is most useful on the server. For clients, this variable will always be the server since Forge Networking follows a completely Authoritative Design in how it communicates. For the server the **SendingPlayer** will be the client who sent the RPC.

Example:  Let's say that the client wanted to assign its name on the server and so it sends a string via RPC to request the name assignment. Using the **SendingPlayer** from the RpcInfo we can easily get the player object and assign the name.

### Using RpcInfo
```csharp
// Client calls this code somewhere
networkObject.SendRpc("AssignName", Receivers.Server, "My New Name");

// The Rpc would look like the following
public override void AssignName(RpcArgs args)
{
	if (!networkObject.IsServer)
		return;

	string playerName = args.GetNext<string>();
	RPCInfo info = args.Info;
	info.SendingPlayer.Name = playerName;
}```