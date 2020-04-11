using System;

namespace Forge.Networking.Messaging
{
	public class MessageContractAttribute : Attribute
	{
		internal const int SERVER_LISTING_CODE_OFFSET = 0x7FFFFFFF - 3000;
		internal const int NAT_HOLE_PUNCH_CODE_OFFSET = 0x7FFFFFFF - 2000;
		internal const int UNIT_TEST_CODE_OFFSET = 0x7FFFFFFF - 1000;

		protected int inputId;
		protected int id;
		protected Type type;

		protected MessageContractAttribute() { }
		internal MessageContractAttribute(int id, Type type)
		{
			this.id = id;
			this.type = type;
		}
		internal int GetId() => id;
		internal int GetInputId() => inputId;
		internal Type GetClassType() => type;
	}

	public sealed class UnitTestingMessageContract : MessageContractAttribute
	{
		public UnitTestingMessageContract(int id, Type type)
			: base(id + UNIT_TEST_CODE_OFFSET, type) { inputId = id; }
	}

	public sealed class ServerListingMessageContractAttribute : MessageContractAttribute
	{
		public ServerListingMessageContractAttribute(int id, Type type)
			: base(id + SERVER_LISTING_CODE_OFFSET, type) { inputId = id; }
	}

	public sealed class NatHolePunchMessageContractAttribute : MessageContractAttribute
	{
		public NatHolePunchMessageContractAttribute(int id, Type type)
			: base(id + NAT_HOLE_PUNCH_CODE_OFFSET, type) { inputId = id; }
	}

	public sealed class EngineMessageContractAttribute : MessageContractAttribute
	{
		public EngineMessageContractAttribute(int id, Type type)
			: base(-id, type) { inputId = id; }
	}
}
