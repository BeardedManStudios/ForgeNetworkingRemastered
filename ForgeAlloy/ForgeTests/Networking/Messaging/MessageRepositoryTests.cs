using System;
using System.Threading;
using FakeItEasy;
using Forge;
using Forge.Networking.Messaging;
using NUnit.Framework;

namespace ForgeTests.Networking.Messaging
{
	[TestFixture]
	public class MessageRepositoryTests : ForgeNetworkingTest
	{
		[Test]
		public void AddMessage_ShouldExist()
		{
			var message = A.Fake<IMessage>();
			message.Receipt = A.Fake<IMessageReceipt>();
			message.Receipt.Signature = new Guid();
			var repo = ForgeTypeFactory.GetNew<IMessageRepository>();
			repo.AddMessage(message);
			Assert.IsTrue(repo.Exists(message.Receipt.Signature));
		}

		[Test]
		public void AddMessageWithoutReceipt_ShouldThrow()
		{
			var message = A.Fake<IMessage>();
			message.Receipt = null;
			var repo = ForgeTypeFactory.GetNew<IMessageRepository>();
			Assert.Throws<MessageRepositoryMissingGuidOnMessageException>(() => repo.AddMessage(message));
		}

		[Test]
		public void AddMessageWithSameGuid_ShouldThrow()
		{
			var message = A.Fake<IMessage>();
			message.Receipt = A.Fake<IMessageReceipt>();
			message.Receipt.Signature = new Guid();
			var repo = ForgeTypeFactory.GetNew<IMessageRepository>();
			repo.AddMessage(message);
			Assert.Throws<MessageWithReceiptSignatureAlreadyExistsException>(() => repo.AddMessage(message));
		}

		[Test]
		public void CheckForMessage_ShouldNotExist()
		{
			Guid fake = new Guid();
			var repo = ForgeTypeFactory.GetNew<IMessageRepository>();
			Assert.IsFalse(repo.Exists(fake));
		}

		[Test]
		public void RemoveMessage_ShouldBeRemoved()
		{
			var message = A.Fake<IMessage>();
			message.Receipt = A.Fake<IMessageReceipt>();
			message.Receipt.Signature = new Guid();
			var repo = ForgeTypeFactory.GetNew<IMessageRepository>();
			repo.AddMessage(message);
			Assert.IsTrue(repo.Exists(message.Receipt.Signature));
			repo.RemoveMessage(message.Receipt.Signature);
			Assert.IsFalse(repo.Exists(message.Receipt.Signature));
			repo.AddMessage(message);
			Assert.IsTrue(repo.Exists(message.Receipt.Signature));
			repo.RemoveMessage(message);
			Assert.IsFalse(repo.Exists(message.Receipt.Signature));
		}

		[Test]
		public void MessageAfterTTL_ShouldBeRemoved()
		{
			var message = A.Fake<IMessage>();
			message.Receipt = A.Fake<IMessageReceipt>();
			message.Receipt.Signature = new Guid();
			var repo = ForgeTypeFactory.GetNew<IMessageRepository>();
			repo.AddMessage(message, 3);
			Assert.IsTrue(repo.Exists(message.Receipt.Signature));
			Thread.Sleep(1);
			Assert.IsTrue(repo.Exists(message.Receipt.Signature));
			Thread.Sleep(50);
			Assert.IsFalse(repo.Exists(message.Receipt.Signature));
		}

		[Test]
		public void AddMessageWithNegativeTTL_ShouldThrow()
		{
			var repo = ForgeTypeFactory.GetNew<IMessageRepository>();
			Assert.Throws<InvalidMessageRepositoryTTLProvided>(() => repo.AddMessage(A.Fake<IMessage>(), -1));
		}

		[Test]
		public void AddMessageWithZeroTTL_ShouldThrow()
		{
			var repo = ForgeTypeFactory.GetNew<IMessageRepository>();
			Assert.Throws<InvalidMessageRepositoryTTLProvided>(() => repo.AddMessage(A.Fake<IMessage>(), 0));
		}
	}
}
