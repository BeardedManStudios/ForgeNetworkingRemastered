using BeardedManStudios.Forge.Networking;

namespace BeardedManStudios
{
    public struct InterpolateFloat3 : IInterpolator<Float3>
    {
        public Float3 current;
        public Float3 target;
        public float LerpT { get; set; }
        public bool Enabled { get; set; }
        public ulong Timestep { get; set; }

        public Float3 Interpolate()
        {
            if (!Enabled) return target;

            current = Float3.Lerp(current, target, LerpT);
            return current;
        }
    }
}