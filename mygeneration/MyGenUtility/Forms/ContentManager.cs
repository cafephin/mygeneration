using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace MyGeneration
{
    public abstract class ContentManager : IContentManager
    {
        #region Static factory type members
        public static void AddNewContentMenuItems(EventHandler onClickEvent, ToolStripMenuItem pluginsMenuItem, ToolStrip toolStrip)
        {
            pluginsMenuItem.DropDownItems.Clear();
            List<ToolStripItem> itemsToRemove = new List<ToolStripItem>();
            foreach (ToolStripItem tsi in toolStrip.Items) if (tsi.Name.Contains("ContentPlugin")) itemsToRemove.Add(tsi);
            foreach (ToolStripItem itr in itemsToRemove) toolStrip.Items.Remove(itr);

            if (PluginManager.ContentManagers.Count > 0)
            {
                pluginsMenuItem.Visible = true;
                int i = 0;
                foreach (ContentManager cm in PluginManager.ContentManagers.Values)
                {
                    string id = "ContentPlugin" + (++i).ToString();
                    ToolStripMenuItem item = new ToolStripMenuItem(cm.Name, cm.MenuImage, onClickEvent, "toolStripItem" + id);
                    item.ImageTransparentColor = Color.Magenta;
                    pluginsMenuItem.DropDownItems.Add(item);

                    if (cm.MenuImage != null)
                    {
                        ToolStripButton b = new ToolStripButton(cm.Name, cm.MenuImage, onClickEvent, "toolStripButton" + id);
                        b.ImageTransparentColor = Color.Magenta;
                        toolStrip.Items.Add(b);
                    }
                }
            }
            else
            {
                pluginsMenuItem.Visible = false;
            }
        }

        public static IMyGenContent CreateContent(IMyGenerationMDI mdi, string key)
        {
            IMyGenContent mygencontent = null;
            if (PluginManager.ContentManagers.ContainsKey(key)) 
            {
                mygencontent = PluginManager.ContentManagers[key].Create(mdi);
            }
            return mygencontent;
        }
        #endregion

        public abstract string Name { get; }
        public abstract IMyGenContent Create(IMyGenerationMDI mdi, params string[] args);

        public virtual Image MenuImage
        {
            get { return null; }
        }
    }
}
