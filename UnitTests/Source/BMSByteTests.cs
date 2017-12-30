using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeardedManStudios.Tests
{
	[TestClass()]
	public class BMSByteTests
	{
		[TestMethod()]
		public void BMSByteTest()
		{
			byte b = 1;
			for (int i = 0; i < 10000; i++)
			{
				BMSByte data = ObjectMapper.BMSByte(b, b, b, b, b, b, b, b, b, b, b);
			}
		}
	}
}