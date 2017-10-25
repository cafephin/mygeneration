namespace MyMeta
{
    public static class MyMetaDrivers
    {
        public const string Access = "ACCESS";
        public const string Advantage = "ADVANTAGE";
        public const string DB2 = "DB2";
        public const string Firebird = "FIREBIRD";
        public const string Interbase = "INTERBASE";
        public const string ISeries = "ISERIES";
        public const string MySql = "MYSQL";
        public const string MySql2 = "MYSQL2";
        public const string None = "NONE";
        public const string Oracle = "ORACLE";
        public const string Pervasive = "PERVASIVE";
        public const string PostgreSQL = "POSTGRESQL";
        public const string PostgreSQL8 = "POSTGRESQL8";
        public const string SQLite = "SQLITE";
        public const string SQL = "SQL";
#if !IGNORE_VISTA
        public const string VistaDB = "VISTADB";
#endif
        public static dbDriver GetDbDriverFromName(string name)
        {
            switch (name)
            {
                case MyMetaDrivers.SQL:
                    return dbDriver.SQL;
                case MyMetaDrivers.Oracle:
                    return dbDriver.Oracle;
                case MyMetaDrivers.Access:
                    return dbDriver.Access;
                case MyMetaDrivers.MySql:
                    return dbDriver.MySql;
                case MyMetaDrivers.MySql2:
                    return dbDriver.MySql2;
                case MyMetaDrivers.DB2:
                    return dbDriver.DB2;
                case MyMetaDrivers.ISeries:
                    return dbDriver.ISeries;
                case MyMetaDrivers.Pervasive:
                    return dbDriver.Pervasive;
                case MyMetaDrivers.PostgreSQL:
                    return dbDriver.PostgreSQL;
                case MyMetaDrivers.PostgreSQL8:
                    return dbDriver.PostgreSQL8;
                case MyMetaDrivers.Firebird:
                    return dbDriver.Firebird;
                case MyMetaDrivers.Interbase:
                    return dbDriver.Interbase;
                case MyMetaDrivers.SQLite:
                    return dbDriver.SQLite;
#if !IGNORE_VISTA
                case MyMetaDrivers.VistaDB:
                    return dbDriver.VistaDB;
#endif
                case MyMetaDrivers.Advantage:
                    return dbDriver.Advantage;
                case MyMetaDrivers.None:
                    return dbDriver.None;
                default:
                    return dbDriver.Plugin;
            }
        }
    }
}