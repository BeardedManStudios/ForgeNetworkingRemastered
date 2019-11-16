using Forge.Networking.Messaging;
using Forge.Reflection;
using UnityEditor;

namespace Forge.Editor.UI.WIndows
{
	public class MessageListWindow : IEditorWindow
	{
		private readonly TypeReflectionRepository _reflectionRepository;
		private readonly ILabeledList _labelList = new LabeledList();
		private readonly ILabeledEditorUI _searchBox = new TextInput();

		public MessageListWindow()
		{
			_reflectionRepository = new TypeReflectionRepository();
			_reflectionRepository.AddType<IMessage>();

			var messages = _reflectionRepository.GetTypesFor<IMessage>();
			foreach (var message in messages)
			{
				_labelList.AddElement(new Label
				{
					Text = message.Name
				});
			}
		}

		public void Draw()
		{
			_searchBox.Draw();
			EditorGUILayout.Space();
			_labelList.FilterDraw(_searchBox.Text);
		}
	}
}
