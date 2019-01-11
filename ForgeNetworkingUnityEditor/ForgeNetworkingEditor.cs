//#define FORGE_EDITOR_DEBUGGING

using BeardedManStudios.Templating;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	public class ForgeNetworkingEditor : EditorWindow
	{
		#region Constants
		/// <summary>
		/// This is the default interpolation we will be using
		/// </summary>
		public const float DEFAULT_INTERPOLATE_TIME = 0.15f;
		/// <summary>
		/// This is the editor directory to pull any extra files from
		/// </summary>
		public const string EDITOR_RESOURCES_DIR = "BMS_Forge_Editor";
		#endregion

		#region Private Variables
		/// <summary>
		/// This is a singleton class because we need to access it in our nested classes
		/// </summary>
		private static ForgeNetworkingEditor _instance;
		public static ForgeNetworkingEditor Instance { get { return _instance; } }

		/// <summary>
		/// This is the network class we are searching for
		/// </summary>
		private string _searchField = "";
		/// <summary>
		/// This is the path we are storing the Forge Networking generated classes
		/// </summary>
		private string _storingPath;
		/// <summary>
		/// This is the path we are storing the Forge Networking generated classes that the user has made
		/// </summary>
		private string _userStoringPath;
		/// <summary>
		/// This will toggle the lighting background of the window
		/// </summary>
		private bool _lightsOff = true;
		/// <summary>
		/// Determined for whether they are on the pro skin or not, this will allow the text to be the proper colors
		/// </summary>
		public static bool ProVersion = false;

		/// <summary>
		/// This is the generated folder path
		/// </summary>
		private const string GENERATED_FOLDER_PATH = "Bearded Man Studios Inc/Generated";
		/// <summary>
		/// This is the user generated folder path
		/// </summary>
		private const string USER_GENERATED_FOLDER_PATH = "Bearded Man Studios Inc/Generated/UserGenerated";
		/// <summary>
		/// This is the wizard data stored by the user previously
		/// </summary>
		private const string FN_WIZARD_DATA = "FNWizardData.bin";

		/// <summary>
		/// This is our scrolling bar we will use for looking through the classes
		/// </summary>
		private Vector2 _scrollView;
		/// <summary>
		/// This is the active forge networking class we are working with
		/// </summary>
		public ForgeEditorButton ActiveButton;

		/// <summary>
		/// Reference to all the editor buttons to serialize
		/// </summary>
		public List<ForgeEditorButton> _editorButtons;

		/// <summary>
		/// This is a reference to all the variables that can be generated
		/// </summary>
		private Dictionary<object, string> _referenceVariables = new Dictionary<object, string>();
		/// <summary>
		/// This is the bitwise 
		/// </summary>
		private List<string> _referenceBitWise = new List<string>();

		/// <summary>
		/// This is the undo action available when we hit the create button
		/// </summary>
		private Action _createUndo;
		/// <summary>
		/// This is the undo action available when we hit the modify button
		/// </summary>
		private Action _modifyUndo;

		/// <summary>
		/// The current menu we are looking at
		/// </summary>
		private ForgeEditorActiveMenu _currentMenu = ForgeEditorActiveMenu.Main;

		private static CodeDomProvider _provider = CodeDomProvider.CreateProvider("C#");

		//COLORS!
		//Reference to all the cool colors we use!
		public static Color Gold = new Color(1, 0.8f, 0.6f);
		public static Color TealBlue = new Color(0.5f, 0.87f, 1f);
		public static Color CoolBlue = new Color(0.4f, 0.7f, 1);
		public static Color LightBlue = new Color(0.6f, 0.8f, 1);
		public static Color ShadedBlue = new Color(0.24f, 0.72f, 1);
		public static Color DarkBlue = new Color(0.2f, 0.6f, 1);
		public static Color LightsOffBackgroundColor = new Color(0.0117f, 0.122f, 0.192f);
		public static Color LightsOnBackgroundColor = new Color(0.0118f, 0.18f, 0.286f);

		#region Textures
		//TEXTURES! ALL THE TEXTURES
		public static Texture2D Arrow;
		public static Texture2D SideArrow;
		public static Texture2D SideArrowInverse;
		public static Texture2D Star;
		public static Texture2D TrashIcon;
		public static Texture2D SubtractIcon;
		public static Texture2D AddIcon;
		public static Texture2D SaveIcon;
		public static Texture2D LightbulbIcon;
		public static Texture2D BackgroundTexture;
		#endregion

		#endregion

		#region Nested Enums
		/// <summary>
		/// We have different states we can access from the editor window
		/// </summary>
		public enum ForgeEditorActiveMenu
		{
			Main = 0,
			Create,
			Modify
		}
		#endregion

		#region Initialize and Constructor
		// Add menu named "Network Editor" to the Window menu
		[MenuItem("Window/Forge Networking/Network Contract Wizard %g")]
		public static void Init()
		{
			if (_instance == null)
			{
				// Get existing open window or if none, make a new one:
				_instance = (ForgeNetworkingEditor)EditorWindow.GetWindow(typeof(ForgeNetworkingEditor));
				_instance.Initialize();
				_instance.Show();
			}
			else
			{
				_instance.CloseFinal();
			}
		}

		/// <summary>
		/// Setup all the variables to be used within the editor
		/// </summary>
		public void Initialize()
		{
			titleContent = new GUIContent("Forge Wizard");
			ProVersion = EditorGUIUtility.isProSkin;

			ForgeClassObject.IDENTITIES = 0;
			_referenceBitWise = new List<string>();
			_referenceBitWise.Add("0x1");
			_referenceBitWise.Add("0x2");
			_referenceBitWise.Add("0x4");
			_referenceBitWise.Add("0x8");
			_referenceBitWise.Add("0x10");
			_referenceBitWise.Add("0x20");
			_referenceBitWise.Add("0x40");
			_referenceBitWise.Add("0x80");

			_referenceVariables = new Dictionary<object, string>();
			_referenceVariables.Add(typeof(bool).Name, "bool");
			_referenceVariables.Add(typeof(byte).Name, "byte");
			_referenceVariables.Add(typeof(char).Name, "char");
			_referenceVariables.Add(typeof(short).Name, "short");
			_referenceVariables.Add(typeof(ushort).Name, "ushort");
			_referenceVariables.Add(typeof(int).Name, "int");
			_referenceVariables.Add(typeof(uint).Name, "uint");
			_referenceVariables.Add(typeof(float).Name, "float");
			_referenceVariables.Add(typeof(long).Name, "long");
			_referenceVariables.Add(typeof(ulong).Name, "ulong");
			_referenceVariables.Add(typeof(double).Name, "double");
			_referenceVariables.Add(typeof(string).Name, "string");
			_referenceVariables.Add(typeof(Vector2).Name, "Vector2");
			_referenceVariables.Add(typeof(Vector3).Name, "Vector3");
			_referenceVariables.Add(typeof(Vector4).Name, "Vector4");
			_referenceVariables.Add(typeof(Quaternion).Name, "Quaternion");
			_referenceVariables.Add(typeof(Color).Name, "Color");
			_referenceVariables.Add(typeof(object).Name, "object");
			_referenceVariables.Add(typeof(object[]).Name, "object[]");
			_referenceVariables.Add(typeof(byte[]).Name, "byte[]");

			_scrollView = Vector2.zero;
			_editorButtons = new List<ForgeEditorButton>();
			_instance = this;

			_storingPath = Path.Combine(Application.dataPath, GENERATED_FOLDER_PATH);
			_userStoringPath = Path.Combine(Application.dataPath, USER_GENERATED_FOLDER_PATH);

			if (!Directory.Exists(_storingPath))
				Directory.CreateDirectory(_storingPath);

			if (!Directory.Exists(_userStoringPath))
				Directory.CreateDirectory(_userStoringPath);

			string[] files = Directory.GetFiles(_storingPath, "*.cs", SearchOption.TopDirectoryOnly);
			string[] userFiles = Directory.GetFiles(_userStoringPath, "*.cs", SearchOption.TopDirectoryOnly);

			//if (File.Exists(Path.Combine(Application.persistentDataPath, FN_WIZARD_DATA))) //Check for our temp file, this will make it so that we can load this data from memory regaurdless of errors
			//{
			//	IFormatter bFormatter = new BinaryFormatter();
			//	bool updateColors = false;
			//	using (Stream s = new FileStream(Path.Combine(Application.persistentDataPath, FN_WIZARD_DATA), FileMode.Open, FileAccess.Read, FileShare.Read))
			//	{
			//		try
			//		{
			//			object deserializedObject = bFormatter.Deserialize(s);
			//			if (deserializedObject != null)
			//			{
			//				_editorButtons = (List<ForgeEditorButton>)deserializedObject;
			//				bool cleared = true;
			//				for (int i = 0; i < _editorButtons.Count; ++i)
			//				{
			//					_editorButtons[i].SetupLists();
			//					if (_editorButtons[i].TiedObject == null)
			//					{
			//						cleared = false;
			//						break;
			//					}
			//				}

			//				if (cleared)
			//					updateColors = true;
			//				else
			//				{
			//					_editorButtons = new List<ForgeEditorButton>();
			//					ReloadScripts(files, userFiles);
			//				}
			//			}
			//			else
			//				ReloadScripts(files, userFiles);
			//		}
			//		catch
			//		{
			//			ReloadScripts(files, userFiles);
			//		}
			//	}

			//	if (updateColors)
			//	{
			//		for (int i = 0; i < _editorButtons.Count; ++i)
			//		{
			//			_editorButtons[i].UpdateButtonColor();
			//			if (_editorButtons[i].IsNetworkObject)
			//				ForgeClassObject.IDENTITIES++;
			//		}
			//	}
			//}
			//else
			ReloadScripts(files, userFiles);

			#region Texture Loading
			Arrow = Resources.Load<Texture2D>("Arrow");
			SideArrow = Resources.Load<Texture2D>("SideArrow");
			SideArrowInverse = FlipTexture(SideArrow);
			Star = Resources.Load<Texture2D>("Star");
			TrashIcon = Resources.Load<Texture2D>("Trash");
			SubtractIcon = Resources.Load<Texture2D>("Subtract");
			AddIcon = Resources.Load<Texture2D>("Add");
			SaveIcon = Resources.Load<Texture2D>("Save");
			LightbulbIcon = Resources.Load<Texture2D>("Lightbulb");
			BackgroundTexture = new Texture2D(1, 1);
			BackgroundTexture.SetPixel(0, 0, LightsOffBackgroundColor);
			BackgroundTexture.Apply();
			#endregion

			_createUndo = () =>
			{
				if (ActiveButton.IsDirty)
				{
					if (EditorUtility.DisplayDialog("Confirmation", "Are you sure? This will trash the current object", "Yes", "No"))
					{
						ActiveButton = null;
						ChangeMenu(ForgeEditorActiveMenu.Main);
					}
				}
				else
				{
					//We don't care because they didn't do anything
					ActiveButton = null;
					ChangeMenu(ForgeEditorActiveMenu.Main);
				}
			};

			_modifyUndo = () =>
			{
				bool isDirty = ActiveButton.IsDirty;

				if (isDirty)
				{
					if (EditorUtility.DisplayDialog("Confirmation", "Are you sure? This will undo the current changes", "Yes", "No"))
					{
						ActiveButton.ResetToDefaults();
						ActiveButton = null;
						ChangeMenu(ForgeEditorActiveMenu.Main);
					}
				}
				else
				{
					ActiveButton.ResetToDefaults();
					ActiveButton = null;
					ChangeMenu(ForgeEditorActiveMenu.Main);
				}
			};
			AssetDatabase.Refresh();
		}

		private Texture2D FlipTexture(Texture2D asset)
		{
			int width = asset.width;
			int height = asset.height;

			Texture2D flippedTexture = new Texture2D(width, height);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					flippedTexture.SetPixel(width - x - 1, y, asset.GetPixel(x, y));
				}
			}
			flippedTexture.Apply();

			return flippedTexture;
		}

		public void CloseFinal()
		{
			Close();
			_instance = null;
		}
		#endregion

		/// <summary>
		/// Changes the menu to a different one of the editor
		/// </summary>
		/// <param name="nextMenu"></param>
		public void ChangeMenu(ForgeEditorActiveMenu nextMenu)
		{
			_currentMenu = nextMenu;
		}

		private void OnGUI()
		{
			if (ProVersion)
				GUI.DrawTexture(new Rect(0, 0, position.width, position.height), BackgroundTexture);

			EditorGUILayout.BeginHorizontal();
			if (ProVersion)
			{
				Rect backBtn = EditorGUILayout.BeginVertical("Button", GUILayout.Width(40), GUILayout.Height(10));
				GUI.color = _lightsOff ? LightBlue : DarkBlue;
				if (GUI.Button(backBtn, GUIContent.none))
				{
					_lightsOff = !_lightsOff;

					if (_lightsOff)
					{
						BackgroundTexture.SetPixel(0, 0, LightsOffBackgroundColor);
						BackgroundTexture.Apply();
					}
					else
					{
						BackgroundTexture.SetPixel(0, 0, LightsOnBackgroundColor);
						BackgroundTexture.Apply();
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();
					return;
				}
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				GUI.color = Color.white;
				GUILayout.FlexibleSpace();
				GUILayout.Label(LightbulbIcon);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}

			GUILayout.FlexibleSpace();
			EditorGUILayout.BeginVertical();
			GUILayout.Label("Network Contract Wizard", EditorStyles.boldLabel);
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Separator();
			switch (_currentMenu)
			{
				case ForgeEditorActiveMenu.Main:
					RenderMainMenu();
					break;
				case ForgeEditorActiveMenu.Create:
					RenderCreateMenu();
					break;
				case ForgeEditorActiveMenu.Modify:
					RenderModifyWindow();
					break;
			}
		}

		#region Menu Renders
		/// <summary>
		/// This will render the main menu
		/// </summary>
		private void RenderMainMenu()
		{
			if (_editorButtons == null)
			{
				Initialize();
				return; //Editor is getting refreshed
			}

			EditorGUILayout.HelpBox("Please note when using source control to please ignore the FNWizardData.bin that is generated because of the NCW. This is because the serialization is based on the computer that has done it. The serialization is a process to help make upgrading easier, so this file is not necessary unless upgrading.", MessageType.Info);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Search", GUILayout.Width(50));
			_searchField = GUILayout.TextField(_searchField);
			Rect verticleButton = EditorGUILayout.BeginVertical("Button", GUILayout.Width(100), GUILayout.Height(15));
			if (ProVersion)
				GUI.color = TealBlue;
			if (GUI.Button(verticleButton, GUIContent.none))
			{
				ActiveButton = new ForgeEditorButton("");
				ActiveButton.IsCreated = true;
				ChangeMenu(ForgeEditorActiveMenu.Create);
			}
			GUI.color = Color.white;
			GUILayout.BeginHorizontal();
			GUILayout.Label(Star);
			GUILayout.Label("Create", EditorStyles.boldLabel);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			GUILayout.EndHorizontal();

			_scrollView = GUILayout.BeginScrollView(_scrollView);

			for (int i = 0; i < _editorButtons.Count; ++i)
			{
				if (_editorButtons[i].IsNetworkBehavior)
					continue;

				if (string.IsNullOrEmpty(_searchField) || _editorButtons[i].PossiblyMatches(_searchField))
					_editorButtons[i].Render();

				if (_editorButtons[i].MarkedForDeletion)
				{
					if (EditorUtility.DisplayDialog("Confirmation", "Are you sure? This will delete the class", "Yes", "No"))
					{
						if (_editorButtons[i].TiedObject.IsNetworkObject || _editorButtons[i].TiedObject.IsNetworkBehavior)
						{
							//Then we will need to remove this from the factory and destroy the other object as well
							string searchName = _editorButtons[i].TiedObject.StrippedSearchName;
							string folderPath = _editorButtons[i].TiedObject.FileLocation.Substring(0, _editorButtons[i].TiedObject.FileLocation.Length - _editorButtons[i].TiedObject.Filename.Length);
							string filePathBehavior = Path.Combine(folderPath, searchName + "Behavior.cs");
							string filePathNetworkedObj = Path.Combine(folderPath, searchName + "NetworkObject.cs");

							if (File.Exists(filePathBehavior)) //Delete the behavior
								File.Delete(filePathBehavior);
							if (File.Exists(filePathNetworkedObj)) //Delete the object
								File.Delete(filePathNetworkedObj);
							_editorButtons.RemoveAt(i);

							string factoryData = SourceCodeFactory();
							using (StreamWriter sw = File.CreateText(Path.Combine(_storingPath, "NetworkObjectFactory.cs")))
							{
								sw.Write(factoryData);
							}

							string networkManagerData = SourceCodeNetworkManager();
							using (StreamWriter sw = File.CreateText(Path.Combine(_storingPath, "NetworkManager.cs")))
							{
								sw.Write(networkManagerData);
							}

							//IFormatter previousSavedState = new BinaryFormatter();
							//using (Stream s = new FileStream(Path.Combine(Application.persistentDataPath, FN_WIZARD_DATA), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
							//{
							//	previousSavedState.Serialize(s, _editorButtons);
							//}
						}
						else
						{
							//Random object
							//File.Delete(_editorButtons[i].TiedObject.FileLocation);
						}
						AssetDatabase.Refresh();
						CloseFinal();
						break;
					}
					else
						_editorButtons[i].MarkedForDeletion = false;
				}
			}

			GUILayout.EndScrollView();

			Rect backBtn = EditorGUILayout.BeginVertical("Button", GUILayout.Height(50));
			if (ProVersion)
				GUI.color = ShadedBlue;
			if (GUI.Button(backBtn, GUIContent.none))
			{
				//CloseFinal();
				Close();
				_instance = null;
				return;
			}
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			GUI.color = Color.white;
			GUILayout.FlexibleSpace();
			GUIStyle boldStyle = new GUIStyle(GUI.skin.GetStyle("boldLabel"));
			boldStyle.alignment = TextAnchor.UpperCenter;
			GUILayout.Label("Close", boldStyle);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

		/// <summary>
		/// This will render the create menu
		/// </summary>
		private void RenderCreateMenu()
		{
			if (_editorButtons == null)
			{
				Initialize();
				return; //Editor is getting refreshed
			}

			if (ActiveButton == null)
				return;

			EditorGUILayout.Space();
			_scrollView = GUILayout.BeginScrollView(_scrollView);
			bool canBack = ActiveButton.TiedObject == null;
			bool renderedSuccessful = ActiveButton.RenderExposed(_createUndo);
			GUILayout.EndScrollView();

			if (!renderedSuccessful)
				return;

			bool generatedMonobehavior = EditorGUILayout.Toggle("Generate MonoBehavior", ActiveButton.BaseType != ForgeBaseClassType.NetworkBehavior);

			if (generatedMonobehavior)
				ActiveButton.BaseType = ForgeBaseClassType.MonoBehavior;
			else
				ActiveButton.BaseType = ForgeBaseClassType.NetworkBehavior;

			GUILayout.BeginHorizontal();
			if (canBack)
			{
				Rect backBtn = EditorGUILayout.BeginVertical("Button", GUILayout.Width(100), GUILayout.Height(50));
				GUI.color = Color.red;
				if (GUI.Button(backBtn, GUIContent.none))
				{
					if (_createUndo != null)
						_createUndo();
				}
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				GUI.color = Color.white;
				GUILayout.FlexibleSpace();
				GUIStyle boldStyle = new GUIStyle(GUI.skin.GetStyle("boldLabel"));
				boldStyle.alignment = TextAnchor.UpperCenter;
				GUILayout.Label("Back", boldStyle);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.Space();

			Rect verticleButton = EditorGUILayout.BeginVertical("Button", GUILayout.Width(100), GUILayout.Height(50));
			if (ProVersion)
				GUI.color = TealBlue;
			if (GUI.Button(verticleButton, GUIContent.none))
			{
				_editorButtons.Add(ActiveButton);
				Compile();
				ChangeMenu(ForgeEditorActiveMenu.Main);
			}
			GUI.color = Color.white;
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			GUILayout.Label(SaveIcon);
			GUILayout.Label("Save & Compile", EditorStyles.boldLabel);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();


			GUILayout.EndHorizontal();
		}

		/// <summary>
		/// This will render the modify window
		/// </summary>
		private void RenderModifyWindow()
		{
			if (_editorButtons == null || ActiveButton == null)
			{
				Initialize();
				return; //Editor is getting refreshed
			}

			EditorGUILayout.Space();
			_scrollView = GUILayout.BeginScrollView(_scrollView);

			bool renderSuccessful = ActiveButton.RenderExposed(_modifyUndo);
			GUILayout.EndScrollView();

			if (!renderSuccessful)
				return;

			bool generatedMonobehavior = EditorGUILayout.Toggle("Generate MonoBehavior", ActiveButton.BaseType != ForgeBaseClassType.NetworkBehavior);

			if (generatedMonobehavior)
				ActiveButton.BaseType = ForgeBaseClassType.MonoBehavior;
			else
				ActiveButton.BaseType = ForgeBaseClassType.NetworkBehavior;

			GUILayout.BeginHorizontal();
			Rect backBtn = EditorGUILayout.BeginVertical("Button", GUILayout.Width(100), GUILayout.Height(50));
			GUI.color = Color.red;
			if (GUI.Button(backBtn, GUIContent.none))
			{
				if (_modifyUndo != null)
					_modifyUndo();
			}
			GUI.color = Color.white;
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			GUI.color = Color.white;
			GUILayout.FlexibleSpace();
			GUIStyle boldStyle = new GUIStyle(GUI.skin.GetStyle("boldLabel"));
			boldStyle.alignment = TextAnchor.UpperCenter;
			GUILayout.Label("Back", boldStyle);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();

			Rect verticleButton = EditorGUILayout.BeginVertical("Button", GUILayout.Width(100), GUILayout.Height(50));
			if (ProVersion)
				GUI.color = TealBlue;
			if (GUI.Button(verticleButton, GUIContent.none))
			{
				_editorButtons.Add(ActiveButton);
				Compile();
				ChangeMenu(ForgeEditorActiveMenu.Main);
			}
			GUI.color = Color.white;
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			GUILayout.Label(SaveIcon);
			GUILayout.Label("Save & Compile", EditorStyles.boldLabel);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			GUILayout.EndHorizontal();
		}
		#endregion

		#region Forge Factory
		/// <summary>
		/// Generate the forge factory class
		/// </summary>
		public void MakeForgeFactory()
		{
			string classGenerationFactory = SourceCodeFactory();

			using (StreamWriter sw = File.CreateText(Path.Combine(_storingPath, "NetworkObjectFactory.cs")))
			{
				sw.Write(classGenerationFactory);
			}
		}

		#endregion

		#region Code Generation
		/// <summary>
		/// Generate a source network object based on the class and button provided
		/// </summary>
		/// <param name="cObj">The class we a generating</param>
		/// <param name="btn">The button that holds key information to this class</param>
		/// <param name="identity">The network identity that we will assing this class</param>
		/// <returns>The generated string to save to a file</returns>
		public string SourceCodeNetworkObject(ForgeClassObject cObj, ForgeEditorButton btn, int identity)
		{
			TextAsset asset = Resources.Load<TextAsset>(EDITOR_RESOURCES_DIR + "/NetworkObjectTemplate");
			TemplateSystem template = new TemplateSystem(asset.text);

			template.AddVariable("className", btn.StrippedSearchName + "NetworkObject");
			template.AddVariable("identity", cObj == null ? identity : cObj.IdentityValue);
			template.AddVariable("bitwiseSize", Math.Ceiling(btn.ClassVariables.Count / 8.0));

			List<object[]> variables = new List<object[]>();
			List<object[]> rewinds = new List<object[]>();
			string interpolateValues = string.Empty;

			string interpolateType = string.Empty;
			int i = 0, j = 0;
			for (i = 0, j = 0; i < btn.ClassVariables.Count; ++i)
			{
				Type t = ForgeClassFieldValue.GetTypeFromAcceptable(btn.ClassVariables[i].FieldType);
				interpolateType = ForgeClassFieldValue.GetInterpolateFromAcceptable(_referenceVariables[t.Name], btn.ClassVariables[i].FieldType);

				if (i != 0 && i % 8 == 0)
					j++;

				object[] fieldData = new object[]
				{
					_referenceVariables[t.Name],						// Data type
					btn.ClassVariables[i].FieldName.Replace(" ", string.Empty),	// Field name
					btn.ClassVariables[i].Interpolate,					// Interpolated
					interpolateType,									// Interpolate type
					btn.ClassVariables[i].InterpolateValue,				// Interpolate time
					_referenceBitWise[i % 8],							// Hexcode
					j													// Dirty fields index
				};

				if (i + 1 < btn.ClassVariables.Count)
					interpolateValues += btn.ClassVariables[i].InterpolateValue.ToString(CultureInfo.InvariantCulture) + ",";
				else
					interpolateValues += btn.ClassVariables[i].InterpolateValue.ToString(CultureInfo.InvariantCulture);

				variables.Add(fieldData);
			}

			// TODO:  This should relate to the rewind variables
			for (i = 0; i < 0; i++)
			{
				object[] rewindData = new object[]
				{
					"Vector3",		// The data type for this rewind
					"Position",		// The name except with the first letter uppercase
					5000			// The time in ms for this rewind to track
				};

				rewinds.Add(rewindData);
			}

			template.AddVariable("variables", variables.ToArray());
			template.AddVariable("rewinds", rewinds.ToArray());
			template.AddVariable("interpolateValues", interpolateValues.Replace("\"", "\\\""));
			return template.Parse();
		}

		/// <summary>
		/// Generate a network behavior from a class object and a button that contains key information about the class
		/// </summary>
		/// <param name="cObj">The class object</param>
		/// <param name="btn">The button containing key information</param>
		/// <returns>The generated string to save to a file</returns>
		public string SourceCodeNetworkBehavior(ForgeClassObject cObj, ForgeEditorButton btn)
		{
			string behaviorPath = string.Empty;

			if (btn.BaseType == ForgeBaseClassType.NetworkBehavior)
				behaviorPath = EDITOR_RESOURCES_DIR + "/StandAloneNetworkBehaviorTemplate";
			else
				behaviorPath = EDITOR_RESOURCES_DIR + "/NetworkBehaviorTemplate";

			TextAsset asset = Resources.Load<TextAsset>(behaviorPath);
			TemplateSystem template = new TemplateSystem(asset.text);

			template.AddVariable("className", btn.StrippedSearchName + "Behavior");
			template.AddVariable("networkObject", btn.StrippedSearchName + "NetworkObject");
			StringBuilder generatedJSON = new StringBuilder();
			StringBuilder generatedHelperTypesJSON = new StringBuilder();

			string caps = "QWERTYUIOPASDFGHJKLZXCVBNM";
			List<object[]> rpcs = new List<object[]>();
			List<object[]> constRpcs = new List<object[]>();

			for (int i = 0; i < btn.RPCVariables.Count; ++i)
			{
				StringBuilder innerTypes = new StringBuilder();
				StringBuilder helperNames = new StringBuilder();
				StringBuilder innerJSON = new StringBuilder();
				StringBuilder innerHelperTypesJSON = new StringBuilder();
				for (int x = 0; x < btn.RPCVariables[i].ArgumentCount; ++x)
				{
					Type t = ForgeClassFieldRPCValue.GetTypeFromAcceptable(btn.RPCVariables[i].FieldTypes[x].Type);

					helperNames.AppendLine("\t\t/// " + _referenceVariables[t.Name] + " " + btn.RPCVariables[i].FieldTypes[x].HelperName);

					string fieldHelper = btn.RPCVariables[i].FieldTypes[x].HelperName;
					if (x + 1 < btn.RPCVariables[i].ArgumentCount)
					{
						innerTypes.Append(", typeof(" + _referenceVariables[t.Name] + ")");
						innerJSON.Append("\"" + _referenceVariables[t.Name] + "\", ");
						innerHelperTypesJSON.Append("\"" + fieldHelper + "\", ");

					}
					else
					{
						innerTypes.Append(", typeof(" + _referenceVariables[t.Name] + ")");
						innerJSON.Append("\"" + _referenceVariables[t.Name] + "\"");
						innerHelperTypesJSON.Append("\"" + fieldHelper + "\"");
					}
				}

				object[] rpcData = new object[]
				{
					btn.RPCVariables[i].FieldName,				// The function name
					innerTypes.ToString(),						// The list of types
					helperNames.ToString().TrimEnd()
				};

				string constRpc = "";
				for (int j = 0; j < btn.RPCVariables[i].FieldName.Length; j++)
				{
					if (constRpc.Length > 0 && caps.Contains(btn.RPCVariables[i].FieldName[j]))
						constRpc += "_";

					constRpc += btn.RPCVariables[i].FieldName[j].ToString().ToUpper();
				}
				constRpc = constRpc.Replace("R_P_C_", "");

				object[] constRpcData = new object[]
				{
					constRpc,				                    // The function name
					innerTypes.ToString(),						// The list of types
					helperNames.ToString().TrimEnd()
				};

				rpcs.Add(rpcData);
				constRpcs.Add(constRpcData);
				generatedJSON.Append("[");
				generatedJSON.Append(innerJSON.ToString());
				generatedJSON.Append("]");
				generatedHelperTypesJSON.Append("[");
				generatedHelperTypesJSON.Append(innerHelperTypesJSON.ToString());
				generatedHelperTypesJSON.Append("]");
			}

			template.AddVariable("generatedTypes", generatedJSON.ToString().Replace("\"", "\\\""));
			template.AddVariable("generatedHelperTypes", generatedHelperTypesJSON.ToString().Replace("\"", "\\\""));
			template.AddVariable("rpcs", rpcs.ToArray());
			template.AddVariable("constRpcs", constRpcs.ToArray());

			return template.Parse();
		}

		/// <summary>
		/// Generates the code factory for all our custom network objects
		/// </summary>
		/// <returns>The string for the save file</returns>
		public string SourceCodeFactory()
		{
			TextAsset asset = Resources.Load<TextAsset>(EDITOR_RESOURCES_DIR + "/NetworkObjectFactoryTemplate");
			TemplateSystem template = new TemplateSystem(asset.text);

			List<object> networkObjects = new List<object>();
			for (int i = 0; i < _editorButtons.Count; ++i)
			{
				if (!_editorButtons[i].IsNetworkObject)
					continue;

				string name = _editorButtons[i].StrippedSearchName + "NetworkObject";
				if (networkObjects.Contains(name))
					continue;

				networkObjects.Add(name);
			}

			template.AddVariable("networkObjects", networkObjects.ToArray());
			return template.Parse();
		}

		/// <summary>
		/// Generates the network manager that will allow the instantiation of these new network objects
		/// </summary>
		/// <returns>The string to save to a file</returns>
		public string SourceCodeNetworkManager()
		{
			TextAsset asset = Resources.Load<TextAsset>(EDITOR_RESOURCES_DIR + "/NetworkManagerTemplate");
			TemplateSystem template = new TemplateSystem(asset.text);

			List<object> networkObjects = new List<object>();
			for (int i = 0; i < _editorButtons.Count; ++i)
			{
				if (!_editorButtons[i].IsNetworkObject || _editorButtons[i].BaseType == ForgeBaseClassType.NetworkBehavior)
					continue;

				string name = _editorButtons[i].StrippedSearchName;
				if (networkObjects.Contains(name))
					continue;

				networkObjects.Add(name);
			}

			template.AddVariable("networkObjects", networkObjects.ToArray());
			return template.Parse();
		}

		#endregion

		#region Compiling
		/// <summary>
		/// Reloads the scripts into the editor
		/// </summary>
		private void ReloadScripts(string[] files, string[] userFiles)
		{
			List<ForgeClassObject> correctFiles = new List<ForgeClassObject>();
			for (int i = 0; i < files.Length; ++i)
			{
				if (!files[i].EndsWith(".meta")) //Ignore all meta files
					correctFiles.Add(new ForgeClassObject(files[i]));
			}

			for (int i = 0; i < userFiles.Length; ++i)
			{
				if (!userFiles[i].EndsWith(".meta")) //Ignore all meta files
					correctFiles.Add(new ForgeClassObject(userFiles[i]));
			}

			if (!ForgeClassObject.HasExactFilename(correctFiles, "NetworkObjectFactory"))
				MakeForgeFactory(); //We do not have the Forge Factory, we need to make this!

			for (int i = 0; i < correctFiles.Count; ++i)
			{
				var btn = new ForgeEditorButton(correctFiles[i]);

				if (btn.IsNetworkObject || btn.IsNetworkBehavior)
					_editorButtons.Add(btn);
			}
		}

		/// <summary>
		/// Compiles our generated code for the user
		/// </summary>
		public void Compile()
		{
			if (ActiveButton == null)
			{
				Debug.LogError("WHAT?! LOL");
				return;
			}

			if (string.IsNullOrEmpty(ActiveButton.ButtonName))
			{
				Debug.LogError("Can't have an empty class name");
				return;
			}

			EditorApplication.LockReloadAssemblies();

			int identity = 1;
			for (int i = 0; i < _editorButtons.Count; ++i)
			{
				ForgeEditorButton btn = _editorButtons[i];
				ValidationResult validate = btn.ValidateSetup();
				if(!validate.Result)
				{
					foreach (string error in validate.errorMessages)
						Debug.LogError(error);
					Debug.LogError(String.Format("Compilation of {0} failed. Please resolve any outputted errors and try again.", btn.ButtonName));
					break;
				}

				if (_editorButtons[i].IsCreated)
				{
					//Brand new class being added!
					string networkObjectData = SourceCodeNetworkObject(null, _editorButtons[i], identity);
					string networkBehaviorData = SourceCodeNetworkBehavior(null, _editorButtons[i]);
					if (!string.IsNullOrEmpty(networkObjectData))
					{
						using (StreamWriter sw = File.CreateText(Path.Combine(_userStoringPath, string.Format("{0}{1}.cs", _editorButtons[i].StrippedSearchName, "NetworkObject"))))
						{
							sw.Write(networkObjectData);
						}

						using (StreamWriter sw = File.CreateText(Path.Combine(_userStoringPath, string.Format("{0}{1}.cs", _editorButtons[i].StrippedSearchName, "Behavior"))))
						{
							sw.Write(networkBehaviorData);
						}
						identity++;

						string strippedName = _editorButtons[i].StrippedSearchName;
						_editorButtons[i].ButtonName = strippedName + "NetworkObject";
					}
				}
				else
				{
					if (_editorButtons[i].TiedObject != null)
					{
						if (_editorButtons[i].TiedObject.IsNetworkBehavior)
						{
							string networkBehaviorData = SourceCodeNetworkBehavior(null, _editorButtons[i]);

							using (StreamWriter sw = File.CreateText(Path.Combine(_userStoringPath, _editorButtons[i].TiedObject.Filename)))
							{
								sw.Write(networkBehaviorData);
							}
						}
						else if (_editorButtons[i].TiedObject.IsNetworkObject)
						{
							string networkObjectData = SourceCodeNetworkObject(null, _editorButtons[i], identity);
							using (StreamWriter sw = File.CreateText(Path.Combine(_userStoringPath, _editorButtons[i].TiedObject.Filename)))
							{
								sw.Write(networkObjectData);
							}
							identity++;
						}
					}
				}
			}

			string factoryData = SourceCodeFactory();
			using (StreamWriter sw = File.CreateText(Path.Combine(_storingPath, "NetworkObjectFactory.cs")))
			{
				sw.Write(factoryData);
			}

			string networkManagerData = SourceCodeNetworkManager();
			using (StreamWriter sw = File.CreateText(Path.Combine(_storingPath, "NetworkManager.cs")))
			{
				sw.Write(networkManagerData);
			}

			//IFormatter previousSavedState = new BinaryFormatter();
			//using (Stream s = new FileStream(Path.Combine(Application.persistentDataPath, FN_WIZARD_DATA), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
			//{
			//    previousSavedState.Serialize(s, _editorButtons);
			//}

			EditorApplication.UnlockReloadAssemblies();

			AssetDatabase.Refresh();

			//_editorButtons.Remove(ActiveButton);
			ActiveButton = null;
			CloseFinal();
		}

		#endregion

		#region Checks
		/// <summary>
		/// Check whether a given identifier is a valid C# identifier
		/// </summary>
		/// <param name="identifier">The string to validate</param>
		/// <returns>Whether or not the input is a valid C# identifier</returns>
		public static bool IsValidName(string identifier)
		{
			return _provider.IsValidIdentifier(identifier);
		}

		#endregion
	}
}
