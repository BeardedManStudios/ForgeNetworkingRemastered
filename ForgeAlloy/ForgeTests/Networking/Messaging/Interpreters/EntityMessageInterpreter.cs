using System.Net;
using FakeItEasy;
using Forge.Engine;
using Forge.Factory;
using Forge.Networking;
using Forge.Networking.Messaging.Interpreters;
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
			var engine = A.Fake<IEngineProxy>();
			var entityMessage = A.Fake<ForgeEntityMessage>();
			A.CallTo(() => entityMessage.Interpreter).Returns(new ForgeEntityMessageInterpreter());
			var network = AbstractFactory.Get<INetworkTypeFactory>().GetNew<INetworkMediator>();
			network.ChangeEngineProxy(engine);

			A.CallTo(() => engine.EntityRepository.GetEntityById(A<int>._)).Returns(entity);
			entityMessage.Interpreter.Interpret(network, A.Fake<EndPoint>(), entityMessage);

			A.CallTo(() => entityMessage.ProcessUsing(A<IEntity>._)).MustHaveHappenedOnceExactly();
		}
	}
}
