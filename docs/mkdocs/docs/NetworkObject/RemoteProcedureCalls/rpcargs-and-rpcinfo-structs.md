# RpcArgs and RpcInfo Structs
When you setup a Remote Procedure Call you will notice that the argument supplied to this is called `RpcArgs`. This is a struct that helps ease the pain of getting objects from the network byte array. This `RpcArgs` has a `Info` member variable which is a `RpcInfo` struct. This one argument to your RPC helps you get not only the data you sent on the network, but some other useful network meta-data as well.

## Getting Data From RpcArgs
The RpcArgs is a wrapper around the network data with one important method that you actually care about. This method is the `RpcArgs::GetNext` method. This is an overloaded method with 2 overloads, the first being the parameter-less easy-use method and the other being the more advanced object selection by index method. If you look at the signature for the overload with the `int index` parameter, you may be thinking "what is index for?". This index is the index of the object you wish to pull from the network byte array. So lets say that you called a RPC with 3 arguments (a int, a string, and a float). If you wanted to get the float you could use `GetNext<float>(2)` where **2** is the index of arguments. the int being **0**, the string being **1**, and the float being **2**. The power of the parameter-less `GetNext<T>()` method is that it auto moves to the next argument. So if you wanted to read the int, string and float from the preceding example you would do the following:

```csharp
int num = args.GetNext<int>();
string name = args.GetNext<string>();
float val = args.GetNext<float>();
```

### Warning!!
You must pull out the arguments **IN ORDER**, if you do not, you will run into an error. So the following **WILL NOT** work for the same example because of the order of the network data:

```csharp
float val = args.GetNext<float>();
string name = args.GetNext<string>();
int num = args.GetNext<int>();
```

## Using the Info
Currently, at the time of this writing, the `Info` member variable in the `RpcArgs` has 2 properties. A `NetworkingPlayer` named `SendingPlayer` and a `ulong` named `TimeStep`. The `NetworkingPlayer` is the sender of this RPC. **NOTE** This is only useful on the server, clients will always see the server as the sending player since all network traffic first goes though the server. The `ulong` is used to know what timestamp that this message was sent at from the sender (this is time in milliseconds since the server started).

## SendingPlayer.Networker.IsServer and SendingPlayer.IsHost
You may notice in client execution that the value of `SendingPlayer.Networker.IsServer` is **False** but the value of `SendingPlayer.IsHost` is **True** and this has caused some confusion for the community. The way that you should think of `SendingPlayer.Networker` is that it is the NetWorker that is **used TO** communicate **with** the server, it is not the server's NetWorker. So if it is the NetWorker used to talk to the server, then it is this client's NetWorker, which makes the `IsServer` getter **False**. This is actually why we created the **IsHost** boolean to help aid this confusion and it is less code to get to.
