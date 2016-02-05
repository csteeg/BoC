// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using log4net;
using log4net.Core;
using System.Reflection;

namespace BoC.Logging
{
    [Serializable]
    public class Log4netLogger : ILogger
    {
        private static Type ownerType = typeof(ILogger);

        public IDisposable Stack(string name)
        {
            return log4net.LogicalThreadContext.Stacks["NDC"].Push(name);
        }

        private log4net.Core.ILogger logger = log4net.LogManager.GetLogger(Assembly.GetCallingAssembly(), String.Empty).Logger;
        protected internal log4net.Core.ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        public override string ToString()
        {
            return Logger.ToString();
        }

        #region Debug

        public void Debug(String message)
        {
            if (IsDebugEnabled)
            {
                Logger.Log(ownerType, Level.Debug, message, null);
            }
        }

        public void Debug(String message, Exception exception)
        {
            if (IsDebugEnabled)
            {
                Logger.Log(ownerType, Level.Debug, message, exception);
            }
        }

        public void DebugFormat(String format, params Object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Log(ownerType, Level.Debug, String.Format(format, args), null);
            }
        }

        public void DebugFormat(Exception exception, String format, params Object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Log(ownerType, Level.Debug, String.Format(format, args), exception);
            }
        }

        public void DebugFormat(IFormatProvider formatProvider, String format, params Object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Log(ownerType, Level.Debug, String.Format(formatProvider, format, args), null);
            }
        }

        public void DebugFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Log(ownerType, Level.Debug, String.Format(formatProvider, format, args), exception);
            }
        }

        #endregion

        #region Info

        public void Info(String message)
        {
            if (IsInfoEnabled)
            {
                Logger.Log(ownerType, Level.Info, message, null);
            }
        }

        public void Info(String message, Exception exception)
        {
            if (IsInfoEnabled)
            {
                Logger.Log(ownerType, Level.Info, message, exception);
            }
        }

        public void InfoFormat(String format, params Object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Log(ownerType, Level.Info, String.Format(format, args), null);
            }
        }

        public void InfoFormat(Exception exception, String format, params Object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Log(ownerType, Level.Info, String.Format(format, args), exception);
            }
        }

        public void InfoFormat(IFormatProvider formatProvider, String format, params Object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Log(ownerType, Level.Info, String.Format(formatProvider, format, args), null);
            }
        }

        public void InfoFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Log(ownerType, Level.Info, String.Format(formatProvider, format, args), exception);
            }
        }

        #endregion

        #region Warn

        public void Warn(String message)
        {
            if (IsWarnEnabled)
            {
                Logger.Log(ownerType, Level.Warn, message, null);
            }
        }

        public void Warn(String message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                Logger.Log(ownerType, Level.Warn, message, exception);
            }
        }

        public void WarnFormat(String format, params Object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Log(ownerType, Level.Warn, String.Format(format, args), null);
            }
        }

        public void WarnFormat(Exception exception, String format, params Object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Log(ownerType, Level.Warn, String.Format(format, args), exception);
            }
        }

        public void WarnFormat(IFormatProvider formatProvider, String format, params Object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Log(ownerType, Level.Warn, String.Format(formatProvider, format, args), null);
            }
        }

        public void WarnFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Log(ownerType, Level.Warn, String.Format(formatProvider, format, args), exception);
            }
        }

        #endregion

        #region Error

        public void Error(String message)
        {
            if (IsErrorEnabled)
            {
                Logger.Log(ownerType, Level.Error, message, null);
            }
        }

        public void Error(String message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                Logger.Log(ownerType, Level.Error, message, exception);
            }
        }

        public void ErrorFormat(String format, params Object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Log(ownerType, Level.Error, String.Format(format, args), null);
            }
        }

        public void ErrorFormat(Exception exception, String format, params Object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Log(ownerType, Level.Error, String.Format(format, args), exception);
            }
        }

        public void ErrorFormat(IFormatProvider formatProvider, String format, params Object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Log(ownerType, Level.Error, String.Format(formatProvider, format, args), null);
            }
        }

        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Log(ownerType, Level.Error, String.Format(formatProvider, format, args), exception);
            }
        }

        #endregion

        #region Fatal

        public void Fatal(String message)
        {
            if (IsFatalEnabled)
            {
                Logger.Log(ownerType, Level.Fatal, message, null);
            }
        }

        public void Fatal(String message, Exception exception)
        {
            if (IsFatalEnabled)
            {
                Logger.Log(ownerType, Level.Fatal, message, exception);
            }
        }

        public void FatalFormat(String format, params Object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Log(ownerType, Level.Fatal, String.Format(format, args), null);
            }
        }

        public void FatalFormat(Exception exception, String format, params Object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Log(ownerType, Level.Fatal, String.Format(format, args), exception);
            }
        }

        public void FatalFormat(IFormatProvider formatProvider, String format, params Object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Log(ownerType, Level.Fatal, String.Format(formatProvider, format, args), null);
            }
        }

        public void FatalFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Log(ownerType, Level.Fatal, String.Format(formatProvider, format, args), exception);
            }
        }

        #endregion

        #region Is (...) Enabled

        public bool IsErrorEnabled
        {
            get { return Logger.IsEnabledFor(Level.Error); }
        }

        public bool IsWarnEnabled
        {
            get { return Logger.IsEnabledFor(Level.Warn); }
        }

        public bool IsDebugEnabled
        {
            get { return Logger.IsEnabledFor(Level.Debug); }
        }

        public bool IsFatalEnabled
        {
            get { return Logger.IsEnabledFor(Level.Fatal); }
        }

        public bool IsInfoEnabled
        {
            get { return Logger.IsEnabledFor(Level.Info); }
        }

        #endregion
    }
}