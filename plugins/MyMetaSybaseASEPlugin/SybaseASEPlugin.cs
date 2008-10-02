using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using MyMeta;

namespace MyMeta.Plugins
{
    public class SybaseASEPlugin : IMyMetaPlugin
	{

		private IMyMetaPluginContext context;

        public void Initialize(IMyMetaPluginContext context)
        {
            this.context = context;
        }

        public string ProviderName
        {
            get { return @"Sybase ASE"; }
        }

        public string ProviderUniqueKey
        {
            get { return @"SYBASEASE"; }
        }

        public string ProviderAuthorInfo
        {
            get { return @"Sybase ASE MyMeta Plugin Written by komma8komma1"; }
        }

        public Uri ProviderAuthorUri
        {
            get { return new Uri(@"http://www.mygenerationsoftware.com/"); }
        }

        public bool StripTrailingNulls
        {
            get { return false; }
        }

        public bool RequiredDatabaseName
        {
            get { return false; }
        }

        public string SampleConnectionString
        {
            get { return @"Provider=ASEOLEDB;Data Source=GREENMACHINE:5000;Catalog=pubs2;User Id=sa;Password=;"; }
        }

        public IDbConnection NewConnection
        {
            get
            {
                if (IsIntialized)
				{
                    OleDbConnection cn = new OleDbConnection(this.context.ConnectionString);
					return cn as IDbConnection;
				}
                else
                    return null;
            }
        }

        public string DefaultDatabase
        {
            get
            {
				return this.GetDatabaseName();
            }
        }

        public DataTable Databases
        {
            get
            {
                using (OleDbConnection cn = this.NewConnection as OleDbConnection)
                {
                    cn.Open();
                    return cn.GetOleDbSchemaTable(OleDbSchemaGuid.Catalogs, new Object[] { null });
                }

                return this.context.CreateDatabasesDataTable();
            }
        }

        public DataTable GetTables(string database)
        {
            using (OleDbConnection cn = this.NewConnection as OleDbConnection)
            {
                cn.Open();
                string type = context.IncludeSystemEntities ? null : "TABLE";
                return cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { database, null, null, type });
            }//this.Database.Name, null, null, type

            //return this.context.CreateTablesDataTable();

        }

        public DataTable GetViews(string database)
		{
			//DataTable metaData = new DataTable();
			//IVistaDBDatabase db = null;

			/*try
			{
				metaData = context.CreateViewsDataTable();

				using (VistaDBConnection conn = new VistaDBConnection())
				{
					conn.ConnectionString = context.ConnectionString;
					conn.Open();

					using (VistaDBCommand cmd = new VistaDBCommand("SELECT * FROM GetViews()", conn))
					{
						using (VistaDBDataAdapter da = new VistaDBDataAdapter(cmd))
						{
							DataTable views = new DataTable();
							da.Fill(views);

							foreach(DataRow vistaRow in views.Rows)
							{
								DataRow row = metaData.NewRow();
								metaData.Rows.Add(row);

								row["TABLE_NAME"]   = vistaRow["VIEW_NAME"];
								row["DESCRIPTION"]  = vistaRow["DESCRIPTION"];
								row["VIEW_TEXT"]    = vistaRow["VIEW_DEFINITION"];
								row["IS_UPDATABLE"] = vistaRow["IS_UPDATABLE"];
							}
						}						 
					}
				}
			}
			catch{}*/

            //return metaData;
            return this.context.CreateViewsDataTable();
		}

        public DataTable GetProcedures(string database)
        {
            return this.context.CreateProceduresDataTable();
        }

        public DataTable GetDomains(string database)
        {
            return this.context.CreateDomainsDataTable();
        }

        public DataTable GetProcedureParameters(string database, string procedure)
        {
            return new DataTable();
        }

        public DataTable GetProcedureResultColumns(string database, string procedure)
        {
            return this.context.CreateResultColumnsDataTable();
        }

