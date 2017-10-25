using System.Collections.Generic;
using MyGeneration;
using MyGeneration.Configuration;
using Zeus.Projects;

namespace Zeus
{
    public class ZeusController : IZeusController
    {
        private static ZeusController _instance;
        public static ZeusController Instance 
        {
            get { return _instance ?? (_instance = new ZeusController()); }
        }

        private ZeusController() { }

        public IZeusSavedInput CollectTemplateInput(IZeusContext context, string templatePath)
        {
            var template = new ZeusTemplate(templatePath);
            var collectedInput = new ZeusSavedInput();
            
            DefaultSettings.Instance.PopulateZeusContext(context);
            template.Collect(context, DefaultSettings.Instance.ScriptTimeout, collectedInput.InputData.InputItems);
            collectedInput.Save();

            return collectedInput;
        }

        public IZeusSavedInput ExecuteTemplateAndCollectInput(IZeusContext context, string templatePath)
        {
            var template = new ZeusTemplate(templatePath);
            var collectedInput = new ZeusSavedInput();
            
            DefaultSettings.Instance.PopulateZeusContext(context);
            template.ExecuteAndCollect(context, DefaultSettings.Instance.ScriptTimeout, collectedInput.InputData.InputItems);
            collectedInput.Save();

            return collectedInput;
        }

        public void ExecuteTemplate(IZeusContext context, string templateFilePath)
        {
            ExecuteTemplate(context, templateFilePath, null);
        }

        public void ExecuteTemplate(IZeusContext context, string templatePath, string inputFilePath)
        {
            var template = new ZeusTemplate(templatePath);
            ZeusSavedInput savedInput = null;
            
            if (!string.IsNullOrEmpty(inputFilePath))
            {
                savedInput = new ZeusSavedInput(inputFilePath);
            }

            context.Log.Write("Executing: " + template.Title);
            if (savedInput != null)
            {
                context.Input.AddItems(savedInput.InputData.InputItems);
                template.Execute(context, DefaultSettings.Instance.ScriptTimeout, true);
            }
            else
            {
                DefaultSettings.Instance.PopulateZeusContext(context);
                template.Execute(context, DefaultSettings.Instance.ScriptTimeout, false);
            }
        }

        public void ExecuteProject(IZeusContext context, string projectFilePath)
        {
            ExecuteProjectModule(context, projectFilePath);
        }

        public void ExecuteProjectModule(IZeusContext context, string projectFilePath, params string[] modules)
        {
            var zeusProject = new ZeusProject(projectFilePath);
            
            if (modules.Length == 0)
            {
                context.Log.Write("Executing: " + zeusProject.Name);
                zeusProject.Execute(DefaultSettings.Instance.ScriptTimeout, context.Log);

            }
            else
            {
                foreach (var mod in modules)
                {
                    context.Log.Write("Executing: " + mod);
                    ExecuteModules(context, zeusProject, new List<string>(modules), DefaultSettings.Instance.ScriptTimeout);
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
