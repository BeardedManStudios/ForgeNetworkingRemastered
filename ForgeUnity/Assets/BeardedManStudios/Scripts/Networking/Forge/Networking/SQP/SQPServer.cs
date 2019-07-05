using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.SQP
{
	public class SQPServer : BaseSQP
	{
		public ServerInfo.Data ServerInfoData
		{
			get { return serverInfo.ServerInfoData; }
			set { serverInfo.ServerInfoData = value; }
		}

		private System.Random random;
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

			random = new System.Random();
		}

		public void Update()
		{
			if (socket.Poll(0, SelectMode.SelectRead))
			{
				buffer.SetSize(MAX_PACKET_SIZE);
				buffer.ResetPointer();
				int read = socket.ReceiveFrom(buffer.byteArr, buffer.byteArr.Length, SocketFlags.None, ref endpoint);
				if (read > 0)
				{
					var header = new QueryHeader();
					header.Deserialize(buffer);

					var type = (MessageType) header.Type;

					switch (type)
					{
						case MessageType.ChallengeRequest:
							{
								if (!outstandingTokens.ContainsKey(endpoint))
								{
									buffer.Clear();
									// Issue a challenge token to this endpoint
									uint token = GetNextToken();

									var response = new ChallengeResponse();
									response.Header.ChallengeId = token;
									response.Serialize(ref buffer);

									var data = buffer.CompressBytes();
									socket.SendTo(data, data.Length, SocketFlags.None, endpoint);

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
								request.Deserialize(buffer);

								if (request.Header.ChallengeId != token)
									return;

								if ((ChunkType)request.RequestedChunks == ChunkType.ServerInfo)
								{
									var response = serverInfo;
									response.QueryHeader.Header.ChallengeId = token;

									buffer.Clear();
									response.Serialize(ref buffer);
									var data = buffer.CompressBytes();
									socket.SendTo(data, data.Length, SocketFlags.None, endpoint);
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
