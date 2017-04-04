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
	/// This is the rewind value for this class object
	/// </summary>
	[Serializable]
	public class ForgeClassRewindValue
	{
		public string RewindName;
		public ForgeAcceptableRPCTypes RewindType;
		public int RewindTime;

		public ForgeClassRewindValue(MethodInfo method, ForgeAcceptableRPCTypes type, int time)
		{
			RewindName = method.Name;
			RewindType = type;
			RewindTime = time;
		}

		public static ForgeAcceptableRPCTypes GetATypeFromPInfo(ParameterInfo pInfo)
		{
			ForgeAcceptableRPCTypes type = ForgeAcceptableRPCTypes.STRING;
			Type fieldType = pInfo.ParameterType;
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

			return type;
		}
	}
}