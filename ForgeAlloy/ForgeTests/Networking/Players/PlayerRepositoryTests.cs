using FakeItEasy;
using Forge.Networking.Players;
using NUnit.Framework;

namespace Forge.Tests.Networking.Player
{
	[TestFixture]
	public class PlayerRepositoryTests : ForgeNetworkingTest
	{
		[Test]
		public void AddPlayer_ShouldGetSamePlayer()
		{
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.Id).Returns(9);
			var repo = ForgeTypeFactory.Get<IPlayerRepository>();
			Assert.AreEqual(0, repo.Count);
			repo.AddPlayer(player);
			Assert.AreEqual(1, repo.Count);
			INetPlayer found = repo.GetPlayer(player.Id);
			Assert.AreEqual(player.Id, found.Id);
			Assert.AreEqual(1, repo.Count);
		}

		[Test]
		public void RemovePlayer_ShouldRemoveSamePlayer()
		{
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.Id).Returns(9);
			var repo = ForgeTypeFactory.Get<IPlayerRepository>();

			// Test using the reference to the player
			repo.AddPlayer(player);
			Assert.AreEqual(1, repo.Count);
			repo.RemovePlayer(player);
			Assert.AreEqual(0, repo.Count);

			// Test using just the id of the player
			repo.AddPlayer(player);
			Assert.AreEqual(1, repo.Count);
			repo.RemovePlayer(player.Id);
			Assert.AreEqual(0, repo.Count);
		}

		[Test]
		public void GettingPlayerFromEmptyRepo_ShouldThrow()
		{
			var repo = ForgeTypeFactory.Get<IPlayerRepository>();
			Assert.Throws<PlayerNotFoundException>(() => repo.GetPlayer(9));
		}

		[Test]
		public void GettingPlayerThatDoesNotExist_ShouldThrow()
		{
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.Id).Returns(9);
			var repo = ForgeTypeFactory.Get<IPlayerRepository>();
			Assert.Throws<PlayerNotFoundException>(() => repo.GetPlayer(1));
		}
	}
}
