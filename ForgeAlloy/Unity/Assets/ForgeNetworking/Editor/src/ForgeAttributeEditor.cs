using System.Collections;
using System.Collections.Generic;
using Forge.Networking.Unity;
using UnityEditor;
using UnityEngine;

namespace Forge.Editor
{
	[CustomPropertyDrawer(typeof(ForgeAttribute))]
	public class ForgeAttributeEditor : PropertyDrawer
	{
		private MonoBehaviour monoBehaviour;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			ForgeAttribute forgeAttribute = attribute as ForgeAttribute;
			EditorGUI.PropertyField(position, property, label);
			//EditorGUI.LabelField(position, "Hello");
		}
	}
}
