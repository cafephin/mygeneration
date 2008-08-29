using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MyGeneration;
using Zeus;

namespace MyGeneration.Forms
{
    public partial class ApplicationReleasesControl : UserControl
    {
        public ApplicationReleasesControl()
        {
            InitializeComponent();
        }

        private void ApplicationReleasesControl_Load(object sender, EventArgs e)
        {
            try
            {
                //TODO: need to spin off another thread here at some point
                foreach (IAppRelease rel in ZeusController.Instance.ReleaseList)
                {
                    int i = this.dataGridViewUpdates.Rows.Add();

                    //this.dataGridViewUpdates.Rows[i].Cells[this.ColumnDate.Index].Value = rel.Date;
                    this.dataGridViewUpdates.Rows[i].Cells[this.ColumnTitle.Index].Value = rel.Title;
                    this.dataGridViewUpdates.Rows[i].Cells[this.ColumnDownload.Index].Tag = rel.DownloadLink;
                    this.dataGridViewUpdates.Rows[i].Cells[this.ColumnReleaseNotes.Index].Tag = rel.ReleaseNotesLink;
                }
            }
            catch { }
        }

        private void dataGridViewUpdates_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == this.ColumnDownload.Index)
                {
                    Uri u = this.dataGridViewUpdates.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag as Uri;
                    WindowsTools.LaunchBrowser(u.AbsoluteUri, System.Diagnostics.ProcessWindowStyle.Normal, true);
                }
                else if (e.ColumnIndex == this.ColumnReleaseNotes.Index)
                {
                    Uri u = this.dataGridViewUpdates.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag as Uri;
                    WindowsTools.LaunchBrowser(u.AbsoluteUri, System.Diagnostics.ProcessWindowStyle.Normal, true);
                }
            }
        }
    }
}
