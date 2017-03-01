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

#if BMS_DEBUGGING
#define BMS_DEBUGGING_UNITY
#endif

using System;
using System.Text;

namespace BeardedManStudios.Forge.Networking
{
	public class ObjectMapper
	{
		protected static ObjectMapper instance = null;
		public static ObjectMapper Instance
		{
			get
			{
				if (instance != null)
					return instance;

				instance = new ObjectMapper();
				return instance;
			}
		}

		protected ObjectMapper() { }

		protected int byteArrSize, size = 0;

		public void UseAsDefault() { instance = this; }

		/// <summary>
		/// Map a type of object from a BMSByte to a object
		/// </summary>
		/// <param name="type">Type of object to map</param>
		/// <param name="stream">BMSByte to be used</param>
		/// <returns>Returns the mapped object</returns>
		public virtual object Map(Type type, BMSByte stream)
		{
			object obj = null;

			if (type == typeof(string))
				obj = stream.GetBasicType<string>();
			else if (type == typeof(Vector))
				obj = stream.GetBasicType<Vector>();
			else if (type.IsArray)
				obj = MapArray(type, stream);
			else if (type == typeof(BMSByte))
				obj = MapBMSByte(stream);
			else if (type.IsEnum)
				obj = MapBasicType(Enum.GetUnderlyingType(type), stream);
			else
				obj = MapBasicType(type, stream);

			return obj;
		}

		/// <summary>
		/// Get a mapped value out of the BMSByte
		/// </summary>
		/// <typeparam name="T">Value to get out of it</typeparam>
		/// <param name="stream">BMSByte to be used</param>
		/// <returns>Returns a mapped value from the BMSByte</returns>
		public virtual T Map<T>(BMSByte stream)
		{
			object obj = null;
			var genericType = typeof(T);

			if (genericType == typeof(string))
				obj = stream.GetBasicType<string>();
			else if (genericType == typeof(Vector))
				obj = stream.GetBasicType<Vector>();
			else if (genericType.IsArray)
				obj = MapArray(genericType, stream);
			else if (genericType == typeof(BMSByte))
				obj = MapBMSByte(stream);
			else if (genericType.IsEnum)
				obj = MapBasicType(Enum.GetUnderlyingType(genericType), stream);
			else
				obj = MapBasicType(genericType, stream);

			return (T)obj;
		}

		/// <summary>
		/// Get a mapped basic type of object from the BMSByte
		/// </summary>
		/// <param name="type">Type of object to be mapped</param>
		/// <param name="stream">BMSByte to be used</param>
		/// <returns>Returns a mapped object of the given type</returns>
		public object MapBasicType(Type type, BMSByte stream)
		{
			if (type == typeof(sbyte))
				return (sbyte)stream.GetBasicType<sbyte>();
			else if (type == typeof(byte))
				return (byte)stream.GetBasicType<byte>();
			else if (type == typeof(char))
				return (byte)stream.GetBasicType<char>();
			else if (type == typeof(short))
				return stream.GetBasicType<short>();
			else if (type == typeof(ushort))
				return stream.GetBasicType<ushort>();
			else if (type == typeof(bool))
				return stream.GetBasicType<bool>();
			else if (type == typeof(int))
				return stream.GetBasicType<int>();
			else if (type == typeof(uint))
				return stream.GetBasicType<uint>();
			else if (type == typeof(float))
				return stream.GetBasicType<float>();
			else if (type == typeof(long))
				return stream.GetBasicType<long>();
			else if (type == typeof(ulong))
				return stream.GetBasicType<ulong>();
			else if (type == typeof(double))
				return stream.GetBasicType<double>();
			else if (type == typeof(string))
				return stream.GetBasicType<string>();
			else if (type == typeof(Vector))
				return stream.GetBasicType<Vector>();
			else
				// TODO:  Make this an object mapper exception
				throw new BaseNetworkException("The type " + type.ToString() + " is not allowed to be sent over the Network (yet)");
		}

		/// <summary>
		/// Map a generic array from the BMSBytes
		/// </summary>
		/// <param name="type">The type of the array</param>
		/// <param name="bytes">The bytes to read from</param>
		/// <returns>The array that was found</returns>
		public object MapArray(Type type, BMSByte bytes)
		{
			int rank = type.GetArrayRank();
			Type targetType = type.GetElementType();

			if (targetType != typeof(byte))
				throw new Exception("Currently only byte arrays can be sent as arrays");

			if (rank > 4)
				throw new Exception("Currently the system only supports up to 4 dimensions in an array");

			int i, j, k, l;

			// Read each dimension length first
			int[] lengths = new int[rank];
			for (i = 0; i < rank; i++)
				lengths[i] = bytes.GetBasicType<int>();

			switch (rank)
			{
				case 1:
					byte[] arr1 = new byte[lengths[0]];

					for (i = 0; i < lengths[0]; i++)
						arr1[i] = bytes.GetBasicType<byte>();

					return arr1;
				case 2:
					byte[,] arr2 = new byte[lengths[0], lengths[1]];

					for (i = 0; i < lengths[0]; i++)
						for (j = 0; j < lengths[1]; j++)
							arr2[i, j] = bytes.GetBasicType<byte>();

					return arr2;
				case 3:
					byte[,,] arr3 = new byte[lengths[0], lengths[1], lengths[2]];

					for (i = 0; i < lengths[0]; i++)
						for (j = 0; j < lengths[1]; j++)
							for (k = 0; k < lengths[2]; k++)
								arr3[i, j, k] = bytes.GetBasicType<byte>();

					return arr3;
				case 4:
					byte[,,,] arr4 = new byte[lengths[0], lengths[1], lengths[2], lengths[3]];

					for (i = 0; i < lengths[0]; i++)
						for (j = 0; j < lengths[1]; j++)
							for (k = 0; k < lengths[2]; k++)
								for (l = 0; l < lengths[3]; l++)
									arr4[i, j, k, l] = bytes.GetBasicType<byte>();

					return arr4;
			}

