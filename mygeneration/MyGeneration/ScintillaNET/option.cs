using System;
using System.Xml.Serialization ;
using System.Xml;


namespace MyGeneration.Configuration
{
	/// <summary>
	/// Summary description for option.
	/// </summary>
	[Serializable]
    public class option : Scintilla.Configuration.Legacy.ConfigItem 
	{
		[XmlAttributeAttribute]
		public string name;

		[XmlAttributeAttribute("value")]
		public string val;
	}
}
