using BeardedManStudios;
using BeardedManStudios.Forge.Logging;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.SimpleJSON;
using System;
using System.Collections.Generic;

namespace ConsoleTestProgram
{
	class MainClass
	{
		private static NetWorker networkHandle = null;

		public static void Main(string[] args)
		{
			BeardedManStudios.Templating.TemplateSystem template = new BeardedManStudios.Templating.TemplateSystem(tmp);

			template.AddVariable("className", "NetworkCameraNetworkObject");
			template.AddVariable("identity", "1");
			template.AddVariable("bitwiseSize", "1");
			List<object[]> variables = new List<object[]>();
			template.AddVariable("variables", variables.ToArray());

			string end = template.Parse();

			Program1();
		}

		#region Program1
		private static void Program1()
		{
			ConsoleBehavior obj = null;

			string serverIP = "127.0.0.1";
			//string hostIP = "0.0.0.0";
			//ushort port = NetWorker.DEFAULT_PORT;
			//ushort masterServerPort = 15940;
			//string natHost = "0.0.0.0";

			BMSLog.Instance.RegisterLoggerService(new ConsoleLogger());

			Console.Write("server or client: ");
			string serverOrClient = Console.ReadLine().ToLower();

			if (serverOrClient == "client" || serverOrClient == "c")
			{
				networkHandle = new TCPClient();
				((TCPClient)networkHandle).Connect(serverIP/*, port*/);
			}
			else if (serverOrClient == "server" || serverOrClient == "s")
			{
				networkHandle = new TCPServer(32);
				((TCPServer)networkHandle).Connect();
				//RegisterOnMasterServer(networkHandle, 32, serverIP, masterServerPort);
			}

			//if (serverOrClient == "client" || serverOrClient == "c")
			//{
			//	networkHandle = new UDPClient();
			//	((UDPClient)networkHandle).Connect(serverIP/*, port, natHost*/);
			//}
			//else if (serverOrClient == "server" || serverOrClient == "s")
			//{
			//	networkHandle = new UDPServer(32);
			//	((UDPServer)networkHandle).Connect(/*hostIP, port, natHost*/);
			//}
			else
			{
				Console.WriteLine("Invalid");
				return;
			}

			networkHandle.textMessageReceived += ReadTextMessage;
			networkHandle.binaryMessageReceived += ReadBinaryMessage;
			NetworkObject.Factory = new NetworkObjectFactory();
			networkHandle.objectCreated += (NetworkObject target) =>
			{
				if (target is ConsoleNetworkObject)
				{
					obj = new ConsoleDerivedNetworkObject();
					obj.Initialize(target);
				}
			};

			networkHandle.serverAccepted += () =>
			{
				if (networkHandle is IClient)
					new ConsoleNetworkObject(networkHandle);
			};

			while (true)
			{
				Console.Write("Enter a message: ");
				string message = Console.ReadLine();

				if (message == "disconnect")
					networkHandle.Disconnect(false);
				else if (message == "exit")
					break;
				else if (message == "file")
				{
					BMSByte data = new BMSByte();
					data.Clone(System.IO.File.ReadAllBytes("testSend.txt"));
					data.InsertRange(0, new byte[1] { 212 });

					if (networkHandle is TCPServer)
						((TCPServer)networkHandle).SendAll(new Binary(networkHandle.Time.Timestep, false, data, Receivers.All, 55, true));
					else if (networkHandle is TCPClient)
						((TCPClient)networkHandle).Send(new Binary(networkHandle.Time.Timestep, true, data, Receivers.All, 55, true));
					else if (networkHandle is BaseUDP)
						((BaseUDP)networkHandle).Send(new Binary(networkHandle.Time.Timestep, false, data, Receivers.All, 55, false), false);
				}
				else if (message == "rpc")
					obj.networkObject.SendRpc(ConsoleBehavior.RPC_HELLO_WORLD, Receivers.AllBuffered, null, "World!");
				else if (message.StartsWith("set num to "))
					obj.networkObject.Num = int.Parse(message.Substring("set name to ".Length));
				else if (message == "num")
					Console.WriteLine("Number is currently " + obj.networkObject.Num);
				else
				{
					if (networkHandle is TCPServer)
						obj.networkObject.SendRpc(ConsoleBehavior.RPC_HELLO_WORLD, Receivers.Others, message);
					//                        ((TCPServer)networkHandle).SendAll(Text.CreateFromString(networkHandle.Time.Timestep, message, false, Receivers.All, 55, true));
					else if (networkHandle is TCPClient)
						((TCPClient)networkHandle).Send(Text.CreateFromString(networkHandle.Time.Timestep, message, true, Receivers.All, 55, true));
					else if (networkHandle is BaseUDP)
						((BaseUDP)networkHandle).Send(Text.CreateFromString(networkHandle.Time.Timestep, message, false, Receivers.All, 55, false), true);
				}
			}
		}

		private static void ReadTextMessage(NetworkingPlayer player, Text frame)
		{
			Console.WriteLine("");
			Console.WriteLine("Read: " + frame.ToString());
			Console.Write("Enter a message: ");
		}

