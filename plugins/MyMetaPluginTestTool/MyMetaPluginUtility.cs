using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using MyMeta;

namespace MyMetaPluginTestTool
{
    public partial class MyMetaPluginUtility : Form
    {
        private FileInfo _cfgFile = null;

        public MyMetaPluginUtility()
        {
            InitializeComponent();
        }

        public FileInfo ConfigFile
        {
            get
            {
                if (_cfgFile == null)
                {
                    _cfgFile = new FileInfo(Assembly.GetEntryAssembly().Location + ".cfg");
                }
                return _cfgFile;
            }
        }

        private void MyMetaPluginUtility_Load(object sender, EventArgs e)
        {
            //dbRoot root = new dbRoot();
            foreach (IMyMetaPlugin plugin in dbRoot.Plugins.Values)
            {
                IMyMetaPlugin pluginTest = dbRoot.Plugins[plugin.ProviderUniqueKey] as IMyMetaPlugin;
                if (pluginTest == plugin)
                {
                    this.comboBoxPlugins.Items.Add(plugin.ProviderUniqueKey);
                }
            }
            dbRoot root = new dbRoot();
            foreach (string dbd in Enum.GetNames(typeof(dbDriver)))
            {
                if (!dbd.Equals("Plugin", StringComparison.CurrentCultureIgnoreCase))
                    this.comboBoxPlugins.Items.Add(dbd.ToUpper());
            }

            if (ConfigFile.Exists)
            {
                string[] lines = File.ReadAllLines(ConfigFile.FullName);
                if (lines.Length > 1)
                {
                    int idx = this.comboBoxPlugins.FindStringExact(lines[0]); 
                    if (idx >= 0) this.comboBoxPlugins.SelectedIndex = idx;
                    this.textBoxConnectionString.Text = lines[1];
                }
            }
        }

