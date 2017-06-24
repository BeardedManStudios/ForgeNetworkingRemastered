using BeardedManStudios.Forge.Networking;

namespace ConsoleTestProgram
{
	public abstract class ConsoleBehavior
	{
		public const byte RPC_HELLO_WORLD = 0 + 4;

		public ConsoleNetworkObject networkObject = null;

		public void Initialize(NetworkObject obj)
		{
			networkObject = (ConsoleNetworkObject)obj;
			networkObject.RegisterRpc("HelloWorld", HelloWorld, typeof(string));
			networkObject.RegistrationComplete();
		}

		public void Initialize(NetWorker networker)
		{
			Initialize(new ConsoleNetworkObject(networker));
		}

		public abstract void HelloWorld(RpcArgs rpcArgs);

		// DO NOT TOUCH, THIS GETS GENERATED
	}
}