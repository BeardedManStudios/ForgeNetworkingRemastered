using System;
using Forge.Engine;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public class ForgeUnityLogger : IForgeLogger
	{
		public void Log(string message)
		{
			Debug.Log(message);
		}

		public void LogError(string error)
		{
			Debug.LogError(error);
		}

		public void LogException(Exception exception)
		{
			Debug.LogException(exception);
		}

		public void LogWarning(string warning)
		{
			Debug.LogWarning(warning);
		}
	}
}
