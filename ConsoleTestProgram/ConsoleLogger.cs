using BeardedManStudios.Forge.Logging;
using System;

public class ConsoleLogger : IBMSLogger
{
	//private string BMSLogs;

	public void Log(string log)
	{
		Console.WriteLine("DEBUG:" + log);
	}

	public void LogFormat(string log, params object[] args)
	{
		string logInfo = string.Format(log, args);
		Console.WriteLine("DEBUG:" + logInfo);
	}

	public void LogWarning(string log)
	{
		Console.WriteLine("WARNING: " + log);
	}

	public void LogWarningFormat(string log, params object[] args)
	{
		string logInfo = string.Format(log, args);
		Console.WriteLine("WARNING: " + log);
	}

	public void LogException(string log)
	{
		Console.WriteLine("ERROR: " + log);
	}

	public void LogExceptionFormat(string log, params object[] args)
	{
		string logInfo = string.Format(log, args);
		Console.WriteLine("ERROR: " + log);
	}
}