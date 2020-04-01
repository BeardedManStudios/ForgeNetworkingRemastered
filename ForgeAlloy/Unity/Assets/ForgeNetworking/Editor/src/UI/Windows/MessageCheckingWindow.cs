using System;
using System.Collections.Generic;
using Forge.Reflection.Networking.Messaging;
using UnityEditor;

namespace Forge.Editor.UI.Windows
{
	public class MessageCheckingWindow : IEditorWindow
	{
		public string Name => "Forge: Network Message Checkup";
		private readonly MessageContractChecker _checker;
		private readonly Dictionary<Type, string> _errors;

		public MessageCheckingWindow()
		{
			_checker = new MessageContractChecker();
			_errors = _checker.GetAllErrors();
		}

		public void Draw()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"Next available message code is:  {_checker.HighestCodeFound + 1}");
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			if (_errors.Count == 0)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox($"All IMessage implementations are looking good!", MessageType.Info);
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				foreach (var kv in _errors)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.HelpBox(kv.Value, MessageType.Error);
					EditorGUILayout.EndHorizontal();
				}
			}
		}
	}
}
