using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;

namespace Zeus.Projects
{
	/// <summary>
	/// Summary description for ZeusModule.
	/// </summary>
	public class ZeusProject : ZeusModule
	{
		private string _path;

		public ZeusProject() : base() {}

		public ZeusProject(string path) : base() 
		{
			//load from file!
			this._path = path;
		}

		public bool Save() 
		{
			if (File.Exists(this._path)) 
			{
				FileAttributes fa = File.GetAttributes(this._path);

				if ((FileAttributes.ReadOnly & fa) == FileAttributes.ReadOnly) 
					throw new Exception(this._path + " is read only!");
			}
			StreamWriter sw = new StreamWriter(this._path, false);
			XmlTextWriter xml = new XmlTextWriter(sw.BaseStream as Stream, Encoding.UTF8);
			xml.Formatting = Formatting.Indented;
			xml.WriteStartDocument();

			this.BuildXML(xml);

			xml.Flush();
			xml.Close();

			return true;
		}

		override public ZeusModule ParentModule
		{
			get { return null; }
		}

		public string FilePath
		{
			get { return _path; }
			set { _path = value; }
		}


		public bool Load() 
		{
			if (!File.Exists(this._path)) 
			{
				throw new Exception("Project file not found: " + this._path);
			}		
			else
			{
				bool inStartElement = true;
				string tagName;
				string projectDirPath = this._path.Substring(0, (this._path.LastIndexOf('\\') + 1));

				XmlTextReader xr = new XmlTextReader(File.OpenText(this._path));

				while (xr.Read()) 
				{
					inStartElement = xr.IsStartElement();
					tagName = xr.LocalName;

					if (inStartElement && ((tagName == "project") || (tagName == "module")) )
					{
						tagName = this.ReadXML(xr);
					}
				}
				xr.Close();

				return true;
			}
		}
	}
}
