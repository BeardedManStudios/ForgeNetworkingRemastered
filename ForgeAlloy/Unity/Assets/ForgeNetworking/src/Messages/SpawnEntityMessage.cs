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
			Id = ForgeSerializationStrategy.Instance.Deserialize<int>(buffer);
			PrefabId = ForgeSerializationStrategy.Instance.Deserialize<int>(buffer);
			Position = ForgeSerializationStrategy.Instance.Deserialize<Vector3>(buffer);
			Rotation = ForgeSerializationStrategy.Instance.Deserialize<Quaternion>(buffer);
			Scale = ForgeSerializationStrategy.Instance.Deserialize<Vector3>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Id));
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(PrefabId));
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Position));
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Rotation));
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Scale));
		}
	}
}
