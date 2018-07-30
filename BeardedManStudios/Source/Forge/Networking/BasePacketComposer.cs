/*-----------------------------+-------------------------------\
|                                                              |
|                         !!!NOTICE!!!                         |
|                                                              |
|  These libraries are under heavy development so they are     |
|  subject to make many changes as development continues.      |
|  For this reason, the libraries may not be well commented.   |
|  THANK YOU for supporting forge with all your feedback       |
|  suggestions, bug reports and comments!                      |
|                                                              |
|                              - The Forge Team                |
|                                Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

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