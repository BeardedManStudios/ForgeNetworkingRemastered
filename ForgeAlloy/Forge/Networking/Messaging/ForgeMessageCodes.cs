using System;
using System.Collections.Generic;

namespace Forge.Networking.Messaging
{
	public static partial class ForgeMessageCodes
	{
		private static readonly List<Type> _messageTypes = new List<Type>();

		public static void Register<T>()
		{
			var t = typeof(T);
			if (_messageTypes.IndexOf(t) >= 0)
				throw new DuplicateMessageTypeRegistrationException(t);
			_messageTypes.Add(t);
		}

		public static void Unregister(Type t)
		{
			_messageTypes.Remove(t);
		}

		public static void Unregister(int code)
		{
			_messageTypes.RemoveAt(code);
		}

		public static object Instantiate(int code)
		{
			if (code < 0 || code >= _messageTypes.Count)
				throw new MessageCodeNotFoundException(code);
			return Activator.CreateInstance(_messageTypes[code]);
		}

		public static int GetCodeFromType(Type type)
		{
			int code = _messageTypes.IndexOf(type);
			if (code < 0)
				throw new MessageTypeNotFoundException(type);
			return code;
		}

		public static void Clear()
		{
			_messageTypes.Clear();
		}
	}
}
