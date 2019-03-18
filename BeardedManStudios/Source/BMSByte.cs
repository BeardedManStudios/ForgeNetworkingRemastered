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
using System.Collections.Generic;
using System.Text;

namespace BeardedManStudios
{
	/// <summary>
	/// This is the main byte wrapper class for manipulating cached bytes
	/// </summary>
	public sealed class BMSByte
	{
		/// <summary>
		/// A lookup of various types that are allowed to be stored and pulled from this object
		/// </summary>
		private static Dictionary<Type, Array> allowedTypes = new Dictionary<Type, Array>()
		{
			{ typeof(char), new char[1] },
			{ typeof(string), new string[1] },
			{ typeof(bool), new bool[1] },
			{ typeof(sbyte), new sbyte[1] },
			{ typeof(byte), new byte[1] },
			{ typeof(short), new short[1] },
			{ typeof(ushort), new ushort[1] },
			{ typeof(int), new int[1] },
			{ typeof(uint), new uint[1] },
			{ typeof(long), new long[1] },
			{ typeof(ulong), new ulong[1] },
			{ typeof(float), new float[1] },
			{ typeof(double), new double[1] },
		};

		/// <summary>
		/// The current read/write index for this object
		/// </summary>
		private int index = 0;

		/// <summary>
		/// The starting byte for the internal byte array
		/// </summary>
		public int StartPointer { get; private set; }

		/// <summary>
		/// The interpreted size of the internal byte array
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// The internal byte array
		/// </summary>
		public byte[] byteArr = new byte[1];

		/// <summary>
		/// Constructs a BMSByte
		/// </summary>
		/// <param name="raw">This determines if this is being used for WriteRaw</param>
		public BMSByte()
		{
			Size = 0;
			StartPointer = 0;
		}

		/// <summary>
		/// Manually set the interpreted size of the interal byte array (will resize if larger)
		/// </summary>
		/// <param name="newSize">The size to be resized to</param>
		public void SetSize(int newSize)
		{
			if (byteArr.Length < newSize)
				Array.Resize<byte>(ref byteArr, newSize);

			Size = newSize;
		}

		/// <summary>
		/// Get the starting index (virtual head) of the internal byte array
		/// </summary>
		/// <param name="offset">Adds this amount to the internal index to start from there</param>
		/// <returns></returns>
		public int StartIndex(int offset = 0)
		{
			return StartPointer + offset;
		}

		/// <summary>
		/// Moves the starting index (virtual head) manually
		/// </summary>
		/// <param name="offset">The amount to move the starting index (virtual head) by</param>
		public int MoveStartIndex(int offset)
		{
			StartPointer += offset;

			if (index < StartPointer)
				index = StartPointer;

			Size -= offset;

			return StartPointer;
		}

		/// <summary>
		/// Resets the internal byte array and assignes the passed byte array to it
		/// </summary>
		/// <param name="input">The bytes to be cloned into the internal byte array</param>
		public BMSByte Clone(byte[] input, int count = 0)
		{
			if (count == 0)
				count = input.Length;

			PointToStart();

			if (byteArr.Length <= count)
				Array.Resize<byte>(ref byteArr, count + 1);

			Buffer.BlockCopy(input, 0, byteArr, index, count);
			Size = count;
			PointToEnd();

			return this;
		}

		/// <summary>
		/// Resets the internal byte array and assignes the internal byte array of the passed in BMSByte to this ones internal byte array
		/// </summary>
		/// <param name="otherBytes">The other BMSByte that will have its internal byte array cloned to this one</param>
		/// <returns></returns>
		public BMSByte Clone(BMSByte otherBytes)
		{
			PointToStart();

			if (byteArr.Length <= otherBytes.Size)
				Array.Resize<byte>(ref byteArr, otherBytes.Size + 1);

			Buffer.BlockCopy(otherBytes.byteArr, otherBytes.StartIndex(), byteArr, StartIndex() + index, otherBytes.Size);

			Size = otherBytes.Size;
			PointToEnd();

			return this;
		}

