using System;
using System.Collections.Generic;

namespace Forge.Networking.Messaging
{
	public static partial class ForgeMessageCodes
	{
		public const int UNIT_TEST_MOCK_MESSAGE = -1;
		public const int SEND_NAME_MESSAGE = 0;



		private static readonly Dictionary<int, Type> _codeTypeLookup = new Dictionary<int, Type>();

		public static void Register<T>(int code)
		{
			// TODO:  Throw an exception here if it already exists
			_codeTypeLookup.Add(code, typeof(T));
		}

		public static void Unregister(int code)
		{
			_codeTypeLookup.Remove(code);
		}

		public static object Instantiate(int code)
		{
			// TODO:  Throw an exception here that means something
			Type t = _codeTypeLookup[code];
			return Activator.CreateInstance(t);
		}
	}
}
