# Changing Ownership

There are two different methods to use for changing ownership of an object; one for the client and one for the server. The client method is **TakeOwnership** and has no arguments. Any client can call this method on the **networkObject** to request ownership of an object. The method that you would use to force ownership from the server is **AssignOwnership** which has an argument **NetworkingPlayer** (the player who is going to be the new owner for the network object). Only the server can call this method on the **networkObject** due to it's authoritative nature. If a client were to call this method, nothing would happen.

As for verifying an ownership change request, which if you don't do it will just accept the change ownership request, there is one method to override. We live in a world where we just can't trust our clients to send us messages that they should be in the way that we wish them to. Clients may want to cheat the system and try to exploit the network for fun or just so that they can win. Because of this we have created a simple method for you to override in order to check any game critical RPCs that are received from the client and verify them. Before continuing with this part of the documentation, please review the Extending Generated Classes documentation to learn more about **partial** classes.

When we use the Network Contract Wizard (NCW), it will create 2 generated classes; one for our MonoBehaviour and one for our NetworkObject.

The one that we want to focus on in this example is the generated NetworkObject. So let's say that we opened up the Network Contract Wizard (NCW) and we created a contract named **Car** ; this will generate a NetworkObject class named **CarNetworkObject**. Now in another folder (not in the Generated folder) you will create a new C# script called **CarNetworkObject**. In here you will create your [partial class](https://msdn.microsoft.com/en-us/library/wa80x488.aspx) for the **CarNetworkObject** and you will override the **AllowOwnershipChange** method like so:

### AllowOwnershipChange
```csharp
protected override bool AllowOwnershipChange(NetworkingPlayer newOwner)
{
	// The newOwner is the NetworkingPlayer that is requesting the ownership change, you can get the current owner with just "Owner"
}
```

Notice that you need to return a boolean from this function. If you return **true** then the ownership change will be allowed to go through, it will be invoked on the server and on the clients. If you return **false** then the ownership change will not be invoked at all and will be dropped.
