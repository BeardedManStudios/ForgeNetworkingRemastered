using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Generated;

namespace BasicStandAloneServer
{
    public class NetworkObjectFactory : INetworkObjectFactory
    {
        public void NetworkCreateObject(NetWorker networker, int identity, uint id, FrameStream frame, System.Action<NetworkObject> callback)
        {
            bool availableCallback = false;
            NetworkObject obj = null;
            switch (identity)
            {
                case BasicCubeNetworkObject.IDENTITY:
                    availableCallback = true;
                    obj = new BasicCubeNetworkObject(networker, id, frame);
                    callback?.Invoke(obj);
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