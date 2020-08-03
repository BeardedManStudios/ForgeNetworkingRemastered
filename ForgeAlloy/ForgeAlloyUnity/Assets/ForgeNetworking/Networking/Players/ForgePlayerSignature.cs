using Forge.DataStructures;

namespace Forge.Networking.Players
{
	public class ForgePlayerSignature : ForgeSignature, IPlayerSignature
	{
		public ForgePlayerSignature(ISignatureGenerator<int> generator)
			: base(generator)
		{

		}
	}
}
