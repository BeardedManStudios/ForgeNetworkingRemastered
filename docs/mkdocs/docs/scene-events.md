# Scene Events
There are some events that you can register to in order to know the flow of the scene loading events. Below are a list of the available events to hook into for scene loading and short descriptions of what they are for. **Note** All of these events are available through the `NetworkManager.Instance` so for example `playerLoadedScene` would be `NetworkManager.Instance.playerLoadedScene`.

## playerLoadedScene
This is used on the server to know when a specific player has finished loading into a level.

## networkSceneChanging
This is used on the client to know if it has been told by the server to change the scene. This happens just before the scene change is invoked.

## networkSceneLoaded
This is used on the client and the server to know when the scene has been successfully loaded and is ready to begin with networking code (starting with spawning Network Objects).