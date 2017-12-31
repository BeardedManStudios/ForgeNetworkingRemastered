# Main Threading RPCs

The RPC behaviors in Forge Networking work on the network reading thread for purposes of performance when it comes to reading network data. This means that accessing any Unity specific objects in this thread will cause an error (please see Threading in Unity). Many of our newer users will be unconformable working with function pointers, lambda expressions or threading so we have created an easy way for you to make all RPC calls perform on the main thread. At any point in your application you can assign the Rpc's **MainThreadRunner** object. By doing this you will essentially make all incoming RPC calls run on Unity's main thread. To do this you would do the following in your code.

For an example of this, please see the **MultiplayerMenu.cs** file and follow the **useMainThreadManagerForRPCs** variable.

If you wish to manually say when to run an RPC on the main thread. You can always use the **MainThreadManage.Run** method to queue actions to perform on the main thread. To understand this further, please review Threading in Unity.
