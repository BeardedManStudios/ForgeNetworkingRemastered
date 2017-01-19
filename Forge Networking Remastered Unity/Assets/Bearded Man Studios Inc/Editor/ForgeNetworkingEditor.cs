//#define FORGE_EDITOR_DEBUGGING

using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Templating;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	public class ForgeNetworkingEditor : EditorWindow
	{
		#region Constants
		private const float DEFAULT_INTERPOLATE_TIME = 0.15f;
		private const string EDITOR_RESOURCES_DIR = "BMS_Forge_Editor";
		public const string REGEX_MATCH = @"^[a-zA-Z]+"; //Must be either lowercase or uppercase characters
		#endregion

		#region Private Variables
		private static ForgeNetworkingEditor _instance;
		public static ForgeNetworkingEditor Instance { get { return _instance; } }

		private string _searchField = "";
		private string _storingPath;
		private string _userStoringPath;
		private bool _lightsOff = true;
		private static bool _proVersion = false;

		private const string GENERATED_FOLDER_PATH = "Generated";
		private const string USER_GENERATED_FOLDER_PATH = "Generated/UserGenerated";
		private static int IDENTITIES = 0;

		private Vector2 _scrollView;
		public ForgeEditorButton ActiveButton;
		public List<ForgeEditorButton> _editorButtons;

		private Dictionary<object, string> _referenceVariables = new Dictionary<object, string>();
		private List<string> _referenceBitWise = new List<string>();

		private Action _createUndo;
		private Action _modifyUndo;

		private ForgeEditorActiveMenu _currentMenu = ForgeEditorActiveMenu.Main;

		public static Color Gold = new Color(1, 0.8f, 0.6f);
		public static Color TealBlue = new Color(0.5f, 0.87f, 1f);
		public static Color CoolBlue = new Color(0.4f, 0.7f, 1);
		public static Color LightBlue = new Color(0.6f, 0.8f, 1);
		public static Color ShadedBlue = new Color(0.24f, 0.72f, 1);
		public static Color DarkBlue = new Color(0.2f, 0.6f, 1);
		public static Color LightsOffBackgroundColor = new Color(0.0117f, 0.122f, 0.192f);
		public static Color LightsOnBackgroundColor = new Color(0.0118f, 0.18f, 0.286f);

		#region Textures
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
		public enum ForgeEditorActiveMenu
		{
			Main = 0,
			Create,
			Modify
		}

		public enum AcceptableFieldTypes
		{
			//Unknown = -1, //Unsupported
			BYTE = 0,
			CHAR = 1,
			SHORT = 2,
			USHORT = 3,
			BOOL = 4,
			INT = 5,
			UINT = 6,
			FLOAT = 7,
			LONG = 8,
			ULONG = 9,
			DOUBLE = 10,
			//STRING = 11, //Unsupported
			VECTOR2 = 12,
			VECTOR3 = 13,
			VECTOR4 = 14,
			QUATERNION = 15,
			COLOR = 16,
			//OBJECT_ARRAY = 17, //Unsupported
			BYTE_ARRAY = 18
		}

		public enum AcceptableRPCTypes
		{
			Unknown = -1,
			BYTE = 0,
			CHAR = 1,
			SHORT = 2,
			USHORT = 3,
			BOOL = 4,
			INT = 5,
			UINT = 6,
			FLOAT = 7,
			LONG = 8,
			ULONG = 9,
			DOUBLE = 10,
			STRING = 11,
			VECTOR2 = 12,
			VECTOR3 = 13,
			VECTOR4 = 14,
			QUATERNION = 15,
			COLOR = 16,
			OBJECT_ARRAY = 17,
			BYTE_ARRAY = 18
		}

		public enum ForgeBaseClassType
		{
			Custom = 0,
			Enums,
			MonoBehavior,
			NetworkObject,
			ObjectFactory
		}
		#endregion

		#region Nested Classes
		public class ForgeClassObject
		{
			public string FileLocation;
			public string Filename;
			public string ExactFilename;

			public int IdentityValue = -1;

			public bool IsNetworkBehavior { get { return ExactFilename.EndsWith("Behavior"); } }
			public bool IsNetworkObject { get { return ExactFilename.EndsWith("NetworkObject"); } }

			public string StrippedSearchName
			{
				get
				{
					return IsNetworkBehavior ? ExactFilename.Substring(0, ExactFilename.IndexOf("Behavior")) :
						IsNetworkObject ? ExactFilename.Substring(0, ExactFilename.IndexOf("NetworkObject")) :
						ExactFilename;
				}
			}

			public ForgeBaseClassType ObjectClassType;
			public List<ForgeClassFieldValue> Fields = new List<ForgeClassFieldValue>();
			public List<ForgeClassRPCValue> RPCS = new List<ForgeClassRPCValue>();
			public List<ForgeClassRewindValue> Rewinds = new List<ForgeClassRewindValue>();

			public ForgeClassObject(string location)
			{
				this.FileLocation = location;
				this.Filename = Path.GetFileName(FileLocation);
				this.ExactFilename = Path.GetFileNameWithoutExtension(FileLocation);

				if (ExactFilename == "NetworkManager")
					return;

				List<float> _interpolationValues = new List<float>();
				JSONNode typeData = null;
				JSONNode typeHelperData = null;
				JSONNode interpolData = null;
				Type currentType = GetType("BeardedManStudios.Forge.Networking.Generated." + this.ExactFilename);
				GeneratedRPCAttribute aRPC = (GeneratedRPCAttribute)Attribute.GetCustomAttribute(currentType, typeof(GeneratedRPCAttribute));
				GeneratedRPCVariableNamesAttribute aNames = (GeneratedRPCVariableNamesAttribute)Attribute.GetCustomAttribute(currentType, typeof(GeneratedRPCVariableNamesAttribute));
				GeneratedInterpolAttribute aInterpol = (GeneratedInterpolAttribute)Attribute.GetCustomAttribute(currentType, typeof(GeneratedInterpolAttribute));

				if (aRPC != null && !string.IsNullOrEmpty(aRPC.JsonData))
					typeData = JSON.Parse(aRPC.JsonData);
				else
					typeData = new JSONClass();

				if (aNames != null && !string.IsNullOrEmpty(aNames.JsonData))
					typeHelperData = JSON.Parse(aNames.JsonData);
				else
					typeHelperData = new JSONClass();

				if (aInterpol != null && !string.IsNullOrEmpty(aInterpol.JsonData))
					interpolData = JSON.Parse(aInterpol.JsonData);
				else
					interpolData = new JSONClass();

#if FORGE_EDITOR_DEBUGGING
			string forgeClassDebug = "Loaded - " + this.ExactFilename + System.Environment.NewLine;
#endif

				List<MethodInfo> uniqueMethods = new List<MethodInfo>();
				List<PropertyInfo> uniqueProperties = new List<PropertyInfo>();
				List<FieldInfo> uniqueFields = new List<FieldInfo>();

				if (currentType == null)
					throw new NullReferenceException("CANNOT PUT SOURCE CODE IN GENERATED FOLDER! PLEASE REMOVE NON GENERATED CODE!");

				MethodInfo[] methods = currentType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Where(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(RpcArgs)).ToArray();
				PropertyInfo[] properties = currentType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				FieldInfo[] fields = currentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

				uniqueMethods.AddRange(methods);
				uniqueProperties.AddRange(properties);
				uniqueFields.AddRange(fields);

				if (currentType.BaseType != null)
				{
					Type baseType = currentType.BaseType;
					Type factoryInterface = currentType.GetInterface("INetworkObjectFactory");

					if (baseType == typeof(NetworkObject))
					{
						ObjectClassType = ForgeBaseClassType.NetworkObject;
						IdentityValue = ++IDENTITIES;
					}
					else if (baseType == typeof(MonoBehaviour))
						ObjectClassType = ForgeBaseClassType.MonoBehavior;
					else if (factoryInterface != null)
						ObjectClassType = ForgeBaseClassType.ObjectFactory;
					else if (baseType == typeof(Enum))
						ObjectClassType = ForgeBaseClassType.Enums;
					else
						ObjectClassType = ForgeBaseClassType.Custom;

					MethodInfo[] baseMethods = baseType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
					PropertyInfo[] baseProperties = baseType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
					FieldInfo[] baseFields = baseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

					for (int i = 0; i < baseMethods.Length; ++i)
					{
						for (int x = 0; x < uniqueMethods.Count; ++x)
						{
							if (uniqueMethods[x].Name == baseMethods[i].Name
							&& uniqueMethods[x].GetParameters().Length == baseMethods[i].GetParameters().Length)
							{
								var argsA = uniqueMethods[x].GetParameters();
								var argsB = baseMethods[i].GetParameters();
								bool same = true;

								for (int j = 0; j < argsA.Length; j++)
								{
									if (!argsA[j].Equals(argsB[j]))
									{
										same = false;
										break;
									}
								}

								if (same)
									uniqueMethods.RemoveAt(x);

								break;
							}
						}
					}

					for (int i = 0; i < baseProperties.Length; ++i)
					{
						for (int x = 0; x < uniqueProperties.Count; ++x)
						{
							if (uniqueProperties[x].Name == baseProperties[i].Name)
							{
								uniqueProperties.RemoveAt(x);
								break;
							}
						}
					}

					for (int i = 0; i < baseFields.Length; ++i)
					{
						for (int x = 0; x < uniqueFields.Count; ++x)
						{
							if (uniqueFields[x].Name == baseFields[i].Name)
							{
								uniqueFields.RemoveAt(x);
								break;
							}
						}
					}
				}

				#region IGNORES
				for (int i = 0; i < uniqueFields.Count; ++i)
				{
					if (uniqueFields[i].Name == "IDENTITY")
					{
						uniqueFields.RemoveAt(i);
						i--;
						continue;
					}

					if (uniqueFields[i].Name == "networkObject")
					{
						uniqueFields.RemoveAt(i);
						i--;
						continue;
					}

					if (uniqueFields[i].Name.EndsWith("Changed"))
					{
						uniqueFields.RemoveAt(i);
						i--;
						continue;
					}

					if (uniqueFields[i].Name == "_dirtyFields")
					{
						uniqueFields.RemoveAt(i);

						//TODO: Store the types for re-use
						i--;
						continue;
					}

					if (uniqueFields[i].Name.EndsWith("Interpolation"))
					{
						uniqueFields.RemoveAt(i);

						//TODO: Store the types for re-use
						i--;
						continue;
					}

					if (uniqueFields[i].Name == "dirtyFields")
					{
						uniqueFields.RemoveAt(i);

						//TODO: Store the types for re-use
						i--;
						continue;
					}
				}

				for (int i = 0; i < uniqueMethods.Count; ++i)
				{
					if (uniqueMethods[i].Name.ToLower() == "initialize")
					{
						uniqueMethods.RemoveAt(i);
						--i;
						continue;
					}

					if (uniqueMethods[i].Name.ToLower() == "networkcreateobject")
					{
						uniqueMethods.RemoveAt(i);
						--i;
						continue;
					}

					if (uniqueMethods[i].Name.EndsWith("Changed"))
					{
						uniqueMethods.RemoveAt(i);
						--i;
						continue;
					}

					if (uniqueMethods[i].Name.StartsWith("get_"))
					{
						uniqueMethods.RemoveAt(i);
						--i;
						continue;
					}

					if (uniqueMethods[i].Name.StartsWith("set_"))
					{
						uniqueMethods.RemoveAt(i);
						--i;
						continue;
					}
				}
				#endregion

#if FORGE_EDITOR_DEBUGGING
			forgeClassDebug += "Properties:\n";
			foreach (PropertyInfo a in uniqueProperties)
			{
				forgeClassDebug += a.Name + " (" + a.PropertyType + ")" + System.Environment.NewLine;
			}
			forgeClassDebug += System.Environment.NewLine;

			forgeClassDebug += "Fields:\n";
#endif
				if (ObjectClassType != ForgeBaseClassType.Enums)
				{
					if (interpolData != null)
					{
						JSONArray currentInterpolationVariables = interpolData["inter"].AsArray;
						if (currentInterpolationVariables != null)
						{
							for (int i = 0; i < currentInterpolationVariables.Count; ++i)
							{
								float interPolVal = currentInterpolationVariables[i].AsFloat;
								_interpolationValues.Add(interPolVal);
							}
						}
					}
					else
					{
						for (int i = 0; i < uniqueFields.Count; ++i)
							_interpolationValues.Add(DEFAULT_INTERPOLATE_TIME);
					}

					for (int i = 0; i < uniqueFields.Count; ++i)
					{
						ForgeClassFieldValue val = ForgeClassFieldValue.GetClassField(uniqueFields[i], currentType, _interpolationValues[i] > 0, _interpolationValues[i]);
						Fields.Add(val);
#if FORGE_EDITOR_DEBUGGING
					Debug.Log(val);
					forgeClassDebug += uniqueFields[i].Name + " (" + uniqueFields[i].FieldType + ")" + System.Environment.NewLine;
#endif
					}
				}
#if FORGE_EDITOR_DEBUGGING
			forgeClassDebug += System.Environment.NewLine;

			forgeClassDebug += "Methods:\n";
#endif
				List<List<AcceptableRPCTypes>> rpcSupportedTypes = new List<List<AcceptableRPCTypes>>();
				if (typeData != null)
				{
					JSONArray currentRPCVariables = typeData["types"].AsArray;
					if (currentRPCVariables != null)
					{
						for (int i = 0; i < currentRPCVariables.Count; ++i)
						{
							JSONArray singularArray = currentRPCVariables[i].AsArray;
							if (singularArray != null)
							{
								List<AcceptableRPCTypes> singularSupportedTypes = new List<AcceptableRPCTypes>();
								for (int x = 0; x < singularArray.Count; ++x)
								{
									AcceptableRPCTypes singularType = ForgeClassFieldRPCValue.GetTypeFromAcceptable(singularArray[x].Value);
									singularSupportedTypes.Add(singularType);
								}
								rpcSupportedTypes.Add(singularSupportedTypes);
							}
						}
					}
				}
				else
				{
					for (int i = 0; i < uniqueMethods.Count; ++i)
						rpcSupportedTypes.Add(new List<AcceptableRPCTypes>());
				}

				List<List<string>> typeHelpers = new List<List<string>>();
				if (typeHelperData != null)
				{
					JSONArray currentHelperRPCTypes = typeHelperData["types"].AsArray;
					if (currentHelperRPCTypes != null)
					{
						for (int i = 0; i < currentHelperRPCTypes.Count; ++i)
						{
							JSONArray singularHelperArray = currentHelperRPCTypes[i].AsArray;
							if (singularHelperArray != null)
							{
								List<string> singularSupportedTypes = new List<string>(new string[Mathf.Max(singularHelperArray.Count, rpcSupportedTypes.Count)]);
								for (int x = 0; x < singularHelperArray.Count; ++x)
								{
									string singularHelperType = singularHelperArray[x].Value.Replace(" ", string.Empty);
									singularSupportedTypes[x] = singularHelperType;
								}
								typeHelpers.Add(singularSupportedTypes);
							}
						}
					}
				}
				else
				{
					//This is missing the type helper data
					for (int i = 0; i < rpcSupportedTypes.Count; ++i)
					{
						typeHelpers.Add(new List<string>(new string[rpcSupportedTypes[i].Count]));
					}
				}

				for (int i = 0; i < uniqueMethods.Count; ++i)
				{
					RPCS.Add(new ForgeClassRPCValue(uniqueMethods[i], rpcSupportedTypes[i], typeHelpers[i]));
#if FORGE_EDITOR_DEBUGGING
				ParameterInfo[] paramsInfo = a.GetParameters();
				string parameters = "";
				foreach (ParameterInfo info in paramsInfo)
					parameters += info.ParameterType + ", ";

				forgeClassDebug += a.Name + " (" + parameters + ")" + System.Environment.NewLine;
#endif
				}

#if FORGE_EDITOR_DEBUGGING
			forgeClassDebug += "Class Type: " + ObjectClassType;
			forgeClassDebug += "\nSearchName: " + StrippedSearchName;
			forgeClassDebug += "\nIsNetworkedObject: " + IsNetworkObject;
			forgeClassDebug += "\nIsNetworkBehavior: " + IsNetworkBehavior;
			forgeClassDebug += System.Environment.NewLine;

			Debug.Log(forgeClassDebug);
#endif
			}

			public static bool HasFilename(List<ForgeClassObject> collection, string filename)
			{
				bool returnValue = false;

				foreach (ForgeClassObject fo in collection)
				{
					if (fo.Filename.ToLower() == filename.ToLower())
					{
						returnValue = true;
						break;
					}
				}

				return returnValue;
			}

			public static bool HasExactFilename(List<ForgeClassObject> collection, string filename)
			{
				bool returnValue = false;

				foreach (ForgeClassObject fo in collection)
				{
					if (fo.ExactFilename.ToLower() == filename.ToLower())
					{
						returnValue = true;
						break;
					}
				}

				return returnValue;
			}

			public static ForgeClassObject GetClassObjectFromFilename(List<ForgeClassObject> collection, string filename)
			{
				ForgeClassObject returnValue = null;

				foreach (ForgeClassObject fo in collection)
				{
					if (fo.Filename.ToLower() == filename.ToLower())
					{
						returnValue = fo;
						break;
					}
				}

				return returnValue;
			}

			public static ForgeClassObject GetClassObjectFromExactFilename(List<ForgeClassObject> collection, string filename)
			{
				ForgeClassObject returnValue = null;

				foreach (ForgeClassObject fo in collection)
				{
					if (fo.ExactFilename.ToLower() == filename.ToLower())
					{
						returnValue = fo;
						break;
					}
				}

				return returnValue;
			}

			public static Type GetType(string typeName)
			{
				var type = Type.GetType(typeName);
				if (type != null) return type;
				foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
				{
					type = a.GetType(typeName);
					if (type != null)
						return type;
				}
				return null;
			}
		}

		public class ForgeClassFieldRPCValue
		{
			public string FieldRPCName;
			public object FieldRPCValue;
			public bool Interpolate;
			public float InterpolateValue;
			public AcceptableRPCTypes FieldType;
			public bool IsNetworkedObject { get { return FieldRPCName.ToLower() == "networkobject"; } }

			public ForgeClassFieldRPCValue()
			{
				FieldRPCName = string.Empty;
				FieldRPCValue = null;
				Interpolate = false;
				InterpolateValue = 0;
				FieldType = AcceptableRPCTypes.BYTE;
			}

			public ForgeClassFieldRPCValue(string name, object value, AcceptableRPCTypes type, bool interpolate, float interpolateValue)
			{
				this.FieldRPCName = name;
				this.FieldRPCValue = value;
				this.FieldType = type;
				this.Interpolate = interpolate;
				this.InterpolateValue = interpolateValue;
			}

			public static ForgeClassFieldRPCValue GetClassField(FieldInfo field, Type t, bool interpolate, float interpolateValue)
			{
				string name = field.Name.Replace("_", string.Empty);
				object value = null;
				//if (!t.IsAbstract)
				//    value = field.GetValue(t);

				AcceptableRPCTypes type = AcceptableRPCTypes.BYTE;
				Type fieldType = field.FieldType;
				if (fieldType == typeof(int))
					type = AcceptableRPCTypes.INT;
				else if (fieldType == typeof(uint))
					type = AcceptableRPCTypes.UINT;
				else if (fieldType == typeof(bool))
					type = AcceptableRPCTypes.BOOL;
				else if (fieldType == typeof(byte))
					type = AcceptableRPCTypes.BYTE;
				else if (fieldType == typeof(char))
					type = AcceptableRPCTypes.CHAR;
				else if (fieldType == typeof(double))
					type = AcceptableRPCTypes.DOUBLE;
				else if (fieldType == typeof(float))
					type = AcceptableRPCTypes.FLOAT;
				else if (fieldType == typeof(long))
					type = AcceptableRPCTypes.LONG;
				else if (fieldType == typeof(ulong))
					type = AcceptableRPCTypes.ULONG;
				else if (fieldType == typeof(short))
					type = AcceptableRPCTypes.SHORT;
				else if (fieldType == typeof(ushort))
					type = AcceptableRPCTypes.USHORT;
				else if (fieldType == typeof(Color))
					type = AcceptableRPCTypes.COLOR;
				else if (fieldType == typeof(Quaternion))
					type = AcceptableRPCTypes.QUATERNION;
				else if (fieldType == typeof(Vector2))
					type = AcceptableRPCTypes.VECTOR2;
				else if (fieldType == typeof(Vector3))
					type = AcceptableRPCTypes.VECTOR3;
				else if (fieldType == typeof(Vector4))
					type = AcceptableRPCTypes.VECTOR4;
				else if (fieldType == typeof(string))
					type = AcceptableRPCTypes.STRING;
				else if (fieldType == typeof(object[]))
					type = AcceptableRPCTypes.OBJECT_ARRAY;
				else if (fieldType == typeof(byte[]))
					type = AcceptableRPCTypes.BYTE_ARRAY;
				else
					type = AcceptableRPCTypes.Unknown;

				return new ForgeClassFieldRPCValue(name, value, type, interpolate, interpolateValue);
			}

			public static Type GetTypeFromAcceptable(AcceptableRPCTypes type)
			{
				switch (type)
				{
					case AcceptableRPCTypes.INT:
						return typeof(int);
					case AcceptableRPCTypes.UINT:
						return typeof(uint);
					case AcceptableRPCTypes.BOOL:
						return typeof(bool);
					case AcceptableRPCTypes.BYTE:
						return typeof(byte);
					case AcceptableRPCTypes.CHAR:
						return typeof(char);
					case AcceptableRPCTypes.DOUBLE:
						return typeof(double);
					case AcceptableRPCTypes.FLOAT:
						return typeof(float);
					case AcceptableRPCTypes.LONG:
						return typeof(long);
					case AcceptableRPCTypes.ULONG:
						return typeof(ulong);
					case AcceptableRPCTypes.SHORT:
						return typeof(short);
					case AcceptableRPCTypes.USHORT:
						return typeof(ushort);
					case AcceptableRPCTypes.COLOR:
						return typeof(Color);
					case AcceptableRPCTypes.QUATERNION:
						return typeof(Quaternion);
					case AcceptableRPCTypes.VECTOR2:
						return typeof(Vector2);
					case AcceptableRPCTypes.VECTOR3:
						return typeof(Vector3);
					case AcceptableRPCTypes.VECTOR4:
						return typeof(Vector4);
					case AcceptableRPCTypes.STRING:
						return typeof(string);
					case AcceptableRPCTypes.OBJECT_ARRAY:
						return typeof(object[]);
					case AcceptableRPCTypes.BYTE_ARRAY:
						return typeof(byte[]);
					default:
						return null;
				}
			}

			public static AcceptableRPCTypes GetTypeFromAcceptable(string val)
			{
				switch (val.Replace(" ", string.Empty).ToLower())
				{
					case "int":
						return AcceptableRPCTypes.INT;
					case "uint":
						return AcceptableRPCTypes.UINT;
					case "bool":
						return AcceptableRPCTypes.BOOL;
					case "byte":
						return AcceptableRPCTypes.BYTE;
					case "char":
						return AcceptableRPCTypes.CHAR;
					case "double":
						return AcceptableRPCTypes.DOUBLE;
					case "float":
						return AcceptableRPCTypes.FLOAT;
					case "long":
						return AcceptableRPCTypes.LONG;
					case "ulong":
						return AcceptableRPCTypes.ULONG;
					case "short":
						return AcceptableRPCTypes.SHORT;
					case "ushort":
						return AcceptableRPCTypes.USHORT;
					case "color":
						return AcceptableRPCTypes.COLOR;
					case "quaternion":
						return AcceptableRPCTypes.QUATERNION;
					case "vector2":
						return AcceptableRPCTypes.VECTOR2;
					case "vector3":
						return AcceptableRPCTypes.VECTOR3;
					case "vector4":
						return AcceptableRPCTypes.VECTOR4;
					case "string":
						return AcceptableRPCTypes.STRING;
					case "object[]":
						return AcceptableRPCTypes.OBJECT_ARRAY;
					case "byte[]":
						return AcceptableRPCTypes.BYTE_ARRAY;
					default:
						return AcceptableRPCTypes.Unknown;
				}
			}

			public override string ToString()
			{
				return string.Format("[ Name: {0}, Value: {1}, Type: {2}, IsNetObj: {3}]", FieldRPCName, FieldRPCValue, FieldType, IsNetworkedObject);
			}
		}

		public class ForgeClassFieldValue
		{
			public string FieldName;
			public object FieldValue;
			public bool Interpolate;
			public float InterpolateValue;
			public AcceptableFieldTypes FieldType;
			public bool IsNetworkedObject { get { return FieldName.ToLower() == "networkobject"; } }

			public ForgeClassFieldValue()
			{
				FieldName = string.Empty;
				FieldValue = null;
				Interpolate = false;
				InterpolateValue = 0;
				FieldType = AcceptableFieldTypes.BYTE;
			}

			public ForgeClassFieldValue(string name, object value, AcceptableFieldTypes type, bool interpolate, float interpolateValue)
			{
				this.FieldName = name;
				this.FieldValue = value;
				this.FieldType = type;
				this.Interpolate = interpolate;
				this.InterpolateValue = interpolateValue;
			}

			public static ForgeClassFieldValue GetClassField(FieldInfo field, Type t, bool interpolate, float interpolateValue)
			{
				string name = field.Name.Replace("_", string.Empty);
				object value = null;
				//if (!t.IsAbstract)
				//    value = field.GetValue(t);

				AcceptableFieldTypes type = AcceptableFieldTypes.BYTE;
				Type fieldType = field.FieldType;
				if (fieldType == typeof(int))
					type = AcceptableFieldTypes.INT;
				else if (fieldType == typeof(uint))
					type = AcceptableFieldTypes.UINT;
				else if (fieldType == typeof(bool))
					type = AcceptableFieldTypes.BOOL;
				else if (fieldType == typeof(byte))
					type = AcceptableFieldTypes.BYTE;
				else if (fieldType == typeof(char))
					type = AcceptableFieldTypes.CHAR;
				else if (fieldType == typeof(double))
					type = AcceptableFieldTypes.DOUBLE;
				else if (fieldType == typeof(float))
					type = AcceptableFieldTypes.FLOAT;
				else if (fieldType == typeof(long))
					type = AcceptableFieldTypes.LONG;
				else if (fieldType == typeof(ulong))
					type = AcceptableFieldTypes.ULONG;
				else if (fieldType == typeof(short))
					type = AcceptableFieldTypes.SHORT;
				else if (fieldType == typeof(ushort))
					type = AcceptableFieldTypes.USHORT;
				else if (fieldType == typeof(Color))
					type = AcceptableFieldTypes.COLOR;
				else if (fieldType == typeof(Quaternion))
					type = AcceptableFieldTypes.QUATERNION;
				else if (fieldType == typeof(Vector2))
					type = AcceptableFieldTypes.VECTOR2;
				else if (fieldType == typeof(Vector3))
					type = AcceptableFieldTypes.VECTOR3;
				else if (fieldType == typeof(Vector4))
					type = AcceptableFieldTypes.VECTOR4;
				//else if (fieldType == typeof(string))
				//	type = AcceptableFieldTypes.STRING; //Unsupported
				//else if (fieldType == typeof(object[]))
				//	type = AcceptableFieldTypes.OBJECT_ARRAY; //Unsupported
				else if (fieldType == typeof(byte[]))
					type = AcceptableFieldTypes.BYTE_ARRAY;
				//else
				//	type = AcceptableFieldTypes.Unknown; //Unsupported

				return new ForgeClassFieldValue(name, value, type, interpolate, interpolateValue);
			}

			public static Type GetTypeFromAcceptable(AcceptableFieldTypes type)
			{
				switch (type)
				{
					case AcceptableFieldTypes.INT:
						return typeof(int);
					case AcceptableFieldTypes.UINT:
						return typeof(uint);
					case AcceptableFieldTypes.BOOL:
						return typeof(bool);
					case AcceptableFieldTypes.BYTE:
						return typeof(byte);
					case AcceptableFieldTypes.CHAR:
						return typeof(char);
					case AcceptableFieldTypes.DOUBLE:
						return typeof(double);
					case AcceptableFieldTypes.FLOAT:
						return typeof(float);
					case AcceptableFieldTypes.LONG:
						return typeof(long);
					case AcceptableFieldTypes.ULONG:
						return typeof(ulong);
					case AcceptableFieldTypes.SHORT:
						return typeof(short);
					case AcceptableFieldTypes.USHORT:
						return typeof(ushort);
					case AcceptableFieldTypes.COLOR:
						return typeof(Color);
					case AcceptableFieldTypes.QUATERNION:
						return typeof(Quaternion);
					case AcceptableFieldTypes.VECTOR2:
						return typeof(Vector2);
					case AcceptableFieldTypes.VECTOR3:
						return typeof(Vector3);
					case AcceptableFieldTypes.VECTOR4:
						return typeof(Vector4);
					//case AcceptableFieldTypes.STRING: //Unsupported
					//	return typeof(string);
					//case AcceptableFieldTypes.OBJECT_ARRAY: //Unsupported
					//	return typeof(object[]);
					case AcceptableFieldTypes.BYTE_ARRAY:
						return typeof(byte[]);
					default:
						return null;
				}
			}

			public static string GetInterpolateFromAcceptable(AcceptableFieldTypes type)
			{
				string returnValue = string.Empty;

				switch (type)
				{
					case AcceptableFieldTypes.FLOAT:
						returnValue = "InterpolateFloat";
						break;
					case AcceptableFieldTypes.VECTOR2:
						returnValue = "InterpolateVector2";
						break;
					case AcceptableFieldTypes.VECTOR3:
						returnValue = "InterpolateVector3";
						break;
					case AcceptableFieldTypes.VECTOR4:
						returnValue = "InterpolateVector4";
						break;
					case AcceptableFieldTypes.QUATERNION:
						returnValue = "InterpolateQuaternion";
						break;
				}

				return !string.IsNullOrEmpty(returnValue) ? returnValue : "InterpolateUnknown";
			}

			public static bool IsInterpolatable(AcceptableFieldTypes type)
			{
				bool returnValue = false;

				switch (type)
				{
					case AcceptableFieldTypes.FLOAT:
					case AcceptableFieldTypes.VECTOR2:
					case AcceptableFieldTypes.VECTOR3:
					case AcceptableFieldTypes.VECTOR4:
					case AcceptableFieldTypes.QUATERNION:
						returnValue = true;
						break;
				}

				return returnValue;
			}

			public static AcceptableFieldTypes GetTypeFromAcceptable(string val)
			{
				switch (val.Replace(" ", string.Empty).ToLower())
				{
					case "int":
						return AcceptableFieldTypes.INT;
					case "uint":
						return AcceptableFieldTypes.UINT;
					case "bool":
						return AcceptableFieldTypes.BOOL;
					case "byte":
						return AcceptableFieldTypes.BYTE;
					case "char":
						return AcceptableFieldTypes.CHAR;
					case "double":
						return AcceptableFieldTypes.DOUBLE;
					case "float":
						return AcceptableFieldTypes.FLOAT;
					case "long":
						return AcceptableFieldTypes.LONG;
					case "ulong":
						return AcceptableFieldTypes.ULONG;
					case "short":
						return AcceptableFieldTypes.SHORT;
					case "ushort":
						return AcceptableFieldTypes.USHORT;
					case "color":
						return AcceptableFieldTypes.COLOR;
					case "quaternion":
						return AcceptableFieldTypes.QUATERNION;
					case "vector2":
						return AcceptableFieldTypes.VECTOR2;
					case "vector3":
						return AcceptableFieldTypes.VECTOR3;
					case "vector4":
						return AcceptableFieldTypes.VECTOR4;
					//case "string":
					//	return AcceptableFieldTypes.STRING; //Unsupported
					//case "object[]":
					//	return AcceptableFieldTypes.OBJECT_ARRAY; //Unsupported
					case "byte[]":
						return AcceptableFieldTypes.BYTE_ARRAY;
					default:
						return AcceptableFieldTypes.BYTE;
						//return AcceptableFieldTypes.Unknown; //Unsupported
				}
			}

			public override string ToString()
			{
				return string.Format("[ Name: {0}, Value: {1}, Type: {2}, IsNetObj: {3}]", FieldName, FieldValue, FieldType, IsNetworkedObject);
			}
		}

		public class ForgeClassRPCValue
		{
			public string RPCName;
			public List<AcceptableRPCTypes> Arguments = new List<AcceptableRPCTypes>();
			public List<string> HelperTypes = new List<string>();

			public ForgeClassRPCValue(MethodInfo method, List<AcceptableRPCTypes> args = null, List<string> helperTypes = null)
			{
				RPCName = method.Name;
				ParameterInfo[] paramsInfo = method.GetParameters();
				if (args != null)
					Arguments = args;
				else
				{
					foreach (ParameterInfo info in paramsInfo)
						Arguments.Add(GetATypeFromPInfo(info));
				}

				if (helperTypes != null)
					HelperTypes = helperTypes;
				else
				{
					for (int i = 0; i < Arguments.Count; ++i)
						HelperTypes.Add(string.Empty);
				}
			}

			public static AcceptableRPCTypes GetATypeFromPInfo(ParameterInfo pInfo)
			{
				AcceptableRPCTypes type = AcceptableRPCTypes.STRING;
				Type fieldType = pInfo.ParameterType;
				if (fieldType == typeof(int))
					type = AcceptableRPCTypes.INT;
				else if (fieldType == typeof(uint))
					type = AcceptableRPCTypes.UINT;
				else if (fieldType == typeof(bool))
					type = AcceptableRPCTypes.BOOL;
				else if (fieldType == typeof(byte))
					type = AcceptableRPCTypes.BYTE;
				else if (fieldType == typeof(char))
					type = AcceptableRPCTypes.CHAR;
				else if (fieldType == typeof(double))
					type = AcceptableRPCTypes.DOUBLE;
				else if (fieldType == typeof(float))
					type = AcceptableRPCTypes.FLOAT;
				else if (fieldType == typeof(long))
					type = AcceptableRPCTypes.LONG;
				else if (fieldType == typeof(ulong))
					type = AcceptableRPCTypes.ULONG;
				else if (fieldType == typeof(short))
					type = AcceptableRPCTypes.SHORT;
				else if (fieldType == typeof(ushort))
					type = AcceptableRPCTypes.USHORT;
				else if (fieldType == typeof(Color))
					type = AcceptableRPCTypes.COLOR;
				else if (fieldType == typeof(Quaternion))
					type = AcceptableRPCTypes.QUATERNION;
				else if (fieldType == typeof(Vector2))
					type = AcceptableRPCTypes.VECTOR2;
				else if (fieldType == typeof(Vector3))
					type = AcceptableRPCTypes.VECTOR3;
				else if (fieldType == typeof(Vector4))
					type = AcceptableRPCTypes.VECTOR4;
				else if (fieldType == typeof(string))
					type = AcceptableRPCTypes.STRING;
				else if (fieldType == typeof(object[]))
					type = AcceptableRPCTypes.OBJECT_ARRAY;
				else if (fieldType == typeof(byte[]))
					type = AcceptableRPCTypes.BYTE_ARRAY;
				else
					type = AcceptableRPCTypes.Unknown;

				return type;
			}
		}

		public class ForgeClassRewindValue
		{
			public string RewindName;
			public AcceptableRPCTypes RewindType;
			public int RewindTime;

			public ForgeClassRewindValue(MethodInfo method, AcceptableRPCTypes type, int time)
			{
				RewindName = method.Name;
				RewindType = type;
				RewindTime = time;
			}

			public static AcceptableRPCTypes GetATypeFromPInfo(ParameterInfo pInfo)
			{
				AcceptableRPCTypes type = AcceptableRPCTypes.STRING;
				Type fieldType = pInfo.ParameterType;
				if (fieldType == typeof(int))
					type = AcceptableRPCTypes.INT;
				else if (fieldType == typeof(uint))
					type = AcceptableRPCTypes.UINT;
				else if (fieldType == typeof(bool))
					type = AcceptableRPCTypes.BOOL;
				else if (fieldType == typeof(byte))
					type = AcceptableRPCTypes.BYTE;
				else if (fieldType == typeof(char))
					type = AcceptableRPCTypes.CHAR;
				else if (fieldType == typeof(double))
					type = AcceptableRPCTypes.DOUBLE;
				else if (fieldType == typeof(float))
					type = AcceptableRPCTypes.FLOAT;
				else if (fieldType == typeof(long))
					type = AcceptableRPCTypes.LONG;
				else if (fieldType == typeof(ulong))
					type = AcceptableRPCTypes.ULONG;
				else if (fieldType == typeof(short))
					type = AcceptableRPCTypes.SHORT;
				else if (fieldType == typeof(ushort))
					type = AcceptableRPCTypes.USHORT;
				else if (fieldType == typeof(Color))
					type = AcceptableRPCTypes.COLOR;
				else if (fieldType == typeof(Quaternion))
					type = AcceptableRPCTypes.QUATERNION;
				else if (fieldType == typeof(Vector2))
					type = AcceptableRPCTypes.VECTOR2;
				else if (fieldType == typeof(Vector3))
					type = AcceptableRPCTypes.VECTOR3;
				else if (fieldType == typeof(Vector4))
					type = AcceptableRPCTypes.VECTOR4;
				else if (fieldType == typeof(string))
					type = AcceptableRPCTypes.STRING;
				else if (fieldType == typeof(object[]))
					type = AcceptableRPCTypes.OBJECT_ARRAY;
				else if (fieldType == typeof(byte[]))
					type = AcceptableRPCTypes.BYTE_ARRAY;
				else
					type = AcceptableRPCTypes.Unknown;

				return type;
			}
		}

		public class ForgeClassIdentity
		{
			public string IdentityName;
			public int IdentityID;

			public ForgeClassIdentity()
			{
				IdentityName = string.Empty;
				IdentityID = -1;
			}

			public ForgeClassIdentity(string name, int id)
			{
				this.IdentityName = name;
				this.IdentityID = id;
			}
		}

		public class FNRPCTypes
		{
			public string HelperName;
			public AcceptableRPCTypes Type;
		}

		public class ForgeEditorRPCField
		{
			public string FieldName;
			public bool CanRender = true;
			public List<FNRPCTypes> FieldTypes;
			public int ArgumentCount { get { return FieldTypes.Count; } }
			public bool DELETED;
			public bool Dropdown;

			public ForgeEditorRPCField()
			{
				FieldName = "";
				FieldTypes = new List<FNRPCTypes>();
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
				GUILayout.Label(TrashIcon, GUILayout.Height(15));
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
						FieldTypes[i].Type = (AcceptableRPCTypes)EditorGUILayout.EnumPopup(FieldTypes[i].Type);
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

						if (_proVersion)
							GUI.color = Color.white;
						else
							GUI.color = Color.black;

						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.Space();
						GUILayout.FlexibleSpace();
						GUILayout.Label(SubtractIcon, GUILayout.Height(12));
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
						FieldTypes.Add(new FNRPCTypes() { Type = AcceptableRPCTypes.BYTE });
					}
					if (_proVersion)
						GUI.color = Color.white;
					else
						GUI.color = Color.black;
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space();
					GUILayout.FlexibleSpace();
					GUILayout.Label(AddIcon, GUILayout.Height(12));
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
					GUI.color = Color.white;
				}
			}

			public void AddRange(AcceptableRPCTypes[] types, string[] helperNames)
			{
				for (int i = 0; i < types.Length; ++i)
				{
					FieldTypes.Add(new FNRPCTypes() { Type = types[i], HelperName = helperNames[i] });
				}
			}
		}

		public class ForgeEditorField
		{
			public string FieldName;
			public bool CanRender = true;
			public bool Interpolate;
			public float InterpolateValue;
			public AcceptableFieldTypes FieldType;
			public bool DELETED;

			public ForgeEditorField(string name = "", bool canRender = true, AcceptableFieldTypes type = AcceptableFieldTypes.BYTE, bool interpolate = false, float interpolateValue = 0f)
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
				FieldType = (AcceptableFieldTypes)EditorGUILayout.EnumPopup(FieldType, GUILayout.Width(75));
				//if (FieldType == AcceptableFieldTypes.Unknown) //Unsupported
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
						InterpolateValue = DEFAULT_INTERPOLATE_TIME;
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
				GUILayout.Label(TrashIcon, GUILayout.Height(15));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();

				GUILayout.EndHorizontal();
			}
		}

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

			public bool CanRenderFields = true;
			public bool CanRenderRPCS = true;
			public Color ButtonColor;
			public Action InvokedAction;
			public List<ForgeEditorField> ClassVariables = new List<ForgeEditorField>();
			private int _defaultClassVariablesCount;
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
				ButtonColor = CoolBlue;
			}

			public ForgeEditorButton(ForgeClassObject fcObj)
			{
				TiedObject = fcObj;
				Setup();

				if (fcObj.IsNetworkBehavior)
					ButtonColor = DarkBlue;
				else if (fcObj.IsNetworkObject)
					ButtonColor = LightBlue;
				else
					ButtonColor = CoolBlue;

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
				if (_proVersion)
					GUI.color = ButtonColor;
				if (GUI.Button(verticleButton, GUIContent.none))
				{
					if (InvokedAction != null)
						InvokedAction();

					ForgeNetworkingEditor.Instance.ActiveButton = this;
					ForgeNetworkingEditor.Instance.ChangeMenu(ForgeEditorActiveMenu.Modify);
				}
				EditorGUILayout.BeginHorizontal();
				if (_proVersion)
					GUI.color = Color.white;
				else
					GUI.color = Color.black;

				GUILayout.Label(SideArrow);
				GUILayout.FlexibleSpace();
				GUI.color = Color.white;
				GUIStyle boldStyle = new GUIStyle(GUI.skin.GetStyle("boldLabel"));
				boldStyle.alignment = TextAnchor.UpperCenter;
				GUILayout.Label(ButtonName, boldStyle);
				GUILayout.FlexibleSpace();
				if (_proVersion)
					GUI.color = Color.white;
				else
					GUI.color = Color.black;
				GUILayout.Label(SideArrowInverse);
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
				GUILayout.Label(TrashIcon, GUILayout.Height(20));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();

				EditorGUILayout.EndHorizontal();
			}

			public bool RenderExposed(Action callback = null, bool ignoreButton = false)
			{
				bool returnValue = false;
				if (TiedObject == null)
				{
					EditorStyles.boldLabel.alignment = TextAnchor.MiddleLeft;
					GUILayout.Label("Name", EditorStyles.boldLabel);
					EditorStyles.boldLabel.alignment = TextAnchor.MiddleCenter;
					ButtonName = GUILayout.TextField(ButtonName, GUILayout.Width(ForgeNetworkingEditor.Instance.position.width - 50));
					returnValue = true;
				}
				else if (!ignoreButton)
				{
					Rect verticleButton = EditorGUILayout.BeginVertical("Button");
					if (_proVersion)
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
					if (_proVersion)
						GUI.color = Color.white;
					else
						GUI.color = Color.black;
					GUILayout.Label(SideArrow);
					GUI.color = Color.white;
					GUILayout.FlexibleSpace();
					GUIStyle boldStyle = new GUIStyle(GUI.skin.GetStyle("boldLabel"));
					boldStyle.alignment = TextAnchor.UpperCenter;
					GUILayout.Label(ButtonName, boldStyle);
					GUILayout.FlexibleSpace();
					if (_proVersion)
						GUI.color = Color.white;
					else
						GUI.color = Color.black;
					GUILayout.Label(SideArrowInverse);
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

					for (int i = 0; i < ClassVariables.Count; ++i)
					{
						if (ClassVariables[i].DELETED)
						{
							ClassVariables.RemoveAt(i);
							i--;
							continue;
						}
						ClassVariables[i].Render();
					}

					Rect addFieldBtn = EditorGUILayout.BeginVertical("Button", GUILayout.Width(75), GUILayout.Height(25));
					GUI.color = Color.green;
					if (GUI.Button(addFieldBtn, GUIContent.none))
					{
						ClassVariables.Add(new ForgeEditorField());
					}

					EditorGUILayout.BeginHorizontal();
					GUI.color = Color.white;
					GUILayout.FlexibleSpace();
					GUIStyle boldStyle = new GUIStyle(GUI.skin.GetStyle("boldLabel"));
					boldStyle.alignment = TextAnchor.UpperCenter;
					GUILayout.Label("Add Field", boldStyle);
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
				}

				if (CanRenderRPCS)
				{
					EditorGUILayout.Space();
					EditorStyles.boldLabel.alignment = TextAnchor.MiddleLeft;
					GUILayout.Label("Remote Procedure Calls", EditorStyles.boldLabel);
					EditorStyles.boldLabel.alignment = TextAnchor.MiddleCenter;

					for (int i = 0; i < RPCVariables.Count; ++i)
					{
						if (RPCVariables[i].DELETED)
						{
							RPCVariables.RemoveAt(i);
							i--;
							continue;
						}
						RPCVariables[i].Render();
					}

					Rect addRpcBtn = EditorGUILayout.BeginVertical("Button", GUILayout.Width(75), GUILayout.Height(25));
					GUI.color = Color.green;
					if (GUI.Button(addRpcBtn, GUIContent.none))
					{
						RPCVariables.Add(new ForgeEditorRPCField());
					}

					EditorGUILayout.BeginHorizontal();
					GUI.color = Color.white;
					GUILayout.FlexibleSpace();
					GUIStyle boldStyle = new GUIStyle(GUI.skin.GetStyle("boldLabel"));
					boldStyle.alignment = TextAnchor.UpperCenter;
					GUILayout.Label("Add RPC", boldStyle);
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
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

			public bool IsSetupCorrectly()
			{
				bool returnValue = true;

				List<string> variableNames = new List<string>();

				//Make sure it doesn't match the class name
				if (IsValidName(ButtonName))
					variableNames.Add(ButtonName);
				else
					returnValue = false;

				string checkedName = string.Empty;
				//Check the fields
				if (returnValue)
				{
					for (int i = 0; i < ClassVariables.Count; ++i)
					{
						checkedName = ClassVariables[i].FieldName;
						if (variableNames.Contains(checkedName))
						{
							returnValue = false;
							break;
						}

						if (IsValidName(checkedName))
							variableNames.Add(checkedName);
						else
						{
							Debug.LogError("Invalid Character Found");
							returnValue = false;
							break;
						}
					}
				}

				//Check the rpcs
				if (returnValue)
				{
					for (int i = 0; i < RPCVariables.Count; ++i)
					{
						checkedName = RPCVariables[i].FieldName;
						if (variableNames.Contains(checkedName))
						{
							returnValue = false;
							break;
						}

						if (IsValidName(checkedName))
							variableNames.Add(checkedName);
						else
						{
							Debug.LogError("Invalid Character Found");
							returnValue = false;
							break;
						}
					}
				}

				//Check the rewinds
				if (returnValue)
				{
					for (int i = 0; i < RewindVariables.Count; ++i)
					{
						checkedName = RewindVariables[i].FieldName;
						if (variableNames.Contains(checkedName))
						{
							returnValue = false;
							break;
						}

						if (IsValidName(checkedName))
							variableNames.Add(checkedName);
						else
						{
							Debug.LogError("Invalid Character Found");
							returnValue = false;
							break;
						}
					}
				}

				return returnValue;
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
				_defaultClassVariablesCount = ClassVariables.Count;

				//TODO: RewindVariables here

				for (int i = 0; i < TiedObject.RPCS.Count; ++i)
				{
					ForgeEditorRPCField rpc = new ForgeEditorRPCField();

					rpc.FieldName = TiedObject.RPCS[i].RPCName;
					rpc.AddRange(TiedObject.RPCS[i].Arguments.ToArray(), TiedObject.RPCS[i].HelperTypes.ToArray());
					if (TiedObject.RPCS[i].RPCName.ToLower().Equals("initialize"))
						rpc.CanRender = false;

					RPCVariables.Add(rpc);
				}
				_defaultRPCVariablesCount = RPCVariables.Count;
			}
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

		public void Initialize()
		{
			titleContent = new GUIContent("Forge Wizard");
			_proVersion = EditorGUIUtility.isProSkin;

			IDENTITIES = 0;
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

			string[] files = Directory.GetFiles(_storingPath, "*", SearchOption.TopDirectoryOnly);
			string[] userFiles = Directory.GetFiles(_userStoringPath, "*", SearchOption.TopDirectoryOnly);
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
			//Close();
			//_instance = null;
		}
		#endregion

		public void ChangeMenu(ForgeEditorActiveMenu nextMenu)
		{
			_currentMenu = nextMenu;
		}

		private void OnGUI()
		{
			if (_proVersion)
				GUI.DrawTexture(new Rect(0, 0, position.width, position.height), BackgroundTexture);

			EditorGUILayout.BeginHorizontal();
			if (_proVersion)
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
			GUILayout.Label("Forge Remastered Version: 1.3.0b", EditorStyles.boldLabel);
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
		private void RenderMainMenu()
		{
			if (_editorButtons == null)
			{
				Initialize();
				return; //Editor is getting refreshed
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("Search", GUILayout.Width(50));
			_searchField = GUILayout.TextField(_searchField);
			Rect verticleButton = EditorGUILayout.BeginVertical("Button", GUILayout.Width(100), GUILayout.Height(15));
			if (_proVersion)
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
			if (_proVersion)
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

			bool canBack = ActiveButton.RenderExposed(_createUndo);

			GUILayout.EndScrollView();

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
			if (_proVersion)
				GUI.color = TealBlue;
			if (GUI.Button(verticleButton, GUIContent.none))
			{
				if (ActiveButton.IsSetupCorrectly())
				{
					_editorButtons.Add(ActiveButton);
					Compile();
					ChangeMenu(ForgeEditorActiveMenu.Main);
				}
				else
					Debug.LogError("Duplicate variable/rpc names found, please correct before compiling");
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

		private void RenderModifyWindow()
		{
			if (_editorButtons == null || ActiveButton == null)
			{
				Initialize();
				return; //Editor is getting refreshed
			}

			EditorGUILayout.Space();
			_scrollView = GUILayout.BeginScrollView(_scrollView);

			ActiveButton.RenderExposed(_modifyUndo);

			GUILayout.EndScrollView();

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
			if (_proVersion)
				GUI.color = TealBlue;
			if (GUI.Button(verticleButton, GUIContent.none))
			{
				if (ActiveButton.IsSetupCorrectly())
				{
					Compile();
					ChangeMenu(ForgeEditorActiveMenu.Main);
				}
				else
					Debug.LogError("Duplicate variable/rpc names found, please correct before compiling");
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
				interpolateType = ForgeClassFieldValue.GetInterpolateFromAcceptable(btn.ClassVariables[i].FieldType);

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
					interpolateValues += btn.ClassVariables[i].InterpolateValue.ToString() + ",";
				else
					interpolateValues += btn.ClassVariables[i].InterpolateValue.ToString();

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

		public string SourceCodeNetworkBehavior(ForgeClassObject cObj, ForgeEditorButton btn)
		{
			TextAsset asset = Resources.Load<TextAsset>(EDITOR_RESOURCES_DIR + "/NetworkBehaviorTemplate");
			TemplateSystem template = new TemplateSystem(asset.text);

			template.AddVariable("className", btn.StrippedSearchName + "Behavior");
			template.AddVariable("networkObject", btn.StrippedSearchName + "NetworkObject");
			StringBuilder generatedJSON = new StringBuilder();
			StringBuilder generatedHelperTypesJSON = new StringBuilder();

			List<object[]> rpcs = new List<object[]>();

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

				rpcs.Add(rpcData);
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
			return template.Parse();
		}

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

		public string SourceCodeNetworkManager()
		{
			TextAsset asset = Resources.Load<TextAsset>(EDITOR_RESOURCES_DIR + "/NetworkManagerTemplate");
			TemplateSystem template = new TemplateSystem(asset.text);

			List<object> networkObjects = new List<object>();
			for (int i = 0; i < _editorButtons.Count; ++i)
			{
				if (!_editorButtons[i].IsNetworkObject)
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

							using (StreamWriter sw = File.CreateText(_editorButtons[i].TiedObject.FileLocation))
							{
								sw.Write(networkBehaviorData);
							}
						}
						else if (_editorButtons[i].TiedObject.IsNetworkObject)
						{
							string networkObjectData = SourceCodeNetworkObject(null, _editorButtons[i], identity);
							using (StreamWriter sw = File.CreateText(_editorButtons[i].TiedObject.FileLocation))
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

			EditorApplication.UnlockReloadAssemblies();

			AssetDatabase.Refresh();

			_editorButtons.Remove(ActiveButton);
			ActiveButton = null;
			CloseFinal();
		}

		#endregion

		#region Checks

		public static bool IsValidName(string expression)
		{
			return (System.Text.RegularExpressions.Regex.IsMatch(expression, REGEX_MATCH, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
		}

		#endregion
	}
}