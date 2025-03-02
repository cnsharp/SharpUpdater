using System;
using System.Xml;

namespace CnSharp.Updater.Packaging
{
    public class PackageFeedResolver
    {
        private readonly XmlDocument _xmlDoc;
        private XmlNamespaceManager _namespaceManager;

        private void AddNamespaceManager()
        {
            _namespaceManager = new XmlNamespaceManager(_xmlDoc.NameTable);
            _namespaceManager.AddNamespace("ns", "http://www.w3.org/2005/Atom");
            _namespaceManager.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            _namespaceManager.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
        }

        public PackageFeedResolver(string xml) 
        {
            _xmlDoc = new XmlDocument();
            _xmlDoc.LoadXml(xml);
            AddNamespaceManager();
        }

        public PackageFeedResolver(XmlDocument doc) 
        {
            _xmlDoc = doc;
            AddNamespaceManager();
        }

        public PackageFeedSummary GetSummary()
        {
            var root = _xmlDoc.ChildNodes[1];
            return new PackageFeedSummary
            {
                Id = root.SelectSingleNode("//ns:entry/ns:title",_namespaceManager).InnerText,
                Version = root.SelectSingleNode("//ns:entry/m:properties/d:Version", _namespaceManager).InnerText,
                Updated = DateTimeOffset.Parse(root.SelectSingleNode("//ns:entry/ns:updated", _namespaceManager).InnerText),
                ReleaseNotes = root.SelectSingleNode("//ns:entry/m:properties/d:ReleaseNotes", _namespaceManager).InnerText,
                PackageUrl = root.SelectSingleNode("//ns:entry/ns:content", _namespaceManager).Attributes["src"].Value,
                PackageSize = long.Parse(root.SelectSingleNode("//ns:entry/m:properties/d:PackageSize", _namespaceManager).InnerText)
            };
        }
    }
}