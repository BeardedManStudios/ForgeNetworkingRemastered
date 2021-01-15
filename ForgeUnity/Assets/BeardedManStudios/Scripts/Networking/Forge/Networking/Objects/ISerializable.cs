namespace BeardedManStudios.Forge.Networking
{
	/// <summary>
	/// Represents the context of a request.
	/// </summary>
	public interface ISerializable
	{
		void Serialize(BMSByte buffer);
		void Unserialize(BMSByte buffer);
   }
}
