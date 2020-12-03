using System;

namespace BeardedManStudios.Forge.Networking.StandAlone
{
	public class StandAloneObjectMapper : ObjectMapper
	{
		private float x, y, z, w;

		private new static StandAloneObjectMapper instance = null;
		public static new StandAloneObjectMapper Instance
		{
			get
			{
				if (instance != null)
					return instance;

				instance = new StandAloneObjectMapper();
				return instance;
			}
		}

		protected StandAloneObjectMapper() { }

		/// <summary>
		/// Map a type of object from a FrameStream to a object
		/// </summary>
		/// <param name="type">Type of object to map</param>
		/// <param name="stream">FrameStream to be used</param>
		/// <returns>Returns the mapped object</returns>
		public override object Map(Type type, BMSByte stream)
		{
			object obj = null;

			if (type == typeof(Float2))
				obj = MapFloat2(stream);
			else if (type == typeof(Float3))
				obj = MapFloat3(stream);
			else if (type == typeof(Float4))
				obj = MapFloat4(stream);
			else
				obj = base.Map(type, stream);

			return obj;
		}

		/// <summary>
		/// Get a mapped value out of the FrameStream
		/// </summary>
		/// <typeparam name="T">Value to get out of it</typeparam>
		/// <param name="stream">FrameStream to be used</param>
		/// <returns>Returns a mapped value from the FrameStream</returns>
		public override T Map<T>(BMSByte stream)
		{
			object obj = null;
			var genericType = typeof(T);

			if (genericType == typeof(Float2))
				obj = MapFloat2(stream);
			else if (genericType == typeof(Float3))
				obj = MapFloat3(stream);
			else if (genericType == typeof(Float4))
				obj = MapFloat4(stream);
			else
				obj = base.Map<T>(stream);

			return (T)obj;
		}

		/// <summary>
		/// Gets the bytes for the Instance of an Object and appends them to a <c>BMSByte</c>.
		/// </summary>
		/// <param name="o">The Instance of the Object.</param>
		/// <param name="type">The Type of the Object.</param>
		/// <param name="bytes"><c>BMSByte</c> to which the bytes should be added.</param>
		protected override void GetBytes(object o, Type type, ref BMSByte bytes)
		{
			if (type == typeof(Float2))
			{
				bytes.Append(BitConverter.GetBytes(((Float2)o).x));
				bytes.Append(BitConverter.GetBytes(((Float2)o).y));
			}
			else if (type == typeof(Float3))
			{
				bytes.Append(BitConverter.GetBytes(((Float3)o).x));
				bytes.Append(BitConverter.GetBytes(((Float3)o).y));
				bytes.Append(BitConverter.GetBytes(((Float3)o).z));
			}
			else if (type == typeof(Float4))
			{
				bytes.Append(BitConverter.GetBytes(((Float4)o).x));
				bytes.Append(BitConverter.GetBytes(((Float4)o).y));
				bytes.Append(BitConverter.GetBytes(((Float4)o).z));
				bytes.Append(BitConverter.GetBytes(((Float4)o).w));
			}
			else
				base.GetBytes(o, type, ref bytes);
		}

		protected override byte[] GetBytesArray(object o, Type type)
		{
			byte[] data;
			if (type == typeof(Float2))
			{
				data = new byte[sizeof(float) * 2];
				Buffer.BlockCopy(BitConverter.GetBytes(((Float2)o).x), 0, data, 0, sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Float2)o).y), 0, data, sizeof(float), sizeof(float));
				return data;
			}
			else if (type == typeof(Float3))
			{
				data = new byte[sizeof(float) * 3];
				Buffer.BlockCopy(BitConverter.GetBytes(((Float3)o).x), 0, data, 0, sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Float3)o).y), 0, data, sizeof(float), sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Float3)o).z), 0, data, sizeof(float) * 2, sizeof(float));
				return data;
			}
			else if (type == typeof(Float4))
			{
				data = new byte[sizeof(float) * 4];
				Buffer.BlockCopy(BitConverter.GetBytes(((Float4)o).x), 0, data, 0, sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Float4)o).y), 0, data, sizeof(float), sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Float4)o).z), 0, data, sizeof(float) * 2, sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Float4)o).w), 0, data, sizeof(float) * 3, sizeof(float));
				return data;
			}
			else
				return base.GetBytesArray(o, type);
		}
		
		/// <summary>
		/// Get a Vector2 out of a FrameStream
		/// </summary>
		/// <param name="stream">FrameStream to be used</param>
		/// <returns>A Vector2 out of the FrameStream</returns>
		public object MapFloat2(BMSByte stream)
		{
			return new Float2(
				stream.GetBasicType<float>(),
				stream.GetBasicType<float>()
			);
		}

		/// <summary>
		/// Get a Vector3 out of a FrameStream
		/// </summary>
		/// <param name="stream">FrameStream to be used</param>
		/// <returns>A Vector3 out of the FrameStream</returns>
		public object MapFloat3(BMSByte stream)
		{
			return new Float3(
				stream.GetBasicType<float>(),
				stream.GetBasicType<float>(),
				stream.GetBasicType<float>()
			);
		}

		/// <summary>
		/// Get a Vector4 out of a FrameStream
		/// </summary>
		/// <param name="type">Type of object to be mapped</param>
		/// <param name="stream">FrameStream to be used</param>
		/// <returns>A type of Vector4 (Vector4/Color/Quaternion) out of the FrameStream</returns>
		public object MapFloat4(BMSByte stream)
		{
			x = stream.GetBasicType<float>();
			y = stream.GetBasicType<float>();
			z = stream.GetBasicType<float>();
			w = stream.GetBasicType<float>();
			
			return new Float4(x, y, z, w);
		}
	}
}
