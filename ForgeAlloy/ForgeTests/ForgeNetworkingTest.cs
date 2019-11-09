using Forge;
using NUnit.Framework;

namespace ForgeTests
{
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
