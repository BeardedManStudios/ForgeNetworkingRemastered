namespace Forge.Networking.Messaging.Paging
{
	public struct ForgeMessagePage : IMessagePage
	{
		public int StartOffset { get; set; }
		public int Length { get; set; }
	}
}
