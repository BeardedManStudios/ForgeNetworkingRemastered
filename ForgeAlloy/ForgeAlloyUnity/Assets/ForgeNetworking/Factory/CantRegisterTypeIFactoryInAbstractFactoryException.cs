using System;

namespace Forge.Factory
{
	internal class CantRegisterTypeIFactoryInAbstractFactoryException : Exception
	{
		public CantRegisterTypeIFactoryInAbstractFactoryException()
			: base($"You can not register an IFactory with the abstract factory, " +
				  $"it should be a derivative of IFactory that does not implement " +
				  $"IFactory directly. See ForgeTypeFactory for an example")
		{
		}
	}
}
