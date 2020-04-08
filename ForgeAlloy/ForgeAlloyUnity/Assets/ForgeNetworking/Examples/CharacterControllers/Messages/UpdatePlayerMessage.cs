using Forge.Networking.Messaging;
using Forge.Serialization;
using Puzzle.Networking.Messages.Interpreters;
using UnityEngine;

namespace Puzzle.Networking.Messages
{
	[EngineMessageContract(12, typeof(UpdatePlayerMessage))]
	public class UpdatePlayerMessage : ForgeMessage
	{
		public int EntityId { get; set; }
		public Vector3 Position { get; set; }
		public float RotationX { get; set; }
		public float RotationY { get; set; }

		public override IMessageInterpreter Interpreter => UpdatePlayerInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			EntityId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			Position = ForgeSerializer.Instance.Deserialize<Vector3>(buffer);
			RotationX = ForgeSerializer.Instance.Deserialize<float>(buffer);
			RotationY = ForgeSerializer.Instance.Deserialize<float>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(EntityId, buffer);
			ForgeSerializer.Instance.Serialize(Position, buffer);
			ForgeSerializer.Instance.Serialize(RotationX, buffer);
			ForgeSerializer.Instance.Serialize(RotationY, buffer);
		}
	}
}
