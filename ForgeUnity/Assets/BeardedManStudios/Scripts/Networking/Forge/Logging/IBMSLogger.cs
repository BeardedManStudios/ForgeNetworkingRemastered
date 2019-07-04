namespace BeardedManStudios.Forge.Logging
{
	public interface IBMSLogger
	{
        void Log(string log);
        void LogFormat(string log, params object[] args);
        void LogWarning(string log);
        void LogWarningFormat(string log, params object[] args);
        void LogException(string log);
        void LogExceptionFormat(string log, params object[] args);
	}
}
