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
		public string[] SceneIdentifiers { get; set; }

		public override IMessageInterpreter Interpreter => SendEntitiesToNewPlayerInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			EntityCount = ForgeSerializer.Instance.Deserialize<int>(buffer);
			Ids = new int[EntityCount];
			PrefabIds = new int[EntityCount];
			Positions = new Vector3[EntityCount];
			Rotations = new Quaternion[EntityCount];
			Scales = new Vector3[EntityCount];
			SceneIdentifiers = new string[EntityCount];
			for (int i = 0; i < EntityCount; i++)
			{
				Ids[i] = ForgeSerializer.Instance.Deserialize<int>(buffer);
				PrefabIds[i] = ForgeSerializer.Instance.Deserialize<int>(buffer);
				Positions[i] = ForgeSerializer.Instance.Deserialize<Vector3>(buffer);
				Rotations[i] = ForgeSerializer.Instance.Deserialize<Quaternion>(buffer);
				Scales[i] = ForgeSerializer.Instance.Deserialize<Vector3>(buffer);
				SceneIdentifiers[i] = ForgeSerializer.Instance.Deserialize<string>(buffer);
			}
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(EntityCount, buffer);
			for (int i = 0; i < EntityCount; i++)
			{
				ForgeSerializer.Instance.Serialize(Ids[i], buffer);
				ForgeSerializer.Instance.Serialize(PrefabIds[i], buffer);
				ForgeSerializer.Instance.Serialize(Positions[i], buffer);
				ForgeSerializer.Instance.Serialize(Rotations[i], buffer);
				ForgeSerializer.Instance.Serialize(Scales[i], buffer);
				ForgeSerializer.Instance.Serialize(SceneIdentifiers[i], buffer);
			}
		}
	}
}
