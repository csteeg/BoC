using System;
using System.Linq;
using BoC.InversionOfControl;

namespace BoC.Profiling
{
    /// <summary>
    /// 
    /// </summary>
    public class Profiler
    {
        /// <summary>
        /// 
        /// </summary>
        private class DummyProfiler: IDisposable
        {
            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose(){}
        }
        /// <summary>
        /// Set to true to enable profiler
        /// </summary>
        public static bool Enabled = false;

        /// <summary>
        /// The _performanceProfilers
        /// </summary>
        private readonly IPerformanceProfiler[] _performanceProfilers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Profiler"/> class.
        /// </summary>
        /// <param name="performanceProfilers">The performanceProfilers.</param>
        public Profiler(IPerformanceProfiler[] performanceProfilers)
        {
            _performanceProfilers = performanceProfilers;
        }

        /// <summary>
        /// Begins the profiling.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="stringFormatParameters">The string format parameters.</param>
        /// <returns></returns>
        public static IDisposable StartContext(string key, params object[] stringFormatParameters)
        {
            if (!Enabled)
                return new DummyProfiler();

            Enabled = false;//don't profile the profiler
            try
            {
                var profiler = IoC.Resolver.Resolve<Profiler>();
                if (stringFormatParameters != null && stringFormatParameters.Length > 0)
                {
                    key = string.Format(key, stringFormatParameters);
                }
                return new ProfilingContext(key, profiler._performanceProfilers);
            }
            finally
            {
                Enabled = true;
            }
        }

        private sealed class ProfilingContext: IDisposable
        {
            private readonly string _key;
            private readonly IPerformanceProfiler[] _performanceProfilers;

            /// <summary>
            /// Initializes a new instance of the <see cref="ProfilingContext"/> class.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="performanceProfilers">The performance profilers.</param>
            public ProfilingContext(string key, IPerformanceProfiler[] performanceProfilers)
            {
                _key = key;
                _performanceProfilers = performanceProfilers;
                if (_performanceProfilers == null || _performanceProfilers.Length == 0)
                    return;
                foreach (var profiler in _performanceProfilers)
                {
                    profiler.BeginSession(key);
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
            private void Dispose(bool disposing)
            {
                if (_performanceProfilers == null || _performanceProfilers.Length == 0)
                    return;
                foreach (var profiler in _performanceProfilers)
                {
                    profiler.EndSession(_key);
                }
            }
        }
    }
}