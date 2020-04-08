using UnityEditor;

namespace Forge.Editor.UI
{
	public class Foldout : IFoldout
	{
		public string Text { get; set; } = "";
		public bool IsUnfolded { get; private set; } = false;

		public void Draw()
		{
			EditorGUILayout.BeginHorizontal();
			IsUnfolded = EditorGUILayout.Foldout(IsUnfolded, Text);
			EditorGUILayout.EndHorizontal();
		}
	}
}
