using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Diagnostics;
using Zeus;
using Zeus.Configuration;
using Zeus.Projects;
using Zeus.Serializers;
using Zeus.UserInterface;
using MyGeneration;
using MyMeta;
using System.Xml;

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

        public List<IAppRelease> ReleaseList
        {
            get
            {
                List<IAppRelease> releases = new List<IAppRelease>();
                XmlDocument xmldoc = Zeus.HttpTools.GetXmlFromUrl(DefaultSettings.Instance.VersionRSSUrl, DefaultSettings.Instance.WebProxy);
                XmlNodeList nodes = xmldoc.SelectNodes("//item");
                foreach (XmlNode node in nodes)
                {
                    AppRelease r = new AppRelease();

                    XmlNode titleNode = node.SelectSingleNode("./title");
                    XmlNode descriptionNode = node.SelectSingleNode("./description");
                    XmlNode linkNode = node.SelectSingleNode("./link");
                    XmlNode authorNode = node.SelectSingleNode("./author");
                    XmlNode pubDateNode = node.SelectSingleNode("./pubDate");

                    if (titleNode != null)
                    {
                        r.Title = titleNode.InnerText;
                    }
                    if (descriptionNode != null)
                    {
                        r.Title = descriptionNode.InnerText;
                    }
                    if (authorNode != null)
                    {
                        r.Author = authorNode.InnerText;
                    }
                    if (linkNode != null)
                    {
                        r.ReleaseNotesLink = new Uri(linkNode.InnerText);
                        r.DownloadLink = new Uri(FindSourceForgeDownloadUrl(linkNode.InnerText));
                    }
                    if (pubDateNode != null)
                    {
                        r.Date = DateTime.Parse(pubDateNode.InnerText);
                    }

                    releases.Add(r);
                }
                return releases;
            }
        }

        private string FindSourceForgeDownloadUrl(string starturl)
        {
            string groupId = DefaultSettings.Instance.VersionRSSUrl.Substring(DefaultSettings.Instance.VersionRSSUrl.IndexOf("group_id"));
            string newurl = starturl.Replace("shownotes", "showfiles") + "&" + groupId;
            string tokenToFind = "http://downloads.sourceforge.net/mygeneration/";
            string u = Zeus.HttpTools.GetTextFromUrl(newurl, DefaultSettings.Instance.WebProxy);
            int i = 0, i2 = 0, j, k, l;
            try
            {
                do
                {
                    i = u.IndexOf(tokenToFind, i);
                    if (i > 0)
                    {
                        j = u.IndexOf('\"', i);
                        if (j > i)
                        {
                            k = u.IndexOf(">", j);
                            if (k > j)
                            {
                                l = u.IndexOf("</a>", k);

                                string url = u.Substring(i, (j - i));
                                string filename = u.Substring(k + 1, (l - k-1));

                                i += tokenToFind.Length;
                                if (!filename.Contains("plugin") && filename.StartsWith("mygen") && filename.EndsWith(".exe"))
                                {
                                    newurl = url;
                                    break;
                                }
                            }
                        }
                    }
                } while (i >= 0);
            }
            catch (Exception ex)
            {
                // do something with the exception?
                throw ex;
            }
            return newurl;
        }

    }
    public class AppRelease : IAppRelease
    {
        private string _title, _description, _author;
        private Uri _downloadLink, _releaseNotesLink;
        private DateTime _date;

        public string Title { get { return _title;  } set { _title = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public string Author { get { return _author; } set { _author = value; } }
        public Uri DownloadLink { get { return _downloadLink; } set { _downloadLink = value; } }
        public Uri ReleaseNotesLink { get { return _releaseNotesLink; } set { _releaseNotesLink = value; } }
        public DateTime Date { get { return _date; } set { _date = value; } }
    }
}
