using System;
using System.Collections;

namespace MyMeta
{
#if ENTERPRISE
    using System.Runtime.InteropServices;
    [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IPropertyCollection))]
#endif 
	public class PropertyCollectionAll : Collection, IPropertyCollection, IEnumerator, ICollection
	{
		internal void Load(IPropertyCollection local, IPropertyCollection global)
		{
			this._local  = local;
			this._global = global;
		}

		#region IPropertyCollection
#if ENTERPRISE
		[DispId(0)]
#endif		
		public IProperty this[string key] 
		{ 
			get
			{
				if(this._local.ContainsKey(key))
				{
					return this._local[key];
				}
				else if(this._global.ContainsKey(key))
				{
					return this._global[key];
				}

				return null;
			}
		}

		/// <summary>
		/// This method will either add or update a key value pair.  If the key already exists in the collection the value will be updated.
		/// If this key doesn't exist the key/value pair will be added.  If only want to update, and not add new items, use <see cref="ContainsKey"/> to determine if the key already exists.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public IProperty AddKeyValue(string key, string value)
		{
			throw new NotImplementedException("Cannot call AddKeyValue on this collection");
		}

		/// <summary>
		/// Removes a key/value pair from the collection, no error is thrown if the key doesn't exist.
		/// </summary>
		/// <param name="key">The key of the desired key/value pair</param>
		public void RemoveKey(string key)
		{
			throw new NotImplementedException("Cannot call AddKeyValue on this collection");
		}

		/// <summary>
		/// Use ContainsKey to determine if a key exists in the collection.
		/// </summary>
		/// <param name="key">The key of the desired key/value pair</param>
		/// <returns>True if the key exists, False if not</returns>
		public bool ContainsKey(string key)
		{
			if(this._local.ContainsKey(key))
			{
				return true;
			}
			else if(this._global.ContainsKey(key))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Removes all key/value pairs from the collection.
		/// </summary>
		public new void Clear()
		{
			throw new NotImplementedException("Cannot call AddKeyValue on this collection");
		}
		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			Reset();
			return this;
		}

		#endregion

		#region IEnumerator Members

		public void Reset()
		{
			_useLocalEnum = true;
			_wereDone = false;
			_localEnumerator  = this._local.GetEnumerator();
			_globalEnumerator = this._global.GetEnumerator();
		}

		public object Current
		{
			get
			{
				IProperty prop = null;

				if(_useLocalEnum)
				{
					prop = (IProperty)_localEnumerator.Current;
				}
				else
				{
					prop = (IProperty)_globalEnumerator.Current;
				}

				return prop;
			}
		}

		public bool MoveNext()
		{
			if(this._useLocalEnum)
			{
				if(_localEnumerator.MoveNext()) return true;
			}

			if(!_wereDone)
			{
				this._useLocalEnum = false;

				while(true)
				{
					if(_globalEnumerator.MoveNext())
					{
						IProperty prop = (IProperty)_globalEnumerator.Current;

						if(!this._local.ContainsKey(prop.Key))
						{
							return true;
						}
					}
					else
					{
						break;
					}
				}
				
				_wereDone = true;
			}	
			
			return false;
		}

		#endregion

		#region ICollection Members

		public new  bool IsSynchronized
		{
			get
			{
				// TODO:  Add Databases.IsSynchronized getter implementation
				return false;
			}
		}

		public new void CopyTo(Array array, int index)
		{
			// TODO:  Add Databases.CopyTo implementation
		}

		public new object SyncRoot
		{
			get
			{
				// TODO:  Add Databases.SyncRoot getter implementation
				return null;
			}
		}

		#endregion

		private IPropertyCollection _local;
		private IPropertyCollection _global;

		private bool _useLocalEnum = true;
		private bool _wereDone;
		private IEnumerator _localEnumerator;
		private IEnumerator _globalEnumerator;
	}
}
