namespace Forge.DataStructures
{
	public interface ISignature
	{
		byte[] Serialize();
		void Deserialize(byte[] data);
	}
}
