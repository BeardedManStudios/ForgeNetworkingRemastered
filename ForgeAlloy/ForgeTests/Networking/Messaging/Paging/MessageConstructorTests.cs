using System;
using System.Linq;
using System.Net;
using FakeItEasy;
using Forge.Factory;
using Forge.Networking.Messaging.Paging;
using Forge.Networking.Players;
using Forge.Serialization;
using NUnit.Framework;

namespace ForgeTests.Networking.Messaging.Paging
{
	[TestFixture]
	public class MessageConstructorTests : ForgeNetworkingTest
	{
		private static BMSByte GetPageSection(BMSByte buffer, IPagenatedMessage pm, int pageNumber)
		{
			var page = pm.Pages[pageNumber];
			var pageBuffer = new BMSByte();
			pageBuffer.SetArraySize(page.Length);
			pageBuffer.BlockCopy(buffer.byteArr, page.StartOffset, page.Length);
			return pageBuffer;
		}

		[Test]
		public void PartialSinglePage_ShouldBeSame()
		{
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			var bufferInterpreter = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBufferInterpreter>();
			buffer.AugmentSize(destructor.MaxPageLength / 2);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			var beforeBuffer = new BMSByte();
			beforeBuffer.Clone(buffer);
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			IMessageConstructor constructor = bufferInterpreter.ReconstructPacketPage(pm.Buffer, A.Fake<EndPoint>());
			Assert.IsTrue(constructor.MessageReconstructed);
			Assert.AreEqual(beforeBuffer.Size, constructor.MessageBuffer.Size);
			for (int i = 0; i < beforeBuffer.Size; i++)
				Assert.AreEqual(beforeBuffer[i], constructor.MessageBuffer[i]);
		}

		[Test]
		public void ExactMaxPageSize_ShouldBeSame()
		{
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			var bufferInterpreter = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBufferInterpreter>();
			buffer.AugmentSize(destructor.MaxPageLength);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			var beforeBuffer = new BMSByte();
			beforeBuffer.Clone(buffer);
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			IMessageConstructor constructor = bufferInterpreter.ReconstructPacketPage(pm.Buffer, A.Fake<EndPoint>());
			Assert.IsTrue(constructor.MessageReconstructed);
			Assert.AreEqual(beforeBuffer.Size, constructor.MessageBuffer.Size);
			for (int i = 0; i < beforeBuffer.Size; i++)
				Assert.AreEqual(beforeBuffer[i], constructor.MessageBuffer[i]);
		}

		[Test]
		public void ExactDoublePageSize_ShouldBeSame()
		{
			var ep = A.Fake<EndPoint>();
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			var bufferInterpreter = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBufferInterpreter>();
			buffer.AugmentSize(destructor.MaxPageLength * 2);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			var beforeBuffer = new BMSByte();
			beforeBuffer.Clone(buffer);
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			IMessageConstructor constructor = null;
			for (int i = 0; i < pm.Pages.Count; i++)
			{
				BMSByte pageBuffer = GetPageSection(buffer, pm, i);
				constructor = bufferInterpreter.ReconstructPacketPage(pageBuffer, ep);
			}
			Assert.IsNotNull(constructor);
			Assert.IsTrue(constructor.MessageReconstructed);
			Assert.AreEqual(beforeBuffer.Size, constructor.MessageBuffer.Size);
			for (int i = 0; i < beforeBuffer.Size; i++)
				Assert.AreEqual(beforeBuffer[i], constructor.MessageBuffer[i]);
		}

		[Test]
		public void ExactTripplePageSize_ShouldBeSame()
		{
			var ep = A.Fake<EndPoint>();
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			var bufferInterpreter = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBufferInterpreter>();
			buffer.AugmentSize(destructor.MaxPageLength * 3);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			var beforeBuffer = new BMSByte();
			beforeBuffer.Clone(buffer);
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			IMessageConstructor constructor = null;
			for (int i = 0; i < pm.Pages.Count; i++)
			{
				BMSByte pageBuffer = GetPageSection(buffer, pm, i);
				constructor = bufferInterpreter.ReconstructPacketPage(pageBuffer, ep);
			}
			Assert.IsNotNull(constructor);
			Assert.IsTrue(constructor.MessageReconstructed);
			Assert.AreEqual(beforeBuffer.Size, constructor.MessageBuffer.Size);
			for (int i = 0; i < beforeBuffer.Size; i++)
				Assert.AreEqual(beforeBuffer[i], constructor.MessageBuffer[i]);
		}

