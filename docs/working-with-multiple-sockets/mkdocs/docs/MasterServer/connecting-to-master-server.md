# Connecting to Master Server
_If you have not already, we would suggest reviewing the [Master Server Quick Start](master-server-quick-start) documentation to get familiar with the Master Server and to setup the stand alone executable._

When you first initialize your **NetworkManager** (as seen in `MultiplayerMenu.cs`) you will need to provide it with the NetWorker that you wish to use for standard game network communications. To do this the NetworkManager has a method `MasterServerRegisterData` that you can use in order to get the registration data object to allow you to register your game server with the Master Server. Below is an example of how to use the `MasterServerRegisterData` method to generate the json data needed to register the game server.

```csharp
string serverId = "myGame";
string serverName = "Forge Game";
string type = "Deathmatch";
string mode = "Teams";
string comment = "Demo comment...";
bool useElo = false;
int eloRequired = 0;

JSONNode masterServerData = NetworkManager.Instance.MasterServerRegisterData(networker, serverId, serverName, type, mode, comment, useElo, eloRequired);
```

You may be curious what to do next with this data and the answer is to send it into the NetworkManager's `Initialize` method as seen below.

```csharp
// This should be the IP address of the machine that your master server is hosted on
string masterServerHost = "127.0.0.1";

// This can be left as the default port (15940) if you did not change it when hosting the master server
ushort masterServerPort = 15940;

NetworkManager.Instance.Initialize(networker, masterServerHost, masterServerPort, masterServerData);
```

With all of this setup your game server should now be automatically registered on the Master Server once this method is called.