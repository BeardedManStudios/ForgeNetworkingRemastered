# Throttling Network Internal
So there are many tools out there to download and use to test applications and the general stability of networks, however we wanted to go over some quick and easy ways that you can test the network using just Forge Networking.

# Network Latency Simulation
The network latency simulation lets you test how your code would behave if there were to be network latency. This is a very simple tool that makes it so that the core of Forge will delay the reading/sending of messages by the given milliseconds. This is useful to test and see how clients with slow networks will experience your game. The latency simulation is **per NetWorker** so to test this out quickly you can use `NetworkManager.Instance.Networker`. Below is a sample of how to set the latency to 900 milliseconds (0.9 seconds):

```csharp
// Somewhere in code, even during runtime
NetworkManager.Instance.Networker.LatencySimulation = 900;	// 900 is the time in milliseconds
```

# Packet Loss Simulation
The packet loss simulation lets you test how your code would behave if there were to be packet loss of a specific chance. This tool allows you to set a percentage of packet loss. What this does internally is generate a random number, if it is within the specified percantage then it will delete the packet and ignore reading it. This will allow you to test your code to see what would happen if you were to lose packets during gameplay. The packet loss simulation is **per NetWorker** so to test this out quickly you can use `NetworkManager.Instance.Networker`. Below is a sample of how to set the percentage to 0.1f (10% packet loss chance):

```csharp
// Somewhere in code, even during runtime
NetworkManager.Instance.Networker.PacketLossSimulation = 0.1f;	// 0.1f is the percentage chance of packet loss (10% in this case)
```