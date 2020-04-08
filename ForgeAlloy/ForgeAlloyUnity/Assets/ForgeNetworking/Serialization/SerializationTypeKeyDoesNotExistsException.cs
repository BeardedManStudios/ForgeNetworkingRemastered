using System;

namespace Forge.Serialization
{
	public class SerializationTypeKeyDoesNotExistsException : Exception
	{
		public SerializationTypeKeyDoesNotExistsException(Type t)
			: base($"The type {t} has not been registered with the serialization container")
		{

		}
	}
}
