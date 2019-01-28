using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	/// <summary>
	/// This is the forge editor button that contains key information to the behavior or network object
	/// </summary>
	[Serializable]
	public class ForgeEditorButton
	{
		public string ButtonName;
		private string _defaultName;

		public bool IsNetworkBehavior { get { return ButtonName.EndsWith("Behavior"); } }
		public bool IsNetworkObject { get { return ButtonName.EndsWith("NetworkObject"); } }

		public string StrippedSearchName
		{
			get
			{
				return IsNetworkBehavior ? ButtonName.Substring(0, ButtonName.IndexOf("Behavior")) :
					IsNetworkObject ? ButtonName.Substring(0, ButtonName.IndexOf("NetworkObject")) :
					ButtonName;
			}
		}

		public bool CanRender = true;
		public bool IsCreated = false;
		public bool MarkedForDeletion = false;
		private ForgeBaseClassType _baseType = ForgeBaseClassType.MonoBehavior;
		public ForgeBaseClassType BaseType
		{
			get
			{
				if (_tiedBehavior != null)
					return _tiedBehavior.BaseType;
				return _baseType;
			}
			set
			{
				if (_tiedBehavior != null)
					_tiedBehavior.BaseType = value;
				else
					_baseType = value;
			}
		}

		public bool CanRenderFields = true;
		public bool CanRenderRPCS = true;
		[NonSerialized]
		public Color ButtonColor;
		public Action InvokedAction;
		[NonSerialized]
		private ReorderableList _classOrderList;
		public List<ForgeEditorField> ClassVariables = new List<ForgeEditorField>();
		private int _defaultClassVariablesCount;
		[NonSerialized]
		private ReorderableList _rpcOrderList;
		public List<ForgeEditorRPCField> RPCVariables = new List<ForgeEditorRPCField>();
		private int _defaultRPCVariablesCount;

		//NEW -Below RPC, not fields
		public List<ForgeEditorField> RewindVariables = new List<ForgeEditorField>();

		//private int _defaultRewindVariablesCount;

		public ForgeClassObject TiedObject;
		private ForgeEditorButton _tiedBehavior;

		public bool IsDirty
		{
			get
			{
				return _defaultName != ButtonName ||
					_defaultClassVariablesCount != ClassVariables.Count ||
					_defaultRPCVariablesCount != RPCVariables.Count/* ||
					_defaultRewindVariablesCount != RewindVariables.Count*/;
			}
		}

		public ForgeEditorButton(string name, Action callback = null)
		{
			ButtonName = name;
			InvokedAction = callback;
			ButtonColor = ForgeNetworkingEditor.CoolBlue;
			SetupLists();
		}

		public ForgeEditorButton(ForgeClassObject fcObj)
		{
			_baseType = fcObj.ObjectClassType;
			TiedObject = fcObj;
			Setup();

			if (fcObj.IsNetworkBehavior)
				ButtonColor = ForgeNetworkingEditor.DarkBlue;
			else if (fcObj.IsNetworkObject)
				ButtonColor = ForgeNetworkingEditor.LightBlue;
			else
				ButtonColor = ForgeNetworkingEditor.CoolBlue;

			if (IsNetworkObject)
			{
				for (int i = 0; i < ForgeNetworkingEditor.Instance._editorButtons.Count; ++i)
				{
					if (ForgeNetworkingEditor.Instance._editorButtons[i].StrippedSearchName == StrippedSearchName &&
						ForgeNetworkingEditor.Instance._editorButtons[i].IsNetworkBehavior)
					{
						_tiedBehavior = ForgeNetworkingEditor.Instance._editorButtons[i];
						break;
					}
				}
			}
		}

		public void UpdateButtonColor()
		{
			if (TiedObject.IsNetworkBehavior)
				ButtonColor = ForgeNetworkingEditor.DarkBlue;
			else if (TiedObject.IsNetworkObject)
				ButtonColor = ForgeNetworkingEditor.LightBlue;
			else
				ButtonColor = ForgeNetworkingEditor.CoolBlue;
		}

		public bool PossiblyMatches(string searchName)
		{
			return ButtonName.ToLower().StartsWith(searchName);
		}

		public void Render()
		{
			if (!CanRender)
				return;

			EditorGUILayout.BeginHorizontal();
			Rect verticleButton = EditorGUILayout.BeginVertical("Button");
			if (ForgeNetworkingEditor.ProVersion)
				GUI.color = ButtonColor;
			if (GUI.Button(verticleButton, GUIContent.none))
			{
				if (InvokedAction != null)
					InvokedAction();

				ForgeNetworkingEditor.Instance.ActiveButton = this;
				ForgeNetworkingEditor.Instance.ChangeMenu(ForgeNetworkingEditor.ForgeEditorActiveMenu.Modify);
			}
			EditorGUILayout.BeginHorizontal();
			if (ForgeNetworkingEditor.ProVersion)
				GUI.color = Color.white;
			else
				GUI.color = Color.black;

			GUILayout.Label(ForgeNetworkingEditor.SideArrow);
			GUILayout.FlexibleSpace();
			GUI.color = Color.white;
			GUIStyle boldStyle = new GUIStyle(GUI.skin.GetStyle("boldLabel"));
			boldStyle.alignment = TextAnchor.UpperCenter;
			GUILayout.Label(ButtonName, boldStyle);
			GUILayout.FlexibleSpace();
			if (ForgeNetworkingEditor.ProVersion)
				GUI.color = Color.white;
			else
				GUI.color = Color.black;
			GUILayout.Label(ForgeNetworkingEditor.SideArrowInverse);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			GUI.color = Color.white;

			Rect deletionButton = EditorGUILayout.BeginVertical("Button", GUILayout.Width(50));
			GUI.color = Color.red;
			if (GUI.Button(deletionButton, GUIContent.none))
			{
				MarkedForDeletion = true;
				GUILayout.EndVertical();
				return;
			}
			GUI.color = Color.white;

			GUILayout.BeginHorizontal();//Center the icon
			GUILayout.FlexibleSpace();
			GUILayout.Label(ForgeNetworkingEditor.TrashIcon, GUILayout.Height(20));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
		}

		public bool RenderExposed(Action callback = null, bool ignoreButton = false)
		{
			bool returnValue = true;
			if (TiedObject == null)
			{
				EditorStyles.boldLabel.alignment = TextAnchor.MiddleLeft;
				GUILayout.Label("Name", EditorStyles.boldLabel);
				EditorStyles.boldLabel.alignment = TextAnchor.MiddleCenter;
				ButtonName = GUILayout.TextField(ButtonName, GUILayout.Width(ForgeNetworkingEditor.Instance.position.width - 50));
			}
			else if (!ignoreButton)
			{
				Rect verticleButton = EditorGUILayout.BeginVertical("Button");
				if (ForgeNetworkingEditor.ProVersion)
					GUI.color = ButtonColor;
				if (GUI.Button(verticleButton, GUIContent.none))
				{
					if (callback != null)
					{
						callback();
						return false;
					}
				}
				EditorGUILayout.BeginHorizontal();
				if (ForgeNetworkingEditor.ProVersion)
					GUI.color = Color.white;
				else
					GUI.color = Color.black;
				GUILayout.Label(ForgeNetworkingEditor.SideArrow);
				GUI.color = Color.white;
				GUILayout.FlexibleSpace();
				GUIStyle boldStyle = new GUIStyle(GUI.skin.GetStyle("boldLabel"));
				boldStyle.alignment = TextAnchor.UpperCenter;
				GUILayout.Label(ButtonName, boldStyle);
				GUILayout.FlexibleSpace();
				if (ForgeNetworkingEditor.ProVersion)
					GUI.color = Color.white;
				else
					GUI.color = Color.black;
				GUILayout.Label(ForgeNetworkingEditor.SideArrowInverse);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				GUI.color = Color.white;
			}

			if (CanRenderFields)
			{
				EditorGUILayout.Space();
				EditorStyles.boldLabel.alignment = TextAnchor.MiddleLeft;
				GUILayout.Label("Fields", EditorStyles.boldLabel);
				EditorStyles.boldLabel.alignment = TextAnchor.MiddleCenter;

				_classOrderList.DoLayoutList();

				//Do we still need this?
				Rect addFieldBtn = EditorGUILayout.BeginVertical("Button", GUILayout.Width(75), GUILayout.Height(25));
				GUI.color = Color.green;
				if (GUI.Button(addFieldBtn, GUIContent.none))
					ClassVariables.Add(new ForgeEditorField());

				EditorGUILayout.BeginHorizontal();
				GUI.color = Color.white;
				GUILayout.FlexibleSpace();
				GUIStyle boldStyle = new GUIStyle(GUI.skin.GetStyle("boldLabel"));
				boldStyle.alignment = TextAnchor.UpperCenter;
				GUILayout.Label("Add Field", boldStyle);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				//
			}

			if (CanRenderRPCS)
			{
				EditorGUILayout.Space();
				EditorStyles.boldLabel.alignment = TextAnchor.MiddleLeft;
				GUILayout.Label("Remote Procedure Calls", EditorStyles.boldLabel);
				EditorStyles.boldLabel.alignment = TextAnchor.MiddleCenter;

				if (TiedObject != null)
					EditorGUILayout.HelpBox("Re-arranging the RPC Arguments will require you to manually update your deriving code to use the new logic, please be aware of this.", MessageType.Warning);

				_rpcOrderList.DoLayoutList();

				Rect addRpcBtn = EditorGUILayout.BeginVertical("Button", GUILayout.Width(75), GUILayout.Height(25));
				GUI.color = Color.green;
				if (GUI.Button(addRpcBtn, GUIContent.none))
				{
					RPCVariables.Add(new ForgeEditorRPCField());
				}

				//Do we still need this?
				EditorGUILayout.BeginHorizontal();
				GUI.color = Color.white;
				GUILayout.FlexibleSpace();
				GUIStyle boldStyle = new GUIStyle(GUI.skin.GetStyle("boldLabel"));
				boldStyle.alignment = TextAnchor.UpperCenter;
				GUILayout.Label("Add RPC", boldStyle);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				//
			}
			else
			{
				if (_tiedBehavior != null)
					_tiedBehavior.RenderExposed(null, true);
			}

			return returnValue;
		}

		public void ResetToDefaults()
		{
			if (_tiedBehavior != null)
				_tiedBehavior.ResetToDefaults();

			ButtonName = _defaultName;
			Setup();
		}

		public ValidationResult ValidateSetup()
		{
			ValidationResult result = new ValidationResult();
			List<string> variableNames = new List<string>();

			//Make sure the class name is valid, and that no fields/RPCs/Rewinds match the class name
			if (ForgeNetworkingEditor.IsValidName(ButtonName))
				variableNames.Add(ButtonName);
			else
				result.ReportValidationError(String.Format("Class \"{0}\" has a non-valid name", ButtonName));

			string checkedName = string.Empty;
			//Validate Fields
			for (int i = 0; i < ClassVariables.Count; ++i)
			{
				checkedName = ClassVariables[i].FieldName;
				if (variableNames.Contains(checkedName))
				{
					result.ReportValidationError(String.Format("Duplicate element \"{0}\" found in \"{1}\"", checkedName, ButtonName));
				}

				switch (ClassVariables[i].FieldName)
				{
					case "IDENTITY":
					case "networkObject":
					case "fieldAltered":
					case "_dirtyFields":
					case "dirtyFields":
						result.ReportValidationError(String.Format("Field \"{0}\" conflicts with a field name reserved for Forge Networking Remastered", checkedName));
						break;
				}

				string duplicate = string.Empty;
				if (checkedName.EndsWith("Changed"))
				{
					duplicate = checkedName.Substring(0, checkedName.LastIndexOf("Changed"));
					if (variableNames.Contains(duplicate))
					{
						result.ReportValidationError(String.Format("Field \"{0}\" conflicts with Changed event of {1}", checkedName, duplicate));
					}
				}

				if (checkedName.EndsWith("Interpolation"))
				{
					duplicate = checkedName.Substring(0, checkedName.LastIndexOf("Interpolation"));
					if (variableNames.Contains(duplicate))
					{
						result.ReportValidationError(String.Format("Field \"{0}\" conflicts with Interpolation field of {1}", checkedName, duplicate));
					}
				}

				//Check if the field name is a valid identifier
				if (ForgeNetworkingEditor.IsValidName(checkedName))
					variableNames.Add(checkedName);
				else
				{
					result.ReportValidationError(String.Format("Invalid identifier for Field \"{0}\" in \"{1}\"", checkedName, ButtonName));
				}
			}

			//Validate RPCs
			for (int i = 0; i < RPCVariables.Count; ++i)
			{

				checkedName = RPCVariables[i].FieldName;
				if (variableNames.Contains(checkedName))
				{
					result.ReportValidationError(String.Format("Duplicate element \"{0}\" found in \"{1}\"", checkedName, ButtonName));
				}

				//Check if RPC name is a valid identifier
				if (ForgeNetworkingEditor.IsValidName(checkedName))
					variableNames.Add(checkedName);
				else
				{
					result.ReportValidationError(String.Format("Invalid identifier for RPC \"{0}\" in \"{1}\"", checkedName, ButtonName));
				}

				switch (RPCVariables[i].FieldName.ToLower())
				{
					case "initialize":
					case "networkcreateobject":
						result.ReportValidationError(String.Format("RPC \"{0}\" conflicts with a method name reserved for Forge Networking Remastered", checkedName));
						break;
				}

				if (RPCVariables[i].FieldName.StartsWith("get_") ||
					RPCVariables[i].FieldName.StartsWith("set_"))
				{
					result.ReportValidationError(String.Format("RPC \"{0}\" cannot start with \"get_\" or \"set_\"", checkedName));
					break;
				}
			}

			//Validate Rewinds
			for (int i = 0; i < RewindVariables.Count; ++i)
			{
				checkedName = RewindVariables[i].FieldName;
				if (variableNames.Contains(checkedName))
				{
					result.ReportValidationError(String.Format("Duplicate element \"{0}\" found in \"{1}\"", checkedName, ButtonName));
				}
				//Check if Rewind name is a valid identifier
				if (ForgeNetworkingEditor.IsValidName(checkedName))
					variableNames.Add(checkedName);
				else
				{
					result.ReportValidationError(String.Format("Invalid identifier for Rewind \"{0}\" in \"{1}\"", checkedName, ButtonName));
				}
			}

			return result;
		}

		private void Setup()
		{
			ClassVariables.Clear();
			RPCVariables.Clear();
			RewindVariables.Clear();

			if (TiedObject.ObjectClassType == ForgeBaseClassType.Enums || TiedObject.ObjectClassType == ForgeBaseClassType.ObjectFactory)
				CanRender = false;

			if (TiedObject.ExactFilename == "NetworkBehavior") //Ignore this abstract class
				CanRender = false;

			if (TiedObject.IsNetworkObject)
				CanRenderRPCS = false;

			if (TiedObject.IsNetworkBehavior)
				CanRenderFields = false;

			ButtonName = TiedObject.ExactFilename;
			_defaultName = TiedObject.ExactFilename;
			for (int i = 0; i < TiedObject.Fields.Count; ++i)
			{
				bool canInterpolate = TiedObject.Fields[i].Interpolate;
				float interpolateValue = TiedObject.Fields[i].InterpolateValue;

				ClassVariables.Add(new ForgeEditorField(TiedObject.Fields[i].FieldName, true, TiedObject.Fields[i].FieldType, canInterpolate, interpolateValue));
			}

			if (TiedObject != null)
			{
				for (int i = 0; i < TiedObject.RPCS.Count; ++i)
				{
					ForgeEditorRPCField rpc = new ForgeEditorRPCField();

					rpc.FieldName = TiedObject.RPCS[i].RPCName;
					rpc.AddRange(TiedObject.RPCS[i].Arguments.ToArray(), TiedObject.RPCS[i].HelperTypes.ToArray());
					if (TiedObject.RPCS[i].RPCName.ToLower().Equals("initialize"))
						rpc.CanRender = false;

					RPCVariables.Add(rpc);
				}
			}

			SetupLists();
		}

		public void SetupLists()
		{
			if (ClassVariables == null)
				ClassVariables = new List<ForgeEditorField>();
			_defaultClassVariablesCount = ClassVariables.Count;
			if (_classOrderList == null)
			{
				_classOrderList = new ReorderableList(ClassVariables, typeof(ForgeEditorField), true, true, true, true);
				_classOrderList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
				{
					ClassVariables[index].Render(rect, isActive, isFocused);
				};
				_classOrderList.headerHeight = 0;
				//_classOrderList.drawHeaderCallback = (rect) =>
				//{
				//	GUI.Label(rect, "Fields");
				//};
				_classOrderList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
				{
					ClassVariables.Add(new ForgeEditorField());
				};
			}

			//TODO: RewindVariables here
			if (RPCVariables == null)
				RPCVariables = new List<ForgeEditorRPCField>();

			_defaultRPCVariablesCount = RPCVariables.Count;

			if (_rpcOrderList == null)
			{
				_rpcOrderList = new ReorderableList(RPCVariables, typeof(ForgeEditorRPCField), true, true, true, true);
				_rpcOrderList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
				{
					RPCVariables[index].Render(rect, isActive, isFocused);
				};
				_rpcOrderList.elementHeightCallback = (int index) =>
				{
					float height = EditorGUIUtility.singleLineHeight + 4;
					if (RPCVariables[index].Dropdown)
					{
						height += RPCVariables[index].FieldTypes.Count * EditorGUIUtility.singleLineHeight * 1.2f;
						height += EditorGUIUtility.singleLineHeight + 4;
					}
					return height;
				};
				_rpcOrderList.headerHeight = 0;
				//_rpcOrderList.drawHeaderCallback = (rect) =>
				//{
				//	GUI.Label(rect, "RPCs");
				//};
				_rpcOrderList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
				{
					RPCVariables.Add(new ForgeEditorRPCField());
				};
			}
		}
	}
}