using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace BeardedManStudios.Forge.Networking.SQP
{
	public enum ClientState
	{
		Idle,
		WaitingForChallange,
		WaitingForResponse,
	}

	public class SQPClient
	{
		protected const int QUERY_TIMEOUT = 3000;
		public static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

		private Socket socket;
		private BMSByte buffer = new BMSByte();
		private EndPoint endpoint = new IPEndPoint(0, 0);
		private List<Query> Queries = new List<Query>();

		public SQPClient()
		{
			IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
			InitSocket(localEP);

			buffer.SetSize(1472);
		}

		/// <summary>
		/// Get the SQP <see cref="Query"/> for a given server endpoint
		/// </summary>
		/// <param name="server">The <see cref="IPEndPoint"/> of the server</param>
		/// <returns></returns>
		public Query GetQuery(IPEndPoint server)
		{
			Query q = null;
			foreach (var query in Queries)
			{
				if (query.State == ClientState.Idle && query.Server == null)
				{
					q = query;
					break;
				}
			}

			if (q == null)
			{
				q = new Query();
				Queries.Add(q);
			}

			q.Init(server);

			return q;
		}

		public void Update()
		{
			if (socket.Poll(0, SelectMode.SelectRead))
			{
				int read = socket.ReceiveFrom(buffer.byteArr, buffer.Size, SocketFlags.None, ref endpoint);
				if (read > 0)
				{
					var header = new QueryHeader();
					header.Deserialize(ref buffer);

					foreach (var query in Queries)
					{
						if (query.Server == null || !endpoint.Equals(query.Server))
							continue;

						switch (query.State)
						{
							case ClientState.Idle:
								break;
							case ClientState.WaitingForChallange:
								if ((MessageType) header.Type == MessageType.ChallengeResponse)
								{
									query.ChallengeId = header.ChallengeId;
									query.RTT = SQPClient.stopwatch.ElapsedMilliseconds - query.StartTime;
									query.StartTime = SQPClient.stopwatch.ElapsedMilliseconds;
									SendServerInfoQuery(query);
								}
								break;
							case ClientState.WaitingForResponse:
								if ((MessageType) header.Type == MessageType.QueryResponse)
								{
									buffer.ResetPointer();
									query.ServerInfo.Deserialize(ref buffer);

									query.RTT =
										(query.RTT + (SQPClient.stopwatch.ElapsedMilliseconds - query.StartTime)) / 2;

									query.ValidResult = true;
									query.State = ClientState.Idle;
								}

								break;
							default:
								break;
						}
					}
				}
			}

			foreach (var query in Queries)
			{
				if (query.State != ClientState.Idle)
				{
					var now = SQPClient.stopwatch.ElapsedMilliseconds;
					if (now - query.StartTime > QUERY_TIMEOUT)
					{
						query.State = ClientState.Idle;
					}
				}
			}
		}

		public void SendChallengeRequest(Query query)
		{
			if (query.State != ClientState.Idle)
				return;

			query.StartTime = SQPClient.stopwatch.ElapsedMilliseconds;

			buffer.Clear();
			var request = new ChallengeRequest();
			request.Serialize(ref buffer);

			socket.SendTo(buffer.CompressBytes(), buffer.Size, SocketFlags.None, query.Server);
			query.State = ClientState.WaitingForChallange;
		}

		/// <summary>
		/// Send a <see cref="ServerInfo"/> <see cref="Query"/> to the server.
		/// </summary>
		/// <param name="query">The <see cref="Query"/> to send</param>
		private void SendServerInfoQuery(Query query)
		{
			var request = new QueryRequest();
			request.Header.ChallengeId = query.ChallengeId;
			request.RequestedChunks = (byte) ChunkType.ServerInfo;

			buffer.Clear();
			request.Serialize(ref buffer);

			query.State = ClientState.WaitingForResponse;
			socket.SendTo(buffer.CompressBytes(), buffer.Size, SocketFlags.None, query.Server);
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
	}
}
