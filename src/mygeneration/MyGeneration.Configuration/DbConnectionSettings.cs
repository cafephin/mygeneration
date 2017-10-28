using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MyGeneration.Configuration
{
    [Serializable]
    public class DbConnectionSettings : SettingsBase
    {
        public DbConnectionSettings()
        {
            ConnectionInfoCollection = new List<ConnectionInfo>();
            UserDatabaseAliases = new List<DatabaseAlias>();
        }

        [XmlArray("SavedConnections"), XmlArrayItem(typeof(ConnectionInfo), ElementName = "ConnectionInfo")]
        public List<ConnectionInfo> ConnectionInfoCollection { get; set; }

        [XmlElement]
        public bool ShowDefaultDatabaseOnly { get; set; }

        [XmlElement]
        public string Driver { get; set; }

        [XmlElement]
        public string ConnectionString { get; set; }

        [XmlElement]
        public string LanguageMappingFile { get { return ApplicationPath + @"\Settings\Languages.xml"; } }

        [XmlElement]
        public string Language { get; set; }

        [XmlElement]
        public string DbTargetMappingFile { get { return ApplicationPath + @"\Settings\DbTargets.xml"; } }

        [XmlElement]
        public string DbTarget { get; set; }

        [XmlElement]
        public string UserMetaDataFileName { get; set; }

        [XmlArray, XmlArrayItem(typeof(DatabaseAlias), ElementName = "DatabaseAlias")]
        public List<DatabaseAlias> UserDatabaseAliases { get; set; }

        [XmlIgnore]
        public string UserDatabaseAliasesDisplay
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var databaseAlias in UserDatabaseAliases)
                {
                    if (sb.Length > 0)
                        sb.Append(", \r\n");
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

                var entries = value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var entry in entries)
                {
                    var keyValuePair = entry.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                    if (keyValuePair.Length == 2)
                        UserDatabaseAliases.Add(new DatabaseAlias()
                                                {
                                                    DatabaseName = keyValuePair[0].Trim(),
                                                    Alias = keyValuePair[1].Trim()
                                                });
                }
            }
        }
    }
}
