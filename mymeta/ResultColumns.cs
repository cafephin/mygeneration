using System;
using System.Xml;
using System.Collections;

namespace MyMeta
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(false), ClassInterface(ClassInterfaceType.AutoDual)]
#endif 
	public class ResultColumns : Collection, IResultColumns, IEnumerable, ICollection
	{
		public ResultColumns()
		{

		}

		#region XML User Data

#if ENTERPRISE
		[ComVisible(false)]
#endif
		override public string UserDataXPath
		{ 
			get
			{
				return Procedure.UserDataXPath + @"/ResultColumns";
			} 
		}

#if ENTERPRISE
		[ComVisible(false)]
#endif
		override internal bool GetXmlNode(out XmlNode node, bool forceCreate)
		{
			node = null;
			bool success = false;

			if(null == _xmlNode)
			{
				// Get the parent node
				XmlNode parentNode = null;
				if(this.Procedure.GetXmlNode(out parentNode, forceCreate))
				{
					// See if our user data already exists
					string xPath = @"./ResultColumns";
					if(!GetUserData(xPath, parentNode, out _xmlNode) && forceCreate)
					{
						// Create it, and try again
						this.CreateUserMetaData(parentNode);
						GetUserData(xPath, parentNode, out _xmlNode);
					}
				}
			}

			if(null != _xmlNode)
			{
				node = _xmlNode;
				success = true;
			}

			return success;
		}

#if ENTERPRISE
		[ComVisible(false)]
#endif
		override public void CreateUserMetaData(XmlNode parentNode)
		{
			XmlNode myNode = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "ResultColumns", null);
			parentNode.AppendChild(myNode);
		}

		#endregion

		virtual internal void LoadAll()
		{

		}

		public IResultColumn this[object index] 
		{ 
			get
			{
				return null;
			}
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new Enumerator(this._array);
		}

		#endregion

		#region IList Members

		object System.Collections.IList.this[int index]
		{
			get	{ return this[index];}
			set	{ }
		}

		#endregion

		internal Procedure Procedure = null;
	}
}
