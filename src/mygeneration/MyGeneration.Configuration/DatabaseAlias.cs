using System;
using System.Xml.Serialization;

namespace MyGeneration.Configuration
{
    [Serializable]
    public class DatabaseAlias
    {
        [XmlAttribute]
        public string DatabaseName { get; set; }

        [XmlAttribute]
        public string Alias { get; set; }
    }
}
