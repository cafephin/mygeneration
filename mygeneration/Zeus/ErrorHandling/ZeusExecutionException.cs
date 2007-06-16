using System;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace Zeus.ErrorHandling
{
	/// <summary>
	/// Summary description for ZeusCompilerException.
	/// </summary>
	public class ZeusExecutionException : ZeusException
	{
		protected IZeusExecutionError[] _errors;
		private bool _isTemplateScript = true;

		public ZeusExecutionException(IZeusExecutionError[] errors, bool isTemplateScript) : base("Template Execution Error")
		{
			this._errors = errors;
			this._isTemplateScript = isTemplateScript;
		}

		public IZeusExecutionError[] Errors 
		{
			get 
			{
				return _errors;
			}
		}

		public bool IsTemplateScript
		{
			get 
			{
				return _isTemplateScript;
			}
			set 
			{
				_isTemplateScript = value;
			}
		}

	}
} 
