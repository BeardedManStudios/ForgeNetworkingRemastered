using System.Net;
using System.Threading;
using FakeItEasy;
using Forge.Factory;
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
			message.Receipt = A.Fake<IMessageReceiptSignature>();
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
			repo.AddMessage(message, A.Fake<EndPoint>());
			Assert.IsTrue(repo.Exists(message.Receipt));
		}

		[Test]
		public void AddMessageWithoutReceipt_ShouldThrow()
		{
			var message = A.Fake<IMessage>();
			message.Receipt = null;
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
			Assert.Throws<MessageRepositoryMissingReceiptOnMessageException>(() => repo.AddMessage(message, A.Fake<EndPoint>()));
		}

		[Test]
		public void AddMessageWithSameGuid_ShouldThrow()
		{
			var message = A.Fake<IMessage>();
			message.Receipt = A.Fake<IMessageReceiptSignature>();
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
			repo.AddMessage(message, A.Fake<EndPoint>());
			Assert.Throws<MessageWithReceiptSignatureAlreadyExistsException>(() => repo.AddMessage(message, A.Fake<EndPoint>()));
		}

		[Test]
		public void CheckForMessage_ShouldNotExist()
		{
			var fake = A.Fake<IMessageReceiptSignature>();
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
			Assert.IsFalse(repo.Exists(fake));
		}

		[Test]
		public void RemoveMessage_ShouldBeRemoved()
		{
			var message = A.Fake<IMessage>();
			message.Receipt = A.Fake<IMessageReceiptSignature>();
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
			repo.AddMessage(message, A.Fake<EndPoint>());
			Assert.IsTrue(repo.Exists(message.Receipt));
			repo.RemoveMessage(message.Receipt);
			Assert.IsFalse(repo.Exists(message.Receipt));
			repo.AddMessage(message, A.Fake<EndPoint>());
			Assert.IsTrue(repo.Exists(message.Receipt));
			repo.RemoveMessage(message);
			Assert.IsFalse(repo.Exists(message.Receipt));
		}

		[Test, MaxTime(1000)]
		public void MessageAfterTTL_ShouldBeRemoved()
		{
			var message = A.Fake<IMessage>();
			message.Receipt = A.Fake<IMessageReceiptSignature>();
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
			repo.AddMessage(message, A.Fake<EndPoint>(), 3);
			Assert.IsTrue(repo.Exists(message.Receipt));
			Thread.Sleep(1);
			Assert.IsTrue(repo.Exists(message.Receipt));
			bool removed;
			do
			{
				removed = repo.Exists(message.Receipt);
			} while (!removed);
			Assert.IsTrue(removed);
		}

		[Test]
		public void AddMessageWithNegativeTTL_ShouldThrow()
		{
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
			Assert.Throws<InvalidMessageRepositoryTTLProvided>(() => repo.AddMessage(A.Fake<IMessage>(), A.Fake<EndPoint>(), -1));
		}

		[Test]
		public void AddMessageWithZeroTTL_ShouldThrow()
		{
			var repo = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IMessageRepository>();
			Assert.Throws<InvalidMessageRepositoryTTLProvided>(() => repo.AddMessage(A.Fake<IMessage>(), A.Fake<EndPoint>(), 0));
		}

		// TODO:  Validate the endpoint on the messages
		// TODO:  Validate the iterator of the messages
		// TODO:  Validate the RemoveAllFor of the messages
	}
}
