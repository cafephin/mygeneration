using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using Zeus;
using Zeus.Configuration;
using Zeus.Projects;
using Zeus.Serializers;
using Zeus.UserInterface;
using MyGeneration;
using MyMeta;

namespace Zeus
{
    public class ZeusController : IZeusController
    {
        private static ZeusController _instance;
        public static ZeusController Instance 
        {
            get 
            {
                if (_instance == null) _instance = new ZeusController();
                return _instance;
            }
        }

        private ZeusController() { }

        public IZeusSavedInput CollectTemplateInput(IZeusContext context, string templatePath)
        {
            ZeusTemplate template = new ZeusTemplate(templatePath);
            ZeusSavedInput collectedInput = new ZeusSavedInput();
            DefaultSettings settings = DefaultSettings.Instance;

            settings.PopulateZeusContext(context);
            template.Collect(context, settings.ScriptTimeout, collectedInput.InputData.InputItems);
            collectedInput.Save();

            return collectedInput;
        }

        public IZeusSavedInput ExecuteTemplateAndCollectInput(IZeusContext context, string templatePath)
        {
            ZeusTemplate template = new ZeusTemplate(templatePath);
            ZeusSavedInput collectedInput = new ZeusSavedInput();
            DefaultSettings settings = DefaultSettings.Instance;

            settings.PopulateZeusContext(context);
            template.ExecuteAndCollect(context, settings.ScriptTimeout, collectedInput.InputData.InputItems);
            collectedInput.Save();

            return collectedInput;
        }

        public void ExecuteTemplate(IZeusContext context, string templateFilePath)
        {
            this.ExecuteTemplate(context, templateFilePath, null);
        }

        public void ExecuteTemplate(IZeusContext context, string templatePath, string inputFilePath)
        {
            ZeusTemplate template = new ZeusTemplate(templatePath);
            ZeusSavedInput savedInput = null;
            DefaultSettings settings = DefaultSettings.Instance;

            if (!string.IsNullOrEmpty(inputFilePath))
            {
                savedInput = new ZeusSavedInput(inputFilePath);
            }

            context.Log.Write("Executing: " + template.Title);
            if (savedInput != null)
            {
                context.Input.AddItems(savedInput.InputData.InputItems);
                template.Execute(context, settings.ScriptTimeout, true);
            }
            else
            {
                settings.PopulateZeusContext(context);
                template.Execute(context, settings.ScriptTimeout, false);
            }
        }

        public void ExecuteProject(IZeusContext context, string projectFilePath)
        {
            ExecuteProjectModule(context, projectFilePath);
        }

        public void ExecuteProjectModule(IZeusContext context, string projectFilePath, params string[] modules)
        {
            ZeusProject proj = new ZeusProject(projectFilePath);
            DefaultSettings settings = DefaultSettings.Instance;

            if (modules.Length == 0)
            {
                context.Log.Write("Executing: " + proj.Name);
                proj.Execute(settings.ScriptTimeout, context.Log);

            }
            else
            {
                foreach (string mod in modules)
                {
                    context.Log.Write("Executing: " + mod);
                    ExecuteModules(context, proj, new List<string>(modules), settings.ScriptTimeout);
                }
            }
        }

        private void ExecuteModules(IZeusContext context, ZeusModule parent, List<string> names, int timeout)
        {
            foreach (ZeusModule module in parent.ChildModules)
            {
                if (names.Contains(module.Name))
                {
                    module.Execute(timeout, context.Log);
                }
                else
                {
                    ExecuteModules(context, module, names, timeout);
                }
            }
        }

    }
}