        public DataTable GetViewColumns(string database, string view)
        {
            return this.context.CreateColumnsDataTable();
			//DataTable metaData = new DataTable();
			//IVistaDBDatabase db = null;

			/*try
			{
				metaData = context.CreateColumnsDataTable();

				using (VistaDBConnection conn = new VistaDBConnection())
				{
					conn.ConnectionString = context.ConnectionString;
					conn.Open();

					string sql = "SELECT * FROM GetViewColumns('" + view + "')";

					using (VistaDBCommand cmd = new VistaDBCommand(sql, conn))
					{
						using (VistaDBDataAdapter da = new VistaDBDataAdapter(cmd))
						{
							DataTable views = new DataTable();
							da.Fill(views);

							foreach(DataRow vistaRow in views.Rows)
							{
								DataRow row = metaData.NewRow();
								metaData.Rows.Add(row);

								int width		= Convert.ToInt32(vistaRow["COLUMN_SIZE"]);
								int dec			= 0; 
								int length      = 0;
								int octLength   = width;
								bool timestamp  = false;

								string type = vistaRow["DATA_TYPE_NAME"] as string;

								switch(type)
								{
									case "Char":
									case "NChar":
									case "NText":
									case "NVarchar":
									case "Text":
									case "Varchar":
										length = width;
										width  = 0;
										dec    = 0;
										break;

									case "Currency":
									case "Double":
									case "Decimal":
									case "Single":
										break;

									case "Timestamp":
										timestamp = true;
										break;

									default:
										width = 0;
										dec   = 0;
										break;
								}

								string def = Convert.ToString(vistaRow["DEFAULT_VALUE"]);

								row["TABLE_NAME"] = view;
								row["COLUMN_NAME"] = vistaRow["COLUMN_NAME"];
								row["ORDINAL_POSITION"] = vistaRow["COLUMN_ORDINAL"];
								row["IS_NULLABLE"] = vistaRow["ALLOW_NULL"];
								row["COLUMN_HASDEFAULT"] = def == string.Empty ? false : true;
								row["COLUMN_DEFAULT"] = def;
								row["IS_AUTO_KEY"] = vistaRow["IDENTITY_VALUE"];
								row["AUTO_KEY_SEED"] = vistaRow["IDENTITY_SEED"];
								row["AUTO_KEY_INCREMENT"] = vistaRow["IDENTITY_STEP"];
								row["TYPE_NAME"] = type;
								row["NUMERIC_PRECISION"] = width;
								row["NUMERIC_SCALE"] = dec;
								row["CHARACTER_MAXIMUM_LENGTH"] = length;
								row["CHARACTER_OCTET_LENGTH"] = octLength;
								row["DESCRIPTION"] = vistaRow["COLUMN_DESCRIPTION"];

								if (timestamp)
								{
									row["IS_COMPUTED"] = true;
								}
							}
						}						 
					}
				}
			}
			catch{}*/

			//return metaData;
        }

