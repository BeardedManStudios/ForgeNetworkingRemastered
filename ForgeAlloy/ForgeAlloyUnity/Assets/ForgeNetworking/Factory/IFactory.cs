using System;

namespace Forge.Factory
{
	public interface IFactory
	{
		void PrimeRegistry();
		void Register<T>(Func<object> factoryMethod);
		void Register<TInterface, TActual>() where TActual : TInterface, new();
		void Replace<T>(Func<object> factoryMethod);
		void Replace<TInterface, TActual>() where TActual : TInterface, new();
		void Unregister<T>();
		T GetNew<T>();
		void Clear();
	}
}
