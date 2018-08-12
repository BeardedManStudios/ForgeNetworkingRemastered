# UDP LAN Discovery
LAN Discovery is a way for you to quickly find other game servers on the local area network. This is done by sending out a broadcast message on the local network that gets picked up by the game servers that are currently running. When they receive the lan discovery request they respond to the sender to let the sender know the IP addresses and port numbers that are available. There is only 1 method that needs to be called to use LAN Discovery in Forge Networking:

```csharp
NetWorker.RefreshLocalUdpListings();
```

The above is a static method that is called to trigger the request. Since this request is a threaded request, you can not expect to have any results immediately after the request has been made. So often you will want to listen for the various servers to respond. Below is an example of how to do this:

```csharp
NetWorker.localServerLocated += LocalServerLocated;
NetWorker.RefreshLocalUdpListings();

// ... Some code and stuff

private void LocalServerLocated(NetWorker.BroadcastEndpoints endpoint)
{
    Debug.Log("Found endpoint: " + endpoint.Address + ":" + endpoint.Port);
}
```

You can replace the above debug log with whatever code you would like. The endpoint contains the server that has responded (it's address and port number). With this information you are able to connect to the server.

## RefreshLocalUdpListings
Notice that this is a refresh method, that means that you will need to call it each time you want to refresh the listings of servers on the network. This method can be called at an interval if you would like or you could have your players click a button to invoke it. Also notice that the `RefreshLocalUdpListings` takes in an argument (time in milliseconds) to wait for server responses. The default is 1000 (1 second). If a server doesn't respond in that amount of time it will not be counted. If you do not want to rely on the event callback alone you can also get the listings of servers from the `NetWorker.LocalEndpoints` list.
