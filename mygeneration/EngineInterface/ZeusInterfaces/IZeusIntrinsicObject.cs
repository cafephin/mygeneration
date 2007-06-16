using System;
using System.Collections;

namespace Zeus
{
	public interface IZeusIntrinsicObject
	{
		string VariableName { get; }
		string ClassPath { get; }
		string AssemblyPath { get; }
		string DllReference { get; }
		string Namespace { get; }
	}
}
