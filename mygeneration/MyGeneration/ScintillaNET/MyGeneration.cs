using System;
using System.Xml.Serialization;
using System.Xml;

namespace MyGeneration.Configuration
{
	/// <summary>
	/// Summary description for ScNotepad.
	/// </summary>
	[Serializable]
    public class MyGeneration : Scintilla.Legacy.Configuration.ConfigFile 
	{
		[XmlArray("options"),XmlArrayItem("option")]
		public option[] options; 

		[XmlElement(ElementName = "Scintilla")]
		public Scintilla.Legacy.Configuration.Scintilla  scintilla;

        public override void init(Scintilla.Legacy.Configuration.ConfigurationUtility utility, Scintilla.Legacy.Configuration.ConfigFile theParent)
		{
			base.init (utility, theParent);

			// This will ensure we have a link for the 'master' scintilla configuration object.
			// 'cause after we've loaded all the dependant children, we gotta tell the scintilla
			// object who they are.
			if( theParent == null && scintilla == null )
				scintilla  = new Scintilla.Legacy.Configuration.Scintilla();

			if( scintilla != null )
				scintilla.init( utility, _parent );
		}

        protected override Scintilla.Legacy.Configuration.Scintilla ChildScintilla
		{
			get
			{
				return scintilla;
			}
		}


		public static MyGeneration PopulatedObject 
		{
			get
			{
				MyGeneration result = new MyGeneration();
				result._parent = null;
				result.filename = "filename.txt";
				result.includes = new Scintilla.Legacy.Configuration.include[1];
				result.includes[0] = new Scintilla.Legacy.Configuration.include();
				result.includes[0].file = "sample.xml";

				result.options = new option[2];
				result.options[0] = new option();
				result.options[0].name="name";
				result.options[0].val="val";
				result.options[1] = new option();
				result.options[1].name="name2";
				result.options[1].val="val2";
				
				result.scintilla = new Scintilla.Legacy.Configuration.Scintilla();
				result.scintilla.globals = new Scintilla.Legacy.Configuration.Value[1];
				result.scintilla.globals[0] = new Scintilla.Legacy.Configuration.Value();
				result.scintilla.globals[0].name = "test";
				result.scintilla.globals[0].val = "val";
				
				result.scintilla.includes = new Scintilla.Legacy.Configuration.include[1];
				result.scintilla.includes[0] = new Scintilla.Legacy.Configuration.include();
				result.scintilla.includes[0].file = "xml.test";

				System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(typeof(MyGeneration));
				System.IO.StringWriter sw = new System.IO.StringWriter();
 
				mySerializer.Serialize(sw, result);

				String x = sw.ToString();
				
				return result;
			}
		}
	}

	
}
