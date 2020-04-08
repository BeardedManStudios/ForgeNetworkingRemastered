using System;

namespace Forge.Serialization
{
	public class SerializationTypeKeyAlreadyExistsException : Exception
	{
		public SerializationTypeKeyAlreadyExistsException(Type t)
			: base($"The type {t} has already been registered with the serialization container")
		{

		}
	}
}