        public DataTable GetTableColumns(string database, string table)
        {
            return this.context.CreateColumnsDataTable();
			//DataTable metaData = new DataTable();
			/*IVistaDBDatabase db = null;

			try
			{
				metaData = context.CreateColumnsDataTable();

				db = DDA.OpenDatabase(this.GetFullDatabaseName(), 
					VistaDBDatabaseOpenMode.NonexclusiveReadOnly, "");
				ArrayList tables = db.EnumTables();

				IVistaDBTableSchema tblStructure = db.TableSchema(table);

				foreach (IVistaDBColumnAttributes c in tblStructure) 
				{ 
					string colName = c.Name;

					string def = "";
					if(tblStructure.Defaults.Contains(colName))
					{
						def = tblStructure.Defaults[colName].Expression;
					}
					int width		= c.MaxLength; //c.ColumnWidth;
					int dec			= 0; //c.ColumnDecimals;
					int length      = 0;
					int octLength   = width;

					IVistaDBIdentityInformation identity = null;
					if(tblStructure.Identities.Contains(colName))
					{
						identity = tblStructure.Identities[colName];
					}

					string[] pks = null;
					if(tblStructure.Indexes.Contains("PrimaryKey"))
					{
						pks = tblStructure.Indexes["PrimaryKey"].KeyExpression.Split(';');
					}
					else
					{
						foreach(IVistaDBIndexInformation pk in tblStructure.Indexes)
						{
							if(pk.Primary)
							{
								pks = pk.KeyExpression.Split(';');
								break;
							}
						}
					}

					System.Collections.Hashtable pkCols = null;
					if(pks != null)
					{
						pkCols = new Hashtable();
						foreach(string pkColName in pks)
						{
							pkCols[pkColName] = true;
						}
					}

					switch(c.Type)
					{
						case VistaDBType.Char:
						case VistaDBType.NChar:
						case VistaDBType.NText:
						case VistaDBType.NVarChar:
						case VistaDBType.Text:
						case VistaDBType.VarChar:
							length    = width;
							width     = 0;
							dec       = 0;
							break;

						case VistaDBType.Money:
						case VistaDBType.Float:
						case VistaDBType.Decimal:
						case VistaDBType.Real:
							break;

						default:
							width = 0;
							dec   = 0;
							break;
					}

					DataRow row = metaData.NewRow();
					metaData.Rows.Add(row);

					row["TABLE_NAME"] = tblStructure.Name;
					row["COLUMN_NAME"] = c.Name;
					row["ORDINAL_POSITION"] = c.RowIndex;
					row["IS_NULLABLE"] = c.AllowNull;
					row["COLUMN_HASDEFAULT"] = def == string.Empty ? false : true;
					row["COLUMN_DEFAULT"] = def;
					row["IS_AUTO_KEY"] = identity == null ? false : true;
					row["AUTO_KEY_SEED"] = 1;
					row["AUTO_KEY_INCREMENT"] = identity == null ? 0 : Convert.ToInt32(identity.StepExpression);
					row["TYPE_NAME"] = c.Type.ToString();
					row["NUMERIC_PRECISION"] = width;
					row["NUMERIC_SCALE"] = dec;
					row["CHARACTER_MAXIMUM_LENGTH"] = length;
					row["CHARACTER_OCTET_LENGTH"] = octLength;
					row["DESCRIPTION"] = c.Description;

					if (c.Type == VistaDBType.Timestamp)
					{
						row["IS_COMPUTED"] = true;
					}
				} 

			}
			finally
			{
				if(db != null) db.Close();
			}*/

			//return metaData;
        }

        public List<string> GetPrimaryKeyColumns(string database, string table)
        {
            return new List<string>();
			//List<string> primaryKeys = new List<string>();
			/*IVistaDBDatabase db = null;

			try
			{
				db = DDA.OpenDatabase(this.GetFullDatabaseName(), 
					VistaDBDatabaseOpenMode.NonexclusiveReadOnly, "");

				IVistaDBTableSchema tblStructure = db.TableSchema(table);

				string[] pks = null;
				if(tblStructure.Indexes.Contains("PrimaryKey"))
				{
					pks = tblStructure.Indexes["PrimaryKey"].KeyExpression.Split(';');
				}
				else
				{
					foreach(IVistaDBIndexInformation pk in tblStructure.Indexes)
					{
						if(pk.Primary)
						{
							pks = pk.KeyExpression.Split(';');
							break;
						}
					}
				}

				if(pks != null)
				{
					foreach(string pkColName in pks)
					{
						primaryKeys.Add(pkColName);
					}
				}
			}
			finally
			{
				if(db != null) db.Close();
			}*/

			//return primaryKeys;
        }

        public List<string> GetViewSubViews(string database, string view)
        {
            return new List<string>();
        }

        public List<string> GetViewSubTables(string database, string view)
        {
            return new List<string>();
        }

