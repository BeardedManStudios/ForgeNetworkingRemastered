using System;

namespace Forge.Engine
{
	public class ForgeConsoleLogger : IForgeLogger
	{
		public void Log(string message)
		{
			Console.WriteLine($"DBG: {message}");
		}

		public void LogWarning(string warning)
		{
			Console.WriteLine($"WRN: {warning}");
		}

		public void LogError(string error)
		{
			Console.WriteLine($"ERR: {error}");
		}

		public void LogException(Exception exception)
		{
			Console.WriteLine($"EXCEPTION: {exception.Message}\n{exception.StackTrace}");
		}
	}
}
