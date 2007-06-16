using System;
using System.Data;
using System.Data.OleDb;

namespace MyMeta.Sql
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IParameters))]
#endif 
	public class SqlParameters : Parameters
	{
		public SqlParameters()
		{

		}

		override internal void LoadAll()
		{
			try
			{
				DataTable metaData = this.LoadData(OleDbSchemaGuid.Procedure_Parameters, 
					new object[]{this.Procedure.Database.Name, this.Procedure.Schema, this.Procedure.Name});

				PopulateArray(metaData);
			}
			catch {}
		}
	}
}
