using System;
using System.Xml;
using System.Collections;

namespace Zeus.Projects
{
	/// <summary>
	/// Summary description for ZeusModule.
	/// </summary>
	public class ZeusModule
	{
		private string _name;
		private string _description;
		private InputItemCollection _items;
		private SavedTemplateInputCollection _objs;
		private ZeusModuleCollection _modules;
		private ZeusModule _parentModule;

		public ZeusModule() {}

		public string Name 
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Description 
		{
			get { return _description; }
			set { _description = value; }
		}

		internal void SetParentModule(ZeusModule module) 
		{
			_parentModule = module;
		}

		virtual public ZeusModule ParentModule
		{
			get 
			{
				return _parentModule;
			}
		}

		public bool IsParentModule
		{
			get 
			{
				return (_parentModule == null);
			}
		}

		public InputItemCollection SavedItems
		{
			get 
			{
				if (_items == null) 
				{
					_items = new InputItemCollection();
				}
				return _items;
			}
			set 
			{
				_items = value;
			}
		}

		public SavedTemplateInputCollection SavedObjects 
		{
			get 
			{
				if (_objs == null) 
				{
					_objs = new SavedTemplateInputCollection();
				}
				return _objs;
			}
			set 
			{
				_objs = value;
			}
		}

		public ZeusModuleCollection ChildModules 
		{
			get 
			{
				if (_modules == null) 
				{
					_modules = new ZeusModuleCollection(this);
				}
				return _modules;
			}
			set 
			{
				_modules = value;
			}
		}

		public void Execute(int timeout, ILog log) 
		{
			log.Write("Executing {0} '{1}'", (this.IsParentModule ? "Project" : "Module"), this.Name);
			this.SavedObjects.Execute(timeout, log);
			this.ChildModules.Execute(timeout, log);
		}

		private static void FillZeusInputRecursive(ZeusModule module, IZeusInput input) 
		{
			if (!module.IsParentModule) 
			{
				FillZeusInputRecursive(module.ParentModule, input);
			}

			foreach (InputItem item in module.SavedItems) 
			{
				input[item.VariableName] = item.DataObject;
			}
		}

		public void PopulateZeusContext(IZeusContext context) 
		{
			FillZeusInputRecursive(this, context.Input);
		}

		public void BuildXML(XmlTextWriter xml) 
		{
			xml.WriteStartElement( (this._parentModule == null) ? "project" : "module" );
			xml.WriteAttributeString("name", this.Name);
			xml.WriteAttributeString("description", this.Description);
			
			if (_items != null) 
				this.SavedItems.BuildXML(xml);

			if (_objs != null) 
				this.SavedObjects.BuildXML(xml);

			if (_modules != null) 
				this.ChildModules.BuildXML(xml);

			xml.WriteEndElement();
		}

		public string ReadXML(XmlTextReader xr) 
		{
			string tagName = string.Empty;
			bool inStartElement, inEmptyElement, skipread = false;

			this.Name = xr.GetAttribute("name");
			this.Description = xr.GetAttribute("description");

			inEmptyElement = xr.IsEmptyElement;

			if (!inEmptyElement) 
			{
				while ( (skipread) || (xr.Read()) ) 
				{
					inStartElement = xr.IsStartElement();
					inEmptyElement = xr.IsEmptyElement;
				
					if (skipread) 
					{
						skipread = false;
					}
					else
					{
						tagName = xr.LocalName;
					}

					if (inStartElement) 
					{
						// a module start
						if (tagName == "module") 
						{
							ZeusModule module = new ZeusModule();
							tagName = module.ReadXML(xr);
							skipread = true;
						
							module.SetParentModule(this);
							this.ChildModules.Add(module);

						}
							// a saved item start
						else if (tagName == "item") 
						{
							InputItem item = new InputItem();
							item.ReadXML(xr);
						
							this.SavedItems.Add(item);

						}
							// a saved object start
						else if (tagName == "obj") 
						{
							SavedTemplateInput input = new SavedTemplateInput();
							tagName = input.ReadXML(xr);
							skipread = true;
						
							this.SavedObjects.Add(input);
						}
					}
					else
					{
						// if not in a sub module and this is an end module tag, break!
						if (tagName == "module") 
						{
							break;
						}
					}				 
				}
			}

			xr.Read();
			inStartElement = xr.IsStartElement();
			tagName = xr.LocalName;

			return tagName;
		}
	}
}
