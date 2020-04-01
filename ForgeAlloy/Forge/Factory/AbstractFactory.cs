using System;
using System.Collections.Generic;

namespace Forge.Factory
{
	public static class AbstractFactory
	{
		private static readonly Dictionary<Type, IFactory> _factories = new Dictionary<Type, IFactory>();

		public static void Register<TInterface, TActual>() where TActual : TInterface, IFactory, new()
		{
			if (typeof(TInterface) == typeof(IFactory))
				throw new CantRegisterTypeIFactoryInAbstractFactoryException();
			var t = typeof(TInterface);
			if (_factories.ContainsKey(t))
				throw new Exception($"The type ({t}) is already registered");
			var actual = new TActual();
			actual.PrimeRegistry();
			_factories.Add(t, actual);
		}

		public static void Unregister<T>()
		{
			_factories.Remove(typeof(T));
		}

		public static T Get<T>()
		{
			var t = typeof(T);
			if (!_factories.TryGetValue(t, out var factory))
				throw new Exception($"The type ({t}) is not registered");
			return (T)factory;
		}

		public static void Clear()
		{
			foreach (var factory in _factories)
				factory.Value.Clear();
			_factories.Clear();
		}
	}
}