			return null;
		}

		/// <summary>
		/// Get a BMSByte out of a BMSByte
		/// </summary>
		/// <param name="type">Type of object to be mapped</param>
		/// <param name="stream">BMSByte to be used</param>
		/// <returns>A BMSByte that was read from the BMSByte</returns>
		public object MapBMSByte(BMSByte stream)
		{
			size = Map<int>(stream);
			return new BMSByte().Clone(Map<byte[]>(stream));
		}

		/// <summary>
		/// Get a byte[] out of a BMSByte
		/// </summary>
		/// <param name="args">Arguments passed through to get mapped</param>
		/// <returns>A byte[] of the mapped arguments</returns>
		public BMSByte MapBytes(BMSByte bytes, params object[] args)
		{
			foreach (object o in args)
			{
				Type type = null;
				if (o != null)
					type = o.GetType();

				GetBytes(o, type, ref bytes);
			}

			return bytes;
		}

		/// <summary>
		/// Gets the bytes for the Instance of an Object and appends them to a <c>BMSByte</c>.
		/// </summary>
		/// <param name="o">The Instance of the Object.</param>
		/// <param name="type">The Type of the Object.</param>
		/// <param name="bytes"><c>BMSByte</c> to which the bytes should be added.</param>
		protected virtual void GetBytes(object o, Type type, ref BMSByte bytes)
		{
			if (type == typeof(string))
			{
				var strBytes = Encoding.UTF8.GetBytes(o == null ? string.Empty : (string)o);
				// TODO:  Need to make custom string serialization to binary
				bytes.Append(BitConverter.GetBytes(strBytes.Length));

				if (strBytes.Length > 0)
					bytes.Append(strBytes);
			}
			else if (type == typeof(Vector))
			{
				Vector vec = (Vector)o;
				bytes.Append(BitConverter.GetBytes(vec.x));
				bytes.Append(BitConverter.GetBytes(vec.y));
				bytes.Append(BitConverter.GetBytes(vec.z));
			}
			else if (type == null) //TODO: Check if this causes other issues
				bytes.Append(new byte[1] { 0 });
			else if (type == typeof(sbyte))
				bytes.BlockCopy<sbyte>(o, 1);
			else if (type == typeof(byte))
				bytes.BlockCopy<byte>(o, 1);
			else if (type == typeof(char))
				bytes.BlockCopy<char>(o, 1);
			else if (type == typeof(bool))
				bytes.Append(BitConverter.GetBytes((bool)o));
			else if (type == typeof(short))
				bytes.Append(BitConverter.GetBytes((short)o));
			else if (type == typeof(ushort))
				bytes.Append(BitConverter.GetBytes((ushort)o));
			else if (type == typeof(int))
				bytes.Append(BitConverter.GetBytes((int)o));
			else if (type == typeof(uint))
				bytes.Append(BitConverter.GetBytes((uint)o));
			else if (type == typeof(long))
				bytes.Append(BitConverter.GetBytes((long)o));
			else if (type == typeof(ulong))
				bytes.Append(BitConverter.GetBytes((ulong)o));
			else if (type == typeof(float))
				bytes.Append(BitConverter.GetBytes((float)o));
			else if (type == typeof(double))
				bytes.Append(BitConverter.GetBytes((double)o));
			else if (type.IsArray)
			{
				int rank = type.GetArrayRank();
				Type targetType = type.GetElementType();

				if (targetType != typeof(byte))
					throw new Exception("Currently only byte arrays can be sent as arrays");

				if (rank > 4)
					throw new Exception("Currently the system only supports up to 4 dimensions in an array");

				int i, j, k, l;

				// Write each dimension length first
				int[] lengths = new int[rank];
				for (i = 0; i < rank; i++)
				{
					lengths[i] = ((Array)o).GetLength(i);
					bytes.Append(BitConverter.GetBytes(lengths[i]));
				}

				switch (rank)
				{
					case 1:
						for (i = 0; i < lengths[0]; i++)
							GetBytes(((Array)o).GetValue(i), targetType, ref bytes);
						break;
					case 2:
						for (i = 0; i < lengths[0]; i++)
							for (j = 0; j < lengths[1]; j++)
								GetBytes(((Array)o).GetValue(i, j), targetType, ref bytes);
						break;
					case 3:
						for (i = 0; i < lengths[0]; i++)
							for (j = 0; j < lengths[1]; j++)
								for (k = 0; k < lengths[2]; k++)
									GetBytes(((Array)o).GetValue(i, j, k), targetType, ref bytes);
						break;
					case 4:
						for (i = 0; i < lengths[0]; i++)
							for (j = 0; j < lengths[1]; j++)
								for (k = 0; k < lengths[2]; k++)
									for (l = 0; l < lengths[3]; l++)
										GetBytes(((Array)o).GetValue(i, j, k, l), targetType, ref bytes);
						break;
				}
			}
			else if (type == typeof(BMSByte))
			{
				bytes.Append(BitConverter.GetBytes(((BMSByte)o).Size));
				bytes.BlockCopy(((BMSByte)o).byteArr, ((BMSByte)o).StartIndex(), ((BMSByte)o).Size);
			}
			else if (type.IsEnum)
				GetBytes(o, Enum.GetUnderlyingType(type), ref bytes);
			else
			{
				// TODO:  Make this a more appropriate exception
				throw new BaseNetworkException("The type " + type.ToString() + " is not allowed to be sent over the Network (yet)");
			}
		}
	}
}