using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;
using UnityEngine;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(8, typeof(SendEntitiesToNewPlayerMessage))]
	public class SendEntitiesToNewPlayerMessage : ForgeMessage
	{
		public int EntityCount { get; set; }
		public int[] Ids { get; set; }
		public int[] PrefabIds { get; set; }
		public Vector3[] Positions { get; set; }
		public Quaternion[] Rotations { get; set; }
		public Vector3[] Scales { get; set; }

		public override IMessageInterpreter Interpreter => SendEntitiesToNewPlayerInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			EntityCount = ForgeSerializationStrategy.Instance.Deserialize<int>(buffer);
			Ids = new int[EntityCount];
			PrefabIds = new int[EntityCount];
			Positions = new Vector3[EntityCount];
			Rotations = new Quaternion[EntityCount];
			Scales = new Vector3[EntityCount];
			for (int i = 0; i < EntityCount; i++)
			{
				Ids[i] = ForgeSerializationStrategy.Instance.Deserialize<int>(buffer);
				PrefabIds[i] = ForgeSerializationStrategy.Instance.Deserialize<int>(buffer);
				Positions[i] = ForgeSerializationStrategy.Instance.Deserialize<Vector3>(buffer);
				Rotations[i] = ForgeSerializationStrategy.Instance.Deserialize<Quaternion>(buffer);
				Scales[i] = ForgeSerializationStrategy.Instance.Deserialize<Vector3>(buffer);
			}
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializationStrategy.Instance.Serialize(EntityCount, buffer);
			for (int i = 0; i < EntityCount; i++)
			{
				ForgeSerializationStrategy.Instance.Serialize(Ids[i], buffer);
				ForgeSerializationStrategy.Instance.Serialize(PrefabIds[i], buffer);
				ForgeSerializationStrategy.Instance.Serialize(Positions[i], buffer);
				ForgeSerializationStrategy.Instance.Serialize(Rotations[i], buffer);
				ForgeSerializationStrategy.Instance.Serialize(Scales[i], buffer);
			}
		}
	}
}
