using BoC.Helpers;

namespace BoC.EventAggregator
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public class DelegateReference : IDelegateReference
    {
        /// <summary>
        /// The _delegate
        /// </summary>
        private readonly Delegate _delegate;
        /// <summary>
        /// The _weak reference
        /// </summary>
        private readonly WeakReference _weakReference;
        /// <summary>
        /// The _method
        /// </summary>
        private readonly MethodInfo _method;
        /// <summary>
        /// The _delegate type
        /// </summary>
        private readonly Type _delegateType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateReference"/> class.
        /// </summary>
        /// <param name="delegate">The delegate.</param>
        /// <param name="keepReferenceAlive">if set to <c>true</c> [keep reference alive].</param>
        public DelegateReference(Delegate @delegate, bool keepReferenceAlive)
        {
            Check.Argument.IsNotNull(@delegate, "@delegate");

            if (keepReferenceAlive)
            {
                _delegate = @delegate;
            }
            else
            {
                _weakReference = new WeakReference(@delegate.Target);
                _method = @delegate.Method;
                _delegateType = @delegate.GetType();
            }
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public Delegate Target
        {
            [DebuggerStepThrough]
            get
            {
                return _delegate ?? TryGetDelegate();
            }
        }

        /// <summary>
        /// Tries the get delegate.
        /// </summary>
        /// <returns></returns>
        private Delegate TryGetDelegate()
        {
            if (_method.IsStatic)
            {
                return Delegate.CreateDelegate(_delegateType, null, _method);
            }

            object target = _weakReference.Target;

            return (target != null) ? Delegate.CreateDelegate(_delegateType, target, _method) : null;
        }
    }
}