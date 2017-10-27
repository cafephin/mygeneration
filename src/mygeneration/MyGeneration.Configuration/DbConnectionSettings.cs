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
        public List<ConnectionInfo> ConnectionInfoCollection
        {
            get;
            set;
        }

	    [XmlElement]
	    public bool ShowDefaultDatabaseOnly
	    {
	        get;
	        set;
	    }

	    private string _driver;
	    [XmlElement]
	    public string Driver
	    {
	        get { return "SQL"; } //TODO: return some default value?
	        set {_driver = value; }
	    }
	    
        private string _connectionString;
        [XmlElement]
	    public string ConnectionString
	    {
	        get { return "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=sa;Data Source=localhost"; } //TODO: return some default value?
	        set { _connectionString = value; }
	    }

	    private string _languageMappingFile;
        [XmlElement]
	    public string LanguageMappingFile
	    {
	        get { return SettingsDirectoryPath + @"\Languages.xml"; } //TODO: return some default value?
	        set { _languageMappingFile = value; }
	    }

	    private string _language;
	    [XmlElement]
	    public string Language
	    {
	        get { return "C#"; } //TODO: return some default value?
	        set { _language = value; }
	    }

	    private string _dbTargetMappingFile;
	    [XmlElement]
	    public string DbTargetMappingFile
	    {
	        get { return SettingsDirectoryPath + @"\DbTargets.xml"; } //TODO: return some default value?
	        set { _dbTargetMappingFile = value; }
	    }

	    private string _dbTarget;
	    [XmlElement]
	    public string DbTarget
	    {
	        get { return "SqlClient"; } //TODO: return some default value?
	        set { _dbTarget = value; }
	    }

	    private string _userMetadataFilename;
	    [XmlElement]
	    public string UserMetaDataFileName
	    {
	        get { return UserDataPath + @"\Settings\UserMetaData.xml"; } //TODO: return some default value?
	        set { _userMetadataFilename = value; }
	    }

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

                var entries = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
