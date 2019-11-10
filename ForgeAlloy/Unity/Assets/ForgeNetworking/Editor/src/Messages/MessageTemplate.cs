using System.Collections.Generic;
using Forge.Networking.Messaging;

namespace Forge.Editor.Messages
{
	public class MessageTemplate : IMessageTemplate
	{
		private List<IMessageProperty> _properties = new List<IMessageProperty>();
		public string Name { get; set; }
		public IMessageInterpreter Interpreter { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public void Add(IMessageProperty property)
		{
			_properties.Add(property);
		}

		public void Remove(IMessageProperty property)
		{
			_properties.Remove(property);
		}

		public void Remove(string name)
		{
			for (int i = 0; i < _properties.Count; ++i)
			{
				if (_properties[i].Name == name)
				{
					_properties.RemoveAt(i);
					break;
				}
			}
		}

		public IMessageProperty[] ToArray()
		{
			return _properties.ToArray();
		}
	}
}
