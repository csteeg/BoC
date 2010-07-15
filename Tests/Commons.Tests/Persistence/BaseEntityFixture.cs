using BoC.Persistence;
using Xunit;

namespace BoC.Tests.Persistence
{
    public class TestableEntity1: BaseEntity<int> {}
    public class TestableEntity2: BaseEntity<int> { }

    public class BaseEntityFixture
    {
        [Fact]
        public void Equals_Should_Return_True_When_Id_Is_Equal()
        {
            var entity1 = new TestableEntity1() {Id = 1};
            var entity2 = new TestableEntity1() {Id = 1};

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
        public void Equals_Should_Return_True_When_The_Instances_Are_The_Same()
        {
            var entity1 = new TestableEntity1() { Id = 1 };
            var entity2 = entity1;

            Assert.True(entity1.Equals(entity2));
            Assert.True(entity1 == entity2);
        }
    }
}
