using System;
using System.Collections.Generic;

namespace Forge.Reflection
{
	public interface ITypeReflectionRepository
	{
		void AddType<T>();
		void RemoveType<T>();
		List<Type> GetTypesFor<T>();
	}
}
