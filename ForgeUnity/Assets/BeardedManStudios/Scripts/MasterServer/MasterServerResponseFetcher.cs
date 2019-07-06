using System;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.SimpleJSON;

namespace BeardedManStudios.Forge.Networking.MasterServer
{
	public class MasterServerResponseFetcher
	{
		public struct FetchRequestData
		{
			public string gameId;
			public string gameType;
			public string gameMode;
			public string hostAddress;
			public Action<MasterServerResponse> completedCallback;
			public int elo;
			public ushort hostPort;
		}

		private FetchRequestData fetchData;
		private TCPMasterClient client;

		public static MasterServerResponseFetcher CreateWithData(FetchRequestData fetchData)
		{
			return new MasterServerResponseFetcher()
			{
				fetchData = fetchData
			};
		}

		private MasterServerResponseFetcher()
		{
			client = null;
		}

		public void SendFetchRequest()
		{
			SetupTCPClientForRequest();
			try
			{
				client.Connect(fetchData.hostAddress, fetchData.hostPort);
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
			client.serverAccepted += SendInformationFetchRequest;
			client.textMessageReceived += ProcessResponseFromServer;
		}

		private void SendInformationFetchRequest(NetWorker sender)
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
			getData.Add("id", fetchData.gameId);
			getData.Add("type", fetchData.gameType);
			getData.Add("mode", fetchData.gameMode);
			getData.Add("elo", new JSONData(fetchData.elo));
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
			if (fetchData.completedCallback != null)
			{
				MainThreadManager.Run(() =>
				{
					fetchData.completedCallback(null);
				});
			}
		}

		private void ExecuteSuccessCompletedCallbackAs(MasterServerResponse response)
		{
			if (fetchData.completedCallback != null)
			{
				MainThreadManager.Run(() =>
				{
					fetchData.completedCallback(response);
				});
			}
		}
	}
}
