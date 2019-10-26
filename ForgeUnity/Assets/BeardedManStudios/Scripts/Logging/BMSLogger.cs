﻿using BeardedManStudios.Forge.Logging;
using UnityEngine;

public class BMSLogger : MonoBehaviour, IBMSLogger
{
	#region Singleton

	private static BMSLogger _instance;

	public static BMSLogger Instance
	{
		get
		{
			if (_instance == null)
				Init();
			return _instance;
		}
	}

	#endregion

	#region Public Data

	public bool LoggerVisible;
	public bool LogToFile;

	#endregion

	#region Private Data

	private bool showLogger = false;
	private static string filepath;
	private string BMSLogs;
	private object Mutex = new object();
	private Vector2 ScrollPos;
	private const string SAVE_FILE_DIRECTORY_NAME = "Logs/";
	private const string SAVE_FILE_NAME = "bmslog.txt";

	#endregion

	#region Runtime

	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		if (_instance != null)
			return;

#if !UNITY_IOS
		string directory = Application.dataPath + "/" + SAVE_FILE_DIRECTORY_NAME;
		filepath = directory + SAVE_FILE_NAME;
		if (!System.IO.Directory.Exists(directory))
			System.IO.Directory.CreateDirectory(directory);
		if (!System.IO.File.Exists(filepath))
		{
			using (System.IO.File.Create(filepath))
			{
			}
		}
#endif

		GameObject prefab = Resources.Load<GameObject>("BMSLogger");
		if (prefab != null)
		{
			BMSLogger comp = prefab.GetComponent<BMSLogger>();
			_instance = new GameObject("BMSLogger", typeof(BMSLogger)).GetComponent<BMSLogger>();
			_instance.LoggerVisible = comp.LoggerVisible;
			_instance.LogToFile = comp.LogToFile;
		}
		else
			_instance = new GameObject("BMSLogger", typeof(BMSLogger)).GetComponent<BMSLogger>();

		if (_instance.LogToFile)
			Debug.LogFormat("Logging to file: {0}", filepath);
	}

	#endregion

	#region Public API

	public static void DebugLog(string log)
	{
		Instance.Log(log);
	}

	public void Log(string log)
	{
		PutLogInFile(BMSLog.Logtype.Info, log);
		BMSLogs += log + System.Environment.NewLine;

		if (!LoggerVisible)
			return;

		Debug.Log(log);
	}

	public void LogFormat(string log, params object[] args)
	{
		string logInfo = string.Format(log, args);
		PutLogInFile(BMSLog.Logtype.Info, logInfo);
		BMSLogs += logInfo + System.Environment.NewLine;

		if (!LoggerVisible)
			return;

		Debug.Log(logInfo);
	}

	public void LogWarning(string log)
	{
		string coloredWarning = $"<color=yellow>{log}</color>";
		PutLogInFile(BMSLog.Logtype.Warning, log);
		BMSLogs += coloredWarning + System.Environment.NewLine;

		if (!LoggerVisible)
			return;

		Debug.LogWarning(log);
	}

	public void LogWarningFormat(string log, params object[] args)
	{
		string logInfo = string.Format(log, args);
		string coloredWarning = $"<color=yellow>{logInfo}</color>";
		PutLogInFile(BMSLog.Logtype.Warning, logInfo);
		BMSLogs += coloredWarning + System.Environment.NewLine;

		if (!LoggerVisible)
			return;

		Debug.LogWarning(logInfo);
	}

	public void LogException(string log)
	{
		string coloredError = $"<color=red>{log}</color>";
		Debug.LogError(log);
		PutLogInFile(BMSLog.Logtype.Exception, log);
		BMSLogs += coloredError + System.Environment.NewLine;
	}

	public void LogExceptionFormat(string log, params object[] args)
	{
		string logInfo = string.Format(log, args);
		string coloredError = $"<color=red>{logInfo}</color>";
		Debug.LogError(logInfo);
		PutLogInFile(BMSLog.Logtype.Exception, logInfo);
		BMSLogs += coloredError + System.Environment.NewLine;
	}

	#endregion

	#region Unity API

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			BMSLogs = string.Empty;
			ScrollPos = Vector2.zero;
			DontDestroyOnLoad(_instance.gameObject);
			BMSLog.Instance.RegisterLoggerService(_instance);
		}
	}

	private void Start()
	{
		if (LogToFile)
			BMSLog.Log("========= START RUN =========");
	}

	private void OnGUI()
	{
		if (!LoggerVisible)
			return;

		showLogger = GUILayout.Toggle(showLogger, string.Empty);

		if (!showLogger)
			return;

		int w = Screen.width, h = Screen.height;

		GUIStyle style = GUI.skin.textArea;
		style.alignment = TextAnchor.LowerLeft;
		style.fontSize = h * 2 / 100;
		style.richText = true;

		GUILayout.BeginArea(new Rect(0, h - (h * 0.3f), w * 0.4f, h * 0.3f));
		ScrollPos = GUILayout.BeginScrollView(ScrollPos);
		GUILayout.TextArea(BMSLogs, style, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	#endregion

	private void PutLogInFile(BMSLog.Logtype type, string log)
	{
		// TODO:  Need to use the isolated storage for IOS
#if UNITY_IOS
		return;
#else
		if (!LogToFile)
			return;

		lock (Mutex)
		{
			string read = string.Empty;
			using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
			{
				read = sr.ReadToEnd();
			}

			string dlog = $"[{System.DateTime.Now.ToString()} - {type.ToString().ToUpper()}] {log}";
			if (type == BMSLog.Logtype.Exception)
			{
				dlog = $"{dlog}{System.Environment.NewLine}{System.Environment.StackTrace}";
			}

			string finalLog = $"{read}{dlog}";

			using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filepath))
			{
				sw.WriteLine(finalLog);
			}
		}
#endif
	}
}
