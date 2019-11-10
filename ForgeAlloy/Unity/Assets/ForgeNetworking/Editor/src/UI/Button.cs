using System;
using UnityEngine;

namespace Forge.Editor.UI
{
	public class Button<T> : IButton<T>
	{
		public Action<T> Callback { get; set; }
		public T State { get; set; }
		public string Label { get; set; }

		public void Draw()
		{
			if (GUILayout.Button(Label))
				Callback?.Invoke(State);
		}
	}
}
