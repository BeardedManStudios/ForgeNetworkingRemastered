using Forge.Engine;
using Forge.Networking.Messaging.Messages;
using Forge.Serialization;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public class EntityStartupMessage : ForgeEntityMessage
	{
		public Vector3 Position { get; set; }
		public Vector3 Rotation { get; set; }
		public Vector3 Scale { get; set; }

		public override void ProcessUsing(IEntity entity)
		{
			var unityEntity = entity as MonoBehaviour;
			var transform = unityEntity.transform;
			transform.position = Position;
			transform.rotation = Quaternion.Euler(Rotation);
			transform.localScale = Scale;
		}

		public override void Deserialize(BMSByte buffer)
		{
			base.Deserialize(buffer);

			Position = ForgeSerializationStrategy.Instance.Deserialize<Vector3>(buffer);
			Rotation = ForgeSerializationStrategy.Instance.Deserialize<Vector3>(buffer);
			Scale = ForgeSerializationStrategy.Instance.Deserialize<Vector3>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			base.Serialize(buffer);

			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Position));
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Rotation));
			buffer.Append(ForgeSerializationStrategy.Instance.Serialize(Scale));
		}
	}
}
