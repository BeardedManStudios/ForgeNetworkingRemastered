using UnityEditor;
using UnityEngine;

namespace Forge.Editor.UI.Windows
{
	public class ContributingWindow : IEditorWindow
	{
		public string Name => "Forge: Contributing";
		private readonly string[] lines;

		public ContributingWindow()
		{
			var asset = Resources.Load<TextAsset>("ForgeNetworking/html/contributing");
			lines = asset.text.Replace("\r", "").Split('\n');
			for (int i = 0; i < lines.Length; i++)
				lines[i] = lines[i].ToUnityUiHtml();
		}

		public void Draw()
		{
			EditorStyles.label.wordWrap = true;
			EditorStyles.label.richText = true;

			foreach (string line in lines)
				PrintLine(line);
		}

		private void PrintLine(string line)
		{
			if (line.Contains("<hr />"))
			{
				EditorGUILayout.Separator();
				return;
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(line);
			EditorGUILayout.EndHorizontal();

			int idx = line.IndexOf("<a href=\"");
			if (idx >= 0)
			{
				idx += 9;
				int endPos = line.IndexOf("\"", idx);
				if (endPos > idx)
				{
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Open above link"))
						Application.OpenURL(line.Substring(idx, endPos - idx));
					EditorGUILayout.EndHorizontal();
				}
			}
		}
	}
}
