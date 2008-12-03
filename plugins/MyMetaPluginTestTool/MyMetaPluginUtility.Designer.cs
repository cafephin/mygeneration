namespace MyMetaPluginTestTool
{
    partial class MyMetaPluginUtility
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
            this.tabControlPluginStuff = new System.Windows.Forms.TabControl();
            this.tabPageTest = new System.Windows.Forms.TabPage();
            this.textBoxResults = new System.Windows.Forms.TextBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxConnectionString = new System.Windows.Forms.TextBox();
            this.tabPageData = new System.Windows.Forms.TabPage();
            this.comboBoxPlugins = new System.Windows.Forms.ComboBox();
            this.checkBoxAPI = new System.Windows.Forms.CheckBox();
            this.checkBoxPlugin = new System.Windows.Forms.CheckBox();
            this.tabControlPluginStuff.SuspendLayout();
            this.tabPageTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlPluginStuff
            // 
            this.tabControlPluginStuff.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlPluginStuff.Controls.Add(this.tabPageTest);
            this.tabControlPluginStuff.Controls.Add(this.tabPageData);
            this.tabControlPluginStuff.Location = new System.Drawing.Point(12, 39);
            this.tabControlPluginStuff.Name = "tabControlPluginStuff";
            this.tabControlPluginStuff.SelectedIndex = 0;
            this.tabControlPluginStuff.Size = new System.Drawing.Size(487, 315);
            this.tabControlPluginStuff.TabIndex = 0;
            // 
            // tabPageTest
            // 
            this.tabPageTest.Controls.Add(this.checkBoxPlugin);
            this.tabPageTest.Controls.Add(this.checkBoxAPI);
            this.tabPageTest.Controls.Add(this.textBoxResults);
            this.tabPageTest.Controls.Add(this.buttonTest);
            this.tabPageTest.Controls.Add(this.label1);
            this.tabPageTest.Controls.Add(this.textBoxConnectionString);
            this.tabPageTest.Location = new System.Drawing.Point(4, 22);
            this.tabPageTest.Name = "tabPageTest";
            this.tabPageTest.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTest.Size = new System.Drawing.Size(479, 289);
            this.tabPageTest.TabIndex = 0;
            this.tabPageTest.Text = "Test";
            this.tabPageTest.UseVisualStyleBackColor = true;
            // 
            // textBoxResults
            // 
            this.textBoxResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResults.Location = new System.Drawing.Point(7, 49);
            this.textBoxResults.MaxLength = 999999;
            this.textBoxResults.Multiline = true;
            this.textBoxResults.Name = "textBoxResults";
            this.textBoxResults.Size = new System.Drawing.Size(469, 234);
            this.textBoxResults.TabIndex = 3;
            // 
            // buttonTest
            // 
            this.buttonTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTest.Location = new System.Drawing.Point(411, 19);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(62, 23);
            this.buttonTest.TabIndex = 2;
            this.buttonTest.Text = "Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Connection String";
            // 
            // textBoxConnectionString
            // 
            this.textBoxConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxConnectionString.Location = new System.Drawing.Point(6, 22);
            this.textBoxConnectionString.Name = "textBoxConnectionString";
            this.textBoxConnectionString.Size = new System.Drawing.Size(399, 20);
            this.textBoxConnectionString.TabIndex = 0;
            // 
            // tabPageData
            // 
            this.tabPageData.Location = new System.Drawing.Point(4, 22);
            this.tabPageData.Name = "tabPageData";
            this.tabPageData.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageData.Size = new System.Drawing.Size(479, 289);
            this.tabPageData.TabIndex = 1;
            this.tabPageData.Text = "Data";
            this.tabPageData.UseVisualStyleBackColor = true;
            // 
            // comboBoxPlugins
            // 
            this.comboBoxPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPlugins.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPlugins.FormattingEnabled = true;
            this.comboBoxPlugins.Location = new System.Drawing.Point(12, 13);
            this.comboBoxPlugins.Name = "comboBoxPlugins";
            this.comboBoxPlugins.Size = new System.Drawing.Size(483, 21);
            this.comboBoxPlugins.TabIndex = 1;
            this.comboBoxPlugins.SelectedIndexChanged += new System.EventHandler(this.comboBoxPlugins_SelectedIndexChanged);
            // 
            // checkBoxAPI
            // 
            this.checkBoxAPI.AutoSize = true;
            this.checkBoxAPI.Checked = true;
            this.checkBoxAPI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAPI.Location = new System.Drawing.Point(388, 3);
            this.checkBoxAPI.Name = "checkBoxAPI";
            this.checkBoxAPI.Size = new System.Drawing.Size(95, 17);
            this.checkBoxAPI.TabIndex = 4;
            this.checkBoxAPI.Text = "Do API Tests?";
            this.checkBoxAPI.UseVisualStyleBackColor = true;
            // 
            // checkBoxPlugin
            // 
            this.checkBoxPlugin.AutoSize = true;
            this.checkBoxPlugin.Checked = true;
            this.checkBoxPlugin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPlugin.Location = new System.Drawing.Point(277, 3);
            this.checkBoxPlugin.Name = "checkBoxPlugin";
            this.checkBoxPlugin.Size = new System.Drawing.Size(107, 17);
            this.checkBoxPlugin.TabIndex = 5;
            this.checkBoxPlugin.Text = "Do Plugin Tests?";
            this.checkBoxPlugin.UseVisualStyleBackColor = true;
            // 
            // MyMetaPluginUtility
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 366);
            this.Controls.Add(this.comboBoxPlugins);
            this.Controls.Add(this.tabControlPluginStuff);
            this.Name = "MyMetaPluginUtility";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MyMetaPluginUtility_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MyMetaPluginUtility_FormClosing);
            this.tabControlPluginStuff.ResumeLayout(false);
            this.tabPageTest.ResumeLayout(false);
            this.tabPageTest.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlPluginStuff;
        private System.Windows.Forms.TabPage tabPageTest;
        private System.Windows.Forms.TabPage tabPageData;
        private System.Windows.Forms.ComboBox comboBoxPlugins;
        private System.Windows.Forms.TextBox textBoxConnectionString;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.TextBox textBoxResults;
        private System.Windows.Forms.CheckBox checkBoxAPI;
        private System.Windows.Forms.CheckBox checkBoxPlugin;
    }
}

