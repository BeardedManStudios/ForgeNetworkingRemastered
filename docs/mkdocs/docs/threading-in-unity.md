# Threading in Unity

If you have dealt with threading before and attempted to use it within Unity, then you probably know by now that you are not able to access the native Unity features from any thread other than the **main thread**. This does raise a bit of a complication when it comes to a completely multithreaded system such as Forge Networking. We've decided that running only on the main thread restricts our users and ourselves from achieving the full potential in performance that we would like. For this reason, we allow the user to manage how code is offloaded to the main thread in their applications.

**What is threading?**

_If you are familiar with threading then you can skip this section._

This is not going to be a complicated explanation of what the processor is doing at a hardware level but a simple abstract explanation so that it is not such an unfamiliar topic. I'm sure by now we are all familiar with machines that say things like **dual-core** or **quad-core** or **octa-core** etc. Imagine if you will that the machine has 2 cpu chips when we say **dual core** if this were the case, you could also imagine that you can run 2 things at the same exact time. This works much like if you hired someone to fold papers, if you hired a second person to fold papers at the same time, then you could get twice the work done in the same amount of time at the same exact time. Now lets imagine that the papers were a variable such as an int, what if both employees finished folding a paper at the same exact time and they both reached for the next paper in the stack? Well only one person can alter one paper at a time, you could wind up with the paper being folded twice!

Now that you have that basic understanding of a simple issue that comes up with parallel computing (threading) you can say, "what if unity messes with the game object, but I do at the same time?". Of course no good will come out of this, this is precisely why Unity does not allow you to manipulate Unity objects from an external thread. There are probably a large number of reasons that Unity has chosen not to simply allow you to lock a mutex, but I digress. For this reason, we have created a helper class named **MainThreadManager** which we will talk more in depth in a

later section.

**NOTE:  ** Many modern CPUs can have multiple threads per core, for example the i7-6700K has 2 threads per core making a 4 core processor with 8 threads.

**What runs on a separate thread in Forge?**

In forge we have 2 critical threads for both the client and the server, and 1 extra critical thread for the server (in TCP mode). When in TCP mode the special thread that the server runs is the connection thread. This thread listens for new client connections and will begin the acceptance process from here. The other 2 threads that are shared on client and server, UDP and TCP, is the write and read threads. There is one long lived thread that is used for the reading of network messages. This reading thread will also execute RPC methods, read message events, and so forth. Almost everything that is processed on the network can be traced back to the read thread. The second thread, the write thread, is shared for all players. This thread is shared so that it can support thousands of connections/players at a time on lower end CPUs. This thread is an on-demand thread. It starts up when messages are being sent, and it shuts down when there are no more messages being sent. The client also uses the same write thread logic, however it is not as active as the server write thread for obvious reasons (it only communicates with the server and nobody else).

Currently the only network communication that you need to worry about as an end user that get's called on a separate thread is an RPC.