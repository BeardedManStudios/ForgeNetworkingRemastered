using UnityEditor;
using UnityEngine;

namespace Forge.Editor
{
	public class ForgeEditor : EditorWindow
	{
		[MenuItem("Forge/Editor")]
		public static void Init()
		{
			var window = GetWindow<ForgeEditor>();
			window.Initialize();
			window.Show();
		}

		public void Initialize()
		{
			this.titleContent = new GUIContent("Forge Editor");
		}
	}
}
