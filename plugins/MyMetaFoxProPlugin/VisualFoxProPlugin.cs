using MyMeta;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace MyMeta.Plugins
{
	public class VisualFoxProPlugin : IMyMetaPlugin
	{
		private const string PROVIDER_KEY = @"VISUALFOXPRO";
		private const string PROVIDER_NAME = @"Visual FoxPro";
		private const string AUTHOR_INFO = @"Visual Fox Pro MyMeta plugin written by Timothy Moriarty.";
		private const string AUTHOR_URI = @"http://www.mygenerationsoftware.com/";
		private const string SAMPLE_CONNECTION = @"Provider=vfpoledb.1;data source=C:\Program Files\Microsoft Visual FoxPro OLE DB Provider\Samples\Northwind\northwind.dbc;";
		private IMyMetaPluginContext context = null;
        private Dictionary<int, string> typeLookup = new Dictionary<int, string>()
        {
            {3, "Integer"}
            ,{5, "Double"}
            ,{6, "Currency"}
            ,{7, "Date"}
            ,{11, "Logical"}
            ,{128, "Varbinary"}
            ,{129, "Varchar"}
            ,{131, "Float"}
            ,{133, "Date"}
            ,{135, "Date Time"}
        };

		private bool IsIntialized { get { return (context != null); } }

		public void Initialize(IMyMetaPluginContext context)
		{
			this.context = context;
		}

		public string ProviderName
		{
			get { return PROVIDER_NAME; }
		}

		public string ProviderUniqueKey
		{
			get { return PROVIDER_KEY; }
		}

		public string ProviderAuthorInfo
		{
			get { return AUTHOR_INFO; }
		}

		public Uri ProviderAuthorUri
		{
			get { return new Uri(AUTHOR_URI); }
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
			get { return SAMPLE_CONNECTION; }
		}

		public IDbConnection NewConnection
		{
			get
			{
				if (IsIntialized)
					return new OleDbConnection(context.ConnectionString);
				else
					return null;
			}
		}

		public string DefaultDatabase
		{
			get
			{
				OleDbConnection c = this.NewConnection as OleDbConnection;
				c.Open();
				string defaultDB = c.DataSource;
				c.Close();

				return defaultDB;
			}
		}

		public DataTable Databases
		{
			get
			{
				OleDbConnection c = this.NewConnection as OleDbConnection;
				c.Open();
				DataTable dt = context.CreateDatabasesDataTable();
				DataRow row = dt.NewRow();
				row["CATALOG_NAME"] = c.DataSource;
				row["DESCRIPTION"] = "The folder where the delimited text files reside.";
				dt.Rows.Add(row);
				c.Close();

				return dt;
			}
		}

		public DataTable GetTables(string database)
		{
            using (OleDbConnection c = this.NewConnection as OleDbConnection)
            {
                c.Open();
                DataTable tableMetaData = c.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                c.Close();
                return tableMetaData;
            }
		}

		public DataTable GetViews(string database)
		{
            using (OleDbConnection c = this.NewConnection as OleDbConnection)
            {
                c.Open();
                DataTable viewMetaData = c.GetOleDbSchemaTable(OleDbSchemaGuid.Views, new object[] { null, null, null });
                c.Close();
                return viewMetaData;
            }
		}

		public DataTable GetProcedures(string database)
		{
            using (OleDbConnection c = this.NewConnection as OleDbConnection)
            {
                c.Open();
                DataTable meta = c.GetOleDbSchemaTable(OleDbSchemaGuid.Procedures, new object[] { null, null, null });;
                c.Close();
                return meta;
            }
		}

		public DataTable GetDomains(string database)
		{
			return new DataTable();
		}

		public DataTable GetProcedureParameters(string database, string procedure)
		{
            DataTable meta = context.CreateParametersDataTable();
			return new DataTable();
		}

		public DataTable GetProcedureResultColumns(string database, string procedure)
		{
			return new DataTable();
		}

		public DataTable GetViewColumns(string database, string view)
		{
            using (OleDbConnection c = this.NewConnection as OleDbConnection)
            {
                c.Open();
                DataTable meta = c.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, view });
                this.SetDataTypes(c, database, view, meta);
                c.Close();
                return meta;
            }
		}

		public DataTable GetTableColumns(string database, string table)
		{
            using (OleDbConnection c = this.NewConnection as OleDbConnection)
            {
                c.Open();
                DataTable meta = c.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, table });
                this.SetDataTypes(c, database, table, meta);
                c.Close();
                return meta;
            }
		}

		public List<string> GetPrimaryKeyColumns(string database, string table)
		{
            using (OleDbConnection c = this.NewConnection as OleDbConnection)
            {
                List<string> primaryKeys = new List<string>();
                c.Open();
                DataTable meta = c.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new object[] { null, null, table });

                foreach (DataRow row in meta.Rows)
                {
                    primaryKeys.Add(row["PK_NAME"].ToString());
                }
                c.Close();
                return primaryKeys;
            }
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
            using (OleDbConnection c = this.NewConnection as OleDbConnection)
            {
                c.Open();
                DataTable meta = c.GetOleDbSchemaTable(OleDbSchemaGuid.Indexes, new object[] { null, null, null, null, table });
                c.Close();
                return meta;
            }
		}

		public DataTable GetForeignKeys(string database, string table)
		{
            using (OleDbConnection c = this.NewConnection as OleDbConnection)
            {
                c.Open();
                DataTable meta = c.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new object[] { null, null, table });
                c.Close();
                return meta;
            }
		}

		public object GetDatabaseSpecificMetaData(object myMetaObject, string key)
		{
			return null;
		}

		private void SetDataTypes(OleDbConnection conn, string database, string entityName, DataTable metaData)
		{
			if (!metaData.Columns.Contains("TYPE_NAME"))
			{
				metaData.Columns.Add("TYPE_NAME");
			}
			if (!metaData.Columns.Contains("TYPE_NAME_COMPLETE"))
			{
				metaData.Columns.Add("TYPE_NAME_COMPLETE");
			}


            foreach (DataRow row in metaData.Rows)
            {
                if (row["DATA_TYPE"] != DBNull.Value)
                {
                    int dataType = (int)row["DATA_TYPE"];
                    string typeName = typeLookup[dataType];
                    row["TYPE_NAME"] = typeName;
                    this.SetDataTypeComplete(conn, row, entityName, row["COLUMN_NAME"].ToString(), typeName);
                }
            }
		}

		private void SetDataTypeComplete(OleDbConnection conn, DataRow row, string entityName, string fieldName, string typeName)
		{
			if (typeName == "Varchar" || typeName == "Varbinary")
			{
				OleDbCommand command = new OleDbCommand();
				command.Connection = conn;
				command.CommandText = string.Format("SELECT FSIZE('{0}') FROM {1}", fieldName, entityName);
				command.CommandType = CommandType.Text;

				object o = command.ExecuteScalar();
				int fsize;

				if (o != null && int.TryParse(o.ToString(), out fsize))
				{
					row["TYPE_NAME_COMPLETE"] = string.Format("{0}({1})", typeName, fsize);
					System.Diagnostics.Debug.WriteLine(string.Format("{0}({1})", typeName, fsize));
				}
			}
			else
			{
				row["TYPE_NAME_COMPLETE"] = typeName;
			}
		}
	}
}