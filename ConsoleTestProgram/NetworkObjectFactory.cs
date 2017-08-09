using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;

namespace ConsoleTestProgram
{
	public class NetworkObjectFactory : INetworkObjectFactory
	{
		public void NetworkCreateObject(NetWorker networker, int identity, uint id, FrameStream frame, System.Action<NetworkObject> callback)
		{
			bool availableCallback = false;
			NetworkObject obj = null;
			switch (identity)
			{
				case ConsoleNetworkObject.IDENTITY:
					availableCallback = true;
					obj = new ConsoleNetworkObject(networker, id, frame);
					if (callback != null)
						callback(obj);
					break;
			}

			if (!availableCallback && callback != null)
			{ 
				callback(obj);
			}
		}

		// DO NOT TOUCH, THIS GETS GENERATED
	}
}