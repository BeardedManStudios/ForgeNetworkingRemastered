using System;
using System.Collections.Generic;

namespace Forge.Factory
{
	public abstract class TypeFactory : IFactory
	{
		private readonly Dictionary<Type, Func<object>> _typeLookup = new Dictionary<Type, Func<object>>();

		public abstract void PrimeRegistry();

		public void Register<T>(Func<object> factoryMethod)
		{
			var t = typeof(T);
			if (_typeLookup.ContainsKey(t))
				throw new Exception($"The type ({t}) is already registered");
			_typeLookup.Add(t, factoryMethod);
		}

		public void Register<TInterface, TActual>() where TActual : TInterface, new()
		{
			var t = typeof(TInterface);
			if (_typeLookup.ContainsKey(t))
				throw new Exception($"The type ({t}) is already registered");
			_typeLookup.Add(t, () => new TActual());
		}

		public void Unregister<T>()
		{
			_typeLookup.Remove(typeof(T));
		}

		public T GetNew<T>()
		{
			var t = typeof(T);
			if (!_typeLookup.TryGetValue(t, out var factoryMethod))
				throw new Exception($"The type ({t}) is not registered");
			return (T)factoryMethod();
		}

		public void Clear()
		{
			_typeLookup.Clear();
		}

		public void Replace<T>(Func<object> factoryMethod)
		{
			var t = typeof(T);
			_typeLookup.Remove(t);
			_typeLookup.Add(t, factoryMethod);
		}

		public void Replace<TInterface, TActual>() where TActual : TInterface, new()
		{
			var t = typeof(TInterface);
			_typeLookup.Remove(t);
			_typeLookup.Add(t, () => new TActual());
		}
	}
}
