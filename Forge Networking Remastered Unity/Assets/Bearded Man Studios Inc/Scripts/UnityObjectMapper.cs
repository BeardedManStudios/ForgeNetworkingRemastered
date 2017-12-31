/*-----------------------------+-------------------------------\
|                                                              |
|                         !!!NOTICE!!!                         |
|                                                              |
|  These libraries are under heavy development so they are     |
|  subject to make many changes as development continues.      |
|  For this reason, the libraries may not be well commented.   |
|  THANK YOU for supporting forge with all your feedback       |
|  suggestions, bug reports and comments!                      |
|                                                              |
|                              - The Forge Team                |
|                                Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/



using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public class UnityObjectMapper : ObjectMapper
	{
		private float x, y, z, w;

		private new static UnityObjectMapper instance = null;
		public static new UnityObjectMapper Instance
		{
			get
			{
				if (instance != null)
					return instance;

				instance = new UnityObjectMapper();
				return instance;
			}
		}

		protected UnityObjectMapper() { }

		/// <summary>
		/// Map a type of object from a FrameStream to a object
		/// </summary>
		/// <param name="type">Type of object to map</param>
		/// <param name="stream">FrameStream to be used</param>
		/// <returns>Returns the mapped object</returns>
		public override object Map(Type type, BMSByte stream)
		{
			object obj = null;

			if (type == typeof(Vector2))
				obj = MapVector2(stream);
			else if (type == typeof(Vector3))
				obj = MapVector3(stream);
			else if (type == typeof(Vector4) || type == typeof(Color) || type == typeof(Quaternion))
				obj = MapVector4(type, stream);
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

			if (genericType == typeof(Vector2))
				obj = MapVector2(stream);
			else if (genericType == typeof(Vector3))
				obj = MapVector3(stream);
			else if (genericType == typeof(Vector4) || genericType == typeof(Color) || genericType == typeof(Quaternion))
				obj = MapVector4(genericType, stream);
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
			if (type == typeof(Vector2))
			{
				bytes.Append(BitConverter.GetBytes(((Vector2)o).x));
				bytes.Append(BitConverter.GetBytes(((Vector2)o).y));
			}
			else if (type == typeof(Vector3))
			{
				bytes.Append(BitConverter.GetBytes(((Vector3)o).x));
				bytes.Append(BitConverter.GetBytes(((Vector3)o).y));
				bytes.Append(BitConverter.GetBytes(((Vector3)o).z));
			}
			else if (type == typeof(Vector4))
			{
				bytes.Append(BitConverter.GetBytes(((Vector4)o).x));
				bytes.Append(BitConverter.GetBytes(((Vector4)o).y));
				bytes.Append(BitConverter.GetBytes(((Vector4)o).z));
				bytes.Append(BitConverter.GetBytes(((Vector4)o).w));
			}
			else if (type == typeof(Color))
			{
				bytes.Append(BitConverter.GetBytes(((Color)o).r));
				bytes.Append(BitConverter.GetBytes(((Color)o).g));
				bytes.Append(BitConverter.GetBytes(((Color)o).b));
				bytes.Append(BitConverter.GetBytes(((Color)o).a));
			}
			else if (type == typeof(Quaternion))
			{
				bytes.Append(BitConverter.GetBytes(((Quaternion)o).x));
				bytes.Append(BitConverter.GetBytes(((Quaternion)o).y));
				bytes.Append(BitConverter.GetBytes(((Quaternion)o).z));
				bytes.Append(BitConverter.GetBytes(((Quaternion)o).w));
			}
			else
				base.GetBytes(o, type, ref bytes);
		}

		protected override byte[] GetBytesArray(object o, Type type)
		{
			byte[] data;
			byte[] buffer;
			if (type == typeof(Vector2))
			{
				data = new byte[sizeof(float) * 2];
				Buffer.BlockCopy(BitConverter.GetBytes(((Vector2)o).x), 0, data, 0, sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Vector2)o).y), 0, data, sizeof(float), sizeof(float));
				return data;
			}
			else if (type == typeof(Vector3))
			{
				data = new byte[sizeof(float) * 3];
				Buffer.BlockCopy(BitConverter.GetBytes(((Vector3)o).x), 0, data, 0, sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Vector3)o).y), 0, data, sizeof(float), sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Vector3)o).z), 0, data, sizeof(float) * 2, sizeof(float));
				return data;
			}
			else if (type == typeof(Vector4))
			{
				data = new byte[sizeof(float) * 4];
				Buffer.BlockCopy(BitConverter.GetBytes(((Vector4)o).x), 0, data, 0, sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Vector4)o).y), 0, data, sizeof(float), sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Vector4)o).z), 0, data, sizeof(float) * 2, sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Vector4)o).w), 0, data, sizeof(float) * 3, sizeof(float));
				return data;
			}
			else if (type == typeof(Color))
			{
				data = new byte[sizeof(float) * 4];
				Buffer.BlockCopy(BitConverter.GetBytes(((Color)o).r), 0, data, 0, sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Color)o).g), 0, data, sizeof(float), sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Color)o).b), 0, data, sizeof(float) * 2, sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Color)o).a), 0, data, sizeof(float) * 3, sizeof(float));
				return data;
			}
			else if (type == typeof(Quaternion))
			{
				data = new byte[sizeof(float) * 4];
				Buffer.BlockCopy(BitConverter.GetBytes(((Quaternion)o).x), 0, data, 0, sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Quaternion)o).y), 0, data, sizeof(float), sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Quaternion)o).z), 0, data, sizeof(float) * 2, sizeof(float));
				Buffer.BlockCopy(BitConverter.GetBytes(((Quaternion)o).w), 0, data, sizeof(float) * 3, sizeof(float));
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
		public object MapVector2(BMSByte stream)
		{
			return new Vector2(
				stream.GetBasicType<float>(),
				stream.GetBasicType<float>()
			);
		}

		/// <summary>
		/// Get a Vector3 out of a FrameStream
		/// </summary>
		/// <param name="stream">FrameStream to be used</param>
		/// <returns>A Vector3 out of the FrameStream</returns>
		public object MapVector3(BMSByte stream)
		{
			return new Vector3(
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
		public object MapVector4(Type type, BMSByte stream)
		{
			x = stream.GetBasicType<float>();
			y = stream.GetBasicType<float>();
			z = stream.GetBasicType<float>();
			w = stream.GetBasicType<float>();

			if (type == typeof(Color))
				return new Color(x, y, z, w);
			else if (type == typeof(Quaternion))
				return new Quaternion(x, y, z, w);

			return new Vector4(x, y, z, w);
		}
	}
}