using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using BoC;
using BoC.InversionOfControl;
using BoC.InversionOfControl.Unity;
using BoC.Persistence;
using Commons.Persistence.db4o.Tests.Model;
using Commons.Persistence.db4o.UnitOfWork;
using Xunit;

namespace Commons.Persistence.db4o.Tests.Fixtures
{
    public class Db4oFixture : IDisposable
    {
        private IRepository<Person> _repository;
        private const String DbPath = @"C:\temp\db4o.yap";

        public Db4oFixture()
        {
            if (File.Exists(DbPath))
            {
                File.Delete(DbPath);
            }

            Initializer.Execute();

            _repository = IoC.Resolver.Resolve<IRepository<Person>>();

            Assert.NotNull(_repository);
        }

        public void Dispose()
        {
            IoC.Resolver.Resolve<ISessionFactory>().Dispose();
            _repository = null;
            IoC.Reset();
        }

        [Fact]
        public void TestSaveSingleEntity()
        {
            var person = new Person();

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                _repository.Save(person);
            }

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                Assert.Equal(1, _repository.Query().Count());
            }
        }

        [Fact]
        public void TestSaveMultipleEntities()
        {
            var person1 = new Person();
            var person2 = new Person();
            var person3 = new Person();

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                _repository.Save(person1);
                _repository.Save(person2);
                _repository.Save(person3);

                Assert.Equal(3, _repository.Query().Count());
            }

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                Assert.Equal(3, _repository.Query().Count());
            }
        }

        [Fact]
        public void TestSaveSingleEntityWithRelations()
        {
            var person = new Person();
            person.HomeAddress = new Address { Type = "Home" };
            person.WorkAddress = new Address { Type = "Work" };

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                _repository.Save(person);
            }

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                var persistentPerson = _repository.Query().FirstOrDefault();
                Assert.NotNull(persistentPerson);
                Assert.NotNull(persistentPerson.HomeAddress);
                Assert.NotNull(persistentPerson.WorkAddress);

                Assert.Equal(person, persistentPerson);
                Assert.NotSame(person, persistentPerson);
                Assert.Equal(person.HomeAddress, persistentPerson.HomeAddress);
                Assert.NotSame(person.HomeAddress, persistentPerson.HomeAddress);
                Assert.Equal(person.WorkAddress, persistentPerson.WorkAddress);
                Assert.NotSame(person.WorkAddress, persistentPerson.WorkAddress);

                var addressRepository = IoC.Resolver.Resolve<IRepository<Address>>();
                Assert.Equal(2, addressRepository.Query().Count());
            }
        }

        [Fact]
        public void TestDeleteSingleEntity()
        {
            
        }

        [Fact]
        public void TestDeleteSingleEntityWithRelations()
        {
            var person = new Person();
            person.HomeAddress = new Address { Type = "Home" };
            person.WorkAddress = new Address { Type = "Work" };

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                _repository.Save(person);

                Assert.Equal(1, _repository.Query().Count());
            }

            using(var unitOfWork = BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork() as Db4oUnitOfWork)
            {
                unitOfWork.Session.Ext().Configure()
                    .ObjectClass(typeof(Person)).CascadeOnDelete(true);

                _repository.Delete(_repository.Query().First());

                Assert.Equal(0, _repository.Query().Count());

                var addressRepository = IoC.Resolver.Resolve<IRepository<Address>>();
                Assert.Equal(2, addressRepository.Query().Count()); // No cascades setup, addresses remain in db.

            }
        }
    }
}
