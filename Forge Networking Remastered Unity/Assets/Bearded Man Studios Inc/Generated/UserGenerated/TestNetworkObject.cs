using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0,0,0,0,0,0,0,0,0.15,0,0,0]")]
	public partial class TestNetworkObject : NetworkObject
	{
		public const int IDENTITY = 5;

		private byte[] _dirtyFields = new byte[2];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private byte _fieldByte;
		public event FieldEvent<byte> fieldByteChanged;
		public Interpolated<byte> fieldByteInterpolation = new Interpolated<byte>() { LerpT = 0f, Enabled = false };
		public byte fieldByte
		{
			get { return _fieldByte; }
			set
			{
				// Don't do anything if the value is the same
				if (_fieldByte == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_fieldByte = value;
				hasDirtyFields = true;
			}
		}

		public void SetfieldByteDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_fieldByte(ulong timestep)
		{
			if (fieldByteChanged != null) fieldByteChanged(_fieldByte, timestep);
			if (fieldAltered != null) fieldAltered("fieldByte", _fieldByte, timestep);
		}
		[ForgeGeneratedField]
		private char _fieldChar;
		public event FieldEvent<char> fieldCharChanged;
		public Interpolated<char> fieldCharInterpolation = new Interpolated<char>() { LerpT = 0f, Enabled = false };
		public char fieldChar
		{
			get { return _fieldChar; }
			set
			{
				// Don't do anything if the value is the same
				if (_fieldChar == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_fieldChar = value;
				hasDirtyFields = true;
			}
		}

		public void SetfieldCharDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_fieldChar(ulong timestep)
		{
			if (fieldCharChanged != null) fieldCharChanged(_fieldChar, timestep);
			if (fieldAltered != null) fieldAltered("fieldChar", _fieldChar, timestep);
		}
		[ForgeGeneratedField]
		private short _fieldShort;
		public event FieldEvent<short> fieldShortChanged;
		public Interpolated<short> fieldShortInterpolation = new Interpolated<short>() { LerpT = 0f, Enabled = false };
		public short fieldShort
		{
			get { return _fieldShort; }
			set
			{
				// Don't do anything if the value is the same
				if (_fieldShort == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_fieldShort = value;
				hasDirtyFields = true;
			}
		}

		public void SetfieldShortDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_fieldShort(ulong timestep)
		{
			if (fieldShortChanged != null) fieldShortChanged(_fieldShort, timestep);
			if (fieldAltered != null) fieldAltered("fieldShort", _fieldShort, timestep);
		}
		[ForgeGeneratedField]
		private ushort _fieldUShort;
		public event FieldEvent<ushort> fieldUShortChanged;
		public Interpolated<ushort> fieldUShortInterpolation = new Interpolated<ushort>() { LerpT = 0f, Enabled = false };
		public ushort fieldUShort
		{
			get { return _fieldUShort; }
			set
			{
				// Don't do anything if the value is the same
				if (_fieldUShort == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_fieldUShort = value;
				hasDirtyFields = true;
			}
		}

		public void SetfieldUShortDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_fieldUShort(ulong timestep)
		{
			if (fieldUShortChanged != null) fieldUShortChanged(_fieldUShort, timestep);
			if (fieldAltered != null) fieldAltered("fieldUShort", _fieldUShort, timestep);
		}
		[ForgeGeneratedField]
		private bool _fieldBool;
		public event FieldEvent<bool> fieldBoolChanged;
		public Interpolated<bool> fieldBoolInterpolation = new Interpolated<bool>() { LerpT = 0f, Enabled = false };
		public bool fieldBool
		{
			get { return _fieldBool; }
			set
			{
				// Don't do anything if the value is the same
				if (_fieldBool == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x10;
				_fieldBool = value;
				hasDirtyFields = true;
			}
		}

		public void SetfieldBoolDirty()
		{
			_dirtyFields[0] |= 0x10;
			hasDirtyFields = true;
		}

		private void RunChange_fieldBool(ulong timestep)
		{
			if (fieldBoolChanged != null) fieldBoolChanged(_fieldBool, timestep);
			if (fieldAltered != null) fieldAltered("fieldBool", _fieldBool, timestep);
		}
		[ForgeGeneratedField]
		private int _fieldInt;
		public event FieldEvent<int> fieldIntChanged;
		public Interpolated<int> fieldIntInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int fieldInt
		{
			get { return _fieldInt; }
			set
			{
				// Don't do anything if the value is the same
				if (_fieldInt == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x20;
				_fieldInt = value;
				hasDirtyFields = true;
			}
		}

		public void SetfieldIntDirty()
		{
			_dirtyFields[0] |= 0x20;
			hasDirtyFields = true;
		}

		private void RunChange_fieldInt(ulong timestep)
		{
			if (fieldIntChanged != null) fieldIntChanged(_fieldInt, timestep);
			if (fieldAltered != null) fieldAltered("fieldInt", _fieldInt, timestep);
		}
		[ForgeGeneratedField]
		private uint _fieldUInt;
		public event FieldEvent<uint> fieldUIntChanged;
		public Interpolated<uint> fieldUIntInterpolation = new Interpolated<uint>() { LerpT = 0f, Enabled = false };
		public uint fieldUInt
		{
			get { return _fieldUInt; }
			set
			{
				// Don't do anything if the value is the same
				if (_fieldUInt == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x40;
				_fieldUInt = value;
				hasDirtyFields = true;
			}
		}

		public void SetfieldUIntDirty()
		{
			_dirtyFields[0] |= 0x40;
			hasDirtyFields = true;
		}

		private void RunChange_fieldUInt(ulong timestep)
		{
			if (fieldUIntChanged != null) fieldUIntChanged(_fieldUInt, timestep);
			if (fieldAltered != null) fieldAltered("fieldUInt", _fieldUInt, timestep);
		}
		[ForgeGeneratedField]
		private float _fieldFloat;
		public event FieldEvent<float> fieldFloatChanged;
		public InterpolateFloat fieldFloatInterpolation = new InterpolateFloat() { LerpT = 0f, Enabled = false };
		public float fieldFloat
		{
			get { return _fieldFloat; }
			set
			{
				// Don't do anything if the value is the same
				if (_fieldFloat == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x80;
				_fieldFloat = value;
				hasDirtyFields = true;
			}
		}

		public void SetfieldFloatDirty()
		{
			_dirtyFields[0] |= 0x80;
			hasDirtyFields = true;
		}

		private void RunChange_fieldFloat(ulong timestep)
		{
			if (fieldFloatChanged != null) fieldFloatChanged(_fieldFloat, timestep);
			if (fieldAltered != null) fieldAltered("fieldFloat", _fieldFloat, timestep);
		}
		[ForgeGeneratedField]
		private float _fieldFloatInterpolate;
		public event FieldEvent<float> fieldFloatInterpolateChanged;
		public InterpolateFloat fieldFloatInterpolateInterpolation = new InterpolateFloat() { LerpT = 0.15f, Enabled = true };
		public float fieldFloatInterpolate
		{
			get { return _fieldFloatInterpolate; }
			set
			{
				// Don't do anything if the value is the same
				if (_fieldFloatInterpolate == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[1] |= 0x1;
				_fieldFloatInterpolate = value;
				hasDirtyFields = true;
			}
		}

		public void SetfieldFloatInterpolateDirty()
		{
			_dirtyFields[1] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_fieldFloatInterpolate(ulong timestep)
		{
			if (fieldFloatInterpolateChanged != null) fieldFloatInterpolateChanged(_fieldFloatInterpolate, timestep);
			if (fieldAltered != null) fieldAltered("fieldFloatInterpolate", _fieldFloatInterpolate, timestep);
		}
		[ForgeGeneratedField]
		private long _fieldLong;
		public event FieldEvent<long> fieldLongChanged;
		public Interpolated<long> fieldLongInterpolation = new Interpolated<long>() { LerpT = 0f, Enabled = false };
		public long fieldLong
		{
			get { return _fieldLong; }
			set
			{
				// Don't do anything if the value is the same
				if (_fieldLong == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[1] |= 0x2;
				_fieldLong = value;
				hasDirtyFields = true;
			}
		}

		public void SetfieldLongDirty()
		{
			_dirtyFields[1] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_fieldLong(ulong timestep)
		{
			if (fieldLongChanged != null) fieldLongChanged(_fieldLong, timestep);
			if (fieldAltered != null) fieldAltered("fieldLong", _fieldLong, timestep);
		}
		[ForgeGeneratedField]
		private ulong _fieldULong;
		public event FieldEvent<ulong> fieldULongChanged;
		public Interpolated<ulong> fieldULongInterpolation = new Interpolated<ulong>() { LerpT = 0f, Enabled = false };
		public ulong fieldULong
		{
			get { return _fieldULong; }
			set
			{
				// Don't do anything if the value is the same
				if (_fieldULong == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[1] |= 0x4;
				_fieldULong = value;
				hasDirtyFields = true;
			}
		}

		public void SetfieldULongDirty()
		{
			_dirtyFields[1] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_fieldULong(ulong timestep)
		{
			if (fieldULongChanged != null) fieldULongChanged(_fieldULong, timestep);
			if (fieldAltered != null) fieldAltered("fieldULong", _fieldULong, timestep);
		}
		[ForgeGeneratedField]
		private double _fieldDouble;
		public event FieldEvent<double> fieldDoubleChanged;
		public Interpolated<double> fieldDoubleInterpolation = new Interpolated<double>() { LerpT = 0f, Enabled = false };
		public double fieldDouble
		{
			get { return _fieldDouble; }
			set
			{
				// Don't do anything if the value is the same
				if (_fieldDouble == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[1] |= 0x8;
				_fieldDouble = value;
				hasDirtyFields = true;
			}
		}

		public void SetfieldDoubleDirty()
		{
			_dirtyFields[1] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_fieldDouble(ulong timestep)
		{
			if (fieldDoubleChanged != null) fieldDoubleChanged(_fieldDouble, timestep);
			if (fieldAltered != null) fieldAltered("fieldDouble", _fieldDouble, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			fieldByteInterpolation.current = fieldByteInterpolation.target;
			fieldCharInterpolation.current = fieldCharInterpolation.target;
			fieldShortInterpolation.current = fieldShortInterpolation.target;
			fieldUShortInterpolation.current = fieldUShortInterpolation.target;
			fieldBoolInterpolation.current = fieldBoolInterpolation.target;
			fieldIntInterpolation.current = fieldIntInterpolation.target;
			fieldUIntInterpolation.current = fieldUIntInterpolation.target;
			fieldFloatInterpolation.current = fieldFloatInterpolation.target;
			fieldFloatInterpolateInterpolation.current = fieldFloatInterpolateInterpolation.target;
			fieldLongInterpolation.current = fieldLongInterpolation.target;
			fieldULongInterpolation.current = fieldULongInterpolation.target;
			fieldDoubleInterpolation.current = fieldDoubleInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _fieldByte);
			UnityObjectMapper.Instance.MapBytes(data, _fieldChar);
			UnityObjectMapper.Instance.MapBytes(data, _fieldShort);
			UnityObjectMapper.Instance.MapBytes(data, _fieldUShort);
			UnityObjectMapper.Instance.MapBytes(data, _fieldBool);
			UnityObjectMapper.Instance.MapBytes(data, _fieldInt);
			UnityObjectMapper.Instance.MapBytes(data, _fieldUInt);
			UnityObjectMapper.Instance.MapBytes(data, _fieldFloat);
			UnityObjectMapper.Instance.MapBytes(data, _fieldFloatInterpolate);
			UnityObjectMapper.Instance.MapBytes(data, _fieldLong);
			UnityObjectMapper.Instance.MapBytes(data, _fieldULong);
			UnityObjectMapper.Instance.MapBytes(data, _fieldDouble);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_fieldByte = UnityObjectMapper.Instance.Map<byte>(payload);
			fieldByteInterpolation.current = _fieldByte;
			fieldByteInterpolation.target = _fieldByte;
			RunChange_fieldByte(timestep);
			_fieldChar = UnityObjectMapper.Instance.Map<char>(payload);
			fieldCharInterpolation.current = _fieldChar;
			fieldCharInterpolation.target = _fieldChar;
			RunChange_fieldChar(timestep);
			_fieldShort = UnityObjectMapper.Instance.Map<short>(payload);
			fieldShortInterpolation.current = _fieldShort;
			fieldShortInterpolation.target = _fieldShort;
			RunChange_fieldShort(timestep);
			_fieldUShort = UnityObjectMapper.Instance.Map<ushort>(payload);
			fieldUShortInterpolation.current = _fieldUShort;
			fieldUShortInterpolation.target = _fieldUShort;
			RunChange_fieldUShort(timestep);
			_fieldBool = UnityObjectMapper.Instance.Map<bool>(payload);
			fieldBoolInterpolation.current = _fieldBool;
			fieldBoolInterpolation.target = _fieldBool;
			RunChange_fieldBool(timestep);
			_fieldInt = UnityObjectMapper.Instance.Map<int>(payload);
			fieldIntInterpolation.current = _fieldInt;
			fieldIntInterpolation.target = _fieldInt;
			RunChange_fieldInt(timestep);
			_fieldUInt = UnityObjectMapper.Instance.Map<uint>(payload);
			fieldUIntInterpolation.current = _fieldUInt;
			fieldUIntInterpolation.target = _fieldUInt;
			RunChange_fieldUInt(timestep);
			_fieldFloat = UnityObjectMapper.Instance.Map<float>(payload);
			fieldFloatInterpolation.current = _fieldFloat;
			fieldFloatInterpolation.target = _fieldFloat;
			RunChange_fieldFloat(timestep);
			_fieldFloatInterpolate = UnityObjectMapper.Instance.Map<float>(payload);
			fieldFloatInterpolateInterpolation.current = _fieldFloatInterpolate;
			fieldFloatInterpolateInterpolation.target = _fieldFloatInterpolate;
			RunChange_fieldFloatInterpolate(timestep);
			_fieldLong = UnityObjectMapper.Instance.Map<long>(payload);
			fieldLongInterpolation.current = _fieldLong;
			fieldLongInterpolation.target = _fieldLong;
			RunChange_fieldLong(timestep);
			_fieldULong = UnityObjectMapper.Instance.Map<ulong>(payload);
			fieldULongInterpolation.current = _fieldULong;
			fieldULongInterpolation.target = _fieldULong;
			RunChange_fieldULong(timestep);
			_fieldDouble = UnityObjectMapper.Instance.Map<double>(payload);
			fieldDoubleInterpolation.current = _fieldDouble;
			fieldDoubleInterpolation.target = _fieldDouble;
			RunChange_fieldDouble(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _fieldByte);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _fieldChar);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _fieldShort);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _fieldUShort);
			if ((0x10 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _fieldBool);
			if ((0x20 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _fieldInt);
			if ((0x40 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _fieldUInt);
			if ((0x80 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _fieldFloat);
			if ((0x1 & _dirtyFields[1]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _fieldFloatInterpolate);
			if ((0x2 & _dirtyFields[1]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _fieldLong);
			if ((0x4 & _dirtyFields[1]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _fieldULong);
			if ((0x8 & _dirtyFields[1]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _fieldDouble);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

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
				if (fieldByteInterpolation.Enabled)
				{
					fieldByteInterpolation.target = UnityObjectMapper.Instance.Map<byte>(data);
					fieldByteInterpolation.Timestep = timestep;
				}
				else
				{
					_fieldByte = UnityObjectMapper.Instance.Map<byte>(data);
					RunChange_fieldByte(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (fieldCharInterpolation.Enabled)
				{
					fieldCharInterpolation.target = UnityObjectMapper.Instance.Map<char>(data);
					fieldCharInterpolation.Timestep = timestep;
				}
				else
				{
					_fieldChar = UnityObjectMapper.Instance.Map<char>(data);
					RunChange_fieldChar(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (fieldShortInterpolation.Enabled)
				{
					fieldShortInterpolation.target = UnityObjectMapper.Instance.Map<short>(data);
					fieldShortInterpolation.Timestep = timestep;
				}
				else
				{
					_fieldShort = UnityObjectMapper.Instance.Map<short>(data);
					RunChange_fieldShort(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (fieldUShortInterpolation.Enabled)
				{
					fieldUShortInterpolation.target = UnityObjectMapper.Instance.Map<ushort>(data);
					fieldUShortInterpolation.Timestep = timestep;
				}
				else
				{
					_fieldUShort = UnityObjectMapper.Instance.Map<ushort>(data);
					RunChange_fieldUShort(timestep);
				}
			}
			if ((0x10 & readDirtyFlags[0]) != 0)
			{
				if (fieldBoolInterpolation.Enabled)
				{
					fieldBoolInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					fieldBoolInterpolation.Timestep = timestep;
				}
				else
				{
					_fieldBool = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_fieldBool(timestep);
				}
			}
			if ((0x20 & readDirtyFlags[0]) != 0)
			{
				if (fieldIntInterpolation.Enabled)
				{
					fieldIntInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					fieldIntInterpolation.Timestep = timestep;
				}
				else
				{
					_fieldInt = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_fieldInt(timestep);
				}
			}
			if ((0x40 & readDirtyFlags[0]) != 0)
			{
				if (fieldUIntInterpolation.Enabled)
				{
					fieldUIntInterpolation.target = UnityObjectMapper.Instance.Map<uint>(data);
					fieldUIntInterpolation.Timestep = timestep;
				}
				else
				{
					_fieldUInt = UnityObjectMapper.Instance.Map<uint>(data);
					RunChange_fieldUInt(timestep);
				}
			}
			if ((0x80 & readDirtyFlags[0]) != 0)
			{
				if (fieldFloatInterpolation.Enabled)
				{
					fieldFloatInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					fieldFloatInterpolation.Timestep = timestep;
				}
				else
				{
					_fieldFloat = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_fieldFloat(timestep);
				}
			}
			if ((0x1 & readDirtyFlags[1]) != 0)
			{
				if (fieldFloatInterpolateInterpolation.Enabled)
				{
					fieldFloatInterpolateInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					fieldFloatInterpolateInterpolation.Timestep = timestep;
				}
				else
				{
					_fieldFloatInterpolate = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_fieldFloatInterpolate(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[1]) != 0)
			{
				if (fieldLongInterpolation.Enabled)
				{
					fieldLongInterpolation.target = UnityObjectMapper.Instance.Map<long>(data);
					fieldLongInterpolation.Timestep = timestep;
				}
				else
				{
					_fieldLong = UnityObjectMapper.Instance.Map<long>(data);
					RunChange_fieldLong(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[1]) != 0)
			{
				if (fieldULongInterpolation.Enabled)
				{
					fieldULongInterpolation.target = UnityObjectMapper.Instance.Map<ulong>(data);
					fieldULongInterpolation.Timestep = timestep;
				}
				else
				{
					_fieldULong = UnityObjectMapper.Instance.Map<ulong>(data);
					RunChange_fieldULong(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[1]) != 0)
			{
				if (fieldDoubleInterpolation.Enabled)
				{
					fieldDoubleInterpolation.target = UnityObjectMapper.Instance.Map<double>(data);
					fieldDoubleInterpolation.Timestep = timestep;
				}
				else
				{
					_fieldDouble = UnityObjectMapper.Instance.Map<double>(data);
					RunChange_fieldDouble(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (fieldByteInterpolation.Enabled && !fieldByteInterpolation.current.UnityNear(fieldByteInterpolation.target, 0.0015f))
			{
				_fieldByte = (byte)fieldByteInterpolation.Interpolate();
				//RunChange_fieldByte(fieldByteInterpolation.Timestep);
			}
			if (fieldCharInterpolation.Enabled && !fieldCharInterpolation.current.UnityNear(fieldCharInterpolation.target, 0.0015f))
			{
				_fieldChar = (char)fieldCharInterpolation.Interpolate();
				//RunChange_fieldChar(fieldCharInterpolation.Timestep);
			}
			if (fieldShortInterpolation.Enabled && !fieldShortInterpolation.current.UnityNear(fieldShortInterpolation.target, 0.0015f))
			{
				_fieldShort = (short)fieldShortInterpolation.Interpolate();
				//RunChange_fieldShort(fieldShortInterpolation.Timestep);
			}
			if (fieldUShortInterpolation.Enabled && !fieldUShortInterpolation.current.UnityNear(fieldUShortInterpolation.target, 0.0015f))
			{
				_fieldUShort = (ushort)fieldUShortInterpolation.Interpolate();
				//RunChange_fieldUShort(fieldUShortInterpolation.Timestep);
			}
			if (fieldBoolInterpolation.Enabled && !fieldBoolInterpolation.current.UnityNear(fieldBoolInterpolation.target, 0.0015f))
			{
				_fieldBool = (bool)fieldBoolInterpolation.Interpolate();
				//RunChange_fieldBool(fieldBoolInterpolation.Timestep);
			}
			if (fieldIntInterpolation.Enabled && !fieldIntInterpolation.current.UnityNear(fieldIntInterpolation.target, 0.0015f))
			{
				_fieldInt = (int)fieldIntInterpolation.Interpolate();
				//RunChange_fieldInt(fieldIntInterpolation.Timestep);
			}
			if (fieldUIntInterpolation.Enabled && !fieldUIntInterpolation.current.UnityNear(fieldUIntInterpolation.target, 0.0015f))
			{
				_fieldUInt = (uint)fieldUIntInterpolation.Interpolate();
				//RunChange_fieldUInt(fieldUIntInterpolation.Timestep);
			}
			if (fieldFloatInterpolation.Enabled && !fieldFloatInterpolation.current.UnityNear(fieldFloatInterpolation.target, 0.0015f))
			{
				_fieldFloat = (float)fieldFloatInterpolation.Interpolate();
				//RunChange_fieldFloat(fieldFloatInterpolation.Timestep);
			}
			if (fieldFloatInterpolateInterpolation.Enabled && !fieldFloatInterpolateInterpolation.current.UnityNear(fieldFloatInterpolateInterpolation.target, 0.0015f))
			{
				_fieldFloatInterpolate = (float)fieldFloatInterpolateInterpolation.Interpolate();
				//RunChange_fieldFloatInterpolate(fieldFloatInterpolateInterpolation.Timestep);
			}
			if (fieldLongInterpolation.Enabled && !fieldLongInterpolation.current.UnityNear(fieldLongInterpolation.target, 0.0015f))
			{
				_fieldLong = (long)fieldLongInterpolation.Interpolate();
				//RunChange_fieldLong(fieldLongInterpolation.Timestep);
			}
			if (fieldULongInterpolation.Enabled && !fieldULongInterpolation.current.UnityNear(fieldULongInterpolation.target, 0.0015f))
			{
				_fieldULong = (ulong)fieldULongInterpolation.Interpolate();
				//RunChange_fieldULong(fieldULongInterpolation.Timestep);
			}
			if (fieldDoubleInterpolation.Enabled && !fieldDoubleInterpolation.current.UnityNear(fieldDoubleInterpolation.target, 0.0015f))
			{
				_fieldDouble = (double)fieldDoubleInterpolation.Interpolate();
				//RunChange_fieldDouble(fieldDoubleInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[2];

		}

		public TestNetworkObject() : base() { Initialize(); }
		public TestNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public TestNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