		[Test]
		public void AboveMaxPageSize_ShouldBeSame()
		{
			var ep = A.Fake<EndPoint>();
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			var bufferInterpreter = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBufferInterpreter>();
			buffer.AugmentSize(destructor.MaxPageLength + destructor.MaxPageLength / 2);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			var beforeBuffer = new BMSByte();
			beforeBuffer.Clone(buffer);
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			IMessageConstructor constructor = null;
			for (int i = 0; i < pm.Pages.Count; i++)
			{
				BMSByte pageBuffer = GetPageSection(buffer, pm, i);
				constructor = bufferInterpreter.ReconstructPacketPage(pageBuffer, ep);
			}
			Assert.IsNotNull(constructor);
			Assert.IsTrue(constructor.MessageReconstructed);
			Assert.AreEqual(beforeBuffer.Size, constructor.MessageBuffer.Size);
			for (int i = 0; i < beforeBuffer.Size; i++)
				Assert.AreEqual(beforeBuffer[i], constructor.MessageBuffer[i]);
		}

		[Test]
		public void ExactTripplePageSizeInRandomOrder_ShouldBeSame()
		{
			var ep = A.Fake<EndPoint>();
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			var bufferInterpreter = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBufferInterpreter>();
			buffer.AugmentSize(destructor.MaxPageLength * 3);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
				buffer[i] = (byte)(i % byte.MaxValue);
			var beforeBuffer = new BMSByte();
			beforeBuffer.Clone(buffer);
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			IMessageConstructor constructor = null;
			int[] indexes = new int[pm.Pages.Count];
			for (int i = 0; i < indexes.Length; i++)
				indexes[i] = i;
			Random rnd = new Random();
			indexes = indexes.OrderBy(x => rnd.Next()).ToArray();
			for (int i = 0; i < indexes.Length; i++)
			{
				BMSByte pageBuffer = GetPageSection(buffer, pm, indexes[i]);
				constructor = bufferInterpreter.ReconstructPacketPage(pageBuffer, ep);
			}
			Assert.IsNotNull(constructor);
			Assert.IsTrue(constructor.MessageReconstructed);
			Assert.AreEqual(beforeBuffer.Size, constructor.MessageBuffer.Size);
			for (int i = 0; i < beforeBuffer.Size; i++)
				Assert.AreEqual(beforeBuffer[i], constructor.MessageBuffer[i]);
		}

		[Test]
		public void RemovingAPlayer_ShouldRemoveBufferedMessages()
		{
			var ep = A.Fake<EndPoint>();
			var player = A.Fake<INetPlayer>();
			A.CallTo(() => player.EndPoint).Returns(ep);
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			var bufferInterpreter = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageBufferInterpreter>();
			buffer.AugmentSize(destructor.MaxPageLength + destructor.MaxPageLength / 2);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			var beforeBuffer = new BMSByte();
			beforeBuffer.Clone(buffer);
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			IMessageConstructor constructor = null;
			for (int i = 0; i < pm.Pages.Count - 1; i++)
			{
				BMSByte pageBuffer = GetPageSection(buffer, pm, i);
				constructor = bufferInterpreter.ReconstructPacketPage(pageBuffer, ep);
			}
			bufferInterpreter.ClearBufferFor(player);
			Assert.IsNotNull(constructor);
			Assert.IsFalse(constructor.MessageReconstructed);

			BMSByte pbuf = GetPageSection(buffer, pm, pm.Pages.Count - 1);
			constructor = bufferInterpreter.ReconstructPacketPage(pbuf, ep);
			Assert.IsFalse(constructor.MessageReconstructed);
		}
	}
}
