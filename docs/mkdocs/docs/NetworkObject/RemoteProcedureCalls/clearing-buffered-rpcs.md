# Clearing Buffered RPCs

When buffering RPC calls, you may have noticed that the buffered call exists for the entire life-span of the network object. This is so that critical functions can be performed on an object to clients who connect later (after the RPC was executed). The problem you may run into with this is that you would need to delete the object to reset the level or partly reset the level. This of course presents the problem of managing and instantiating objects remotely.

To resolve this issue, you can clear out the buffered RPCs of an object using the `NetworkObject::ClearRpcBuffer`. 
This will request for the buffered RPCs of a specific object to be cleared on the server if called from the client, otherwise it will immediately clear the buffered RPCs for the network object in question if called from the server.
