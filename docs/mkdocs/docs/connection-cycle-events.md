# Connection Cycle Events
Below are a quick explanation of the various events that happen during the connection and acceptance cycles on both the server and the client.

## Server & Client Events
The below are events that are local-aware, that is to say that they are called without any need for an active connection.

### bindSuccessful
Called when the local machine allowed the binding of the port requested.

### bindFailure
Called when the local machine rejected the binding of the port requested.

### disconnected
Called when the local machine has completely disconnected from any communications in the Forge life cycle

## Server Events
The following events are called on the **server only** based on the behavior of clients connecting or disconnecting.

### playerConnected
Called when a player has successfully connected with the server. This player **CAN** be rejected by the server, so this event is mainly useful for evaluation of the player or preparing anything you need for the player. You should not be communicating with the player at all until they have been accepted.

### playerGuidAssigned
This event is called once the server has gotten the unique guid for a player. This triggers just before the accepted message and is useful to prepare any network player lookups.

### playerAccepted
Fired when the player has been officially accepted by the server and now is the time you are able to start sending your messages to this player.

### playerRejected
Fired when a player has been rejected for any reason by the server. This event will be expanded later but currently is used for when a client has sent invalid connection headers.

### playerTimeout
Called when a player has timed out on the server. This happens after they have been accepted and if the server hasn't received any message (ping or otherwise) from the specified client. The timeout time (in milliseconds) for a player can be found/set in the `NetworkingPlayer::TimeoutMilliseconds` property.

### playerDisconnected
Called at any time that a player has disconnected on its own or by the server.

## Client Events
The following events are called on the client only and deal with the various connection events that relate to this client and the server.

### serverAccepted
Called whenever the server has accepted this client. At this point it is safe to start sending messages to the server and to know it is a live connection.
