using System;
using System.Collections.Generic;
using System.Linq;

namespace Forge.Reflection
{
	public class TypeReflectionRepository : ITypeReflectionRepository
	{
		private readonly List<Type> _typeList = new List<Type>();
		private readonly Dictionary<Type, List<Type>> _reflectedTypeList = new Dictionary<Type, List<Type>>();

		public void AddType<T>()
		{
			var t = typeof(T);
			if (_typeList.Contains(t))
				throw new ReflectedTypeAlreadyRegisteredInRepositoryException(t);
			_typeList.Add(t);
			UpdateRegistration(t);
		}

		public void RemoveType<T>()
		{
			var t = typeof(T);
			_typeList.Remove(t);
			_reflectedTypeList.Remove(t);
		}

		public List<Type> GetTypesFor<T>()
		{
			return _reflectedTypeList[typeof(T)];
		}

		private void UpdateRegistration(Type baseType)
		{
			if (_reflectedTypeList.ContainsKey(baseType))
				return;
			var foundTypes = new List<Type>();
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				foundTypes.AddRange(assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && !t.IsInterface));
			_reflectedTypeList.Add(baseType, foundTypes);
		}
	}
}
