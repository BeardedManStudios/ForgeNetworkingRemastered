using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	/// <summary>
	/// This is the rpc field for the network object
	/// </summary>
	[Serializable]
	public class ForgeEditorRPCField
	{
		public string FieldName;
		public bool CanRender = true;
		public List<ForgeNRPCTypes> FieldTypes;
		public int ArgumentCount { get { return FieldTypes.Count; } }
		public bool Dropdown;

		public ForgeEditorRPCField()
		{
			FieldName = "";
			FieldTypes = new List<ForgeNRPCTypes>();
		}

		public ForgeEditorRPCField Clone()
		{
			ForgeEditorRPCField returnValue = new ForgeEditorRPCField();

			returnValue.FieldName = this.FieldName;
			returnValue.FieldTypes.AddRange(this.FieldTypes);
			returnValue.Dropdown = this.Dropdown;
			returnValue.CanRender = this.CanRender;

			return returnValue;
		}

		public void Render()
		{
			if (!CanRender)
				return;

			GUILayout.BeginHorizontal();
			FieldName = GUILayout.TextField(FieldName);

			Dropdown = EditorGUILayout.Foldout(Dropdown, "Arguments");

			if (Dropdown)
			{
				for (int i = 0; i < FieldTypes.Count; ++i)
				{
					GUILayout.BeginHorizontal();
					FieldTypes[i].HelperName = EditorGUILayout.TextField(FieldTypes[i].HelperName);
					FieldTypes[i].Type = (ForgeAcceptableRPCTypes)EditorGUILayout.EnumPopup(FieldTypes[i].Type);
					//if (FieldTypes[i].Type == AcceptableTypes.Unknown) //Unsupported
					//{
					//	Debug.LogError("Can't set the type to unknown (Not Allowed)");
					//	FieldTypes[i].Type = AcceptableTypes.INT;
					//}

					Rect subtractBtn = EditorGUILayout.BeginVertical("Button", GUILayout.Width(75), GUILayout.Height(13));
					GUI.color = Color.red;
					if (GUI.Button(subtractBtn, GUIContent.none))
					{
						FieldTypes.RemoveAt(i);
						i--;
					}

					if (ForgeNetworkingEditor.ProVersion)
						GUI.color = Color.white;
					else
						GUI.color = Color.black;

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space();
					GUILayout.FlexibleSpace();
					GUILayout.Label(ForgeNetworkingEditor.SubtractIcon, GUILayout.Height(12));
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.EndVertical();
					GUI.color = Color.white;

					GUILayout.EndHorizontal();
				}

				Rect addBtn = EditorGUILayout.BeginVertical("Button", GUILayout.Width(75), GUILayout.Height(13));
				GUI.color = Color.green;
				if (GUI.Button(addBtn, GUIContent.none))
				{
					FieldTypes.Add(new ForgeNRPCTypes() { Type = ForgeAcceptableRPCTypes.BYTE });
				}
				if (ForgeNetworkingEditor.ProVersion)
					GUI.color = Color.white;
				else
					GUI.color = Color.black;
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				GUILayout.FlexibleSpace();
				GUILayout.Label(ForgeNetworkingEditor.AddIcon, GUILayout.Height(12));
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				GUI.color = Color.white;
			}
		}

		public void Render(Rect rect, bool isActive, bool isFocused)
		{
			if (!CanRender)
				return;

			rect.y += 2;

			Rect changingRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
			FieldName = EditorGUI.TextField(changingRect, FieldName);
			changingRect.x = rect.x + rect.width * 0.3f + 10;
			changingRect.width = rect.width * 0.2f - 10;
			Dropdown = EditorGUI.Foldout(changingRect, Dropdown, "Arguments", true);

			if (Dropdown)
			{
				for (int i = 0; i < FieldTypes.Count; ++i)
				{
					rect.height += EditorGUIUtility.singleLineHeight + 2;
					changingRect.y += EditorGUIUtility.singleLineHeight + 2;


					if (i > 0)
					{
						changingRect.x = rect.width * 0.035f;
						changingRect.width = rect.width * 0.025f;
						if (GUI.Button(changingRect, "^"))
						{
							if (i - 1 >= 0)
							{
								ForgeNRPCTypes t1 = FieldTypes[i - 1];
								ForgeNRPCTypes t2 = FieldTypes[i];
								FieldTypes[i - 1] = t2;
								FieldTypes[i] = t1;
							}
							else
							{
								ForgeNRPCTypes t1 = FieldTypes[FieldTypes.Count - 1];
								ForgeNRPCTypes t2 = FieldTypes[i];
								FieldTypes[FieldTypes.Count - 1] = t2;
								FieldTypes[i] = t1;
							}
						}
					}

					if (i < FieldTypes.Count - 1)
					{
						changingRect.x = rect.width * 0.06f;
						changingRect.width = rect.width * 0.025f;
						if (GUI.Button(changingRect, "v"))
						{
							if (i + 1 < FieldTypes.Count)
							{
								ForgeNRPCTypes t1 = FieldTypes[i + 1];
								ForgeNRPCTypes t2 = FieldTypes[i];
								FieldTypes[i + 1] = t2;
								FieldTypes[i] = t1;
							}
							else
							{
								ForgeNRPCTypes t1 = FieldTypes[0];
								ForgeNRPCTypes t2 = FieldTypes[i];
								FieldTypes[0] = t2;
								FieldTypes[i] = t1;
							}
						}
					}

					changingRect.x = rect.width * 0.1f;
					changingRect.width = rect.width * 0.3f;
					FieldTypes[i].HelperName = EditorGUI.TextField(changingRect, FieldTypes[i].HelperName);

					changingRect.x += rect.width * 0.3f + 10;
					FieldTypes[i].Type = (ForgeAcceptableRPCTypes)EditorGUI.EnumPopup(changingRect, FieldTypes[i].Type);

					changingRect.x += rect.width * 0.4f + 10;
					changingRect.width = rect.width * 0.2f;

					GUI.color = Color.red;
					if (GUI.Button(changingRect, GUIContent.none))
					{
						FieldTypes.RemoveAt(i);
						i--;
					}

					if (ForgeNetworkingEditor.ProVersion)
						GUI.color = Color.white;
					else
						GUI.color = Color.black;

					GUI.Label(changingRect, "Remove", EditorStyles.boldLabel);
					//GUI.DrawTexture(changingRect, ForgeNetworkingEditor.SubtractIcon, ScaleMode.ScaleToFit);
					GUI.color = Color.white;
				}

				rect.height += EditorGUIUtility.singleLineHeight;
				changingRect.y += EditorGUIUtility.singleLineHeight;

				changingRect.x = rect.width * 0.1f;
				changingRect.width = rect.width * 0.3f;

				GUI.color = Color.green;
				if (GUI.Button(changingRect, GUIContent.none))
				{
					FieldTypes.Add(new ForgeNRPCTypes() { Type = ForgeAcceptableRPCTypes.BYTE });
				}
				if (ForgeNetworkingEditor.ProVersion)
					GUI.color = Color.white;
				else
					GUI.color = Color.black;

				GUI.Label(changingRect, "Add", EditorStyles.boldLabel);
				//GUI.DrawTexture(changingRect, ForgeNetworkingEditor.AddIcon, ScaleMode.ScaleToFit);
				GUI.color = Color.white;
			}
		}

		public void AddRange(ForgeAcceptableRPCTypes[] types, string[] helperNames)
		{
			for (int i = 0; i < types.Length; ++i)
			{
				FieldTypes.Add(new ForgeNRPCTypes() { Type = types[i], HelperName = helperNames[i] });
			}
		}
	}
}