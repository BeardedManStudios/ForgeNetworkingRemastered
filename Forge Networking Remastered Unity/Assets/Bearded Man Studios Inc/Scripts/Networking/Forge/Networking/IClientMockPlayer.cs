namespace BeardedManStudios.Forge.Networking
{
	public interface IClientMockPlayer
	{
		uint NetworkId { get; set; }
		string Name { get; set; }
        int AvatarID { get; set; }
        int TeamID { get; set; }
	}
}
