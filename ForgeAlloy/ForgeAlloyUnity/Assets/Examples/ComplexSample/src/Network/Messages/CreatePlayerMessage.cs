using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Unity.Messages;
using Forge.Serialization;
using Puzzle.Networking.Messages.Interpreters;
using UnityEngine;

namespace Puzzle.Networking.Messages
{
	[EngineMessageContract(10, typeof(CreatePlayerMessage))]
	public class CreatePlayerMessage : SpawnEntityMessage
	{
		public int ProxyPrefabId { get; set; }
		public IPlayerSignature OwningPlayer { get; set; }

		public override IMessageInterpreter Interpreter => CreatePlayerInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			Id = ForgeSerializer.Instance.Deserialize<int>(buffer);
			PrefabId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			ProxyPrefabId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			Position = ForgeSerializer.Instance.Deserialize<Vector3>(buffer);
			Rotation = ForgeSerializer.Instance.Deserialize<Quaternion>(buffer);
			Scale = ForgeSerializer.Instance.Deserialize<Vector3>(buffer);
			OwningPlayer = ForgeSerializer.Instance.Deserialize<IPlayerSignature>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(Id, buffer);
			ForgeSerializer.Instance.Serialize(PrefabId, buffer);
			ForgeSerializer.Instance.Serialize(ProxyPrefabId, buffer);
			ForgeSerializer.Instance.Serialize(Position, buffer);
			ForgeSerializer.Instance.Serialize(Rotation, buffer);
			ForgeSerializer.Instance.Serialize(Scale, buffer);
			ForgeSerializer.Instance.Serialize(OwningPlayer, buffer);
		}
	}
}
