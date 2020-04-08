using System;
using System.Collections.Generic;

namespace Forge.Serialization
{
	public class BMSBytePool
	{
		private const int APPROX_SIZE_ZONE = 128;
		private readonly List<BMSByte> _availableBuffers = new List<BMSByte>();
		private readonly List<BMSByte> _inUseBuffers = new List<BMSByte>();

		public BMSByte Get(int size)
		{
			if (_availableBuffers.Count == 0)
				return CreateNewBuffer(size);
			else
			{
				BMSByte buff = GetAvailableBuffer(size);
				if (buff.byteArr.Length < size)
					buff.SetArraySize(size);
				return buff;
			}
		}

		public void Release(BMSByte buffer)
		{
			// TODO:  Figure out why _inUseBuffers.Remove(buffer) is returning false
			bool found = false;
			for (int i = 0; i < _inUseBuffers.Count; i++)
			{
				if (_inUseBuffers[i] == buffer)
				{
					_inUseBuffers.RemoveAt(i);
					found = true;
					break;
				}
			}
			if (!found)
				throw new BMSBytePoolReleaseUnmanagedBufferException();
			_availableBuffers.Add(buffer);
			buffer.Clear();
		}

		private BMSByte GetAvailableBuffer(int size)
		{
			var idx = LocateAvailableBufferNear(size);
			BMSByte buff = _availableBuffers[idx];
			_availableBuffers.RemoveAt(idx);
			_inUseBuffers.Add(buff);
			return buff;
		}

		private int LocateAvailableBufferNear(int size)
		{
			int idx = 0;
			for (int i = 0; i < _availableBuffers.Count; i++)
			{
				if (_availableBuffers[i].Size == size)
					return i;
				else if (System.Math.Abs(_availableBuffers[i].Size - size) <= APPROX_SIZE_ZONE)
					idx = i;
			}
			return idx;
		}

		private BMSByte CreateNewBuffer(int size)
		{
			BMSByte buff = new BMSByte();
			if (size > 0)
				buff.SetArraySize(size);
			_inUseBuffers.Add(buff);
			return buff;
		}
	}
}
