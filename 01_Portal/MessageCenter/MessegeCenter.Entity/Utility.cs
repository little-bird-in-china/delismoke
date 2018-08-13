using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MessageCenter 
{
    internal class Utility
    {
        public static string SerializeToString(object info,XmlRootAttribute root)
        {
            if (info == null)
            {
                return null;
            }
            using (Stream xmlStream = new MemoryStream())
            {
                using (XmlWriter write = XmlWriter.Create(xmlStream, new XmlWriterSettings()
                {
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = true
                }))
                {
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(info.GetType(),root);

                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    xmlSerializer.Serialize(write, info, ns);
                    xmlStream.Position = 0;
                    StreamReader reader = new StreamReader(xmlStream);
                    string xmlString = reader.ReadToEnd();
                    return xmlString;
                }
            }
        }

        public static object DeserializeFromString(string xmlString, Type type, XmlRootAttribute root)
        {
            if (string.IsNullOrWhiteSpace(xmlString)) return null;
            using (StringReader sReader = new StringReader(xmlString))
            {
                using (XmlReader reader = XmlReader.Create(sReader))
                {
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(type, root);
                    object o = xmlSerializer.Deserialize(reader);
                    return o;
                }
            }
        } }
}
