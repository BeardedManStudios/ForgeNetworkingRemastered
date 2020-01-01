using System;
using Forge.Serialization;
using UnityEngine;

namespace Forge.Networking.Unity
{
	public class QuaternionSerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			return new Quaternion(buffer.GetBasicType<float>(),
				buffer.GetBasicType<float>(),
				buffer.GetBasicType<float>(),
				buffer.GetBasicType<float>());
		}

		public byte[] Serialize(object val)
		{
			var quat = (Quaternion)val;
			byte[] bytes = new byte[sizeof(float) * 3];
			Buffer.BlockCopy(BitConverter.GetBytes(quat.x), 0, bytes, 0, sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(quat.y), 0, bytes, sizeof(float), sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(quat.z), 0, bytes, sizeof(float) * 2, sizeof(float));
			Buffer.BlockCopy(BitConverter.GetBytes(quat.w), 0, bytes, sizeof(float) * 3, sizeof(float));
			return bytes;
		}
	}
}
