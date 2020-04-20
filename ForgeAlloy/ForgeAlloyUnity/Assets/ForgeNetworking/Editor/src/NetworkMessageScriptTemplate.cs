using System;
using System.IO;
using System.Text.RegularExpressions;
using Forge.Reflection.Networking.Messaging;
using UnityEditor;
using UnityEngine;

namespace Forge.Editor
{
	public class NetworkMessageScriptTemplate : ScriptableWizard
	{
		private string _messageName = "";
		private bool _isClient = false;
		private bool _isServer = false;
		private bool _useSingletonInterpreter = true;

		[MenuItem("Assets/Create/Forge Networking/Network Message Script")]
		private static void CreateWizard()
		{
			ScriptableWizard.DisplayWizard<NetworkMessageScriptTemplate>("Network Message Script Wizard", "Create");
		}

		private void OnWizardCreate()
		{
			SanitizeMessageName();
			var reg = new Regex(@"^[^0-9][a-zA-Z0-9_]+$");
			if (reg.IsMatch(_messageName))
				CreateNewMessageFilesBasedOnTemplate();
			else
				throw new Exception($"The name '{_messageName}' is not valid. This should be as if you were naming a new C# class. It must not start with a number and contain only a-z, A-Z, 0-9, and/or _.");
		}

		private void SanitizeMessageName()
		{
			if (_messageName.Contains(" "))
				Debug.LogWarning($"The name {_messageName} contains spaces, they will be automatically converted to _");
			_messageName = _messageName.Replace(" ", "_");
		}

		private void CreateNewMessageFilesBasedOnTemplate()
		{
			if (!_isClient && !_isServer)
				throw new Exception("The interpreter requires one or both of the check boxes for Client/Server to be enabled");

			var checker = new MessageContractChecker();
			int nextId = checker.HighestCodeFound + 1;

			string messageTemplatePath = "Templates/MessageTemplate";
			string interpreterTemplatePath = "Templates/MessageInterpreterTemplate";
			if (_useSingletonInterpreter)
			{
				messageTemplatePath += "Singleton";
				interpreterTemplatePath += "Singleton";
			}

			var messageTemplate = Resources.Load<TextAsset>(messageTemplatePath);
			var interpreterTemplate = Resources.Load<TextAsset>(interpreterTemplatePath);

			string messageCode = string.Format(messageTemplate.text, nextId, _messageName);
			string interpreterCode = string.Format(interpreterTemplate.text,
				_messageName, _isClient.ToString().ToLower(),
				_isServer.ToString().ToLower());

			string path = Selection.activeObject != null
				? AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID())
				: "Assets";

			string interpreterPath = path;
			if (Directory.Exists($"{interpreterPath}/Interpreters"))
				interpreterPath += "/Interpreters";

			File.WriteAllText($"{path}/{_messageName}Message.cs", messageCode);
			File.WriteAllText($"{interpreterPath}/{_messageName}Interpreter.cs", interpreterCode);
			AssetDatabase.Refresh();
		}

		protected override bool DrawWizardGUI()
		{
			EditorGUILayout.BeginHorizontal();
			_messageName = EditorGUILayout.TextField("Message Name:", _messageName);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			_isServer = EditorGUILayout.Toggle("Interpret on server:", _isServer);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			_isClient = EditorGUILayout.Toggle("Interpret on client:", _isClient);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			_useSingletonInterpreter = EditorGUILayout.Toggle("Singleton Interpreter:", _useSingletonInterpreter);
			EditorGUILayout.EndHorizontal();
			return base.DrawWizardGUI();
		}
	}
}
