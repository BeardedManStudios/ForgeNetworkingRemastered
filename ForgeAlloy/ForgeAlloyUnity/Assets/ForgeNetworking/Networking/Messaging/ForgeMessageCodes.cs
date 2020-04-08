using System;
using System.Collections.Generic;
using System.Linq;

namespace Forge.Networking.Messaging
{
	public static partial class ForgeMessageCodes
	{
		private static readonly Dictionary<int, Type> _messageTypes = new Dictionary<int, Type>();
		private static readonly Dictionary<Type, int> _messageCodes = new Dictionary<Type, int>();
		private static readonly MessagePoolMulti _messagePool = new MessagePoolMulti();

		public static void Register()
		{
			Type baseType = typeof(IMessage);
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					var types = asm.GetTypes().Where(t => baseType.IsAssignableFrom(t) && !t.IsInterface);
					foreach (var t in types)
					{
						var attrs = (MessageContractAttribute[])t.GetCustomAttributes(typeof(MessageContractAttribute), true);
						foreach (var a in attrs)
							Register(a.GetId(), a.GetClassType());
					}
				}
				catch { /* TODO:  Make sure this is a load assembly only exception */ }
			}
		}

		private static void Register(int code, Type type)
		{
			if (_messageTypes.ContainsKey(code))
				throw new DuplicateMessageTypeRegistrationException(type, code);
			else if (_messageCodes.ContainsKey(type))
				throw new DuplicateMessageTypeRegistrationException(type, code);
			_messageTypes.Add(code, type);
			_messageCodes.Add(type, code);
		}

		public static object Instantiate(int code)
		{
			if (!_messageTypes.TryGetValue(code, out var type))
				throw new MessageCodeNotFoundException(code);
			return _messagePool.Get(type);
		}

		public static int GetCodeFromType(Type type)
		{
			if (!_messageCodes.TryGetValue(type, out var code))
				throw new MessageTypeNotFoundException(type);
			return code;
		}

		public static void Clear()
		{
			_messageTypes.Clear();
			_messageCodes.Clear();
		}
	}
}
