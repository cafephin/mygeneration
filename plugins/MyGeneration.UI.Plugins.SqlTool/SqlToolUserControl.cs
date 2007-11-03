using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using Scintilla;
using Scintilla.Forms;
using Scintilla.Enums;
using Scintilla.Configuration;
using MyMeta;

namespace MyGeneration.UI.Plugins.SqlTool
{
    public partial class SqlToolUserControl : UserControl
    {
        private IMyGenerationMDI _mdi;
        private ScintillaConfig _scintillaConfig;
        private IDbConnection _connection;
        private string _filename = string.Empty;
        private string _fileId;
        private bool _isNew = true;

        public SqlToolUserControl()
        {
            InitializeComponent();

            _scintillaConfig = new ScintillaConfig();
            this.scintilla.Configure = _scintillaConfig.Configure;
            this.scintilla.ConfigurationLanguage = "sql";
            this.scintilla.SmartIndentType = SmartIndent.Simple;
        }

        public void Initialize(IMyGenerationMDI mdi)
        {
            this._fileId = Guid.NewGuid().ToString();
            _mdi = mdi;
        }

        public void Open(string filename)
        {
            this._filename = filename;
            if (File.Exists(_filename))
            {
                try
                {
                    this.scintilla.Clear();
                    this.scintilla.Text = File.ReadAllText(_filename);
                    _isNew = false;
                    SetClean();
                }
                catch (Exception x)
                {
                    _mdi.Console.Write(x);
                    _isNew = true;
                }
            }
        }

        private void FileSave()
        {
            if (!string.IsNullOrEmpty(this.FileName))
            {
                string path = this.FileName;
                FileInfo fi = new FileInfo(path);
                if (fi.Exists && fi.IsReadOnly)
                {
                    MessageBox.Show(this, "File is read only.");
                    this._mdi.Console.Write("File \"{0}\" is read only, cannot save.", fi.FullName);
                }
                else
                {
                    try
                    {
                        File.WriteAllText(path, scintilla.Text);
                        SetClean();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Error saving file. " + ex.Message);
                        this._mdi.Console.Write(ex);
                    }

                }
            }
        }

        private void FileSaveAs(string path)
        {
            bool isopen = _mdi.IsDocumentOpen(path, this.Parent as IMyGenDocument);

            if (!isopen)
            {
                FileInfo fi = new FileInfo(path);
                if (fi.Exists)
                {
                    if (fi.IsReadOnly)
                    {
                        MessageBox.Show(this, "File is read only, cannot save.");
                        this._mdi.Console.Write("File \"{0}\" is read only, cannot save.", fi.FullName);
                    }
                    else
                    {
                        try
                        {
                            File.WriteAllText(path, scintilla.Text);
                            this._filename = path;

                            SetClean();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, "Error saving file. " + ex.Message);
                            this._mdi.Console.Write("Error saving file.");
                            this._mdi.Console.Write(ex);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(this, "The template you are trying to overwrite is currently open.\r\nClose the editor window for that template if you want to overwrite it.");
            }
        }

        public void Save()
        {
            if (this._isNew)
            {
                this.SaveAs();
            }
            else
            {
                this.FileSave();
            }
            this.scintilla.GrabFocus();
        }

        public void SaveAs()
        {
            Stream myStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "SQL Files (*.sql)|*.sql|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            saveFileDialog.FileName = this.FileName;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                myStream = saveFileDialog.OpenFile();

                if (null != myStream)
                {
                    myStream.Close();
                    this.FileSaveAs(saveFileDialog.FileName);
                }
            }
            this.scintilla.GrabFocus();
        }

        public IDbConnection Connection 
        {
            get
            {
                if (_connection == null)
                {
                    string dbdriver = _mdi.PerformMdiFuntion(this.Parent as IMyGenContent, "getmymetadbdriver") as String;
                    string connString = _mdi.PerformMdiFuntion(this.Parent as IMyGenContent, "getmymetaconnection") as String;
                    dbRoot mymeta = new dbRoot();
                    _connection = mymeta.BuildConnection(dbdriver, connString);
                }
                return _connection;
            }
        }

        public void Execute()
        {
            IDbConnection conn = null;
            IDataReader r = null;
            try
            {
                conn = Connection;
                conn.Open();

                IDbCommand db = conn.CreateCommand();
                db.CommandText = scintilla.Text;
                r = db.ExecuteReader();

                this.dataGridViewResults.Rows.Clear();
                this.dataGridViewResults.Columns.Clear();
                int rowindex = 0;
                if ((r != null) && (!r.IsClosed))
                {
                    while (r.Read())
                    {
                        if (rowindex == 0)
                        {
                            for (int i = 0; i < r.FieldCount; i++)
                            {

                                dataGridViewResults.Columns.Add(r.GetName(i), r.GetName(i));
                            }
                        }

                        dataGridViewResults.Rows.Add();
                        for (int i = 0; i < r.FieldCount; i++)
                        {
                            dataGridViewResults.Rows[rowindex].Cells[i].Value = r[i];
                        }
                        rowindex++;
                    }
                }

                if (rowindex == 0)
                {
                    dataGridViewResults.Columns.Add("Result", "Result");
                    dataGridViewResults.Rows.Add();
                    dataGridViewResults.Rows[0].Cells[0].Value = "";
                }
                r.Close();
                r.Dispose();

                conn.Close();
                conn.Dispose();

            }
            catch (Exception ex)
            {
                if (r != null) r.Dispose();
                if (conn != null) conn.Dispose();

                dataGridViewResults.Rows.Clear();
                dataGridViewResults.Columns.Clear();
                dataGridViewResults.Columns.Add("Error", "Error");
                dataGridViewResults.Rows.Add();
                dataGridViewResults.Rows[0].Cells[0].Value = ex.Message;

                this._mdi.ErrorList.AddErrors(ex);
            }
        }

        public string FileName
        {
            get
            {
                return _filename; 
            }
        }

        public bool IsDirty
        {
            get
            {
                return (this.scintilla.IsModify);
            }
        }

        public string DocumentIndentity
        {
            get { return string.IsNullOrEmpty(_filename) ? _fileId : _filename; }
        }

        public string TextContent
        {
            get
            {
                return scintilla.Text;
            }
        }

        protected void SetClean()
        {
            scintilla.EmptyUndoBuffer();
            scintilla.SetSavePoint();
        }
    }
}
