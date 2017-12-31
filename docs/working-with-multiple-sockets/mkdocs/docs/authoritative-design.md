# Authoritative Design

Often you will hear the term **authoritative server** and in most cases, it is used to mean **authoritative game logic**. So, we would like to first go over the difference between the two as we often state that Forge Networking (including Remastered) is **authoritative server** by design.

Authoritative Server:  When all network traffic goes through the server before it is relayed to other clients. In other words, clients do not communicate with each other directly.

Authoritative Game Logic:  When all your game code is written in a way where the server is completely in control of the flow and owns all of the network objects. This is unique to your game and how your game plays, it can be complex based on your game and we would suggest exploring the idea of not having authoritative game logic unless your game requires it. Games that require it often are games that have long term tracking (such as Battlefield) or track data for users in a database (such as an MMO). Games that plan to have non-tracked multiplayer (such as Minecraft) would not opt into having full server authority due to the overhead of complexity.

So when we say that Forge Networking (and Remastered) is an **authoritative server** we are talking about the former where all network traffic goes through the server first and then is relayed to all of the clients.