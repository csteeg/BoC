using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Mvc;

namespace BoC.Web.Mvc.ActionResults
{
    public class DataContractXmlResult : ActionResult
    {
        public DataContractXmlResult(object data): this(data, "text/xml"){}
        public DataContractXmlResult(object data, string contentType){
            Data = data;
            ContentType = contentType;
        }

        public DataContractXmlResult(object data, string contentType, string rootName, string @namespace)
            : this(data, contentType)
        {
            RootName = rootName;
            Namespace = @namespace;
        }

        public object Data { get; set; }
        public string ContentType { get; set; }
        public string RootName { get; set; }
        public string Namespace { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var response = context.HttpContext.Response;
            response.Clear();
            response.ContentType = ContentType;

            DataContractSerializer serializer;
            if (Namespace == null)
            {
                serializer = new DataContractSerializer(Data.GetType());
            }
            else
            {
                serializer = new DataContractSerializer(Data.GetType(), GetOrDeriveRootName(Data), Namespace);
            }
            serializer.WriteObject(response.OutputStream, Data);
        }

        string GetOrDeriveRootName(object data)
        {
            string rootName = RootName;
            if (rootName == null)
            {
                rootName = data.GetType().Name;
                if (rootName.EndsWith("Resource")) rootName = rootName.Substring(0, rootName.Length - "Resource".Length);
            }
            return rootName;
        }
    }
}