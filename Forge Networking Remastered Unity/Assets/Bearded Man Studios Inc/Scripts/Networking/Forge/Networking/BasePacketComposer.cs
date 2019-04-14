using BeardedManStudios.Forge.Networking.Frame;

namespace BeardedManStudios.Forge.Networking
{
    public class BasePacketComposer
    {
        public virtual void ResendPackets(ulong timestep, ref int counter) { Logging.BMSLog.LogWarning("Warning: BasePacketComposer:ResendPackets() was invoked directly. This is not supposed to happen!"); }

        /// <summary>
        /// The frame that is to be sent to the user
        /// </summary>
        public FrameStream Frame { get; protected set; }
    }
}
