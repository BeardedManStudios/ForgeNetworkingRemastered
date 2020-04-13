using System;

namespace Forge.Engine
{
	public interface IForgeLogger
	{
		void Log(string message);
		void LogWarning(string warning);
		void LogError(string error);
		void LogException(Exception exception);
	}
}