        public DataTable GetTableIndexes(string database, string table)
        {
            return this.context.CreateIndexesDataTable();
			//DataTable metaData = new DataTable();
			/*IVistaDBDatabase db = null;

			try
			{
				metaData = context.CreateIndexesDataTable();

				db = DDA.OpenDatabase(this.GetFullDatabaseName(), 
					VistaDBDatabaseOpenMode.NonexclusiveReadOnly, "");

				ArrayList tables = db.EnumTables();

				IVistaDBTableSchema tblStructure = db.TableSchema(table);

				foreach (IVistaDBIndexInformation indexInfo in tblStructure.Indexes) 
				{ 
					string[] pks = indexInfo.KeyExpression.Split(';');

					int index = 0;
					foreach(string colName in pks)
					{
						DataRow row = metaData.NewRow();
						metaData.Rows.Add(row);

						row["TABLE_CATALOG"] = GetDatabaseName();
						row["TABLE_NAME"] = tblStructure.Name;
						row["INDEX_CATALOG"] = GetDatabaseName();
						row["INDEX_NAME"] = indexInfo.Name;
						row["UNIQUE"] = indexInfo.Unique;
						row["COLLATION"] = indexInfo.KeyStructure[index++].Descending ? 2 : 1;
						row["COLUMN_NAME"] = colName;
					}
				} 
			}
			finally
			{
				if(db != null) db.Close();
			}*/

			//return metaData;
        }

        public DataTable GetForeignKeys(string database, string tableName)
        {
            return this.context.CreateForeignKeysDataTable();
            //DataTable metaData = this.context.CreateForeignKeysDataTable();
			/*IVistaDBDatabase db = null;

			try
			{
				metaData = context.CreateForeignKeysDataTable();

				db = DDA.OpenDatabase(this.GetFullDatabaseName(), 
					VistaDBDatabaseOpenMode.NonexclusiveReadOnly, "");

				ArrayList tables = db.EnumTables(); 

				foreach (string table in tables) 
				{
					IVistaDBTableSchema tblStructure = db.TableSchema(table);

					foreach (IVistaDBRelationshipInformation relInfo in tblStructure.ForeignKeys) 
					{ 
						if(relInfo.ForeignTable != tableName && relInfo.PrimaryTable != tableName)
							continue;

						string fCols = relInfo.ForeignKey; 
						string pCols = String.Empty; 

						string primaryTbl  = relInfo.PrimaryTable; 
						string pkName = "";

						using (IVistaDBTableSchema pkTableStruct = db.TableSchema(primaryTbl)) 
						{ 
							foreach (IVistaDBIndexInformation idxInfo in pkTableStruct.Indexes) 
							{ 
								if (!idxInfo.Primary) 
								continue; 
								        
								pkName = idxInfo.Name;
								pCols = idxInfo.KeyExpression; 
								break; 
							} 
						} 

						string [] fColumns = fCols.Split(';'); 
						string [] pColumns = pCols.Split(';'); 

						for(int i = 0; i < fColumns.GetLength(0); i++)
						{
							DataRow row = metaData.NewRow();
							metaData.Rows.Add(row);

							row["PK_TABLE_CATALOG"] = GetDatabaseName();
							row["PK_TABLE_SCHEMA"]  = DBNull.Value;
							row["FK_TABLE_CATALOG"] = DBNull.Value;
							row["FK_TABLE_SCHEMA"]  = DBNull.Value;
							row["FK_TABLE_NAME"]    = tblStructure.Name;
							row["PK_TABLE_NAME"]    = relInfo.PrimaryTable;
							row["ORDINAL"]          = 0;
							row["FK_NAME"]          = relInfo.Name;
							row["PK_NAME"]          = pkName;
							row["PK_COLUMN_NAME"]   = pColumns[i]; 
							row["FK_COLUMN_NAME"]   = fColumns[i];

							row["UPDATE_RULE"]		= relInfo.UpdateIntegrity;
							row["DELETE_RULE"]		= relInfo.DeleteIntegrity;
						}
					} 
				}
			}
			finally
			{
				if(db != null) db.Close();
			}*/

			//return metaData;
        }

        public object GetDatabaseSpecificMetaData(object myMetaObject, string key)
        {
            return null;
        }

        private bool IsIntialized { get { return (context != null); } }

		public string GetDatabaseName()
		{
            using (OleDbConnection cn = this.NewConnection as OleDbConnection)
            {
                cn.Open();
                return cn.Database;
            }
            return string.Empty;
		}
	}
}
