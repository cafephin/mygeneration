using System;
using System.Text;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace Zeus.Projects
{
	/// <summary>
	/// Summary description for ZeusModule.
	/// </summary>
	public class ZeusModule
	{
		private string _name;
		private string _description;
        private bool _defaultSettingsOverride = false;
		private InputItemCollection _items;
		private SavedTemplateInputCollection _objs;
		private ZeusModuleCollection _modules;
        private ZeusModule _parentModule;
        private List<string> _filesChanged;
        private InputItemCollection _userItems;

		public ZeusModule() {}

		public string Name 
		{
			get { return _name; }
			set { _name = value; }
        }

        public string ProjectPath
        {
            get 
            {
                Stack<string> stack = new Stack<string>();
                ZeusModule mm = this;
                do
                {
                    stack.Push(mm.Name);
                    mm = mm.ParentModule;
                } while (mm != null);

                StringBuilder sb = new StringBuilder();
                while (stack.Count > 0)
                {
                    sb.Append("/").Append(stack.Pop());
                }

                return sb.ToString(); 
            }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public bool DefaultSettingsOverride
        {
            get { return _defaultSettingsOverride; }
            set { _defaultSettingsOverride = value; }
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

        internal ZeusProject RootProject
        {
            get
            {
                ZeusModule m = this;
                do
                {
                    if (m.IsParentModule)
                    {
                        return m as ZeusProject;
                    }

                    m = this.ParentModule;
                } while (m != null);

                return null;
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

        public InputItemCollection UserSavedItems
        {
            get
            {
                if (_userItems == null)
                {
                    _userItems = new InputItemCollection();
                }
                return _userItems;
            }
            set
            {
                _userItems = value;
            }
        }

		public SavedTemplateInputCollection SavedObjects 
		{
			get 
			{
				if (_objs == null) 
				{
					_objs = new SavedTemplateInputCollection();
                    _objs.ApplyOverrideDataDelegate = new ApplyOverrideDataDelegate(ApplyRuntimeOverrides);
				}
				return _objs;
			}
			set 
			{
				_objs = value;
                _objs.ApplyOverrideDataDelegate = new ApplyOverrideDataDelegate(ApplyRuntimeOverrides);
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

        public List<string> SavedFiles
        {
            get
            {
                if (_filesChanged == null) _filesChanged = new List<string>();
                return this._filesChanged;
            }
        }

		public void Execute(int timeout, ILog log) 
		{
			log.Write("Executing {0} '{1}'", (this.IsParentModule ? "Project" : "Module"), this.Name);
			this.SavedObjects.Execute(timeout, log);
            this.ChildModules.Execute(timeout, log);

            foreach (string file in SavedObjects.SavedFiles)
            {
                if (!SavedFiles.Contains(file)) SavedFiles.Add(file);
            }

            foreach (string file in ChildModules.SavedFiles)
            {
                if (!SavedFiles.Contains(file)) SavedFiles.Add(file);
            }
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

            foreach (InputItem item in module.UserSavedItems)
            {
                input[item.VariableName] = item.DataObject;
            }

            if (module.RootProject.DefaultSettingsOverride)
            {
                Dictionary<string, string> ds = module.RootProject.GetDefaultSettings();

                foreach (string key in ds.Keys)
                {
                    input[key] = ds[key];
                }
            }
        }

        private static void FillZeusInputRuntimeRecursive(ZeusModule module, IZeusInput input, ref bool hasDoneDefSettings)
        {
            bool doSettings = false;
            if (!module.IsParentModule)
            {
                FillZeusInputRuntimeRecursive(module.ParentModule, input, ref hasDoneDefSettings);
            }

            if (!hasDoneDefSettings)
            {
                doSettings = module.DefaultSettingsOverride;
            }

            if (doSettings)
            {
                hasDoneDefSettings = true;

                Dictionary<string, string> ds = module.RootProject.GetDefaultSettings();

                foreach (string key in ds.Keys)
                {
                    input[key] = ds[key];
                }
            }

            foreach (InputItem item in module.UserSavedItems)
            {
                input[item.VariableName] = item.DataObject;
            }
        }

        public void PopulateZeusContext(IZeusContext context)
        {
            FillZeusInputRecursive(this, context.Input);
        }

        public void ApplyRuntimeOverrides(IZeusInput input)
        {
            bool hasDoneDefaults = false;
            FillZeusInputRuntimeRecursive(this, input, ref hasDoneDefaults);
        }

        public void BuildUserXML(XmlTextWriter xml)
        {
            xml.WriteStartElement((this._parentModule == null) ? "project" : "module");
            xml.WriteAttributeString("name", this.Name);

            if (_userItems != null)
                this.UserSavedItems.BuildXML(xml);

            if (_modules != null)
                this.ChildModules.BuildUserXML(xml);

            xml.WriteEndElement();
        }

		public void BuildXML(XmlTextWriter xml) 
		{
			xml.WriteStartElement( (this._parentModule == null) ? "project" : "module" );
			xml.WriteAttributeString("name", this.Name);
			xml.WriteAttributeString("description", this.Description);
			xml.WriteAttributeString("defaultSettingsOverride", this.DefaultSettingsOverride.ToString());
			
			if (_items != null) 
				this.SavedItems.BuildXML(xml);

			if (_objs != null) 
				this.SavedObjects.BuildXML(xml);

			if (_modules != null) 
				this.ChildModules.BuildXML(xml);

			xml.WriteEndElement();
        }

        public string ReadUserXML(XmlTextReader xr)
        {
            string tagName = string.Empty;
            bool inStartElement, inEmptyElement, skipread = false;

            inEmptyElement = xr.IsEmptyElement;

            if (!inEmptyElement)
            {
                while ((skipread) || (xr.Read()))
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
			                string name = xr.GetAttribute("name");
                            if (this.ChildModules.Contains(name))
                            {
                                ZeusModule module = this.ChildModules[name];
                                tagName = module.ReadUserXML(xr);
                                skipread = true;
                            }
                        }
                        // a saved item start
                        else if (tagName == "item")
                        {
                            InputItem item = new InputItem();
                            item.ReadXML(xr);

                            this.UserSavedItems.Add(item);
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

		public string ReadXML(XmlTextReader xr) 
		{
			string tagName = string.Empty;
			bool inStartElement, inEmptyElement, skipread = false;

			this.Name = xr.GetAttribute("name");
            this.Description = xr.GetAttribute("description");
            string tmp = xr.GetAttribute("defaultSettingsOverride");
            if (!string.IsNullOrEmpty(tmp))
            {
                this.DefaultSettingsOverride = Convert.ToBoolean(tmp);
            }

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
