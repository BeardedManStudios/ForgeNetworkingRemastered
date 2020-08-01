using Forge.Serialization;

namespace Forge.DataStructures
{
	public class ForgeSignature : ISignature
	{
		private int _id;

		public ForgeSignature(ISignatureGenerator<int> generator)
		{
			_id = generator.Generate();
		}

		public void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(_id, buffer);
		}

		public void Deserialize(BMSByte buffer)
		{
			_id = ForgeSerializer.Instance.Deserialize<int>(buffer);
		}

		public override bool Equals(object obj)
		{
			return _id == ((ForgeSignature)obj)._id;
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}

		public override string ToString()
		{
			return _id.ToString();
		}

		public void Initialize(ISignatureGenerator<int> generator)
		{
			throw new System.NotImplementedException();
		}

		public static bool operator ==(ForgeSignature lhs, ForgeSignature rhs)
		{
			return lhs._id == rhs._id;
		}

		public static bool operator !=(ForgeSignature lhs, ForgeSignature rhs)
		{
			return lhs._id != rhs._id;
		}
	}
}
