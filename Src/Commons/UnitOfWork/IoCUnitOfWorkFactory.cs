using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.InversionOfControl;

namespace BoC.UnitOfWork
{
    public class IoCUnitOfWorkFactory: IUnitOfWorkFactory
    {
        public IUnitOfWork CreateUnitOfWork()
        {
            return IoC.Resolve<IUnitOfWork>();
        }
    }
}