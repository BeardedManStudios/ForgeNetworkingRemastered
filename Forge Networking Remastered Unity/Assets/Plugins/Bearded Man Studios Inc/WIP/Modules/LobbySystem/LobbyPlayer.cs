using UnityEngine;
using System.Collections;
using System;

namespace BeardedManStudios.Forge.Networking.Unity.Lobby
{
	public class LobbyPlayer : IClientMockPlayer
	{
		private string _name;
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		private uint _networkID;
		public uint NetworkId
		{
			get
			{
				return _networkID;
			}

			set
			{
				_networkID = value;
			}
		}

		public int AvatarID = 0;
		public int TeamID = 0;
	}
}