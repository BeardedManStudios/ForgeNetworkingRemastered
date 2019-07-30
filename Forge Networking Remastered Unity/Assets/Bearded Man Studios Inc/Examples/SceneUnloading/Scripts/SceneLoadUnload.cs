using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadUnload : MonoBehaviour
{
	private void Update()
	{
		if (!NetworkManager.Instance || !NetworkManager.Instance.IsServer)
			return;

		bool bLoadUnloadScene01 = Input.GetKeyDown(KeyCode.Alpha1);
		bool bLoadUnloadScene02 = Input.GetKeyDown(KeyCode.Alpha2);

		if(bLoadUnloadScene01)
		{
			string sceneName = "Additive Scene 01";
			Scene scene = SceneManager.GetSceneByName(sceneName);
			if (!scene.isLoaded)
			{
				SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			}
			else
			{
				NetworkManager.Instance.UnloadSceneAdditive(sceneName);
			}
		}

		if (bLoadUnloadScene02)
		{
			string sceneName = "Additive Scene 02";
			Scene scene = SceneManager.GetSceneByName(sceneName);
			if (!scene.isLoaded)
			{
				SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			}
			else
			{
				NetworkManager.Instance.UnloadSceneAdditive(sceneName);
			}
		}
	}
}
