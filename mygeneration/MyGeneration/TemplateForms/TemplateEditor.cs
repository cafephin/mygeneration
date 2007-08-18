using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Reflection;

using Zeus;
using Zeus.ErrorHandling;
using Zeus.UserInterface;
using Zeus.UserInterface.WinForms;

using MyMeta;
using WeifenLuo.WinFormsUI.Docking;
using Scintilla;

namespace MyGeneration
{
	/// <summary>
	/// Summary description for TemplateProperties.
	/// </summary>
    public class TemplateEditor : DockContent, IScintillaEditControl, ILogger
	{
		public const string FILE_TYPES = "JScript Templates (*.jgen)|*.jgen|VBScript Templates (*.vbgen)|*.vbgen|C# Templates (*.csgen)|*.csgen|Zeus Templates (*.zeus)|*.zeus|All files (*.*)|*.*";
		public const int DEFAULT_OPEN_FILE_TYPE_INDEX = 5;
		public const int DEFAULT_SAVE_FILE_TYPE_INDEX = 4;


        private IMyGenerationMDI mdi; 
        private System.Windows.Forms.ToolBar toolBar1;
		
		private ZeusScintillaControl scintillaTemplateCode = null;
		private ZeusScintillaControl scintillaGUICode = null;
		private ZeusScintillaControl scintillaTemplateSource = null;
		private ZeusScintillaControl scintillaGuiSource = null;
		private ZeusScintillaControl scintillaOutput = null;
		
		private System.Windows.Forms.TabPage tabTemplateCode;
		private System.Windows.Forms.TabPage tabInterfaceCode;
		private System.Windows.Forms.TabPage tabTemplateSource;
		private System.Windows.Forms.TabPage tabOutput;

		private System.Windows.Forms.ToolBarButton toolBarButtonProperties;
		private System.Windows.Forms.ToolBarButton toolBarButtonConsole;
		private System.Windows.Forms.Panel panelConsole;
		private System.Windows.Forms.Panel panelProperties;
		private NJFLib.Controls.CollapsibleSplitter splitterProperties;
		private System.Windows.Forms.TabControl tabControlTemplate;
		private System.Windows.Forms.TextBox textBoxConsole;
		private System.Windows.Forms.Label labelComments;
		private System.Windows.Forms.TextBox textBoxComments;
		private System.Windows.Forms.GroupBox groupBoxScripting;
		private System.Windows.Forms.Label labelEndTag;
		private System.Windows.Forms.Label labelStartTag;
		private System.Windows.Forms.TextBox textBoxStartTag;
		private System.Windows.Forms.TextBox textBoxShortcutTag;
		private System.Windows.Forms.TextBox textBoxEndTag;
		private System.Windows.Forms.ComboBox comboBoxLanguage;
		private System.Windows.Forms.Label labelLanguage;
		private NJFLib.Controls.CollapsibleSplitter splitterConsole;

		private System.Windows.Forms.ToolBarButton toolBarButtonExecute;
		private System.Windows.Forms.ToolBarButton toolBarSeperator1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItemExecute;
		private System.Windows.Forms.MenuItem menuItemTemplate;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button buttonSelectFile;
		private System.Windows.Forms.Label labelUniqueID;
		private System.Windows.Forms.TextBox textBoxUniqueID;
		private System.Windows.Forms.ImageList imageListToolbar;
		private System.Windows.Forms.ToolBarButton toolBarButtonSave;
		private System.Windows.Forms.ToolBarButton toolBarButtonSeperator0;
		private OpenFileDialog openFileDialog = new OpenFileDialog();
		private System.Windows.Forms.ToolBarButton toolBarButtonSaveAs;

		private System.Windows.Forms.MenuItem menuItemFile;
		private System.Windows.Forms.MenuItem menuItemSave;
		private System.Windows.Forms.MenuItem menuItemClose;
		private System.Windows.Forms.Label labelShortcutTag;
		private System.Windows.Forms.Label labelMode;
		private System.Windows.Forms.ComboBox comboBoxMode;
		private System.Windows.Forms.MenuItem menuItemLineNumbers;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemClipboard;
		private System.Windows.Forms.ListBox listBoxIncludedTemplateFiles;
		private System.Windows.Forms.MenuItem menuItemWhitespace;
		private System.Windows.Forms.MenuItem menuItemIndentation;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItemEndOfLine;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;

		private System.Windows.Forms.ComboBox comboBoxOutputLanguage;
		private System.Windows.Forms.Label labelOutputLanguage;
		private System.Windows.Forms.Label labelGroup;
		private System.Windows.Forms.TextBox textBoxGroup;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.TextBox textBoxTitle;
		private System.Windows.Forms.MenuItem menuItemZoomIn;
		private System.Windows.Forms.MenuItem menuItemZoomOut;
		private System.Windows.Forms.ComboBox comboBoxEngine;
		private System.Windows.Forms.Label labelEngine;
		private System.Windows.Forms.MenuItem menuItem7;

		protected bool _isDirty = false;
		private System.Windows.Forms.ComboBox comboBoxGuiEngine;
		private System.Windows.Forms.Label labelGuiEngine;
		private System.Windows.Forms.ComboBox comboBoxGuiLanguage;
		private System.Windows.Forms.Label labelGuiLanguage;
		private System.Windows.Forms.ComboBox comboBoxType;
		private System.Windows.Forms.Label labelType;
		private System.Windows.Forms.Label labelIncludedTemplates;
		private System.Windows.Forms.TabPage tabInterfaceSource;
		private System.Windows.Forms.Button buttonNewGuid;
		private System.Windows.Forms.ToolBarButton toolBarSaveTemplateInput;
		private System.Windows.Forms.MenuItem menuItemEdit;
		private System.Windows.Forms.MenuItem menuItemFind;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItemReplace;
		private System.Windows.Forms.MenuItem menuItemReplaceHidden;
		private System.Windows.Forms.MenuItem menuItemSaveAs;
		private System.Windows.Forms.MenuItem menuItemEncryptAs;
		private System.Windows.Forms.MenuItem menuItemCompileAs;
        private System.Windows.Forms.MenuItem menuItem5;
        private MenuItem menuItemFindNext;
		private ZeusTemplate _template;

		public TemplateEditor(IMyGenerationMDI mdi)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            this.mdi = mdi;
			this.panelProperties.VisibleChanged += new EventHandler(panel_ToggleVisibility);
			this.panelConsole.VisibleChanged += new EventHandler(panel_ToggleVisibility);

            
            // Create Scintilla Code controls for editing the template
            scintillaTemplateCode = new ZeusScintillaControl();
            //scintillaTemplateCode.AddShortcutsFromForm(this);
            scintillaTemplateCode.AddShortcuts(this);
            scintillaTemplateCode.InitializeFindReplace();

			scintillaGUICode = new ZeusScintillaControl();
            scintillaGUICode.AddShortcuts(this);

			scintillaTemplateSource = new ZeusScintillaControl();
            scintillaTemplateSource.AddShortcuts(this);

			scintillaGuiSource = new ZeusScintillaControl();
            scintillaGuiSource.AddShortcuts(this);

			scintillaOutput = new ZeusScintillaControl();
            scintillaOutput.AddShortcuts(this);

            DefaultSettings settings = DefaultSettings.Instance;
            this.SetCodePageOverride(settings.CodePage);
            this.SetFontOverride(settings.FontFamily);

            this.tabTemplateCode.Controls.Add(this.scintillaTemplateCode);
			this.tabInterfaceCode.Controls.Add(this.scintillaGUICode);
			this.tabTemplateSource.Controls.Add(this.scintillaTemplateSource);
			this.tabInterfaceSource.Controls.Add(this.scintillaGuiSource);
			this.tabOutput.Controls.Add(this.scintillaOutput);

			menuItemLineNumbers.Checked = false;
			if (settings.EnableLineNumbering) 
			{
				menuItemLineNumbers_Click(null, new EventArgs());
			}

			menuItemClipboard.Checked = false;
			if (settings.EnableClipboard) 
			{
				menuItemClipboard_Click(null, new EventArgs());
			}

			//LoseFocus IsDirty Handlers
			this.textBoxComments.Leave += new EventHandler(this.CheckPropertiesDirty);

			// Keep the Status bar up to date
			this.scintillaTemplateCode.UpdateUI += new EventHandler<UpdateUIEventArgs>(UpdateUI);
            this.scintillaTemplateSource.UpdateUI += new EventHandler<UpdateUIEventArgs>(UpdateUI);
            this.scintillaGUICode.UpdateUI += new EventHandler<UpdateUIEventArgs>(UpdateUI);
            this.scintillaGuiSource.UpdateUI += new EventHandler<UpdateUIEventArgs>(UpdateUI);
            this.scintillaOutput.UpdateUI += new EventHandler<UpdateUIEventArgs>(UpdateUI);

			//this.KeyDown += new KeyEventHandler(TemplateEditor_KeyDown);
		}

		/*private void TemplateEditor_KeyDown(object sender, KeyEventArgs args) 
		{
			if (args.KeyCode == System.Windows.Forms.Keys.F3) 
			{
                ZeusScintillaControl.FindDialog.FindNext();
			}
		}*/

		protected override string GetPersistString()
		{
			return GetType().ToString() + "," + this.FileName;
		}

		public IEditControl CurrentEditControl 
		{
			get { return CurrentScintilla; }
		}

		protected ZeusScintillaControl CurrentScintilla 
		{
			get 
			{
				switch (this.tabControlTemplate.SelectedIndex)
				{
					case 1:
						return this.scintillaGUICode;
					case 2:
						return this.scintillaTemplateSource;
					case 3:
						return this.scintillaGuiSource;
					case 4:
						return this.scintillaOutput;
					default:
						return this.scintillaTemplateCode;
				}
			}
		}

		private void UpdateUI(object sender, UpdateUIEventArgs args)
		{
            ScintillaControl scintilla = sender as ScintillaControl;

            int caretPos = scintilla.CurrentPos;
			int line = scintilla.LineFromPosition(caretPos);
			///MDIParent.TheParent.statusRow.Text = "Line: "   + (scintilla.LineFromPosition(caretPos) + 1).ToString(); 
			//MDIParent.TheParent.statusCol.Text = "Column: " + (scintilla.Column(caretPos) + 1).ToString(); 
		}

		protected void CheckPropertiesDirty(object sender, EventArgs args) 
		{
			if (!this._isDirty) 
			{
				if ((this._template.UniqueID != this.textBoxUniqueID.Text) ||
					(this._template.Title != this.textBoxTitle.Text) ||
					(this._template.Comments != this.textBoxComments.Text) ||
					(this._template.TagEnd != this.textBoxEndTag.Text))
				{
					this._isDirty = true;
				}
			}
        }

        private void SetCodePageOverride(int codePage)
        {
            this.scintillaGUICode.CodePageOverride = codePage;
            this.scintillaGuiSource.CodePageOverride = codePage;
            this.scintillaOutput.CodePageOverride = codePage;
            this.scintillaTemplateCode.CodePageOverride = codePage;
            this.scintillaTemplateSource.CodePageOverride = codePage;
        }

