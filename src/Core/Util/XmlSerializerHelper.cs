using System.IO;
using System.Xml.Serialization;

namespace CnSharp.Updater.Util
{
    /// <summary>
    ///  serialize/deserialize object by xml formatter
    /// </summary>
    public class XmlSerializerHelper
    {
        /// <summary>
        /// copy a instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T Copy<T>(T t)
        {
            string xml = GetXmlStringFromObject(t);
            return LoadObjectFromXmlString<T>(xml);
        }

        /// <summary> 
        /// serialize object to xml string
        /// </summary> 
        /// <param name="obj">object</param> 
        /// <returns>XML string</returns> 
        public static string GetXmlStringFromObject<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Deserialize object from xml file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename">xml file name</param>
        /// <returns></returns>
        public static T LoadObjectFromXml<T>(string filename)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(filename))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Deserialize object from xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">xml string</param>
        /// <returns></returns>
        public static T LoadObjectFromXmlString<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static void SerializeToXmlFile<T>(T obj, string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, obj);
            }
        }

    }
}