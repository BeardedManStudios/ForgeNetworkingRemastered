namespace BeardedManStudios.Forge.Networking.SQP
{
	public interface ISQPMessage
	{
		void Serialize(ref BMSByte buffer);
		void Deserialize(BMSByte buffer);
	}
}
