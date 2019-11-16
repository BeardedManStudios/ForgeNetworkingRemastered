using System;
using UnityEngine;

namespace Forge.Editor.UI
{
	public class Button : IButton
	{
		public Action Callback { get; set; }
		public string Text { get; set; } = "";

		public void Draw()
		{
			if (GUILayout.Button(Text))
				Callback?.Invoke();
		}
	}

	public class Button<T> : IButton<T>
	{
		public Action<T> Callback { get; set; }
		public T State { get; set; }
		public string Text { get; set; } = "";

		public void Draw()
		{
			if (GUILayout.Button(Text))
				Callback?.Invoke(State);
		}
	}
}
