namespace BeardedManStudios.Forge.Networking
{
	public interface IInterpolator<T>
	{
		float LerpT { get; set; }
		bool Enabled { get; set; }
		ulong Timestep { get; set; }

		T Interpolate();
	}
}
