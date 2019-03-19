/*-----------------------------+-------------------------------\
|                                                              |
|                         !!!NOTICE!!!                         |
|                                                              |
|  These libraries are under heavy development so they are     |
|  subject to make many changes as development continues.      |
|  For this reason, the libraries may not be well commented.   |
|  THANK YOU for supporting forge with all your feedback       |
|  suggestions, bug reports and comments!                      |
|                                                              |
|                              - The Forge Team                |
|                                Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

namespace BeardedManStudios.Forge.Logging
{
	public class BMSLog
	{
		#region Singleton
		private static BMSLog _instance;
		public static BMSLog Instance
		{
			get { if (_instance == null) _instance = new BMSLog(); return _instance; }
		}
		#endregion

		public enum Logtype
		{
			Info,
			Warning,
			Exception
		}

		private IBMSLogger _loggerService;

		#region Public API
		public static void Log(string text)
		{
			Instance.InternalLog(Logtype.Info, text);
		}

		public static void LogFormat(string text, params object[] args)
		{
			Instance.InternalLog(Logtype.Info, text, args);
		}

		public static void LogException(System.Exception ex)
		{
			Instance.InternalLog(Logtype.Exception, string.Format("Message: {0}{1}{2}", ex.Message, System.Environment.NewLine, ex.StackTrace));
		}

		public static void LogException(string text)
		{
			Instance.InternalLog(Logtype.Exception, text);
		}

		public static void LogExceptionFormat(string text, params object[] args)
		{
			Instance.InternalLog(Logtype.Exception, text, args);
		}

		public static void LogWarning(string text)
		{
			Instance.InternalLog(Logtype.Warning, text);
		}

		public static void LogWarningFormat(string text, params object[] args)
		{
			Instance.InternalLog(Logtype.Warning, text, args);
		}

		public void RegisterLoggerService(IBMSLogger service)
		{
			_loggerService = service;
		}
		#endregion

		private void InternalLog(Logtype type, string text, params object[] args)
		{
#if !UNITY_IOS
			if (_loggerService == null)
				return;

			switch (type)
			{
				case Logtype.Info:
					if (args != null && args.Length > 0)
						_loggerService.LogFormat(text, args);
					else
						_loggerService.Log(text);
					break;
				case Logtype.Warning:
					if (args != null && args.Length > 0)
						_loggerService.LogWarningFormat(text, args);
					else
						_loggerService.LogWarning(text);
					break;
				case Logtype.Exception:
					if (args != null && args.Length > 0)
						_loggerService.LogExceptionFormat(text, args);
					else
						_loggerService.LogException(text);
					break;
			}
#endif
		}
	}
}
