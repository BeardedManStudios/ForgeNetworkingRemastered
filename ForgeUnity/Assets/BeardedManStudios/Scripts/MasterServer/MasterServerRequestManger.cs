using System;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.SimpleJSON;

namespace BeardedManStudios.Forge.Networking.MasterServer
{
	public class MasterServerRequestManger
	{
		public struct RequestData
		{
			public string gameId;
			public string gameType;
			public string gameMode;
			public string hostAddress;
			public string comment;
			public Action<MasterServerResponse> completedCallback;
			public int elo;
			public int playerCount;
			public ushort hostPort;
			public ushort hostedServerPort;
		}

		private RequestData requestData;
		private TCPMasterClient client;

		public static MasterServerRequestManger CreateWithData(RequestData fetchData)
		{
			return new MasterServerRequestManger()
			{
				requestData = fetchData
			};
		}

		private MasterServerRequestManger()
		{
			client = null;
		}

		public void SendRegisterRequest()
		{
			SetupTCPClientForRequest();
			try
			{
				client.Connect(requestData.hostAddress, requestData.hostPort);
			}
			catch (Exception ex)
			{
				BMSLogger.Instance.LogException(ex.Message);
				ExecuteFailedCompletedCallback();
			}
		}

		private void SetupTCPClientForRequest()
		{
			client = new TCPMasterClient();
			client.serverAccepted += SendRegistrationInformation;
			client.textMessageReceived += ProcessResponseFromServer;
		}

		private void SendRegistrationInformation(NetWorker sender)
		{
			try
			{
				JSONNode sendData = CreateJsonRequestData();
				var request = Text.CreateFromString(client.Time.Timestep, sendData.ToString(), true,
					Receivers.Server, MessageGroupIds.MASTER_SERVER_GET, true);
				client.Send(request);
			}
			catch
			{
				client.Disconnect(true);
				client = null;
				ExecuteFailedCompletedCallback();
			}
		}

		private JSONNode CreateJsonRequestData()
		{
			// Create the get request with the desired filters
			var sendData = JSONNode.Parse("{}");
			var getData = new JSONClass();
			getData.Add("id", requestData.gameId);
			getData.Add("type", requestData.gameType);
			getData.Add("mode", requestData.gameMode);
			getData.Add("elo", new JSONData(requestData.elo));
			sendData.Add("get", getData);
			return sendData;
		}

		private void ProcessResponseFromServer(NetworkingPlayer player, Text frame, NetWorker sender)
		{
			try
			{
				var data = JSONNode.Parse(frame.ToString());
				if (data != null && data["hosts"] != null)
					ExecuteSuccessCompletedCallbackAs(new MasterServerResponse(data["hosts"].AsArray));
				else
					ExecuteFailedCompletedCallback();
			}
			finally
			{
				if (client != null)
				{
					// If we succeed or fail the client needs to disconnect from the Master Server
					client.Disconnect(true);
					client = null;
				}
			}
		}

		private void ExecuteFailedCompletedCallback()
		{
			if (requestData.completedCallback != null)
			{
				MainThreadManager.Run(() =>
				{
					requestData.completedCallback(null);
				});
			}
		}

		private void ExecuteSuccessCompletedCallbackAs(MasterServerResponse response)
		{
			if (requestData.completedCallback != null)
			{
				MainThreadManager.Run(() =>
				{
					requestData.completedCallback(response);
				});
			}
		}

		public void SendUpdateRequest()
		{
			if (string.IsNullOrEmpty(requestData.hostAddress))
				throw new System.Exception("This server is not registered on a master server, please ensure that you are passing a master server host and port into the initialize");

			// The Master Server communicates over TCP
			var client = new TCPMasterClient();

			// Once this client has been accepted by the master server it should send it's update request
			client.serverAccepted += SendUpdateInformation;
			client.Connect(requestData.hostAddress, requestData.hostPort);
		}

		private void SendUpdateInformation(NetWorker sender)
		{
			try
			{
				var sendData = GetUpdateRequestInJSONForm();
				var request = Text.CreateFromString(client.Time.Timestep, sendData.ToString(),
					true, Receivers.Server, MessageGroupIds.MASTER_SERVER_UPDATE, true);
				client.Send(request);
			}
			finally
			{
				// If anything fails, then this client needs to be disconnected
				client.Disconnect(true);
				client = null;
			}
		}

		private JSONNode GetUpdateRequestInJSONForm()
		{
			var sendData = JSONNode.Parse("{}");
			var updateData = new JSONClass();
			updateData.Add("playerCount", new JSONData(requestData.playerCount));
			updateData.Add("comment", requestData.comment);
			updateData.Add("type", requestData.gameType);
			updateData.Add("mode", requestData.gameMode);
			updateData.Add("port", new JSONData(requestData.hostedServerPort));
			sendData.Add("update", updateData);
			return sendData;
		}
	}
}
