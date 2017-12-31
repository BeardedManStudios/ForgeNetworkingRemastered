# Thread Safe Player Iteration

Sometimes it is important for our server to go through every player on the network and perform some action for them. Being that the players can connect asynchronously to the server, the **Players** list can be altered at any point, even while we are currently iterating through the players. There are two ways to be able to go through the players in a thread-safe manor **WHICH YOU MUST ALWAYS DO**. You either will be required to lock the players before performing any actions on them, or you can use the provided helper method **NetWorker::IteratePlayers**.

If you were to use a simple lock, then let's say we had a reference to a **NetWorker** with the variable name myNetWorker. Now you have direct access to the players via the myNetWorker.Players getter, however it is not thread safe to just simply access them in this way; you instead will lock them as seen in the following:

### Locking
```csharp
lock (myNetWorker.Players)
{
	// Do your player iteration logic here
}
```

The second way would be to use the **NetWorker::IteratePlayers** method as described above. There are two ways that you can do this, the first being to provide a lambda expression [INSERT LINK] for quick inline actions and the other is to provide a function pointer.

### Lambda Expression
```csharp
// This is an example of using a lambda expression
myNetWorker.IteratePlayers((player) =>
{
	// Do your player iteration logic here
});
```

### Function Pointer
```csharp
// This is an example of using a function pointer
myNetWorker.IteratePlayers(GoThroughPlayers);

// ...
private void GoThroughPlayers(NetworkingPlayer player)
{
	// Do your player iteration logic here
}
```