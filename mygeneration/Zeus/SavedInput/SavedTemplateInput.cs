using System;
using System.Text;
using System.Xml;
using System.Collections;
using Zeus;
using Zeus.UserInterface;

namespace Zeus
{
	/// <summary>
	/// Summary description for SavedTemplateInput.
	/// </summary>
	public class SavedTemplateInput
	{
		private string _savedObjectName;
		private string _templatePath;
		private string _templateUniqueID;
		private InputItemCollection _inputItems;

		public SavedTemplateInput(string objectName, IZeusTemplate template)
		{
			_savedObjectName = objectName;
			_templateUniqueID = template.UniqueID;
			_templatePath = template.FilePath + template.FileName;
		}

		public SavedTemplateInput(string objectName, IZeusTemplate template, IZeusInput input) : this(objectName, template)
		{
			_inputItems = new InputItemCollection(input);
		}

		public SavedTemplateInput(string objectName, string uniqueid, string filepath)
		{
			_savedObjectName = objectName;
			_templateUniqueID = uniqueid;
			_templatePath = filepath;
		}

		public SavedTemplateInput() {}
		
		public string TemplateUniqueID 
		{
			get { return _templateUniqueID; }
			set { _templateUniqueID = value; }
		}

		public string TemplatePath 
		{
			get { return _templatePath; }
			set { _templatePath = value; }
		}

		public string SavedObjectName 
		{
			get { return _savedObjectName; }
			set { _savedObjectName = value; }
		}

		public InputItemCollection InputItems 
		{
			get 
			{
				if (_inputItems == null) 
				{
					_inputItems = new InputItemCollection();
				}
				return _inputItems;
			}
			set 
			{
				_inputItems = value;
			}
		}

		#region Xml Related Methods
		public void BuildXML(XmlTextWriter xml) 
		{
			xml.WriteStartElement("obj");
			xml.WriteAttributeString("name", this._savedObjectName);
			xml.WriteAttributeString("uid", this._templateUniqueID);
			xml.WriteAttributeString("path", this._templatePath);
			
			this.InputItems.BuildXML(xml);

			xml.WriteEndElement();
		}

		public string ReadXML(XmlTextReader xr) 
		{
			string tagName;
			bool inStartElement;

			this._savedObjectName = xr.GetAttribute("name");
			this._templateUniqueID = xr.GetAttribute("uid");
			this._templatePath = xr.GetAttribute("path");

			while (xr.Read()) 
			{
				inStartElement = xr.IsStartElement();
				tagName = xr.LocalName;

				if (inStartElement) 
				{
					// a module start
					if (tagName == "item") 
					{
						InputItem item = new InputItem();

						item.ReadXML(xr);
						
						this.InputItems.Add(item);
					}
				}
				else 
				{
					// if not in a sub object and this is an end object tag, break!
					if (tagName == "obj") 
					{
						break;
					}
				}				 
			}

			xr.Read();
			inStartElement = xr.IsStartElement();
			tagName = xr.LocalName;

			return tagName;
		}
		#endregion


		public void Execute(int timeout, ILog log)
		{
			log.Write("Executing Template Instance '{0}'", this.SavedObjectName);

			string path = FileTools.ResolvePath(this.TemplatePath);
			
			ZeusTemplate template = new ZeusTemplate(path);
			
			ZeusInput zin = new ZeusInput();
			zin.AddItems(this.InputItems);

			ZeusContext context = new ZeusContext(zin, new GuiController(), new Hashtable());
			context.Log = log;

			template.Execute(context, timeout, true);
		}

		public SavedTemplateInput Copy() 
		{
			SavedTemplateInput copy = new SavedTemplateInput("Copy of " + this.SavedObjectName, this.TemplateUniqueID, this.TemplatePath);
			copy.InputItems = this.InputItems.Copy();
			return copy;
		}
	}
}
