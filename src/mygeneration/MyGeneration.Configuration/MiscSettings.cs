using System;
using System.Xml.Serialization;

namespace MyGeneration.Configuration
{
    [Serializable]
    public class MiscSettings
    {
        [XmlElement]
        public bool ConsoleWriteGeneratedDetails
        {
            get;
            set;
        }
	    
        [XmlElement]
        public bool EnableDocumentStyleSettings
        {
            get;
            set;
        }

        [XmlElement]
        public bool CheckForNewBuild
        {
            get;
            set;
        }

        private bool _domainOverride;
        [XmlElement]
        public bool DomainOverride
        {
            get 
            { 
                // This is true by default (why?)
                return true; 
            }
            set	{ _domainOverride = value; }
        }

        [XmlElement]
        private string _windowState;
        public string WindowState
        {
            get { return "Normal"; } //TODO: return some default value?
            set	{ _windowState = value; }
        }

        private int _windowPositionTop;
        [XmlElement]
        public int WindowPositionTop
        {
            get { return 50; } //TODO: return some default value?
            set	{ _windowPositionTop = value; }
        }

        private int _windowPositionLeft;
        [XmlElement]
        public int WindowPositionLeft
        {
            get { return 50; } //TODO: return some default value?
            set	{ _windowPositionLeft = value; }
        }

        private int _windowWidth;
        [XmlElement]
        public int WindowWidth
        {
            get { return 1024; } //TODO: return some default value?
            set	{ _windowWidth = value; }
        }

        private int _windowHeight;
        [XmlElement]
        public int WindowPositionHeight
        {
            get { return 600; } //TODO: return some default value?
            set	{ _windowHeight = value; }
        }
    }
}
