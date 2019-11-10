using System;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public class ForgeMessageConstructor : IMessageConstructor
	{
		public bool MessageReconstructed { get; private set; } = false;
		public BMSByte MessageBuffer { get; private set; } = new BMSByte();
		private byte[][] _pages;

		public void ReconstructMessagePage(BMSByte page)
		{
			if (MessageReconstructed)
				return;

			// TODO:  Throw or catch when page.Size == 0

			// TODO:  Make sure the guid we have matches this (probably done elsewhere)
			var guid = Guid.Parse(page.GetBasicType<string>());

			var pageNumber = page.GetBasicType<int>();
			var totalSize = page.GetBasicType<int>();

			// TODO:  Throw or catch when page.Size == 0

			if (page.Size + page.StartPointer == totalSize)
			{
				MessageBuffer.Append(page);
				MessageReconstructed = true;
			}
			else
			{
				if (_pages == null)
				{
					double actualSize = page.StartPointer + page.Size;
					MessageBuffer.SetArraySize(totalSize);
					_pages = new byte[(int)Math.Ceiling(totalSize / actualSize)][];
				}
				if (_pages[pageNumber] == null)
					_pages[pageNumber] = page.CompressBytes();
				if (IsDone())
				{
					foreach (var p in _pages)
						MessageBuffer.Append(p);
					MessageReconstructed = true;
				}
			}
		}

		private bool IsDone()
		{
			foreach (var p in _pages)
			{
				if (p == null)
				{
					return false;
				}
			}
			return true;
		}
	}
}
