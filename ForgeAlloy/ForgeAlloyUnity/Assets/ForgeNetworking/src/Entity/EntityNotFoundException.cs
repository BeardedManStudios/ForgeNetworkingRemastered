using System;

namespace Forge.Networking.Unity
{
	[Serializable]
	public class EntityNotFoundException : Exception
	{
		public EntityNotFoundException(int id)
			: base($"The entity with id {id} was searched for but was not found")
		{
		}
	}
}
