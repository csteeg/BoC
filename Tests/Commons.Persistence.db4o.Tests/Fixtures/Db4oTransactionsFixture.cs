using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using BoC;
using BoC.InversionOfControl;
using BoC.Persistence;
using Commons.Persistence.db4o.Tests.Model;
using Xunit;

namespace Commons.Persistence.db4o.Tests.Fixtures
{
    public class Db4oTransactionsFixture : IDisposable
    {
        private IRepository<Person> _repository;
        private const String DbPath = @"C:\temp\db4o.yap";

        public Db4oTransactionsFixture()
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
        public void TestSaveSingleEntityInTransactionScope()
        {
            var person = new Person();
            {
                using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        _repository.Save(person);
                        scope.Complete();
                    }
                }
            }

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                Assert.Equal(1, _repository.Query().Count());
            }
        }

        [Fact]
        public void TestSaveAndRollbackSingleEntityInTransactionScope()
        {
            var person = new Person();

            try
            {
                using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        _repository.Save(person);
                        throw new ApplicationException("I'm forcing a rollback.");
                        scope.Complete();
                    }
                }
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e);
            }

            using (BoC.UnitOfWork.UnitOfWork.BeginUnitOfWork())
            {
                Assert.Equal(0, _repository.Query().Count());
            }
        }
    }
}
