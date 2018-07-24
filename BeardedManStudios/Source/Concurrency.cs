using System;
using System.Collections.Generic;
using System.Threading;

namespace BeardedManStudios.Concurrency
{
    // Locking for quick operations. No need to expensively block a thread.
    internal class SpinLock
    {
        private int @lock = 0;
        public void Enter(ref bool lockTaken)
        {
            if (lockTaken)
                throw new ArgumentException("Lock was already taken.");
            while (1 == Interlocked.CompareExchange(ref @lock, 1, 0))
            {
                while (@lock == 1) ;
            }
            lockTaken = true;
        }

        public void Exit()
        {
            Interlocked.CompareExchange(ref @lock, 0, 1);
        }
    }

    internal class ConcurrentQueue<T> : Queue<T>
    {
        private readonly SpinLock @lock = new SpinLock();

        public bool TryDequeue(out T result)
        {
            bool lockTaken = false;
            try
            {
                @lock.Enter(ref lockTaken);
                if (Count > 0)
                {
                    result = Dequeue();
                    return true;
                } else
                {
                    result = default(T);
                    return false;
                }
            } finally
            {
                if (lockTaken) @lock.Exit();
            }
        }

        public new void Enqueue(T item)
        {
            bool lockTaken = false;
            try
            {
                @lock.Enter(ref lockTaken);
                base.Enqueue(item);
            } finally
            {
                if (lockTaken) @lock.Exit();
            }
        }
    }
}
