using BeardedManStudios.Concurrency;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BeardedManStudios
{
    public class BufferManager
    {
        public readonly int bufferSize;
        public readonly int bufferPageSize;
        public readonly int maxBufferPages;
        private readonly object @lock = new object();

        public long MaxBufferPoolSize { get { return bufferSize * (long)bufferPageSize * maxBufferPages; } }

        private readonly ConcurrentQueue<ArraySegment<byte>> POOLED_BUFFERS = new ConcurrentQueue<ArraySegment<byte>>();
        private readonly byte[][] PAGES;

        public BufferManager(int buffersSize = 4096, int bufferPageSize = 256, int maxBufferPages = 128)
        {
            if (buffersSize < 1 || bufferPageSize < 1 || maxBufferPages < 1)
                throw new ArgumentOutOfRangeException("Arguments must be positive.");
            this.bufferSize = buffersSize;
            this.bufferPageSize = bufferPageSize;
            this.maxBufferPages = maxBufferPages;
            PAGES = new byte[maxBufferPages][];
            AddPage();
        }

        private int BinarySearchForFirstNull(Array array, int start, int end)
        {
            // No match
            if (start == end)
                return -1;

            int midpoint = (end + start) / 2;

            // Not null, so no match
            if (array.GetValue(midpoint) != null)
                return BinarySearchForFirstNull(array, midpoint + 1, end);

            // Match
            if ((midpoint == 0) || (array.GetValue(midpoint - 1) != null))
                return midpoint;

            // Not first null, so no match
            return BinarySearchForFirstNull(array, start, midpoint);
        }

        private bool AddPage()
        {

            lock (@lock)
            {
                if (POOLED_BUFFERS.Count > 0)
                {
                    return true;
                }
                int index = BinarySearchForFirstNull(PAGES, 0, PAGES.Length);
                if (index < 0)
                    return false; // PAGES is full
                PAGES[index] = new byte[bufferSize * bufferPageSize];
                for (int i = 0; i < bufferPageSize; i++)
                    POOLED_BUFFERS.Enqueue(new ArraySegment<byte>(PAGES[index], i * bufferSize, bufferSize));
            }
            
            return true;
        }

        public bool TryTakeBuffer(out ArraySegment<byte> buffer)
        {
            do
            {
                if (POOLED_BUFFERS.TryDequeue(out buffer))
                    return true;
            } while (AddPage());

            return false;
        }

        public void ReturnBuffer(ArraySegment<byte> buffer)
        {
            POOLED_BUFFERS.Enqueue(buffer);
        }
    }
}
