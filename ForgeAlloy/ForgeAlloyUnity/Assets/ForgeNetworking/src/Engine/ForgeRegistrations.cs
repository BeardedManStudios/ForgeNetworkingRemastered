using Forge.Factory;
using Forge.Networking.Unity.Serialization;
using Forge.Serialization;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public static class ForgeRegistrations
	{
		public static void Initialize()
		{
			ForgeRegistration.Initialize();
			RegisterFactoryInterfaces();
			SetupExtraSerializers();
		}

		private static void RegisterFactoryInterfaces()
		{
			var factory = AbstractFactory.Get<INetworkTypeFactory>();
			factory.Register<IEntityRepository, EntityRepository>();
		}

		private static void SetupExtraSerializers()
		{
			ForgeSerializer.Instance.AddSerializer<Vector3>(new Vector3Serializer());
			ForgeSerializer.Instance.AddSerializer<Quaternion>(new QuaternionSerializer());
		}
	}
}
