using System;
using Forge.Serialization;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public class Vector3Serializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return new Vector3(buffer.GetBasicType<float>(),
				buffer.GetBasicType<float>(),
				buffer.GetBasicType<float>());
		}

		public byte[] Serialize(object val)
		{
			var vec = (Vector3)val;
			byte[] bytes = new byte[sizeof(float) * 3];
			Buffer.BlockCopy(BitConverter.GetBytes(vec.x), 0, bytes, 0, sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(vec.y), 0, bytes, sizeof(float), sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(vec.z), 0, bytes, sizeof(float) * 2, sizeof(float));
			return bytes;
		}
	}
}
