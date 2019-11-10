using Forge;
using Forge.Networking.Messaging.Paging;
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
			var destructor = ForgeTypeFactory.Get<IMessageDestructor>();
			var constructor = ForgeTypeFactory.Get<IMessageConstructor>();
			buffer.AugmentSize(destructor.MaxPageLength / 2);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			var beforeBuffer = new BMSByte();
			beforeBuffer.Clone(buffer);
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			pm.Buffer.GetBasicType<string>(); // This guid is pulled off in the IMessageBufferInterpreter
			constructor.ReconstructMessagePage(pm.Buffer);
			Assert.IsTrue(constructor.MessageReconstructed);
			Assert.AreEqual(beforeBuffer.Size, constructor.MessageBuffer.Size);
			for (int i = 0; i < beforeBuffer.Size; i++)
				Assert.AreEqual(beforeBuffer[i], constructor.MessageBuffer[i]);
		}

		[Test]
		public void ExactMaxPageSize_ShouldBeSame()
		{
			var buffer = new BMSByte();
			var destructor = ForgeTypeFactory.Get<IMessageDestructor>();
			var constructor = ForgeTypeFactory.Get<IMessageConstructor>();
			buffer.AugmentSize(destructor.MaxPageLength);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			var beforeBuffer = new BMSByte();
			beforeBuffer.Clone(buffer);
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			pm.Buffer.GetBasicType<string>(); // This guid is pulled off in the IMessageBufferInterpreter
			constructor.ReconstructMessagePage(pm.Buffer);
			Assert.IsTrue(constructor.MessageReconstructed);
			Assert.AreEqual(beforeBuffer.Size, constructor.MessageBuffer.Size);
			for (int i = 0; i < beforeBuffer.Size; i++)
				Assert.AreEqual(beforeBuffer[i], constructor.MessageBuffer[i]);
		}

		[Test]
		public void ExactDoublePageSize_ShouldBeSame()
		{
			var buffer = new BMSByte();
			var destructor = ForgeTypeFactory.Get<IMessageDestructor>();
			var constructor = ForgeTypeFactory.Get<IMessageConstructor>();
			buffer.AugmentSize(destructor.MaxPageLength * 2);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			var beforeBuffer = new BMSByte();
			beforeBuffer.Clone(buffer);
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			for (int i = 0; i < pm.Pages.Count; i++)
			{
				BMSByte pageBuffer = GetPageSection(buffer, pm, i);
				Assert.IsFalse(constructor.MessageReconstructed);
				Assert.AreEqual(0, constructor.MessageBuffer.Size);
				pageBuffer.GetBasicType<string>(); // This guid is pulled off in the IMessageBufferInterpreter
				constructor.ReconstructMessagePage(pageBuffer);
			}
			Assert.IsTrue(constructor.MessageReconstructed);
			Assert.AreEqual(beforeBuffer.Size, constructor.MessageBuffer.Size);
			for (int i = 0; i < beforeBuffer.Size; i++)
				Assert.AreEqual(beforeBuffer[i], constructor.MessageBuffer[i]);
		}

		[Test]
		public void ExactTripplePageSize_ShouldBeSame()
		{
			var buffer = new BMSByte();
			var destructor = ForgeTypeFactory.Get<IMessageDestructor>();
			var constructor = ForgeTypeFactory.Get<IMessageConstructor>();
			buffer.AugmentSize(destructor.MaxPageLength * 3);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			var beforeBuffer = new BMSByte();
			beforeBuffer.Clone(buffer);
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			for (int i = 0; i < pm.Pages.Count; i++)
			{
				BMSByte pageBuffer = GetPageSection(buffer, pm, i);
				Assert.IsFalse(constructor.MessageReconstructed);
				Assert.AreEqual(0, constructor.MessageBuffer.Size);
				pageBuffer.GetBasicType<string>(); // This guid is pulled off in the IMessageBufferInterpreter
				constructor.ReconstructMessagePage(pageBuffer);
			}
			Assert.IsTrue(constructor.MessageReconstructed);
			Assert.AreEqual(beforeBuffer.Size, constructor.MessageBuffer.Size);
			for (int i = 0; i < beforeBuffer.Size; i++)
				Assert.AreEqual(beforeBuffer[i], constructor.MessageBuffer[i]);
		}

		[Test]
		public void AboveMaxPageSize_ShouldBeSame()
		{
			var buffer = new BMSByte();
			var destructor = ForgeTypeFactory.Get<IMessageDestructor>();
			var constructor = ForgeTypeFactory.Get<IMessageConstructor>();
			buffer.AugmentSize(destructor.MaxPageLength + destructor.MaxPageLength / 2);
			buffer.PointToEnd();
			for (int i = 0; i < buffer.Size; i++)
			{
				buffer[i] = (byte)(i % byte.MaxValue);
			}
			var beforeBuffer = new BMSByte();
			beforeBuffer.Clone(buffer);
			IPagenatedMessage pm = destructor.BreakdownMessage(buffer);
			for (int i = 0; i < pm.Pages.Count; i++)
			{
				BMSByte pageBuffer = GetPageSection(buffer, pm, i);
				Assert.IsFalse(constructor.MessageReconstructed);
				Assert.AreEqual(0, constructor.MessageBuffer.Size);
				pageBuffer.GetBasicType<string>(); // This guid is pulled off in the IMessageBufferInterpreter
				constructor.ReconstructMessagePage(pageBuffer);
			}
			Assert.IsTrue(constructor.MessageReconstructed);
			Assert.AreEqual(beforeBuffer.Size, constructor.MessageBuffer.Size);
			for (int i = 0; i < beforeBuffer.Size; i++)
				Assert.AreEqual(beforeBuffer[i], constructor.MessageBuffer[i]);
		}
	}
}
