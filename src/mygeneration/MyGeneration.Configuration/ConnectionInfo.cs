using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MyGeneration.Configuration;

namespace MyGeneration
{
    [Serializable]
    public class ConnectionInfo
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement]
        public string DbDriver { get; set; }

        [XmlElement]
        public string ConnectionString {get; set; }

        [XmlElement]
        public string DbTarget {get; set; }

        [XmlElement]
        public string DbTargetPath { get; set; }

        [XmlElement]
        public string Language { get; set; }

        [XmlElement]
        public string LanguagePath { get; set; }

        [XmlElement]
        public string UserMetaDataPath { get; set; }
        
        public string DatabaseUserDataXmlMappingsString
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var databaseAlias in UserDatabaseAliases)
                {
                    if (sb.Length > 0) sb.Append(",");
                    sb.Append(databaseAlias.DatabaseName)
                      .Append("=")
                      .Append(databaseAlias.Alias);
                }
                return sb.ToString();
            }

            set
            {
                UserDatabaseAliases.Clear();
                if (string.IsNullOrWhiteSpace(value)) return;

                var entries = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var entry in entries)
                {
                    var keyValuePair = entry.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);

                    if (keyValuePair.Length == 2)
                        UserDatabaseAliases.Add(new DatabaseAlias
                                                        {
                                                            DatabaseName = keyValuePair[0],
                                                            Alias = keyValuePair[1]
                                                        });
                }
            }
        }
        
        private List<DatabaseAlias> _userDatabaseAliases;
        [XmlArray, XmlArrayItem(typeof(DatabaseAlias), ElementName = "DatabaseAlias")]
        public List<DatabaseAlias> UserDatabaseAliases
        {
            get { return _userDatabaseAliases ?? (_userDatabaseAliases = new List<DatabaseAlias>()); }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
