using System;
using System.Collections;
using MyGeneration;

namespace Zeus
{
	public interface IZeusController
	{
        IZeusSavedInput CollectTemplateInput(IZeusContext context, string templatePath);
        IZeusSavedInput ExecuteTemplateAndCollectInput(IZeusContext context, string templatePath);
        void ExecuteTemplate(IZeusContext context, string templateFilePath);
        void ExecuteTemplate(IZeusContext context, string templatePath, string inputFilePath);
        void ExecuteProject(IZeusContext context, string projectFilePath);
        void ExecuteProjectModule(IZeusContext context, string projectFilePath, params string[] modules);
	}
}