using System;
using System.Net;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Paging
{
	public class ForgeMessageConstructor : IMessageConstructor
	{
		public EndPoint Sender { get; private set; }
		public bool MessageReconstructed { get; private set; } = false;
		public BMSByte MessageBuffer { get; private set; } = null;
		public int Id { get; private set; }

		private BMSByte[] _pages = null;
		private BMSBytePool _bufferPool = null;

		public void Setup(BMSBytePool bufferPool, int id)
		{
			_bufferPool = bufferPool;
			MessageBuffer = _bufferPool.Get(1024);
			Id = id;
		}

		public void Teardown()
		{
			_bufferPool.Release(MessageBuffer);
		}

		public void ReconstructMessagePage(BMSByte page, EndPoint sender)
		{
			Sender = sender;
			if (MessageReconstructed)
				return;

			if (page.Size == 0)
				throw new ArgumentException($"The input page buffer had an invalid size of 0");

			var pageNumber = page.GetBasicType<int>();
			var totalSize = page.GetBasicType<int>();

			if (page.Size == 0)
				throw new ArgumentException($"The input page buffer had an invalid size of 0");

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
					int count = (int)System.Math.Ceiling(totalSize / actualSize);
					_pages = new BMSByte[count];
				}
				if (_pages[pageNumber] == null)
				{
					_pages[pageNumber] = _bufferPool.Get(page.Size);
					_pages[pageNumber].Clone(page);
				}
				if (IsDone())
				{
					foreach (var p in _pages)
					{
						MessageBuffer.Append(p);
						_bufferPool.Release(p);
					}
					MessageReconstructed = true;
				}
			}
		}

		private bool IsDone()
		{
			foreach (var p in _pages)
			{
				if (p == null)
					return false;
			}
			return true;
		}
	}
}
