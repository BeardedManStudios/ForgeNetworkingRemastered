namespace Forge.Networking.Messaging.Paging
{
	public interface IMessagePage
	{
		int StartOffset { get; set; }
		int Length { get; set; }
	}
}