		private static void RegisterOnMasterServer(NetWorker networker, int maxPlayers, string masterServerHost, ushort masterServerPort)
		{
			// The Master Server communicates over TCP
			TCPClient client = new TCPClient();

			// Once this client has been accepted by the master server it should send it's get request
			client.serverAccepted += () =>
			{
				try
				{
					Console.WriteLine("\nAccepted On Master Server");
					// Create the get request with the desired filters
					JSONNode sendData = JSONNode.Parse("{}");
					JSONClass registerData = new JSONClass();
					registerData.Add("id", "myGame");
					registerData.Add("name", "Forge Game");
					registerData.Add("port", new JSONData(networker.Port));
					registerData.Add("playerCount", new JSONData(0));
					registerData.Add("maxPlayers", new JSONData(maxPlayers));
					registerData.Add("comment", "Demo comment...");
					registerData.Add("type", "Deathmatch");
					registerData.Add("mode", "Teams");
					registerData.Add("protocol", networker is BaseUDP ? "udp" : "tcp");

					sendData.Add("register", registerData);

					BeardedManStudios.Forge.Networking.Frame.Text temp = BeardedManStudios.Forge.Networking.Frame.Text.CreateFromString(client.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_REGISTER, true);

					// Send the request to the server
					client.Send(temp);
				}
				catch
				{
					// If anything fails, then this client needs to be disconnected
					client.Disconnect(true);
					client = null;
				}
			};

			client.Connect(masterServerHost, masterServerPort);

			client.disconnected += () =>
			{
				client.Disconnect(false);
				Console.WriteLine("Master server disconnected");
			};
			//Networker.disconnected += () =>
			//{
			//	client.Disconnect(false);
			//	MasterServerNetworker = null;
			//};
		}

		private static void ReadBinaryMessage(NetworkingPlayer player, Binary frame)
		{
			BMSByte data = new BMSByte();
			data.Clone(frame.GetData());

			if (data[0] == 212)
			{
				data.MoveStartIndex(1);
				System.IO.File.WriteAllBytes("testRead.txt", data.CompressBytes());
				Console.WriteLine("Wrote file from network");
			}
		}

		private static void TestRpc(object[] args)
		{
			Console.WriteLine("Hello {0}!", args[0]);
		}
		#endregion

		private static string tmp = @"using System;
using UnityEngine;
using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;

namespace BeardedManStudios.Forge.Networking.Generated
{
	public class >:className:< : NetworkObject
	{
		public const int IDENTITY = >:identity:<;

		private byte[] _dirtyFields = new byte[>:bitwiseSize:<];

		>:FOREVERY variables:<
		private >:[0]:< _>:[1]:<;
		public event FieldEvent<>:[0]:<> >:[1]:<Changed;
		private Interpolate>:[0]:< >:[1]:<Interpolation = new Interpolate>:[0]:<() { lerpT = >:[3]:<, enabled = >:[2]:< };
		public >:[0]:< >:[1]:<
		{
			get { return _>:[1]:<; }
			set
			{
				// Don't do anything if the value is the same
				if (_>:[1]:< == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[>:[5]:<] |= >:[4]:<;
				_>:[1]:< = value;
				hasDirtyFields = true;
			}
		}

		>:ENDFOREVERY:<

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			>:FOREVERY variables:<
			UnityObjectMapper.Instance.MapBytes(data, _>:[1]:<);
			>:ENDFOREVERY:<

			return data;
		}

		protected override void ReadPayload(BMSByte payload)
		{
			>:FOREVERY variables:<
			_>:[1]:< = UnityObjectMapper.Instance.Map<>:[0]:<>(payload);
			>:[1]:<Interpolation.current = _>:[1]:<;
			>:[1]:<Interpolation.target = _>:[1]:<;
			if (>:[1]:<Changed != null) >:[1]:<Changed(_>:[1]:<);
			>:ENDFOREVERY:<
		}

		protected override BMSByte SerializeDirtyFields()
		{
			BMSByte data = new BMSByte();
			data.Append(_dirtyFields);

			>:FOREVERY variables:<
			if ((>:[4]:< & _dirtyFields[>:[5]:<]) != 0)
				UnityObjectMapper.Instance.MapBytes(data, _>:[1]:<);
			>:ENDFOREVERY:<

			return data;
		}

		protected override void ReadDirtyFields(BMSByte data)
		{
			byte[] readFlags = new byte[_dirtyFields.Length];
			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readFlags, 0, readFlags.Length);
			data.MoveStartIndex(readFlags.Length);

			>:FOREVERY variables:<
			if ((>:[4]:< & readFlags[>:[5]:<]) != 0)
			{
				>:[1]:<Interpolation.target = UnityObjectMapper.Instance.Map<>:[0]:<>(data);
				if (>:[1]:<Changed != null) >:[1]:<Changed(_>:[1]:<);
			}
			>:ENDFOREVERY:<
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			>:FOREVERY variables:<
			if (>:[1]:<Interpolation.enabled && >:[1]:<Interpolation.current != >:[1]:<Interpolation.target)
			{
				_>:[1]:< = >:[1]:<Interpolation.Interpolate();
				if (>:[1]:<Changed != null) >:[1]:<Changed(>:[1]:<);
			}
			>:ENDFOREVERY:<
		}

		public >:className:<(NetWorker networker, INetworkBehavior networkBehavior = null) : base(networker, networkBehavior) { }
		public >:className:<(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}";
	}
}