		/// <summary>
		/// Adds the bytes to the end of the byte array and resizes when needed
		/// </summary>
		/// <param name="input">The bytes to be appended to the end of the internal byte array</param>
		public void Append(byte[] input)
		{
			if (byteArr.Length <= index + input.Length)
				Array.Resize<byte>(ref byteArr, index + input.Length + 1);

			Buffer.BlockCopy(input, 0, byteArr, index, input.Length);

			Size += input.Length;

			PointToEnd();
		}


		/// <summary>
		/// Adds the arrays of bytes to the end of the byte array and resizes only ONCE
		/// </summary>
		/// <param name="input">The bytes arrays to be appended to the end of the internal byte array</param>
		public void Append(params byte[][] input)
		{
			int lengthSum = 0;
			foreach (byte[] array in input)
			{
				lengthSum += array.Length;
			}

			if (byteArr.Length <= index + lengthSum)
				Array.Resize<byte>(ref byteArr, index + lengthSum + 1);

			foreach (byte[] array in input)
			{
				Buffer.BlockCopy(array, 0, byteArr, index, array.Length);
				Size += array.Length;
				PointToEnd();
			}

		}


		/// <summary>
		/// Adds the bytes to the end of the byte array and resizes when needed
		/// </summary>
		/// <param name="input">The BMSByte where the bytes will be pulled and appended to the end of this internal byte array</param>
		public void Append(BMSByte input)
		{
			if (input == null)
				return;

			if (byteArr.Length <= index + input.Size)
				Array.Resize<byte>(ref byteArr, index + input.Size + 1);

			Buffer.BlockCopy(input.byteArr, input.StartIndex(), byteArr, index, input.Size);

			Size += input.Size;

			PointToEnd();
		}

		/// <summary>
		/// Copies the passed byte array starting at the start index and for a specified count to
		/// the end of the internal byte array and resizes when needed
		/// </summary>
		/// <param name="input">The byte array to be appended to the end of the internal byte array</param>
		/// <param name="start">What index to start copying from on the passed byte array</param>
		/// <param name="count">How many bytes to read from the start of the passed byte array</param>
		public void BlockCopy(byte[] input, int start, int count)
		{
			if (count == 0)
				return;

			if (byteArr.Length <= index + count)
				Array.Resize<byte>(ref byteArr, count + Size + 1);

			Buffer.BlockCopy(input, start, byteArr, index, count);
			Size += count;
			PointToEnd();
		}

		/// <summary>
		/// Copies the passed object's bytes starting at the start index and for a specified count to
		/// the end of the internal byte array and resizes when needed
		/// </summary>
		/// <param name="obj">The object to be converted into bytes</param>
		/// <param name="count">The size in bytes to copy from the start of the passed object</param>
		public void BlockCopy<T>(object obj, int count)
		{
			if (count == 0)
				return;

			if (byteArr.Length <= index + count)
				Array.Resize<byte>(ref byteArr, Size + count + 1);

			if (obj is string)
				Buffer.BlockCopy(Encoding.UTF8.GetBytes(((string)obj)), 0, byteArr, index, count);
			else
				Buffer.BlockCopy(GetArray<T>(obj), 0, byteArr, index, count);

			Size = index + count;
			PointToEnd();
		}

		/// <summary>
		/// Removes a range of bytes from the internal byte array
		/// </summary>
		/// <param name="count">How many bytes to be removed from the start</param>
		public void RemoveStart(int count)
		{
			if (count > byteArr.Length)
				throw new Exception("Can't remove more than what is in the array");

			StartPointer += count;

			Size -= count;

			if (index < StartPointer)
				index = StartPointer;
		}

		/// <summary>
		/// Remove a range of bytes from the internal byte array, this will iterate
		/// and rearrange so using it often is not suggested, use RemoveStart when you can
		/// </summary>
		/// <param name="start">The start index for the internal byte array to begin removing bytes from</param>
		/// <param name="count">The amount of bytes to remove starting from the passed in start</param>
		public void RemoveRange(int start, int count)
		{
			if (start + count > byteArr.Length)
				throw new Exception("Can't remove more than what is in the array");

			for (int i = 0; i < byteArr.Length - count - start - 1; i++)
				byteArr[start + i] = byteArr[start + count + i];

			Size -= count;
			index -= count;
		}

