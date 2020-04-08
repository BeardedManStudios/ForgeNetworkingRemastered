using System;

namespace Forge.Networking.Unity
{
	[Serializable]
	public class EntityAlreadyInRepositoryException : Exception
	{
		public EntityAlreadyInRepositoryException(int id)
			: base($"The entity with id {id} is already in the repository and can't be added again")
		{
		}
	}
}
