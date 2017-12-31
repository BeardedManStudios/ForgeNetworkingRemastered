# Getting Host List From Master Server
_If you have not already, we would suggest reviewing the [Master Server Quick Start](master-server-quick-start) documentation to get familiar with the Master Server and to setup the stand alone executable and then reviewing [Connecting to Master Server](connecting-to-master-server) to learn how to register your game server on the master server._

## Request stages
There are a few stages when requesting the list of servers from the master server:

1. Connect to Master Server
2. Create a JSONNode request
3. Send the request to the server
4. Get response from the server
5. Parse response from server
6. Implement your code

(1) So the very first thing you have to do is to connect to the master server. We have created a helper class to get you doing this quickly:

```csharp
// TODO:  This will be the address of the machine your master server is running on
string host = "127.0.0.1";

ushort port = 15937;

// The Master Server communicates over TCP
TCPMasterClient client = new TCPMasterClient();

// Just call the connect method and you are ready to go
client.Connect(host, ort);
```

(2) Once your server is accepted (which you can use the `serverAccepted` event) you can create your request to the master server. The request is a JSONNode object that follows this structure:

```
{
    get:
    {
        id: "myGame",
        type: "any",
        mode: "all"
    }
}
```

You can do this with code similar to the following:

```csharp
// The overall game id to select from
string gameId = "myGame";

// The game type to choose from, if "any" then all types will be returned
string gameType = "any";

// The game mode to choose from, if "all" then all game modes will be returned
string gameMode = "all";

// Create the get request with the desired filters
JSONNode sendData = JSONNode.Parse("{}");
JSONClass getData = new JSONClass();

// The id of the game to get
getData.Add("id", gameId);
getData.Add("type", gameType);
getData.Add("mode", gameMode);

sendData.Add("get", getData);
```

(3) Now that we have the json `sendData` we need to send it to the server. Since JSON can be a string representation, you can send it directly to the server as a **Text** frame in Forge:

```csharp
// Send the request to the server
client.Send(Frame.Text.CreateFromString(client.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_GET, true));
```

(4) If you listen to the `textMessageReceived` event on the `client` object you will be able to capture the response from the server. This response will contain any errors (if any) and most importantly, your game list!

(5) Now that you have the JSON response from the server, you will need to parse it into the `MasterServerResponse` object. Lets imagine that your json response variable is called `data`, then the code you will need to parse the response to this object is:

```csharp
// Create a C# object for the response from the master server
MasterServerResponse response = new MasterServerResponse(data["hosts"].AsArray);
```

**Note**: If `data["hosts"]` is null, then there was an error.

(6) Now that you have the `MasterServerResponse` object, you are ready to start writing your code for the UI and any other behaviors you want to execute based on the server listing.

```csharp
foreach (MasterServerResponse.Server server in response.serverResponse)
{
    // TODO:  Do something with the server information here!
}
```

## Refresh Complete Sample

Below is a compelte example of how to:

- Connect to master server
- Request the games list from the master server
- Process the list of games returned from the master server
- Disconnect from the master server

The below example can be used to refresh the current listings of hosts from the master server. Please be sure to read the comments within the code to better understand what is going on. It is also good to scroll up and reference the top of this page to see why and when things happen in this sample.

```csharp
public void Refresh()
{
    // TODO:  Clear out any previously listed servers

    // The Master Server communicates over TCP
    TCPMasterClient client = new TCPMasterClient();

    // Once this client has been accepted by the master server it should sent it's get request
    client.serverAccepted += () =>
    {
        try
        {
            // The overall game id to select from
            string gameId = "myGame";

            // The game type to choose from, if "any" then all types will be returned
            string gameType = "any";

            // The game mode to choose from, if "all" then all game modes will be returned
            string gameMode = "all";

            // Create the get request with the desired filters
            JSONNode sendData = JSONNode.Parse("{}");
            JSONClass getData = new JSONClass();

            // The id of the game to get
            getData.Add("id", gameId);
            getData.Add("type", gameType);
            getData.Add("mode", gameMode);

            sendData.Add("get", getData);

            // Send the request to the server
            client.Send(Frame.Text.CreateFromString(client.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_GET, true));
        }
        catch
        {
            // If anything fails, then this client needs to be disconnected
            client.Disconnect(true);
            client = null;
        }
    };

    // An event that is raised when the server responds with hosts
    client.textMessageReceived += (player, frame) =>
    {
        try
        {
            // Get the list of hosts to iterate through from the frame payload
            JSONNode data = JSONNode.Parse(frame.ToString());
            if (data["hosts"] != null)
            {
                // Create a C# object for the response from the master server
                MasterServerResponse response = new MasterServerResponse(data["hosts"].AsArray);

                if (response != null && response.serverResponse.Count > 0)
                {
                    // Go through all of the available hosts and add them to the server browser
                    foreach (MasterServerResponse.Server server in response.serverResponse)
                    {
                        Debug.Log("Name: " + server.Name);
                        Debug.Log("Address: " + server.Address);
                        Debug.Log("Port: " + server.Port);
                        Debug.Log("Comment: " + server.Comment);
                        Debug.Log("Type: " + server.Type);
                        Debug.Log("Mode: " + server.Mode);
                        Debug.Log("Players: " + server.Players);
                        Debug.Log("Max Players: " + server.MaxPlayers);
                        Debug.Log("Protocol: " + server.Protocol);

                        // TODO:  Update UI or something with the above data
                    }
                }
            }
        }
        finally
        {
            if (client != null)
            {
                // If we succeed or fail the client needs to disconnect from the Master Server
                client.Disconnect(true);
                client = null;
            }
        }
    };

    client.Connect(masterServerHost, (ushort)masterServerPort);
}
```