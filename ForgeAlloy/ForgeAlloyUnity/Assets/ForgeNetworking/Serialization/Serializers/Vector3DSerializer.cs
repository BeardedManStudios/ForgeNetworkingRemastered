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

		public void Serialize(object val, BMSByte buffer)
		{
			var vec = (Vector3D)val;
			buffer.Append(BitConverter.GetBytes(vec.X));
			buffer.Append(BitConverter.GetBytes(vec.Y));
			buffer.Append(BitConverter.GetBytes(vec.Z));
		}
	}
}
