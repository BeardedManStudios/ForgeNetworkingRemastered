namespace Forge.Editor.Messages
{
	public interface IMessageTemplateRepository
	{
		void Add(IMessageTemplate template);
		void Remove(IMessageTemplate template);
		void Remove(string name);
		bool Exists(string name);
		IMessageTemplate[] ToArray();
	}
}
