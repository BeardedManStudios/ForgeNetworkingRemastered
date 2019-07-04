//#define FORGE_EDITOR_DEBUGGING

using BeardedManStudios.Forge.Networking.Generated;
using SimpleJSONEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using BeardedManStudios.Forge.Networking.UnityEditor.Serializer;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	/// <summary>
	/// This is the forge class object we generate from a file
	/// </summary>
	[Serializable]
	public class ForgeClassObject
	{
		public string FileLocation;
		public string Filename;
		public string ExactFilename;

		public int IdentityValue = -1;

		[NonSerialized]
		public static int IDENTITIES = 0;

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

			MethodInfo[] methods = currentType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.FullName == "BeardedManStudios.Forge.Networking.RpcArgs").ToArray();
			PropertyInfo[] properties = currentType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			FieldInfo[] fields = currentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(attr => attr.IsDefined(typeof(ForgeGeneratedFieldAttribute), false)).ToArray();

			uniqueMethods.AddRange(methods);
			uniqueProperties.AddRange(properties);
			uniqueFields.AddRange(fields);

			//If we don't find any fields containing the attribute, the class is either empty, or using the old format. Try parsing old format
			if (fields.Length == 0)
			{
				FieldInfo[] legacyFields = currentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				uniqueFields.AddRange(legacyFields);

				for (int i = 0; i < uniqueFields.Count; ++i)
				{
					switch (uniqueFields[i].Name)
					{
						case "IDENTITY":
						case "networkObject":
						case "fieldAltered":
						case "_dirtyFields":
						case "dirtyFields":
							uniqueFields.RemoveAt(i--);
							//TODO: Store the types for re-use
							continue;
					}

					if (uniqueFields[i].Name.EndsWith("Changed"))
					{
						uniqueFields.RemoveAt(i--);
						continue;
					}

					if (uniqueFields[i].Name.EndsWith("Interpolation"))
					{
						uniqueFields.RemoveAt(i--);

						//TODO: Store the types for re-use
						continue;
					}
				}
			}

			if (currentType.BaseType != null)
			{
				Type baseType = currentType.BaseType;
				Type networkBehavior = currentType.GetInterface("INetworkBehavior");
				Type factoryInterface = currentType.GetInterface("INetworkObjectFactory");
				bool isMonobehavior = currentType.IsSubclassOf(typeof(MonoBehaviour));

				if (baseType.FullName == "BeardedManStudios.Forge.Networking.NetworkObject")
				{
					ObjectClassType = ForgeBaseClassType.NetworkObject;
					IdentityValue = ++IDENTITIES;
				}
				else if (networkBehavior != null && !isMonobehavior)
					ObjectClassType = ForgeBaseClassType.NetworkBehavior;
				else if (baseType == typeof(MonoBehaviour) || isMonobehavior)
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
						_interpolationValues.Add(ForgeNetworkingEditor.DEFAULT_INTERPOLATE_TIME);
				}

				for (int i = 0; i < uniqueFields.Count; ++i)
				{
					if (_interpolationValues.Count == 0)
						break;

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
			List<List<ForgeAcceptableRPCTypes>> rpcSupportedTypes = new List<List<ForgeAcceptableRPCTypes>>();
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
							List<ForgeAcceptableRPCTypes> singularSupportedTypes = new List<ForgeAcceptableRPCTypes>();
							for (int x = 0; x < singularArray.Count; ++x)
							{
								ForgeAcceptableRPCTypes singularType = ForgeClassFieldRPCValue.GetTypeFromAcceptable(singularArray[x].Value);
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
					rpcSupportedTypes.Add(new List<ForgeAcceptableRPCTypes>());
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
}