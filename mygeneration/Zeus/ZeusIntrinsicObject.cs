using System;

namespace Zeus
{
	/// <summary>
	/// Summary description for ZeusScriptableObject.
	/// </summary>
	public class ZeusIntrinsicObject : IZeusIntrinsicObject
	{
		protected string _variableName;
		protected string _classPath;
		protected string _assemblyPath;
		protected string _namespace = null;
		protected string _dllref = null;

		/*internal ZeusIntrinsicObject(string line) 
		{
			string[] items = line.Split(',');
			if (items.Length == 2) 
			{
				this._assemblyPath = string.Empty;
				this._classPath = items[0];
				this._variableName = items[1];
			}
			else if (items.Length == 3) 
			{
				this._assemblyPath = items[0];
				this._classPath = items[1];
				this._variableName = items[2];
			}
			else 
			{
				throw new Exception("Invalid entry in ZeusScriptingObjects.zcfg");
			}
		}*/

		public ZeusIntrinsicObject(string assemblyPath, string classPath, string variableName)
		{
			if (assemblyPath == null) assemblyPath = string.Empty;
			this._assemblyPath = assemblyPath;
			this._classPath = classPath;
			this._variableName = variableName;
		}

		public string DllReference
		{
			get
			{
				if (_dllref == null)
				{
					_dllref = this.AssemblyPath;
					int idx = _dllref.LastIndexOf("\\");
					if (idx > 0) _dllref = _dllref.Substring(idx + 1);
					if (_dllref == string.Empty) _dllref = null;
				}
				return _dllref;
			}
		}

		public string Namespace
		{
			get
			{
				if (_namespace == null)
				{
					_namespace = this.ClassPath;
					int idx = _namespace.LastIndexOf(".");
					if (idx > 0) _namespace = _namespace.Substring(0, idx);
					if (_namespace == string.Empty) _namespace = null;
				}
				return _namespace;
			}
		}

		public string VariableName
		{
			get { return _variableName; }
		}
		public string ClassPath
		{
			get { return _classPath; }
		}

		public string AssemblyPath
		{
			get { return _assemblyPath; }
		}
	}
}
