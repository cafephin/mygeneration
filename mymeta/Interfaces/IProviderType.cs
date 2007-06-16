using System;
using System.Runtime.InteropServices;

namespace MyMeta
{
	[GuidAttribute("4ce1ed26-0164-4b9a-8e53-8ae5288c6add"),InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IProviderType 
	{
		string Type { get; }
		System.Int32 DataType { get; }
		System.Int32 ColumnSize { get; }
		string LiteralPrefix { get; }
		string LiteralSuffix { get; }
		string CreateParams { get; }
		System.Boolean IsNullable { get; }
		System.Boolean IsCaseSensitive { get; }
		string Searchable { get; } // convert
		System.Boolean IsUnsigned { get; }
		System.Boolean HasFixedPrecScale { get; }
		System.Boolean CanBeAutoIncrement { get; }
		string LocalType { get; }
		System.Int32 MinimumScale { get; }
		System.Int32 MaximumScale { get; }
		Guid TypeGuid { get; }
		string TypeLib { get; }
		string Version { get; }
		System.Boolean IsLong { get; }
		System.Boolean BestMatch { get; }
		System.Boolean IsFixedLength { get; }
	}
}

