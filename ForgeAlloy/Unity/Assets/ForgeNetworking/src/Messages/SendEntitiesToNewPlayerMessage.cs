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
			EntityCount = ForgeSerialization.Instance.Deserialize<int>(buffer);
			Ids = new int[EntityCount];
			PrefabIds = new int[EntityCount];
			Positions = new Vector3[EntityCount];
			Rotations = new Quaternion[EntityCount];
			Scales = new Vector3[EntityCount];
			for (int i = 0; i < EntityCount; i++)
			{
				Ids[i] = ForgeSerialization.Instance.Deserialize<int>(buffer);
				PrefabIds[i] = ForgeSerialization.Instance.Deserialize<int>(buffer);
				Positions[i] = ForgeSerialization.Instance.Deserialize<Vector3>(buffer);
				Rotations[i] = ForgeSerialization.Instance.Deserialize<Quaternion>(buffer);
				Scales[i] = ForgeSerialization.Instance.Deserialize<Vector3>(buffer);
			}
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerialization.Instance.Serialize(EntityCount, buffer);
			for (int i = 0; i < EntityCount; i++)
			{
				ForgeSerialization.Instance.Serialize(Ids[i], buffer);
				ForgeSerialization.Instance.Serialize(PrefabIds[i], buffer);
				ForgeSerialization.Instance.Serialize(Positions[i], buffer);
				ForgeSerialization.Instance.Serialize(Rotations[i], buffer);
				ForgeSerialization.Instance.Serialize(Scales[i], buffer);
			}
		}
	}
}
