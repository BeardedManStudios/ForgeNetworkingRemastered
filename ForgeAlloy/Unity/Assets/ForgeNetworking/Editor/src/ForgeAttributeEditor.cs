using System.Linq;
using System.Text;
using Forge.Networking.Unity;
using UnityEditor;
using UnityEngine;

namespace Forge.Editor
{
	[CustomPropertyDrawer(typeof(ForgeAttribute))]
	public class ForgeAttributeEditor : PropertyDrawer
	{
		private StringBuilder sb;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			ForgeAttribute forgeAttribute = attribute as ForgeAttribute;

			if (forgeAttribute == null)
			{
				return;
			}

			if (sb == null)
			{
				sb = new StringBuilder();
				for (int i = 0; i < forgeAttribute.Types.Length; ++i)
				{
					sb.Append(forgeAttribute.Types[i].Name);
					if (i + 1 < forgeAttribute.Types.Length)
						sb.Append(", ");
				}
			}

			label.text = $"{ label.text } ({ sb })";
			var obj = (Component)EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Component), true);

			if (obj != null && forgeAttribute.Types.Length > 0)
			{
				if (forgeAttribute.Types.Any(o => o.IsInstanceOfType(obj)))
				{
					property.objectReferenceValue = obj;
				}
				else
				{
					bool found = false;
					for (int i = 0; i < forgeAttribute.Types.Length; ++i)
					{
						var type = forgeAttribute.Types[i];
						var components = obj.GetComponents(type);
						if (components.Length > 0)
						{
							property.objectReferenceValue = components[0];
							found = true;
							break;
						}
					}

					if (!found)
					{
						Debug.LogError($"Must be a valid type of: { sb }");
						property.objectReferenceValue = null;
					}
				}
			}
			else
			{
				property.objectReferenceValue = null;
			}
		}
	}
}
