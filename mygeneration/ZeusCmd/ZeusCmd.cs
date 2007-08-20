using System;
using System.Text;
using System.Data;
using System.Collections;
using System.IO;
using Zeus;
using Zeus.Configuration;
using Zeus.Projects;
using Zeus.Serializers;
using Zeus.UserInterface;
using MyGeneration;
using MyMeta;

namespace Zeus
{
	/// <summary>
	/// Summary description for ZeusCmd.
	/// </summary>
	class ZeusCmd
	{
		private Log _log;
		private CmdInput _argmgr;
		private int _returnValue = 0;

		public ZeusCmd(string[] args) 
		{
			_log = new Log();
			if (_ProcessArgs(args)) 
			{
				switch (_argmgr.Mode) 
				{
					case ProcessMode.Project:
						_ProcessProject();
						break;
					case ProcessMode.Template:
						_ProcessTemplate();
						break;
					case ProcessMode.MyMeta:
						_ProcessMyMeta();
						break;
				}
			}
			else 
			{
				_returnValue = -1;
			}
			_log.Close();
		}

		public int ReturnValue { get { return _returnValue; } }

		private bool _ProcessArgs(string[] args) 
		{
			//Process arguments, validate, fill variables
			_argmgr = new CmdInput(args);

			if (_argmgr.ShowHelp) 
			{
				Console.Write(HELP_TEXT);
				return false;
			}

			if (_argmgr.IntrinsicObjects.Count > 0) 
			{
				ZeusConfig cfg = ZeusConfig.Current;
				foreach (object obj in _argmgr.IntrinsicObjects) 
				{
					ZeusIntrinsicObject io = null;

					if (obj is ZeusIntrinsicObject) 
					{
						io = obj as ZeusIntrinsicObject;
						bool exists = false;
						foreach (ZeusIntrinsicObject existingObj in cfg.IntrinsicObjects) 
						{
							if (existingObj.VariableName == io.VariableName) 
							{
								exists = true;
								break;
							}
						}

						if (!exists) 
						{
							cfg.IntrinsicObjects.Add(io);
						}
					}
					else if (obj is String) 
					{
						string varName = (string)obj;
						foreach (ZeusIntrinsicObject existingObj in cfg.IntrinsicObjects) 
						{
							if (existingObj.VariableName == varName) 
							{
								io = existingObj;
								break;
							}
						}
						if (io != null) 
						{
							cfg.IntrinsicObjects.Remove(io);
						}
					}
				}
				cfg.Save();
			}
			
			if (_argmgr.IsValid) 
			{
				if (_argmgr.EnableLog) 
				{
					_log.IsLogEnabled = true;
					_log.FileName = _argmgr.PathLog;
				}

				_log.IsConsoleEnabled = !_argmgr.IsSilent;
				return true;
			}
			else 
			{
				Console.WriteLine(_argmgr.ErrorMessage);
				Console.Write("Use the \"-?\" switch to view the help.");
				return false;
			}
		}

		private void _ProcessMyMeta() 
		{
			IDbConnection connection = null;
			try 
			{
				dbRoot mymeta = new dbRoot();
				connection = mymeta.BuildConnection(_argmgr.ConnectionType, _argmgr.ConnectionString);
				_log.Write("Beginning test for {0}: {1}", connection.GetType().ToString(), _argmgr.ConnectionString);
				connection.Open();
				connection.Close();
				_log.Write("Test Successful");
			}
			catch (Exception ex)
			{
				_log.Write("Test Failed");
				if (_log != null) _log.Write(ex);
				_returnValue = -1;
			}

			if (connection != null) 
			{
				connection.Close();
				connection = null;
			}
		}

		private void _ProcessProject() 
		{
			ZeusProject proj = this._argmgr.Project;
			
			this._log.Write("Begin Project Processing: " + proj.Name);
			if (this._argmgr.ModuleNames.Count == 0) 
			{
				this._log.Write("Executing: " + proj.Name);
				try 
				{
					proj.Execute(this._argmgr.Timeout, this._log);
				}
				catch (Exception ex)
				{
					this._log.Write(ex);
					this._log.Write("Project execution failed.");
				}
			}
			else 
			{
				foreach (string mod in _argmgr.ModuleNames) 
				{
					this._log.Write("Executing: " + mod);
					try 
					{
						ExecuteModules(proj, _argmgr.ModuleNames);
					}
					catch (Exception ex)
					{
						this._log.Write(ex);
						this._log.Write("Module execution failed.");
					}
				}
			}
			this._log.Write("End Project Processing: " + proj.Name);
		}

