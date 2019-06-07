#if FACEPUNCH_STEAMWORKS
using UnityEngine;
using Steamworks;

public class FacepunchSteamworksController : MonoBehaviour
{
	public static FacepunchSteamworksController facepunchSteamworksController;

	private void Awake()
	{
		if (facepunchSteamworksController == null)
		{
			facepunchSteamworksController = this;
			DontDestroyOnLoad(gameObject);
			try
			{
				SteamClient.Init(480);
			}
			catch (System.Exception e)
			{
				Debug.Log("Could not initialize SteamClient. Exception:");
				Debug.LogException(e);
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

#if UNITY_EDITOR
	private void OnDisable()
	{
		if (facepunchSteamworksController == this)
			ShutdownSteamClient();
	}
#else
	private void OnApplicationQuit()
	{
		if (facepunchSteamworksController == this)
			ShutdownSteamClient();
	}
#endif

	private void ShutdownSteamClient()
	{
		Debug.Log("Shutting down Steam Client");
		SteamClient.Shutdown();
	}

}
#endif
