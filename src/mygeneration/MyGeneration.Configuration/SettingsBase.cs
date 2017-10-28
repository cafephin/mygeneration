using System;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Xml.Serialization;

namespace MyGeneration.Configuration
{
    public class SettingsBase
    {
        private string _applicationPath;
        
        [XmlIgnore]
	    protected string ApplicationPath
	    {
	        get 
	        {
	            if (_applicationPath != null) return _applicationPath;

	            Assembly entryAssembly = Assembly.GetEntryAssembly();
	            if (entryAssembly == null) return _applicationPath;
	            _applicationPath = entryAssembly.Location;
	            _applicationPath = _applicationPath.Substring(0, _applicationPath.LastIndexOf(@"\", StringComparison.Ordinal));
	            return _applicationPath; 
	        }
	    }

	    private string _userDataPath;

        [XmlIgnore]
	    protected string UserDataPath
	    {
	        get
	        {
	            if (_userDataPath != null) return _userDataPath;

	            _userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyGeneration");
                if (Directory.Exists(_userDataPath)) return _userDataPath;

	            Directory.CreateDirectory(_userDataPath);
	            Directory.CreateDirectory(Path.Combine(_userDataPath, "Settings"));
	            Directory.CreateDirectory(Path.Combine(_userDataPath, "Templates"));
	            return _userDataPath;
	        }
	    }

	    private string _settingsDirectoryPath;

        [XmlIgnore]
	    protected string SettingsDirectoryPath
	    {
	        get
	        {
	            if (!string.IsNullOrWhiteSpace(_settingsDirectoryPath)) return _settingsDirectoryPath;
	            _settingsDirectoryPath = IsAdministrator() ? ApplicationPath : UserDataPath;
	            _settingsDirectoryPath += @"\Settings";
	            return _settingsDirectoryPath;
	        }
	    }

	    private string _settingsFileName;

        [XmlIgnore]
	    public string SettingsFilename
	    {
	        get
	        {
	            if (!string.IsNullOrWhiteSpace(_settingsFileName)) return _settingsFileName;
	            _settingsFileName = SettingsDirectoryPath + @"\DefaultSettings.xml";
	            return _settingsFileName;
	        }
	        set
	        {
	            _settingsFileName = value;
	        }
	    }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                .IsInRole(WindowsBuiltInRole.Administrator);
        }    
    }
}