		private void ExecuteModules(ZeusModule parent, ArrayList names) 
		{
			foreach (ZeusModule module in parent.ChildModules) 
			{
				if (names.Contains(module.Name)) 
				{
					module.Execute(this._argmgr.Timeout, this._log);
				}
				else 
				{
					ExecuteModules(module, names);
				}
			}
		}

		private void _ProcessTemplate() 
		{
			ZeusTemplate template = this._argmgr.Template;
			ZeusSavedInput savedInput = this._argmgr.SavedInput;
			ZeusSavedInput collectedInput = this._argmgr.InputToSave;
			ZeusContext context = new ZeusContext();
			context.Log = _log;
			DefaultSettings settings;
			
			this._log.Write("Executing: " + template.Title);
			try 
			{
				if (savedInput != null) 
				{
					context.Input.AddItems(savedInput.InputData.InputItems);
					template.Execute(context, this._argmgr.Timeout, true);
				}
				else if (collectedInput != null) 
				{
					settings = DefaultSettings.Instance;
					settings.PopulateZeusContext(context);
					template.ExecuteAndCollect(context, this._argmgr.Timeout, collectedInput.InputData.InputItems);
					collectedInput.Save();
				}
				else 
				{
					settings = DefaultSettings.Instance;
					settings.PopulateZeusContext(context);
					template.Execute(context, this._argmgr.Timeout, false);
				}
				
				if (this._argmgr.PathOutput != null) 
				{
					StreamWriter writer = File.CreateText(this._argmgr.PathOutput);
					writer.Write(context.Output.text);
					writer.Flush();
					writer.Close();
				}
				else 
				{
					if (!_argmgr.IsSilent)
						Console.WriteLine(context.Output.text);
				}
			}
			catch (Exception ex)
			{
				this._log.Write(ex);
				this._log.Write("Template execution failed.");
			}
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			ZeusCmd cmd = new ZeusCmd(args);
			return cmd.ReturnValue;
		}

		#region Help Text
		public const string HELP_TEXT = @"
|=======================================================================
| ZeusCmd.exe: Switches, arguments, etc.
|=======================================================================
| General switches
|-----------------------------------------------------------------------
| -?, -h                               | show usage text
| -s                                   | silent mode (no console output)
| -r					               | make paths relative to exe
| -l <logpath>                         | log process events and errors
| -e <integer>                         | template execution timeout
|-----------------------------------------------------------------------
| Project switches
|-----------------------------------------------------------------------
| -p <projectpath>                     | generate a template
| -m <modulename>                      | regenerate a specific module
|-----------------------------------------------------------------------
| Template switches
|-----------------------------------------------------------------------
| -i <xmldatapath>                     | xml input file
| -t <templatepath>                    | template file path
| -o <outputpath>                      | output path
| -c <saveinputpath>                   | collect input and save to file
|-----------------------------------------------------------------------
| Intrinsic Object switches
|-----------------------------------------------------------------------
| -aio <dllpath> <classpath> <varname> | add an intrinsic object
| -rio <varname>                       | remove an intrinsic object
|-----------------------------------------------------------------------
| MyMeta switches
|-----------------------------------------------------------------------
| -tc <providername> <connectstring>   | test a database connection
|=======================================================================
| EXAMPLE 1
| Execute a template:
|-----------------------------------------------------------------------
| ZeusCmd -t c:\template.jgen
|========================================================================
| EXAMPLE 2
| Execute a template, no console output, log to file:
|-----------------------------------------------------------------------
| ZeusCmd -s -t c:\template.jgen -l c:\zeuscmd.log
|========================================================================
| EXAMPLE 3
| Execute template, save input to an xml file:
|-----------------------------------------------------------------------
| ZeusCmd -t c:\template.jgen -c c:\savedInput.zinp
|========================================================================
| EXAMPLE 4
| Regenerate from the saved input in example 3 above:
|-----------------------------------------------------------------------
| ZeusCmd -i c:\savedInput.zinp
|========================================================================
| EXAMPLE 5
| Add an intrinsic object to the Zeus Configuration file.
|-----------------------------------------------------------------------
| ZeusCmd -aio TheDLLOfmyDreams.dll Lib.MyGen.Plugin.Utilities myVar
|========================================================================
| EXAMPLE 6
| Remove an intrinsic object to the Zeus Configuration file.
|-----------------------------------------------------------------------
| ZeusCmd -rio myVar
|========================================================================
| EXAMPLE 7
| Test a MyMeta Connection
|-----------------------------------------------------------------------
| ZeusCmd -tc SQL ""Provider=SQLOLEDB.1;User ID=sa;Data Source=localhost""
|========================================================================
";
		#endregion
	}
}
