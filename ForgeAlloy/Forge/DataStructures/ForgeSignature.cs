using System;

namespace Forge.DataStructures
{
	public class ForgeSignature : ISignature
	{
		private Guid _guid;

		public ForgeSignature()
		{
			_guid = Guid.NewGuid();
		}

		public byte[] Serialize()
		{
			return _guid.ToByteArray();
		}

		public void Deserialize(byte[] data)
		{
			_guid = new Guid(data);
		}

		public override bool Equals(object obj)
		{
			return _guid == ((ForgeSignature)obj)._guid;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return _guid.ToString();
		}

		public static bool operator ==(ForgeSignature lhs, ForgeSignature rhs)
		{
			return lhs._guid == rhs._guid;
		}

		public static bool operator !=(ForgeSignature lhs, ForgeSignature rhs)
		{
			return lhs._guid != rhs._guid;
		}
	}
}
