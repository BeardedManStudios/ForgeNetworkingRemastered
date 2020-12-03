using BeardedManStudios.Forge.Networking;

namespace BeardedManStudios
{
    public struct InterpolateFloat4 : IInterpolator<Float4>
    {
        public Float4 current;
        public Float4 target;
        public float LerpT { get; set; }
        public bool Enabled { get; set; }
        public ulong Timestep { get; set; }

        public Float4 Interpolate()
        {
            if (!Enabled) return target;

            current = Float4.Lerp(current, target, LerpT);
            return current;
        }
    }
}