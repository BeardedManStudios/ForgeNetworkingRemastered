using System.Threading;

namespace Forge.DataStructures
{
	public class ForgeSignatureIdGenerator : ISignatureGenerator<int>
	{
		private int _num = 0;

		public int Generate()
		{
			return Interlocked.Increment(ref _num);
		}
	}
}
