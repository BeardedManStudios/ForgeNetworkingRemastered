using System;
using Forge.Factory;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public class ForgeMessageDestructor : IMessageDestructor
	{
		private static int _num = 0;
		private static object _numMutex = new object();

		private static int NextId()
		{
			lock (_numMutex)
			{
				return _num++;
			}
		}

		public int HeaderLength => sizeof(int) * 3;
		public int MaxPageLength => 1000;
		public BMSBytePool BufferPool { private get; set; } = new BMSBytePool();

		public IPagenatedMessage BreakdownMessage(BMSByte messageBuffer)
		{
			if (messageBuffer.Size == 0)
				throw new CantBreakdownEmptyMessageException();
			int offset = 0;
			var pm = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPagenatedMessage>();
			pm.Buffer = messageBuffer;

			int messageId = NextId();
			int pageCount = ((messageBuffer.Size - 1) / MaxPageLength) + 1;
			int totalSize = messageBuffer.Size + (pageCount * HeaderLength);
			messageBuffer.SetArraySize(totalSize);
			do
			{
				BMSByte header = GetHeader(messageId, pm.Pages.Count, totalSize);
				var page = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessagePage>();
				page.StartOffset = offset;
				page.Length = System.Math.Min(messageBuffer.Size - offset, MaxPageLength) + header.Size;
				messageBuffer.InsertRange(offset, header);
				pm.Pages.Add(page);
				offset += page.Length;
				BufferPool.Release(header);
			} while (offset < messageBuffer.Size);
			return pm;
		}

		private BMSByte GetHeader(int messageId, int pageNumber, int totalSize)
		{
			BMSByte header = BufferPool.Get(HeaderLength);
			ForgeSerializer.Instance.Serialize(messageId, header);
			ForgeSerializer.Instance.Serialize(pageNumber, header);
			ForgeSerializer.Instance.Serialize(totalSize, header);
			return header;
		}
	}
}
