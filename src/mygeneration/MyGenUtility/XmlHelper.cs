using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MyGeneration
{
    public class XmlHelper
    {
        /// <summary>
        /// robust replacement for parentNode.Attributes[name].Value = ...
        /// creates the attribute if it does not existent.
        /// </summary>
        internal static void SetAttribute(XmlNode parentNode, string name, string value)
        {
            if (parentNode != null)
            {
                XmlAttribute attr = GetOrCreateAttribute(parentNode, name);
                attr.Value = value;
            }
        }

        /// <summary>
        /// robust replacement for x = parentNode.Attributes[name].Value
        /// </summary>
        public static string GetAttribute(XmlNode parentNode, string name, string notFoundValue)
        {
            if (parentNode != null)
            {
                XmlAttribute attr = parentNode.Attributes[name];
                if (attr != null)
                    return attr.Value;
            }
            return notFoundValue;
        }

        private static XmlAttribute GetOrCreateAttribute(XmlNode parentNode, string name)
        {
            if (parentNode == null)
                return null;
            XmlAttribute attr = parentNode.Attributes[name];
            if (attr == null)
                attr = parentNode.Attributes.Append(parentNode.OwnerDocument.CreateAttribute(name));
            return attr;
        }
    }
}
