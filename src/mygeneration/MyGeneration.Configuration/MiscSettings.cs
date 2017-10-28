using System;
using System.Xml.Serialization;

namespace MyGeneration.Configuration
{
    [Serializable]
    public class MiscSettings
    {
        [XmlElement]
        public bool ConsoleWriteGeneratedDetails { get; set; }

        [XmlElement]
        public bool EnableDocumentStyleSettings { get; set; }

        [XmlElement]
        public bool CheckForNewBuild { get; set; }

        private bool _domainOverride;

        [XmlElement]
        public bool DomainOverride
        {
            get
            {
                // This is true by default (why?)
                return true;
            }
            set { _domainOverride = value; }
        }

        [XmlElement] private string _windowState;
        public string WindowState { get; set; }

        [XmlElement]
        public int WindowPositionTop { get; set; }

        [XmlElement]
        public int WindowPositionLeft { get; set; }

        [XmlElement]
        public int WindowWidth { get; set; }

        [XmlElement]
        public int WindowPositionHeight { get; set; }
    }
}
