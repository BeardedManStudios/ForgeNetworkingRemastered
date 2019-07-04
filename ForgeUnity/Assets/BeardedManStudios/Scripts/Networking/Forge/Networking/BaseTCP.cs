using System.Net.Sockets;
using System;

namespace BeardedManStudios.Forge.Networking
{
    // new BaseTCP as of September 2018
    public abstract class BaseTCP : NetWorker
    {
        protected BaseTCP() : base() { }
        protected BaseTCP(int maxConnections) : base(maxConnections) { }

        protected struct ReceiveToken
        {
            public NetworkingPlayer player;
            public int maxAllowedBytes;
            public int bytesReceived;
            public byte[] dataHolder;
            public ArraySegment<byte> internalBuffer;
        }

        private static readonly byte[] HEADER_DELIM = { 13, 10, 13, 10 }; // UTF-8 for \r\n\r\n i.e. CRLF CRLF, which appears after the last HTTP header field

        protected byte[] HandleHttpHeader(SocketAsyncEventArgs e, ref int bytesAlreadyProcessed)
        {
            if (bytesAlreadyProcessed < 0)
                throw new ArgumentException("bytesAlreadyProcessed must be non-negative.");
            else if (bytesAlreadyProcessed >= e.BytesTransferred)
                throw new ArgumentException("bytesAlreadyProcessed must be less than e.BytesTransferred.");

            ReceiveToken token = (ReceiveToken)e.UserToken;
            int totalBytes = token.bytesReceived + e.BytesTransferred - bytesAlreadyProcessed;
            if (totalBytes >= 4)
            {
                int searchSpace = totalBytes - 3; // totalBytes - HEADER_DELIM.Length + 1
                byte[] data = token.internalBuffer.Array;
                for (int i = 0; i < searchSpace; i++)
                {
                    if (HEADER_DELIM[0] == data[token.internalBuffer.Offset + i])
                    {
                        if (HEADER_DELIM[1] == data[token.internalBuffer.Offset + i + 1] &&
                            HEADER_DELIM[2] == data[token.internalBuffer.Offset + i + 2] &&
                            HEADER_DELIM[3] == data[token.internalBuffer.Offset + i + 3])
                        {
                            byte[] header = new byte[i + 4];
                            Buffer.BlockCopy(data, token.internalBuffer.Offset, header, 0, header.Length);
                            if (token.bytesReceived > 0)
                            {
                                bytesAlreadyProcessed += header.Length - token.bytesReceived;
                                token.bytesReceived = 0;
                                e.SetBuffer(token.internalBuffer.Offset, token.internalBuffer.Count);
                            }
                            else
                            {
                                bytesAlreadyProcessed += header.Length;
                            }
                            if (header.Length < totalBytes)
                            {
                                Buffer.BlockCopy(data, token.internalBuffer.Offset + header.Length, data, token.internalBuffer.Offset, totalBytes - header.Length);
                            }
                            e.UserToken = token;
                            return header;
                        }
                    }
                }
                if (totalBytes == token.internalBuffer.Count)
                    throw new Exception(String.Format("Could not find end of header within {0} bytes.", token.internalBuffer.Count));
            }

            token.bytesReceived = totalBytes;
            e.SetBuffer(token.internalBuffer.Offset + totalBytes, token.internalBuffer.Count - totalBytes);
            bytesAlreadyProcessed = e.BytesTransferred;
            e.UserToken = token;
            return null;
        }

