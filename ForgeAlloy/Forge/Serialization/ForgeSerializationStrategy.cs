using System;
using System.Collections.Generic;

namespace Forge.Serialization
{
	public class ForgeSerializer : ISerializationStrategy
	{
		private readonly Dictionary<Type, ITypeSerializer> _serializers = new Dictionary<Type, ITypeSerializer>();
		public static ForgeSerializer Instance { get; private set; } = new ForgeSerializer();

		private ForgeSerializer() { }

		public void AddSerializer<T>(ITypeSerializer serializer)
		{
			var t = typeof(T);
			if (_serializers.ContainsKey(t))
				throw new SerializationTypeKeyAlreadyExistsException(t);
			_serializers.Add(t, serializer);
		}

		public void Serialize<T>(T val, BMSByte intoBuffer)
		{
			var serializer = GetSerializer<T>();
			serializer.Serialize(val, intoBuffer);
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
