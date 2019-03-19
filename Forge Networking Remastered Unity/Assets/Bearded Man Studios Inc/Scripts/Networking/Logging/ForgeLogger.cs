using System;
using System.IO;
using System.Text;

namespace BeardedManStudios.Source.Logging
{
    public class ForgeLogger
    {
        private enum EchoType
        {
            Debug,
            Warning,
            Error,
            Exception,
            WTF
        }

        public static string LogDirectory = "ForgeNetworking";
        private static string ForgeLog { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), LogDirectory); } }

        private static void Echo(object message, EchoType echoType)
        {
#if UNITY_EDITOR
            if (echoType == EchoType.Debug)
                UnityEngine.Debug.Log(message);
            else if (echoType == EchoType.Warning)
                UnityEngine.Debug.LogWarning(message);
            else if (echoType == EchoType.Error)
                UnityEngine.Debug.LogError(message);
            else if (echoType == EchoType.Exception)
                UnityEngine.Debug.LogException(message is string ? new Exception(message.ToString()) : (Exception)message);
            else if (echoType == EchoType.WTF)
            {
                UnityEngine.Debug.Log(message);
                UnityEngine.Debug.LogWarning(message);
                UnityEngine.Debug.LogError(message);
            }
#else
            if (echoType == EchoType.Debug)
                System.Diagnostics.Debug.WriteLine("DEBUG: " + message);
            else if (echoType == EchoType.Warning)
                System.Diagnostics.Debug.WriteLine("WARNING: " + message);
            else if (echoType == EchoType.Error)
                System.Diagnostics.Debug.WriteLine("ERROR: " + message);
            else if (echoType == EchoType.Exception)
                System.Diagnostics.Debug.WriteLine(message);
            else if (echoType == EchoType.WTF)
            {
                System.Diagnostics.Debug.WriteLine(">>>WTF!!!<<<: " + message);
                System.Diagnostics.Debug.WriteLine(">>>WTF!!!<<<: " + message);
                System.Diagnostics.Debug.WriteLine(">>>WTF!!!<<<: " + message);
            }
#endif
        }

        private static void Log(string message)
        {
            string filePath = Path.Combine(ForgeLog, DateTime.Now.ToString("yyyy-MM-dd") + ".log");

            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(ForgeLog))
                    Directory.CreateDirectory(ForgeLog);

                File.Create(filePath);
            }

            File.AppendAllText(filePath, message + Environment.NewLine);
        }

        public static void Debug(string message, bool echo = false)
        {
            Log("DEBUG: " + message);

            if (echo)
                Echo(message, EchoType.Debug);
        }

        public static void Warning(string message, bool echo = false)
        {
            Log("WARNING: " + message);

            if (echo)
                Echo(message, EchoType.Warning);
        }

        public static void Error(string message, bool echo = false)
        {
            Log("ERROR: " + message);

            if (echo)
                Echo(message, EchoType.Error);
        }

        public static void Exception(string message, Exception e, bool echo = false)
        {
            StringBuilder fullMessage = new StringBuilder("Developer Message: " + message);
            fullMessage.Append(Environment.NewLine);
            fullMessage.AppendLine("Exception Message: " + e.Message);
            fullMessage.AppendLine("Trace:");
            fullMessage.Append(Environment.NewLine + e.StackTrace.ToString());
            string finalMessage = "---------" + Environment.NewLine + "EXCEPTION:" + Environment.NewLine + fullMessage.ToString() + Environment.NewLine + "---------";

            Log(finalMessage);

            if (echo)
            {
#if UNITY_EDITOR
                Echo(message, EchoType.Exception);
#else
                Echo(finalMessage, EchoType.Exception);
#endif

            }
        }

        public static void WTF(string message, bool echo = false)
        {
            Log(">>>WTF!!!<<<: " + message);

            if (echo)
                Echo(message, EchoType.WTF);
        }
    }
}