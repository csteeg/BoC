using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DryIoc;

namespace BoC.InversionOfControl.DryIoC
{
    public static class WebReuse
    {
        public static readonly IReuse InHttpContext = Reuse.InCurrentNamedScope(HttpScopeContext.ROOT_SCOPE_NAME);
    }

    public sealed class HttpScopeContext : IScopeContext
    {
        public static readonly object ROOT_SCOPE_NAME = typeof(HttpScopeContext);

        public object RootScopeName { get { return ROOT_SCOPE_NAME; } }

        public IScope GetCurrentOrDefault()
        {
            var httpContext = HttpContext.Current;
            return httpContext == null ? _fallbackScope : (IScope)httpContext.Items[ROOT_SCOPE_NAME];
        }

        public void SetCurrent(Func<IScope, IScope> update)
        {
            var currentOrDefault = GetCurrentOrDefault();
            var newScope = update.ThrowIfNull().Invoke(currentOrDefault);
            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                _fallbackScope = newScope;
            }
            else
            {
                httpContext.Items[ROOT_SCOPE_NAME] = newScope;
                _fallbackScope = null;
            }
        }

        private IScope _fallbackScope;
    }
}