		/// <summary>
		/// Compresses the byte array so that it is sent properly across the Network
		/// This is often used just as you are sending the message
		/// </summary>
		/// <returns>Returns this BMSByte object</returns>
		public BMSByte Compress()
		{
			if (StartPointer == 0)
				return this;

			for (int i = 0; i < byteArr.Length - StartPointer - 1; i++)
				byteArr[i] = byteArr[StartPointer + i];

			StartPointer = 0;

			return this;
		}

		/// <summary>
		/// Pulls a byte array based off of the BMSByte metadata
		/// </summary>
		/// <returns>The array of raw byte data</returns>
		public byte[] CompressBytes()
		{
			byte[] data = new byte[Size];
			Buffer.BlockCopy(byteArr, StartPointer, data, 0, data.Length);
			return data;
		}

		/// <summary>
		/// Inserts a set of bytes at a specified index into the internal byte array
		/// </summary>
		/// <param name="start">The index that the new byte array will be inserted from in the internal byte array</param>
		/// <param name="data">The bytes that will be inserted at the specified point</param>
		public void InsertRange(int start, byte[] data)
		{
			if (byteArr.Length <= Size + data.Length)
				Array.Resize<byte>(ref byteArr, data.Length + Size);

			for (int i = byteArr.Length - 1; i > start + data.Length - 1; i--)
				byteArr[i] = byteArr[i - data.Length];

			Size = index + data.Length - StartPointer;
			index = start;

			Buffer.BlockCopy(data, 0, byteArr, index, data.Length);

			PointToEnd();
		}

		/// <summary>
		/// Inserts a set of bytes at a specified index into the internal byte array
		/// </summary>
		/// <param name="start">The index that the new byte array will be inserted from in the internal byte array</param>
		/// <param name="data">The BMSByte object will be inserted at the specified point</param>
		public void InsertRange(int start, BMSByte data)
		{
			if (byteArr.Length <= Size + data.Size)
				Array.Resize<byte>(ref byteArr, data.Size + Size);

			for (int i = byteArr.Length - 1; i > start + data.Size - 1; i--)
				byteArr[i] = byteArr[i - data.Size];

			Size = index + data.Size - StartPointer;
			index = start;

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), byteArr, index, data.Size);

