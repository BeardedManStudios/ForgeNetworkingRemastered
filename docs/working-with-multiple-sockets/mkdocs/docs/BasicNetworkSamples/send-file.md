# Send File
Forge Networking works with all kinds of data, but that doesn't exclude sending files across the network. At the end of the day, files are just an array of bytes (`byte[]`) and by now, you probably know you can send that stuff over the network with Forge Networking. So let's get started with a simple desktop->desktop file transfer shall we!

## The Setup
To get started we are going to need to setup our environment so that we can quickly and easily test our code. So let's open up a project with Forge Networking imported into it to get started.

1) Download the `deal-with-it.jpg` image (cat image below)
2) Open project with Forge Networking imported
3) Create a folder named `FileTransfer`
4) Copy the `deal-with-it.jpg` image into the `FileTransfer` folder
5) Create a new scene and save it into the `FileTransfer` folder named `FileTransferTest`
6) Create a C# script in the `FileTransfer` folder named `SendThatFile`
7) Open that script in your favorite editor
8) Put the following code into the script

![Deal With It!](https://raw.githubusercontent.com/BeardedManStudios/ForgeNetworkingRemastered/develop/docs/mkdocs/docs/images/deal-with-it.jpg)

*Note that this code is not optimized but made simple for quick understanding*
```csharp
using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System.IO;
using UnityEngine;

public class SendThatFile : MonoBehaviour
{
	public string filePath;
	private bool sentFile;

	private void Start()
	{
		NetworkManager.Instance.Networker.binaryMessageReceived += ReceiveFile;
	}

	private void ReceiveFile(NetworkingPlayer player, Binary frame)
	{
		// We are looking to read a very specific message
		if (frame.GroupId != MessageGroupIds.START_OF_GENERIC_IDS + 1)
			return;

		Debug.Log("Reading file!");

		// Read the string from the beginning of the payload
		string fileName = frame.StreamData.GetBasicType<string>();

		Debug.Log("File name is " + fileName);

		if (File.Exists(fileName))
		{
			Debug.LogError("The file " + fileName + " already exists!");
			return;
		}

		// Write the rest of the payload as the contents of the file and
		// use the file name that was extracted as the file's name
		File.WriteAllBytes(fileName, frame.StreamData.CompressBytes());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space) && !sentFile)
		{
			sentFile = true;

			// Throw an error if this is not the server
			var networker = NetworkManager.Instance.Networker;
			if (!networker.IsServer)
			{
				Debug.LogError("Only the server can send files in this example!");
				return;
			}

			// Throw an error if the file does not exist
			if (!File.Exists(filePath))
			{
				Debug.LogError("The file " + filePath + " could not be found");
				return;
			}

			// Prepare a byte array for sending
			BMSByte allData = new BMSByte();
			
			// Add the file name to the start of the payload
			ObjectMapper.Instance.MapBytes(allData, Path.GetFileName(filePath));

			// Add the data to the payload
			allData.Append(File.ReadAllBytes(filePath));

			// Send the file to all connected clients
			Binary frame = new Binary(
				networker.Time.Timestep,					// The current timestep for this frame
				false,                                      // We are server, no mask needed
				allData,									// The file that is being sent
				Receivers.Others,							// Send to all clients
				MessageGroupIds.START_OF_GENERIC_IDS + 1,   // Some random fake number
				networker is TCPServer);

			if (networker is UDPServer)
				((UDPServer)networker).Send(frame, true);
			else
				((TCPServer)networker).SendAll(frame);
		}
	}
}
```

## Preparing to run
1) Open the Unity build menu
2) Add the `MultiplayerMenu` scene as the first scene
3) Add the `FileTransferTest` as the second scene
4) Open the Player Settings...
5) Turn on run in background
6) Open the `FileTransferTest` scene
7) Select the camera or any object in the scene
8) Add the `SendThatFile` script to the object
9) Update the `File Path` field on the script to have the full path to the `deal-with-it.jpg` image inside of the `FileTransfer` folder
10) Save the scene
11) Build and Run
12) Open 2 instances of the game
13) Set one to be the server then connect with the other
14) Press the spacebar on the server
15) Go into the build folder for the player and then go up a directory, you will notice a `deal-with-it.jpg` has been created