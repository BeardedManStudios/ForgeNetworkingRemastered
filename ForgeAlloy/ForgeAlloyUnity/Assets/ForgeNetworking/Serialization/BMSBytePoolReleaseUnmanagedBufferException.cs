using System;

namespace Forge.Serialization
{
	[Serializable]
	public class BMSBytePoolReleaseUnmanagedBufferException : Exception
	{
		public BMSBytePoolReleaseUnmanagedBufferException()
			: base($"The buffer requested to be released isn't managed by the buffer pool")
		{

		}
	}
}
