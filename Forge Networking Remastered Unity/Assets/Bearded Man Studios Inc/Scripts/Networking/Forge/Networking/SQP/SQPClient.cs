using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using BeardedManStudios.Forge.Logging;

namespace BeardedManStudios.Forge.Networking.SQP
{
	public enum ClientState
	{
		Idle,
		WaitingForChallange,
		WaitingForResponse,
	}

	public class SQPClient : BaseSQP
	{
		public static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

		private List<Query> Queries = new List<Query>();

		public SQPClient()
		{
			IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
			InitSocket(localEP);
			stopwatch.Start();
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
				int read = 0;
				buffer.SetSize(MAX_PACKET_SIZE); // This should just reset the internal size to MAX_PACKET_SIZE
				buffer.ResetPointer();
				try
				{
					read = socket.ReceiveFrom(buffer.byteArr, buffer.byteArr.Length, SocketFlags.None,
						ref endpoint);
				}
				catch (SocketException e)
				{
					// If the exception was a connection reset then ignore it otherwise rethrow it.
					if (e.SocketErrorCode != SocketError.ConnectionReset)
						throw;
				}

				if (read > 0)
				{
					var header = new QueryHeader();
					header.Deserialize(buffer);

					buffer.ResetPointer();
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
									query.ServerInfo.Deserialize(buffer);

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

					// If we have not received anything from a server then let's reset it's state to Idle
					if (now - query.StartTime > QUERY_TIMEOUT)
					{
						query.State = ClientState.Idle;
						query.ValidResult = false;
					}
				}
			}
		}

		/// <summary>
		/// Sends a request for a challenge token to the server.
		/// </summary>
		/// <param name="query"></param>
		public void SendChallengeRequest(Query query)
		{
			if (query.State != ClientState.Idle)
				return;

			query.StartTime = SQPClient.stopwatch.ElapsedMilliseconds;

			buffer.Clear();
			var request = new ChallengeRequest();
			request.Serialize(ref buffer);

			query.State = ClientState.WaitingForChallange;

			var data = buffer.CompressBytes();
			socket.SendTo(data, data.Length, SocketFlags.None, query.Server);
		}

		/// <summary>
		/// Send a <see cref="ServerInfo"/> <see cref="Query"/> to the server.
		/// </summary>
		/// <param name="query">The <see cref="Query"/> to send</param>
		private void SendServerInfoQuery(Query query)
		{
			buffer.Clear();
			var request = new QueryRequest();
			request.Header.ChallengeId = query.ChallengeId;
			request.RequestedChunks = (byte) ChunkType.ServerInfo;

			request.Serialize(ref buffer);

			query.State = ClientState.WaitingForResponse;

			var data = buffer.CompressBytes();
			socket.SendTo(data, data.Length, SocketFlags.None, query.Server);
		}
	}
}
