using System;
using Sitecore.Diagnostics;

namespace BoC.Logging.Sitecore
{
    [Serializable]
    public class SitecoreLogger : ILogger
    {
        private Type ownerType = typeof(SitecoreLogger);
        public Type OwnerType
        {
            get { return ownerType; }
            set { ownerType = value; }
        }

        public bool IsDebugEnabled { get { return true; } }
        public bool IsInfoEnabled { get { return true; } }
        public bool IsWarnEnabled { get { return true; } }
        public bool IsErrorEnabled { get { return true; } }
        public bool IsFatalEnabled { get { return true; } }

        public IDisposable Stack(string name)
        {
            return log4net.NDC.Push(name);
        }

        string formatException(string message, Exception exc)
        {
            return string.Format("{0} \n {1}", message, exc);
        }
        #region Debug

        public void Debug(String message)
        {
            Log.Debug(message);
        }

        public void Debug(String message, Exception exception)
        {
            Log.Debug(formatException(message, exception));
        }

        public void DebugFormat(String format, params Object[] args)
        {
            Log.Debug(String.Format(format, args));
        }

        public void DebugFormat(Exception exception, String format, params Object[] args)
        {
            Log.Debug(formatException(String.Format(format, args), exception));
        }

        public void DebugFormat(IFormatProvider formatProvider, String format, params Object[] args)
        {
            Log.Debug(String.Format(formatProvider, format, args));
        }

        public void DebugFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
        {
            Log.Debug(formatException(String.Format(formatProvider, format, args), exception));
        }

        #endregion

        #region Info

        public void Info(String message)
        {
            Log.Info(message, this);
        }

        public void Info(String message, Exception exception)
        {
            Log.Info(formatException(message, exception), this);
        }

        public void InfoFormat(String format, params Object[] args)
        {
            Log.Info(String.Format(format, args), this);
        }

        public void InfoFormat(Exception exception, String format, params Object[] args)
        {
            Log.Info(formatException(String.Format(format, args), exception), this);
        }

        public void InfoFormat(IFormatProvider formatProvider, String format, params Object[] args)
        {
            Log.Info(String.Format(formatProvider, format, args), this);
        }

        public void InfoFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
        {
            Log.Info(formatException(String.Format(formatProvider, format, args), exception), this);
        }

        #endregion

        #region Warn

        public void Warn(String message)
        {
            Log.Warn(message, this);
        }

        public void Warn(String message, Exception exception)
        {
            Log.Warn(formatException(message, exception), this);
        }

        public void WarnFormat(String format, params Object[] args)
        {
            Log.Warn(String.Format(format, args), this);
        }

        public void WarnFormat(Exception exception, String format, params Object[] args)
        {
            Log.Warn(formatException(String.Format(format, args), exception), this);
        }

        public void WarnFormat(IFormatProvider formatProvider, String format, params Object[] args)
        {
            Log.Warn(String.Format(formatProvider, format, args), this);
        }

        public void WarnFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
        {
            Log.Warn(formatException(String.Format(formatProvider, format, args), exception), this);
        }

        #endregion

        #region Error


        public void Error(String message)
        {
            Log.Error(message, this);
        }

        public void Error(String message, Exception exception)
        {
            Log.Error(formatException(message, exception), this);
        }

        public void ErrorFormat(String format, params Object[] args)
        {
            Log.Error(String.Format(format, args), this);
        }

        public void ErrorFormat(Exception exception, String format, params Object[] args)
        {
            Log.Error(formatException(String.Format(format, args), exception), this);
        }

        public void ErrorFormat(IFormatProvider formatProvider, String format, params Object[] args)
        {
            Log.Error(String.Format(formatProvider, format, args), this);
        }

        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
        {
            Log.Error(formatException(String.Format(formatProvider, format, args), exception), this);
        }
        #endregion

        #region Fatal


        public void Fatal(String message)
        {
            Log.Fatal(message, this);
        }

        public void Fatal(String message, Exception exception)
        {
            Log.Fatal(formatException(message, exception), this);
        }

        public void FatalFormat(String format, params Object[] args)
        {
            Log.Fatal(String.Format(format, args), this);
        }

        public void FatalFormat(Exception exception, String format, params Object[] args)
        {
            Log.Fatal(formatException(String.Format(format, args), exception), this);
        }

        public void FatalFormat(IFormatProvider formatProvider, String format, params Object[] args)
        {
            Log.Fatal(String.Format(formatProvider, format, args), this);
        }

        public void FatalFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
        {
            Log.Fatal(formatException(String.Format(formatProvider, format, args), exception), this);
        }
        #endregion

    }
}