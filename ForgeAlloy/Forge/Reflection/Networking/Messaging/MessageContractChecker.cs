using System;
using System.Collections.Generic;
using Forge.Networking.Messaging;

namespace Forge.Reflection.Networking.Messaging
{
	public class MessageContractChecker
	{
		public int HighestCodeFound { get; private set; }
		private readonly ITypeReflectionRepository _repository = new TypeReflectionRepository();
		private readonly Dictionary<Type, string> _errors = new Dictionary<Type, string>();

		public MessageContractChecker()
		{
			_repository.AddType<IMessage>();
			PrepareErrors();
		}

		private void PrepareErrors()
		{
			var validated = new Dictionary<int, Type>();
			List<Type> messages = _repository.GetTypesFor<IMessage>();
			for (int i = 0; i < messages.Count; i++)
			{
				if (messages[i].IsAbstract)
					continue;
				var attrs = messages[i].GetCustomAttributes(
					typeof(MessageContractAttribute), true) as MessageContractAttribute[];
				if (attrs == null || attrs.Length == 0)
					_errors.Add(messages[i], $"The message {messages[i]} is missing the EngineMessageContract attribute");
				else
				{
					foreach (var a in attrs)
						FindErrorsForMessage(validated, messages[i], a);
				}
			}
		}

		public Dictionary<Type, string> GetAllErrors()
		{
			return _errors;
		}

		private void FindErrorsForMessage(Dictionary<int, Type> validated, Type message, MessageContractAttribute a)
		{
			if (!a.GetClassType().IsAssignableFrom(message))
			{
				_errors.Add(message, $"[Type Error]:  The type used as an argument to the EngineMessageContract " +
					$"for the message {message} is not valid. The type {a.GetClassType()} is " +
					$"not an instance of {message}");
			}
			else if (validated.TryGetValue(a.GetId(), out var current))
			{
				_errors.Add(message, $"[Code Error]:  The message {message} has a message code " +
					$"of {a.GetInputId()} but that code is already in use by {current}");
			}
			else
			{
				validated.Add(a.GetId(), a.GetClassType());

				if (a.GetType() == typeof(EngineMessageContractAttribute))
				{
					HighestCodeFound = System.Math.Max(HighestCodeFound, a.GetInputId());
				}
			}
		}
	}
}
