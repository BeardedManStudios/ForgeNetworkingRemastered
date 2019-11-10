using UnityEditor;

namespace Forge.Editor.UI
{
	public class Foldout : IFoldout
	{
		public string Label { get; set; } = "Label";
		public bool IsUnfolded { get; private set; } = false;

		public void Draw()
		{
			EditorGUILayout.BeginHorizontal();
			IsUnfolded = EditorGUILayout.Foldout(IsUnfolded, Label);
			EditorGUILayout.EndHorizontal();
		}
	}
}
