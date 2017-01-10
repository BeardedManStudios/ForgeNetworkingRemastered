#if !UNITY_WEBGL && !WINDOWS_UWP
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Unity.Modules;
#endif

using UnityEngine;

public class VoipStarter : MonoBehaviour
{
#if !UNITY_WEBGL && !WINDOWS_UWP
	private VOIP voip;
	public string hostAddress;
	public ushort hostPort;
	public bool isLocalTesting;
	public VOIP.Quality MicQuality = VOIP.Quality.High;

	public void Start()
	{
		voip = new VOIP(0.0f, MicQuality);
		voip.IsLocalTesting = isLocalTesting;

		if (NetworkManager.Instance.IsServer)
			voip.StartServer(32);
		else
			voip.StartClient(hostAddress, hostPort);
	}

	public void Stop()
	{
		voip.StopSending();
	}

	private void Update()
	{
		voip.Update();
	}

	private void OnApplicationQuit()
	{
		Stop();
	}
#endif
}