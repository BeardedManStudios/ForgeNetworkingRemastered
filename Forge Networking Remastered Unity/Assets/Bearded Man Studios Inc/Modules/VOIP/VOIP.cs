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
|							   - The Forge Team                |
|								 Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

#if !UNITY_WEBGL && !WINDOWS_UWP
using BeardedManStudios.Forge.Logging;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Threading;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Unity.Modules
{
	public class VOIP
	{
		/// <summary>
		/// Quality of the VOIP
		/// </summary>
		public enum Quality
		{
			Low = 4000,
			Mid = 8000,
			High = 12000
		}

		/// <summary>
		/// The Audiosource to hold the VOIP clip
		/// </summary>
		public AudioSource source = null;

		private int lastSample = 0;
		private AudioClip mic = null;
		private AudioSource audio = null;

		/// <summary>
		/// The Frequency of the VOIP
		/// </summary>
		public int Frequency { get; set; }

		/// <summary>
		/// The NetWorker(Socket) the VOIP is on
		/// </summary>
		public static NetWorker Socket { get; private set; }

		private int channels = 0;
		private int readUpdateId = 0;
		private int previousReadUpdateId = -1;
		private float[] samples = null;
		private List<float> writeSamples = null;
		private List<float> readSamples = null;
		private float audio3D = 0.0f;

		private float WRITE_FLUSH_TIME = 0.5f;
		private float READ_FLUSH_TIME = 0.5f;
		private float readFlushTimer = 0.0f;
		private float writeFlushTimer = 0.0f;

		public bool IsLocalTesting { get; set; }

		private void Init(float audio3D, int frequency)
		{
			writeSamples = new List<float>(1024);
			this.audio3D = audio3D;
			source = new GameObject("VOIP").AddComponent<AudioSource>();

			audio = source;

			Frequency = frequency;
		}

		/// <summary>
		/// Constructor with a check whether or not the audio is 3D or 2D
		/// </summary>
		/// <param name="is3DAudio">Whether the audio will be 3D or 2D</param>
		public VOIP(float audio3D)
		{
			Init(audio3D, (int)Quality.Mid);
		}

		/// <summary>
		/// Constructor with a check if 3D Audio and Quality
		/// </summary>
		/// <param name="is3DAudio">Whether the audio will be 3D or 2D</param>
		/// <param name="quality">Quality of the VOIP</param>
		public VOIP(float audio3D, Quality quality)
		{
			Init(audio3D, (int)quality);
		}

		/// <summary>
		/// Constructor with a check if 3D Audio and Frequency
		/// </summary>
		/// <param name="is3DAudio">Whether the audio will be 3D or 2D</param>
		/// <param name="frequency">Frequency of the VOIP</param>
		public VOIP(float audio3D, int frequency)
		{
			Init(audio3D, frequency);
		}

		private void Setup(string hostAddress, ushort port)
		{
			Socket.binaryMessageReceived += Read;

			if (Socket.IsServer)
			{
				((UDPServer)Socket).Connect(hostAddress, port);
				Socket.playerConnected += (player, sender) => { BMSLog.Log("PLAYER CONNECTED " + player.IPEndPointHandle.Address); };
				Socket.playerAccepted += (player, sender) => { BMSLog.Log("PLAYER ACCEPTED " + player.IPEndPointHandle.Address); };
				Socket.playerRejected += (player, sender) => { BMSLog.Log("PLAYER REJECTED " + player.IPEndPointHandle.Address); };
				Socket.playerDisconnected += (player, sender) => { BMSLog.Log("PLAYER DISCONNECTED " + player.IPEndPointHandle.Address); };
				StartVOIP(Socket);
			}
			else
			{
				Socket.serverAccepted += StartVOIP;
				((UDPClient)Socket).Connect(hostAddress, port);
			}
		}

		private void StartVOIP(NetWorker sender)
		{
			sender.serverAccepted -= StartVOIP;

			MainThreadManager.Run(() =>
			{
				mic = Microphone.Start(null, true, 100, Frequency);
				channels = mic.channels;

				if (mic == null)
				{
					Debug.LogError("A default microphone was not found or plugged into the system");
					return;
				}

				Task.Queue(VOIPWorker);
			});
		}

		/// <summary>
		/// Start the VOIP sending server
		/// </summary>
		/// <param name="hostAddress">The address for the VOIP to host from</param>
		/// <param name="port">Port of the VOIP</param>
		/// <param name="connections">Maximum number of connections to the VOIP</param>
		public void StartServer(int connections, string hostAddress = "0.0.0.0", ushort port = 15959)
		{
			Socket = new UDPServer(connections);
			Setup(hostAddress, port);
		}

		/// <summary>
		/// Start the VOIP sending client
		/// </summary>
		/// <param name="hostAddress">The address for the VOIP to connect to</param>
		/// <param name="port">Port of the VOIP</param>
		public void StartClient(string hostAddress = "127.0.0.1", ushort port = 15959)
		{
			Socket = new UDPClient();
			Setup(hostAddress, port);
		}

		/// <summary>
		/// Stop sending the audio through VOIP
		/// </summary>
		public void StopSending()
		{
			Microphone.End(null);
			mic = null;

			if (Socket != null)
				Socket.Disconnect(false);
		}

		private void ReadMic()
		{
			writeFlushTimer += Time.deltaTime;
			int pos = Microphone.GetPosition(null);
			int diff = pos - lastSample;

			if (diff > 0)
			{
				samples = new float[diff * channels];
				mic.GetData(samples, lastSample);

				lock (writeSamples)
				{
					writeSamples.AddRange(samples);
				}
			}

			lastSample = pos;
		}

		public void Update()
		{
			if (!Socket.IsConnected || mic == null)
				return;

			if (!IsLocalTesting)
				ReadMic();
			else if (Socket.IsServer)
			{
				ReadMic();
				return;
			}

			readFlushTimer += Time.deltaTime;
			if (readFlushTimer > READ_FLUSH_TIME)
			{
				if (readUpdateId != previousReadUpdateId && readSamples != null && readSamples.Count > 0)
				{
					previousReadUpdateId = readUpdateId;

					lock (readSamples)
					{
						audio.clip = AudioClip.Create("VOIP", readSamples.Count, channels, Frequency, false);
						audio.spatialBlend = audio3D;

						audio.clip.SetData(readSamples.ToArray(), 0);
						if (!audio.isPlaying) audio.Play();

						readSamples.Clear();
					}
				}

				readFlushTimer = 0.0f;
			}
		}

		BMSByte writeBuffer = new BMSByte();

		private void VOIPWorker()
		{
			while (Socket.IsConnected)
			{
				if (writeFlushTimer >= WRITE_FLUSH_TIME && writeSamples.Count > 0)
				{
					writeFlushTimer = 0.0f;
					lock (writeSamples)
					{
						writeBuffer.Clone(ToByteArray(writeSamples));
						writeSamples.Clear();
					}

					Binary voice = new Binary(Socket.Time.Timestep, false, writeBuffer, Receivers.All, MessageGroupIds.VOIP, false);

					((BaseUDP)Socket).Send(voice);
				}

				MainThreadManager.ThreadSleep(10);
			}
		}

		private void Read(NetworkingPlayer player, Binary frame, NetWorker sender)
		{
			// This read is not for VOIP
			if (frame.GroupId != MessageGroupIds.VOIP)
				return;

			float[] tmp = ToFloatArray(frame.StreamData);

			if (readSamples == null)
				readSamples = new List<float>(tmp);

			lock (readSamples)
			{
				readSamples.AddRange(tmp);
			}

			readUpdateId++;
		}

		private byte[] ToByteArray(List<float> sampleList)
		{
			int len = sampleList.Count * 4;
			byte[] byteArray = new byte[len];
			int pos = 0;

			for (int i = 0; i < sampleList.Count; i++)
			{
				byte[] data = BitConverter.GetBytes(sampleList[i]);
				Array.Copy(data, 0, byteArray, pos, 4);
				pos += 4;
			}

			return byteArray;
		}

		private float[] ToFloatArray(BMSByte data)
		{
			int len = (data.Size - 1) / 4;
			float[] floatArray = new float[len];

			for (int i = 0; i < data.Size - 1; i += 4)
				floatArray[i / 4] = BitConverter.ToSingle(data.byteArr, i);

			return floatArray;
		}
	}
}
#endif