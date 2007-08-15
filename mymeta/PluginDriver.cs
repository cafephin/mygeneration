using System;
using System.Collections.Generic;
using System.Text;

namespace MyMeta
{
    internal class PluginDriver : InternalDriver
    {
        internal PluginDriver(IMyMetaPlugin plugin)
            : base(plugin.GetType(), plugin.SampleConnectionString, false)
        {
        }

    }
}
