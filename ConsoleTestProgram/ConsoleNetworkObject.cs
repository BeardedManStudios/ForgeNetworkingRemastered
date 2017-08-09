using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using System;

namespace ConsoleTestProgram
{
    public class ConsoleNetworkObject : NetworkObject
    {
        public const int IDENTITY = 1;
        private const int FIELD_COUNT = 1;

        // TODO:  Can put hard coded number in for size which is Math.Ceil(FIELD_COUNT / 8)
        private byte[] dirtyFields = new byte[1];

        private int num = 0;
        public int Num
        {
            get { return num; }
            set
            {
                // Don't do anything if the value is the same
                if (num == value)
                    return;

                // Mark the field as dirty for the network to transmit
                dirtyFields[0] |= 0x1;
                num = value;
                hasDirtyFields = true;
            }
        }

        public override int UniqueIdentity { get { return IDENTITY; } }

        protected override BMSByte WritePayload(BMSByte data)
        {
            ObjectMapper.Instance.MapBytes(data, num);
            return data;
        }

        protected override void ReadPayload(BMSByte payload, ulong timestep)
        {
            num = payload.GetBasicType<int>();
        }

        protected override BMSByte SerializeDirtyFields()
        {
            if (dirtyFields.Length == 0 || !hasDirtyFields)
                return null;

            // TODO:  Check if is owner when calling this method
            BMSByte data = new BMSByte();
            data.Append(dirtyFields);

            // Check to see if the field is dirty
            if ((0x1 & dirtyFields[0]) != 0)
                ObjectMapper.Instance.MapBytes(data, num);

            // Reset all of the dirty fields flags
            for (int i = 0; i < dirtyFields.Length; i++)
                dirtyFields[i] = 0;

            hasDirtyFields = false;

            return data;
        }

        protected override void ReadDirtyFields(BMSByte data, ulong timestep)
        {
            byte[] readFlags = new byte[dirtyFields.Length];
            Buffer.BlockCopy(data.byteArr, data.StartIndex(), readFlags, 0, readFlags.Length);
            data.MoveStartIndex(readFlags.Length);

            // Check to see if the field was sent as dirty
            if ((0x1 & readFlags[0]) != 0)
                num = data.GetBasicType<int>();

            Console.WriteLine("Read data and num is now " + Num);
        }

        public ConsoleNetworkObject(NetWorker networker) : base(networker) { }
        public ConsoleNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { }

        // DO NOT TOUCH, THIS GETS GENERATED
    }
}