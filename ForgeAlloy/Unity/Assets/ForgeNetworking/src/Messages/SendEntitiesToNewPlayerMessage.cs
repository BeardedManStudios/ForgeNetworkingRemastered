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
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(EntityCount));
			for (int i = 0; i < EntityCount; i++)
			{
				buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Ids[i]));
				buffer.Append(ForgeSerializationStrategy.Instance.Serialize(PrefabIds[i]));
				buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Positions[i]));
				buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Rotations[i]));
				buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Scales[i]));
			}
		}
	}
}
