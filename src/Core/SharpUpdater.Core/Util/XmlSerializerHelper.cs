using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CnSharp.Updater.Util
{
    /// <summary>
    ///  serialize/deserialize object by xml formatter
    /// </summary>
    public class XmlSerializerHelper
    {
        #region Public Methods

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
            using (var ms = new MemoryStream())
            using (var writer = new XmlTextWriter(ms, Encoding.UTF8))
            {
                writer.Indentation = 3;
                writer.IndentChar = ' ';
                writer.Formatting = Formatting.Indented;
                var sc = new XmlSerializer(obj.GetType());
                sc.Serialize(writer, obj);
                string xml = Encoding.UTF8.GetString(ms.ToArray());
                int index = xml.IndexOf("?>");
                if (index > 0)
                {
                    xml = xml.Substring(index + 2);
                }
                return xml.Trim();
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
            //using (var sr = new StreamReader(filename, Encoding.UTF8))
            //{
            //    var serializer = new XmlSerializer(typeof(T));
            //    return (T)serializer.Deserialize(sr);
            //}
            var doc = new XmlDocument();
            doc.Load(filename);
            return LoadObjectFromXmlString<T>(doc.InnerXml);
        }

        /// <summary>
        /// Deserialize object from xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">xml string</param>
        /// <returns></returns>
        public static T LoadObjectFromXmlString<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof (T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                return (T) serializer.Deserialize(ms);
            }
        }

        #endregion
    }
}