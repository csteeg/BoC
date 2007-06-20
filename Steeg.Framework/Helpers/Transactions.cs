using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Steeg.Framework.Helpers
{
    public class TransActionOptions
    {
        private TransActionOptions() {}

        public static TransactionOptions ReadUnComitted
        {
            get
            {
                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = IsolationLevel.ReadUncommitted;
                return options;
            }
        }

        public static TransactionOptions ReadComitted
        {
            get
            {
                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = IsolationLevel.ReadCommitted;
                return options;
            }
        }
    }
}
