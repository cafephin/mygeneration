using System;
using System.Data;
using System.Data.OleDb;

namespace MyMeta.Plugin
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IResultColumn))]
#endif 
	public class PluginResultColumn : ResultColumn
    {
        private IMyMetaPlugin plugin;

        public PluginResultColumn(IMyMetaPlugin plugin)
        {
            this.plugin = plugin;
		}
	}
}
