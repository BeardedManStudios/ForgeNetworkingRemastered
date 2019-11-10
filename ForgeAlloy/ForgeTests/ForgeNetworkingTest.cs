using System.Threading;
using Forge;
using NUnit.Framework;

namespace ForgeTests
{
	public sealed class ForgeTestSynchronizationContext : SynchronizationContext
	{
		public override void Post(SendOrPostCallback d, object state)
		{
			d(state);
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			d(state);
		}
	}

	public class ForgeNetworkingTest
	{
		[SetUp]
		public virtual void Setup()
		{
			ForgeRegistration.Initialize();
		}

		[TearDown]
		public virtual void Teardown()
		{
			ForgeRegistration.Teardown();
		}
	}
}
