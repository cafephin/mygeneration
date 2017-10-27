using System;
using System.Xml;
using System.Xml.Serialization;
using MyGeneration.Shared;

namespace MyGeneration.Configuration
{
    [Serializable]
    public class TemplateSettings : SettingsBase
    {
        private bool _executeFromTemplateBrowerAsync;
	    [XmlElement]
	    public bool ExecuteFromTemplateBrowserAsync
	    {
	        get { return true; } //TODO: return some default value?
	        set { _executeFromTemplateBrowerAsync = value; }
	    }

	    [XmlElement]
	    public string TemplateEditorFontFamily
	    {
	        get;
	        set;
	    }

	    [XmlElement]
	    public bool EnableLineNumbering
	    {
	        get;
	        set;
	    }

	    [XmlElement]
	    public bool EnableClipboard
	    {
	        get;
	        set;
	    }

        private int _encodingId;
	    [XmlElement]
	    public int CodePageEncodingId
	    {
	        get { return -1; } //TODO: return some default value?
	        set { _encodingId = value; }
	    }

	    private int _tabSize;
	    [XmlElement]
	    public int TabSize
	    {
	        get { return 4; } //TODO: return some default value?
	        set { _tabSize = value; }
	    }

        private int _scriptTimeoutInSeconds;
	    [XmlElement]
	    public int ScriptTimeout
	    {
	        get { return -1; } //TODO: return some default value?
	        set { _scriptTimeoutInSeconds = value; }
	    }

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
            set	{ _defaultTemplateDirectory = value; }
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
		public bool UseProxyServer
        {
            get;
            set;
        }

        [XmlElement]
		public string ProxyServerUri
        {
            get;
            set;
        }

        [XmlElement]
		public string ProxyAuthUsername
        {
            get;
            set;
        }

	    [XmlElement]
		public string ProxyAuthPassword
	    {
	        get;
	        set;
	    }

	    [XmlElement]
		public string ProxyAuthDomain
	    {
	        get;
	        set;
	    }
    }
}
