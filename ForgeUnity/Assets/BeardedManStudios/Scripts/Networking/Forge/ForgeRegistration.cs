using BeardedManStudios.Forge.Networking.Players;

namespace BeardedManStudios.Forge
{
	public static class ForgeRegistration
	{
		public static void InitializeForge()
		{
			ForgeTypeFactory.Register<IPlayerRepository>(() => new ForgePlayerRepository());
		}
	}
}
