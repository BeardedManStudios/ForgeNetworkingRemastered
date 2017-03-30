using System;
using System.Reflection;
using SimpleJSONEditor;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.UnityEditor.Serializer
{
	[Obsolete("Don't use this because Brent doesn't like Brett coding")]
	public static class FNSerializer
	{
		/// <summary>
		/// Serialize the data back to an object
		/// </summary>
		/// <typeparam name="T">The type of object</typeparam>
		/// <param name="data">The data this object holds</param>
		/// <returns>The type of object serialized back</returns>
		public static T FromJSON<T>(JSONClass data) where T : IFNSerializable
		{
			T returnValue = Activator.CreateInstance<T>(); //Create a brand new instance of this class to return with
			Type t = typeof(T);
			FieldInfo[] fields = t.GetFields();
			for (int i = 0; i < fields.Length; ++i)
			{
				object[] attributesCheck = fields[i].GetCustomAttributes(typeof(FNSerializeMemberAttribute), true);

				if (attributesCheck.Length == 0)
					continue;

				FieldInfo fn = t.GetField(fields[i].Name);
				if (fn != null)
				{
					if (fn.FieldType.IsArray)
					{
						//throw new NotImplementedException("Cannot deserialize arrays just yet!");
						JSONArray arrayData = data[fn.Name].AsArray;
						Array r = (Array)fn.GetValue(returnValue);

						//Get array type here, and figure out the proper casting for it

						//TODO: Cast properly for the correct array values
						//Will need to be able to cast List<string>.
						//This is the most difficult thing out of this serializer that I need to figure out
						for (int j = 0; j < arrayData.Count; ++j)
						{
						}
					}
					else
						SetFieldValues<T>(ref returnValue, data, fn);

					//Debug.Log(string.Format("fromM[{0}] l[{1}] v[{2}]", fields[i].Name, asd.Length, data[fields[i].Name]));
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Converts a serialized object to json
		/// </summary>
		/// <param name="serializableClass">The object to serialize</param>
		/// <returns>The serialized data</returns>
		public static JSONClass ToJSON(IFNSerializable serializedObject)
		{
			JSONClass returnValue = new JSONClass();

			Type t = serializedObject.GetType();
			FieldInfo[] fields = t.GetFields();

			for (int i = 0; i < fields.Length; ++i)
			{
				FieldInfo fn = fields[i];
				object[] attributesCheck = fn.GetCustomAttributes(typeof(FNSerializeMemberAttribute), true);

				if (attributesCheck.Length == 0)
					continue;

				if (fn != null)
				{
					if (fn.FieldType.IsArray)
					{
						//throw new NotImplementedException("Cannot serialize arrays just yet!");
						JSONArray arrayNode = new JSONArray();
						Array r = (Array)fn.GetValue(serializedObject);
						for (int j = 0; j < r.Length; ++j)
						{
							object arrayValue = r.GetValue(j);
							Type arrayType = arrayValue.GetType();

							if (arrayType.IsClass)
							{
								if (arrayType.GetInterface("IFNSerializable") != null)
								{
									IFNSerializable serializableClass = arrayValue as IFNSerializable;
									JSONClass nestSerialization = ToJSON(serializableClass);
									arrayNode.Add(nestSerialization);
								}
								else
									throw new MulticastNotSupportedException(string.Format("Cannot cast anything other than IFNSerializable to JSON, please mark your object [{0}] as IFNSerializable found in {1}.", fn.Name, t.Name));
							}
							else
							{
								//list<string>
							}
						}
						returnValue.Add(fn.Name, arrayNode);
					}
					else
						AddFieldValues(ref returnValue, fn, serializedObject);

					//Debug.Log(string.Format("m[{0}] l[{1}] v[{2}]", fields[i].Name, asd.Length, fn.GetValue(serializedObject)));
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Set field values to a class
		/// </summary>
		/// <param name="data">Data to add to</param>
		/// <param name="field">The field we are adding values from</param>
		/// <param name="obj">The object we are serializing</param>
		private static void SetFieldValues<T>(ref T obj, JSONClass data, FieldInfo field) where T : IFNSerializable
		{
			if (CanGetValue(field, typeof(double)))
				field.SetValue(obj, data[field.Name].AsDouble);
			else if (CanGetValue(field, typeof(float)))
				field.SetValue(obj, data[field.Name].AsFloat);
			else if (CanGetValue(field, typeof(int)) || CanGetValue(field, typeof(Enum))) //TODO: Test enums to make sure this works
				field.SetValue(obj, data[field.Name].AsInt);
			else if (CanGetValue(field, typeof(uint)))
				field.SetValue(obj, data[field.Name].AsUInt);
			else if (CanGetValue(field, typeof(bool)))
				field.SetValue(obj, data[field.Name].AsBool);
			else if (CanGetValue(field, typeof(byte)))
				field.SetValue(obj, data[field.Name].AsByte);
			else if (CanGetValue(field, typeof(ushort)))
				field.SetValue(obj, data[field.Name].AsUShort);
			else if (CanGetValue(field, typeof(short)))
				field.SetValue(obj, data[field.Name].AsShort);
			else if (CanGetValue(field, typeof(string)))
				field.SetValue(obj, data[field.Name].Value);
		}

		/// <summary>
		/// Add field values to an existing json class
		/// </summary>
		/// <param name="data">Data to add to</param>
		/// <param name="field">The field we are adding values from</param>
		/// <param name="obj">The object we are serializing</param>
		private static void AddFieldValues(ref JSONClass data, FieldInfo field, object obj)
		{
			if (CanGetValue(field, typeof(double)))
				data.Add(field.Name, new JSONData((double)field.GetValue(obj)));
			else if (CanGetValue(field, typeof(float)))
				data.Add(field.Name, new JSONData((float)field.GetValue(obj)));
			else if (CanGetValue(field, typeof(int)) || CanGetValue(field, typeof(Enum)))
				data.Add(field.Name, new JSONData((int)field.GetValue(obj)));
			else if (CanGetValue(field, typeof(uint)))
				data.Add(field.Name, new JSONData((uint)field.GetValue(obj)));
			else if (CanGetValue(field, typeof(bool)))
				data.Add(field.Name, new JSONData((bool)field.GetValue(obj)));
			else if (CanGetValue(field, typeof(byte)))
				data.Add(field.Name, new JSONData((byte)field.GetValue(obj)));
			else if (CanGetValue(field, typeof(ushort)))
				data.Add(field.Name, new JSONData((ushort)field.GetValue(obj)));
			else if (CanGetValue(field, typeof(short)))
				data.Add(field.Name, new JSONData((short)field.GetValue(obj)));
			else if (CanGetValue(field, typeof(string)))
				data.Add(field.Name, new JSONData(field.GetValue(obj) as string));
		}

		/// <summary>
		/// Whether we are able to retrieve this value from the field or not
		/// </summary>
		/// <param name="fieldInfo">The field we are checking</param>
		/// <param name="t">The type we are comparing against</param>
		/// <returns>True if it can get a value of this type</returns>
		private static bool CanGetValue(FieldInfo fieldInfo, Type t)
		{
			if (t.IsEnum)
				return fieldInfo.FieldType.IsEnum;

			return fieldInfo.FieldType == t;
		}
	}
}
