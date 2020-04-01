using Forge.Serialization;

namespace Forge.DataStructures
{
	public class ForgeSignature : ISignature
	{
		private static int _num = 0;
		private static object _numMutex = new object();

		private static int NextId()
		{
			lock (_numMutex)
			{
				return _num++;
			}
		}

		private int _id;

		public ForgeSignature()
		{
			_id = NextId();
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
