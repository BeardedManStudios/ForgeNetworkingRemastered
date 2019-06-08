#if FACEPUNCH_STEAMWORKS
using UnityEngine;
using Steamworks;

/// <summary>
/// Singleton class to initialize Facepunch.Steamworks client and shotdown on application quit
/// </summary>
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
				// Use your Steam appID here. 480 is the test SpaceWar app
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

	private void OnApplicationQuit()
	{
		if (facepunchSteamworksController == this)
			SteamClient.Shutdown();
	}
}
#endif
