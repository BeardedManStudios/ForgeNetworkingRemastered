using UnityEditor;

namespace Forge.Editor.UI
{
	public class Label : ILabeledEditorUI
	{
		public string Text { get; set; } = "";

		public void Draw()
		{
			EditorGUILayout.LabelField(Text);
		}
	}
}
