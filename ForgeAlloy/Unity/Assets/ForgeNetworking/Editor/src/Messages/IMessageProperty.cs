using System;

namespace Forge.Editor.Messages
{
	[Flags]
	public enum PropertyTypes
	{
		GET = 0x00,
		SET = 0x01
	}

	public interface IMessageProperty
	{
		Type PropertyType { get; set; }
		string Name { get; set; }
		PropertyTypes Accessor { get; }
	}
}
