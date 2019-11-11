using System;
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
			var pm = ForgeTypeFactory.GetNew<IPagenatedMessage>();
			pm.Buffer = messageBuffer;

			var messageGuid = Guid.NewGuid();
			int pageCount = ((messageBuffer.Size - 1) / MaxPageLength) + 1;
			int totalSize = messageBuffer.Size + (pageCount * HeaderLength);
			messageBuffer.SetArraySize(totalSize);
			do
			{
				BMSByte header = GetHeader(messageGuid, pm.Pages.Count, totalSize);
				var page = ForgeTypeFactory.GetNew<IMessagePage>();
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
			header.Append(
				ForgeSerializationContainer.Instance.Serialize(messageGuid.ToString()),
				ForgeSerializationContainer.Instance.Serialize(pageNumber),
				ForgeSerializationContainer.Instance.Serialize(totalSize)
			);
			return header;
		}
	}
}
