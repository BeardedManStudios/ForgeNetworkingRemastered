using Forge.DataStructures;

namespace Forge.Networking.Messaging
{
	public class ForgeMessageReceipt : IMessageReceipt
	{
		public ISignature Signature { get; set; }

		public ForgeMessageReceipt()
		{
			Signature = new ForgeSignature();
		}

		public override bool Equals(object obj)
		{
			return Signature == ((ForgeMessageReceipt)obj).Signature;
		}

		public override string ToString()
		{
			return Signature.ToString();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(ForgeMessageReceipt current, ForgeMessageReceipt other)
		{
			return current.Signature == other.Signature;
		}

		public static bool operator !=(ForgeMessageReceipt current, ForgeMessageReceipt other)
		{
			return current.Signature != other.Signature;
		}
	}
}
