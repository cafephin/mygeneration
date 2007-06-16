using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace MyMeta.Sql
{
#if ENTERPRISE
	using System.Runtime.InteropServices;
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual), ComDefaultInterface(typeof(IResultColumns))]
#endif 
	public class SqlResultColumns : ResultColumns
	{
		public SqlResultColumns()
		{

		}

		override internal void LoadAll()
		{
			try
			{
				string schema = "";

				if(-1 == this.Procedure.Schema.IndexOf("."))
				{
					schema = this.Procedure.Schema + ".";
				}

				string select = "SET FMTONLY ON EXEC [" + this.Procedure.Database.Name + "]." + schema + "[" +
					this.Procedure.Name + "] ";

				int paramCount = this.Procedure.Parameters.Count;

				if(paramCount > 0)
				{
					IParameters parameters = this.Procedure.Parameters;
					IParameter param = null;

					int c = parameters.Count;

					for(int i = 0; i < c; i++)
					{
						param = parameters[i];

						if(param.Direction == ParamDirection.ReturnValue)
						{
							paramCount--;
						}
					}
				}

				for(int i = 0; i < paramCount; i++)
				{
					if(i > 0) 
					{
						select += ",";
					}

					select += "null";
				}

				DataTable metaData = new DataTable();

				try
				{

					Hashtable conn = dbRoot.ParsedConnectionString;

					string cn = "";
					foreach(string key in conn.Keys)
					{
						switch(key.ToLower())
						{
							case "provider":
								break;
							case "extended properties":
								break;
							default:
								cn += key + "=" + conn[key] as string + ";"; 
								break;
						}
					}

					SqlDataAdapter adapter = new SqlDataAdapter(select, cn);

					metaData = new DataTable();
					adapter.Fill(metaData);
				}
				catch
				{
					// Try it the old way ...
					OleDbDataAdapter adapter = new OleDbDataAdapter(select, this.dbRoot.ConnectionString);

					metaData = new DataTable();
					adapter.Fill(metaData);
				}

				SqlResultColumn resultColumn = null;

				int count = metaData.Columns.Count;
				for(int i = 0; i < count; i++)
				{
					resultColumn = this.dbRoot.ClassFactory.CreateResultColumn() as Sql.SqlResultColumn;
					resultColumn.dbRoot = this.dbRoot;
					resultColumn.ResultColumns = this;
					resultColumn._column = metaData.Columns[i];
					this._array.Add(resultColumn);
				}
			}
			catch {}
		}
	}
}
