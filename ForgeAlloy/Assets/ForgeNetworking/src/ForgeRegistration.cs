using Forge.Networking.Players;

namespace Forge
{
	public static class ForgeRegistration
	{
		public static void InitializeForge()
		{
			ForgeTypeFactory.Register<IPlayerRepository>(() => new ForgePlayerRepository());
		}
	}
}
