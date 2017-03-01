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

namespace BeardedManStudios.Forge.Networking
{
	public struct UDPPacket
	{
		public bool reliable, endPacket, isConfirmation;
		public int groupId, orderId, retryCount;
		public Receivers receivers;
		public ulong uniqueId;
		public byte[] rawBytes;

		public UDPPacket(bool reliable, bool endPacket, int groupId, int orderId, ulong uniqueId, byte[] rawBytes, bool isConfirmation, Receivers receivers)
		{
			this.reliable = reliable;
			this.endPacket = endPacket;
			this.groupId = groupId;
			this.orderId = orderId;
			this.uniqueId = uniqueId;
			this.rawBytes = rawBytes;
			this.isConfirmation = isConfirmation;
			this.receivers = receivers;
			retryCount = 0;
		}

		public void DoingRetry()
		{
			retryCount++;
		}
	}
}