using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace MyGeneration
{
    public class ExceptionDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.TextBox _errorTextBox;


		private System.Windows.Forms.Button _reportErrorButton;

		public ExceptionDialog()
		{
		    InitializeComponent();
		}

	    public ExceptionDialog PopulateUIExceptionDetails(Exception exception)
	    {
	        StringBuilder sb = GetExceptionDetails(exception);
            _errorTextBox.Text = sb.ToString();
            return this;
	    }

	    private static StringBuilder GetExceptionDetails(Exception exception)
	    {
	        Assembly myGenerationAssembly = Assembly.GetAssembly(typeof(MyGenerationMDI));

	        var sb = new StringBuilder();
	        sb.Append("MyGeneration".PadRight(15) + myGenerationAssembly.GetName().Version + "\r\n");
	        sb.Append(DateTime.Now + "\r\n" + "\r\n");

	        Exception tmp = exception;
	        while (tmp != null)
	        {
	            sb.Append("------------------------------------------\r\n");
	            sb.Append(exception.Message + "\r\n\r\n");
	            sb.Append(exception.TargetSite + "\r\n\r\n");
	            sb.Append("Call Stack");
	            sb.Append(exception + "\r\n\r\n");

	            tmp = exception.InnerException;
	        }
	        return sb;
	    }

	    #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this._cancelButton = new System.Windows.Forms.Button();
            this._errorTextBox = new System.Windows.Forms.TextBox();
            this._reportErrorButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _cancelButton
            // 
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(471, 442);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "&Cancel";
            // 
            // _errorTextBox
            // 
            this._errorTextBox.BackColor = System.Drawing.Color.White;
            this._errorTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._errorTextBox.Location = new System.Drawing.Point(24, 12);
            this._errorTextBox.Multiline = true;
            this._errorTextBox.Name = "_errorTextBox";
            this._errorTextBox.ReadOnly = true;
            this._errorTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._errorTextBox.Size = new System.Drawing.Size(640, 420);
            this._errorTextBox.TabIndex = 2;
            // 
            // _reportErrorButton
            // 
            this._reportErrorButton.Location = new System.Drawing.Point(552, 442);
            this._reportErrorButton.Name = "_reportErrorButton";
            this._reportErrorButton.Size = new System.Drawing.Size(112, 23);
            this._reportErrorButton.TabIndex = 3;
            this._reportErrorButton.Text = "Report Error";
            this._reportErrorButton.Click += new System.EventHandler(this.ReportErrorButton_OnClicked);
            // 
            // ExceptionDialog
            // 
            this.AcceptButton = this._reportErrorButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(680, 477);
            this.Controls.Add(this._reportErrorButton);
            this.Controls.Add(this._errorTextBox);
            this.Controls.Add(this._cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExceptionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MyGeneration Exception";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

	    private void ReportErrorButton_OnClicked(object sender, EventArgs e)
	    {
	        Zeus.WindowsTools.LaunchBrowser("https://github.com/cafephin/mygeneration/issues", ProcessWindowStyle.Normal, false);
	    }
	}
}