        private void comboBoxPlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxPlugins.SelectedItem != null)
            {
                IMyMetaPlugin plugin = dbRoot.Plugins[this.comboBoxPlugins.SelectedItem.ToString()] as IMyMetaPlugin;
                if (plugin != null)
                {
                    this.checkBoxPlugin.Enabled = true;
                    this.checkBoxPlugin.Checked = true;
                    this.textBoxConnectionString.Text = plugin.SampleConnectionString;
                }
                else
                {
                    //dbRoot root = new dbRoot();
                    this.checkBoxPlugin.Checked = false;
                    this.checkBoxPlugin.Enabled = false;
                    this.textBoxConnectionString.Text = string.Empty;
                }
            }
        }

        private void MyMetaPluginUtility_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(this.comboBoxPlugins.SelectedItem.ToString());
                sb.AppendLine(this.textBoxConnectionString.Text);

                File.WriteAllText(ConfigFile.FullName, sb.ToString());
            }
            catch {}
        }


        private void buttonTest_Click(object sender, EventArgs e)
        {
            bool doPluginTests = this.checkBoxPlugin.Checked;
            bool doAPITests = this.checkBoxAPI.Checked;
            bool hasErroredOut = false;
            this.textBoxResults.Clear();
            IMyMetaPlugin plugin = null;
            dbRoot root = null;

            if (doPluginTests)
            {
                try
                {
                    plugin = dbRoot.Plugins[this.comboBoxPlugins.SelectedItem.ToString()] as IMyMetaPlugin;

                    IMyMetaPluginContext context = new MyMetaPluginContext(plugin.ProviderUniqueKey, this.textBoxConnectionString.Text);

                    plugin.Initialize(context);
                    using (IDbConnection conn = plugin.NewConnection)
                    {
                        conn.Open();
                        conn.Close();
                    }
                    this.textBoxConnectionString.BackColor = Color.LightGreen;
                    this.AppendLog("Connection Test Successful.");
                }
                catch (Exception ex)
                {
                    hasErroredOut = true;
                    this.textBoxConnectionString.BackColor = Color.Red;
                    this.AppendLog("Error testing connection", ex);
                }
            }

            if (doAPITests)
            {
                if (!hasErroredOut)
                {
                    Application.DoEvents();
                    root = new dbRoot();
                    try
                    {
                        root.Connect(this.comboBoxPlugins.SelectedItem.ToString(), textBoxConnectionString.Text);
                        this.AppendLog("MyMeta dbRoot Connection Successful.");
                    }
                    catch (Exception ex)
                    {
                        hasErroredOut = true;
                        this.AppendLog("Error connecting to dbRoot", ex);
                    }
                }
            }

            TestDatabases(plugin, root, ref hasErroredOut, ref doAPITests, ref doPluginTests);
            TestTables(plugin, root, ref hasErroredOut, ref doAPITests, ref doPluginTests);
            TestViews(plugin, root, ref hasErroredOut, ref doAPITests, ref doPluginTests);
            TestProcedures(plugin, root, ref hasErroredOut, ref doAPITests, ref doPluginTests);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="root"></param>
        /// <param name="hasErroredOut"></param>
        private void TestDatabases(IMyMetaPlugin plugin, dbRoot root, ref bool hasErroredOut, ref bool doAPITests, ref bool doPluginTests)
        {
            string garbage;

            //--------------------------------------------------------
            if (doPluginTests && !hasErroredOut)
            {
                try
                {
                    DataTable dt = plugin.Databases;

                    this.AppendLog(dt.Rows.Count + " databases found through Plugin.");

                }
                catch (Exception ex)
                {
                    hasErroredOut = true;
                    this.AppendLog("Plugin Databases Error", ex);
                }
            }

            //--------------------------------------------------------
            if (doAPITests && !hasErroredOut)
            {
                try
                {
                    foreach (IDatabase db in root.Databases)
                    {
                        garbage = db.Name;
                        garbage = db.SchemaName;
                        garbage = db.SchemaOwner;
                    }

                    this.AppendLog(root.Databases.Count + " databases traversed successfully through MyMeta.");
                }
                catch (Exception ex)
                {
                    hasErroredOut = true;
                    this.AppendLog("Error traversing databases through MyMeta", ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="root"></param>
        /// <param name="hasErroredOut"></param>
        private void TestTables(IMyMetaPlugin plugin, dbRoot root, ref bool hasErroredOut, ref bool doAPITests, ref bool doPluginTests)
        {
            string garbage;
            DataTable dbDt = null;
            if (doPluginTests && !hasErroredOut)
            {
                try
                {
                    dbDt = plugin.Databases;
                }
                catch { dbDt = new DataTable(); }
            }

            //--------------------------------------------------------
            if (doPluginTests && !hasErroredOut)
            {
                foreach (DataRow dbRow in dbDt.Rows)
                {
                    string dbname = dbRow["CATALOG_NAME"].ToString();
                    try
                    {
                        DataTable dt = plugin.GetTables(dbname);
                        this.AppendLog(dt.Rows.Count + " tables in database " + dbname + " found through Plugin.");
                    }
                    catch (Exception ex)
                    {
                        hasErroredOut = true;
                        this.AppendLog("Plugin tables error in database " + dbname, ex);
                    }
                }
            }

            //--------------------------------------------------------
            if (doAPITests && !hasErroredOut)
            {
                foreach (IDatabase db in root.Databases)
                {
                    try
                    {
                        foreach (ITable tbl in db.Tables)
                        {
                            garbage = tbl.Name;
                            garbage = tbl.Schema;
                            garbage = tbl.DateCreated.ToString();
                            garbage = tbl.DateModified.ToString();
                            garbage = tbl.Description;
                            garbage = tbl.Guid.ToString();
                            garbage = tbl.PropID.ToString();
                            garbage = tbl.Type;
                        }

                        this.AppendLog(db.Tables.Count + " tables in database " + db.Name + " traversed successfully through MyMeta.");

                    }
                    catch (Exception ex)
                    {
                        hasErroredOut = true;
                        this.AppendLog("Error traversing tables in database " + db.Name + " through MyMeta", ex);
                    }
                }
            }


            //--------------------------------------------------------
            if (doPluginTests && !hasErroredOut)
            {
                foreach (DataRow dbRow in dbDt.Rows)
                {
                    string dbname = dbRow["CATALOG_NAME"].ToString();
                    DataTable tblDt = plugin.GetTables(dbname);
                    foreach (DataRow tblRow in tblDt.Rows)
                    {
                        string tblname = tblRow["TABLE_NAME"].ToString();
                        try
                        {
                            DataTable dt = plugin.GetTableColumns(dbname, tblname);
                            this.AppendLog(dt.Rows.Count + " columns in table " + dbname + "." + tblname + " found through Plugin.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Plugin table column error in " + dbname + "." + tblname, ex);
                        }

                        try
                        {
                            List<string> pks = plugin.GetPrimaryKeyColumns(dbname, tblname);
                            this.AppendLog(pks.Count + " PK columns in table " + dbname + "." + tblname + " found through Plugin.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Plugin table PK column error in " + dbname + "." + tblname, ex);
                        }

                        try
                        {
                            DataTable dt = plugin.GetForeignKeys(dbname, tblname);
                            this.AppendLog(dt.Rows.Count + " FKs in table " + dbname + "." + tblname + " found through Plugin.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Plugin table FK error in " + dbname + "." + tblname, ex);
                        }
                    }
                }
            }

            //--------------------------------------------------------
            if (doAPITests && !hasErroredOut)
            {
                foreach (IDatabase db in root.Databases)
                {
                    foreach (ITable table in db.Tables)
                    {

                        try
                        {
                            foreach (IColumn column in table.Columns)
                            {
                                garbage = column.AutoKeyIncrement.ToString();
                                garbage = column.AutoKeySeed.ToString();
                                garbage = column.CharacterMaxLength.ToString();
                                garbage = column.CharacterOctetLength.ToString();
                                garbage = column.CharacterSetCatalog;
                                garbage = column.CharacterSetName;
                                garbage = column.CharacterSetSchema;
                                garbage = column.CompFlags.ToString();
                                garbage = column.DataType.ToString();
                                garbage = column.DataTypeName;
                                garbage = column.DataTypeNameComplete;
                                garbage = column.DateTimePrecision.ToString();
                                garbage = column.DbTargetType;
                                garbage = column.Default;
                                garbage = column.Description;
                                garbage = column.DomainCatalog;
                                garbage = column.DomainName;
                                garbage = column.DomainSchema;
                                garbage = column.Flags.ToString();
                                garbage = column.Guid.ToString();
                                garbage = column.HasDefault.ToString();
                                garbage = column.HasDomain.ToString();
                                garbage = column.IsAutoKey.ToString();
                                garbage = column.IsComputed.ToString();
                                garbage = column.IsInForeignKey.ToString();
                                garbage = column.IsInPrimaryKey.ToString();
                                garbage = column.IsNullable.ToString();
                                garbage = column.LanguageType;
                                garbage = column.LCID.ToString();
                                garbage = column.Name;
                                garbage = column.NumericPrecision.ToString();
                                garbage = column.NumericScale.ToString();
                                garbage = column.Ordinal.ToString();
                                garbage = column.PropID.ToString();
                                garbage = column.SortID.ToString();
                                //garbage = column.TDSCollation.ToString(); -- Null means empty?
                                garbage = column.TypeGuid.ToString();
                            }
                            this.AppendLog(table.Columns.Count + " table columns in database " + db.Name + "." + table.Name + " traversed successfully through MyMeta.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Error traversing table columns in " + db.Name + "." + table.Name + " through MyMeta", ex);
                        }

                        try
                        {
                            foreach (IForeignKey fk in table.ForeignKeys)
                            {
                                garbage = fk.Deferrability;
                                garbage = fk.DeleteRule;
                                garbage = fk.Name;
                                garbage = fk.PrimaryKeyName;
                                garbage = fk.UpdateRule;
                                garbage = fk.ForeignColumns.Count.ToString();
                                garbage = fk.PrimaryColumns.Count.ToString();
                                garbage = fk.ForeignTable.Name;
                                garbage = fk.PrimaryTable.Name;
                            }

                            this.AppendLog(table.ForeignKeys.Count + " FKs in table " + db.Name + "." + table.Name + " traversed successfully through MyMeta.");

                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Error traversing FKs in table " + db.Name + "." + table.Name + " through MyMeta", ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="root"></param>
        /// <param name="hasErroredOut"></param>
        private void TestViews(IMyMetaPlugin plugin, dbRoot root, ref bool hasErroredOut, ref bool doAPITests, ref bool doPluginTests)
        {
            string garbage;
            DataTable dbDt = null;
            if (doPluginTests && !hasErroredOut)
            {
                try
                {
                    dbDt = plugin.Databases;
                }
                catch { dbDt = new DataTable(); }
            }

            //--------------------------------------------------------
            if (doPluginTests && !hasErroredOut)
            {
                foreach (DataRow dbRow in dbDt.Rows)
                {
                    string dbname = dbRow["CATALOG_NAME"].ToString();
                    try
                    {
                        DataTable dt = plugin.GetViews(dbname);
                        this.AppendLog(dt.Rows.Count + " views in database " + dbname + " found through Plugin.");
                    }
                    catch (Exception ex)
                    {
                        hasErroredOut = true;
                        this.AppendLog("Plugin views error in database " + dbname, ex);
                    }
                }
            }

            //--------------------------------------------------------
            if (doAPITests && !hasErroredOut)
            {
                foreach (IDatabase db in root.Databases)
                {
                    try
                    {
                        foreach (IView view in db.Views)
                        {

                            garbage = view.Name;
                            garbage = view.Schema;
                            garbage = view.DateCreated.ToString();
                            garbage = view.DateModified.ToString();
                            garbage = view.Description;
                            garbage = view.Guid.ToString();
                            garbage = view.PropID.ToString();
                            garbage = view.Type;
                            garbage = view.IsUpdateable.ToString();
                            garbage = view.CheckOption.ToString();
                            garbage = view.ViewText;
                        }

                        this.AppendLog(db.Views.Count + " views in database " + db.Name + " traversed successfully through MyMeta.");

                    }
                    catch (Exception ex)
                    {
                        hasErroredOut = true;
                        this.AppendLog("Error traversing views in database " + db.Name + " through MyMeta", ex);
                    }
                }
            }

            //--------------------------------------------------------
            if (doPluginTests && !hasErroredOut)
            {
                foreach (DataRow dbRow in dbDt.Rows)
                {
                    string dbname = dbRow["CATALOG_NAME"].ToString();
                    DataTable viewDt = plugin.GetViews(dbname);
                    foreach (DataRow viewRow in viewDt.Rows)
                    {
                        string viewname = viewRow["TABLE_NAME"].ToString();
               
                        try
                        {
                            DataTable dt = plugin.GetViewColumns(dbname, viewname);
                            this.AppendLog(dt.Rows.Count + " columns in view " + dbname + "." + viewname + " found through Plugin.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Plugin view column error in " + dbname + "." + viewname, ex);
                        }

                        try
                        {
                            List<string> list = plugin.GetViewSubTables(dbname, viewname);
                            this.AppendLog(list.Count + " sub-tables in view " + dbname + "." + viewname + " found through Plugin.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Plugin view sub-tables error in " + dbname + "." + viewname, ex);
                        }

                        try
                        {
                            List<string> list = plugin.GetViewSubViews(dbname, viewname);
                            this.AppendLog(list.Count + " sub-views in view " + dbname + "." + viewname + " found through Plugin.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Plugin view sub-views error in " + dbname + "." + viewname, ex);
                        }
                    }
                }
            }

            //--------------------------------------------------------
            if (doAPITests && !hasErroredOut)
            {
                foreach (IDatabase db in root.Databases)
                {
                    foreach (IView view in db.Views)
                    {
                        try
                        {
                            foreach (IColumn column in view.Columns)
                            {
                                garbage = column.AutoKeyIncrement.ToString();
                                garbage = column.AutoKeySeed.ToString();
                                garbage = column.CharacterMaxLength.ToString();
                                garbage = column.CharacterOctetLength.ToString();
                                garbage = column.CharacterSetCatalog;
                                garbage = column.CharacterSetName;
                                garbage = column.CharacterSetSchema;
                                garbage = column.CompFlags.ToString();
                                garbage = column.DataType.ToString();
                                garbage = column.DataTypeName;
                                garbage = column.DataTypeNameComplete;
                                garbage = column.DateTimePrecision.ToString();
                                garbage = column.DbTargetType;
                                garbage = column.Default;
                                garbage = column.Description;
                                garbage = column.DomainCatalog;
                                garbage = column.DomainName;
                                garbage = column.DomainSchema;
                                garbage = column.Flags.ToString();
                                garbage = column.Guid.ToString();
                                garbage = column.HasDefault.ToString();
                                garbage = column.HasDomain.ToString();
                                garbage = column.IsAutoKey.ToString();
                                garbage = column.IsComputed.ToString();
                                garbage = column.IsInForeignKey.ToString();
                                garbage = column.IsInPrimaryKey.ToString();
                                garbage = column.IsNullable.ToString();
                                garbage = column.LanguageType;
                                garbage = column.LCID.ToString();
                                garbage = column.Name;
                                garbage = column.NumericPrecision.ToString();
                                garbage = column.NumericScale.ToString();
                                garbage = column.Ordinal.ToString();
                                garbage = column.PropID.ToString();
                                garbage = column.SortID.ToString();
                                //garbage = column.TDSCollation.ToString(); -- Null means empty?
                                garbage = column.TypeGuid.ToString();
                            }
                            this.AppendLog(view.Columns.Count + " view columns in database " + db.Name + "." + view.Name + " traversed successfully through MyMeta.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Error traversing view columns in " + db.Name + "." + view.Name + " through MyMeta", ex);
                        }

                        try
                        {
                            foreach (ITable subtable in view.SubTables)
                            {
                                garbage = subtable.Name;
                                garbage = subtable.Schema;
                                garbage = subtable.DateCreated.ToString();
                                garbage = subtable.DateModified.ToString();
                                garbage = subtable.Description;
                                garbage = subtable.Guid.ToString();
                                garbage = subtable.PropID.ToString();
                                garbage = subtable.Type;
                            }
                            this.AppendLog(view.SubTables.Count + " view sub-tables in database " + db.Name + "." + view.Name + " traversed successfully through MyMeta.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Error traversing view sub-tables in " + db.Name + "." + view.Name + " through MyMeta", ex);
                        }

                        try
                        {
                            foreach (IView subview in view.SubViews)
                            {
                                garbage = subview.Name;
                                garbage = subview.Schema;
                                garbage = subview.DateCreated.ToString();
                                garbage = subview.DateModified.ToString();
                                garbage = subview.Description;
                                garbage = subview.Guid.ToString();
                                garbage = subview.PropID.ToString();
                                garbage = subview.Type;
                                garbage = subview.IsUpdateable.ToString();
                                garbage = subview.CheckOption.ToString();
                                garbage = subview.ViewText;
                            }
                            this.AppendLog(view.SubViews.Count + " view sub-views in database " + db.Name + "." + view.Name + " traversed successfully through MyMeta.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Error traversing view sub-views in " + db.Name + "." + view.Name + " through MyMeta", ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="root"></param>
        /// <param name="hasErroredOut"></param>
        private void TestProcedures(IMyMetaPlugin plugin, dbRoot root, ref bool hasErroredOut, ref bool doAPITests, ref bool doPluginTests)
        {
            string garbage;
            DataTable dbDt = null;
            if (doPluginTests && !hasErroredOut)
            {
                try
                {
                    dbDt = plugin.Databases;
                }
                catch { dbDt = new DataTable(); }
            }

            //--------------------------------------------------------
            if (doPluginTests && !hasErroredOut)
            {
                foreach (DataRow dbRow in dbDt.Rows)
                {
                    string dbname = dbRow["CATALOG_NAME"].ToString();
                    try
                    {
                        DataTable dt = plugin.GetProcedures(dbname);
                        this.AppendLog(dt.Rows.Count + " procedures in database " + dbname + " found through Plugin.");
                    }
                    catch (Exception ex)
                    {
                        hasErroredOut = true;
                        this.AppendLog("Plugin procedures error in database " + dbname, ex);
                    }
                }
            }

            //--------------------------------------------------------
            if (doAPITests && !hasErroredOut)
            {
                foreach (IDatabase db in root.Databases)
                {
                    try
                    {
                        foreach (IProcedure procedure in db.Procedures)
                        {
                            garbage = procedure.Name;
                            garbage = procedure.Schema;
                            garbage = procedure.DateCreated.ToString();
                            garbage = procedure.DateModified.ToString();
                            garbage = procedure.Description;
                            garbage = procedure.ProcedureText;
                            garbage = procedure.Type.ToString();
                        }

                        this.AppendLog(db.Procedures.Count + " procedures in database " + db.Name + " traversed successfully through MyMeta.");

                    }
                    catch (Exception ex)
                    {
                        hasErroredOut = true;
                        this.AppendLog("Error traversing procedures in database " + db.Name + " through MyMeta", ex);
                    }
                }
            }



            //--------------------------------------------------------
            if (doPluginTests && !hasErroredOut)
            {
                foreach (DataRow dbRow in dbDt.Rows)
                {
                    string dbname = dbRow["CATALOG_NAME"].ToString();
                    DataTable procDt = plugin.GetProcedures(dbname);
                    foreach (DataRow procRow in procDt.Rows)
                    {
                        string procedurename = procRow["PROCEDURE_NAME"].ToString();
                        try
                        {
                            DataTable dt = plugin.GetProcedureParameters(dbname, procedurename);
                            this.AppendLog(dt.Rows.Count + " parameters in procedure " + dbname + "." + procedurename + " found through Plugin.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Plugin procedure parameter error in " + dbname + "." + procedurename, ex);
                        }

                        try
                        {
                            DataTable dt = plugin.GetProcedureResultColumns(dbname, procedurename);
                            this.AppendLog(dt.Rows.Count + " result columns in procedure " + dbname + "." + procedurename + " found through Plugin.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Plugin procedure result columns error in " + dbname + "." + procedurename, ex);
                        }
                    }
                }
            }

            //--------------------------------------------------------
            if (doAPITests && !hasErroredOut)
            {
                foreach (IDatabase db in root.Databases)
                {
                    foreach (IProcedure procedure in db.Procedures)
                    {
                        try
                        {
                            foreach (IParameter parameter in procedure.Parameters)
                            {
                                garbage = parameter.CharacterMaxLength.ToString();
                                garbage = parameter.CharacterOctetLength.ToString();
                                garbage = parameter.DataType.ToString();
                                garbage = parameter.DataTypeNameComplete;
                                garbage = parameter.DbTargetType;
                                garbage = parameter.Default;
                                garbage = parameter.Description;
                                garbage = parameter.Direction.ToString();
                                garbage = parameter.LocalTypeName.ToString();
                                garbage = parameter.HasDefault.ToString();
                                garbage = parameter.IsNullable.ToString();
                                garbage = parameter.LanguageType;
                                garbage = parameter.Name;
                                garbage = parameter.NumericPrecision.ToString();
                                garbage = parameter.NumericScale.ToString();
                                garbage = parameter.Ordinal.ToString();
                                garbage = parameter.TypeName.ToString();
                            }
                            this.AppendLog(procedure.Parameters.Count + " procedure parameters in " + db.Name + "." + procedure.Name + " traversed successfully through MyMeta.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Error traversing procedure parameters in " + db.Name + "." + procedure.Name + " through MyMeta", ex);
                        }

                        try
                        {
                            foreach (IResultColumn column in procedure.ResultColumns)
                            {
                                garbage = column.DataType.ToString();
                                garbage = column.DataTypeName;
                                garbage = column.DataTypeNameComplete;
                                garbage = column.DbTargetType;
                                garbage = column.LanguageType;
                                garbage = column.Name;
                                garbage = column.Ordinal.ToString();
                            }
                            this.AppendLog(procedure.ResultColumns.Count + " procedure result columns in " + db.Name + "." + procedure.Name + " traversed successfully through MyMeta.");
                        }
                        catch (Exception ex)
                        {
                            hasErroredOut = true;
                            this.AppendLog("Error traversing procedure result columns in " + db.Name + "." + procedure.Name + " through MyMeta", ex);
                        }
                    }
                }
            }
        }

        private void AppendLog(string message)
        {
            this.textBoxResults.AppendText(DateTime.Now.ToString());
            this.textBoxResults.AppendText(" - " + message);
            this.textBoxResults.AppendText(Environment.NewLine);
        }

        private void AppendLog(string message, Exception ex)
        {
            this.textBoxResults.AppendText(DateTime.Now.ToString());
            this.textBoxResults.AppendText(" - " + message);
            this.textBoxResults.AppendText(": " + ex.Message + ex);
            this.textBoxResults.AppendText(Environment.NewLine);
        }
    }
}
