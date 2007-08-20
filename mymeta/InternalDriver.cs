using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;

using ADODB;
using MSDASC;

namespace MyMeta
{
    public class InternalDriver
    {
        internal InternalDriver(Type factory, string connString, bool isOleDB)
        {
            this.factory = factory;
            this.IsOleDB = isOleDB;
            this.ConnectString = connString;
        }


        #region driver properties
        private bool isOleDB;

        private string connectString; // last connect string

        private string driverId;

        private Type factory;

        private bool stripTrailingNulls = false;

        private bool requiredDatabaseName = false;

        public bool RequiredDatabaseName
        {
            get { return requiredDatabaseName; }
            set { requiredDatabaseName = value; }
        }
	

        public bool StripTrailingNulls
        {
            get { return stripTrailingNulls; }
            set { stripTrailingNulls = value; }
        }
	

        public IClassFactory CreateBuildInClass()
        {
            if (factory.IsSubclassOf(typeof(IClassFactory)))
                return factory.Assembly.CreateInstance(factory.Name) as IClassFactory;
            return null;
        }

        public IMyMetaPlugin CreateMyMetaPluginClass()
        {
            if (factory.IsSubclassOf(typeof(IMyMetaPlugin)))
                return factory.Assembly.CreateInstance(factory.Name) as IMyMetaPlugin;
            return null;
        }

        public string DriverId
        {
            get { return driverId; }
            private set { driverId = value; }
        }

        public virtual string ConnectString
        {
            get { return connectString; }
            set { connectString = value; }
        }
	
        public bool IsOleDB
        {
            get { return isOleDB; }
            protected set { isOleDB = value; }
        }

        public virtual string GetDataBaseName(IDbConnection con)
        {
            return con.Database;
        }
        #endregion

        public virtual string BrowseConnectionString(string connstr)
        {
            if (this.IsOleDB)
                return BrowseOleDbConnectionString(connstr);
            return null;
        }

        protected string BrowseOleDbConnectionString(string connstr)
        {
            DataLinksClass dl = new MSDASC.DataLinksClass();

            ADODB.Connection conn = new ADODB.ConnectionClass();
            conn.ConnectionString = connstr;

            object objCn = (object)conn;

            //	dl.PromptNew();
            if (dl.PromptEdit(ref objCn))
            {
                return conn.ConnectionString;
            }
            return null;
        }

        #region driver mapping
        public static InternalDriver Register(string driverId, InternalDriver driver)
        {
            internalDrivers[driverId] = driver;
            driver.driverId = driverId;
            return driver;
        }

        private static Dictionary<string, InternalDriver> internalDrivers = new Dictionary<string, InternalDriver>();

        public static InternalDriver Get(string driverId)
        {
            InternalDriver result;
            if (internalDrivers.TryGetValue(driverId,out result))
                return result;
            return null;
        }
        #endregion
    }
}
