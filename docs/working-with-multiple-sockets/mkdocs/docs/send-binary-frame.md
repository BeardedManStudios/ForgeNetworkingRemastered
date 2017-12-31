# Send Binary Frame
The **Binary** frame (BeardedManStudios.Forge.Networking.Frame.Binary) is a child of the **FrameStream** object which is how all messages are sent in Forge (even our internal code). To use this you need to serialize your data to a byte[] or pack it into a BMSByte object (wrapper around byte[] for caching).

First you need to create your data
```csharp
byte[] data = { /* Your data */ };
```

Next you need to create the frame, since you want to send it to a specific player you will need to make the receivers be Target
```csharp
const MY_GROUP_ID = MessageGroupIds .START_OF_GENERIC_IDS + 1; // Just a random message group id that is not being used anywhere else
// …
ulong timestep = NetworkManager.Instance.Time.Timestep;
bool isTcpClient = NetworkManager.Instance.Networker is TCPClient;
bool isTcp = NetworkManager.Instance.Networker is BaseTCP;
Binary bin = new Binary(timestep, isTcpClient, data, Receivers.Target, MY_GROUP_ID, isTcp);
```

Now that you have your binary frame named "bin", you can send it across the network. This is assuming that you are sending this frame from the server
```csharp
if (networker is UDPServer)
{
    bool isReliable = true;
    ((UDPServer)networker).Send(targetNetworkingPlayer, bin, isReliable);
}
else
    ((TCPServer)networker).Send(targetNetworkingPlayer, bin);
```

To read this message you need to listen to the NetWorker::binaryMessageReceived event on the client. You can filter by the group id you've specified
```csharp
NetworkManager.Instance.Networker.binaryMessageReceived += ReadBinary;

// ...

private void ReadBinary(NetworkingPlayer player, Binary frame)
{
    if (frame.GroupId != MY_GROUP_ID)
        return;

    // TODO:  Your code here
}
```