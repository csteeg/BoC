using System;
using System.Data;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Xml;

namespace Steeg.Framework.Web.UI.Helpers
{
    public class XmlHelper
    {
        private XmlHelper() { }

        public const string DEFAULT_XML_NAMESPACE = "http://steeg-framework.org/";

        public static void SetXmlSource(Xml xmlControl, object source)
        {
            if (xmlControl == null)
                return;

            xmlControl.DocumentContent = "";

            if (source == null)
                return;

            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(source.GetType(), DEFAULT_XML_NAMESPACE);
            System.Text.StringBuilder xml = new System.Text.StringBuilder();

            serializer.Serialize(
                System.Xml.XmlTextWriter.Create(xml),
                source);

            xmlControl.DocumentContent = xml.ToString();
        }
    }
}