using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BoC;
using BoC.InversionOfControl;
using BoC.Persistence;
using BoC.Persistence.db4o;
using BoC.Persistence.db4o.UnitOfWork;
using Commons.Persistence.db4o.Tests.Model;
using Db4objects.Db4o;
using Xunit;

namespace Commons.Persistence.db4o.Tests.Fixtures
{
    public class Db4oUnitOfWorkFixture : IDisposable
    {
        private IRepository<Person> _repository;
        private const String DbPath = @"C:\temp\db4o.yap";

        public Db4oUnitOfWorkFixture()
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
        public void TestNestedUnitsOfWork()
        {
            var person = new Person();

            using (var outer = BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork() as Db4oUnitOfWork)
            {
                Assert.NotNull(outer.Session);
                _repository.SaveOrUpdate(person);

                using (var inner1 = BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork() as Db4oUnitOfWork)
                {
                    Assert.Equal(outer.Session, inner1.Session);
                    _repository.SaveOrUpdate(person);

                    using (var inner2 = BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork() as Db4oUnitOfWork)
                    {
                        Assert.Equal(outer.Session, inner2.Session);
                        _repository.SaveOrUpdate(person);
                    }
                }
            }

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                Assert.Equal(1, _repository.Query().Count());
            }
        }

        [Fact]
        public void TestConcurrentUnitsOfWork()
        {
            Db4oUnitOfWork unitOfWork1 = null;
            Db4oUnitOfWork unitOfWork2 = null;
            Db4oUnitOfWork unitOfWork3 = null;

            IObjectContainer session1 = null;
            IObjectContainer session2 = null;
            IObjectContainer session3 = null;

            var thread1 = new Thread(() =>
                           {
                               unitOfWork1 = BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork() as Db4oUnitOfWork;
                               session1 = unitOfWork1.Session;
                           });
            thread1.Start();
            thread1.Join();
            
            var thread2 = new Thread(() =>
                           {
                               unitOfWork2 = BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork() as Db4oUnitOfWork;
                               session2 = unitOfWork2.Session;
                           });
            thread2.Start();
            thread2.Join();
            
            var thread3 = new Thread(() =>
                           {
                               unitOfWork3 = BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork() as Db4oUnitOfWork;
                               session3 = unitOfWork3.Session;
                           });
            thread3.Start();
            thread3.Join();

            Assert.NotSame(session1, session2);
            Assert.NotSame(session2, session3);
            Assert.NotSame(session1, session3);
        }
    }
}
