using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using BoC.InversionOfControl;

namespace BoC.UnitOfWork
{
    public static class UnitOfWork
    {
        public static IUnitOfWork BeginUnitOfWork()
        {
            if (IoC.IsInitialized())
                return IoC.Resolver.Resolve<IUnitOfWork>() ?? new DummyUnitOfWork();
            return new DummyUnitOfWork();
        }
    }

    class DummyUnitOfWork : IUnitOfWork
    {
        public void Dispose()
        {
            
        }
   }
}