			PointToEnd();
		}

		/// <summary>
		/// Point the index to the end of the internal byte array of valid data
		/// </summary>
		private void PointToEnd()
		{
			index = Size;
		}

		/// <summary>
		/// Point the index to the start of the valid internal byte array data (0)
		/// </summary>
		private void PointToStart()
		{
			index = 0;
			StartPointer = 0;
		}

		/// <summary>
		/// An externally accessable method for resetting the pointer, will call internal PointToStart method
		/// </summary>
		public void ResetPointer()
		{
			PointToStart();
		}

		/// <summary>
		/// Clears out the internal byte array
		/// </summary>
		public void Clear(bool raw = false)
		{
			index = 0;
			Size = 0;
			StartPointer = 0;

			if (raw)
				Append(new byte[] { 1 });
		}
		
		/// <summary>
		/// Get an array of a specific type from the internally tracked byte array
		/// </summary>
		/// <param name="type">The type of array to be pulled</param>
		/// <param name="o"></param>
		/// <returns>The updated array of data</returns>
		private Array GetArray<T>(object o)
		{
			Type type = typeof(T);
			if (allowedTypes.ContainsKey(type))
			{
				T[] array = (T[])allowedTypes[type];
				array[0] = (T)o;
				return array;

			}
			else
				throw new Exception("The type " + type.ToString() + " is not allowed");
		}
		/// <summary>
		/// Used to get the size of a specific type of object, will only be types specified in the type lookup
		/// </summary>
		/// <param name="obj">The object to analyze the size of</param>
		/// <returns>The size in bytes required for the object</returns>
		private int GetSize(object obj)
		{
			if (obj is sbyte)
				return 1;
			else if (obj is byte)
				return 1;
			else if (obj is char)
				return 1;
			else if (obj is bool)
				return sizeof(bool);
			else if (obj is short)
				return sizeof(short);
			else if (obj is ushort)
				return sizeof(ushort);
			else if (obj is int)
				return sizeof(int);
			else if (obj is uint)
				return sizeof(uint);
			else if (obj is long)
				return sizeof(long);
			else if (obj is ulong)
				return sizeof(ulong);
			else if (obj is float)
				return sizeof(float);
			else if (obj is double)
				return sizeof(double);
			else
				throw new Exception("The type " + obj.GetType().ToString() + " is not allowed");
		}

		/// <summary>
		/// This will get the value for a basic data type in this data
		/// </summary>
		/// <typeparam name="T">The type to convert the bytes for</typeparam>
		/// <param name="start">The byte start index to read from</param>
		/// <returns>The value of the object</returns>
		public T GetBasicType<T>(int start, bool moveIndex = false)
		{
			return (T)GetBasicType(typeof(T), start, moveIndex);
		}

		public T GetBasicType<T>(bool moveIndex = true)
		{
			T obj = GetBasicType<T>(StartIndex(), moveIndex);
			return obj;
		}

		/// <summary>
		/// This will get the value for a basic data type in this data
		/// </summary>
		/// <param name="type">The type to convert the bytes for</typeparam>
		/// <param name="start">The byte start index to read from</param>
		/// <returns>The value of the object</returns>
		public object GetBasicType(Type type, int start, bool moveIndex = false)
		{
			if (type == typeof(sbyte))
			{
				if (moveIndex)
					MoveStartIndex(sizeof(sbyte));

				return byteArr[start];
			}
			else if (type == typeof(byte))
			{
				if (moveIndex)
					MoveStartIndex(sizeof(byte));

				return byteArr[start];
			}
			else if (type == typeof(char))
			{
				if (moveIndex)
					MoveStartIndex(sizeof(char));

				return byteArr[start];
			}
			else if (type == typeof(short))
			{
				if (moveIndex)
					MoveStartIndex(sizeof(short));

				return BitConverter.ToInt16(byteArr, start);
			}
			else if (type == typeof(ushort))
			{
				if (moveIndex)
					MoveStartIndex(sizeof(ushort));

				return BitConverter.ToUInt16(byteArr, start);
			}
			else if (type == typeof(bool))
			{
				if (moveIndex)
					MoveStartIndex(sizeof(bool));

				return BitConverter.ToBoolean(byteArr, start);
			}
			else if (type == typeof(int))
			{
				if (moveIndex)
					MoveStartIndex(sizeof(int));

				return BitConverter.ToInt32(byteArr, start);
			}
			else if (type == typeof(uint))
			{
				if (moveIndex)
					MoveStartIndex(sizeof(uint));

				return BitConverter.ToUInt32(byteArr, start);
			}
			else if (type == typeof(float))
			{
				if (moveIndex)
					MoveStartIndex(sizeof(float));

				return BitConverter.ToSingle(byteArr, start);
			}
			else if (type == typeof(long))
			{
				if (moveIndex)
					MoveStartIndex(sizeof(long));

				return BitConverter.ToInt64(byteArr, start);
			}
			else if (type == typeof(ulong))
			{
				if (moveIndex)
					MoveStartIndex(sizeof(ulong));

				return BitConverter.ToUInt64(byteArr, start);
			}
			else if (type == typeof(double))
			{
				if (moveIndex)
					MoveStartIndex(sizeof(double));

				return BitConverter.ToDouble(byteArr, start);
			}
			else if (type == typeof(string))
				return GetString(start, moveIndex);
			else if (type == typeof(Vector))
				return GetVector(start, moveIndex);
			else if (type.IsArray)
			{
				int rank = type.GetArrayRank();
				Type targetType = type.GetElementType();

				//int startingIndex = StartIndex();
				MoveStartIndex(sizeof(int));

				if (rank > 4)
					throw new Exception("Currently the system only supports up to 4 dimensions in an array");

				int i, j, k, l, x, y, z, w;
				switch (rank)
				{
					case 1:
						x = GetBasicType<int>(StartIndex(), true);
						object[] one = new object[x];
						for (i = 0; i < x; i++)
							one[i] = GetBasicType(targetType, StartIndex(), true);
						return one;
					case 2:
						x = GetBasicType<int>(StartIndex(), true);
						y = GetBasicType<int>(StartIndex(), true);
						object[,] two = new object[x, y];
						for (i = 0; i < x; i++)
							for (j = 0; j < y; j++)
								two[i, j] = GetBasicType(targetType, StartIndex(), true);
						return two;
					case 3:
						x = GetBasicType<int>(StartIndex(), true);
						y = GetBasicType<int>(StartIndex(), true);
						z = GetBasicType<int>(StartIndex(), true);
						object[,,] three = new object[x, y, z];
						for (i = 0; i < x; i++)
							for (j = 0; j < y; j++)
								for (k = 0; k < z; k++)
									three[i, j, k] = GetBasicType(targetType, StartIndex(), true);
						return three;
					case 4:
						x = GetBasicType<int>(StartIndex(), true);
						y = GetBasicType<int>(StartIndex(), true);
						z = GetBasicType<int>(StartIndex(), true);
						w = GetBasicType<int>(StartIndex(), true);
						object[,,,] four = new object[x, y, z, w];
						for (i = 0; i < x; i++)
							for (j = 0; j < y; j++)
								for (k = 0; k < z; k++)
									for (l = 0; l < w; l++)
										four[i, j, k, l] = GetBasicType(targetType, StartIndex(), true);
						return four;
				}

				throw new Exception("Deserialize case not found for this array");
			}
#if WINDOWS_UWP
			else if (type == typeof(Enum))
#else
			else if (type.IsEnum)
#endif
				return GetBasicType(Enum.GetUnderlyingType(type), start, moveIndex);
			else
				throw new Exception("The type " + type.ToString() + " is gettable from basic type, maybe try one of the other getters?");
		}

		/// <summary>
		/// This will get the string value from this data
		/// </summary>
		/// <param name="start">The byte start index to read from</param>
		/// <returns>The string value</returns>
		public string GetString(int start, bool moveIndex = false)
		{
			int length = BitConverter.ToInt32(byteArr, start);

			if (moveIndex)
				MoveStartIndex(sizeof(int));

			if (length <= 0)
				return string.Empty;

			if (moveIndex)
				MoveStartIndex(length);

			return Encoding.UTF8.GetString(byteArr, start + sizeof(int), length);
		}

		public Vector GetVector(int start, bool moveIndex = false)
		{
			Vector vec = new Vector
			{
				x = GetBasicType<float>(start, false),
				y = GetBasicType<float>(start + sizeof(float), false),
				z = GetBasicType<float>(start + (sizeof(float) * 2), false)
			};

			if (moveIndex)
				MoveStartIndex(sizeof(float) * 3);

			return vec;
		}

		/// <summary>
		/// Gets a byte array from the internally tracked byte array
		/// </summary>
		/// <param name="start">Where to start getting bytes from in the internal byte array</param>
		/// <param name="moveIndex">If the index should be moved automatically after this process</param>
		/// <returns>The found byte array</returns>
		public byte[] GetByteArray(int start, bool moveIndex = false)
		{
			int length = BitConverter.ToInt32(byteArr, start);

			if (moveIndex)
				MoveStartIndex(sizeof(int));

			if (length <= 0)
				return new byte[0];

			if (moveIndex)
				MoveStartIndex(length);

			byte[] data = new byte[length];
			Buffer.BlockCopy(byteArr, start + sizeof(int), data, 0, length);
			return data;
		}

		/// <summary>
		/// Support for indexer
		/// </summary>
		/// <param name="i">The index of the byte array to access</param>
		/// <returns>The byte at the specidifed index</returns>
		public byte this[int i]
		{
			get { return byteArr[i]; }
			set { byteArr[i] = value; }
		}

		/// <summary>
		/// Checks to see if the internal byte array of this and another BMSByte have the same value
		/// If a BMSByte is not supplied, then the base Equals method will be executed
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			// This operation can be slow so don't do it often
			if (obj is BMSByte)
			{
				if (StartIndex() != ((BMSByte)obj).StartIndex())
					return false;

				if (Size != ((BMSByte)obj).Size)
					return false;

				for (int i = StartIndex(); i < Size; i++)
				{
					if (byteArr[i] != ((BMSByte)obj)[i])
						return false;
				}

				return true;
			}
			else
				return base.Equals(obj);
		}

		/// <summary>
		/// Get the hash code
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
