using System;
using System.Collections.Generic;

namespace BoC.Web.Mvc
{
    public class SimpleException
    {
        public SimpleException(Exception exc)
        {
            this.Errors.Add(exc.Message);
            this.StackTrace = exc.StackTrace;
            this.Source = exc.Source;
            this.Type = exc.GetType().FullName;
            this.InnerException = exc;
        }
        List<string> errors = new List<string>();
        public List<string> Errors
        {
            get
            {
                return errors;
            }
        }
        public string StackTrace;
        public string Source;
        public string Type;
        public bool IsException = true;

        internal Exception InnerException;
    }
}