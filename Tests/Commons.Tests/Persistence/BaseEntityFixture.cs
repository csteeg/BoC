using System;
using BoC.Persistence;
using Xunit;

namespace BoC.Tests.Persistence
{
    public class TestableEntity1 : BaseEntity<int> {
        public String Name { get; set; }
    }
    public class TestableEntity2: BaseEntity<int> { }
    public class OtherEntity: BaseEntity<long>{}

    public class BaseEntityFixture
    {
        [Fact]
        public void Equals_Should_Return_True_When_Id_Is_Equal()
        {
            var entity1 = new TestableEntity1() {Id = 1, Name="Number 1"};
            var entity2 = new TestableEntity1() {Id = 1, Name = "Number 2"};

            Assert.True(entity1.Equals(entity2));
            Assert.True(entity1 == entity2);
        }

        [Fact]
        public void Equals_Should_Return_False_When_Id_Is_Equal_But_Types_Are_Different()
        {
            var entity1 = new TestableEntity1() { Id = 1 };
            var entity2 = new TestableEntity2() { Id = 1 };

            Assert.False(entity1.Equals(entity2));
            Assert.False(entity1 == entity2);
        }

        [Fact]
        public void Equals_Should_Return_True_When_The_Compareable_Is_A_Reference()
        {
            var entity1 = new TestableEntity1() { Id = 1 };
            var entity2 = entity1;

            Assert.True(entity1.Equals(entity2));
            Assert.True(entity1.Equals((object)entity2));
            Assert.True(entity1 == entity2);
        }


        [Fact]
        public void NotEquals_Should_Return_False_When_Id_Is_Equal()
        {
            var entity1 = new TestableEntity1() { Id = 1 };
            var entity2 = new TestableEntity1() { Id = 1 };

            Assert.False(entity1 != entity2);
        }

        [Fact]
        public void NotEquals_Should_Return_True_When_Id_Is_Equal_But_Types_Are_Different()
        {
            var entity1 = new TestableEntity1() { Id = 1 };
            var entity2 = new TestableEntity2() { Id = 1 };

            Assert.True(entity1 != entity2);
        }

        [Fact]
        public void NotEquals_Should_Return_False_When_The_Instances_Are_The_Same()
        {
            var entity1 = new TestableEntity1() { Id = 1 };
            var entity2 = entity1;

            Assert.False(entity1 != entity2);
        }

        [Fact]
        public void IBaseEntity_Id_Should_Return_Same_Id()
        {
            var entity1 = new TestableEntity1() { Id = 100 };

            Assert.Equal(100, ((IBaseEntity)entity1).Id);
            Assert.Equal(entity1.Id, ((IBaseEntity)entity1).Id);
        }

        [Fact]
        public void Comparing_To_Other_Entity_Should_Always_Return_False()
        {
            var entity1 = new TestableEntity1() {Id = 1};
            var entity2 = new OtherEntity() {Id = 1};

            Assert.False(entity1.Equals(entity2));
            Assert.False((IBaseEntity)entity1 == (IBaseEntity)entity2);
        }

        [Fact]
        public void Comparing_To_Null_Always_Return_False()
        {
            var entity1 = new TestableEntity1() { Id = 1 };
            TestableEntity1 entity2 = null;

            Assert.False(entity1.Equals(entity2));
            Assert.False(entity1.Equals(null));
            Assert.False(entity1 == entity2);
            Assert.False(entity1 == null);
            Assert.False(entity1 == (null as object));
        }

        [Fact]
        public void GetHashCode_Should_Return_Same_HashCode_If_Id_And_Type_Are_Same()
        {
            var entity1 = new TestableEntity1() { Id = 100, Name="number 1" };
            var entity2 = new TestableEntity1() { Id = 100, Name="number 2" };

            Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_Should_Return_Unique_HashCode_If_Id_Is_Not_Set_Yet()
        {
            var entity1 = new TestableEntity1();
            var entity2 = new TestableEntity1();

            Assert.NotEqual(entity1.GetHashCode(), entity2.GetHashCode());
        }
    }
}
