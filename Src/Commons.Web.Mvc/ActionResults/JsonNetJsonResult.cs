using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using BoC.InversionOfControl;
using Newtonsoft.Json;

namespace BoC.Web.Mvc.ActionResults
{
    public class JsonNetJsonResult : JsonResult
    {

        /// <summary>
        /// Serialises the object that was passed into the constructor to JSON and writes the corresponding JSON to the result stream.
        /// </summary>
        /// <param name="context">The controller context for the current request.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("You're not allowed to do a JSON call through the GET protocol");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                JsonSerializerSettings settings = null;
                if (IoC.Resolver.IsRegistered<JsonSerializerSettings>())
                {
                    settings = IoC.Resolver.Resolve<JsonSerializerSettings>();
                }
                response.Write(JsonConvert.SerializeObject(Data, Formatting.None, settings));
                response.Flush();
				
				//seems sometimes an error is output AFTER the json result, really don't get it :(
				if (!response.TrySkipIisCustomErrors)
            		response.End();
            }
        }
    }
}