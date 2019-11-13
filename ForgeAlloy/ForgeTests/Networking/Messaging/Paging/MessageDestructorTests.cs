using Forge.Factory;
using Forge.Networking.Messaging.Paging;
using Forge.Serialization;
using NUnit.Framework;

namespace ForgeTests.Networking.Messaging.Paging
{
	[TestFixture]
	public class MessageDestructorTests : ForgeNetworkingTest
	{
		[Test]
		public void AtMaxSize_ShouldBeSinglePage()
		{
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			buffer.AugmentSize(destructor.MaxPageLength);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			Assert.AreEqual(1, pm.Pages.Count);
			Assert.AreEqual(buffer.Size, pm.TotalSize);
			Assert.AreEqual(buffer, pm.Buffer);
			Assert.AreEqual(0, pm.Pages[0].StartOffset);
			Assert.AreEqual(buffer.Size, pm.Pages[0].Length);
		}

		[Test]
		public void DoubleMaxSize_ShouldBeTwoPages()
		{
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			int headerLength = destructor.HeaderLength;
			buffer.AugmentSize(destructor.MaxPageLength * 2);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			Assert.AreEqual(2, pm.Pages.Count);
			Assert.AreEqual(buffer.Size, pm.TotalSize);
			Assert.AreEqual(buffer, pm.Buffer);
			Assert.AreEqual(0, pm.Pages[0].StartOffset);
			Assert.AreEqual(destructor.MaxPageLength + headerLength, pm.Pages[0].Length);
			Assert.AreEqual(destructor.MaxPageLength + headerLength, pm.Pages[1].StartOffset);
			Assert.AreEqual(destructor.MaxPageLength + headerLength, pm.Pages[1].Length);
		}

		[Test]
		public void TrippleMaxSize_ShouldBeThreePages()
		{
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			int headerLength = destructor.HeaderLength;
			buffer.AugmentSize(destructor.MaxPageLength * 3);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			Assert.AreEqual(3, pm.Pages.Count);
			Assert.AreEqual(buffer.Size, pm.TotalSize);
			Assert.AreEqual(buffer, pm.Buffer);
			Assert.AreEqual(0, pm.Pages[0].StartOffset);
			Assert.AreEqual(destructor.MaxPageLength + headerLength, pm.Pages[1].StartOffset);
			Assert.AreEqual((destructor.MaxPageLength + headerLength) * 2, pm.Pages[2].StartOffset);
			Assert.AreEqual(destructor.MaxPageLength + headerLength, pm.Pages[0].Length);
			Assert.AreEqual(destructor.MaxPageLength + headerLength, pm.Pages[1].Length);
			Assert.AreEqual(destructor.MaxPageLength + headerLength, pm.Pages[2].Length);
		}

		[Test]
		public void BelowMaxSize_ShouldBeSinglePage()
		{
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			buffer.AugmentSize(destructor.MaxPageLength / 2);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			Assert.AreEqual(1, pm.Pages.Count);
			Assert.AreEqual(buffer.Size, pm.TotalSize);
			Assert.AreEqual(buffer, pm.Buffer);
			Assert.AreEqual(0, pm.Pages[0].StartOffset);
			Assert.AreEqual(buffer.Size, pm.Pages[0].Length);
		}

		[Test]
		public void AboveMaxSize_ShouldBeTwoPagesButSecondIsntMax()
		{
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			int headerLength = destructor.HeaderLength;
			buffer.AugmentSize(destructor.MaxPageLength + destructor.MaxPageLength / 2);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			Assert.AreEqual(2, pm.Pages.Count);
			Assert.AreEqual(buffer.Size, pm.TotalSize);
			Assert.AreEqual(buffer, pm.Buffer);
			Assert.AreEqual(0, pm.Pages[0].StartOffset);
			Assert.AreEqual(destructor.MaxPageLength + headerLength, pm.Pages[1].StartOffset);
			Assert.AreEqual(destructor.MaxPageLength + headerLength, pm.Pages[0].Length);
			Assert.AreEqual(destructor.MaxPageLength / 2 + headerLength, pm.Pages[1].Length);
		}

		[Test]
		public void EmptyMessageBuffer_ShouldThrowException()
		{
			var buffer = new BMSByte();
			var destructor = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageDestructor>();
			Assert.Throws<CantBreakdownEmptyMessageException>(() => destructor.BreakdownMessage(buffer));
		}
	}
}
