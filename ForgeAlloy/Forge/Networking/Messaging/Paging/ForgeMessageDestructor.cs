using System;
using Forge.Factory;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public class ForgeMessageDestructor : IMessageDestructor
	{
		public int HeaderLength => GetHeader(new Guid(), 0, 1).Size;
		public int MaxPageLength => 1000;

		public IPagenatedMessage BreakdownMessage(BMSByte messageBuffer)
		{
			if (messageBuffer.Size == 0)
				throw new CantBreakdownEmptyMessageException();
			int offset = 0;
			var pm = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPagenatedMessage>();
			pm.Buffer = messageBuffer;

			var messageGuid = Guid.NewGuid();
			int pageCount = ((messageBuffer.Size - 1) / MaxPageLength) + 1;
			int totalSize = messageBuffer.Size + (pageCount * HeaderLength);
			messageBuffer.SetArraySize(totalSize);
			do
			{
				BMSByte header = GetHeader(messageGuid, pm.Pages.Count, totalSize);
				var page = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessagePage>();
				page.StartOffset = offset;
				page.Length = Math.Min(messageBuffer.Size - offset, MaxPageLength) + header.Size;
				messageBuffer.InsertRange(offset, header);
				pm.Pages.Add(page);
				offset += page.Length;
			} while (offset < messageBuffer.Size);
			return pm;
		}

		private BMSByte GetHeader(Guid messageGuid, int pageNumber, int totalSize)
		{
			var header = new BMSByte();
			ForgeSerializationStrategy.Instance.Serialize(messageGuid.ToString(), header);
			ForgeSerializationStrategy.Instance.Serialize(pageNumber, header);
			ForgeSerializationStrategy.Instance.Serialize(totalSize, header);
			return header;
		}
	}
}
