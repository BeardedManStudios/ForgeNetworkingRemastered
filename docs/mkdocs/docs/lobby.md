# Lobby System

The lobby system is a way for you to be able to manage your existing players on your particular server on a global level.

**With this system, you are able to do the following:**

* Set Player Name (string)
* Set Avatar ID (int)
* Set Team ID (int)
* Send a message to everyone like the chat system (string)
* Kick player
* Start game button

### How To Use
1: Open the Lobby scene in Unity.
2: Open Build settings put your game in where it says 1 (2nd scene down).
3: Add your game scene to 2 (3rd scene down)
4: Build your Game.
5: enjoy playing around with the lobby!

This system allows us to be able to control the list of players and functionality of those players at runtime. There is a basic dummy lobby master class that will be the default system chosen until overridden with either our custom Lobby Master (unity implementation) or your own Custom Lobby Master. Because of the way it is written for forge, this system does not need to be ran from Unity and can work perfectly fine on your linux builds (more on this later).
