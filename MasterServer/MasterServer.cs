using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Threading;
using BeardedManStudios.SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System;

namespace MasterServer
{
	public class MasterServer
	{
		private const int PING_INTERVAL = 10000;

		public bool IsRunning { get; private set; }
		private TCPServer server;
		private List<Host> hosts = new List<Host>();
		private Dictionary<string, int> _playerRequests = new Dictionary<string, int>();
		private bool _eloRangeSet;
		private int _eloRange;
		public int EloRange
		{
			get { return _eloRange; }
			set
			{
				if (value == 0)
					_eloRangeSet = false;
				else
					_eloRangeSet = true;
				_eloRange = value;
			}
		}

		private bool _logging;
		public bool ToggleLogging()
		{
			_logging = !_logging;
			return _logging;
		}

		private void Log(object message)
		{
			if (!_logging)
				return;

			Console.WriteLine(message);
		}

		public MasterServer(string host, ushort port)
		{
			server = new TCPServer(2048);
			server.Connect(host, port);
			server.textMessageReceived += MessageReceived;

			IsRunning = true;
			server.disconnected += (sender) =>
			{
				IsRunning = false;
			};

			server.playerDisconnected += (player, sender) =>
			{
				for (int i = 0; i < hosts.Count; i++)
				{
					if (hosts[i].Player == player)
					{
						Log($"Host [{hosts[i].Address}] on port [{hosts[i].Port}] with name [{hosts[i].Name}] removed");
						hosts.RemoveAt(i);
						return;
					}
				}
			};

			Task.Queue(() =>
			{
				while (server.IsBound)
				{
					server.SendAll(server.GeneratePing());
					Thread.Sleep(PING_INTERVAL);
				}
			}, PING_INTERVAL);
		}

		private void MessageReceived(NetworkingPlayer player, Text frame, NetWorker sender)
		{
			try
			{
				JSONNode data = JSONNode.Parse(frame.ToString());

				if (data["register"] != null)
					Register(player, data["register"]);
				else if (data["update"] != null)
					Update(player, data["update"]);
				else if (data["get"] != null)
					Get(player, data["get"]);
			}
			catch
			{
				// Ignore the message and disocnnect the requester
				server.Disconnect(player, true);
			}
		}

		private void Register(NetworkingPlayer player, JSONNode data)
		{
			string name = data["name"];
			string address = ((IPEndPoint)player.TcpClientHandle.Client.RemoteEndPoint).Address.ToString();
			ushort port = data["port"].AsUShort;
			int maxPlayers = data["maxPlayers"].AsInt;
			int playerCount = data["playerCount"].AsInt;
			string comment = data["comment"];
			string gameId = data["id"];
			string gameType = data["type"];
			string mode = data["mode"];
			string protocol = data["protocol"];
			int elo = data["elo"].AsInt;
			bool useElo = data["useElo"].AsBool;

			Host host = new Host()
			{
				Name = name,
				Address = address,
				Port = port,
				MaxPlayers = maxPlayers,
				PlayerCount = playerCount,
				Comment = comment,
				Id = gameId,
				Type = gameType,
				Mode = mode,
				Protocol = protocol,
				Player = player,
				Elo = elo,
				UseElo = useElo
			};

			hosts.Add(host);
			Log(string.Format($"Host [{address}] registered on port [{port}] with name [{name}]"));
		}

		private void Update(NetworkingPlayer player, JSONNode data)
		{
			int playerCount = data["playerCount"].AsInt;
			string comment = data["comment"];
			string gameType = data["type"];
			string mode = data["mode"];
			ushort port = data["port"].AsUShort;

			string address = ((IPEndPoint)player.TcpClientHandle.Client.RemoteEndPoint).Address.ToString();
			for (int i = 0; i < hosts.Count; i++)
			{
				if (hosts[i].Address == address && hosts[i].Port == port)
				{
					Host host = hosts[i];
					host.Comment = comment;
					host.Type = gameType;
					host.Mode = mode;
					host.PlayerCount = playerCount;

					hosts[i] = host;
					break;
				}
			}
		}

		private void Get(NetworkingPlayer player, JSONNode data)
		{
			// Pull the game id and the filters from request
			string gameId = data["id"];
			string gameType = data["type"];
			string gameMode = data["mode"];
			int playerElo = data["elo"].AsInt;
			if (_playerRequests.ContainsKey(player.Ip))
				_playerRequests[player.Ip]++;
			else
				_playerRequests.Add(player.Ip, 1);

			int delta = _playerRequests[player.Ip];

			// Get only the list that has the game ids
			List<Host> filter = (from host in hosts where host.Id == gameId select host).ToList();

			// If "any" is supplied use all the types for this game id otherwise select only matching types
			if (gameType != "any")
				filter = (from host in filter where host.Type == gameType select host).ToList();

			// If "all" is supplied use all the modes for this game id otherwise select only matching modes
			if (gameMode != "all")
				filter = (from host in filter where host.Mode == gameMode select host).ToList();

			// Prepare the data to be sent back to the client
			JSONNode sendData = JSONNode.Parse("{}");
			JSONArray filterHosts = new JSONArray();

			foreach (Host host in filter)
			{
				if (host.UseElo)
				{
					if (host.PlayerCount >= host.MaxPlayers) //Ignore servers with max capacity
						continue;

					if (_eloRangeSet && (playerElo > host.Elo - (EloRange * delta) &&
						playerElo < host.Elo + (EloRange * delta)))
					continue;
				}

				JSONClass hostData = new JSONClass();
				hostData.Add("name", host.Name);
				hostData.Add("address", host.Address);
				hostData.Add("port", new JSONData(host.Port));
				hostData.Add("comment", host.Comment);
				hostData.Add("type", host.Type);
				hostData.Add("mode", host.Mode);
				hostData.Add("players", new JSONData(host.PlayerCount));
				hostData.Add("maxPlayers", new JSONData(host.MaxPlayers));
				hostData.Add("protocol", host.Protocol);
				hostData.Add("elo", new JSONData(host.Elo));
				hostData.Add("useElo", new JSONData(host.UseElo));
				hostData.Add("eloDelta", new JSONData(delta));
				filterHosts.Add(hostData);
			}

			if (filterHosts.Count > 0)
				_playerRequests.Remove(player.Ip);

			sendData.Add("hosts", filterHosts);

			// Send the list of hosts (if any) back to the requesting client
			server.Send(player.TcpClientHandle, Text.CreateFromString(server.Time.Timestep, sendData.ToString(), false, Receivers.Target, MessageGroupIds.MASTER_SERVER_GET, true));
		}

		public void Dispose()
		{
			server.Disconnect(true);
			IsRunning = false;
		}
	}
}