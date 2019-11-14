using FakeItEasy;
using Forge.Engine;
using Forge.Factory;
using NUnit.Framework;

namespace ForgeTests.Engine
{
	[TestFixture]
	public class EntityRepositoryTests : ForgeNetworkingTest
	{
		[Test]
		public void OnEntityAddedEvent_ShouldBeFired()
		{
			var entity = A.Fake<IEntity>();
			var addedEvent = A.Fake<EntityAddedToRepository>();
			A.CallTo(() => entity.Id).Returns(9);
			var entityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			entityRepository.onEntityAdded += addedEvent;
			entityRepository.AddEntity(entity);
			bool has = entityRepository.HasEntity(entity.Id);
			A.CallTo(() => addedEvent(A<IEntity>._)).MustHaveHappenedOnceExactly();
		}

		[Test]
		public void AddEntity_ShouldExistWhenChecked()
		{
			var entity = A.Fake<IEntity>();
			A.CallTo(() => entity.Id).Returns(9);
			var entityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			entityRepository.AddEntity(entity);
			Assert.IsTrue(entityRepository.HasEntity(entity.Id));
		}

		[Test]
		public void GetEntityById_ShouldReturnAddedEntity()
		{
			var entity = A.Fake<IEntity>();
			A.CallTo(() => entity.Id).Returns(9);
			var entityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			entityRepository.AddEntity(entity);
			IEntity found = entityRepository.GetEntityById(entity.Id);
			Assert.IsNotNull(found);
			Assert.AreEqual(entity, found);
		}

		[Test]
		public void GetEntityById_ShouldThrowIfNotFound()
		{
			var entity = A.Fake<IEntity>();
			A.CallTo(() => entity.Id).Returns(9);
			var entityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			entityRepository.AddEntity(entity);
			Assert.Throws<EntityNotInRepositoryException>(() => entityRepository.GetEntityById(-1));
		}

		[Test]
		public void GetEntityById_ShouldThrowIfEmpty()
		{
			var entityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			Assert.Throws<EntityNotInRepositoryException>(() => entityRepository.GetEntityById(9));
		}

		[Test]
		public void RemoveEntityRef_ShouldRemoveAddedEntity()
		{
			var entity = A.Fake<IEntity>();
			A.CallTo(() => entity.Id).Returns(9);
			var entityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			entityRepository.AddEntity(entity);
			Assert.IsTrue(entityRepository.HasEntity(entity.Id));
			entityRepository.RemoveEntity(entity);
			Assert.IsFalse(entityRepository.HasEntity(entity.Id));
		}

		[Test]
		public void RemoveEntityRef_ShouldOnlyRemoveTargetEntity()
		{
			var entity1 = A.Fake<IEntity>();
			var entity2 = A.Fake<IEntity>();
			A.CallTo(() => entity1.Id).Returns(9);
			A.CallTo(() => entity2.Id).Returns(10);
			var entityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			entityRepository.AddEntity(entity1);
			entityRepository.AddEntity(entity2);
			Assert.IsTrue(entityRepository.HasEntity(entity1.Id));
			Assert.IsTrue(entityRepository.HasEntity(entity2.Id));
			entityRepository.RemoveEntity(entity2);
			Assert.IsTrue(entityRepository.HasEntity(entity1.Id));
			Assert.IsFalse(entityRepository.HasEntity(entity2.Id));
		}

		[Test]
		public void RemoveEntityId_ShouldRemoveAddedEntity()
		{
			var entity = A.Fake<IEntity>();
			A.CallTo(() => entity.Id).Returns(9);
			var entityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			entityRepository.AddEntity(entity);
			Assert.IsTrue(entityRepository.HasEntity(entity.Id));
			entityRepository.RemoveEntity(entity.Id);
			Assert.IsFalse(entityRepository.HasEntity(entity.Id));
		}

		[Test]
		public void RemoveEntityId_ShouldOnlyRemoveTargetEntity()
		{
			var entity1 = A.Fake<IEntity>();
			var entity2 = A.Fake<IEntity>();
			A.CallTo(() => entity1.Id).Returns(9);
			A.CallTo(() => entity2.Id).Returns(10);
			var entityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			entityRepository.AddEntity(entity1);
			entityRepository.AddEntity(entity2);
			Assert.IsTrue(entityRepository.HasEntity(entity1.Id));
			Assert.IsTrue(entityRepository.HasEntity(entity2.Id));
			entityRepository.RemoveEntity(entity2.Id);
			Assert.IsTrue(entityRepository.HasEntity(entity1.Id));
			Assert.IsFalse(entityRepository.HasEntity(entity2.Id));
		}

		[Test]
		public void HasEntity_ShouldBeTrue()
		{
			var entity = A.Fake<IEntity>();
			A.CallTo(() => entity.Id).Returns(9);
			var entityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			entityRepository.AddEntity(entity);
			Assert.IsTrue(entityRepository.HasEntity(entity.Id));
		}

		[Test]
		public void HasEntity_ShouldBeFalse()
		{
			var entity = A.Fake<IEntity>();
			A.CallTo(() => entity.Id).Returns(9);
			var entityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
			entityRepository.AddEntity(entity);
			Assert.IsFalse(entityRepository.HasEntity(-1));
		}
	}
}
