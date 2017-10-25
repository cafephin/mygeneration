using System.Runtime.InteropServices;

namespace MyMeta
{
    /// <summary>
    /// The current list of support dbDrivers. Typically VBScript and JScript use the string version as defined by MyMeta.DriverString.
    /// </summary>
#if ENTERPRISE
    [Guid("0e781bc8-b741-4d71-bd0f-8ef7303ab826")]
#endif
    public enum dbDriver
    {
        /// <summary>
        /// String form is "SQL" for DriverString property
        /// </summary>
        SQL,

        /// <summary>
        /// String form is "ORACLE" for DriverString property
        /// </summary>
        Oracle,

        /// <summary>
        /// String form is "ACCESS" for DriverString property
        /// </summary>
        Access,

        /// <summary>
        /// String form is "MYSQL" for DriverString property
        /// </summary>
        MySql,

        /// <summary>
        /// String form is "MYSQL" for DriverString property
        /// </summary>
        MySql2,

        /// <summary>
        /// String form is "DB2" for DriverString property
        /// </summary>
        DB2,

        /// <summary>
        /// String form is "ISeries" for DriverString property
        /// </summary>
        ISeries,

        /// <summary>
        /// String form is "PERVASIVE" for DriverString property
        /// </summary>
        Pervasive,

        /// <summary>
        /// String form is "POSTGRESQL" for DriverString property
        /// </summary>
        PostgreSQL,

        /// <summary>
        /// String form is "POSTGRESQL8" for DriverString property
        /// </summary>
        PostgreSQL8,

        /// <summary>
        /// String form is "FIREBIRD" for DriverString property
        /// </summary>
        Firebird,

        /// <summary>
        /// String form is "INTERBASE" for DriverString property
        /// </summary>
        Interbase,

        /// <summary>
        /// String form is "SQLITE" for DriverString property
        /// </summary>
        SQLite,

#if !IGNORE_VISTA
        /// <summary>
        /// String form is "VISTADB" for DriverString property
        /// </summary>
        VistaDB,
#endif

        /// <summary>
        /// String form is "ADVANTAGE" for DriverString property
        /// </summary>
        Advantage,

        /// <summary>
        /// This is a placeholder for plugin providers
        /// </summary>
        Plugin,

        /// <summary>
        /// Use this if you want know connection at all
        /// </summary>
        None
    }
}