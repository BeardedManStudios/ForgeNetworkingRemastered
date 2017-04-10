using UnityEditor;
using UnityEngine;
using System;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	/// <summary>
	/// This is a editor field for the network object
	/// </summary>
	[Serializable]
	public class ForgeEditorField
	{
		public string FieldName;
		public bool CanRender = true;
		public bool Interpolate;
		public float InterpolateValue;
		public ForgeAcceptableFieldTypes FieldType;

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
				{
					if (InterpolateValue == 0)
						InterpolateValue = ForgeNetworkingEditor.DEFAULT_INTERPOLATE_TIME;
					else
						InterpolateValue = EditorGUILayout.FloatField(InterpolateValue, GUILayout.Width(50));
				}
				else
				{
					InterpolateValue = 0;
					//InterpolateValue = ForgeNetworkingEditor.DEFAULT_INTERPOLATE_TIME;
				}
			}
		}

		public void Render(Rect rect, bool isActive, bool isFocused)
		{
			if (!CanRender)
				return;

			rect.y += 2;

			Rect changingRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
			FieldName = EditorGUI.TextField(changingRect, FieldName);
			changingRect.x += rect.width * 0.3f + 5;
			FieldType = (ForgeAcceptableFieldTypes)EditorGUI.EnumPopup(changingRect, FieldType);
			if (ForgeClassFieldValue.IsInterpolatable(FieldType))
			{
				changingRect.x += rect.width * 0.3f + 10;
				changingRect.width = rect.width * 0.2f;
				Interpolate = EditorGUI.ToggleLeft(changingRect, "  Interpolate", Interpolate);

				if (Interpolate)
				{
					if (InterpolateValue == 0)
						InterpolateValue = ForgeNetworkingEditor.DEFAULT_INTERPOLATE_TIME;
					else
					{
						changingRect.x += rect.width * 0.2f + 5;
						changingRect.width = rect.width * 0.3f;
						InterpolateValue = EditorGUI.FloatField(changingRect, InterpolateValue);
					}
				}
				else
					InterpolateValue = 0;
			}
		}
	}
}