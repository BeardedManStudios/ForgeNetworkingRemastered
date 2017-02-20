using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	/// <summary>
	/// This is a editor field for the network object
	/// </summary>
	public class ForgeEditorField
	{
		public string FieldName;
		public bool CanRender = true;
		public bool Interpolate;
		public float InterpolateValue;
		public ForgeAcceptableFieldTypes FieldType;
		public bool DELETED;

		public ForgeEditorField(string name = "", bool canRender = true, ForgeAcceptableFieldTypes type = ForgeAcceptableFieldTypes.BYTE, bool interpolate = false, float interpolateValue = 0f)
		{
			this.FieldName = name;
			this.FieldType = type;
			this.Interpolate = interpolate;
			this.InterpolateValue = interpolateValue;
			this.CanRender = canRender;
		}

		public void Render()
		{
			if (DELETED)
				return;

			if (!CanRender)
				return;

			GUILayout.BeginHorizontal();
			FieldName = GUILayout.TextField(FieldName);
			FieldType = (ForgeAcceptableFieldTypes)EditorGUILayout.EnumPopup(FieldType, GUILayout.Width(75));
			//if (FieldType == ForgeAcceptableFieldTypes.Unknown) //Unsupported
			//{
			//	Debug.LogError("Can't set the type to unknown (Not Allowed)");
			//	FieldType = AcceptableTypes.INT;
			//}

			if (ForgeClassFieldValue.IsInterpolatable(FieldType))
			{
				GUI.color = Interpolate ? Color.white : Color.gray;
				if (GUILayout.Button("Interpolate", GUILayout.Width(100)))
					Interpolate = !Interpolate;

				if (Interpolate)
					InterpolateValue = EditorGUILayout.FloatField(InterpolateValue, GUILayout.Width(50));
				else
					InterpolateValue = ForgeNetworkingEditor.DEFAULT_INTERPOLATE_TIME;
			}

			GUI.color = Color.white;
			Rect verticleButton = EditorGUILayout.BeginVertical("Button", GUILayout.Width(50), GUILayout.Height(10));
			GUI.color = Color.red;
			if (GUI.Button(verticleButton, GUIContent.none))
			{
				DELETED = true;
			}
			GUI.color = Color.white;

			GUILayout.BeginHorizontal();//Center the icon
			EditorGUILayout.Space();
			GUILayout.FlexibleSpace();
			GUILayout.Label(ForgeNetworkingEditor.TrashIcon, GUILayout.Height(15));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			GUILayout.EndHorizontal();
		}
	}
}