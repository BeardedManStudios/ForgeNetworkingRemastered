# NAT Hole Punching

Nat hole punching is a way for users behind a firewall to be able to connect to each other without requiring them to open their firewall ports on their router. To do this, it requires a special server that is dedicated to registering hosts and keeping an open communication with that game host. When a client requests to join a host that is behind a NAT they will communicate with the NAT service host. This NAT service host will then act as a delegate to request that the game host begin communications with the requesting client.

When you look at the **Connect** method of the **ServerUDP** and **ClientUDP** classes you will notice that there are 2 optional parameters related to nat traversal. One is the NAT host address and the other is the NAT server port. When you provide a NAT host address to the **Connect** method on a **ServerUDP** object, you will be telling the server where it needs to register itself and you will be opening communication with the NAT server for it to forward client connection requests. When you provide a NAT host address to the **Connect** method on a **ClientUDP** object, you will be telling the client to communicate with that specific NAT host to open communications with the desired server.

Setting the correct NAT host in the **Connect** method of the **ServerUDP** and **ClientUDP** is all you will need to do in order to enable NAT hole punching in your application.

## Video Tutorial On How To Test
[!embed?max_width=640300&max_height=360](https://www.youtube.com/watch?v=AbZX8GchHS4)
