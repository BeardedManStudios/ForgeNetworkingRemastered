using BeardedManStudios.Forge.Networking.Frame;
using System;

namespace BeardedManStudios.Forge.Networking.Generated
{
    public partial class BasicCubeNetworkObject : NetworkObject
    {
        public const int IDENTITY = 6;

        private byte[] _dirtyFields = new byte[1];

#pragma warning disable 0067
        public event FieldChangedEvent fieldAltered;
#pragma warning restore 0067
        private float _positionX;
        public event FieldEvent<float> positionXChanged;
        public InterpolateFloat positionXInterpolation = new InterpolateFloat() { LerpT = 0f, Enabled = false };
        public float positionX
        {
            get { return _positionX; }
            set
            {
                // Don't do anything if the value is the same
                if (_positionX == value)
                    return;

                // Mark the field as dirty for the network to transmit
                _dirtyFields[0] |= 0x1;
                _positionX = value;
                hasDirtyFields = true;
            }
        }

        public void SetpositionXDirty()
        {
            _dirtyFields[0] |= 0x1;
            hasDirtyFields = true;
        }

        private void RunChange_positionX(ulong timestep)
        {
            if (positionXChanged != null) positionXChanged(_positionX, timestep);
            if (fieldAltered != null) fieldAltered("positionX", _positionX, timestep);
        }
        private float _positionY;
        public event FieldEvent<float> positionYChanged;
        public InterpolateFloat positionYInterpolation = new InterpolateFloat() { LerpT = 0f, Enabled = false };
        public float positionY
        {
            get { return _positionY; }
            set
            {
                // Don't do anything if the value is the same
                if (_positionY == value)
                    return;

                // Mark the field as dirty for the network to transmit
                _dirtyFields[0] |= 0x2;
                _positionY = value;
                hasDirtyFields = true;
            }
        }

        public void SetpositionYDirty()
        {
            _dirtyFields[0] |= 0x2;
            hasDirtyFields = true;
        }

        private void RunChange_positionY(ulong timestep)
        {
            if (positionYChanged != null) positionYChanged(_positionY, timestep);
            if (fieldAltered != null) fieldAltered("positionY", _positionY, timestep);
        }
        private float _positionZ;
        public event FieldEvent<float> positionZChanged;
        public InterpolateFloat positionZInterpolation = new InterpolateFloat() { LerpT = 0f, Enabled = false };
        public float positionZ
        {
            get { return _positionZ; }
            set
            {
                // Don't do anything if the value is the same
                if (_positionZ == value)
                    return;

                // Mark the field as dirty for the network to transmit
                _dirtyFields[0] |= 0x4;
                _positionZ = value;
                hasDirtyFields = true;
            }
        }

        public void SetpositionZDirty()
        {
            _dirtyFields[0] |= 0x4;
            hasDirtyFields = true;
        }

        private void RunChange_positionZ(ulong timestep)
        {
            if (positionZChanged != null) positionZChanged(_positionZ, timestep);
            if (fieldAltered != null) fieldAltered("positionZ", _positionZ, timestep);
        }

        protected override void OwnershipChanged()
        {
            base.OwnershipChanged();
            SnapInterpolations();
        }

        public void SnapInterpolations()
        {
            positionXInterpolation.current = positionXInterpolation.target;
            positionYInterpolation.current = positionYInterpolation.target;
            positionZInterpolation.current = positionZInterpolation.target;
        }

        public override int UniqueIdentity { get { return IDENTITY; } }

        protected override BMSByte WritePayload(BMSByte data)
        {
            ObjectMapper.Instance.MapBytes(data, _positionX);
            ObjectMapper.Instance.MapBytes(data, _positionY);
            ObjectMapper.Instance.MapBytes(data, _positionZ);

            return data;
        }

        protected override void ReadPayload(BMSByte payload, ulong timestep)
        {
            _positionX = ObjectMapper.Instance.Map<float>(payload);
            positionXInterpolation.current = _positionX;
            positionXInterpolation.target = _positionX;
            RunChange_positionX(timestep);
            _positionY = ObjectMapper.Instance.Map<float>(payload);
            positionYInterpolation.current = _positionY;
            positionYInterpolation.target = _positionY;
            RunChange_positionY(timestep);
            _positionZ = ObjectMapper.Instance.Map<float>(payload);
            positionZInterpolation.current = _positionZ;
            positionZInterpolation.target = _positionZ;
            RunChange_positionZ(timestep);
        }

        protected override BMSByte SerializeDirtyFields()
        {
            dirtyFieldsData.Clear();
            dirtyFieldsData.Append(_dirtyFields);

            if ((0x1 & _dirtyFields[0]) != 0)
                ObjectMapper.Instance.MapBytes(dirtyFieldsData, _positionX);
            if ((0x2 & _dirtyFields[0]) != 0)
                ObjectMapper.Instance.MapBytes(dirtyFieldsData, _positionY);
            if ((0x4 & _dirtyFields[0]) != 0)
                ObjectMapper.Instance.MapBytes(dirtyFieldsData, _positionZ);

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
                if (positionXInterpolation.Enabled)
                {
                    positionXInterpolation.target = ObjectMapper.Instance.Map<float>(data);
                    positionXInterpolation.Timestep = timestep;
                }
                else
                {
                    _positionX = ObjectMapper.Instance.Map<float>(data);
                    RunChange_positionX(timestep);
                }
            }
            if ((0x2 & readDirtyFlags[0]) != 0)
            {
                if (positionYInterpolation.Enabled)
                {
                    positionYInterpolation.target = ObjectMapper.Instance.Map<float>(data);
                    positionYInterpolation.Timestep = timestep;
                }
                else
                {
                    _positionY = ObjectMapper.Instance.Map<float>(data);
                    RunChange_positionY(timestep);
                }
            }
            if ((0x4 & readDirtyFlags[0]) != 0)
            {
                if (positionZInterpolation.Enabled)
                {
                    positionZInterpolation.target = ObjectMapper.Instance.Map<float>(data);
                    positionZInterpolation.Timestep = timestep;
                }
                else
                {
                    _positionZ = ObjectMapper.Instance.Map<float>(data);
                    RunChange_positionZ(timestep);
                }
            }
        }

        public override void InterpolateUpdate()
        {
            if (IsOwner)
                return;

            if (positionXInterpolation.Enabled && !positionXInterpolation.current.Near(positionXInterpolation.target, 0.0015f))
            {
                _positionX = (float)positionXInterpolation.Interpolate();
                //RunChange_positionX(positionXInterpolation.Timestep);
            }
            if (positionYInterpolation.Enabled && !positionYInterpolation.current.Near(positionYInterpolation.target, 0.0015f))
            {
                _positionY = (float)positionYInterpolation.Interpolate();
                //RunChange_positionY(positionYInterpolation.Timestep);
            }
            if (positionZInterpolation.Enabled && !positionZInterpolation.current.Near(positionZInterpolation.target, 0.0015f))
            {
                _positionZ = (float)positionZInterpolation.Interpolate();
                //RunChange_positionZ(positionZInterpolation.Timestep);
            }
        }

        private void Initialize()
        {
            if (readDirtyFlags == null)
                readDirtyFlags = new byte[1];

        }

        public BasicCubeNetworkObject() : base() { Initialize(); }
        public BasicCubeNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
        public BasicCubeNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

        // DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
    }
}
