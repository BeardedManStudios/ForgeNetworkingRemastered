using System;
using Forge.DataStructures;

namespace Forge.Serialization.Serializers
{
	public class Vector3DSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return buffer.GetBasicType<Vector3D>();
		}

		public byte[] Serialize(object val)
		{
			var vec = (Vector3D)val;
			byte[] bytes = new byte[sizeof(float) * 3];
			Buffer.BlockCopy(BitConverter.GetBytes(vec.X), 0, bytes, 0, sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(vec.Y), 0, bytes, sizeof(float), sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(vec.Z), 0, bytes, sizeof(float) * 2, sizeof(float));
			return bytes;
		}
	}
}
