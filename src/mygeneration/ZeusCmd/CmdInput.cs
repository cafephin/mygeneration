using System;
using System.Collections.Generic;
using Zeus.Configuration;
using Zeus.Projects;

namespace Zeus
{
    internal class CmdInput
	{
	    private bool _makeRelative;
	    private string _pathProject;
	    private string _pathXmlData;
		private string _pathCollectXmlData;
	    
        public CmdInput(string[] args)
		{
		    IsSilent = false;
		    ShowHelp = false;
		    EnableLog = false;
		    ErrorMessage = null;
		    PathTemplate = null;
		    PathOutput = null;
		    PathLog = null;
		    Template = null;
		    Project = null;
		    SavedInput = null;
		    InputToSave = null;
		    ConnectionType = null;
		    ConnectionString = null;
		    MergeMetaFiles = false;
		    MetaFile1 = null;
		    MetaDatabase1 = null;
		    MetaFile2 = null;
		    MetaDatabase2 = null;
		    MetaFileMerged = null;
		    ProjectItemToRecord = null;

		    Parse(args);
		}

		private void Parse(string[] args) 
		{
			var numberOfArguments = args.Length;

		    if (numberOfArguments == 0) 
			{
				ShowHelp = true;
				return;
			}

			for (var i = 0; i < numberOfArguments; i++)
			{
			    var argument = args[i];

			    switch (argument)
                {
                    case "-internaluse":
                        _internalUse = true;
                        break;
                    case "-tc":
                    case "-testconnection":
                        _mode = ProcessMode.MyMeta;
                        if (numberOfArguments > (i + 2))
                        {
                            ConnectionType = args[++i];
                            ConnectionString = args[++i];
                        }
                        else
                        {
                            _valid = false;
                            ErrorMessage = "Invalid switch usage: " + argument;
                        }
                        break;
					case "-aio":
					case "-addintrinsicobject":
						if (numberOfArguments > (i+3)) 
						{
							var assembly = args[++i];
							var classpath = args[++i];
							var varname = args[++i];

							var iobj = new ZeusIntrinsicObject(assembly, classpath, varname);
							_intrinsicObjects.Add(iobj);
						}
						else 
						{
							_valid = false;
							ErrorMessage = "Invalid switch usage: " + argument;
						}
						break;
					case "-rio":
					case "-removeintrinsicobject":
                        if (numberOfArguments > (i + 1))
                        {
                            var varname = args[++i];
                            foreach (ZeusIntrinsicObject zio in ZeusConfig.Current.IntrinsicObjects)
                            {
                                if (zio.VariableName == varname && !_intrinsicObjectsToRemove.Contains(zio))
                                {
                                    _intrinsicObjectsToRemove.Add(zio);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            _valid = false;
                            ErrorMessage = "Invalid switch usage: " + argument;
                        }
                        break;
					case "-r":
					case "-relative":
						_makeRelative = true;
						break;
					case "-s":
					case "-silent":
						IsSilent = true;
						break;
					case "-?":
					case "-h":
					case "-help":
						ShowHelp = true;
						break;
					case "-l":
					case "-logfile":
						EnableLog = true;
						if (numberOfArguments > (i+1)) 
							PathLog = args[++i];
						else 
						{
							_valid = false;
							ErrorMessage = "Invalid switch usage: " + argument;
						}
						break;
					case "-o":
					case "-outfile":
						if (numberOfArguments > (i+1)) 
							PathOutput = args[++i];
						else 
						{
							_valid = false;
							ErrorMessage = "Invalid switch usage: " + argument;
						}
						break;
					case "-p":
					case "-project":
						_mode = ProcessMode.Project;
						if (numberOfArguments > (i+1)) 
							_pathProject = args[++i];
						else 
						{
							_valid = false;
							ErrorMessage = "Invalid switch usage: " + argument;
						}
						break;
					case "-mumd":
                    case "-mergeusermetadata":
                        _mode = ProcessMode.MyMeta;
                        MergeMetaFiles = true;
                        if (numberOfArguments > (i + 5))
                        {
                            MetaFile1 = args[++i];
                            MetaDatabase1 = args[++i];
                            MetaFile2 = args[++i];
                            MetaDatabase2 = args[++i];
                            MetaFileMerged = args[++i];
                        }
                        else
                        {
                            _valid = false;
                            ErrorMessage = "Invalid switch usage: " + argument;
                        }
                        break;
                    case "-m":
                    case "-pf":
                    case "-module":
                    case "-projectfolder":
						if (numberOfArguments > (i+1)) 
						{
							string data = args[++i];
							if (!_moduleNames.Contains(data))
								_moduleNames.Add(data);
						}
						else 
						{
							_valid = false;
							ErrorMessage = "Invalid switch usage: " + argument;
						}
                        break;
                    case "-rti":
                    case "-recordtemplateinstance":
                        if (numberOfArguments > (i + 1))
                        {
                            ProjectItemToRecord = args[++i];
                            _mode = ProcessMode.Project;
                        }
                        else
                        {
                            _valid = false;
                            ErrorMessage = "Invalid switch usage: " + argument;
                        }
                        break;
                    case "-ti":
                    case "-templateinstance":
                        if (numberOfArguments > (i + 1))
                        {
                            string data = args[++i];
                            if (!_projectItems.Contains(data))
                                _projectItems.Add(data);
                        }
                        else
                        {
                            _valid = false;
                            ErrorMessage = "Invalid switch usage: " + argument;
                        }
                        break;
					case "-i":
					case "-inputfile":
						_mode = ProcessMode.Template;
						if (numberOfArguments > (i+1)) 
							_pathXmlData = args[++i];
						else  
						{
							_valid = false;
							ErrorMessage = "Invalid switch usage: " + argument;
						}
						break;
					case "-t":
					case "-template":
						_mode = ProcessMode.Template;
						if (numberOfArguments > (i+1)) 
							PathTemplate = args[++i];
						else 
						{
							_valid = false;
							ErrorMessage = "Invalid switch usage: " + argument;
						}
						break;
					case "-c":
					case "-collect":
						_mode = ProcessMode.Template;
						if (numberOfArguments > (i+1)) 
							_pathCollectXmlData = args[++i];
						else 
						{
							_valid = false;
							ErrorMessage = "Invalid switch usage: " + argument;
						}
						break;
					case "-e":
					case "-timeout":
						if (numberOfArguments > (i+1)) 
						{
							try 
							{
								_timeout = Int32.Parse(args[++i]);
							}
							catch 
							{
								_timeout = -1;
							}
						}
						else 
						{
							_valid = false;
							ErrorMessage = "Invalid switch usage: " + argument;
						}
						break;
					default:
						_valid = false;
						ErrorMessage = "Invalid argument: " + argument;
						break;
				}
			}

			if (_makeRelative) 
			{
				if (_pathCollectXmlData != null)
					_pathCollectXmlData = Zeus.FileTools.MakeAbsolute(_pathCollectXmlData, FileTools.ApplicationPath);
				if (PathLog != null)
                    PathLog = Zeus.FileTools.MakeAbsolute(PathLog, FileTools.ApplicationPath);
				if (PathOutput != null)
                    PathOutput = Zeus.FileTools.MakeAbsolute(PathOutput, FileTools.ApplicationPath);
				if (_pathProject != null)
                    _pathProject = Zeus.FileTools.MakeAbsolute(_pathProject, FileTools.ApplicationPath);
				if (PathTemplate != null)
                    PathTemplate = Zeus.FileTools.MakeAbsolute(PathTemplate, FileTools.ApplicationPath);
				if (_pathXmlData != null)
                    _pathXmlData = Zeus.FileTools.MakeAbsolute(_pathXmlData, FileTools.ApplicationPath);
                if (MetaFile1 != null)
                    MetaFile1 = Zeus.FileTools.MakeAbsolute(MetaFile1, FileTools.ApplicationPath);
                if (MetaFile2 != null)
                    MetaFile2 = Zeus.FileTools.MakeAbsolute(MetaFile2, FileTools.ApplicationPath);
                if (MetaFileMerged != null)
                    MetaFileMerged = Zeus.FileTools.MakeAbsolute(MetaFileMerged, FileTools.ApplicationPath);
			}

			// Validate required fields are filled out for the selected mode.
			if (_valid) 
			{
                if (Mode == ProcessMode.MyMeta) 
                {
                    if (MergeMetaFiles)
                    {
                        if (!System.IO.File.Exists(MetaFile1) || !System.IO.File.Exists(MetaFile2))
                        {
                            _valid = false;
                            ErrorMessage = "The two source files must exist for the merge to work!";
                        }
                    }
                }
				else if (_mode == ProcessMode.Project) 
				{
					if (_pathProject == null)
					{
						_valid = false;
						ErrorMessage = "Project Path Required";
					}
					else 
					{
						try 
						{
							Project = new ZeusProject(_pathProject);
							Project.Load();
						}
						catch (Exception ex)
						{
							Project = null;
							_valid = false;
							ErrorMessage = ex.Message;
						}
					}


                    if (PathTemplate != null)
                    {
                        try
                        {
                            Template = new ZeusTemplate(PathTemplate);
                        }
                        catch (Exception ex)
                        {
                            Template = null;
                            _valid = false;
                            ErrorMessage = ex.Message;
                        }
                    }
				}
				else if (_mode == ProcessMode.Template) 
				{
					if ( (PathTemplate == null) && (_pathXmlData == null) )
					{
						_valid = false;
						ErrorMessage = "Template path or XML input path required.";
					}
					else 
					{
						if (PathTemplate != null)
						{
							try 
							{
								Template = new ZeusTemplate(PathTemplate);
							}
							catch (Exception ex)
							{
								Template = null;
								_valid = false;
								ErrorMessage = ex.Message;
							}
						}

						if ( (_valid) && (_pathXmlData != null) )
						{
							try 
							{
								SavedInput = new ZeusSavedInput(_pathXmlData);
								SavedInput.Load();

								if (Template == null) 
								{
									Template = new ZeusTemplate(SavedInput.InputData.TemplatePath);
								}
							}
							catch (Exception ex)
							{
								SavedInput = null;
								Template = null;
								_valid = false;
								ErrorMessage = ex.Message;
							}
						}

						if ( (_valid) && (_pathCollectXmlData != null) )
						{
							try 
							{
								InputToSave = new ZeusSavedInput(_pathCollectXmlData);
								InputToSave.InputData.TemplatePath = Template.FilePath + Template.FileName;
								InputToSave.InputData.TemplateUniqueID = Template.UniqueID;
							}
							catch (Exception ex)
							{
								InputToSave = null;
								_valid = false;
								ErrorMessage = ex.Message;
							}
						}
					}
				}

			}
		}

	    private ProcessMode _mode = ProcessMode.Other;
	    public ProcessMode Mode
	    {
	        get { return _mode; }
	    }

	    private bool _internalUse = true;
	    public bool InternalUseOnly
	    {
	        get { return _internalUse; }
	    }

	    private int _timeout = -1;
	    public int Timeout
	    {
	        get { return _timeout; }
	    }

	    private bool _valid = true;
        public bool IsValid
	    {
	        get { return _valid; }
	    }

	    private readonly List<ZeusIntrinsicObject> _intrinsicObjects = new List<ZeusIntrinsicObject>();
	    public List<ZeusIntrinsicObject> IntrinsicObjects
	    {
	        get { return _intrinsicObjects; }
	    }

	    private readonly List<ZeusIntrinsicObject>  _intrinsicObjectsToRemove = new List<ZeusIntrinsicObject>();
	    public List<ZeusIntrinsicObject> IntrinsicObjectsToRemove
	    {
	        get { return _intrinsicObjectsToRemove; }
	    }

	    private readonly List<string> _moduleNames = new List<string>();
	    public List<string> ModuleNames
	    {
	        get { return _moduleNames; }
	    }

	    private readonly List<string> _projectItems = new List<string>();
	    public List<string> ProjectItems
	    { get { return _projectItems; } }

	    public bool IsSilent { get; private set; }
		public bool ShowHelp { get; private set; }
		public bool EnableLog { get; private set; }
	    public string ErrorMessage { get; private set; }
	    public string PathTemplate { get; private set; }
	    public string PathOutput { get; private set; }
	    public string PathLog { get; private set; }
		public ZeusTemplate Template { get; private set; }
	    public ZeusProject Project { get; private set; }
	    public ZeusSavedInput SavedInput { get; private set; }
	    public ZeusSavedInput InputToSave { get; private set; }
	    public string ConnectionType { get; private set; }
	    public string ConnectionString { get; private set; }
	    public bool MergeMetaFiles { get; private set; }
	    public string MetaFile1 { get; private set; }
	    public string MetaDatabase1 { get; private set; }
	    public string MetaFile2 { get; private set; }
	    public string MetaDatabase2 { get; private set; }
	    public string MetaFileMerged { get; private set; }
	    public string ProjectItemToRecord { get; private set; }
	}
}
