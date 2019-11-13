using System;
using System.Collections.Generic;

namespace Forge.Serialization
{
	public class ForgeSerializationStrategy : ISerializationStrategy
	{
		private readonly Dictionary<Type, ITypeSerializer> _serializers = new Dictionary<Type, ITypeSerializer>();

		private static ForgeSerializationStrategy _instance = null;
		public static ForgeSerializationStrategy Instance
		{
			get
			{
				if (_instance == null)
					_instance = new ForgeSerializationStrategy();
				return _instance;
			}
		}

		private ForgeSerializationStrategy() { }

		public void AddSerializer<T>(ITypeSerializer serializer)
		{
			var t = typeof(T);
			if (_serializers.ContainsKey(t))
				throw new SerializationTypeKeyAlreadyExistsException(t);
			_serializers.Add(t, serializer);
		}

		public byte[] Serialize<T>(T val)
		{
			var serializer = GetSerializer<T>();
			return serializer.Serialize(val);
		}

		public T Deserialize<T>(BMSByte buffer)
		{
			var serializer = GetSerializer<T>();
			return (T)serializer.Deserialize(buffer);
		}

		public void Clear()
		{
			_serializers.Clear();
		}

		private ITypeSerializer GetSerializer<T>()
		{
			var t = typeof(T);
			if (!_serializers.TryGetValue(t, out var serializer))
				throw new SerializationTypeKeyDoesNotExistsException(t);
			return serializer;
		}
	}
}
