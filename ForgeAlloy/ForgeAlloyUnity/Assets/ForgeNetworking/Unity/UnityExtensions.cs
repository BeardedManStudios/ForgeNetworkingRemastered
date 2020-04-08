using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Forge.Unity
{
	public static class UnityExtensions
	{
		public static List<T> FindInterfaces<T>()
		{
			return GameObject.FindObjectsOfType<MonoBehaviour>().OfType<T>().ToList();
		}

		public static T FindInterface<T>()
		{
			return GameObject.FindObjectsOfType<MonoBehaviour>().OfType<T>().FirstOrDefault();
		}

		public static bool HasAnyParentEqualTo(this Transform self, Transform parent)
		{
			Transform current = self;
			while (current != null)
			{
				if (current == parent)
					return true;
				current = current.parent;
			}
			return false;
		}
	}
}
