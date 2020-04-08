namespace Forge.Networking.Messaging
{
	internal class MessagePoolEntry
	{
		public bool Available { get; set; }
		public IMessage Message { get; set; }
	}
}
