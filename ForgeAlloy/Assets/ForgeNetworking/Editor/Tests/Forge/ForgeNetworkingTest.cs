using Forge.Networking.Players;
using NUnit.Framework;

namespace Forge.Tests
{
	public class ForgeNetworkingTest
	{
		[SetUp]
		public virtual void Setup()
		{
			// FORGE:  As a developer, if you are going to use a different type factory objects
			// then you should replace these registrations with the ones you are going to use
			ForgeTypeFactory.Register<IPlayerRepository>(() => new ForgePlayerRepository());
		}

		[TearDown]
		public virtual void Teardown()
		{
			ForgeTypeFactory.Unregister<IPlayerRepository>();
		}
	}
}
