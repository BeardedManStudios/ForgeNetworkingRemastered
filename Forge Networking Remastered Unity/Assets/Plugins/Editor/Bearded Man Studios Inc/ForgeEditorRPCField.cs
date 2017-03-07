using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	/// <summary>
	/// This is the rpc field for the network object
	/// </summary>
	public class ForgeEditorRPCField
	{
		public string FieldName;
		public bool CanRender = true;
		public List<ForgeNRPCTypes> FieldTypes;
		public int ArgumentCount { get { return FieldTypes.Count; } }
		public bool DELETED;
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
			returnValue.DELETED = this.DELETED;
			returnValue.Dropdown = this.Dropdown;
			returnValue.CanRender = this.CanRender;

			return returnValue;
		}

		public void Render()
		{
			if (DELETED)
				return;

			if (!CanRender)
				return;

			GUILayout.BeginHorizontal();
			FieldName = GUILayout.TextField(FieldName);

			Dropdown = EditorGUILayout.Foldout(Dropdown, "Arguments");

			Rect verticleButton = EditorGUILayout.BeginVertical("Button", GUILayout.Width(50), GUILayout.Height(10));
			GUI.color = Color.red;
			if (GUI.Button(verticleButton, GUIContent.none))
			{
				DELETED = true;
				return;
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

		public void AddRange(ForgeAcceptableRPCTypes[] types, string[] helperNames)
		{
			for (int i = 0; i < types.Length; ++i)
			{
				FieldTypes.Add(new ForgeNRPCTypes() { Type = types[i], HelperName = helperNames[i] });
			}
		}
	}
}