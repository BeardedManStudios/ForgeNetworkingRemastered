using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;
using UnityEngine;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(7, typeof(SpawnEntityMessage))]
	public class SpawnEntityMessage : ForgeMessage
	{
		public int Id { get; set; }
		public int PrefabId { get; set; }
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }
		public Vector3 Scale { get; set; }

		public override IMessageInterpreter Interpreter => SpawnEntityInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			Id = ForgeSerialization.Instance.Deserialize<int>(buffer);
			PrefabId = ForgeSerialization.Instance.Deserialize<int>(buffer);
			Position = ForgeSerialization.Instance.Deserialize<Vector3>(buffer);
			Rotation = ForgeSerialization.Instance.Deserialize<Quaternion>(buffer);
			Scale = ForgeSerialization.Instance.Deserialize<Vector3>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerialization.Instance.Serialize(Id, buffer);
			ForgeSerialization.Instance.Serialize(PrefabId, buffer);
			ForgeSerialization.Instance.Serialize(Position, buffer);
			ForgeSerialization.Instance.Serialize(Rotation, buffer);
			ForgeSerialization.Instance.Serialize(Scale, buffer);
		}
	}
}
