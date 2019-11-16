using Forge.Editor.UI.WIndows;
using UnityEditor;
using UnityEngine;

namespace Forge.Editor
{
	public class ForgeEditor : EditorWindow
	{
		private readonly IEditorWindow _mainWindow = new MainWindow();

		[MenuItem("Window/Forge/Networking Editor")]
		public static void Init()
		{
			var window = GetWindow<ForgeEditor>();
			window.Initialize();
			window.Show();
		}

		public void Initialize()
		{
			this.titleContent = new GUIContent("Forge: Networking");
		}

		public void OnGUI()
		{
			_mainWindow.Draw();
		}
	}
}
