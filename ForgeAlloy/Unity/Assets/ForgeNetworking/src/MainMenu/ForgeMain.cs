using Forge.Engine;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public class ForgeMain : MonoBehaviour
	{
		[Forge(typeof(IEngineContainer))]
		private IEngineContainer _engineContainer;
		[Forge(typeof(IUDPServerConstructor))]
		private IUDPServerConstructor _serverHostConstructor;

		private void Awake()
		{
			ThrowIfNull(_engineContainer);
			ThrowIfNull(_serverHostConstructor);
		}

		public void Host()
		{
			_serverHostConstructor.CreateAndStartServer(_engineContainer);
		}

		private void ThrowIfNull<T>(T obj)
		{
			if (obj == null)
				throw new System.Exception($"Expected { obj } to implement { typeof(T) }");
		}
	}
}
