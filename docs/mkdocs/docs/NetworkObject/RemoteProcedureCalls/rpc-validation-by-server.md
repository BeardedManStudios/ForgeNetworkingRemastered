# RPC Validation by Server

We live in a world where we just can't trust our clients to send us messages that they should be in the way that we wish them to. Clients may want to cheat the system and try to exploit the network for fun or just so that they can win. Because of this we have created a simple method for you to override in order to check any game critical RPCs that are received from the client and verify them. Before continuing with this part of the documentation, please review the Extending Generated Classes documentation to learn more about **partial** classes.

When we use the Network Contract Wizard (NCW), it will create 2 generated classes; one for our MonoBehaviour and one for our NetworkObject.

The one that we want to focus on in this example is the generated NetworkObject. So let's say that we opened up the Network Contract Wizard (NCW) and we created a contract named **Ball** ; this will generate a NetworkObject class named **BallNetworkObject**. Now in another folder (not in the Generated folder) you will create a new C# script called **BallNetworkObject**. In here you will create your [partial class](https://msdn.microsoft.com/en-us/library/wa80x488.aspx) for the **BallNetworkObj ect** and you will override the **ServerAllowRpc** method like so:

### ServerAllowRpc

```csharp
namespace BeardedManStudios.Forge.Networking.Generated
{
    public partial class BallNetworkObject : NetworkObject
    {
        protected override bool ServerAllowRpc(byte methodId, Receivers receivers, RpcArgs args)
        {
            // The methodName is the name of the RPC that is trying to be called right now
            // The receivers is who the client is trying to send the RPC to
            // The args are the arguments that were sent as part of the RPC message and what the receivers will receive as arguments to the call
        }
    }
}
```

Notice that you need to return a boolean from this function. If you return **true** then the RPC will be allowed to go through, it will be invoked on the server and on the clients (if specified in Receivers). If you return **false** then the RPC will not be invoked at all and will be dropped.
