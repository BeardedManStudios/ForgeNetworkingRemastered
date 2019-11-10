using System.Collections.Generic;
using UnityEditor;

namespace Forge.Editor.UI
{
	public class LabeledList : ILabeledList
	{
		private readonly List<ILabeledEditorUI> _elements = new List<ILabeledEditorUI>();

		public void AddElement(ILabeledEditorUI element)
		{
			_elements.Add(element);
		}

		public void Draw()
		{
			foreach (var elm in _elements)
			{
				EditorGUILayout.BeginHorizontal();
				elm.Draw();
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}
