//#define FORGE_EDITOR_DEBUGGING

using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Templating;
using SimpleJSONEditor;
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
	/// <summary>
	/// This is a class object that contains the rpc value
	/// </summary>
	public class ForgeClassFieldRPCValue
	{
		public string FieldRPCName;
		public object FieldRPCValue;
		public bool Interpolate;
		public float InterpolateValue;
		public ForgeAcceptableRPCTypes FieldType;
		public bool IsNetworkedObject { get { return FieldRPCName.ToLower() == "networkobject"; } }

		public ForgeClassFieldRPCValue()
		{
			FieldRPCName = string.Empty;
			FieldRPCValue = null;
			Interpolate = false;
			InterpolateValue = 0;
			FieldType = ForgeAcceptableRPCTypes.BYTE;
		}

		public ForgeClassFieldRPCValue(string name, object value, ForgeAcceptableRPCTypes type, bool interpolate, float interpolateValue)
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

			ForgeAcceptableRPCTypes type = ForgeAcceptableRPCTypes.BYTE;
			Type fieldType = field.FieldType;
			if (fieldType == typeof(int))
				type = ForgeAcceptableRPCTypes.INT;
			else if (fieldType == typeof(uint))
				type = ForgeAcceptableRPCTypes.UINT;
			else if (fieldType == typeof(bool))
				type = ForgeAcceptableRPCTypes.BOOL;
			else if (fieldType == typeof(byte))
				type = ForgeAcceptableRPCTypes.BYTE;
			else if (fieldType == typeof(char))
				type = ForgeAcceptableRPCTypes.CHAR;
			else if (fieldType == typeof(double))
				type = ForgeAcceptableRPCTypes.DOUBLE;
			else if (fieldType == typeof(float))
				type = ForgeAcceptableRPCTypes.FLOAT;
			else if (fieldType == typeof(long))
				type = ForgeAcceptableRPCTypes.LONG;
			else if (fieldType == typeof(ulong))
				type = ForgeAcceptableRPCTypes.ULONG;
			else if (fieldType == typeof(short))
				type = ForgeAcceptableRPCTypes.SHORT;
			else if (fieldType == typeof(ushort))
				type = ForgeAcceptableRPCTypes.USHORT;
			else if (fieldType == typeof(Color))
				type = ForgeAcceptableRPCTypes.COLOR;
			else if (fieldType == typeof(Quaternion))
				type = ForgeAcceptableRPCTypes.QUATERNION;
			else if (fieldType == typeof(Vector2))
				type = ForgeAcceptableRPCTypes.VECTOR2;
			else if (fieldType == typeof(Vector3))
				type = ForgeAcceptableRPCTypes.VECTOR3;
			else if (fieldType == typeof(Vector4))
				type = ForgeAcceptableRPCTypes.VECTOR4;
			else if (fieldType == typeof(string))
				type = ForgeAcceptableRPCTypes.STRING;
			//else if (fieldType == typeof(object[]))
			//	type = ForgeAcceptableRPCTypes.OBJECT_ARRAY;
			else if (fieldType == typeof(byte[]))
				type = ForgeAcceptableRPCTypes.BYTE_ARRAY;
			else
				type = ForgeAcceptableRPCTypes.Unknown;

			return new ForgeClassFieldRPCValue(name, value, type, interpolate, interpolateValue);
		}

		public static Type GetTypeFromAcceptable(ForgeAcceptableRPCTypes type)
		{
			switch (type)
			{
				case ForgeAcceptableRPCTypes.INT:
					return typeof(int);
				case ForgeAcceptableRPCTypes.UINT:
					return typeof(uint);
				case ForgeAcceptableRPCTypes.BOOL:
					return typeof(bool);
				case ForgeAcceptableRPCTypes.BYTE:
					return typeof(byte);
				case ForgeAcceptableRPCTypes.CHAR:
					return typeof(char);
				case ForgeAcceptableRPCTypes.DOUBLE:
					return typeof(double);
				case ForgeAcceptableRPCTypes.FLOAT:
					return typeof(float);
				case ForgeAcceptableRPCTypes.LONG:
					return typeof(long);
				case ForgeAcceptableRPCTypes.ULONG:
					return typeof(ulong);
				case ForgeAcceptableRPCTypes.SHORT:
					return typeof(short);
				case ForgeAcceptableRPCTypes.USHORT:
					return typeof(ushort);
				case ForgeAcceptableRPCTypes.COLOR:
					return typeof(Color);
				case ForgeAcceptableRPCTypes.QUATERNION:
					return typeof(Quaternion);
				case ForgeAcceptableRPCTypes.VECTOR2:
					return typeof(Vector2);
				case ForgeAcceptableRPCTypes.VECTOR3:
					return typeof(Vector3);
				case ForgeAcceptableRPCTypes.VECTOR4:
					return typeof(Vector4);
				case ForgeAcceptableRPCTypes.STRING:
					return typeof(string);
				//case ForgeAcceptableRPCTypes.OBJECT_ARRAY:
				//	return typeof(object[]);
				case ForgeAcceptableRPCTypes.BYTE_ARRAY:
					return typeof(byte[]);
				default:
					return null;
			}
		}

		public static ForgeAcceptableRPCTypes GetTypeFromAcceptable(string val)
		{
			switch (val.Replace(" ", string.Empty).ToLower())
			{
				case "int":
					return ForgeAcceptableRPCTypes.INT;
				case "uint":
					return ForgeAcceptableRPCTypes.UINT;
				case "bool":
					return ForgeAcceptableRPCTypes.BOOL;
				case "byte":
					return ForgeAcceptableRPCTypes.BYTE;
				case "char":
					return ForgeAcceptableRPCTypes.CHAR;
				case "double":
					return ForgeAcceptableRPCTypes.DOUBLE;
				case "float":
					return ForgeAcceptableRPCTypes.FLOAT;
				case "long":
					return ForgeAcceptableRPCTypes.LONG;
				case "ulong":
					return ForgeAcceptableRPCTypes.ULONG;
				case "short":
					return ForgeAcceptableRPCTypes.SHORT;
				case "ushort":
					return ForgeAcceptableRPCTypes.USHORT;
				case "color":
					return ForgeAcceptableRPCTypes.COLOR;
				case "quaternion":
					return ForgeAcceptableRPCTypes.QUATERNION;
				case "vector2":
					return ForgeAcceptableRPCTypes.VECTOR2;
				case "vector3":
					return ForgeAcceptableRPCTypes.VECTOR3;
				case "vector4":
					return ForgeAcceptableRPCTypes.VECTOR4;
				case "string":
					return ForgeAcceptableRPCTypes.STRING;
				//case "object[]":
				//	return ForgeAcceptableRPCTypes.OBJECT_ARRAY;
				case "byte[]":
					return ForgeAcceptableRPCTypes.BYTE_ARRAY;
				default:
					return ForgeAcceptableRPCTypes.Unknown;
			}
		}

		public override string ToString()
		{
			return string.Format("[ Name: {0}, Value: {1}, Type: {2}, IsNetObj: {3}]", FieldRPCName, FieldRPCValue, FieldType, IsNetworkedObject);
		}
	}
}