using Forge.Editor.UI.Windows;
using UnityEditor;
using UnityEngine;

namespace Forge.Editor
{
	public class ForgeEditor : EditorWindow
	{
		private readonly IEditorWindow _mainWindow;

		public ForgeEditor()
		{
			_mainWindow = new MainWindow { WindowHandle = this };
		}

		[MenuItem("Window/Forge/Networking Editor")]
		public static void Init()
		{
			var window = GetWindow<ForgeEditor>();
			window.Initialize();
			window.Show();
		}

		public void Initialize()
		{
			this.titleContent = new GUIContent(_mainWindow.Name);
		}

		public void OnGUI()
		{
			_mainWindow.Draw();
		}
	}
}
