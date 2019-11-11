using System;
using UnityEngine;

namespace Forge.Networking.Unity
{
	[Serializable]
	public sealed class ForgeAttribute : PropertyAttribute
	{
		public readonly Type[] Types;
		public ForgeAttribute(params Type[] types)
		{
			Types = types;
		}
	}
}
