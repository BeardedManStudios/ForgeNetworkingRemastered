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
			// TODO:  Finish
		}

		public override void Serialize(BMSByte buffer)
		{
			// TODO:  Finish
		}
	}
}
