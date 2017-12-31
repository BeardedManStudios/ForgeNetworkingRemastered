# Loading Scenes
One of the things that you may be worring about is how you can load into new scenes on the network. Some issues you may worry about are:

- How do clients know when to change scenes?
- How do I clean up all the network objects?
- How do clients know what scene to go to?
- How do newly connected clients know what scene is currently active?
- How do I handle additive scenes?

These are all good questions and also all questions that you don't have to worry about in Forge Networking. We hook into Unity's `SceneManager.sceneLoaded` event so that we can detect when the scene changes or new scenes are additively loaded on the server. Once the server detects this scene change it will tell all the connected clients to change to the new scene that the server has loaded. If an addative scene was loaded on the server, the server then tells the clients to also additively load the scene. Also the server keeps track of the scenes that are currently active so that it can send them down to the newly accepted client who just connected.

There are a few important notes about scenes with Forge Networking:

1. The server must load the scene to trigger all of the clients to load scenes
2. Clients can still load scenes without the server having to dictate the change, however when the server changes the scene it will get pushed to the client and overwrite the changes
3. You load scenes as you normally would do in Unity on the server, no extra calls are needed

Below is a sample of how to load a scene on the server only if both the server and client are sharing code. The `Networker` in this case will be a reference to the NetWorker in `NetworkManager.Instance.Networker`.

#### Loading a single scene on the server
```csharp
// This will load scene at build index 1 only if this is the server's code
if (Networker.IsServer)
	SceneManager.LoadScene(1, LoadSceneMode.Single);
```

#### Async loading a single scene on the server
```csharp
// This will load scene at build index 1 only if this is the server's code
if (Networker.IsServer)
	SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
```

#### Loading an addative scene on the server
```csharp
// This will load scene at build index 1 only if this is the server's code
if (Networker.IsServer)
	SceneManager.LoadScene(1, LoadSceneMode.Additive);
```

#### Async loading an additive scene on the server
```csharp
// This will load scene at build index 1 only if this is the server's code
if (Networker.IsServer)
	SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
```