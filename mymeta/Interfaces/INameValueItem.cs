using System;
using System.Runtime.InteropServices;
using System.Collections;

namespace MyMeta
{
	/// <summary>
	/// This interface allows all the collections here to be bound to 
	/// Name/Value collection type objects. with ease
	/// </summary>
	[GuidAttribute("8f28945e-77ca-4142-8f48-c4ed32f2d640"),InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface INameValueItem
	{
		string ItemName{ get; }
		string ItemValue{ get; }
	}
}

