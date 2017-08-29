//#define FORGE_EDITOR_DEBUGGING

using System;
using System.Reflection;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	/// <summary>
	/// This is a class object that contains the field value
	/// </summary>
	[Serializable]
	public class ForgeClassFieldValue
	{
		public string FieldName;
		public object FieldValue;
		public bool Interpolate;
		public float InterpolateValue;
		public ForgeAcceptableFieldTypes FieldType;
		public bool IsNetworkedObject { get { return FieldName.ToLower() == "networkobject"; } }

		public ForgeClassFieldValue()
		{
			FieldName = string.Empty;
			FieldValue = null;
			Interpolate = false;
			InterpolateValue = 0;
			FieldType = ForgeAcceptableFieldTypes.BYTE;
		}

		public ForgeClassFieldValue(string name, object value, ForgeAcceptableFieldTypes type, bool interpolate, float interpolateValue)
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

			ForgeAcceptableFieldTypes type = ForgeAcceptableFieldTypes.BYTE;
			Type fieldType = field.FieldType;
			if (fieldType == typeof(int))
				type = ForgeAcceptableFieldTypes.INT;
			else if (fieldType == typeof(uint))
				type = ForgeAcceptableFieldTypes.UINT;
			else if (fieldType == typeof(bool))
				type = ForgeAcceptableFieldTypes.BOOL;
			else if (fieldType == typeof(byte))
				type = ForgeAcceptableFieldTypes.BYTE;
			else if (fieldType == typeof(char))
				type = ForgeAcceptableFieldTypes.CHAR;
			else if (fieldType == typeof(double))
				type = ForgeAcceptableFieldTypes.DOUBLE;
			else if (fieldType == typeof(float))
				type = ForgeAcceptableFieldTypes.FLOAT;
			else if (fieldType == typeof(long))
				type = ForgeAcceptableFieldTypes.LONG;
			else if (fieldType == typeof(ulong))
				type = ForgeAcceptableFieldTypes.ULONG;
			else if (fieldType == typeof(short))
				type = ForgeAcceptableFieldTypes.SHORT;
			else if (fieldType == typeof(ushort))
				type = ForgeAcceptableFieldTypes.USHORT;
			else if (fieldType == typeof(Color))
				type = ForgeAcceptableFieldTypes.COLOR;
			else if (fieldType == typeof(Quaternion))
				type = ForgeAcceptableFieldTypes.QUATERNION;
			else if (fieldType == typeof(Vector2))
				type = ForgeAcceptableFieldTypes.VECTOR2;
			else if (fieldType == typeof(Vector3))
				type = ForgeAcceptableFieldTypes.VECTOR3;
			else if (fieldType == typeof(Vector4))
				type = ForgeAcceptableFieldTypes.VECTOR4;
			//else if (fieldType == typeof(string))
			//	type = ForgeAcceptableFieldTypes.STRING; //Unsupported
			//else if (fieldType == typeof(object[]))
			//	type = ForgeAcceptableFieldTypes.OBJECT_ARRAY; //Unsupported
			//else if (fieldType == typeof(byte[]))
			//	type = ForgeAcceptableFieldTypes.BYTE_ARRAY;
			//else
			//	type = ForgeAcceptableFieldTypes.Unknown; //Unsupported

			return new ForgeClassFieldValue(name, value, type, interpolate, interpolateValue);
		}

		public static Type GetTypeFromAcceptable(ForgeAcceptableFieldTypes type)
		{
			switch (type)
			{
				case ForgeAcceptableFieldTypes.INT:
					return typeof(int);
				case ForgeAcceptableFieldTypes.UINT:
					return typeof(uint);
				case ForgeAcceptableFieldTypes.BOOL:
					return typeof(bool);
				case ForgeAcceptableFieldTypes.BYTE:
					return typeof(byte);
				case ForgeAcceptableFieldTypes.CHAR:
					return typeof(char);
				case ForgeAcceptableFieldTypes.DOUBLE:
					return typeof(double);
				case ForgeAcceptableFieldTypes.FLOAT:
					return typeof(float);
				case ForgeAcceptableFieldTypes.LONG:
					return typeof(long);
				case ForgeAcceptableFieldTypes.ULONG:
					return typeof(ulong);
				case ForgeAcceptableFieldTypes.SHORT:
					return typeof(short);
				case ForgeAcceptableFieldTypes.USHORT:
					return typeof(ushort);
				case ForgeAcceptableFieldTypes.COLOR:
					return typeof(Color);
				case ForgeAcceptableFieldTypes.QUATERNION:
					return typeof(Quaternion);
				case ForgeAcceptableFieldTypes.VECTOR2:
					return typeof(Vector2);
				case ForgeAcceptableFieldTypes.VECTOR3:
					return typeof(Vector3);
				case ForgeAcceptableFieldTypes.VECTOR4:
					return typeof(Vector4);
				//case ForgeAcceptableFieldTypes.STRING: //Unsupported
				//	return typeof(string);
				//case ForgeAcceptableFieldTypes.OBJECT_ARRAY: //Unsupported
				//	return typeof(object[]);
				//case ForgeAcceptableFieldTypes.BYTE_ARRAY:
				//	return typeof(byte[]);
				default:
					return null;
			}
		}

		public static string GetInterpolateFromAcceptable(string baseTypeString, ForgeAcceptableFieldTypes type)
		{
			string returnValue = string.Empty;

			switch (type)
			{
				case ForgeAcceptableFieldTypes.FLOAT:
					returnValue = "InterpolateFloat";
					break;
				case ForgeAcceptableFieldTypes.VECTOR2:
					returnValue = "InterpolateVector2";
					break;
				case ForgeAcceptableFieldTypes.VECTOR3:
					returnValue = "InterpolateVector3";
					break;
				case ForgeAcceptableFieldTypes.VECTOR4:
					returnValue = "InterpolateVector4";
					break;
				case ForgeAcceptableFieldTypes.QUATERNION:
					returnValue = "InterpolateQuaternion";
					break;
				default:
					returnValue = "Interpolated<" + baseTypeString + ">";
					break;
			}

			return !string.IsNullOrEmpty(returnValue) ? returnValue : "Interpolated<object>";
		}

		public static bool IsInterpolatable(ForgeAcceptableFieldTypes type)
		{
			bool returnValue = false;

			switch (type)
			{
				case ForgeAcceptableFieldTypes.FLOAT:
				case ForgeAcceptableFieldTypes.VECTOR2:
				case ForgeAcceptableFieldTypes.VECTOR3:
				case ForgeAcceptableFieldTypes.VECTOR4:
				case ForgeAcceptableFieldTypes.QUATERNION:
					returnValue = true;
					break;
			}

			return returnValue;
		}

		public static ForgeAcceptableFieldTypes GetTypeFromAcceptable(string val)
		{
			switch (val.Replace(" ", string.Empty).ToLower())
			{
				case "int":
					return ForgeAcceptableFieldTypes.INT;
				case "uint":
					return ForgeAcceptableFieldTypes.UINT;
				case "bool":
					return ForgeAcceptableFieldTypes.BOOL;
				case "byte":
					return ForgeAcceptableFieldTypes.BYTE;
				case "char":
					return ForgeAcceptableFieldTypes.CHAR;
				case "double":
					return ForgeAcceptableFieldTypes.DOUBLE;
				case "float":
					return ForgeAcceptableFieldTypes.FLOAT;
				case "long":
					return ForgeAcceptableFieldTypes.LONG;
				case "ulong":
					return ForgeAcceptableFieldTypes.ULONG;
				case "short":
					return ForgeAcceptableFieldTypes.SHORT;
				case "ushort":
					return ForgeAcceptableFieldTypes.USHORT;
				case "color":
					return ForgeAcceptableFieldTypes.COLOR;
				case "quaternion":
					return ForgeAcceptableFieldTypes.QUATERNION;
				case "vector2":
					return ForgeAcceptableFieldTypes.VECTOR2;
				case "vector3":
					return ForgeAcceptableFieldTypes.VECTOR3;
				case "vector4":
					return ForgeAcceptableFieldTypes.VECTOR4;
				//case "string":
				//	return ForgeAcceptableFieldTypes.STRING; //Unsupported
				//case "object[]":
				//	return ForgeAcceptableFieldTypes.OBJECT_ARRAY; //Unsupported
				//case "byte[]":
				//	return ForgeAcceptableFieldTypes.BYTE_ARRAY;
				default:
					return ForgeAcceptableFieldTypes.BYTE;
					//return ForgeAcceptableFieldTypes.Unknown; //Unsupported
			}
		}

		public override string ToString()
		{
			return string.Format("[ Name: {0}, Value: {1}, Type: {2}, IsNetObj: {3}]", FieldName, FieldValue, FieldType, IsNetworkedObject);
		}
	}
}