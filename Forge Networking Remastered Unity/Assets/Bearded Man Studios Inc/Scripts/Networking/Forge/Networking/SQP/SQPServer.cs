using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace BeardedManStudios.Forge.Networking.SQP
{
	public class SQPServer
	{
		public ServerInfo.Data ServerInfoData
		{
			get { return serverInfo.ServerInfoData; }
			set { serverInfo.ServerInfoData = value; }
		}

		private Socket socket;
		private System.Random random;
		private BMSByte buffer = new BMSByte();
		private EndPoint endpoint = new IPEndPoint(0, 0);
		private ServerInfo serverInfo = new ServerInfo();

		/// <summary>
		/// Stores the endpoints and the matching sent challenge tokens
		/// </summary>
		private Dictionary<EndPoint, uint> outstandingTokens = new Dictionary<EndPoint, uint>();

		public SQPServer(ushort port)
		{
			if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
				throw new ArgumentOutOfRangeException("port");

			IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
			InitSocket(localEP);

			buffer.SetSize(1472);
			random = new System.Random();
		}

		public void Update()
		{
			if (socket.Poll(0, SelectMode.SelectRead))
			{
				buffer.Clear();
				int read = socket.ReceiveFrom(buffer.byteArr, buffer.byteArr.Length, SocketFlags.None, ref endpoint);
				if (read > 0)
				{
					var header = new QueryHeader();
					header.Deserialize(ref buffer);

					var type = (MessageType) header.Type;

					switch (type)
					{
						case MessageType.ChallengeRequest:
							{
								if (!outstandingTokens.ContainsKey(endpoint))
								{
									// Issue a challenge token to this endpoint
									uint token = GetNextToken();

									buffer.Clear();
									var response = new ChallengeResponse();
									response.Header.ChallengeId = token;
									response.Serialize(ref buffer);

									socket.SendTo(buffer.CompressBytes(), buffer.Size, SocketFlags.None, endpoint);

									outstandingTokens.Add(endpoint, token);
								}
							}
							break;
						case MessageType.QueryRequest:
							{
								uint token;
								if (!outstandingTokens.TryGetValue(endpoint, out token))
									return;

								outstandingTokens.Remove(endpoint);

								buffer.ResetPointer();
								var request = new QueryRequest();
								request.Deserialize(ref buffer);

								if ((ChunkType)request.RequestedChunks == ChunkType.ServerInfo)
								{
									buffer.Clear();
									var response = serverInfo;
									response.QueryHeader.Header.ChallengeId = token;

									response.Serialize(ref buffer);
									socket.SendTo(buffer.CompressBytes(), buffer.Size, SocketFlags.None, endpoint);
								}
							}
							break;
						default:
								break;
					}
				}
			}
		}

		/// <summary>
		/// Initialize the socket
		/// </summary>
		/// <param name="ep"></param>
		private void InitSocket(EndPoint ep)
		{
			if (socket != null)
			{
				socket.Close();
				socket = null;
			}

			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Blocking = false;

			socket.Bind(ep);
		}

		/// <summary>
		/// Generate a random uint.
		/// </summary>
		/// <remarks>See https://stackoverflow.com/a/17080161 for explanation on why the random uint is generated this way</remarks>
		/// <returns></returns>
		private uint GetNextToken()
		{
			uint thirtyBits = (uint)random.Next(1 << 30);
			uint twoBits = (uint)random.Next(1 << 2);
			return (thirtyBits << 2) | twoBits;
		}
	}
}
