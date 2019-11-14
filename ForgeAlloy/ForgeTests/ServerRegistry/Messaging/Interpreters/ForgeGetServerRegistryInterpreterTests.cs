using System.Linq;
using System.Net;
using FakeItEasy;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Sockets;
using Forge.ServerRegistry.Messaging.Messages;
using ForgeServerRegistryService.Messaging.Interpreters;
using NUnit.Framework;

namespace ForgeTests.ServerRegistry.Messaging.Interpreters
{
	[TestFixture]
	public class ForgeGetServerRegistryInterpreterTests : ForgeNetworkingTest
	{
		[Test]
		public void InterpretationOfGetMessage_ShouldGetCorrectPlayerList()
		{
			var players = new INetPlayer[]
			{
				A.Fake<INetPlayer>(),
				A.Fake<INetPlayer>()
			};
			A.CallTo(() => players[0].EndPoint).Returns(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15937));
			A.CallTo(() => players[1].EndPoint).Returns(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 15938));
			var net = A.Fake<INetworkMediator>();
			A.CallTo(() => net.PlayerRepository.Count).Returns(players.Length);
			A.CallTo(() => net.PlayerRepository.GetEnumerator()).Returns(players.ToList().GetEnumerator());
			A.CallTo(() => net.MessageBus.SendReliableMessage(A<IMessage>._, A<ISocket>._, A<EndPoint>._))
				.Invokes((ctx) =>
				{
					Assert.IsTrue(ctx.Arguments[0] is ForgeServerRegistryMessage);
					var msg = (ForgeServerRegistryMessage)ctx.Arguments[0];
					Assert.AreEqual(players.Length, msg.Entries.Length);
					for (int i = 0; i < players.Length; i++)
					{
						var ep = new IPEndPoint(IPAddress.Parse(msg.Entries[i].Address), msg.Entries[i].Port);
						Assert.AreEqual(players[i].EndPoint, ep);
					}
				});
			var interpreter = new GetServerRegistryInterpreter();
			interpreter.Interpret(net, players[0].EndPoint, A.Fake<IMessage>());
			A.CallTo(() => net.MessageBus.SendReliableMessage(A<IMessage>._, A<ISocket>._, A<EndPoint>._))
				.MustHaveHappenedOnceExactly();
		}
	}
}
