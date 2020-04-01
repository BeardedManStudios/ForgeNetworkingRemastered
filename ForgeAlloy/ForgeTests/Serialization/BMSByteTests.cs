using Forge.Serialization;
using NUnit.Framework;

namespace ForgeTests.Serialization
{
	[TestFixture]
	public class BMSByteTests
	{
		// TODO:  Finish testing all the other public methods of BMSByte

		[Test]
		public void AppendBytesAndCompressBytes()
		{
			byte[] data = new byte[] { 1, 8, 9, 34, 255, 0, 33, 66, 100, 128 };
			var cache = new BMSByte();
			cache.Append(data);
			Assert.AreEqual(data.Length, cache.Size);
			for (int i = 0; i < data.Length; i++)
				Assert.AreEqual(data[i], cache[i]);
		}

		[Test]
		public void AppendBytesRemoveStart3()
		{
			byte[] data = new byte[] { 1, 8, 9, 34, 255, 0, 33, 66, 100, 128 };
			var cache = new BMSByte();
			cache.Append(data);
			cache.RemoveStart(3);
			for (int i = 3; i < data.Length; i++)
				Assert.AreEqual(data[i], cache[i - 3]);
		}

		[Test]
		public void AppendBytesRemoveStart3Add3ToEnd()
		{
			int removeSize = 3;
			byte[] data = new byte[] { 1, 8, 9, 34, 255, 0, 33, 66, 100, 128 };
			var cache = new BMSByte();
			cache.Append(data);
			cache.RemoveStart(removeSize);
			byte[] trailer = new byte[] { 9, 3, 9 };
			cache.Append(trailer);
			for (int i = removeSize; i < data.Length; i++)
			{
				Assert.AreEqual(data[i], cache[i - removeSize]);
				Assert.AreEqual(data[i], cache[i - removeSize]);
			}
			for (int i = trailer.Length - 1; i >= 0; i--)
				Assert.AreEqual(trailer[i], cache[cache.Size - trailer.Length + i]);
		}

		[Test]
		public void CloneAndCompareBytes()
		{
			int removeSize = 3;
			byte[] data = new byte[] { 1, 8, 9, 34, 255, 0, 33, 66, 100, 128 };
			var cache = new BMSByte();
			cache.Append(data);
			cache.RemoveStart(removeSize);
			var copy = new BMSByte();
			copy.Clone(cache);
			for (int i = 0; i < data.Length - removeSize; i++)
				Assert.AreEqual(copy[i], cache[i]);
		}
	}
}
