using System;

namespace BeardedManStudios.Source.Threading
{
    public interface IThreadRunner
    {
        void Execute(Action action);
    }
}
