using Microsoft.Win32;
namespace ScriptControlTest
{
    partial class Form1
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
            this.labelKeyName = new System.Windows.Forms.Label();
            this.labelKeyValue = new System.Windows.Forms.Label();
            this.textBoxVariable = new System.Windows.Forms.TextBox();
            this.textBoxCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelCode = new System.Windows.Forms.Label();
            this.buttonExec = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxJsonVar = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelKeyName
            // 
            this.labelKeyName.AutoSize = true;
            this.labelKeyName.ForeColor = System.Drawing.Color.Blue;
            this.labelKeyName.Location = new System.Drawing.Point(11, 9);
            this.labelKeyName.Name = "labelKeyName";
            this.labelKeyName.Size = new System.Drawing.Size(76, 13);
            this.labelKeyName.TabIndex = 0;
            this.labelKeyName.Text = "Key not found.";
            // 
            // labelKeyValue
            // 
            this.labelKeyValue.AutoSize = true;
            this.labelKeyValue.ForeColor = System.Drawing.Color.Green;
            this.labelKeyValue.Location = new System.Drawing.Point(12, 31);
            this.labelKeyValue.Name = "labelKeyValue";
            this.labelKeyValue.Size = new System.Drawing.Size(53, 13);
            this.labelKeyValue.TabIndex = 1;
            this.labelKeyValue.Text = "No value.";
            // 
            // textBoxVariable
            // 
            this.textBoxVariable.Location = new System.Drawing.Point(50, 50);
            this.textBoxVariable.Multiline = true;
            this.textBoxVariable.Name = "textBoxVariable";
            this.textBoxVariable.Size = new System.Drawing.Size(70, 19);
            this.textBoxVariable.TabIndex = 2;
            this.textBoxVariable.Text = "12";
            this.textBoxVariable.WordWrap = false;
            // 
            // textBoxCode
            // 
            this.textBoxCode.Location = new System.Drawing.Point(241, 69);
            this.textBoxCode.Multiline = true;
            this.textBoxCode.Name = "textBoxCode";
            this.textBoxCode.Size = new System.Drawing.Size(432, 105);
            this.textBoxCode.TabIndex = 3;
            this.textBoxCode.Text = "(function() { \r\n   var x = 1000;\r\n   return fileInfo.Name + \' ==> \' + (x + parseI" +
    "nt(y.toString()) + \r\n      jsonVar.number);\r\n})()";
            this.textBoxCode.WordWrap = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "var y = ";
            // 
            // labelCode
            // 
            this.labelCode.AutoSize = true;
            this.labelCode.Location = new System.Drawing.Point(238, 53);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(72, 13);
            this.labelCode.TabIndex = 5;
            this.labelCode.Text = "Code To Eval";
            // 
            // buttonExec
            // 
            this.buttonExec.Location = new System.Drawing.Point(598, 180);
            this.buttonExec.Name = "buttonExec";
            this.buttonExec.Size = new System.Drawing.Size(75, 23);
            this.buttonExec.TabIndex = 6;
            this.buttonExec.Text = "Execute";
            this.buttonExec.UseVisualStyleBackColor = true;
            this.buttonExec.Click += new System.EventHandler(this.buttonExec_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "var jsonVar = ";
            // 
            // textBoxJsonVar
            // 
            this.textBoxJsonVar.Location = new System.Drawing.Point(15, 85);
            this.textBoxJsonVar.Multiline = true;
            this.textBoxJsonVar.Name = "textBoxJsonVar";
            this.textBoxJsonVar.Size = new System.Drawing.Size(200, 89);
            this.textBoxJsonVar.TabIndex = 7;
            this.textBoxJsonVar.Text = "{ number: 100 }";
            this.textBoxJsonVar.WordWrap = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 209);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxJsonVar);
            this.Controls.Add(this.buttonExec);
            this.Controls.Add(this.labelCode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxCode);
            this.Controls.Add(this.textBoxVariable);
            this.Controls.Add(this.labelKeyValue);
            this.Controls.Add(this.labelKeyName);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelKeyName;
        private System.Windows.Forms.Label labelKeyValue;
        private System.Windows.Forms.TextBox textBoxVariable;
        private System.Windows.Forms.TextBox textBoxCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelCode;
        private System.Windows.Forms.Button buttonExec;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxJsonVar;
    }
}

