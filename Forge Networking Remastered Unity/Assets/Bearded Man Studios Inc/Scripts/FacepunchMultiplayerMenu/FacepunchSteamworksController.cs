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

	private void OnApplicationQuit()
	{
		if (facepunchSteamworksController == this)
			SteamClient.Shutdown();
	}
}
#endif
