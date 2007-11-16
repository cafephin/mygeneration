namespace MyGenVS2005
{
    partial class AddInTemplateBrowser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.templateBrowserControl1 = new MyGeneration.TemplateBrowserControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // templateBrowserControl1
            // 
            this.templateBrowserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.templateBrowserControl1.Location = new System.Drawing.Point(0, 0);
            this.templateBrowserControl1.Name = "templateBrowserControl1";
            this.templateBrowserControl1.Size = new System.Drawing.Size(288, 422);
            this.templateBrowserControl1.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.templateBrowserControl1);
            this.splitContainer1.Size = new System.Drawing.Size(288, 464);
            this.splitContainer1.SplitterDistance = 422;
            this.splitContainer1.TabIndex = 1;
            // 
            // AddInTemplateBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 464);
            this.Controls.Add(this.splitContainer1);
            this.Name = "AddInTemplateBrowser";
            this.Text = "AddInTemplateBrowser";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MyGeneration.TemplateBrowserControl templateBrowserControl1;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}