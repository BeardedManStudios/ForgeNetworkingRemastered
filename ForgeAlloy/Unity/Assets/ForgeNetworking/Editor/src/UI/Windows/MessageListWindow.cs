using System;
using Forge.Networking.Messaging;
using Forge.Reflection;
using UnityEngine;

namespace Forge.Editor.UI.WIndows
{
	public class MessageListWindow : IEditorWindow
	{
		private readonly TypeReflectionRepository _reflectionRepository;
		private readonly IFoldout _messageListFoldout = new Foldout();
		private readonly ILabeledList _buttonList = new LabeledList();

		public MessageListWindow()
		{
			_reflectionRepository = new TypeReflectionRepository();
			_reflectionRepository.AddType<IMessage>();

			var messages = _reflectionRepository.GetTypesFor<IMessage>();
			foreach (var message in messages)
			{
				_buttonList.AddElement(new Button<Type>
				{
					Label = message.Name,
					State = message,
					Callback = SelectMessageType
				});
			}
		}

		public void Draw()
		{
			_messageListFoldout.Draw();
			if (!_messageListFoldout.IsUnfolded)
				return;
			_buttonList.Draw();
		}

		private void SelectMessageType(Type type)
		{
			Debug.Log($"You have clicked on the type {type}");
		}
	}
}
