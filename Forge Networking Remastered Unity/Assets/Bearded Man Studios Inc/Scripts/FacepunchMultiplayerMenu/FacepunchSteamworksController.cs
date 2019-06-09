#if FACEPUNCH_STEAMWORKS
using UnityEngine;
using Steamworks;

/// <summary>
/// Singleton class to initialize Facepunch.Steamworks client and shutdown on application quit
/// </summary>
public class FacepunchSteamworksController : MonoBehaviour
{
	public static FacepunchSteamworksController Instance;

	// Steam AppId 480 is the test 'spacewar' app
	private const int STEAM_APP_ID = 480;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);

			try
			{
				SteamClient.Init(STEAM_APP_ID);
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
		if (Instance == this)
			SteamClient.Shutdown();
	}
}
#endif
