namespace BeardedManStudios.Forge.Networking
{
	/// <summary>
	/// Represents the context of a request.
	/// </summary>
	public class RequestContext<T>
	{
		private readonly NetworkingPlayer sender;
		public NetworkingPlayer Sender { get => sender; }

		private readonly T data;
		public T Data { get => data; }

		public RequestContext(NetworkingPlayer sender, T data)
		{
			this.sender = sender;
			this.data = data;
		}
	}
}