        protected byte[] HandleData(SocketAsyncEventArgs e, bool isStream, ref int bytesAlreadyProcessed)
        {
            if (bytesAlreadyProcessed < 0)
                throw new ArgumentException("bytesAlreadyProcessed must be non-negative.");
            else if (bytesAlreadyProcessed >= e.BytesTransferred)
                throw new ArgumentException("bytesAlreadyProcessed must be less than e.BytesTransferred.");

            ReceiveToken token = (ReceiveToken)e.UserToken;
            int socketOffset = token.internalBuffer.Offset;
            byte[] bytes = token.internalBuffer.Array;
            int totalBytes;

            #region ParseFrameHeader
            if (token.dataHolder == null) // Null dataHolder means header not parsed yet
            {
                totalBytes = token.bytesReceived + e.BytesTransferred - bytesAlreadyProcessed;
                if (totalBytes < 2) // Not enough bytes to determine length, so move offset
                {
                    token.bytesReceived = totalBytes;
                    e.SetBuffer(socketOffset + totalBytes, token.internalBuffer.Count - totalBytes);
                    bytesAlreadyProcessed = e.BytesTransferred;
                    e.UserToken = token;
                    return null;
                }
                int dataLength = bytes[socketOffset + 1] & 127;
                bool usingMask = bytes[socketOffset + 1] > 127; // same as bytes[socketOffset + 1] & 128 != 0
                int payloadOffset;
                if (dataLength == 126) // 126 means 125 < length < 65536
                {
                    payloadOffset = 4;
                }
                else if (dataLength == 127)  // 127 means length >= 65536
                {
                    payloadOffset = 10;
                }
                else
                {
                    payloadOffset = 2;
                }
                int length;
                if (payloadOffset != 2)
                {
                    if (totalBytes < payloadOffset)  // Not enough bytes to determine length, so move offset
                    {
                        token.bytesReceived = totalBytes;
                        e.SetBuffer(socketOffset + totalBytes, token.internalBuffer.Count - totalBytes);
                        bytesAlreadyProcessed = e.BytesTransferred;
                        e.UserToken = token;
                        return null;
                    }

                    // Need to worry about endian order since length is in big endian
                    if (payloadOffset == 4)
                    {
                        if (BitConverter.IsLittleEndian)
                            length = BitConverter.ToUInt16(new byte[] { bytes[socketOffset + 3], bytes[socketOffset + 2] }, 0);
                        else
                            length = BitConverter.ToUInt16(bytes, socketOffset + 2);

                    }
                    else
                    {
                        // First 4 bytes will be 0 for sizes less than max int32
                        if (BitConverter.IsLittleEndian)
                            length = (int)BitConverter.ToUInt32(new byte[] { bytes[socketOffset + 9], bytes[socketOffset + 8], bytes[socketOffset + 7], bytes[socketOffset + 6] }, 0);
                        else
                            length = (int)BitConverter.ToUInt32(bytes, socketOffset + 6);
                    }

                    // Group id, receivers, time step, unique id, and router id lengths will be present in the payload length

                    //if (isStream)
                    //    length += 21;  // Group id (4), receivers (1), time step (8), unique id (8)
                    //if ((bytes[socketOffset + 0] & 0xF) == (Binary.CONTROL_BYTE & 0xF))
                    //    length += 1; // routerId (1) if this is a binary frame
                }
                else
                {
                    length = dataLength;
                }
                length += usingMask ? 4 + payloadOffset : payloadOffset; // Add full length of header to length
                if (length < 0 || length > token.maxAllowedBytes)
                    throw new UnauthorizedAccessException(String.Format("Tried to receive a frame larger than expected. Got {0}, and expected less than {1}", length, token.maxAllowedBytes));
                e.SetBuffer(token.internalBuffer.Offset, token.internalBuffer.Count);
                token.bytesReceived = 0;
                token.dataHolder = new byte[length];
            }
            #endregion

            totalBytes = token.bytesReceived + e.BytesTransferred - bytesAlreadyProcessed;
            if (totalBytes < token.dataHolder.Length) // Full frame not yet received
            {
                Buffer.BlockCopy(bytes, socketOffset, token.dataHolder, token.bytesReceived, e.BytesTransferred - bytesAlreadyProcessed);
                token.bytesReceived += e.BytesTransferred - bytesAlreadyProcessed;
                bytesAlreadyProcessed = e.BytesTransferred;
                e.UserToken = token;
                return null;
            }
            else
            {
                byte[] data = token.dataHolder;
                int dataProcessed = (data.Length - token.bytesReceived);
                Buffer.BlockCopy(bytes, socketOffset, data, token.bytesReceived, dataProcessed);


                token.bytesReceived = 0;
                token.dataHolder = null;
                bytesAlreadyProcessed += dataProcessed;
                if (bytesAlreadyProcessed < e.BytesTransferred) // More frames left to handle, so shuffle the remaining data to beginning of buffer.
                {
                    Buffer.BlockCopy(bytes, socketOffset + dataProcessed, bytes, socketOffset, e.BytesTransferred - bytesAlreadyProcessed);
                }
                e.UserToken = token;
                return data;
            }
        }

    }
}
