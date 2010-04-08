using System;
using System.Web.Mvc;
using System.Web.Routing;

public static class RouteCollectionExtensions
{
    public static Route MapRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, object dataTokens)
    {
        if (routes == null)
        {
            throw new ArgumentNullException("routes");
        }
        if (url == null)
        {
            throw new ArgumentNullException("url");
        }
        Route item = new Route(url, new MvcRouteHandler())
                         {
                             Defaults = new RouteValueDictionary(defaults),
                             Constraints = new RouteValueDictionary(constraints),
                             DataTokens = new RouteValueDictionary(dataTokens)
                         };
        routes.Add(name, item);
        return item;
    }
}

