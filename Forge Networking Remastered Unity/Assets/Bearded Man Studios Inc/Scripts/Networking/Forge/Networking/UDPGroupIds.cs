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

using System.Collections.Generic;

namespace BeardedManStudios.Forge.Networking
{
	public static class MessageGroupIds
	{
		private static int groupId = START_OF_GENERIC_IDS;
		public static Dictionary<string, int> groupIdLookup = new Dictionary<string, int>();

		public static int GetId(string key)
		{
			if (!groupIdLookup.ContainsKey(key))
				groupIdLookup.Add(key, ++groupId);

			return groupIdLookup[key];
		}

		public const int TCP_FIND_GROUP_ID = 0;
		public const int NETWORK_ID_REQUEST = 1;
		public const int NETWORK_OBJECT_RPC = 2;
		public const int CREATE_NETWORK_OBJECT_REQUEST = 3;
		public const int NAT_SERVER_CONNECT = 4;
		public const int NAT_SERVER_REGISTER = 5;
		public const int NAT_ROUTE_REQUEST = 6;
		public const int MAX_CONNECTIONS = 7;
		public const int DISCONNECT = 8;
		public const int MASTER_SERVER_REGISTER = 9;
		public const int MASTER_SERVER_UPDATE = 10;
		public const int MASTER_SERVER_GET = 11;
		public const int PING = 12;
		public const int PONG = 13;
		public const int VOIP = 14;
		public const int CACHE = 15;
		public const int VIEW_INITIALIZE = 16;
		public const int VIEW_CHANGE = 17;
		public const int NOT_ACCEPT_CONNECTIONS = 18;
        public const int AUTHENTICATION_CHALLENGE = 19;
        public const int AUTHENTICATION_RESPONSE = 20;
        public const int AUTHENTICATION_FAILURE = 21;

        public const int START_OF_GENERIC_IDS = 10000;
	}
}
