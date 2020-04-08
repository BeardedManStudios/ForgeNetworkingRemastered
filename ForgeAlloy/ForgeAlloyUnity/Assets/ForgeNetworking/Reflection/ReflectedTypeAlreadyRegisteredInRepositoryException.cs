using System;

namespace Forge.Reflection
{
	[Serializable]
	internal class ReflectedTypeAlreadyRegisteredInRepositoryException : Exception
	{
		public ReflectedTypeAlreadyRegisteredInRepositoryException(Type t)
			: base($"The type {t} is already registered in the repository")
		{
		}
	}
}
