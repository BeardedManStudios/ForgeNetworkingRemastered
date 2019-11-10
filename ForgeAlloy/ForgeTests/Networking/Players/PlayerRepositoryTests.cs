using System;
using FakeItEasy;
using Forge;
using Forge.Networking.Players;
using NUnit.Framework;

namespace ForgeTests.Networking.Player
{
	[TestFixture]
	public class PlayerRepositoryTests : ForgeNetworkingTest
	{
		[Test]
		public void AddPlayer_ShouldGetSamePlayer()
		{
			var id = Guid.NewGuid();
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.Id).Returns(id);
			var repo = ForgeTypeFactory.GetNew<IPlayerRepository>();
			Assert.AreEqual(0, repo.Count);
			repo.AddPlayer(player);
			Assert.AreEqual(1, repo.Count);
			INetPlayer found = repo.GetPlayer(player.Id);
			Assert.AreEqual(player, found);
			Assert.AreEqual(player.Id, found.Id);
			Assert.AreEqual(1, repo.Count);
		}

		[Test]
		public void RemovePlayer_ShouldRemoveSamePlayer()
		{
			var id = Guid.NewGuid();
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.Id).Returns(id);
			var repo = ForgeTypeFactory.GetNew<IPlayerRepository>();

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
			var repo = ForgeTypeFactory.GetNew<IPlayerRepository>();
			Assert.Throws<PlayerNotFoundException>(() => repo.GetPlayer(Guid.NewGuid()));
		}

		[Test]
		public void GettingPlayerThatDoesNotExist_ShouldThrow()
		{
			var id = Guid.NewGuid();
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.Id).Returns(id);
			var repo = ForgeTypeFactory.GetNew<IPlayerRepository>();
			repo.AddPlayer(player);
			Assert.Throws<PlayerNotFoundException>(() => repo.GetPlayer(Guid.NewGuid()));
		}

		[Test]
		public void GettingPlayerByEndpoint_ShouldFindPlayer()
		{

		}

		[Test]
		public void GettingPlayerByEndpointThatDoesntExist_ShouldThrow()
		{

		}

		[Test]
		public void AddingPlayer_ShouldHaveGuidAssigned()
		{

		}
	}
}
