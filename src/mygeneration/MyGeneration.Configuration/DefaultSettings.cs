using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MyGeneration.Shared;

namespace MyGeneration.Configuration
{
    [Serializable]
    [XmlRoot]
    public class DefaultSettings : SettingsBase
	{
	    private const int MAX_NUMBER_OF_RECENT_FILES = 20;
		
        // Use "DefaultSettings.Instance" instead of "new DefaultSettings()" to get the settings
        // To discard changes without saving call DiscardChanges()
		public DefaultSettings()
		{
            //Load();
		    //Save(SettingsFilename);
        }

	    private static DefaultSettings _instance;
	    public static DefaultSettings Instance
	    {
	        get
	        {
	            if (_instance != null) return _instance;

	            _instance = new DefaultSettings();
	            _instance.Load();
	            return _instance;
	        }
	    }

        #region Settings
	    [XmlAttribute]
	    public string Version
	    {
	        get;
	        set;
	    }

	    private DbConnectionSettings _dbConnectionSettings;

	    [XmlElement]
	    public DbConnectionSettings DbConnectionSettings
	    {
	        get { return _dbConnectionSettings ?? (_dbConnectionSettings = new DbConnectionSettings()); }
            set { _dbConnectionSettings = value; }
	    }

	    private TemplateSettings _templateSettings;

        [XmlElement]
	    public TemplateSettings TemplateSettings
	    {
	        get { return _templateSettings ?? (_templateSettings = new TemplateSettings()); }
	        set { _templateSettings = value; }
	    }

	    private MiscSettings _miscSettings;

        [XmlElement]
	    public MiscSettings MiscSettings
	    {
	        get { return _miscSettings ?? (_miscSettings = new MiscSettings()); } 
            set { _miscSettings = value; }
	    }

	    private ArrayList _recentFiles;
	    [XmlArray("RecentFiles"),  XmlArrayItem(typeof(string), ElementName = "RecentFile")]
	    public ArrayList RecentFiles
	    {
	        get { return _recentFiles ?? (_recentFiles = new ArrayList()); }
	    }
        #endregion

	    public void Load()
	    {
            var xmlSerializer = new XmlSerializer(typeof(DefaultSettings));
	        try
	        {
	            using (var fileStream = new FileStream(SettingsFilename, FileMode.Open, FileAccess.Read))
	            {
	                _instance = (DefaultSettings) xmlSerializer.Deserialize(fileStream);
	            }
	        }
	        catch (Exception ex)
	        {
	            // DefaultSettings.xml doesn't exist or it's an invalid XML; 
                // now try to load from %PROGRAMFILE%\MyGeneration13\Settings\DefaultSettings.xml
	            using (var fileStream = new FileStream(ApplicationPath + @"\Settings\DefaultSettings.xml", FileMode.Open, FileAccess.Read))
	            {
	                _instance = (DefaultSettings) xmlSerializer.Deserialize(fileStream);
	            }
	        }

	        //Assembly myGenerationAssembly = Assembly.GetEntryAssembly();
	        //var version = myGenerationAssembly == null
	        //                  ? "Unknown"
	        //                  : myGenerationAssembly.GetName().Version.ToString();
	        //if (Version == version) return;

	        //// Our Version has changed, or DefaultSettings were just created
	        //// write any new settings and their defaults
	        //FillInMissingSettings(version);
	    }

        //TODO: Remove?
	    private void FillInMissingSettings(string version)
	    {
	        Version = version;

	        foreach (PropertyInfo propertyInfo in GetType().GetProperties())
	        {
	            MethodInfo getter = propertyInfo.GetGetMethod();
	            MethodInfo setter = propertyInfo.GetSetMethod();

	            if (getter != null && setter != null)
	            {
	                var par = new object[1];
	                par[0] = getter.Invoke(this, null);
	                setter.Invoke(this, par);
	            }
	        }
	    }

	    public void Refresh()
	    {
	        _instance = null;
	    }

	    public void DiscardChanges()
	    {
	        _recentFiles = null;

	        Load();
	    }

        public void Save(string settingsFilename)
		{
            var parentDirectory = Path.GetDirectoryName(settingsFilename);
		    if (!Directory.Exists(parentDirectory))
		        Directory.CreateDirectory(parentDirectory);
			var xmlSerializer = new XmlSerializer(typeof(DefaultSettings));
		    using (var fileStream = new FileStream(settingsFilename, FileMode.Create))
		    {
		        xmlSerializer.Serialize(fileStream, Instance);
		    }
		}
	}
}
