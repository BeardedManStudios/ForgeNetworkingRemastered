using System;

namespace Forge.Editor.UI
{
	public interface IButton : ILabeledEditorUI
	{
		Action Callback { get; set; }
	}

	public interface IButton<T> : ILabeledEditorUI
	{
		Action<T> Callback { get; set; }
		T State { get; set; }
	}
}
