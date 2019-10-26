using UnityEngine;

namespace BeardedManStudios
{
	[CreateAssetMenu(menuName = "Forge Networking Remastered/Forge Settings")]
	public class ForgeSettings : ScriptableObject
	{
		[Header("Forge behaviour settings")]
		public bool DontChangeSceneOnConnect = false;
		public bool useMainThreadManagerForRPCs = true;
		public bool useTCP = false;

		public bool connectUsingMatchmaking = false;
		public bool useElo = false;
		public bool useInlineChat = false;
		public int myElo = 0;
		public int eloRequired = 0;

		[Header("Address settings")]
		public string masterServerHost = string.Empty;
		public ushort masterServerPort = 15940;
		public string natServerHost = string.Empty;
		public ushort natServerPort = 15941;



		[Header("Server Query Protocol settings")]
		[Tooltip("Enabled/disable ServerQueryProtocol. Only the server takes this setting into account.")]
		public bool getLocalNetworkConnections = false;
		public bool enableSQP = true;
		public ushort SQPPort = 15900;

		[Header("Game Settings")]
		[Tooltip("Used for sending to the Master server and on the Server Browser")]
		public string serverId = "myGame";
		public string serverName = "Forge Game";
		public string type = "Deathmatch";
		public string mode = "Teams";
		public string comment = "Demo comment...";
	}
}
