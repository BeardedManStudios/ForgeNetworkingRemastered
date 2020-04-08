#if UNITY_ENGINE
using UnityEngine;

namespace Forge
{
	public class ForgeInitializer : MonoBehaviour
	{
		private void Awake()
		{
			ForgeRegistration.Initialize();
		}
	}
}
#endif
