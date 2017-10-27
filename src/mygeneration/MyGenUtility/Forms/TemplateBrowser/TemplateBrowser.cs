using System;
using System.Windows.Forms;
using MyGeneration.Configuration;
using WeifenLuo.WinFormsUI.Docking;
using Zeus;

namespace MyGeneration
{
    public partial class TemplateBrowser : DockContent, IMyGenContent
    {
        private bool _consoleWriteGeneratedDetails;
        private IMyGenerationMDI _mdi;
        private ZeusProcessStatusDelegate _executionCallback;

        public TemplateBrowser(IMyGenerationMDI mdi)
        {
            this._mdi = mdi;
            this._executionCallback = ExecutionCallback;
            this._consoleWriteGeneratedDetails = DefaultSettings.Instance.MiscSettings.ConsoleWriteGeneratedDetails;
            this.DockPanel = mdi.DockPanel;

            InitializeComponent();

            this.templateBrowserControl.Initialize();
            if (DefaultSettings.Instance.TemplateSettings.ExecuteFromTemplateBrowserAsync)
            {
                this.templateBrowserControl.ExecuteTemplateOverride = ExecuteTemplateOverride;
            }
        }

        private bool ExecuteTemplateOverride(TemplateOperations operation, ZeusTemplate template, ZeusSavedInput input, ShowGUIEventHandler guiEventHandler)
        {
            switch (operation)
            {
                case TemplateOperations.Execute:
                    this._mdi.PerformMdiFunction(this, "ExecutionQueueStart");
                    ZeusProcessManager.ExecuteTemplate(template.FullFileName, _executionCallback);
                    break;
                case TemplateOperations.ExecuteLoadedInput:
                    this._mdi.PerformMdiFunction(this, "ExecutionQueueStart");
                    ZeusProcessManager.ExecuteSavedInput(input.FilePath, _executionCallback);
                    break;
                case TemplateOperations.SaveInput:
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Zues Input Files (*.zinp)|*.zinp";
                    saveFileDialog.FilterIndex = 0;
                    saveFileDialog.RestoreDirectory = true;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        this._mdi.PerformMdiFunction(this, "ExecutionQueueStart");
                        ZeusProcessManager.RecordTemplateInput(template.FullFileName, saveFileDialog.FileName, _executionCallback);
                    }
                    break;
            }
            return true;
        }

        private void ExecutionCallback(ZeusProcessStatusEventArgs args)
        {
            if (args.Message != null)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(_executionCallback, args);
                }
                else
                {
                    if (_consoleWriteGeneratedDetails)
                    {
                        if (this._mdi.Console.DockContent.IsHidden) this._mdi.Console.DockContent.Show(_mdi.DockPanel);
                        if (!this._mdi.Console.DockContent.IsActivated) this._mdi.Console.DockContent.Activate();
                    }

                    if (args.Message.StartsWith(ZeusProcessManagerTags.GENERATED_FILE_TAG))
                    {
                        string generatedFile = args.Message.Substring(ZeusProcessManagerTags.GENERATED_FILE_TAG.Length);
                        this._mdi.WriteConsole("File Generated: " + generatedFile);
                        this._mdi.SendAlert(this, "FileGenerated", generatedFile);
                    }
                    else
                    {
                        if (_consoleWriteGeneratedDetails) this._mdi.WriteConsole(args.Message);
                    }

                    if (!args.IsRunning)
                    {
                        this._mdi.PerformMdiFunction(this, "ExecutionQueueUpdate");
                    }
                }
            }
        }

        private void templateBrowserControl_ExecutionStatusUpdate(bool isRunning, string message)
        {
            if (_consoleWriteGeneratedDetails)
            {
                if (this._mdi.Console.DockContent.IsHidden) this._mdi.Console.DockContent.Show(_mdi.DockPanel);
                if (!this._mdi.Console.DockContent.IsActivated) this._mdi.Console.DockContent.Activate();
            }

            if (message.StartsWith(ZeusProcessManagerTags.GENERATED_FILE_TAG))
            {
                string generatedFile = message.Substring(ZeusProcessManagerTags.GENERATED_FILE_TAG.Length);
                this._mdi.WriteConsole("File Generated: " + generatedFile);
                this._mdi.SendAlert(this, "FileGenerated", generatedFile);
            }
            else
            {
                if (_consoleWriteGeneratedDetails) this._mdi.WriteConsole(message);
            }
        }

        private void templateBrowserControl_ErrorsOccurred(object sender, EventArgs e)
        {
            if (sender is Exception)
            {
                this._mdi.ErrorsOccurred(sender as Exception);
            }
        }

        private void templateBrowserControl_TemplateOpen(object sender, EventArgs e)
        {
            if (sender != null)
            {
                this._mdi.OpenDocuments(sender.ToString());
            }
        }

        private void templateBrowserControl_TemplateUpdate(object sender, EventArgs e)
        {
            if (sender != null)
            {
                this._mdi.SendAlert(this, "UpdateTemplate", sender.ToString());
            }
        }

        private void templateBrowserControl_TemplateDelete(object sender, EventArgs e)
        {
            if (sender != null)
            {
                this._mdi.SendAlert(this, "DeleteTemplate", sender.ToString());
            }
        }

        #region IMyGenContent Members

        public ToolStrip ToolStrip
        {
            get { return null; }
        }

        public void ProcessAlert(IMyGenContent sender, string command, params object[] args)
        {
            if (!command.Equals("UpdateDefaultSettings", StringComparison.CurrentCultureIgnoreCase)) return;
            
            _consoleWriteGeneratedDetails = DefaultSettings.Instance.MiscSettings.ConsoleWriteGeneratedDetails;
            var doRefresh = false;

            if (DefaultSettings.Instance.TemplateSettings.ExecuteFromTemplateBrowserAsync)
                templateBrowserControl.ExecuteTemplateOverride = ExecuteTemplateOverride;
            else
                templateBrowserControl.ExecuteTemplateOverride = null;

            try
            {
                if (templateBrowserControl.TreeBuilder.DefaultTemplatePath != DefaultSettings.Instance.TemplateSettings.DefaultTemplateDirectory)
                    doRefresh = true;
            }
            catch
            {
                doRefresh = true;
            }

            if (doRefresh)
                templateBrowserControl.RefreshTree();
        }

        public bool CanClose(bool allowPrevent)
        {
            return true;
        }

        public DockContent DockContent
        {
            get { return this; }
        }

        #endregion
    }
}