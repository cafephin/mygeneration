using System;
using System.Data;
using Dl3bak.Data.Xsd3b;

namespace MyMeta.Plugins.Xsd3b
{
    /// <summary>
    /// Dummy DB-Connection for MyMeta.Plugins.Xsd3b, required by MyGen api
    /// 
    /// (c) 2007 by dl3bak@qsl.net
    /// 
    /// MyGenXsd3b ("MyMeta.Plugins.Xsd3b.dll") is a provider plugin 
    ///     for MyGeneration that allows to use XSDFile s (*.xsd, *.xsd3b) 
    ///     as datasource instead of an online sqldatabase.
    /// 
    /// MyGeneration from http://www.mygenerationsoftware.com is a template 
    ///     driven sourcecodegenerator that you can use free of charge. 
    /// It uses the RelationalDatastructure of a sql database 
    ///         (ie Microsoft SQL, Oracle, IBM DB2, MySQL, PostgreSQL, 
    ///         Microsoft Access, FireBird, Interbase, SQLite, ...) 
    ///         and produces sourcecode (ie C#, VB.NET code,
    ///         SQL Stored Procedures, PHP, HTML, ...) 
    /// 
    /// There is an online repository with lots of templates available. 
    /// </summary>
	public class Xsd3bConnection : IDbConnection
	{
        private String connectionString = "";
		public Xsd3bConnection() 
		{
		}

        #region IDbConnection Members

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return null;
        }

        public IDbTransaction BeginTransaction()
        {
            return null;
        }

        public void ChangeDatabase(string databaseName)
        {
        }

        private SchemaXsd3bEx xsd3b = null;
        public SchemaXsd3bEx Xsd3b
        {
            get
            {
                if (xsd3b == null)
                    xsd3b = SchemaXsd3bEx.ReadXsd3b(this.ConnectionString, null);
                //xsd3b = SchemaXsd3bEx.ReadXsd3b(@"D:\Eigene Dateien\SharpDevelop Projects\Xsd3bAll\nwind3b.xsd3b", null); // this.context.ConnectionString,null);
                return xsd3b;
            }
        }


        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                xsd3b = null;
                connectionString = value;
            }
        }

        public int ConnectionTimeout
        {
            get { return 0; }
        }

        public IDbCommand CreateCommand()
        {
            return null;
        }

        public string Database
        {
            get { return null; }
        }

        public void Open()
        {

            try
            {
                xsd3b = SchemaXsd3bEx.ReadXsd3b(this.ConnectionString, null);
                state = ConnectionState.Open;
            }
            catch (Exception)
            {
                state = ConnectionState.Broken;
                throw;
            }
        }

        public void Close()
        {
            state = ConnectionState.Closed;
        }

        ConnectionState state = ConnectionState.Closed;
        public ConnectionState State
        {
            get { return state; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
