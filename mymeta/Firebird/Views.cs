using System;
using System.Data;

using FirebirdSql.Data.Firebird;

namespace MyMeta.Firebird
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IViews))]
#endif 
	public class FirebirdViews : Views
	{
		public FirebirdViews()
		{

		}

		override internal void LoadAll()
		{
			try
			{
				FbConnection cn = new FirebirdSql.Data.Firebird.FbConnection(this._dbRoot.ConnectionString);
				cn.Open();
				DataTable metaData = cn.GetSchema("Views", new string[] {null, null, null});
				cn.Close();

				metaData.Columns["VIEW_NAME"].ColumnName = "TABLE_NAME";

				PopulateArray(metaData);
			}
			catch(Exception ex)
			{
				string m = ex.Message;
			}
		}
	}
}
