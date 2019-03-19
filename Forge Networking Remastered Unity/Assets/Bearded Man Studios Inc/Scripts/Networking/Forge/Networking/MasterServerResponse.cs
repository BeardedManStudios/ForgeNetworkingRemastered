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

using BeardedManStudios.SimpleJSON;
using System.Collections.Generic;

namespace BeardedManStudios.Forge.Networking
{
	public class MasterServerResponse
	{
		#region Server
		public struct Server
		{
			public string Name;
			public string Address;
			public ushort Port;
			public string Comment;
			public string Type;
			public string Mode;
			public int PlayerCount;
			public int MaxPlayers;
			public string Protocol;
			public int Elo;
			public bool UseElo;
			public int EloDelta;

			public Server(string addr, ushort port)
			{
				Name = string.Empty;
				Address = addr;
				Port = port;
				Comment = string.Empty;
				Type = string.Empty;
				Mode = string.Empty;
				PlayerCount = 0;
				MaxPlayers = 0;
				Protocol = string.Empty;
				Elo = 0;
				UseElo = false;
				EloDelta = 0;
			}

			public Server(JSONClass data)
			{
				Name = data["name"];
				Address = data["address"];
				Port = data["port"].AsUShort;
				Comment = data["comment"];
				Type = data["type"];
				Mode = data["mode"];
				PlayerCount = data["players"].AsInt;
				MaxPlayers = data["maxPlayers"].AsInt;
				Protocol = data["protocol"];
				Elo = data["elo"].AsInt;
				UseElo = data["useElo"].AsBool;
				EloDelta = data["eloDelta"].AsInt;
			}

			public JSONClass ToJSON
			{
				get
				{
					JSONClass returnValue = new JSONClass();

					returnValue.Add("name", Name);
					returnValue.Add("address", Address);
					returnValue.Add("port", new JSONData(Port));
					returnValue.Add("comment", Comment);
					returnValue.Add("type", Type);
					returnValue.Add("mode", Mode);
					returnValue.Add("players", new JSONData(PlayerCount));
					returnValue.Add("maxPlayers", new JSONData(MaxPlayers));
					returnValue.Add("protocol", Protocol);
					returnValue.Add("elo", new JSONData(Elo));
					returnValue.Add("useElo", new JSONData(UseElo));
					returnValue.Add("eloDelta", new JSONData(EloDelta));

					return returnValue;
				}
			}


			#region Operators
			public static explicit operator JSONClass(Server server)
			{
				checked
				{
					return server.ToJSON;
				}
			}

			public static explicit operator Server(JSONClass data)
			{
				checked
				{
					Server newServer = new Server(data);
					return newServer;
				}
			}
			#endregion
		}
		#endregion

		public List<Server> serverResponse = new List<Server>();

		public MasterServerResponse(List<Server> servers = null)
		{
			if (servers != null)
				serverResponse = servers;
		}

		public MasterServerResponse(JSONArray data)
		{
			if (data != null)
			{
				for (int i = 0; i < data.Count; ++i)
				{
					serverResponse.Add((Server)data[i]);
				}
			}
		}
	}
}
