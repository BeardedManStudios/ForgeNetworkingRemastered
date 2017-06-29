# Buffered RPCs
So you've probably gone through the [Basic RPC Example](basic-rpc-example) by now and are curious about the various `Receivers` are that you could add when calling an RPC. If you browsed through the available `Receivers` you would have noticed ones suffixed with the word **Buffered**. For example `AllBuffered` or `OthersBuffered`. Buffering of RPC calls is important for players who are going to be joining the game late. For example let us say that you blew up a barrel at some point in the game. Now imagine someone coming into the game for the first time well after you blew up the barrel. How is that player going to know that the barrel is blown up so that they can't blow it up again? One solution you may try is using **Buffered RPCs** woo!

What happens with buffered RPCs is that the RPC is stored on the server. The moment that a player has been accepted they will first receive all of the currently active network objects. Once the player has confirmed that they have received the network objects then the server will send down the buffered RPCs. Effectively calling the explode barrel RPC that was called a long time ago. Now of course this is a bad example of something to do gameplay-wise because then the player who just joined would have a bunch of barrels exploding out of nowhere, but you should be able to understand the purpose of buffered RPCs now.

So to buffer an RPC all you have to do is select a `Receivers` which has a **Buffered** prefix as seen below:
```csharp
float explosionVelocity = 99.98f;
networkObject.SendRpc(RPC_BLOWUP_BARREL, Receivers.AllBuffered, explosionVelocity);
```