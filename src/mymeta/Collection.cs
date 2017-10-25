using System;
using System.Collections;

namespace MyMeta
{
#if ENTERPRISE
    using System.Runtime.InteropServices;
	[ComVisible(false), ClassInterface(ClassInterfaceType.AutoDual)]
#endif 
	public class Collection : MetaObject
	{
		public virtual int Count
		{
			get
			{
				return _array.Count;
			}
		}

		#region ICollection Members

		public bool IsSynchronized
		{
			get	{ return false;	}
		}

		public void CopyTo(Array array, int index)
		{

		}

		public object SyncRoot
		{
			get	{ return null; }
		}

		#endregion

		#region IList Members

		public bool IsReadOnly
		{
			get	{return true; }
		}

//		object System.Collections.IList.this[int index]
//		{
//			get	{ return this[index];}
//			set	{ }
//		}

		public void RemoveAt(int index)
		{
			
		}

		public void Insert(int index, object value)
		{
		
		}

		public void Remove(object value)
		{
			
		}

		public bool Contains(object value)
		{
			return false;
		}

		public void Clear()
		{
			
		}

		public int IndexOf(object value)
		{
			return 0;
		}

		public int Add(object value)
		{
		
			return 0;
		}

		public bool IsFixedSize
		{
			get	{ return true; 	}
		}

		#endregion

#if ENTERPRISE
	[ComVisible(false)]
#endif 
		public bool CompareStrings(string s1, string s2)
		{
			return 0 == string.Compare(s1, s2, _dbRoot.IgnoreCase);
		}

		protected ArrayList _array = new ArrayList();
		protected bool _fieldsBound = false;
	}
}
