using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MyGeneration
{
    public class ConnectionInfo
    {
        private Dictionary<string, string> _databaseUserMetaMappings;

        public string Name;
        public string Driver;
        public string ConnectionString;
        public string DbTarget;
        public string DbTargetPath;
        public string Language;
        public string LanguagePath;
        public string UserMetaDataPath;
        public bool ShowDefaultDatabaseOnly;

        public ConnectionInfo() {}

        public ConnectionInfo(XmlNode node)
        {
            Name = XmlHelper.GetAttribute(node, "name", "");
            Driver = XmlHelper.GetAttribute(node, "driver", "");
            ConnectionString = XmlHelper.GetAttribute(node, "connstr", "");
            DbTargetPath = XmlHelper.GetAttribute(node, "dbtargetpath", "");
            LanguagePath = XmlHelper.GetAttribute(node, "languagepath", "");
            UserMetaDataPath = XmlHelper.GetAttribute(node, "usermetapath", "");
            DbTarget = XmlHelper.GetAttribute(node, "dbtarget", "");
            Language = XmlHelper.GetAttribute(node, "language", "");
            ShowDefaultDatabaseOnly = Convert.ToBoolean(XmlHelper.GetAttribute(node, "defaultdatabaseonly", "True"));

            foreach (XmlNode xmlDbAlias in node.ChildNodes)
            {
                if (xmlDbAlias.Name != "SavedXmlUserMetaAlias") continue;
                try
                {
                    string db = XmlHelper.GetAttribute(xmlDbAlias, "database", ""),
                           alias = XmlHelper.GetAttribute(xmlDbAlias, "alias", "");

                    DatabaseUserDataXmlMappings[db] = alias;
                }
                catch { }
            }
        }

        public string DatabaseUserDataXmlMappingsString
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var key in DatabaseUserDataXmlMappings.Keys)
                {
                    if (sb.Length > 0) sb.Append(",");
                    sb.Append(key).Append("=").Append(DatabaseUserDataXmlMappings[key]);
                }
                return sb.ToString();
            }
            set
            {
                DatabaseUserDataXmlMappings.Clear();
                var combpairs = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string pair in combpairs)
                {
                    var keyVal = pair.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    if (keyVal.Length == 2)
                        DatabaseUserDataXmlMappings[keyVal[0]] = keyVal[1];
                }
            }
        }

        public Dictionary<string, string> DatabaseUserDataXmlMappings
        {
            get
            {
                if (_databaseUserMetaMappings == null)
                    _databaseUserMetaMappings = new Dictionary<string, string>();
                return _databaseUserMetaMappings;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}