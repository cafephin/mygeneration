using System;
using System.Xml.Serialization;

namespace MyGeneration.Configuration
{
    [Serializable]
    public class TemplateSettings : SettingsBase
    {
        [XmlElement]
        public bool ExecuteFromTemplateBrowserAsync { get; set; }

        [XmlElement]
        public string TemplateEditorFontFamily { get; set; }

        [XmlElement]
        public bool EnableLineNumbering { get; set; }

        [XmlElement]
        public bool EnableClipboard { get; set; }

        [XmlElement]
        public int CodePageEncodingId { get; set; }

        [XmlElement]
        public int TabSize { get; set; }

        [XmlElement]
        public int ScriptTimeout { get; set; }

        private string _defaultTemplateDirectory;

        [XmlElement]
        public string DefaultTemplateDirectory
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_defaultTemplateDirectory)) return _defaultTemplateDirectory;

                _defaultTemplateDirectory = IsAdministrator() ? ApplicationPath + @"\Templates\" : UserDataPath + @"\Templates\";

                return _defaultTemplateDirectory;
            }
            set { _defaultTemplateDirectory = value; }
        }

        private string _defaultOutputDirectory;

        [XmlElement]
        public string DefaultOutputDirectory
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_defaultOutputDirectory)) return _defaultOutputDirectory;
                _defaultOutputDirectory = IsAdministrator() ? ApplicationPath + @"\GeneratedCode\" : UserDataPath + @"\GeneratedCode\";
                return _defaultOutputDirectory;
            }
            set { _defaultOutputDirectory = value; }
        }

        [XmlElement]
        public bool UseProxyServer { get; set; }

        [XmlElement]
        public string ProxyServerUri { get; set; }

        [XmlElement]
        public string ProxyAuthUsername { get; set; }

        [XmlElement]
        public string ProxyAuthPassword { get; set; }

        [XmlElement]
        public string ProxyAuthDomain { get; set; }
    }
}
