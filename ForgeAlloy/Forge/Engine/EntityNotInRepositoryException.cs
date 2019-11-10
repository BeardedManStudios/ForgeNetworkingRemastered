using System;

namespace Forge.Engine
{
	public class EntityNotInRepositoryException : Exception
	{
		public EntityNotInRepositoryException(int id)
			: base($"The entity with the id {id} was not found in the repository")
		{
		}
	}
}
