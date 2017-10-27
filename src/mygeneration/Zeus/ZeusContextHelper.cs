using System.Reflection;
using MyGeneration.Configuration;
using Zeus;

namespace MyGeneration
{
    public class ZeusContextHelper
    {
        public void PopulateZeusContext(IZeusContext context) 
		{
			var settings = new DefaultSettings();
			IZeusInput input = context.Input;

			if (!input.Contains("__version"))
			{
				Assembly ver = Assembly.GetEntryAssembly();
                if (ver != null)
                {
                    input["__version"] = ver.GetName().Version.ToString();
                }
			}
			
			//-- BEGIN LEGACY VARIABLE SUPPORT -----
			if (!input.Contains("defaultTemplatePath")) 
				input["defaultTemplatePath"] = settings.TemplateSettings.DefaultTemplateDirectory;
			if (!input.Contains("defaultOutputPath")) 
				input["defaultOutputPath"] = settings.TemplateSettings.DefaultOutputDirectory;
			//-- END LEGACY VARIABLE SUPPORT -------

			if (!input.Contains("__defaultTemplatePath")) 
				input["__defaultTemplatePath"] = settings.TemplateSettings.DefaultTemplateDirectory;

			if (!input.Contains("__defaultOutputPath")) 
				input["__defaultOutputPath"] = settings.TemplateSettings.DefaultOutputDirectory;

			if (!string.IsNullOrWhiteSpace(settings.DbConnectionSettings.Driver))
			{
				//-- BEGIN LEGACY VARIABLE SUPPORT -----
				if (!input.Contains("dbDriver")) 
					input["dbDriver"] = settings.DbConnectionSettings.Driver;
				if (!input.Contains("dbConnectionString")) 
					input["dbConnectionString"] = settings.MiscSettings.DomainOverride;
				//-- END LEGACY VARIABLE SUPPORT -------

				if (!input.Contains("__dbDriver"))
					input["__dbDriver"] = settings.DbConnectionSettings.Driver;
				
				if (!input.Contains("__dbConnectionString"))
					input["__dbConnectionString"] = settings.DbConnectionSettings.ConnectionString;

                if (!input.Contains("__showDefaultDatabaseOnly"))
                    input["__showDefaultDatabaseOnly"] = settings.DbConnectionSettings.ShowDefaultDatabaseOnly.ToString();
			
				if (!input.Contains("__domainOverride"))
					input["__domainOverride"] = settings.MiscSettings.DomainOverride;

				if (!string.IsNullOrWhiteSpace(settings.DbConnectionSettings.DbTarget) && !input.Contains("__dbTarget"))
					input["__dbTarget"] = settings.DbConnectionSettings.DbTarget;
			
				if (!string.IsNullOrWhiteSpace(settings.DbConnectionSettings.DbTargetMappingFile) && !input.Contains("__dbTargetMappingFileName"))
					input["__dbTargetMappingFileName"] = settings.DbConnectionSettings.DbTargetMappingFile;

				if (!string.IsNullOrWhiteSpace(settings.DbConnectionSettings.LanguageMappingFile) && !input.Contains("__dbLanguageMappingFileName"))
					input["__dbLanguageMappingFileName"] = settings.DbConnectionSettings.LanguageMappingFile;

				if (!string.IsNullOrWhiteSpace(settings.DbConnectionSettings.Language) && !input.Contains("__language"))
					input["__language"] = settings.DbConnectionSettings.Language;

				if (!string.IsNullOrWhiteSpace(settings.DbConnectionSettings.UserMetaDataFileName) && !input.Contains("__userMetaDataFileName"))
					input["__userMetaDataFileName"] = settings.DbConnectionSettings.UserMetaDataFileName;

                if (settings.DbConnectionSettings.UserDatabaseAliases.Count > 0)
                {
                    foreach (var databaseAlias in settings.DbConnectionSettings.UserDatabaseAliases)
                    {
                        input["__dbmap__" + databaseAlias.DatabaseName] = databaseAlias.Alias;
                    }
                }
			}
		}
    }
}
