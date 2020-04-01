using System;
using System.Net;
using FakeItEasy;
using Forge.Factory;
using Forge.Networking.Players;
using Forge.Networking.Sockets;
using NUnit.Framework;

namespace ForgeTests.Networking.Player
{
	[TestFixture]
	public class PlayerRepositoryTests : ForgeNetworkingTest
	{
		[Test]
		public void AddPlayer_ShouldGetSamePlayer()
		{
			var id = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerSignature>();
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.Id).Returns(id);
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
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
			var id = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerSignature>();
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.Id).Returns(id);
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();

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
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			Assert.Throws<PlayerNotFoundException>(() => repo.GetPlayer(AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerSignature>()));
		}

		[Test]
		public void GettingPlayerThatDoesNotExist_ShouldThrow()
		{
			var id = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerSignature>();
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.Id).Returns(id);
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			repo.AddPlayer(player);
			Assert.Throws<PlayerNotFoundException>(() => repo.GetPlayer(AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerSignature>()));
		}

		[Test]
		public void GettingPlayerByEndpoint_ShouldFindPlayer()
		{
			var ep = new IPEndPoint(IPAddress.Parse(CommonSocketBase.LOCAL_IPV4), 12345);
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.EndPoint).Returns(ep);
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			repo.AddPlayer(player);
			var found = repo.GetPlayer(ep);
			Assert.AreEqual(player, found);
			Assert.AreEqual(player.EndPoint, found.EndPoint);
		}

		[Test]
		public void GettingPlayerByEndpointThatDoesntExist_ShouldThrow()
		{
			var ep = new IPEndPoint(IPAddress.Parse(CommonSocketBase.LOCAL_IPV4), 12345);
			var wrongEp = new IPEndPoint(IPAddress.Parse(CommonSocketBase.LOCAL_IPV4), 3515);
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.EndPoint).Returns(ep);
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			repo.AddPlayer(player);
			Assert.Throws<PlayerNotFoundException>(() => repo.GetPlayer(wrongEp));
		}

		[Test]
		public void AddingPlayer_ShouldHaveGuidAssigned()
		{
			var initialId = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerSignature>();
			var player = A.Fake<INetPlayer>();
			player.Id = initialId;
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			repo.AddPlayer(player);
			Assert.AreNotEqual(initialId, player.Id);
			Assert.AreNotEqual(new Guid(), player.Id);
		}

		[Test]
		public void AddingPlayer_ShouldInvokeTheAddedPlayerEventAndMatch()
		{
			var player = A.Fake<INetPlayer>();
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			var evt = A.Fake<PlayerAddedToRepository>();
			repo.onPlayerAddedSubscription += evt;
			repo.AddPlayer(player);
			A.CallTo(() => evt(player)).MustHaveHappenedOnceExactly();
		}

		[Test]
		public void AddingPlayerAndCheckingExist_ShouldReturnTrue()
		{
			var ep = new IPEndPoint(IPAddress.Parse(CommonSocketBase.LOCAL_IPV4), 12345);
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.EndPoint).Returns(ep);
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			repo.AddPlayer(player);
			bool exists = repo.Exists(player.EndPoint);
			Assert.IsTrue(exists);
		}

		[Test]
		public void EmptyRepositoryPlayerExist_ShouldReturnFalse()
		{
			var ep = new IPEndPoint(IPAddress.Parse(CommonSocketBase.LOCAL_IPV4), 12345);
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			bool exists = repo.Exists(ep);
			Assert.IsFalse(exists);
		}

		[Test]
		public void CheckingExistOnNonAddedPlayerOnNonEmptyRepository_ShouldReturnFalse()
		{
			var ep = new IPEndPoint(IPAddress.Parse(CommonSocketBase.LOCAL_IPV4), 12345);
			var wrongEp = new IPEndPoint(IPAddress.Parse(CommonSocketBase.LOCAL_IPV4), 3574);
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.EndPoint).Returns(ep);
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerRepository>();
			repo.AddPlayer(player);
			bool exists = repo.Exists(wrongEp);
			Assert.IsFalse(exists);
		}
	}
}
