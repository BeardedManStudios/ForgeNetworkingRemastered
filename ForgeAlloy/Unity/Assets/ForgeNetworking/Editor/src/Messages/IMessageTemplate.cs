using Forge.Networking.Messaging;

namespace Forge.Editor.Messages
{
	public interface IMessageTemplate
	{
		string Name { get; set; }
		IMessageInterpreter Interpreter { get; set; }
		void Add(IMessageProperty property);
		void Remove(IMessageProperty property);
		void Remove(string name);
		IMessageProperty[] ToArray();
	}
}
