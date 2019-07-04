using System;
using System.Diagnostics;
using System.Reflection;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using NUnit.Framework;

namespace BeardedManStudios.Forge.Tests
{
	[TestFixture]
	public class FactoryTests
	{
		protected static DateTime start;

		[Test]
		public void DecodeMessageTest()
		{
			start = DateTime.UtcNow;

			var objType = typeof(Factory);
			var t = Type.GetType(objType.AssemblyQualifiedName);
			MethodInfo testMethod = t.GetMethod("DecodeHead", BindingFlags.NonPublic | BindingFlags.Static);

			Binary sendFrame = MakeBinary(false, true, "testing veru long string cuz why not it will be ibsidfngosidfose");
			int firstIdx = 0;
			object[] arr = { sendFrame.StreamData.byteArr, false, firstIdx };

			DateTime time = DateTime.Now;
			for (int i = 0; i < 10000; i++)
			{
				testMethod.Invoke(null, arr);
			}
			Debug.WriteLine("Test 1: " + (DateTime.Now - time).TotalMilliseconds);
		}

		private Binary MakeBinary(bool stream, bool useMask, params object[] args)
		{
			BMSByte data = new BMSByte();
			ObjectMapper.Instance.MapBytes(data, args);

			ulong timestep = (ulong)(DateTime.UtcNow - start).TotalMilliseconds;
			return new Binary(timestep, useMask, data, Receivers.Server, 17931, stream);
		}
	}
}
