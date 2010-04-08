using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace BoC.Web.Mvc.ActionResults
{
    public class XmlResult : ActionResult
    {
        
        public XmlResult(object objectToSerialize)
        {
            ObjectToSerialize = objectToSerialize;
        }

        public object ObjectToSerialize { get; set; }

        /// <summary>
        /// Serialises the object that was passed into the constructor to XML and writes the corresponding XML to the result stream.
        /// </summary>
        /// <param name="context">The controller context for the current request.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (ObjectToSerialize != null)
            {
                var xs = new XmlSerializer(ObjectToSerialize.GetType());
                context.HttpContext.Response.ContentType = "text/xml";
                xs.Serialize(context.HttpContext.Response.Output, ObjectToSerialize);
            }
        }
    }
}