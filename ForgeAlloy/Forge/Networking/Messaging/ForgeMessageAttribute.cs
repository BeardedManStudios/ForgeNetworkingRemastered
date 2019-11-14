using System;

namespace Forge.Networking.Messaging
{
	public class MessageContractAttribute : Attribute
	{
		internal const int SERVER_LISTING_CODE_OFFSET = 50000;
		internal const int NAT_HOLE_PUNCH_CODE_OFFSET = 60000;
		internal const int CLIENT_CODE_OFFSET = 10000000;

		protected int id;
		protected Type type;

		protected MessageContractAttribute() { }
		internal MessageContractAttribute(int id, Type type)
		{
			this.id = id;
			this.type = type;
		}
		internal int GetId() => id;
		internal Type GetClassType() => type;
	}

	public class ServerListingMessageContractAttribute : MessageContractAttribute
	{
		public ServerListingMessageContractAttribute(int id, Type type)
			: base(id + SERVER_LISTING_CODE_OFFSET, type) { }
	}

	public class UnitTestingMessageContractAttribute : MessageContractAttribute
	{
		public UnitTestingMessageContractAttribute(int id, Type type)
			: base(-id, type) { }
	}

	public class NatHolePunchMessageContractAttribute : MessageContractAttribute
	{
		public NatHolePunchMessageContractAttribute(int id, Type type)
			: base(id + NAT_HOLE_PUNCH_CODE_OFFSET, type) { }
	}

	public class EngineMessageContractAttribute : MessageContractAttribute
	{
		public EngineMessageContractAttribute(int id, Type type)
			: base(id + NAT_HOLE_PUNCH_CODE_OFFSET, type) { }
	}
}
