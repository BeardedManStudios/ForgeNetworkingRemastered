using Forge.Serialization;

namespace Forge.DataStructures
{
	public interface ISignature
	{
		void Serialize(BMSByte buffer);
		void Deserialize(BMSByte buffer);
	}
}
