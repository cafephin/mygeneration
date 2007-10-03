namespace MyGeneration.Forms
{
    partial class ErrorDetailControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxFile = new System.Windows.Forms.TextBox();
            this.labelFile = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.textBoxMethod = new System.Windows.Forms.TextBox();
            this.textBoxColumn = new System.Windows.Forms.TextBox();
            this.textBoxLine = new System.Windows.Forms.TextBox();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.textBoxStackTrace = new System.Windows.Forms.TextBox();
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.textBoxExceptionType = new System.Windows.Forms.TextBox();
            this.labelMethod = new System.Windows.Forms.Label();
            this.labelColumn = new System.Windows.Forms.Label();
            this.labelLine = new System.Windows.Forms.Label();
            this.labelStackTrace = new System.Windows.Forms.Label();
            this.labelMessage = new System.Windows.Forms.Label();
            this.labelSource = new System.Windows.Forms.Label();
            this.labelExceptionType = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxFile
            // 
            this.textBoxFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFile.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxFile.Location = new System.Drawing.Point(12, 50);
            this.textBoxFile.Name = "textBoxFile";
            this.textBoxFile.ReadOnly = true;
            this.textBoxFile.Size = new System.Drawing.Size(535, 20);
            this.textBoxFile.TabIndex = 40;
            // 
            // labelFile
            // 
            this.labelFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFile.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFile.Location = new System.Drawing.Point(12, 34);
            this.labelFile.Name = "labelFile";
            this.labelFile.Size = new System.Drawing.Size(535, 16);
            this.labelFile.TabIndex = 39;
            this.labelFile.Text = "File:";
            this.labelFile.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelTitle
            // 
            this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTitle.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(12, 10);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(535, 24);
            this.labelTitle.TabIndex = 37;
            this.labelTitle.Text = "System Exception";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxMethod
            // 
            this.textBoxMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMethod.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMethod.Location = new System.Drawing.Point(12, 170);
            this.textBoxMethod.Name = "textBoxMethod";
            this.textBoxMethod.ReadOnly = true;
            this.textBoxMethod.Size = new System.Drawing.Size(535, 20);
            this.textBoxMethod.TabIndex = 36;
            // 
            // textBoxColumn
            // 
            this.textBoxColumn.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxColumn.Location = new System.Drawing.Point(92, 210);
            this.textBoxColumn.Name = "textBoxColumn";
            this.textBoxColumn.ReadOnly = true;
            this.textBoxColumn.Size = new System.Drawing.Size(72, 20);
            this.textBoxColumn.TabIndex = 32;
            // 
            // textBoxLine
            // 
            this.textBoxLine.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLine.Location = new System.Drawing.Point(12, 210);
            this.textBoxLine.Name = "textBoxLine";
            this.textBoxLine.ReadOnly = true;
            this.textBoxLine.Size = new System.Drawing.Size(72, 20);
            this.textBoxLine.TabIndex = 31;
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMessage.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMessage.Location = new System.Drawing.Point(12, 250);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ReadOnly = true;
            this.textBoxMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxMessage.Size = new System.Drawing.Size(535, 40);
            this.textBoxMessage.TabIndex = 30;
            // 
            // textBoxStackTrace
            // 
            this.textBoxStackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStackTrace.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxStackTrace.Location = new System.Drawing.Point(12, 306);
            this.textBoxStackTrace.Multiline = true;
            this.textBoxStackTrace.Name = "textBoxStackTrace";
            this.textBoxStackTrace.ReadOnly = true;
            this.textBoxStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxStackTrace.Size = new System.Drawing.Size(535, 276);
            this.textBoxStackTrace.TabIndex = 28;
            // 
            // textBoxSource
            // 
            this.textBoxSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSource.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSource.Location = new System.Drawing.Point(12, 130);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.ReadOnly = true;
            this.textBoxSource.Size = new System.Drawing.Size(535, 20);
            this.textBoxSource.TabIndex = 26;
            // 
            // textBoxExceptionType
            // 
            this.textBoxExceptionType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExceptionType.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxExceptionType.Location = new System.Drawing.Point(12, 90);
            this.textBoxExceptionType.Name = "textBoxExceptionType";
            this.textBoxExceptionType.ReadOnly = true;
            this.textBoxExceptionType.Size = new System.Drawing.Size(535, 20);
            this.textBoxExceptionType.TabIndex = 24;
            // 
            // labelMethod
            // 
            this.labelMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMethod.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMethod.Location = new System.Drawing.Point(12, 154);
            this.labelMethod.Name = "labelMethod";
            this.labelMethod.Size = new System.Drawing.Size(535, 16);
            this.labelMethod.TabIndex = 35;
            this.labelMethod.Text = "Method:";
            this.labelMethod.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelColumn
            // 
            this.labelColumn.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelColumn.Location = new System.Drawing.Point(92, 194);
            this.labelColumn.Name = "labelColumn";
            this.labelColumn.Size = new System.Drawing.Size(72, 16);
            this.labelColumn.TabIndex = 34;
            this.labelColumn.Text = "Column:";
            this.labelColumn.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelLine
            // 
            this.labelLine.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLine.Location = new System.Drawing.Point(12, 194);
            this.labelLine.Name = "labelLine";
            this.labelLine.Size = new System.Drawing.Size(72, 16);
            this.labelLine.TabIndex = 33;
            this.labelLine.Text = "Line:";
            this.labelLine.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelStackTrace
            // 
            this.labelStackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStackTrace.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStackTrace.Location = new System.Drawing.Point(12, 290);
            this.labelStackTrace.Name = "labelStackTrace";
            this.labelStackTrace.Size = new System.Drawing.Size(535, 16);
            this.labelStackTrace.TabIndex = 29;
            this.labelStackTrace.Text = "Stack Trace:";
            this.labelStackTrace.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelMessage
            // 
            this.labelMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMessage.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.Location = new System.Drawing.Point(12, 234);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(535, 16);
            this.labelMessage.TabIndex = 27;
            this.labelMessage.Text = "Message:";
            this.labelMessage.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelSource
            // 
            this.labelSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSource.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSource.Location = new System.Drawing.Point(12, 114);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(535, 16);
            this.labelSource.TabIndex = 25;
            this.labelSource.Text = "Source:";
            this.labelSource.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelExceptionType
            // 
            this.labelExceptionType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelExceptionType.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExceptionType.Location = new System.Drawing.Point(12, 74);
            this.labelExceptionType.Name = "labelExceptionType";
            this.labelExceptionType.Size = new System.Drawing.Size(535, 16);
            this.labelExceptionType.TabIndex = 23;
            this.labelExceptionType.Text = "Exception Type:";
            this.labelExceptionType.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // ErrorDetailControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxFile);
            this.Controls.Add(this.labelFile);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.textBoxMethod);
            this.Controls.Add(this.textBoxColumn);
            this.Controls.Add(this.textBoxLine);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.textBoxStackTrace);
            this.Controls.Add(this.textBoxSource);
            this.Controls.Add(this.textBoxExceptionType);
            this.Controls.Add(this.labelMethod);
            this.Controls.Add(this.labelColumn);
            this.Controls.Add(this.labelLine);
            this.Controls.Add(this.labelStackTrace);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.labelSource);
            this.Controls.Add(this.labelExceptionType);
            this.Name = "ErrorDetailControl";
            this.Size = new System.Drawing.Size(566, 600);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxFile;
        private System.Windows.Forms.Label labelFile;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TextBox textBoxMethod;
        private System.Windows.Forms.TextBox textBoxColumn;
        private System.Windows.Forms.TextBox textBoxLine;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.TextBox textBoxStackTrace;
        private System.Windows.Forms.TextBox textBoxSource;
        private System.Windows.Forms.TextBox textBoxExceptionType;
        private System.Windows.Forms.Label labelMethod;
        private System.Windows.Forms.Label labelColumn;
        private System.Windows.Forms.Label labelLine;
        private System.Windows.Forms.Label labelStackTrace;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Label labelSource;
        private System.Windows.Forms.Label labelExceptionType;
    }
}