        private void SetFontOverride(string family)
        {
            this.scintillaGUICode.FontFamilyOverride = family;
            this.scintillaGuiSource.FontFamilyOverride = family;
            this.scintillaOutput.FontFamilyOverride = family;
            this.scintillaTemplateCode.FontFamilyOverride = family;
            this.scintillaTemplateSource.FontFamilyOverride = family;
        }

		/*public void DefaultSettingsChanged(DefaultSettings settings)
		{
            SetCodePageOverride(settings.CodePage);
            SetFontOverride(settings.FontFamily);

            this.scintillaTemplateCode.TabWidth = settings.Tabs;
			this.menuItemClipboard.Checked = settings.EnableClipboard;
		}*/

		public bool CanClose(bool allowPrevent)
		{
			return PromptForSave(allowPrevent);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateEditor));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButtonSave = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonSaveAs = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonSeperator0 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonProperties = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonConsole = new System.Windows.Forms.ToolBarButton();
            this.toolBarSeperator1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarSaveTemplateInput = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonExecute = new System.Windows.Forms.ToolBarButton();
            this.imageListToolbar = new System.Windows.Forms.ImageList(this.components);
            this.panelConsole = new System.Windows.Forms.Panel();
            this.textBoxConsole = new System.Windows.Forms.TextBox();
            this.splitterConsole = new NJFLib.Controls.CollapsibleSplitter();
            this.panelProperties = new System.Windows.Forms.Panel();
            this.buttonNewGuid = new System.Windows.Forms.Button();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.labelType = new System.Windows.Forms.Label();
            this.comboBoxGuiEngine = new System.Windows.Forms.ComboBox();
            this.labelGuiEngine = new System.Windows.Forms.Label();
            this.comboBoxGuiLanguage = new System.Windows.Forms.ComboBox();
            this.labelGuiLanguage = new System.Windows.Forms.Label();
            this.comboBoxEngine = new System.Windows.Forms.ComboBox();
            this.labelEngine = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.comboBoxOutputLanguage = new System.Windows.Forms.ComboBox();
            this.labelOutputLanguage = new System.Windows.Forms.Label();
            this.listBoxIncludedTemplateFiles = new System.Windows.Forms.ListBox();
            this.labelUniqueID = new System.Windows.Forms.Label();
            this.textBoxUniqueID = new System.Windows.Forms.TextBox();
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.labelIncludedTemplates = new System.Windows.Forms.Label();
            this.groupBoxScripting = new System.Windows.Forms.GroupBox();
            this.labelEndTag = new System.Windows.Forms.Label();
            this.labelShortcutTag = new System.Windows.Forms.Label();
            this.labelStartTag = new System.Windows.Forms.Label();
            this.textBoxStartTag = new System.Windows.Forms.TextBox();
            this.textBoxShortcutTag = new System.Windows.Forms.TextBox();
            this.textBoxEndTag = new System.Windows.Forms.TextBox();
            this.labelComments = new System.Windows.Forms.Label();
            this.textBoxComments = new System.Windows.Forms.TextBox();
            this.labelGroup = new System.Windows.Forms.Label();
            this.textBoxGroup = new System.Windows.Forms.TextBox();
            this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
            this.comboBoxMode = new System.Windows.Forms.ComboBox();
            this.labelLanguage = new System.Windows.Forms.Label();
            this.labelMode = new System.Windows.Forms.Label();
            this.splitterProperties = new NJFLib.Controls.CollapsibleSplitter();
            this.tabControlTemplate = new System.Windows.Forms.TabControl();
            this.tabTemplateCode = new System.Windows.Forms.TabPage();
            this.tabInterfaceCode = new System.Windows.Forms.TabPage();
            this.tabTemplateSource = new System.Windows.Forms.TabPage();
            this.tabInterfaceSource = new System.Windows.Forms.TabPage();
            this.tabOutput = new System.Windows.Forms.TabPage();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItemFile = new System.Windows.Forms.MenuItem();
            this.menuItemSave = new System.Windows.Forms.MenuItem();
            this.menuItemSaveAs = new System.Windows.Forms.MenuItem();
            this.menuItemClose = new System.Windows.Forms.MenuItem();
            this.menuItemEdit = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItemFind = new System.Windows.Forms.MenuItem();
            this.menuItemReplace = new System.Windows.Forms.MenuItem();
            this.menuItemReplaceHidden = new System.Windows.Forms.MenuItem();
            this.menuItemFindNext = new System.Windows.Forms.MenuItem();
            this.menuItemTemplate = new System.Windows.Forms.MenuItem();
            this.menuItemExecute = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemEncryptAs = new System.Windows.Forms.MenuItem();
            this.menuItemCompileAs = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItemLineNumbers = new System.Windows.Forms.MenuItem();
            this.menuItemWhitespace = new System.Windows.Forms.MenuItem();
            this.menuItemIndentation = new System.Windows.Forms.MenuItem();
            this.menuItemEndOfLine = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItemZoomIn = new System.Windows.Forms.MenuItem();
            this.menuItemZoomOut = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItemClipboard = new System.Windows.Forms.MenuItem();
            this.panelConsole.SuspendLayout();
            this.panelProperties.SuspendLayout();
            this.groupBoxScripting.SuspendLayout();
            this.tabControlTemplate.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBar1
            // 
            this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBar1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButtonSave,
            this.toolBarButtonSaveAs,
            this.toolBarButtonSeperator0,
            this.toolBarButtonProperties,
            this.toolBarButtonConsole,
            this.toolBarSeperator1,
            this.toolBarSaveTemplateInput,
            this.toolBarButtonExecute});
            this.toolBar1.ButtonSize = new System.Drawing.Size(26, 22);
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.ImageList = this.imageListToolbar;
            this.toolBar1.Location = new System.Drawing.Point(0, 0);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(976, 29);
            this.toolBar1.TabIndex = 0;
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // toolBarButtonSave
            // 
            this.toolBarButtonSave.ImageIndex = 3;
            this.toolBarButtonSave.Name = "toolBarButtonSave";
            this.toolBarButtonSave.Tag = "1";
            // 
            // toolBarButtonSaveAs
            // 
            this.toolBarButtonSaveAs.ImageIndex = 4;
            this.toolBarButtonSaveAs.Name = "toolBarButtonSaveAs";
            this.toolBarButtonSaveAs.Tag = "2";
            // 
            // toolBarButtonSeperator0
            // 
            this.toolBarButtonSeperator0.Name = "toolBarButtonSeperator0";
            this.toolBarButtonSeperator0.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            this.toolBarButtonSeperator0.Tag = "-1";
            // 
            // toolBarButtonProperties
            // 
            this.toolBarButtonProperties.ImageIndex = 2;
            this.toolBarButtonProperties.Name = "toolBarButtonProperties";
            this.toolBarButtonProperties.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.toolBarButtonProperties.Tag = "3";
            // 
            // toolBarButtonConsole
            // 
            this.toolBarButtonConsole.ImageIndex = 0;
            this.toolBarButtonConsole.Name = "toolBarButtonConsole";
            this.toolBarButtonConsole.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.toolBarButtonConsole.Tag = "4";
            // 
            // toolBarSeperator1
            // 
            this.toolBarSeperator1.Name = "toolBarSeperator1";
            this.toolBarSeperator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            this.toolBarSeperator1.Tag = "01";
            // 
            // toolBarSaveTemplateInput
            // 
            this.toolBarSaveTemplateInput.ImageIndex = 7;
            this.toolBarSaveTemplateInput.Name = "toolBarSaveTemplateInput";
            this.toolBarSaveTemplateInput.Visible = false;
            // 
            // toolBarButtonExecute
            // 
            this.toolBarButtonExecute.ImageIndex = 1;
            this.toolBarButtonExecute.Name = "toolBarButtonExecute";
            this.toolBarButtonExecute.Tag = "5";
            // 
            // imageListToolbar
            // 
            this.imageListToolbar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListToolbar.ImageStream")));
            this.imageListToolbar.TransparentColor = System.Drawing.Color.Fuchsia;
            this.imageListToolbar.Images.SetKeyName(0, "");
            this.imageListToolbar.Images.SetKeyName(1, "");
            this.imageListToolbar.Images.SetKeyName(2, "");
            this.imageListToolbar.Images.SetKeyName(3, "");
            this.imageListToolbar.Images.SetKeyName(4, "");
            this.imageListToolbar.Images.SetKeyName(5, "");
            this.imageListToolbar.Images.SetKeyName(6, "");
            this.imageListToolbar.Images.SetKeyName(7, "");
            // 
            // panelConsole
            // 
            this.panelConsole.Controls.Add(this.textBoxConsole);
            this.panelConsole.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelConsole.Location = new System.Drawing.Point(0, 601);
            this.panelConsole.Name = "panelConsole";
            this.panelConsole.Size = new System.Drawing.Size(976, 88);
            this.panelConsole.TabIndex = 0;
            this.panelConsole.Visible = false;
            // 
            // textBoxConsole
            // 
            this.textBoxConsole.BackColor = System.Drawing.Color.Black;
            this.textBoxConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxConsole.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxConsole.ForeColor = System.Drawing.Color.Lime;
            this.textBoxConsole.Location = new System.Drawing.Point(0, 0);
            this.textBoxConsole.MaxLength = 9999999;
            this.textBoxConsole.Multiline = true;
            this.textBoxConsole.Name = "textBoxConsole";
            this.textBoxConsole.ReadOnly = true;
            this.textBoxConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxConsole.Size = new System.Drawing.Size(976, 88);
            this.textBoxConsole.TabIndex = 0;
            this.textBoxConsole.WordWrap = false;
            // 
            // splitterConsole
            // 
            this.splitterConsole.AnimationDelay = 20;
            this.splitterConsole.AnimationStep = 20;
            this.splitterConsole.BorderStyle3D = System.Windows.Forms.Border3DStyle.Flat;
            this.splitterConsole.ControlToHide = this.panelConsole;
            this.splitterConsole.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterConsole.ExpandParentForm = false;
            this.splitterConsole.Location = new System.Drawing.Point(0, 598);
            this.splitterConsole.Name = "splitterConsole";
            this.splitterConsole.TabIndex = 5;
            this.splitterConsole.TabStop = false;
            this.splitterConsole.UseAnimations = false;
            this.splitterConsole.VisualStyle = NJFLib.Controls.VisualStyles.Mozilla;
            // 
            // panelProperties
            // 
            this.panelProperties.AutoScroll = true;
            this.panelProperties.Controls.Add(this.buttonNewGuid);
            this.panelProperties.Controls.Add(this.comboBoxType);
            this.panelProperties.Controls.Add(this.labelType);
            this.panelProperties.Controls.Add(this.comboBoxGuiEngine);
            this.panelProperties.Controls.Add(this.labelGuiEngine);
            this.panelProperties.Controls.Add(this.comboBoxGuiLanguage);
            this.panelProperties.Controls.Add(this.labelGuiLanguage);
            this.panelProperties.Controls.Add(this.comboBoxEngine);
            this.panelProperties.Controls.Add(this.labelEngine);
            this.panelProperties.Controls.Add(this.labelTitle);
            this.panelProperties.Controls.Add(this.textBoxTitle);
            this.panelProperties.Controls.Add(this.comboBoxOutputLanguage);
            this.panelProperties.Controls.Add(this.labelOutputLanguage);
            this.panelProperties.Controls.Add(this.listBoxIncludedTemplateFiles);
            this.panelProperties.Controls.Add(this.labelUniqueID);
            this.panelProperties.Controls.Add(this.textBoxUniqueID);
            this.panelProperties.Controls.Add(this.buttonSelectFile);
            this.panelProperties.Controls.Add(this.labelIncludedTemplates);
            this.panelProperties.Controls.Add(this.groupBoxScripting);
            this.panelProperties.Controls.Add(this.labelComments);
            this.panelProperties.Controls.Add(this.textBoxComments);
            this.panelProperties.Controls.Add(this.labelGroup);
            this.panelProperties.Controls.Add(this.textBoxGroup);
            this.panelProperties.Controls.Add(this.comboBoxLanguage);
            this.panelProperties.Controls.Add(this.comboBoxMode);
            this.panelProperties.Controls.Add(this.labelLanguage);
            this.panelProperties.Controls.Add(this.labelMode);
            this.panelProperties.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelProperties.Location = new System.Drawing.Point(0, 29);
            this.panelProperties.Name = "panelProperties";
            this.panelProperties.Size = new System.Drawing.Size(304, 569);
            this.panelProperties.TabIndex = 3;
            // 
            // buttonNewGuid
            // 
            this.buttonNewGuid.ImageList = this.imageListToolbar;
            this.buttonNewGuid.Location = new System.Drawing.Point(216, 24);
            this.buttonNewGuid.Name = "buttonNewGuid";
            this.buttonNewGuid.Size = new System.Drawing.Size(64, 23);
            this.buttonNewGuid.TabIndex = 6;
            this.buttonNewGuid.Text = "New Guid";
            this.buttonNewGuid.Click += new System.EventHandler(this.buttonNewGuid_Click);
            // 
            // comboBoxType
            // 
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.Location = new System.Drawing.Point(8, 144);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(128, 21);
            this.comboBoxType.TabIndex = 15;
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.comboBoxType_SelectedIndexChanged);
            // 
            // labelType
            // 
            this.labelType.Location = new System.Drawing.Point(8, 128);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(112, 16);
            this.labelType.TabIndex = 36;
            this.labelType.Text = "Type:";
            this.labelType.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // comboBoxGuiEngine
            // 
            this.comboBoxGuiEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGuiEngine.Location = new System.Drawing.Point(152, 184);
            this.comboBoxGuiEngine.Name = "comboBoxGuiEngine";
            this.comboBoxGuiEngine.Size = new System.Drawing.Size(128, 21);
            this.comboBoxGuiEngine.TabIndex = 27;
            this.comboBoxGuiEngine.SelectedIndexChanged += new System.EventHandler(this.comboBoxGuiEngine_SelectedIndexChanged);
            // 
            // labelGuiEngine
            // 
            this.labelGuiEngine.Location = new System.Drawing.Point(152, 168);
            this.labelGuiEngine.Name = "labelGuiEngine";
            this.labelGuiEngine.Size = new System.Drawing.Size(128, 16);
            this.labelGuiEngine.TabIndex = 34;
            this.labelGuiEngine.Text = "Gui Scripting Engine:";
            this.labelGuiEngine.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // comboBoxGuiLanguage
            // 
            this.comboBoxGuiLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGuiLanguage.Location = new System.Drawing.Point(152, 224);
            this.comboBoxGuiLanguage.Name = "comboBoxGuiLanguage";
            this.comboBoxGuiLanguage.Size = new System.Drawing.Size(128, 21);
            this.comboBoxGuiLanguage.TabIndex = 30;
            this.comboBoxGuiLanguage.SelectedIndexChanged += new System.EventHandler(this.comboBoxGuiLanguage_SelectedIndexChanged);
            // 
            // labelGuiLanguage
            // 
            this.labelGuiLanguage.Location = new System.Drawing.Point(152, 208);
            this.labelGuiLanguage.Name = "labelGuiLanguage";
            this.labelGuiLanguage.Size = new System.Drawing.Size(128, 16);
            this.labelGuiLanguage.TabIndex = 32;
            this.labelGuiLanguage.Text = "Gui Language:";
            this.labelGuiLanguage.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // comboBoxEngine
            // 
            this.comboBoxEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEngine.Location = new System.Drawing.Point(8, 184);
            this.comboBoxEngine.Name = "comboBoxEngine";
            this.comboBoxEngine.Size = new System.Drawing.Size(128, 21);
            this.comboBoxEngine.TabIndex = 18;
            this.comboBoxEngine.SelectedIndexChanged += new System.EventHandler(this.comboBoxEngine_SelectedIndexChanged);
            // 
            // labelEngine
            // 
            this.labelEngine.Location = new System.Drawing.Point(8, 168);
            this.labelEngine.Name = "labelEngine";
            this.labelEngine.Size = new System.Drawing.Size(144, 16);
            this.labelEngine.TabIndex = 30;
            this.labelEngine.Text = "Template Scripting Engine:";
            this.labelEngine.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelTitle
            // 
            this.labelTitle.Location = new System.Drawing.Point(8, 48);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(208, 16);
            this.labelTitle.TabIndex = 29;
            this.labelTitle.Text = "Title:";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Location = new System.Drawing.Point(8, 64);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(272, 20);
            this.textBoxTitle.TabIndex = 9;
            // 
            // comboBoxOutputLanguage
            // 
            this.comboBoxOutputLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOutputLanguage.Location = new System.Drawing.Point(152, 272);
            this.comboBoxOutputLanguage.Name = "comboBoxOutputLanguage";
            this.comboBoxOutputLanguage.Size = new System.Drawing.Size(128, 21);
            this.comboBoxOutputLanguage.Sorted = true;
            this.comboBoxOutputLanguage.TabIndex = 39;
            this.comboBoxOutputLanguage.SelectedIndexChanged += new System.EventHandler(this.comboBoxOutputLanguage_SelectedIndexChanged);
            // 
            // labelOutputLanguage
            // 
            this.labelOutputLanguage.Location = new System.Drawing.Point(152, 256);
            this.labelOutputLanguage.Name = "labelOutputLanguage";
            this.labelOutputLanguage.Size = new System.Drawing.Size(112, 16);
            this.labelOutputLanguage.TabIndex = 26;
            this.labelOutputLanguage.Text = "Output Language:";
            this.labelOutputLanguage.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // listBoxIncludedTemplateFiles
            // 
            this.listBoxIncludedTemplateFiles.Location = new System.Drawing.Point(8, 472);
            this.listBoxIncludedTemplateFiles.Name = "listBoxIncludedTemplateFiles";
            this.listBoxIncludedTemplateFiles.Size = new System.Drawing.Size(248, 82);
            this.listBoxIncludedTemplateFiles.TabIndex = 45;
            this.listBoxIncludedTemplateFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxIncludedTemplateFiles_KeyDown);
            // 
            // labelUniqueID
            // 
            this.labelUniqueID.Location = new System.Drawing.Point(8, 8);
            this.labelUniqueID.Name = "labelUniqueID";
            this.labelUniqueID.Size = new System.Drawing.Size(208, 16);
            this.labelUniqueID.TabIndex = 21;
            this.labelUniqueID.Text = "Unique ID:";
            this.labelUniqueID.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // textBoxUniqueID
            // 
            this.textBoxUniqueID.Location = new System.Drawing.Point(8, 24);
            this.textBoxUniqueID.Name = "textBoxUniqueID";
            this.textBoxUniqueID.Size = new System.Drawing.Size(208, 20);
            this.textBoxUniqueID.TabIndex = 5;
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.Location = new System.Drawing.Point(256, 472);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(24, 24);
            this.buttonSelectFile.TabIndex = 48;
            this.buttonSelectFile.Text = "...";
            this.buttonSelectFile.Click += new System.EventHandler(this.buttonSelectFile_Click);
            // 
            // labelIncludedTemplates
            // 
            this.labelIncludedTemplates.Location = new System.Drawing.Point(8, 448);
            this.labelIncludedTemplates.Name = "labelIncludedTemplates";
            this.labelIncludedTemplates.Size = new System.Drawing.Size(208, 16);
            this.labelIncludedTemplates.TabIndex = 14;
            this.labelIncludedTemplates.Text = "Included Template Scripts:";
            this.labelIncludedTemplates.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // groupBoxScripting
            // 
            this.groupBoxScripting.Controls.Add(this.labelEndTag);
            this.groupBoxScripting.Controls.Add(this.labelShortcutTag);
            this.groupBoxScripting.Controls.Add(this.labelStartTag);
            this.groupBoxScripting.Controls.Add(this.textBoxStartTag);
            this.groupBoxScripting.Controls.Add(this.textBoxShortcutTag);
            this.groupBoxScripting.Controls.Add(this.textBoxEndTag);
            this.groupBoxScripting.Location = new System.Drawing.Point(8, 256);
            this.groupBoxScripting.Name = "groupBoxScripting";
            this.groupBoxScripting.Size = new System.Drawing.Size(128, 96);
            this.groupBoxScripting.TabIndex = 32;
            this.groupBoxScripting.TabStop = false;
            this.groupBoxScripting.Text = "Dynamic Tags";
            // 
            // labelEndTag
            // 
            this.labelEndTag.Location = new System.Drawing.Point(8, 40);
            this.labelEndTag.Name = "labelEndTag";
            this.labelEndTag.Size = new System.Drawing.Size(56, 23);
            this.labelEndTag.TabIndex = 19;
            this.labelEndTag.Text = "End:";
            this.labelEndTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelShortcutTag
            // 
            this.labelShortcutTag.Location = new System.Drawing.Point(8, 64);
            this.labelShortcutTag.Name = "labelShortcutTag";
            this.labelShortcutTag.Size = new System.Drawing.Size(56, 23);
            this.labelShortcutTag.TabIndex = 18;
            this.labelShortcutTag.Text = "Shortcut:";
            this.labelShortcutTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelStartTag
            // 
            this.labelStartTag.Location = new System.Drawing.Point(8, 16);
            this.labelStartTag.Name = "labelStartTag";
            this.labelStartTag.Size = new System.Drawing.Size(56, 23);
            this.labelStartTag.TabIndex = 17;
            this.labelStartTag.Text = "Start:";
            this.labelStartTag.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxStartTag
            // 
            this.textBoxStartTag.Location = new System.Drawing.Point(64, 16);
            this.textBoxStartTag.Name = "textBoxStartTag";
            this.textBoxStartTag.Size = new System.Drawing.Size(40, 20);
            this.textBoxStartTag.TabIndex = 3;
            this.textBoxStartTag.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxStartTag_KeyUp);
            // 
            // textBoxShortcutTag
            // 
            this.textBoxShortcutTag.Location = new System.Drawing.Point(64, 64);
            this.textBoxShortcutTag.Name = "textBoxShortcutTag";
            this.textBoxShortcutTag.ReadOnly = true;
            this.textBoxShortcutTag.Size = new System.Drawing.Size(40, 20);
            this.textBoxShortcutTag.TabIndex = 9;
            // 
            // textBoxEndTag
            // 
            this.textBoxEndTag.Location = new System.Drawing.Point(64, 40);
            this.textBoxEndTag.Name = "textBoxEndTag";
            this.textBoxEndTag.Size = new System.Drawing.Size(40, 20);
            this.textBoxEndTag.TabIndex = 6;
            // 
            // labelComments
            // 
            this.labelComments.Location = new System.Drawing.Point(8, 352);
            this.labelComments.Name = "labelComments";
            this.labelComments.Size = new System.Drawing.Size(208, 16);
            this.labelComments.TabIndex = 5;
            this.labelComments.Text = "Comments:";
            this.labelComments.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // textBoxComments
            // 
            this.textBoxComments.Location = new System.Drawing.Point(8, 368);
            this.textBoxComments.Multiline = true;
            this.textBoxComments.Name = "textBoxComments";
            this.textBoxComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxComments.Size = new System.Drawing.Size(272, 72);
            this.textBoxComments.TabIndex = 42;
            // 
            // labelGroup
            // 
            this.labelGroup.Location = new System.Drawing.Point(8, 88);
            this.labelGroup.Name = "labelGroup";
            this.labelGroup.Size = new System.Drawing.Size(208, 16);
            this.labelGroup.TabIndex = 1;
            this.labelGroup.Text = "Namespace:";
            this.labelGroup.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // textBoxGroup
            // 
            this.textBoxGroup.Location = new System.Drawing.Point(8, 104);
            this.textBoxGroup.Name = "textBoxGroup";
            this.textBoxGroup.Size = new System.Drawing.Size(272, 20);
            this.textBoxGroup.TabIndex = 12;
            // 
            // comboBoxLanguage
            // 
            this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLanguage.Location = new System.Drawing.Point(8, 224);
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.Size = new System.Drawing.Size(128, 21);
            this.comboBoxLanguage.TabIndex = 21;
            this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.comboBoxLanguage_SelectedIndexChanged);
            // 
            // comboBoxMode
            // 
            this.comboBoxMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMode.Location = new System.Drawing.Point(152, 144);
            this.comboBoxMode.Name = "comboBoxMode";
            this.comboBoxMode.Size = new System.Drawing.Size(128, 21);
            this.comboBoxMode.TabIndex = 24;
            this.comboBoxMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxMode_SelectedIndexChanged);
            // 
            // labelLanguage
            // 
            this.labelLanguage.Location = new System.Drawing.Point(8, 208);
            this.labelLanguage.Name = "labelLanguage";
            this.labelLanguage.Size = new System.Drawing.Size(128, 16);
            this.labelLanguage.TabIndex = 12;
            this.labelLanguage.Text = "Template Language:";
            this.labelLanguage.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelMode
            // 
            this.labelMode.Location = new System.Drawing.Point(152, 128);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(112, 16);
            this.labelMode.TabIndex = 22;
            this.labelMode.Text = "Mode:";
            this.labelMode.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // splitterProperties
            // 
            this.splitterProperties.AnimationDelay = 20;
            this.splitterProperties.AnimationStep = 20;
            this.splitterProperties.BorderStyle3D = System.Windows.Forms.Border3DStyle.Flat;
            this.splitterProperties.ControlToHide = this.panelProperties;
            this.splitterProperties.ExpandParentForm = false;
            this.splitterProperties.Location = new System.Drawing.Point(304, 29);
            this.splitterProperties.Name = "splitterProperties";
            this.splitterProperties.TabIndex = 1;
            this.splitterProperties.TabStop = false;
            this.splitterProperties.UseAnimations = false;
            this.splitterProperties.VisualStyle = NJFLib.Controls.VisualStyles.Mozilla;
            // 
            // tabControlTemplate
            // 
            this.tabControlTemplate.Controls.Add(this.tabTemplateCode);
            this.tabControlTemplate.Controls.Add(this.tabInterfaceCode);
            this.tabControlTemplate.Controls.Add(this.tabTemplateSource);
            this.tabControlTemplate.Controls.Add(this.tabInterfaceSource);
            this.tabControlTemplate.Controls.Add(this.tabOutput);
            this.tabControlTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlTemplate.Location = new System.Drawing.Point(312, 29);
            this.tabControlTemplate.Name = "tabControlTemplate";
            this.tabControlTemplate.SelectedIndex = 0;
            this.tabControlTemplate.Size = new System.Drawing.Size(664, 569);
            this.tabControlTemplate.TabIndex = 2;
            this.tabControlTemplate.SelectedIndexChanged += new System.EventHandler(this.tabControlTemplate_SelectedIndexChanged);
            // 
            // tabTemplateCode
            // 
            this.tabTemplateCode.BackColor = System.Drawing.Color.Transparent;
            this.tabTemplateCode.Location = new System.Drawing.Point(4, 22);
            this.tabTemplateCode.Name = "tabTemplateCode";
            this.tabTemplateCode.Size = new System.Drawing.Size(656, 543);
            this.tabTemplateCode.TabIndex = 0;
            this.tabTemplateCode.Text = "Template Code";
            this.tabTemplateCode.UseVisualStyleBackColor = true;
            // 
            // tabInterfaceCode
            // 
            this.tabInterfaceCode.BackColor = System.Drawing.Color.Transparent;
            this.tabInterfaceCode.Location = new System.Drawing.Point(4, 22);
            this.tabInterfaceCode.Name = "tabInterfaceCode";
            this.tabInterfaceCode.Size = new System.Drawing.Size(656, 543);
            this.tabInterfaceCode.TabIndex = 1;
            this.tabInterfaceCode.Text = "Interface Code";
            this.tabInterfaceCode.UseVisualStyleBackColor = true;
            // 
            // tabTemplateSource
            // 
            this.tabTemplateSource.Location = new System.Drawing.Point(4, 22);
            this.tabTemplateSource.Name = "tabTemplateSource";
            this.tabTemplateSource.Size = new System.Drawing.Size(656, 543);
            this.tabTemplateSource.TabIndex = 2;
            this.tabTemplateSource.Text = "Template Source";
            this.tabTemplateSource.UseVisualStyleBackColor = true;
            // 
            // tabInterfaceSource
            // 
            this.tabInterfaceSource.Location = new System.Drawing.Point(4, 22);
            this.tabInterfaceSource.Name = "tabInterfaceSource";
            this.tabInterfaceSource.Size = new System.Drawing.Size(656, 543);
            this.tabInterfaceSource.TabIndex = 4;
            this.tabInterfaceSource.Text = "Interface Source";
            this.tabInterfaceSource.UseVisualStyleBackColor = true;
            // 
            // tabOutput
            // 
            this.tabOutput.Location = new System.Drawing.Point(4, 22);
            this.tabOutput.Name = "tabOutput";
            this.tabOutput.Size = new System.Drawing.Size(656, 543);
            this.tabOutput.TabIndex = 3;
            this.tabOutput.Text = "Output";
            this.tabOutput.UseVisualStyleBackColor = true;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFile,
            this.menuItemEdit,
            this.menuItemTemplate});
            // 
            // menuItemFile
            // 
            this.menuItemFile.Index = 0;
            this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSave,
            this.menuItemSaveAs,
            this.menuItemClose});
            this.menuItemFile.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            this.menuItemFile.Text = "&File";
            // 
            // menuItemSave
            // 
            this.menuItemSave.Index = 0;
            this.menuItemSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.menuItemSave.Text = "&Save";
            this.menuItemSave.Click += new System.EventHandler(this.menuItemSave_Click);
            // 
            // menuItemSaveAs
            // 
            this.menuItemSaveAs.Index = 1;
            this.menuItemSaveAs.Text = "Save &As ...";
            this.menuItemSaveAs.Click += new System.EventHandler(this.menuItemSaveAs_Click);
            // 
            // menuItemClose
            // 
            this.menuItemClose.Index = 2;
            this.menuItemClose.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
            this.menuItemClose.Text = "&Close";
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // menuItemEdit
            // 
            this.menuItemEdit.Index = 1;
            this.menuItemEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem6,
            this.menuItemFind,
            this.menuItemReplace,
            this.menuItemReplaceHidden,
            this.menuItemFindNext});
            this.menuItemEdit.MergeOrder = 1;
            this.menuItemEdit.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            this.menuItemEdit.Text = "&Edit";
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 0;
            this.menuItem6.MergeOrder = 1;
            this.menuItem6.Text = "-";
            // 
            // menuItemFind
            // 
            this.menuItemFind.Index = 1;
            this.menuItemFind.MergeOrder = 1;
            this.menuItemFind.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
            this.menuItemFind.Text = "&Find";
            this.menuItemFind.Click += new System.EventHandler(this.menuItemFind_Click);
            // 
            // menuItemReplace
            // 
            this.menuItemReplace.Index = 2;
            this.menuItemReplace.MergeOrder = 1;
            this.menuItemReplace.Shortcut = System.Windows.Forms.Shortcut.CtrlH;
            this.menuItemReplace.Text = "&Replace";
            this.menuItemReplace.Click += new System.EventHandler(this.menuItemReplace_Click);
            // 
            // menuItemReplaceHidden
            // 
            this.menuItemReplaceHidden.Index = 3;
            this.menuItemReplaceHidden.MergeOrder = 1;
            this.menuItemReplaceHidden.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
            this.menuItemReplaceHidden.Text = "Replace&Hidden";
            this.menuItemReplaceHidden.Visible = false;
            this.menuItemReplaceHidden.Click += new System.EventHandler(this.menuItemReplace_Click);
            // 
            // menuItemFindNext
            // 
            this.menuItemFindNext.Index = 4;
            this.menuItemFindNext.MergeOrder = 1;
            this.menuItemFindNext.Shortcut = System.Windows.Forms.Shortcut.F3;
            this.menuItemFindNext.Text = "Find &Next";
            this.menuItemFindNext.Click += new System.EventHandler(this.menuItemFindNext_Click);
            // 
            // menuItemTemplate
            // 
            this.menuItemTemplate.Index = 2;
            this.menuItemTemplate.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemExecute,
            this.menuItem3,
            this.menuItemEncryptAs,
            this.menuItemCompileAs,
            this.menuItem5,
            this.menuItemLineNumbers,
            this.menuItemWhitespace,
            this.menuItemIndentation,
            this.menuItemEndOfLine,
            this.menuItem2,
            this.menuItem1,
            this.menuItem4,
            this.menuItemZoomIn,
            this.menuItemZoomOut,
            this.menuItem7,
            this.menuItemClipboard});
            this.menuItemTemplate.MergeOrder = 2;
            this.menuItemTemplate.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            this.menuItemTemplate.Text = "&Template";
            // 
            // menuItemExecute
            // 
            this.menuItemExecute.Index = 0;
            this.menuItemExecute.MergeOrder = 1;
            this.menuItemExecute.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.menuItemExecute.Text = "E&xecute";
            this.menuItemExecute.Click += new System.EventHandler(this.menuItemExecute_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.MergeOrder = 2;
            this.menuItem3.Text = "-";
            // 
            // menuItemEncryptAs
            // 
            this.menuItemEncryptAs.Index = 2;
            this.menuItemEncryptAs.Text = "Encr&ypt As ...";
            this.menuItemEncryptAs.Click += new System.EventHandler(this.menuItemEncryptAs_Click);
            // 
            // menuItemCompileAs
            // 
            this.menuItemCompileAs.Index = 3;
            this.menuItemCompileAs.Text = "&Compile As ...";
            this.menuItemCompileAs.Click += new System.EventHandler(this.menuItemCompileAs_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 4;
            this.menuItem5.Text = "-";
            // 
            // menuItemLineNumbers
            // 
            this.menuItemLineNumbers.Index = 5;
            this.menuItemLineNumbers.MergeOrder = 3;
            this.menuItemLineNumbers.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
            this.menuItemLineNumbers.Text = "Line &Numbers";
            this.menuItemLineNumbers.Click += new System.EventHandler(this.menuItemLineNumbers_Click);
            // 
            // menuItemWhitespace
            // 
            this.menuItemWhitespace.Index = 6;
            this.menuItemWhitespace.MergeOrder = 4;
            this.menuItemWhitespace.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
            this.menuItemWhitespace.Text = "Whitespace";
            this.menuItemWhitespace.Click += new System.EventHandler(this.menuItemWhitespace_Click);
            // 
            // menuItemIndentation
            // 
            this.menuItemIndentation.Index = 7;
            this.menuItemIndentation.MergeOrder = 5;
            this.menuItemIndentation.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
            this.menuItemIndentation.Text = "Indentation Guids";
            this.menuItemIndentation.Click += new System.EventHandler(this.menuItemIndentation_Click);
            // 
            // menuItemEndOfLine
            // 
            this.menuItemEndOfLine.Index = 8;
            this.menuItemEndOfLine.MergeOrder = 6;
            this.menuItemEndOfLine.Shortcut = System.Windows.Forms.Shortcut.CtrlE;
            this.menuItemEndOfLine.Text = "End of Line";
            this.menuItemEndOfLine.Click += new System.EventHandler(this.menuItemEndOfLine_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 9;
            this.menuItem2.MergeOrder = 7;
            this.menuItem2.Text = "-";
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 10;
            this.menuItem1.MergeOrder = 8;
            this.menuItem1.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
            this.menuItem1.Text = "Go to Line ...";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 11;
            this.menuItem4.MergeOrder = 9;
            this.menuItem4.Text = "-";
            // 
            // menuItemZoomIn
            // 
            this.menuItemZoomIn.Index = 12;
            this.menuItemZoomIn.MergeOrder = 10;
            this.menuItemZoomIn.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
            this.menuItemZoomIn.Text = "Enlarge Font";
            this.menuItemZoomIn.Click += new System.EventHandler(this.menuItemZoomIn_Click);
            // 
            // menuItemZoomOut
            // 
            this.menuItemZoomOut.Index = 13;
            this.menuItemZoomOut.MergeOrder = 11;
            this.menuItemZoomOut.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftM;
            this.menuItemZoomOut.Text = "Reduce Font";
            this.menuItemZoomOut.Click += new System.EventHandler(this.menuItemZoomOut_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 14;
            this.menuItem7.MergeOrder = 12;
            this.menuItem7.Text = "-";
            // 
            // menuItemClipboard
            // 
            this.menuItemClipboard.Index = 15;
            this.menuItemClipboard.MergeOrder = 13;
            this.menuItemClipboard.Text = "Copy &Output To Clipboard";
            this.menuItemClipboard.Click += new System.EventHandler(this.menuItemClipboard_Click);
            // 
            // TemplateEditor
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(976, 689);
            this.Controls.Add(this.tabControlTemplate);
            this.Controls.Add(this.splitterProperties);
            this.Controls.Add(this.panelProperties);
            this.Controls.Add(this.splitterConsole);
            this.Controls.Add(this.panelConsole);
            this.Controls.Add(this.toolBar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "TemplateEditor";
            this.TabText = "Template Editor";
            this.Text = "Template Editor";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.TemplateEditor_Closing);
            this.DockStateChanged += new System.EventHandler(this.TemplateEditor_DockStateChanged);
            this.Load += new System.EventHandler(this.TemplateEditor_Load);
            this.panelConsole.ResumeLayout(false);
            this.panelConsole.PerformLayout();
            this.panelProperties.ResumeLayout(false);
            this.panelProperties.PerformLayout();
            this.groupBoxScripting.ResumeLayout(false);
            this.groupBoxScripting.PerformLayout();
            this.tabControlTemplate.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public bool IsDirty 
		{
			get 
			{
				if (this.scintillaTemplateCode.IsModify || this.scintillaGUICode.IsModify)
					this._isDirty = true;

				return this._isDirty;
			}
		}

		private string[] PickFiles() 
		{
			openFileDialog.InitialDirectory = this._template.FilePath;
			openFileDialog.RestoreDirectory = true;
			openFileDialog.Multiselect = true;
			openFileDialog.Filter = TemplateEditor.FILE_TYPES;
			openFileDialog.FilterIndex = TemplateEditor.DEFAULT_OPEN_FILE_TYPE_INDEX;
       
			if(openFileDialog.ShowDialog() == DialogResult.OK)
			{
				return openFileDialog.FileNames;
			}
			else 
			{
				return new string[] {};
			}
		}

		public bool IsNew 
		{
			get 
			{
				return (this._template.FilePath == string.Empty);
			}
		}

		public void Log(string input) 
		{
			if (input != null)
				this.textBoxConsole.AppendText(input);

			this.textBoxConsole.Refresh();
		}

		public void LogLn(string input) 
		{
			if (input != null)
				this.textBoxConsole.AppendText(DateTime.Now.ToString() + " - " + input + System.Environment.NewLine);

			this.textBoxConsole.Refresh();
		}

		private void Log_EntryAdded(object sender, EventArgs args) 
		{
			if (sender != null) 
				this.LogLn(sender.ToString());
		}

		#region Populating the Control from the Template and vise-versa
		private void RefreshControlFromTemplate() 
		{
			if (this.comboBoxEngine.Items.Count == 0)
			{
				this.FillEngineDropdowns();
			}

			this.comboBoxEngine.SelectedIndex = this.comboBoxEngine.Items.IndexOf( this._template.BodySegment.Engine );
			this.comboBoxGuiEngine.SelectedIndex = this.comboBoxGuiEngine.Items.IndexOf( this._template.GuiSegment.Engine );

			if (this.comboBoxLanguage.Items.Count == 0)
			{
				this.FillBodyLanguageDropdown();
			}

			if (this.comboBoxGuiLanguage.Items.Count == 0)
			{
				this.FillGuiLanguageDropdown();
			}

			if (this.comboBoxOutputLanguage.Items.Count == 0)
			{
				this.FillLanguageDropdown();
			}

			if (this.comboBoxMode.Items.Count == 0)
			{
				this.FillModeDropdown();
			}

			if (this.comboBoxType.Items.Count == 0)
			{
				this.FillTypeDropdown();
			}

			int tmpPos = this.CurrentScintilla.CurrentPos;
			int tmpStartPos = this.CurrentScintilla.SelectionStart;
			int tmpEndPos = this.CurrentScintilla.SelectionEnd;
			int tmpFirstLineVisible = this.CurrentScintilla.FirstVisibleLine;
	
			this.Text = this._template.Title;
            this.TabText = this._template.Title;
			
			this.textBoxTitle.Text = this._template.Title;
			this.textBoxGroup.Text = this._template.NamespacePathString;
			this.textBoxUniqueID.Text = this._template.UniqueID;
			this.textBoxComments.Text = this._template.Comments;

			this.comboBoxMode.SelectedIndex = (this._template.BodySegment.Mode == ZeusConstants.Modes.MARKUP ? 0 : (this._template.BodySegment.Mode == ZeusConstants.Modes.PURE ? 1 : 2));

			if (this._template.Type == ZeusConstants.Types.GROUP)
			{
				this.buttonSelectFile.Visible = true;
				this.listBoxIncludedTemplateFiles.Visible = true;
				this.labelIncludedTemplates.Visible = true;
				this.listBoxIncludedTemplateFiles.Items.Clear();
				foreach (string path in _template.IncludedTemplatePaths) 
				{
					this.listBoxIncludedTemplateFiles.Items.Add(new FileListItem(path));
				}
			}
			else 
			{
				this.buttonSelectFile.Visible = false;
				this.listBoxIncludedTemplateFiles.Visible = false;
				this.labelIncludedTemplates.Visible = false;
			}

			if (this._template.BodySegment.Mode == ZeusConstants.Modes.MARKUP) 
			{
				this.textBoxStartTag.Text = this._template.TagStart;
				this.textBoxEndTag.Text = this._template.TagEnd;
				this.textBoxShortcutTag.Text = this._template.TagStartShortcut;
			}

			this.scintillaTemplateCode.Clear();
			this.scintillaTemplateCode.Text = this._template.BodySegment.CodeUnparsed;

			this.scintillaTemplateSource.IsReadOnly = false;
			this.scintillaTemplateSource.Text = this._template.BodySegment.Code;
			this.scintillaTemplateSource.IsReadOnly = true;

			this.scintillaGUICode.Clear();
			this.scintillaGUICode.Text = this._template.GuiSegment.CodeUnparsed;

			this.scintillaGuiSource.IsReadOnly = false;
			this.scintillaGuiSource.Text = this._template.GuiSegment.Code;
			this.scintillaGuiSource.IsReadOnly = true;

			this.CurrentScintilla.LineScroll(0, tmpFirstLineVisible);
			this.CurrentScintilla.CurrentPos = tmpPos;
			this.CurrentScintilla.SelectionStart = tmpStartPos;
			this.CurrentScintilla.SelectionEnd = tmpEndPos;

			this.comboBoxType.SelectedIndex = this.comboBoxType.Items.IndexOf( this._template.Type );
			this.comboBoxMode.SelectedIndex = this.comboBoxMode.Items.IndexOf( this._template.BodySegment.Mode );

			this.comboBoxLanguage.SelectedIndex       = this.comboBoxLanguage.Items.IndexOf( this._template.BodySegment.Language );
			this.comboBoxGuiLanguage.SelectedIndex    = this.comboBoxGuiLanguage.Items.IndexOf( this._template.GuiSegment.Language );
			this.comboBoxOutputLanguage.SelectedIndex = this.comboBoxOutputLanguage.Items.IndexOf( this._template.OutputLanguage );
		}
		
		private void RefreshTemplateFromControl() 
		{
			try
			{
				this._template.Title = this.textBoxTitle.Text;
				this._template.NamespacePathString = this.textBoxGroup.Text;
				this._template.UniqueID = this.textBoxUniqueID.Text;
				this._template.Comments = this.textBoxComments.Text;

				if (comboBoxEngine.SelectedIndex >= 0) 
				{
					string newEngine = comboBoxEngine.SelectedItem.ToString();

					if (newEngine != this._template.BodySegment.Engine)
					{
						this._template.BodySegment.Engine = newEngine;
					}
				}
				if (comboBoxLanguage.SelectedIndex >= 0) 
				{
					this._template.BodySegment.Language = comboBoxLanguage.SelectedItem.ToString();
				}
				if (comboBoxOutputLanguage.SelectedIndex >= 0) 
				{
					this._template.OutputLanguage = comboBoxOutputLanguage.SelectedItem.ToString();
				}

				this._template.GuiSegment.CodeUnparsed = this.scintillaGUICode.Text;

				if (this._template.Type == ZeusConstants.Types.GROUP)
				{
					this._template.IncludedTemplatePaths.Clear();
					foreach (FileListItem item in this.listBoxIncludedTemplateFiles.Items)
					{
						this._template.AddIncludedTemplatePath(item.Value);
					}
				}

				this._template.BodySegment.Mode = comboBoxMode.SelectedIndex == 0 ? ZeusConstants.Modes.MARKUP : ZeusConstants.Modes.PURE;

				this._template.TagStart = this.textBoxStartTag.Text;
				this._template.TagEnd = this.textBoxEndTag.Text;

				this._template.BodySegment.CodeUnparsed = this.scintillaTemplateCode.Text;

				this.scintillaTemplateSource.IsReadOnly = false;
				this.scintillaTemplateSource.Text = this._template.BodySegment.Code;
				this.scintillaTemplateSource.IsReadOnly = true;

				this.scintillaGuiSource.IsReadOnly = false;
				this.scintillaGuiSource.Text = this._template.GuiSegment.Code;
				this.scintillaGuiSource.IsReadOnly = true;
			}
			catch (Exception x)
			{
				ZeusDisplayError formError = new ZeusDisplayError(x);
				formError.ErrorIndexChange += new EventHandler(ZeusDisplayError_ErrorIndexChanged);
				formError.ShowDialog(this);

				foreach (string message in formError.LastErrorMessages) 
				{
					LogLn(message);
				}
			}		
		}
		#endregion
		
		#region Action Methods (Save, SaveAs, Initialize, Execute)
		protected void _Save() 
		{
			if (this.IsNew) 
			{
				this.menuItemSaveAs_Click(this.menuItemSaveAs, new EventArgs());
			}
			else 
			{
				this.FileSave();
			}
			this.CurrentScintilla.GrabFocus();// = true;
		}

		protected void _EncryptAs() 
		{
			DialogResult dr = MessageBox.Show(this,
					"Be careful not to overwrite your source template or you will lose your work!\r\n Are you ready to Encrypt?", 
					"Encryption Warning", 
					MessageBoxButtons.OKCancel, 
					MessageBoxIcon.Information);
				if (dr == DialogResult.OK) 
				{
					RefreshTemplateFromControl();
					_template.Encrypt();
					RefreshControlFromTemplate();

					_SaveAs();

					this.menuItemCompileAs.Enabled = false;
					this.menuItemEncryptAs.Enabled = false;
				}
		}

		protected void _CompileAs() 
		{
			DialogResult dr = MessageBox.Show(this,
				"In order to finish compiling a template, the template must be executed completely.\r\nBe careful not to overwrite your source template or you will lose your work!\r\n Are you ready to Compile?", 
				"Compilation Warning", 
				MessageBoxButtons.OKCancel, 
				MessageBoxIcon.Information);
			if (dr == DialogResult.OK) 
			{
				RefreshTemplateFromControl();
				_template.Compile();
				_Execute();
				RefreshControlFromTemplate();

				_SaveAs();

				this.menuItemCompileAs.Enabled = false;
				this.menuItemEncryptAs.Enabled = false;
			}
		}

		protected void _SaveAs() 
		{
			Stream myStream;
			SaveFileDialog saveFileDialog = new SaveFileDialog();
       
			saveFileDialog.Filter = FILE_TYPES;
			saveFileDialog.FilterIndex = DEFAULT_SAVE_FILE_TYPE_INDEX;
			saveFileDialog.RestoreDirectory = true;

			saveFileDialog.FileName = this.FileName;

			if(saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				myStream = saveFileDialog.OpenFile();

				if(null != myStream) 
				{
					myStream.Close();
					this.FileSaveAs(saveFileDialog.FileName);
				}
			}
			this.CurrentScintilla.GrabFocus();// = true;
		}

		private void _Initialize(string path, params object[] options) 
		{
			bool isNew = false;
			if (path == string.Empty) 
			{
				isNew = true;
			}
			else if (File.Exists(path)) 
			{
				try 
				{
					_template = new ZeusTemplate(path);

					if (_template.SourceType == ZeusConstants.SourceTypes.SOURCE) 
					{
						this.LogLn("Opened Template: \"" + _template.Title + "\" from \"" + _template.FilePath + _template.FileName + "\".");
					}
					else 
					{
						throw new Exception("Cannot edit locked templates.");
					}
				}
				catch (Exception x)
				{
					this.LogLn("Error loading template with path: " + path);

					ZeusDisplayError formError = new ZeusDisplayError(x);
					formError.ShowDialog(this);

					foreach (string message in formError.LastErrorMessages) 
					{
						LogLn(message);
					}

					// Make sure it's treated as a new template
					isNew = true;
				}
			}

			if (isNew)
			{
				_template = new ZeusTemplate();
				_template.Title = "Untitled";
				_template.UniqueID = Guid.NewGuid().ToString();

				this.panelProperties.Visible = true;

				if ((options.Length % 2) == 0) 
				{
					string key, val;
					for (int i=0; i < options.Length; i += 2)
					{
						key = options[i].ToString();
						val = options[i+1].ToString();
					
						if (key == "ENGINE")
						{
							_template.BodySegment.Engine = val;
							_template.GuiSegment.Engine = val;
						}
						else if (key == "LANGUAGE")
						{
							_template.BodySegment.Language = val;
							_template.GuiSegment.Language = val;
						}
					}
				}
				_template.GuiSegment.CodeUnparsed = _template.GuiSegment.ZeusScriptingEngine.GetNewGuiText(_template.GuiSegment.Language);
				_template.BodySegment.CodeUnparsed = _template.BodySegment.ZeusScriptingEngine.GetNewTemplateText(_template.BodySegment.Language);
			}

			this.RefreshControlFromTemplate();
		}

		private void _SaveTemplateInput() 
		{
			try 
			{
				Directory.SetCurrentDirectory(Application.StartupPath);
				this.RefreshTemplateFromControl();

				DefaultSettings settings = DefaultSettings.Instance;

				ZeusSimpleLog log = new ZeusSimpleLog();
				log.LogEntryAdded += new EventHandler(Log_EntryAdded);
				ZeusContext context = new ZeusContext();
				context.Log = log;

				ZeusSavedInput collectedInput = new ZeusSavedInput();
				collectedInput.InputData.TemplateUniqueID = _template.UniqueID;
				collectedInput.InputData.TemplatePath = _template.FilePath + _template.FileName;

				settings.PopulateZeusContext(context);

				_template.Collect(context, settings.ScriptTimeout, collectedInput.InputData.InputItems);
					
				if (log.HasExceptions) 
				{
					throw log.Exceptions[0];
				}
				else 
				{
					SaveFileDialog saveFileDialog = new SaveFileDialog();
					saveFileDialog.Filter = "Zues Input Files (*.zinp)|*.zinp";
					saveFileDialog.FilterIndex = 0;
					saveFileDialog.RestoreDirectory = true;
					if(saveFileDialog.ShowDialog() == DialogResult.OK)
					{
						Cursor.Current = Cursors.WaitCursor;

						collectedInput.FilePath = saveFileDialog.FileName;
						collectedInput.Save();
					}
				}

				MessageBox.Show(this, "Input collected and saved to file:" + "\r\n" + collectedInput.FilePath);
			}
			catch (Exception ex)
			{
				HandleExecuteException(ex);
			}

			Cursor.Current = Cursors.Default;
		}

		private void _Execute() 
		{
			this.Cursor = Cursors.WaitCursor;

			Directory.SetCurrentDirectory(Application.StartupPath);

			this.RefreshTemplateFromControl();

			DefaultSettings settings = DefaultSettings.Instance;
			
			ZeusContext context = new ZeusContext();
			if (context.Log is ZeusSimpleLog) 
			{
				ZeusSimpleLog log = context.Log as ZeusSimpleLog;
				log.LogEntryAdded += new EventHandler(Log_EntryAdded);
			}
			IZeusGuiControl guiController = context.Gui;
			IZeusOutput zout = context.Output;
			
			settings.PopulateZeusContext(context);

			bool exceptionOccurred = false;
			bool result = false;
			Exception tmpEx = null;
			try 
			{
				_template.GuiSegment.ZeusScriptingEngine.ExecutionHelper.Timeout = settings.ScriptTimeout;
				_template.GuiSegment.ZeusScriptingEngine.ExecutionHelper.SetShowGuiHandler(new ShowGUIEventHandler(DynamicGUI_Display));
				result = _template.GuiSegment.Execute(context); 
				_template.GuiSegment.ZeusScriptingEngine.ExecutionHelper.Cleanup();

				if (result) 
				{
					_template.BodySegment.ZeusScriptingEngine.ExecutionHelper.Timeout = settings.ScriptTimeout;
					result = _template.BodySegment.Execute(context);
					_template.BodySegment.ZeusScriptingEngine.ExecutionHelper.Cleanup();
				}
			}
			catch (Exception x)
			{
				HandleExecuteException(x);
				tmpEx = x;
				exceptionOccurred = true;
			}
		
			if (context.Log is ZeusSimpleLog) 
			{
				ZeusSimpleLog simpleLog = context.Log as ZeusSimpleLog;
				if (simpleLog.HasExceptions) 
				{
					foreach (Exception ex in simpleLog.Exceptions) 
					{
						if (tmpEx != ex) 
						{
							HandleExecuteException(ex);
							exceptionOccurred = true;
						}
					}
				}
			}

			if (!exceptionOccurred && result)
			{
				zout = context.Output;

				this.scintillaOutput.IsReadOnly = false;
				
				if (zout.text == string.Empty) 
					this.scintillaOutput.ClearAll();
				else
					this.scintillaOutput.Text = zout.text;

				this.scintillaOutput.IsReadOnly = true;
				
				if (this.tabControlTemplate.SelectedTab != this.tabOutput) 
					this.tabControlTemplate.SelectedTab = this.tabOutput;

				this.LogLn("Successfully rendered template: " + this._template.Title);

				if (this.menuItemClipboard.Checked) 
				{
					try 
					{
						Clipboard.SetDataObject(zout.text, true);
					}
					catch
					{
						// HACK: For some reason, Clipboard.SetDataObject throws an error on some systems. I'm cathing it and doing nothing for now.
					}
				}
			}

			this.Cursor = Cursors.Default;
		}
		#endregion

		private void ZeusDisplayError_ErrorIndexChanged(object source, EventArgs args) 
		{
			ZeusDisplayError formError = source as ZeusDisplayError;

			ZeusScintillaControl ctrl = (formError.LastErrorIsTemplate ? scintillaTemplateSource : scintillaGuiSource);
			TabPage tab = (formError.LastErrorIsTemplate ? tabTemplateSource : tabInterfaceSource);
			this.tabControlTemplate.SelectedTab = tab;

			if (formError.LastErrorIsScript)
			{
				try 
				{
					if (formError.LastErrorFileName == this.FileName)
					{
						int lineNumber = formError.LastErrorLineNumber - (formError.LastErrorLineNumber > 0 ? 1 : 0);

						ctrl.GrabFocus();
						ctrl.GotoLine(lineNumber);
						ctrl.EnsureVisibleEnforcePolicy(lineNumber);
						ctrl.SelectionStart = ctrl.CurrentPos;
						ctrl.SelectionEnd   = ctrl.CurrentPos + ctrl.GetCurLine().Length - 1;
					}
				}
				catch {}
			}
		}

		public void HandleExecuteException(Exception ex) 
		{
			this.scintillaOutput.IsReadOnly = false;
			this.scintillaOutput.Text = string.Empty;
			this.scintillaOutput.IsReadOnly = true;

			ZeusDisplayError formError = new ZeusDisplayError(ex);
			formError.ErrorIndexChange += new EventHandler(ZeusDisplayError_ErrorIndexChanged);
			formError.ShowDialog(this);
			
			foreach (string message in formError.LastErrorMessages) 
			{
				LogLn(message);
			}
		}

		public void FileNew(params object[] options) 
		{
			this._Initialize(string.Empty, options);
			SetClean();
		}

		public void FileOpen(string path)
		{
			if (File.Exists(path)) 
			{
				this._Initialize(path);
				SetClean();
			}
		}

		public void FileSave()
		{
			this.RefreshTemplateFromControl();

			if (this._template.FileName != string.Empty) 
			{
				string path = this._template.FilePath + this._template.FileName;
				FileInfo fi = new FileInfo(path);
                if (fi.Exists) 
				{
                    if (fi.IsReadOnly)
                    {
                        MessageBox.Show(this, "File is read only.");
                    }
                    else
                    {
                        try
                        {
                            _template.Save(path);
                            SetClean();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, "Error saving file. " + ex.Message);
                        }
                    }
				} 
			}

		}

		public void FileSaveAs(string path)
		{
			bool isopen = MDIParent.TheParent.IsTemplateOpen(path, this);
			
			if (!isopen) 
			{
				this.RefreshTemplateFromControl();

                FileInfo fi = new FileInfo(path);
                if (fi.Exists)
                {
                    if (fi.IsReadOnly)
                    {
                        MessageBox.Show(this, "File is read only.");
                    }
                    else
                    {
                        try
                        {
                            _template.Save(path);
                            string dir = Path.GetDirectoryName(path);
                            if (!dir.EndsWith("\\")) dir += "\\";

                            this._template.FilePath = dir;
                            this._template.FileName = Path.GetFileName(path);

                            this.RefreshControlFromTemplate();
                            SetClean();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, "Error saving file. " + ex.Message);
                        }
                    }
                } 

			}
			else 
			{
                MessageBox.Show(this, "The template you are trying to overwrite is currently open.\r\nClose the editor window for that template if you want to overwrite it.");
			}
		}

		protected void SetClean() 
		{
			this.scintillaTemplateCode.SetSavePoint();
			this.scintillaGUICode.SetSavePoint();
			this._isDirty = false;
		}

		public string FileName 
		{
			get 
			{
				if ((_template != null) && (_template.FileName != string.Empty))
				{
					return _template.FilePath + _template.FileName;
				}
				else
				{
					return string.Empty;
				}
			}
		}

		public string CompleteFilePath
		{
			get 
			{
				string tmp = this.FileName;
				if (tmp != string.Empty) 
				{
					FileInfo attr = new FileInfo(tmp);
					tmp = attr.FullName;
				}

				return tmp;
			}
		}

		public string UniqueID
		{
			get 
			{
				return this.textBoxUniqueID.Text;
			}
		}

		public string Title
		{
			get 
			{
				return this.textBoxTitle.Text;
			}
		}

		private bool listBoxContainsItem(System.Windows.Forms.ListBox l, String i) 
		{
			foreach (FileListItem item in l.Items) 
			{
				if (item.CompareTo(i) == 0)
					return true;
			}
			return false;
		}

		#region Event Handlers
		private void textBoxStartTag_KeyUp(object sender, KeyEventArgs e) 
		{
			this.textBoxShortcutTag.Text = textBoxStartTag.Text + "=";
			
			this.scintillaTemplateCode.UpdateModeAndLanguage(_template.BodySegment.Language, _template.BodySegment.Mode);

			this._isDirty = true;
		}

		private void buttonNewGuid_Click(object sender, System.EventArgs e)
		{
			this.textBoxUniqueID.Text = Guid.NewGuid().ToString();
			
			this._isDirty = true;
		}

		private void panel_ToggleVisibility(object sender, System.EventArgs e) 
		{
			if (sender == this.panelConsole) 
			{
				this.toolBarButtonConsole.Pushed = this.panelConsole.Visible;
			}
			else 
			{
				this.toolBarButtonProperties.Pushed = this.panelProperties.Visible;
			}
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == toolBarButtonConsole) 
			{
				this.splitterConsole.ToggleState();
			}
			else if (e.Button == toolBarButtonProperties) 
			{
				this.splitterProperties.ToggleState();
			}
			else if (e.Button == toolBarButtonSave) 
			{
				this._Save();
			}
			else if (e.Button == toolBarButtonSaveAs) 
			{
				this._SaveAs();
			}
			else if (e.Button == toolBarButtonExecute) 
			{
				this._Execute();
			}
			else if (e.Button == toolBarSaveTemplateInput) 
			{
				this._SaveTemplateInput();
			}
		}

		public void DynamicGUI_Display(IZeusGuiControl gui, IZeusFunctionExecutioner executioner) 
		{
			this.Cursor = Cursors.Default;

			try 
			{
				DynamicForm df = new DynamicForm(gui as GuiController, executioner);
				df.Logger = this;
				DialogResult result = df.ShowDialog(this);
				
				if(result == DialogResult.Cancel) 
				{
					gui.IsCanceled = true;
				}
			}
			catch (Exception x)
			{
				HandleExecuteException(x);
			}

			this.Cursor = Cursors.WaitCursor;
		}

		private void comboBoxEngine_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string eng = comboBoxEngine.SelectedItem.ToString();
			
			this._template.BodySegment.Engine = eng;

			this.comboBoxLanguage.SelectedIndex = -1;
			this.comboBoxLanguage.Items.Clear();
			this.FillBodyLanguageDropdown();
			//if (comboBoxLanguage.Items.Count > 0) comboBoxLanguage.SelectedIndex = 0;

			this._isDirty = true;
		}

		private void comboBoxGuiEngine_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string eng = comboBoxGuiEngine.SelectedItem.ToString();
			
			this._template.GuiSegment.Engine = eng;

			this.comboBoxGuiLanguage.SelectedIndex = -1;
			this.comboBoxGuiLanguage.Items.Clear();
			this.FillGuiLanguageDropdown();
			//if (comboBoxGuiLanguage.Items.Count > 0) comboBoxGuiLanguage.SelectedIndex = 0;

			this._isDirty = true;
		}

		private void comboBoxLanguage_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (comboBoxLanguage.SelectedIndex >= 0) 
			{
				string lang = comboBoxLanguage.SelectedItem.ToString();
				string mode = this._template.BodySegment.Mode;
		
				this._template.BodySegment.Language = lang;

				this.scintillaTemplateCode.UpdateModeAndLanguage(lang, mode);
				this.scintillaTemplateSource.UpdateModeAndLanguage(lang, ZeusConstants.Modes.PURE);
			}
			this._isDirty = true;
		}

		private void comboBoxGuiLanguage_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (comboBoxGuiLanguage.SelectedIndex >= 0) 
			{
				string lang = comboBoxGuiLanguage.SelectedItem.ToString();
		
				this._template.GuiSegment.Language = lang;

				this.scintillaGUICode.UpdateModeAndLanguage(lang, ZeusConstants.Modes.PURE);
				this.scintillaGuiSource.UpdateModeAndLanguage(lang, ZeusConstants.Modes.PURE);
			}
			this._isDirty = true;
		}

		//TODO: Fix the TemplateGroup Stuff!!!
		private void comboBoxMode_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (comboBoxMode.SelectedIndex >= 0) 
			{
				switch (comboBoxMode.Items[comboBoxMode.SelectedIndex].ToString()) 
				{
					case ZeusConstants.Modes.MARKUP:
						this._template.BodySegment.Mode = ZeusConstants.Modes.MARKUP;

						this.scintillaTemplateCode.UpdateModeAndLanguage(_template.BodySegment.Language, _template.BodySegment.Mode);

						//						this.scintillaTemplateCode.Language = _template.BodySegment.Language;
						//						this.scintillaTemplateCode.Mode = _template.BodySegment.Mode;
						this.groupBoxScripting.Enabled = true;

						break;
					case ZeusConstants.Modes.PURE:
						this._template.BodySegment.Mode = ZeusConstants.Modes.PURE;

						this.scintillaTemplateCode.UpdateModeAndLanguage(_template.BodySegment.Language, _template.BodySegment.Mode);

						//						this.scintillaTemplateCode.Language = _template.BodySegment.Language;
						this.groupBoxScripting.Enabled = false;

						break;
				}

				this._isDirty = true;
			}
		}

		private void comboBoxType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (comboBoxType.SelectedIndex >= 0) 
			{
				switch (comboBoxType.Items[comboBoxType.SelectedIndex].ToString()) 
				{
					case ZeusConstants.Types.TEMPLATE:
						this.listBoxIncludedTemplateFiles.Visible = false;
						this.buttonSelectFile.Visible = false;
						this.labelIncludedTemplates.Visible = false;

						if (this._template != null) this._template.Type = ZeusConstants.Types.TEMPLATE;

						break;
					case ZeusConstants.Types.GROUP:
						this.listBoxIncludedTemplateFiles.Visible = true;
						this.buttonSelectFile.Visible = true;
						this.labelIncludedTemplates.Visible = true;

						if (this._template != null) this._template.Type = ZeusConstants.Types.GROUP;

						break;
				}

				this._isDirty = true;
			}
		}

		private void comboBoxOutputLanguage_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string lang = comboBoxOutputLanguage.SelectedItem.ToString();
			this._template.OutputLanguage = lang;
			this.scintillaOutput.UpdateModeAndLanguage(lang, "");
			//	this.scintillaOutput.Language = lang;
			this._isDirty = true;
		}

		private void menuItemExecute_Click(object sender, System.EventArgs e)
		{
			this._Execute();
		}

		
		private void menuItemFind_Click(object sender, System.EventArgs e)
		{
            Scintilla.Forms.SearchHelper.Instance(CurrentScintilla).Criteria.SearchText = this.CurrentScintilla.GetSelectedText();
			MDIParent.TheParent.LaunchFindReplace(false);//, this.CurrentScintilla.GetSelectedText());
		}

		private void menuItemReplace_Click(object sender, System.EventArgs e)
		{
            Scintilla.Forms.SearchHelper.Instance(CurrentScintilla).Criteria.SearchText = this.CurrentScintilla.GetSelectedText();
            MDIParent.TheParent.LaunchFindReplace(true);//, this.CurrentScintilla.GetSelectedText());
		}

		private void buttonSelectFile_Click(object sender, System.EventArgs e)
		{
			string[] paths = PickFiles();
			foreach (string path in paths) 
			{
				if (path != string.Empty && !listBoxContainsItem(this.listBoxIncludedTemplateFiles, path) )
				{
					this.listBoxIncludedTemplateFiles.Items.Add(new FileListItem(path));
					this._isDirty = true;
				}
			}
		}

		private void listBoxIncludedTemplateFiles_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if ((e.KeyCode == Keys.Delete) || (e.KeyCode == Keys.Back)) 
			{
				int index = listBoxIncludedTemplateFiles.SelectedIndex;
				if (index > 0) index--;

				listBoxIncludedTemplateFiles.Items.Remove(listBoxIncludedTemplateFiles.SelectedItem);
				
				if (index < listBoxIncludedTemplateFiles.Items.Count) listBoxIncludedTemplateFiles.SelectedIndex = index;
				this._isDirty = true;
			}
		}
		
		private void TemplateEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!this.CanClose(true)) 
			{
				e.Cancel = true;
			}	
		}

		private void menuItemClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void menuItemSave_Click(object sender, System.EventArgs e)
		{
			this._Save();
		}

		private void menuItemSaveAs_Click(object sender, System.EventArgs e)
		{
			this._SaveAs();	
		}

		private void menuItemEncryptAs_Click(object sender, System.EventArgs e)
		{
			this._EncryptAs();
		}

		private void menuItemCompileAs_Click(object sender, System.EventArgs e)
		{
			this._CompileAs();
		}

		private void tabControlTemplate_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (tabControlTemplate.SelectedTab == this.tabTemplateSource) 
			{			
				this.RefreshTemplateFromControl();
			}

			//if (tabControlTemplate.SelectedIndex < 2) 
			//{
                this.CurrentScintilla.Activate();
			//}

			// Clear the status bar row/col coordinates text
			MDIParent.TheParent.statusRow.Text = "";
			MDIParent.TheParent.statusCol.Text = "";
		}

		private void menuItemLineNumbers_Click(object sender, System.EventArgs e)
		{
			bool lineNumbersEnabled = !this.menuItemLineNumbers.Checked;
			
			if (lineNumbersEnabled) 
			{
				this.scintillaTemplateCode.MarginWidthN(0, 40);
				this.scintillaGUICode.MarginWidthN(0, 40);
				this.scintillaTemplateSource.MarginWidthN(0, 40);
				this.scintillaGuiSource.MarginWidthN(0, 40);
				this.scintillaOutput.MarginWidthN(0, 40);
			}
			else 
			{
				this.scintillaTemplateCode.MarginWidthN(0, 0);
				this.scintillaGUICode.MarginWidthN(0, 0);
				this.scintillaTemplateSource.MarginWidthN(0, 0);
				this.scintillaGuiSource.MarginWidthN(0, 0);
				this.scintillaOutput.MarginWidthN(0, 0);
			}

			this.menuItemLineNumbers.Checked = lineNumbersEnabled;

		}

		private void menuItemIndentation_Click(object sender, System.EventArgs e)
		{
			// Toggle the state
			bool on = !this.scintillaTemplateCode.IsIndentationGuides;

			this.scintillaTemplateCode.IsIndentationGuides = on;
			this.menuItemIndentation.Checked = on;
		}

		private void menuItemWhitespace_Click(object sender, System.EventArgs e)
		{
			// We reverse the state
			bool on = this.scintillaTemplateCode.ViewWhitespace != Scintilla.Enums.WhiteSpace.Invisible ? false : true;

            this.scintillaTemplateCode.ViewWhitespace = on ? Scintilla.Enums.WhiteSpace.VisibleAlways : Scintilla.Enums.WhiteSpace.Invisible;
			this.menuItemWhitespace.Checked = on;		
		}

		private void menuItemEndOfLine_Click(object sender, System.EventArgs e)
		{
			// Toggle the state
			bool on = !this.scintillaTemplateCode.IsViewEOL;

			this.scintillaTemplateCode.IsViewEOL = on;
			this.menuItemEndOfLine.Checked = on;	
			DefaultSettings settings = DefaultSettings.Instance;
		}

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			Goto go = new Goto();
			DialogResult result = go.ShowDialog();

			if(result == DialogResult.OK)
			{
				int lineNumber = Math.Max(0, go.LineNumber - 1);
				lineNumber = Math.Min(this.CurrentScintilla.LineCount, lineNumber);

				this.CurrentScintilla.GrabFocus();
				this.CurrentScintilla.EnsureVisibleEnforcePolicy(lineNumber);
				this.CurrentScintilla.EnsureVisible(lineNumber);
				this.CurrentScintilla.GotoLine(lineNumber);
			}
		}

		private void menuItemZoomIn_Click(object sender, System.EventArgs e)
		{
			this.scintillaTemplateCode.ZoomIn();
		}

		private void menuItemZoomOut_Click(object sender, System.EventArgs e)
		{
			if(this.scintillaTemplateCode.Zoom > 0)
			{
				this.scintillaTemplateCode.ZoomOut();	
			}
		}

		private void menuItemClipboard_Click(object sender, System.EventArgs e)
		{
			this.menuItemClipboard.Checked = !this.menuItemClipboard.Checked;
		}
		#endregion

		private void TemplateEditor_Load(object sender, System.EventArgs e)
		{
			this.splitterProperties.ToggleState();
		}

		private bool PromptForSave(bool allowPrevent)
		{
			bool canClose = true;

			if(this.IsDirty)
			{
				DialogResult result;

				if(allowPrevent)
				{
					result = MessageBox.Show("This template has been modified, Do you wish to save before closing?", 
						this._template.FileName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				}
				else
				{
					result = MessageBox.Show("This template has been modified, Do you wish to save before closing?", 
						this._template.FileName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				}

				switch(result)
				{
					case DialogResult.Yes:
						this._Save();
						break;
					case DialogResult.Cancel:
						canClose = false;
						break;
				}
			}

			return canClose;
		}

		private void FillLanguageDropdown() 
		{
			this.comboBoxOutputLanguage.Items.Clear();
			foreach (string lang in ZeusFactory.AvailableLanguages)
			{
				this.comboBoxOutputLanguage.Items.Add(lang);
			}
		}

		private void FillBodyLanguageDropdown() 
		{
			this.comboBoxLanguage.Items.Clear();
			foreach (string lang in _template.BodySegment.ZeusScriptingEngine.SupportedLanguages)
			{
				this.comboBoxLanguage.Items.Add(lang);
			}
			this.comboBoxLanguage.SelectedIndex = -1;
		}

		private void FillGuiLanguageDropdown() 
		{
			this.comboBoxGuiLanguage.Items.Clear();
			foreach (string lang in _template.GuiSegment.ZeusScriptingEngine.SupportedLanguages)
			{
				this.comboBoxGuiLanguage.Items.Add(lang);
			}
			this.comboBoxGuiLanguage.SelectedIndex = -1;
		}

		private void FillModeDropdown() 
		{
			this.comboBoxMode.Items.Clear();
			foreach (string mode in ZeusFactory.TemplateModes)
			{
				this.comboBoxMode.Items.Add(mode);
			}
		}

		private void FillTypeDropdown() 
		{
			this.comboBoxType.Items.Clear();
			foreach (string type in ZeusFactory.TemplateTypes)
			{
				this.comboBoxType.Items.Add(type);
			}
		}

		private void FillEngineDropdowns() 
		{
			this.comboBoxEngine.Items.Clear();
			this.comboBoxGuiEngine.Items.Clear();
			foreach (string engine in ZeusFactory.EngineNames)
			{
				this.comboBoxEngine.Items.Add(engine);
				this.comboBoxGuiEngine.Items.Add(engine);
			}
		}

        private void menuItemFindNext_Click(object sender, EventArgs e)
        {
            ZeusScintillaControl.FindDialog.FindNext();
        }

        private void TemplateEditor_DockStateChanged(object sender, EventArgs e)
        {
            if ((this.DockState != DockState.Unknown) &&
                (this.DockState != DockState.Hidden))
            {
                this.scintillaGUICode.SpecialRefresh();
                this.scintillaGuiSource.SpecialRefresh();
                this.scintillaOutput.SpecialRefresh();
                this.scintillaTemplateCode.SpecialRefresh();
                this.scintillaTemplateSource.SpecialRefresh();
            }
        }

        #region IScintillaEditControl Members

        public ScintillaControl ScintillaEditor
        {
            get { return this.CurrentScintilla; }
        }

        #endregion

        #region IMyGenDocument Members

        public string DocumentIndentity
        {
            get { return this.UniqueID; }
        }
        public ToolStrip ToolStrip
        {
            get { return null; }
        }

        #endregion

        #region IMyGenContent Members


        public void Alert(IMyGenContent sender, string command, params object[] args)
        {
            if (command == "UpdateDefaultSettings")
            {
                DefaultSettings settings = DefaultSettings.Instance;
                SetCodePageOverride(settings.CodePage);
                SetFontOverride(settings.FontFamily);

                this.scintillaTemplateCode.TabWidth = settings.Tabs;
                this.menuItemClipboard.Checked = settings.EnableClipboard;
            }
        }

        #endregion
    }

	/// <summary>
	/// An item for a listbox that holds a filename
	/// </summary>
	public class FileListItem : IComparable
	{
		private string _value;
		private string _text;

		public FileListItem(string filename) 
		{
			this._value = filename;
			
			FileInfo f = new FileInfo(filename);
			this._text = f.Name;
		}

		public string Text 
		{
			get 
			{
				return _text;
			}
			set 
			{
				_text = value;
			}
		}

		public string Value 
		{
			get 
			{
				return _value;
			}
			set 
			{
				_value = value;
			}
		}

		public override string ToString()
		{
			return _text;
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			return this._value.CompareTo(obj.ToString());
		}

		#endregion

	}
}
