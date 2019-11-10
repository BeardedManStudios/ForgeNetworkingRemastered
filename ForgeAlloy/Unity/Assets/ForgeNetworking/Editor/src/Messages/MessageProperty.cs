using System;

namespace Forge.Editor.Messages
{
	public class MessageProperty : IMessageProperty
	{
		public Type PropertyType { get; set; }
		public string Name { get; set; }
		public PropertyTypes Accessor => PropertyTypes.GET | PropertyTypes.SET;
	}
}
