using Forge.Editor.UI.WIndows;
using UnityEditor;
using UnityEngine;

namespace Forge.Editor
{
	public class ForgeEditor : EditorWindow
	{
		private readonly IEditorWindow _messageListWindow = new MessageListWindow();

		[MenuItem("Window/Forge/Editor")]
		public static void Init()
		{
			var window = GetWindow<ForgeEditor>();
			window.Initialize();
			window.Show();
		}

		public void Initialize()
		{
			this.titleContent = new GUIContent("Forge");
		}

		public void OnGUI()
		{
			_messageListWindow.Draw();
		}
	}
}
