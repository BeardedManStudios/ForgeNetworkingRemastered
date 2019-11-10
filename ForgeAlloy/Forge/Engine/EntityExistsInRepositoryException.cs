using System;

namespace Forge.Engine
{
	public class EntityExistsInRepositoryException : Exception
	{
		public EntityExistsInRepositoryException(int id)
			: base($"An entity with id {id} already exists in the repository")
		{
		}
	}
}
