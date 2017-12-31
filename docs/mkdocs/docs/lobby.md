# Lobby System

The lobby system is a way for you to be able to manage your existing players on your particular server on a global level.

**With this system, you are able to do the following:**

* Set Player Name (string)
* Set Avatar ID (int)
* Set Team ID (int)
* Send a message to everyone like the chat system (string)
* Kick player
* Start game button

This system allows us to be able to control the list of players and functionality of those players at runtime. There is a basic dummy lobby master class that will be the default system chosen until overridden with either our custom Lobby Master (unity implementation) or your own Custom Lobby Master. Because of the way it is written for forge, this system does not need to be ran from Unity and can work perfectly fine on your linux builds (more on this later).

## Using the Existing Lobby System
The existing lobby system is available out of the box to be used in your application. The one thing you want to check is if you are using the MultiplayerMenu and loading into your game then you'll be fine following skipping to step #2.

1. First ensure that whatever network loading sequence you are going with, that the host will call the following code.

```csharp
LobbyService.Instance.Initialize(serversocket);
```
Where the 'server socket' would be the socket that the host makes when hosting. You can see this being done in the example MultiplayerMenu.cs that is included with Forge Networking.

2. Add the lobby system prefab (LobbySystem.prefab) to your game scene, or wherever you want to include the lobby system to your existing game.
3. If you don't have an event system & a standalone input module in your scene already, then you would need to make sure they are in there to interact with the lobby items as well.

The lobby player item prefab is just a prefab that stores the users name, color, buttons and the like. You are free to modify this prefab to your liking as well as the lobby system itself.

4. Congratulations you have officially added the lobby system to your game and can start using it! Change your name in your player top let, and start talking in the chat bottom left, change the UI to however you please as well as the prefab.

5. PS- To get better results, make sure in the build settings Multiplayermenu -> *Lobby* -> Game

## Lobby Service API Calls
The lobby service has built in api calls that are currently being used in the LobbyManager. For reason of explaining it further, these api calls can be called from anywhere in code as long as you are connected to a socket. This allows you to change the users name from anywhere other than the built in Lobby system.

```csharp
string newName = "";
LobbyService.Instance.SetName(newName);

uint playerID = 999; 
//Only the server will actually kick players
LobbyService.Instance.KickPlayer(playerID);

int avatarID = -1;
LobbyService.Instance.SetAvatar(avatarID);

int teamID = -1;
LobbyService.Instance.SetTeamId(teamID);

string chatMessage = "";
LobbyService.Instance.SendPlayerMessage(chatMessage);
```

Here is some examples of what the data structure of sending these api calls would look like. As stated before, these can be called anywhere in your code and your LobbyMaster/LobbyManager would handle the data received from other players.

You can easily grab the list of players by doing the following:
```csharp
List<IClientMockPlayer> currentPlayers = LobbyService.Instance.MasterLobby.LobbyPlayers;
```
This list will auto populate with the users that join and leave/disconnect.

## Lobby System Enhancements
This section is for advanced users who want to expand the lobby system past what is already there for further functionality.

Please take a look at the following classes:
LobbyManager.cs
LobbyPlayer.cs
LobbyPlayerItem.cs

### Lobby Player Manager
The lobby player manager is an example of how you would expand the lobby system itself. Right now it implements the functionality of ILobbyMaster and replaces the mock lobby system with this one during setup. You'll see that it replaces it in SetupComplete() with the following code.

```csharp
LobbyService.Instance.SetLobbyMaster(this);
```

SetupComplete() also creates the player item for himself as well as for the already connected players.

You'll notice that you can modify the data however you please and expand on it given the Interface API that is implemented because of the system. If you want to fully understand how the implementation is done, please take a further in-depth look at LobbyService.cs (which we will not cover in this documentation at the moment as it is a lot to go over).

### Lobby Player
The purpose of the lobby player is to implement the IClientMockPlayer which will allow you to implement the basic data that we require for the lobby system to work and expand on it with your own custom data. Just note that if you want that data to be synced properly for any additional logic, you would have to implement the code to be in the lobby manager as well as the LobbyService, otherwise you can always sync the data from other networked objects to the lobby manager with RPC's as well. We will have to revisit this to help make expanding the player data, but just or now I recommend taking a look at the way the data is structured and used in the Lobby Manager in order to populate it as well as the Lobby Service.

The Lobby Player is used extensively on our Lobby Manager to store the current state of a given player.

### Lobby Player Item
The lobby player item prefab is used to populate itself with the data of a given player passed in. This is mostly just a ui helper and is not necessary for the player to exist as the lobby system itself will always have all the current players. For demonstration purposes we have included a simple prefab that is being populated in the provided lobby system included with Forge Networking. You are free to populate and change the ui however you please with this data passed in.

We store a reference to the Lobby system itself, but you can easily do other ways of sending the button calls to your own custom lobby system by passing in the actions and the like. This way you would remove the need to have the lobby system being stored as a reference in every lobby player item. The reason we stuck with this format is to just showcase the ease of expanding this item with future features that we plan on adding.

### Start Game
The start game button sends all the players to the Game scene. How this is setup in build settings. 

MultiplayerMenu -> Lobby Scene -> Game 

Here is the script of Start Game, if you want to use it in other applications.

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
public void startGame (int Levelnum)
    { 
        Application.LoadLevel(Levelnum);
    }
}
```

### Create Your Own Lobby (Easier way)
1) Create a new scene.
2) Add the LobbySystem Prefab.
3) Change the UI, Change the buttons and edit/copy off of the LobbySystem Prefab.
4) make sure the scripts in the LobbySystem prefab go into the desired area in the scene as well as the Inspector (Button scripts go in the button scripts, etc).
5) add it to MultiplayerMenu -> *Lobby* -> Game

### Lobby Scene
This Lobby can be used for your game or used as a reference for you to make your own Lobby. This is the Game-Scene.
![Lobby Scene](https://raw.githubusercontent.com/BeardedManStudios/ForgeNetworkingRemastered/develop/docs/mkdocs/docs/images/(Lobby-Scene-View).png)



### Lobby Overview
This system allows us to be able to control the list of players and functionality of those players at runtime. There is a basic dummy lobby master class that will be the default system chosen until overridden with either our custom Lobby Master (unity implementation) or your own Custom Lobby Master. Because of the way it is written for forge, this system does not need to be ran from Unity and can work perfectly fine on your linux builds (more on this later).


### To Be Continued
As the lobby system is a great system to add to your game, we are very open to suggestions that can add further features to this as well as give the users the ability to chime in over Discord. As we add more features expect this documentation to cover that and expand.
