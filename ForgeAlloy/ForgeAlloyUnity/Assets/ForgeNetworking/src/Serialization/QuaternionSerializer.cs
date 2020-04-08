using System;
using Forge.Serialization;
using UnityEngine;

namespace Forge.Networking.Unity.Serialization
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

		public void Serialize(object val, BMSByte buffer)
		{
			var quat = (Quaternion)val;
			buffer.Append(BitConverter.GetBytes(quat.x));
			buffer.Append(BitConverter.GetBytes(quat.y));
			buffer.Append(BitConverter.GetBytes(quat.z));
			buffer.Append(BitConverter.GetBytes(quat.w));
		}
	}
}
