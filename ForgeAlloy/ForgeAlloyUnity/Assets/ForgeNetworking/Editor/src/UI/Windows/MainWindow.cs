using UnityEditor;
using UnityEngine;

namespace Forge.Editor.UI.Windows
{
	public class MainWindow : IEditorWindow
	{
		public EditorWindow WindowHandle { get; set; }
		public string Name => "Forge: Networking";
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
			_buttons.AddElement(new Button
			{
				Text = "Contributing",
				Callback = ViewContributing
			});
			_buttons.AddElement(new Button
			{
				Text = "GitHub Page",
				Callback = OpenGitHub
			});
			_buttons.AddElement(new Button
			{
				Text = "Join Live Chat (Discord)",
				Callback = JoinDiscord
			});
			_currentWindow = this;
		}

		private void ChangeWindow(IEditorWindow window)
		{
			_currentWindow = window;
			WindowHandle.titleContent = new GUIContent(window.Name);
		}
		private void ReturnToMainMenu() => ChangeWindow(this);
		private void ViewMessageChecker() => ChangeWindow(new MessageCheckingWindow());
		private void ViewMessageList() => ChangeWindow(new MessageListWindow());
		private void ViewContributing() => ChangeWindow(new ContributingWindow());
		private void OpenGitHub() => Application.OpenURL("https://github.com/BeardedManStudios/ForgeNetworkingRemastered");
		private void JoinDiscord() => Application.OpenURL("https://discord.gg/v3z2482");

		public void Draw()
		{
			// TODO:  Turn the scroll view into a class
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
			if (_currentWindow != this)
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
