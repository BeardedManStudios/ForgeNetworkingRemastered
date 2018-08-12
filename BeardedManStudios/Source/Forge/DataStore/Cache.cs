/*-----------------------------+-------------------------------\
|                                                              |
|                         !!!NOTICE!!!                         |
|                                                              |
|  These libraries are under heavy development so they are     |
|  subject to make many changes as development continues.      |
|  For this reason, the libraries may not be well commented.   |
|  THANK YOU for supporting forge with all your feedback       |
|  suggestions, bug reports and comments!                      |
|                                                              |
|                              - The Forge Team                |
|                                Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

using BeardedManStudios.Forge.Networking.Frame;
using System;
using System.Collections.Generic;

namespace BeardedManStudios.Forge.Networking.DataStore
{
	/// <summary>
	/// The main class for managing and communicating data from the server cache
	/// </summary>
	/// <remarks>
	/// Cache is used to store any arbitrary data for your game, you can deposit supported data types to the server with a key, then access them again
	/// on any client by using the key to access the specific data stored. Cache.Set() and Cache.Request() being the two primary ways to use Cache.
	/// </remarks>
	public class Cache
	{
		public const int DEFAULT_CACHE_SERVER_PORT = 15942;

		// Default expiry datetime for a cached object.
		private readonly DateTime maxDateTime = DateTime.MaxValue;

		/// <summary>
		/// The memory cache for the data
		/// </summary>
		private Dictionary<string, CachedObject> memory = new Dictionary<string, CachedObject>();

		/// <summary>
		/// The main socket for communicating the cache back and forth
		/// </summary>
		public NetWorker Socket { get; private set; }

		// TODO:  Possibly make this global
		/// <summary>
		/// The set of types that are allowed and a byte mapping to them
		/// </summary>
		private static Dictionary<byte, Type> typeMap = new Dictionary<byte, Type>() {
			{ 0, typeof(byte) },
			{ 1, typeof(char) },
			{ 2, typeof(short) },
			{ 3, typeof(ushort) },
			{ 4, typeof(int) },
			{ 5, typeof(uint) },
			{ 6, typeof(long) },
			{ 7, typeof(ulong) },
			{ 8, typeof(bool) },
			{ 9, typeof(float) },
			{ 10, typeof(double) },
			{ 11, typeof(string) },
			{ 12, typeof(Vector) },/*
			{ 12, typeof(Vector2) },
			{ 13, typeof(Vector3) },
			{ 14, typeof(Vector4) },
			{ 15, typeof(Quaternion) },
			{ 16, typeof(Color) }*/
		};

		/// <summary>
		/// The current id that the callback stack is on
		/// </summary>
		private int responseHookIncrementer = 0;

		/// <summary>
		/// The main callback stack for when requesting data
		/// </summary>
		private Dictionary<int, Action<object>> responseHooks = new Dictionary<int, Action<object>>();

		public Cache(NetWorker socket)
		{
			Socket = socket;
			Socket.binaryMessageReceived += BinaryMessageReceived;
		}

		private void RemoveExpiredObjects()
		{
			foreach (KeyValuePair<string, CachedObject> entry in memory)
				if (entry.Value.IsExpired())
					memory.Remove(entry.Key);
		}

		/// <summary>
		/// Called when the network as interpreted that a cache message has been sent from the server
		/// </summary>
		/// <param name="player">The server</param>
		/// <param name="frame">The data that was received</param>
		private void BinaryMessageReceived(NetworkingPlayer player, Binary frame, NetWorker sender)
		{
			if (frame.GroupId != MessageGroupIds.CACHE)
				return;

			if (sender is IServer)
			{
				byte type = ObjectMapper.Instance.Map<byte>(frame.StreamData);
				int responseHookId = ObjectMapper.Instance.Map<int>(frame.StreamData);
				string key = ObjectMapper.Instance.Map<string>(frame.StreamData);

				object obj = Get(key);

				// TODO:  Let the client know it is null
				if (obj == null)
					return;

				BMSByte data = ObjectMapper.BMSByte(type, responseHookId, obj);

				Binary sendFrame = new Binary(sender.Time.Timestep, sender is TCPClient, data, Receivers.Target, MessageGroupIds.CACHE, sender is BaseTCP);

				if (sender is BaseTCP)
					((TCPServer)sender).Send(player.TcpClientHandle, sendFrame);
				else
					((UDPServer)sender).Send(player, sendFrame, true);
			}
			else
			{
				byte type = ObjectMapper.Instance.Map<byte>(frame.StreamData);
				int responseHookId = ObjectMapper.Instance.Map<int>(frame.StreamData);

				object obj = null;

				if (typeMap[type] == typeof(string))
					obj = ObjectMapper.Instance.Map<string>(frame.StreamData);
				/*else if (typeMap[type] == typeof(Vector2))
					obj = ObjectMapper.Map<Vector2>(stream);
				else if (typeMap[type] == typeof(Vector3))
					obj = ObjectMapper.Map<Vector3>(stream);
				else if (typeMap[type] == typeof(Vector4))
					obj = ObjectMapper.Map<Vector4>(stream);
				else if (typeMap[type] == typeof(Color))
					obj = ObjectMapper.Map<Color>(stream);
				else if (typeMap[type] == typeof(Quaternion))
					obj = ObjectMapper.Map<Quaternion>(stream);*/
				else
					obj = ObjectMapper.Instance.Map(typeMap[type], frame.StreamData);

				if (responseHooks.ContainsKey(responseHookId))
				{
					responseHooks[responseHookId](obj);
					responseHooks.Remove(responseHookId);
				}
			}
		}

		/// <summary>
		/// Get an object from cache
		/// </summary>
		/// <typeparam name="T">The type of object to store</typeparam>
		/// <param name="key">The name variable used for storing the desired object</param>
		/// <returns>Return object from key otherwise return the default value of the type or null</returns>
		private T Get<T>(string key)
		{
			if (!Socket.IsServer)
				return default(T);

			if (!memory.ContainsKey(key))
				return default(T);

			if (memory[key] is T)
				return (T)memory[key].Value;

			return default(T);
		}

		/// <summary>
		/// Used on the server to get an object at a given key from cache
		/// </summary>
		/// <param name="key">The key to be used in the dictionary lookup</param>
		/// <returns>The object at the given key in cache otherwise null</returns>
		private object Get(string key)
		{
			if (!Socket.IsServer)
				return null;

			if (memory.ContainsKey(key))
				return memory[key].Value;

			return null;
		}

		/// <summary>
		/// Get an object from cache
		/// </summary>
		/// <param name="key">The name variable used for storing the desired object</param>
		/// <returns>The string data at the desired key or null</returns>
		/// <remarks>
		/// Allows a client (or the server) to get a value from the Cache, the value is read directly from the server.
		/// A callback must be specified, this is because the code has to be executed after a moment when the response from the server
		/// is received. Request can be done like this:
		/// <code>
		/// void getServerDescription(){
		/// Cache.Request<string>("server_description", delegate (object response){
		///	 Debug.Log(((string) response));
		/// });
		/// }
		/// </code>
		/// The Cache only supports Forge's supported data Types, you can find a list of supported data Types in the NetSync documentation...
		/// </remarks>
		public void Request<T>(string key, Action<object> callback)
		{
			if (callback == null)
				throw new Exception("A callback is needed when requesting data from the server");

			if (Socket.IsServer)
			{
				callback(Get<T>(key));
				return;
			}

			responseHooks.Add(responseHookIncrementer, callback);

			byte targetType = byte.MaxValue;

			foreach (KeyValuePair<byte, Type> kv in typeMap)
			{
				if (typeof(T) == kv.Value)
				{
					targetType = kv.Key;
					break;
				}
			}

			if (targetType == byte.MaxValue)
				throw new Exception("Invalid type specified");

			BMSByte data = ObjectMapper.BMSByte(targetType, responseHookIncrementer, key);

			Binary sendFrame = new Binary(Socket.Time.Timestep, Socket is TCPClient, data, Receivers.Server, MessageGroupIds.CACHE, Socket is BaseTCP);

#if STEAMWORKS
            if (Socket is SteamP2PClient)
                ((SteamP2PClient)Socket).Send(sendFrame, true);
            else if (Socket is BaseTCP)
#else
            if (Socket is BaseTCP)
#endif
                ((TCPClient)Socket).Send(sendFrame);
			else
				((UDPClient)Socket).Send(sendFrame, true);

			responseHookIncrementer++;
		}


        /// <summary>
        /// Inserts a NEW key/value into cache
        /// </summary>
        /// <typeparam name="T">The serializable type of object</typeparam>
        /// <param name="key">The name variable used for storing the specified object</param>
        /// <param name="value">The object that is to be stored into cache</param>
        /// <returns>True if successful insert or False if the key already exists</returns>
        public bool Insert<T>(string key, T value)
		{
			return Insert(key, value, maxDateTime);
		}

		/// <summary>
		/// Inserts a NEW key/value into cache
		/// </summary>
		/// <typeparam name="T">The serializable type of object</typeparam>
		/// <param name="key">The name variable used for storing the specified object</param>
		/// <param name="value">The object that is to be stored into cache</param>
		/// <param name="expireAt">The DateTime defining when the cached object should expire</param>
		/// <returns>True if successful insert or False if the key already exists</returns>
		public bool Insert<T>(string key, T value, DateTime expireAt)
		{
			if (!(Socket is IServer))
				throw new Exception("Inserting cache values is not yet supported for clients!");

			if (!memory.ContainsKey(key))
				return false;

			memory.Add(key, new CachedObject(value, expireAt));

			return true;
		}

		/// <summary>
		/// Inserts a new key/value or updates a key's value in cache
		/// </summary>
		/// <typeparam name="T">The serializable type of object</typeparam>
		/// <param name="key">The name variable used for storing the specified object</param>
		/// <param name="value">The object that is to be stored into cache</param>
		/// <remarks>
		/// This inputs a value into the cache, this can only be called on the server, you can only input Types forge supports (See NetSync for supported Types).
		/// </remarks>
		public void Set<T>(string key, T value)
		{
			Set(key, value, maxDateTime);
		}

		/// <summary>
		/// Inserts a new key/value or updates a key's value in cache
		/// </summary>
		/// <typeparam name="T">The serializable type of object</typeparam>
		/// <param name="key">The name variable used for storing the specified object</param>
		/// <param name="value">The object that is to be stored into cache</param>
		/// <param name="expireAt">The DateTime defining when the cached object should expire</param>
		public void Set<T>(string key, T value, DateTime expireAt)
		{
			if (!(Socket is IServer))
				throw new Exception("Setting cache values is not yet supported for clients!");

			var cachedObject = new CachedObject(value, expireAt);

			if (!memory.ContainsKey(key))
				memory.Add(key, cachedObject);
			else
				memory[key] = cachedObject;
		}

		/// <summary>
		/// CachedObject class.
		/// </summary>
		public class CachedObject
		{
			public object Value { get; private set; }

			public DateTime ExpireAt { get; private set; }

			public CachedObject(object value, DateTime expireAt)
			{
				Value = value;
				ExpireAt = expireAt;
			}

			public bool IsExpired()
			{
				return DateTime.Now >= ExpireAt;
			}
		}
	}
}