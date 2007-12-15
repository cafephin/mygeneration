using System;
using System.Data;

using FirebirdSql.Data.FirebirdClient;

namespace MyMeta.Firebird
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IIndexes))]
#endif 
	public class FirebirdIndexes : Indexes
	{
		public FirebirdIndexes()
		{

		}

		override internal void LoadAll()
		{
			try
			{
				FbConnection cn = new FirebirdSql.Data.FirebirdClient.FbConnection(this._dbRoot.ConnectionString);
				cn.Open();
				DataTable metaData = cn.GetSchema("Indexes", new string[] {null, null, this.Table.Name});
				cn.Close();

				metaData.Columns["IS_UNIQUE"].ColumnName = "UNIQUE";
				metaData.Columns["INDEX_TYPE"].ColumnName = "TYPE";
				metaData.Columns["ORDINAL_POSITION"].ColumnName = "CARDINALITY";
				PopulateArray(metaData);
			}
			catch(Exception ex)
			{
				string m = ex.Message;
			}
		}
	}
}
