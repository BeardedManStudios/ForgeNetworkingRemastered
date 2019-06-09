using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CubeForgeGame : CubeForgeGameBehavior
{
	/// <summary>
	/// This object is a singleton
	/// </summary>
	public static CubeForgeGame Instance { get; private set; }

	/// <summary>
	/// The server can get the player count from the networker but the client
	/// currently can not, so we will track player counts in this variable for
	/// both the client and the server
	/// </summary>
	private int playerCount = 0;

	/// <summary>
	/// A list of all of the primitive objects that can be spawn
	/// </summary>
	public GameObject[] primitives = null;

	/// <summary>
	/// The index that should be used for the primitive selection
	/// </summary>
	private byte primitiveIndex = 0;

	/// <summary>
	/// A list to store all of the references for the created primitives
	/// </summary>
	private Dictionary<Vector3, GameObject> primitiveInstances = new Dictionary<Vector3, GameObject>();

	/// <summary>
	/// The minimum bounds of the world in cube space
	/// </summary>
	private Vector3 min = Vector3.zero;

	/// <summary>
	/// The maximum bound of the world in cube space
	/// </summary>
	private Vector3 max = Vector3.zero;

	/// <summary>
	/// The amount of time in milliseconds for the round trip latency to/from the server
	/// </summary>
	public double RoundTripLatency { get; private set; }

	private NetworkCameraNetworkObject netCam;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	private void Start()
	{
		NetworkManager.Instance.objectInitialized += ObjectInitialized;

		// Since this object is a singleton we can create the player from here as
		// it is in the scene at start time and we want to create a player camera
		// for this newly created server or newly connected client
		NetworkManager.Instance.InstantiateNetworkCamera();

		NetworkManager.Instance.Networker.onPingPong += OnPingPong;

		// If the current networker is the server, then setup the callbacks for when
		// a player connects
		if (NetworkManager.Instance.Networker is IServer)
		{
			// When a player is accepted on the server we need to send them the map
			// information through the rpc attached to this object
			NetworkManager.Instance.Networker.playerAccepted += PlayerAccepted;
		}
		else
		{
			NetworkManager.Instance.Networker.disconnected += DisconnectedFromServer;
		}
	}

	private void PlayerAccepted(NetworkingPlayer player, NetWorker sender)
	{
		MainThreadManager.Run(() => { networkObject.SendRpc(player, RPC_INITIALIZE_MAP, min, max, SerializeMap()); });
	}

	/// <summary>
	/// Called whenever a new object is being initialized on the network
	/// </summary>
	/// <param name="behavior">The behavior for the object that is initialized</param>
	/// <param name="obj">The network object that is being initialized</param>
	private void ObjectInitialized(INetworkBehavior behavior, NetworkObject obj)
	{
		if (!(obj is NetworkCameraNetworkObject))
			return;

		// Since the camera represents the player, if one is being created
		// we need to increment the player count for the display. NOTE:
		playerCount++;

		// When this object is destroyed we need to decrement the player count as the camera
		// represents the player
		obj.onDestroy += (sender) =>
		{
			playerCount--;
		};

		if (NetworkManager.Instance.Networker is IServer)
			obj.Owner.disconnected += (sender) => { obj.Destroy(); };

		netCam = obj as NetworkCameraNetworkObject;
	}

	private void DisconnectedFromServer(NetWorker sender)
	{
		NetworkManager.Instance.Networker.disconnected -= DisconnectedFromServer;

		MainThreadManager.Run(() =>
		{
			NetworkManager.Instance.Disconnect();
			SceneManager.LoadScene(0);
		});
	}

	private void OnPingPong(double ping, NetWorker sender)
	{
		RoundTripLatency = ping;
	}

	private void Update()
	{

		// Pretty useless at the moment as the only primitive supported is a cube
		// will add a sphere or something later
		if (Input.GetKeyDown(KeyCode.Alpha0))
			primitiveIndex = 0;

		if (NetworkManager.Instance == null)
			return;

		if (!NetworkManager.Instance.Networker.IsServer)
			return;

		//if (Input.GetKeyDown(KeyCode.Space))
		//{
		//	// We need to clean up the network objects before we change the scene
		//	// since we are loading this same scene. When that scene loads it will
		//	// re-create the objects, so we don't want lingering ones
		//	Cleanup();
		//	SceneManager.LoadScene(1);
		//}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			NetworkManager.Instance.Disconnect();
			SceneManager.LoadScene(0);
		}

		// TODO:  Add a sphere to this if chain
	}

	private void OnDestroy()
	{
		Cleanup();
	}

	private void Cleanup()
	{
		NetworkManager.Instance.Networker.playerAccepted -= PlayerAccepted;
		NetworkManager.Instance.Networker.onPingPong -= OnPingPong;
		NetworkManager.Instance.objectInitialized -= ObjectInitialized;

		if (networkObject != null)
			networkObject.Destroy();

		if (netCam != null)
			netCam.Destroy();
	}

	private void WriteLabel(Rect rect, string message)
	{
		GUI.color = Color.black;
		GUI.Label(rect, message);

		// Do the same thing as above but make the above UI look like a solid
		// shadow so that the text is readable on any contrast screen
		GUI.color = Color.white;
		GUI.Label(rect, message);
	}

	private void OnGUI()
	{
		if (NetworkManager.Instance == null || NetworkManager.Instance.Networker == null)
			return;

		// If there are no players, then the scene is currently being loaded, otherwise
		// show the current count of players in the game
		if (playerCount == 0)
			WriteLabel(new Rect(14, 14, 100, 25), "Loading...");
		else
			WriteLabel(new Rect(14, 14, 100, 25), "Players: " + playerCount);

		WriteLabel(new Rect(14, 28, 100, 25), "Time: " + NetworkManager.Instance.Networker.Time.Timestep);
		WriteLabel(new Rect(14, 42, 256, 25), "Bandwidth In: " + NetworkManager.Instance.Networker.BandwidthIn);
		WriteLabel(new Rect(14, 56, 256, 25), "Bandwidth Out: " + NetworkManager.Instance.Networker.BandwidthOut);
		WriteLabel(new Rect(14, 70, 256, 25), "Round Trip Latency (ms): " + RoundTripLatency);
	}

	/// <summary>
	/// A helper method to create the currently selected primitive type via RPC
	/// </summary>
	/// <param name="position"></param>
	public void CreatePrimitive(Vector3 position)
	{
		networkObject.SendRpc(RPC_CREATE_PRIMITIVE, Receivers.All, primitiveIndex, position);
	}

	/// <summary>
	/// A helper method to destroy a given primitive on the network
	/// </summary>
	/// <param name="primitive"></param>
	public void DestroyPrimitive(Primitive primitive)
	{
		networkObject.SendRpc(RPC_DESTROY_PRIMITIVE, Receivers.All, primitive.transform.position);
	}

	/// <summary>
	/// Serialize the 3-dimensional cubic map into a single dimensional array
	/// Also takes care of packing the world into bits rather than bytes
	/// </summary>
	/// <returns>The map after being serialized to a byte array</returns>
	private byte[] SerializeMap()
	{
		// Get the 3-dimensional lengths of the world
		int width = (int)Mathf.Ceil((max.x - min.x)) + 1;
		int height = (int)Mathf.Ceil((max.y - min.y)) + 1;
		int depth = (int)Mathf.Ceil((max.z - min.z)) + 1;

		// Create a bit array to store the toggles for cubes
		BitArray bitmap = new BitArray(width * height * depth);

		// pack the cubes into the bit array
		int idx = 0;
		foreach (GameObject obj in primitiveInstances.Values)
		{
			Vector3 d = obj.transform.position - min;
			idx = (int)((d.z * width * height) + (d.y * width) + d.x);
			bitmap[idx] = true;
		}

		// Create a byte array to hold the packed bit data
		byte[] mapData = new byte[(int)Mathf.Ceil(bitmap.Length / 8.0f)];

		// Copy the bit mapped data into the above byte array
		((ICollection)bitmap).CopyTo(mapData, 0);

		return mapData;
	}

	/// <summary>
	/// Used to take a byte array with a bit array mapping of the primitives and expand
	/// it into the representation of the map
	/// </summary>
	/// <param name="minimum">The minimum bounds for the world</param>
	/// <param name="maximum">The maximum bounds for the world</param>
	/// <param name="data">The byte array data that has the bit array mapping</param>
	private void DeserializeMap(Vector3 minimum, Vector3 maximum, byte[] data)
	{
		min = minimum;
		max = maximum;

		// Get the supplied 3-dimensional lengths of the world
		int width = (int)Mathf.Ceil((max.x - min.x)) + 1;
		int height = (int)Mathf.Ceil((max.y - min.y)) + 1;
		//int depth = (int)Mathf.Ceil((max.z - min.z)) + 1;

		// Unpack the byte array into a bit array and begin mapping the world
		int idx, z, y, x;
		BitArray bitmap = new BitArray(data);
		for (int i = 0; i < bitmap.Length; i++)
		{
			if (bitmap[i] == false)
				continue;

			idx = i;

			z = idx / (width * height);
			idx -= (z * width * height);
			y = idx / width;
			x = idx % width;

			Vector3 position = new Vector3(x + min.x, y + min.y, z + min.z);

			var obj = Instantiate(primitives[0], position, Quaternion.identity) as GameObject;
			primitiveInstances.Add(obj.transform.position, obj);
		}
	}

	#region RPCs
	/// <summary>
	/// When received, this RPC will take the byte mapped data and use it to create the world
	/// This is automatically called on the client when it has successfully connected
	/// </summary>
	/// <param name="args">
	/// [0] byte[] is the mapped data for the world
	/// </param>
	public override void InitializeMap(RpcArgs args)
	{
		MainThreadManager.Run(() => { DeserializeMap(args.GetNext<Vector3>(), args.GetNext<Vector3>(), args.GetNext<byte[]>()); });
	}

	/// <summary>
	/// Used to create a primitive on the network
	/// </summary>
	/// <param name="args">
	/// [0] byte The type of primitive to be created
	/// [1] Vector3 The position to spawn the primitive in
	/// </param>
	public override void CreatePrimitive(RpcArgs args)
	{
		byte type = args.GetNext<byte>();
		Vector3 position = args.GetNext<Vector3>();

		if (type >= primitives.Length)
			return;

		MainThreadManager.Run(() =>
		{
			if (primitiveInstances.ContainsKey(position))
				return;

			// RPCs are reliable so they are always in order
			var obj = Instantiate(primitives[type], position, Quaternion.identity) as GameObject;
			primitiveInstances.Add(obj.transform.position, obj);

			min = Vector3.Min(min, position);
			max = Vector3.Max(max, position);
		});
	}

	/// <summary>
	/// Used to remove a primitive on the network
	/// </summary>
	/// <param name="args">
	/// [0] Vector3 The position of the object as it is used for the lookup table
	/// </param>
	public override void DestroyPrimitive(RpcArgs args)
	{
		Action execute = () =>
		{
			var pos = args.GetNext<Vector3>();
			if (!primitiveInstances.ContainsKey(pos))
				return;

			Destroy(primitiveInstances[pos]);
			primitiveInstances.Remove(pos);

			// TODO:  Adjust the min / max
			// TODO:  Send the bit index and remove using that
		};

		// Normally you can just call the method, however since we don't know if
		// you are using the main thread runner in this example, we cover both cases
		if (Rpc.MainThreadRunner != null)
			execute();
		else
			MainThreadManager.Run(execute);
	}

	public override void TestMe(RpcArgs args)
	{
		//throw new NotImplementedException();
	}
	#endregion
}