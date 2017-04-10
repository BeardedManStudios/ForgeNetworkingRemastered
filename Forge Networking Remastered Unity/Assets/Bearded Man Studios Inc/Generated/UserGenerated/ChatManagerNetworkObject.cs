using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class ChatManagerNetworkObject : NetworkObject
	{
		public const int IDENTITY = 1;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		private int _adfgadfgadga;
		public event FieldEvent<int> adfgadfgadgaChanged;
		public InterpolateUnknown adfgadfgadgaInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int adfgadfgadga
		{
			get { return _adfgadfgadga; }
			set
			{
				// Don't do anything if the value is the same
				if (_adfgadfgadga == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_adfgadfgadga = value;
				hasDirtyFields = true;
			}
		}

		public void SetadfgadfgadgaDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_adfgadfgadga(ulong timestep)
		{
			if (adfgadfgadgaChanged != null) adfgadfgadgaChanged(_adfgadfgadga, timestep);
			if (fieldAltered != null) fieldAltered("adfgadfgadga", _adfgadfgadga, timestep);
		}

		protected override void OwnershipChanged()
		{
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			adfgadfgadgaInterpolation.current = adfgadfgadgaInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _adfgadfgadga);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_adfgadfgadga = UnityObjectMapper.Instance.Map<int>(payload);
			adfgadfgadgaInterpolation.current = _adfgadfgadga;
			adfgadfgadgaInterpolation.target = _adfgadfgadga;
			RunChange_adfgadfgadga(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _adfgadfgadga);

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (adfgadfgadgaInterpolation.Enabled)
				{
					adfgadfgadgaInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					adfgadfgadgaInterpolation.Timestep = timestep;
				}
				else
				{
					_adfgadfgadga = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_adfgadfgadga(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (adfgadfgadgaInterpolation.Enabled && !adfgadfgadgaInterpolation.current.Near(adfgadfgadgaInterpolation.target, 0.0015f))
			{
				_adfgadfgadga = (int)adfgadfgadgaInterpolation.Interpolate();
				RunChange_adfgadfgadga(adfgadfgadgaInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public ChatManagerNetworkObject() : base() { Initialize(); }
		public ChatManagerNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public ChatManagerNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}