using System;

namespace Forge.Networking.Messaging
{
	public abstract class ForgeMessage : IMessage
	{
		public int MessageCode { get; set; }

		private ForgeMessageReceipt _receipt;

		public IMessageReceipt Receipt
		{
			get => _receipt;
			set { _receipt = (ForgeMessageReceipt)value; }
		}

		public abstract IMessageInterpreter Interpreter { get; }
		public abstract void SerializeData(BMSByte buffer);
		public abstract void DeserializeData(BMSByte buffer);

		public void Interpret(INetworkHost host)
		{
			Interpreter.Interpret(host, this);
		}

		public byte[] Serialize()
		{
			var buffer = new BMSByte();
			buffer.SetSize(128);
			ObjectMapper.Instance.MapBytes(buffer, MessageCode, Receipt?.Signature.ToString() ?? "");
			SerializeData(buffer);
			return buffer.CompressBytes();
		}

		public static IMessage Deserialize(byte[] message)
		{
			var buffer = new BMSByte();
			buffer.BlockCopy(message, 0, message.Length);
			int code = buffer.GetBasicType<int>();
			var m = (IMessage)ForgeMessageCodes.Instantiate(code);
			m.MessageCode = code;
			string guid = buffer.GetBasicType<string>();
			if (guid.Length > 0)
			{
				m.Receipt = new ForgeMessageReceipt
				{
					Signature = Guid.Parse(guid)
				};
			}
			((ForgeMessage)m).DeserializeData(buffer);
			return m;
		}
	}
}
