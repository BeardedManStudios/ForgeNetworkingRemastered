namespace Forge.Networking.Players
{
	public class ForgePlayer : INetPlayer
	{
		public int Id { get; private set; }
		public string Ip { get; private set; }
		public ushort Port { get; private set; }
		public string Name { get; private set; }
	}
}
