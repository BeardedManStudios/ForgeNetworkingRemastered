using UnityEditor;

namespace Forge.Editor.UI
{
	public class TextInput : ILabeledEditorUI
	{
		public string Text { get; set; } = "";

		public void Draw()
		{
			Text = EditorGUILayout.TextField(Text);
		}
	}
}
