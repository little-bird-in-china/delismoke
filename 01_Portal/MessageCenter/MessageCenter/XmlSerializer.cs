using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Reflection;

namespace MessageCenter.Xml
{
    public class XmlSerializer
    {
        public static string SerializeToString<T>(T info, bool isRemoveDeclarationNode = true, XmlRootAttribute xmlRoot = null)
        {
            return SerializeToString(info, Encoding.UTF8, null, isRemoveDeclarationNode, xmlRoot);
        }

        public static string SerializeToString<T>(T info, Encoding encoding, XmlSerializerNamespaces xmlNS = null, bool isRemoveDeclarationNode = true, XmlRootAttribute xmlRoot = null)
        {
            if (info == null)
            {
                return null;
            }
            using (Stream xmlStream = new MemoryStream())
            {
                using (XmlWriter write = XmlWriter.Create(xmlStream, new XmlWriterSettings()
                {
                    Encoding = encoding,
                    OmitXmlDeclaration = isRemoveDeclarationNode
                }))
                {
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(info.GetType(), xmlRoot);

                    XmlSerializerNamespaces ns = xmlNS;
                    if (ns == null)
                    {
                        ns = new XmlSerializerNamespaces();
                        ns.Add("", "");
                    }

                    xmlSerializer.Serialize(write, info, ns);
                    xmlStream.Position = 0;
                    StreamReader reader = new StreamReader(xmlStream);
                    string xmlString = reader.ReadToEnd();
                    return xmlString;
                }
            }
        }
        public static void SerializeToFile<T>(T info, string fileFullName)
        {
            if (info == null)
            {
                return;
            }
            using (XmlWriter writer = XmlWriter.Create(fileFullName))
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(info.GetType());
                xmlSerializer.Serialize(writer, info);
            }
        }

        public static T DeserializeFromFile<T>(string fileFullName)
        {
            using (XmlReader reader = XmlReader.Create(fileFullName))
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                object o = xmlSerializer.Deserialize(reader);
                return (T)o;
            }
        }

        public static T DeserializeFromString<T>(string xmlString, XmlRootAttribute xmlRoot = null)// where T : class ,new()
        {
            if (string.IsNullOrWhiteSpace(xmlString)) return default(T);
            using (StringReader sReader = new StringReader(xmlString))
            {
                using (XmlReader reader = XmlReader.Create(sReader))
                {
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T), xmlRoot);
                    object o = xmlSerializer.Deserialize(reader);
                    return (T)o;
                }
            }
        }

        public static T ChangeType<T>(object obj) where T : class ,new()
        {
            if (obj == null)
            {
                return null;
            }
            using (MemoryStream stream = new MemoryStream())
            {
                System.Xml.Serialization.XmlSerializer objSerializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
                objSerializer.Serialize(stream, obj);
                stream.Position = 0;
                System.Xml.Serialization.XmlSerializer tSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                object newObj = tSerializer.Deserialize(stream);
                return newObj as T;
            }
        }

    }
}
