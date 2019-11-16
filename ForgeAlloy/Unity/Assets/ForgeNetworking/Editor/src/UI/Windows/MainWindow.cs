using UnityEditor;
using UnityEngine;

namespace Forge.Editor.UI.WIndows
{
	public class MainWindow : IEditorWindow
	{
		private IEditorWindow _currentWindow = null;
		private readonly ILabeledList _buttons = new LabeledList();
		private readonly IButton _returnButton;
		private Vector2 _scrollPosition = new Vector2(0.0f, 0.0f);

		public MainWindow()
		{
			_returnButton = new Button
			{
				Text = "Return to main menu",
				Callback = ReturnToMainMenu
			};
			_buttons.AddElement(new Button
			{
				Text = "Message Checker",
				Callback = ViewMessageChecker
			});
			_buttons.AddElement(new Button
			{
				Text = "Message Listing",
				Callback = ViewMessageList
			});
		}

		private void ReturnToMainMenu() { _currentWindow = null; }
		private void ViewMessageChecker() { _currentWindow = new MessageCheckingWindow(); }
		private void ViewMessageList() { _currentWindow = new MessageListWindow(); }

		public void Draw()
		{
			// TODO:  Turn the scroll view into a class
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
			if (_currentWindow != null)
			{
				_currentWindow.Draw();
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				_returnButton.Draw();
				EditorGUILayout.EndHorizontal();
			}
			else
				_buttons.Draw();
			EditorGUILayout.EndScrollView();
		}
	}
}
