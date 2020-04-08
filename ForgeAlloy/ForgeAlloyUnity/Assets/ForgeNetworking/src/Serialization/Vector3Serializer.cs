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

		public void Serialize(object val, BMSByte buffer)
		{
			var vec = (Vector3)val;
			buffer.Append(BitConverter.GetBytes(vec.x));
			buffer.Append(BitConverter.GetBytes(vec.y));
			buffer.Append(BitConverter.GetBytes(vec.z));
		}
	}
}
