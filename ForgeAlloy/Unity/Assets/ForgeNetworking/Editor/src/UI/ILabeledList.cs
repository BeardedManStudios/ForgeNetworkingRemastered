namespace Forge.Editor.UI
{
	public interface ILabeledList : IEditorUI
	{
		void AddElement(ILabeledEditorUI element);
		void FilterDraw(string text);
	}
}
