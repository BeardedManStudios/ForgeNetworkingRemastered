using FakeItEasy;
using Forge;
using Forge.Engine;
using Forge.Networking;
using Forge.Networking.Messaging.Messages;
using NUnit.Framework;

namespace ForgeTests.Networking.Messaging.Interpreters
{
	[TestFixture]
	public class EntityMessageInterpreter : ForgeNetworkingTest
	{
		[Test]
		public void EntityMessage_ShouldReachEntity()
		{
			var entity = A.Fake<IEntity>();
			var engine = A.Fake<IEngineContainer>();
			var entityMessage = new ForgeEntityMessage();
			var network = ForgeTypeFactory.Get<INetworkContainer>();
			network.ChangeEngineContainer(engine);

			A.CallTo(() => engine.FindEntityWithId(A<int>._)).Returns(entity);
			entityMessage.Interpret(network);

			A.CallTo(() => entity.ProcessNetworkMessage(A<IEntityMessage>._)).MustHaveHappenedOnceExactly();
		}
	}
}
