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
using BoC.Persistence.db4o;
using BoC.Persistence.db4o.UnitOfWork;
using Commons.Persistence.db4o.Tests.Model;
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
        public void TestGetEntityById()
        {
            var person = new Person();

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                person = _repository.Save(person);
            }

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                Assert.NotNull(_repository.Get(person.Id));
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
                _repository.Delete(_repository.Query().First());

                Assert.Equal(0, _repository.Query().Count());

                var addressRepository = IoC.Resolver.Resolve<IRepository<Address>>();
                Assert.Equal(2, addressRepository.Query().Count()); // No cascades setup, addresses remain in db.

            }
        }

        [Fact]
        public void TestAutoIncrementIdProperty()
        {
            var person = new Person();

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                _repository.Save(person);
            }

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                var persistentPerson = _repository.Query().FirstOrDefault();
                Assert.NotEqual(0, persistentPerson.Id);
            }
        }

        [Fact]
        public void TestAutoIncrementIdPropertyWithRelations()
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
                Assert.NotEqual(0, persistentPerson.Id);
                
                var addressRepository = IoC.Resolver.Resolve<IRepository<Address>>();
                foreach (var address in addressRepository.Query())
                {
                    Assert.NotEqual(0, address.Id);
                }
            }
        }

        [Fact]
        public void TestConfigurationExtender()
        {
            var parentRepository = IoC.Resolver.Resolve<IRepository<Parent>>();
            var childRepository = IoC.Resolver.Resolve<IRepository<Child>>();

            var parent = new Parent
                             {
                                 Child = new Child()
                             };

            using(BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                parent = parentRepository.SaveOrUpdate(parent);

                Assert.Equal(1, childRepository.Query().Count());
            }

            using(BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                parentRepository.Delete(parent);

                // cascades should be setup, so no more children.
                Assert.Equal(0, childRepository.Query().Count());
            }
        }
    }
}
