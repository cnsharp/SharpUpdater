using System.Xml;
using System.Xml.Serialization;

namespace CnSharp.Updater
{
    public class UpdateLog
    {
        public string Version { get; set; }
        public string ReleaseDate { get; set; }
        private string _description;
        [XmlIgnore]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [XmlElement("Description")] //注意生成的节点名要同XmlIgnore的名称一致
        public XmlNode TextCData //不要使用XmlCDataSection,否则反序列化会出异常
        {
            get
            {
                return new XmlDocument().CreateCDataSection(_description);
                //注意此处不能写成 this.Description，否则将会得到null
            }
            set { _description = value.Value; }
        }
    }
}