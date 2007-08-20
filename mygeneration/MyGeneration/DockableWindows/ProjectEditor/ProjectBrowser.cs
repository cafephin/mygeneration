using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Zeus;
using Zeus.Projects;
using Zeus.Serializers;
using Zeus.UserInterface;
using Zeus.UserInterface.WinForms;
using MyMeta;
using WeifenLuo.WinFormsUI.Docking;

namespace MyGeneration
{
	/// <summary>
	/// Summary description for ProjectBrowser.
	/// </summary>
    public class ProjectBrowser : DockContent, IMyGenDocument
	{
		private const int INDEX_CLOSED_FOLDER = 0;
		private const int INDEX_OPEN_FOLDER = 1;

        private IMyGenerationMDI mdi;
		private System.Windows.Forms.ToolBar toolBarToolbar;
		private System.Windows.Forms.ImageList imageListFormIcons;
		private System.Windows.Forms.ContextMenu contextMenuTree;
		private System.Windows.Forms.TreeView treeViewProject;
		private System.Windows.Forms.ToolTip toolTipProjectBrowser;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolBarButton toolBarButtonSep1;
		private System.Windows.Forms.ToolBarButton toolBarButtonView;
		private System.Windows.Forms.ToolBarButton toolBarButtonExecute;
		private System.Windows.Forms.ToolBarButton toolBarButtonSave;
		private System.Windows.Forms.ToolBarButton toolBarButtonSep2;
		private System.Windows.Forms.ToolBarButton toolBarButtonSaveAs;
		private System.Windows.Forms.MainMenu mainMenuProject;
		private System.Windows.Forms.MenuItem menuItemProject;
		private System.Windows.Forms.MenuItem menuItemExecute;
		private System.Windows.Forms.MenuItem menuItemFile;
		private System.Windows.Forms.MenuItem menuItemSave;
		private System.Windows.Forms.MenuItem menuItemSaveAs;
		private System.Windows.Forms.MenuItem menuItemClose;
		private System.Windows.Forms.MenuItem contextItemEdit;
		private System.Windows.Forms.MenuItem contextItemAddModule;
		private System.Windows.Forms.MenuItem contextItemAddSavedObject;
		private System.Windows.Forms.MenuItem contextItemExecute;
		private System.Windows.Forms.MenuItem contextItemSave;
		private System.Windows.Forms.MenuItem contextItemSaveAs;
		private System.Windows.Forms.MenuItem contextItemRemove;

		private ProjectTreeNode rootNode;
		private FormAddEditModule formEditModule = new FormAddEditModule();
		private FormAddEditSavedObject formEditSavedObject = new FormAddEditSavedObject();
		private System.Windows.Forms.MenuItem menuItemSep01;
		private System.Windows.Forms.MenuItem menuItemSep02;
		private System.Windows.Forms.MenuItem menuItemSep03;
		private System.Windows.Forms.MenuItem contextItemCopy;
		private System.Windows.Forms.MenuItem contextItemCacheSettings;
		
		private bool _isDirty = false;

        public ProjectBrowser(IMyGenerationMDI mdi)
		{
            InitializeComponent();
            this.mdi = mdi;
		}

		protected override string GetPersistString()
		{
			if(this.rootNode != null &&
				this.rootNode.Project != null &&
				this.rootNode.Project.FilePath != null)
			{
				return GetType().ToString() + "," + this.rootNode.Project.FilePath;
			}
			else
			{
				return GetType().ToString();
			}
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

		public string FileName 
		{
			get 
			{
				if (rootNode.Project.FilePath != null)
				{
					return rootNode.Project.FilePath;
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

		public bool IsDirty 
		{
			get 
			{
				return this._isDirty;
			}
		}

		public bool CanClose(bool allowPrevent)
		{
			return PromptForSave(allowPrevent);
		}

		private bool PromptForSave(bool allowPrevent)
		{
			bool canClose = true;

			if(this.IsDirty)
			{
				DialogResult result;

				if(allowPrevent)
				{
					result = MessageBox.Show("This project has been modified, Do you wish to save before closing?", 
						this.FileName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				}
				else
				{
					result = MessageBox.Show("This project has been modified, Do you wish to save before closing?", 
						this.FileName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				}

				switch(result)
				{
					case DialogResult.Yes:
						this.menuItemSave_Click(this, new EventArgs());
						break;
					case DialogResult.Cancel:
						canClose = false;
						break;
				}
			}

			return canClose;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectBrowser));
            this.toolBarToolbar = new System.Windows.Forms.ToolBar();
            this.toolBarButtonSave = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonSaveAs = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonSep1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonView = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonSep2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonExecute = new System.Windows.Forms.ToolBarButton();
            this.imageListFormIcons = new System.Windows.Forms.ImageList(this.components);
            this.treeViewProject = new System.Windows.Forms.TreeView();
            this.contextMenuTree = new System.Windows.Forms.ContextMenu();
            this.contextItemExecute = new System.Windows.Forms.MenuItem();
            this.contextItemCacheSettings = new System.Windows.Forms.MenuItem();
            this.menuItemSep01 = new System.Windows.Forms.MenuItem();
            this.contextItemAddModule = new System.Windows.Forms.MenuItem();
            this.contextItemAddSavedObject = new System.Windows.Forms.MenuItem();
            this.menuItemSep02 = new System.Windows.Forms.MenuItem();
            this.contextItemEdit = new System.Windows.Forms.MenuItem();
            this.contextItemCopy = new System.Windows.Forms.MenuItem();
            this.contextItemRemove = new System.Windows.Forms.MenuItem();
            this.menuItemSep03 = new System.Windows.Forms.MenuItem();
            this.contextItemSave = new System.Windows.Forms.MenuItem();
            this.contextItemSaveAs = new System.Windows.Forms.MenuItem();
            this.toolTipProjectBrowser = new System.Windows.Forms.ToolTip(this.components);
            this.mainMenuProject = new System.Windows.Forms.MainMenu(this.components);
            this.menuItemFile = new System.Windows.Forms.MenuItem();
            this.menuItemSave = new System.Windows.Forms.MenuItem();
            this.menuItemSaveAs = new System.Windows.Forms.MenuItem();
            this.menuItemClose = new System.Windows.Forms.MenuItem();
            this.menuItemProject = new System.Windows.Forms.MenuItem();
            this.menuItemExecute = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // toolBarToolbar
            // 
            this.toolBarToolbar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBarToolbar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButtonSave,
            this.toolBarButtonSaveAs,
            this.toolBarButtonSep1,
            this.toolBarButtonView,
            this.toolBarButtonSep2,
            this.toolBarButtonExecute});
            this.toolBarToolbar.DropDownArrows = true;
            this.toolBarToolbar.ImageList = this.imageListFormIcons;
            this.toolBarToolbar.Location = new System.Drawing.Point(0, 0);
            this.toolBarToolbar.Name = "toolBarToolbar";
            this.toolBarToolbar.ShowToolTips = true;
            this.toolBarToolbar.Size = new System.Drawing.Size(384, 28);
            this.toolBarToolbar.TabIndex = 0;
            this.toolBarToolbar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarToolbar_ButtonClick);
            // 
            // toolBarButtonSave
            // 
            this.toolBarButtonSave.ImageIndex = 6;
            this.toolBarButtonSave.Name = "toolBarButtonSave";
            // 
            // toolBarButtonSaveAs
            // 
            this.toolBarButtonSaveAs.ImageIndex = 7;
            this.toolBarButtonSaveAs.Name = "toolBarButtonSaveAs";
            // 
            // toolBarButtonSep1
            // 
            this.toolBarButtonSep1.Name = "toolBarButtonSep1";
            this.toolBarButtonSep1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonView
            // 
            this.toolBarButtonView.ImageIndex = 4;
            this.toolBarButtonView.Name = "toolBarButtonView";
            // 
            // toolBarButtonSep2
            // 
            this.toolBarButtonSep2.Name = "toolBarButtonSep2";
            this.toolBarButtonSep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonExecute
            // 
            this.toolBarButtonExecute.ImageIndex = 5;
            this.toolBarButtonExecute.Name = "toolBarButtonExecute";
            // 
            // imageListFormIcons
            // 
            this.imageListFormIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListFormIcons.ImageStream")));
            this.imageListFormIcons.TransparentColor = System.Drawing.Color.Fuchsia;
            this.imageListFormIcons.Images.SetKeyName(0, "");
            this.imageListFormIcons.Images.SetKeyName(1, "");
            this.imageListFormIcons.Images.SetKeyName(2, "");
            this.imageListFormIcons.Images.SetKeyName(3, "");
            this.imageListFormIcons.Images.SetKeyName(4, "");
            this.imageListFormIcons.Images.SetKeyName(5, "");
            this.imageListFormIcons.Images.SetKeyName(6, "");
            this.imageListFormIcons.Images.SetKeyName(7, "");
            // 
            // treeViewProject
            // 
            this.treeViewProject.ContextMenu = this.contextMenuTree;
            this.treeViewProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewProject.ImageIndex = 0;
            this.treeViewProject.ImageList = this.imageListFormIcons;
            this.treeViewProject.Location = new System.Drawing.Point(0, 28);
            this.treeViewProject.Name = "treeViewProject";
            this.treeViewProject.SelectedImageIndex = 0;
            this.treeViewProject.Size = new System.Drawing.Size(384, 514);
            this.treeViewProject.TabIndex = 1;
            this.treeViewProject.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewProject_AfterCollapse);
            this.treeViewProject.DoubleClick += new System.EventHandler(this.treeViewProject_OnDoubleClick);
            this.treeViewProject.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeViewProject_MouseMove);
            this.treeViewProject.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViewProject_KeyDown);
            this.treeViewProject.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeViewProject_AfterExpand);
            this.treeViewProject.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewProject_MouseDown);
            // 
            // contextMenuTree
            // 
            this.contextMenuTree.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.contextItemExecute,
            this.contextItemCacheSettings,
            this.menuItemSep01,
            this.contextItemAddModule,
            this.contextItemAddSavedObject,
            this.menuItemSep02,
            this.contextItemEdit,
            this.contextItemCopy,
            this.contextItemRemove,
            this.menuItemSep03,
            this.contextItemSave,
            this.contextItemSaveAs});
            this.contextMenuTree.Popup += new System.EventHandler(this.contextMenuTree_Popup);
            // 
            // contextItemExecute
            // 
            this.contextItemExecute.Index = 0;
            this.contextItemExecute.Text = "E&xecute";
            this.contextItemExecute.Click += new System.EventHandler(this.contextItemExecute_Click);
            // 
            // contextItemCacheSettings
            // 
            this.contextItemCacheSettings.Index = 1;
            this.contextItemCacheSettings.Text = "Cache &Default Settings";
            this.contextItemCacheSettings.Click += new System.EventHandler(this.contextItemCacheSettings_Click);
            // 
            // menuItemSep01
            // 
            this.menuItemSep01.Index = 2;
            this.menuItemSep01.Text = "-";
            // 
            // contextItemAddModule
            // 
            this.contextItemAddModule.Index = 3;
            this.contextItemAddModule.Text = "Add &Module";
            this.contextItemAddModule.Click += new System.EventHandler(this.contextItemAddModule_Click);
            // 
            // contextItemAddSavedObject
            // 
            this.contextItemAddSavedObject.Index = 4;
            this.contextItemAddSavedObject.Text = "Add &Template Instance";
            this.contextItemAddSavedObject.Click += new System.EventHandler(this.contextItemAddSavedObject_Click);
            // 
            // menuItemSep02
            // 
            this.menuItemSep02.Index = 5;
            this.menuItemSep02.Text = "-";
            // 
            // contextItemEdit
            // 
            this.contextItemEdit.Index = 6;
            this.contextItemEdit.Text = "&Edit";
            this.contextItemEdit.Click += new System.EventHandler(this.contextItemEdit_Click);
            // 
            // contextItemCopy
            // 
            this.contextItemCopy.Index = 7;
            this.contextItemCopy.Text = "Make &Copy";
            this.contextItemCopy.Click += new System.EventHandler(this.contextItemCopy_Click);
            // 
            // contextItemRemove
            // 
            this.contextItemRemove.Index = 8;
            this.contextItemRemove.Text = "&Remove";
            this.contextItemRemove.Click += new System.EventHandler(this.contextItemRemove_Click);
            // 
            // menuItemSep03
            // 
            this.menuItemSep03.Index = 9;
            this.menuItemSep03.Text = "-";
            // 
            // contextItemSave
            // 
            this.contextItemSave.Index = 10;
            this.contextItemSave.Text = "&Save";
            this.contextItemSave.Click += new System.EventHandler(this.contextItemSave_Click);
            // 
            // contextItemSaveAs
            // 
            this.contextItemSaveAs.Index = 11;
            this.contextItemSaveAs.Text = "Save &As";
            this.contextItemSaveAs.Click += new System.EventHandler(this.contextItemSaveAs_Click);
            // 
            // toolTipProjectBrowser
            // 
            this.toolTipProjectBrowser.AutoPopDelay = 5000;
            this.toolTipProjectBrowser.InitialDelay = 500;
            this.toolTipProjectBrowser.ReshowDelay = 100;
            // 
            // mainMenuProject
            // 
            this.mainMenuProject.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFile,
            this.menuItemProject});
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
            this.menuItemSave.Text = "Save";
            this.menuItemSave.Click += new System.EventHandler(this.menuItemSave_Click);
            // 
            // menuItemSaveAs
            // 
            this.menuItemSaveAs.Index = 1;
            this.menuItemSaveAs.Text = "Save As ...";
            this.menuItemSaveAs.Click += new System.EventHandler(this.menuItemSaveAs_Click);
            // 
            // menuItemClose
            // 
            this.menuItemClose.Index = 2;
            this.menuItemClose.Text = "&Close";
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // menuItemProject
            // 
            this.menuItemProject.Index = 1;
            this.menuItemProject.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemExecute});
            this.menuItemProject.MergeOrder = 3;
            this.menuItemProject.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            this.menuItemProject.Text = "&Project";
            // 
            // menuItemExecute
            // 
            this.menuItemExecute.Index = 0;
            this.menuItemExecute.Text = "E&xecute";
            this.menuItemExecute.Click += new System.EventHandler(this.menuItemExecute_Click);
            // 
            // ProjectBrowser
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(384, 542);
            this.Controls.Add(this.treeViewProject);
            this.Controls.Add(this.toolBarToolbar);
            this.DockAreas = ((DockAreas)(((((DockAreas.Float | DockAreas.DockLeft)
                        | DockAreas.DockRight)
                        | DockAreas.DockTop)
                        | DockAreas.DockBottom)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenuProject;
            this.Name = "ProjectBrowser";
            this.ShowHint = DockState.DockLeft;
            this.TabText = "Project Browser";
            this.Text = "Project Browser";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.ProjectBrowser_Closing);
            this.MouseLeave += new System.EventHandler(this.ProjectBrowser_MouseLeave);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		#region TreeView Event Handlers
		private void treeViewProject_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e) 
		{
			if ((e.Node is ProjectTreeNode) || (e.Node is ModuleTreeNode))
			{
				e.Node.SelectedImageIndex = INDEX_OPEN_FOLDER;
				e.Node.ImageIndex = INDEX_OPEN_FOLDER;
			}
		}

		private void treeViewProject_AfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e) 
		{
			if ((e.Node is ProjectTreeNode) || (e.Node is ModuleTreeNode))
			{
				e.Node.SelectedImageIndex = INDEX_CLOSED_FOLDER;
				e.Node.ImageIndex = INDEX_CLOSED_FOLDER;
			}
		}

		
		private void treeViewProject_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) 
		{
			TreeNode node = (TreeNode)treeViewProject.GetNodeAt(e.X, e.Y);
			treeViewProject.SelectedNode = node;
		}

		//TODO: Add keypress events
		private void treeViewProject_KeyDown(object sender, KeyEventArgs e) 
		{
			if (e.KeyCode == Keys.F5) 
			{
				//------------------
			}
		}

		private void treeViewProject_OnDoubleClick(object sender, System.EventArgs e) 
		{
			SortedProjectTreeNode parentnode;
			TreeNode node = this.treeViewProject.SelectedNode;
			if (node is SavedObjectTreeNode)
			{
				SavedTemplateInput input = node.Tag as SavedTemplateInput;
				ZeusModule parentMod = node.Parent.Tag as ZeusModule;
				this.formEditSavedObject.Module = parentMod;
				this.formEditSavedObject.SavedObject = input;
				if (this.formEditSavedObject.ShowDialog() == DialogResult.OK) 
				{
					this._isDirty = true;

					node.Text = input.SavedObjectName;
					parentnode = node.Parent as SortedProjectTreeNode;
					if (parentnode != null) 
					{
						node.Remove();
						parentnode.AddSorted(node);
						this.treeViewProject.SelectedNode = node;
					}
				}
			}
		}

        private object lastObject = null;

        private void treeViewProject_MouseMove(object sender, MouseEventArgs e)
		{
            object obj = treeViewProject.GetNodeAt(e.X, e.Y);
            if (object.Equals(obj, lastObject) || (obj == null && lastObject == null))
            {
                return;
            }
            else
            {
                if (obj is SavedObjectTreeNode)
                {
                    this.toolTipProjectBrowser.SetToolTip(treeViewProject, ((SavedObjectTreeNode)obj).SavedObject.SavedObjectName);
                }
                else if (obj is ProjectTreeNode)
                {
                    this.toolTipProjectBrowser.SetToolTip(treeViewProject, ((ProjectTreeNode)obj).Project.Description);
                }
                else if (obj is ModuleTreeNode)
                {
                    this.toolTipProjectBrowser.SetToolTip(treeViewProject, ((ModuleTreeNode)obj).Module.Description);
                }
                else
                {
                    this.toolTipProjectBrowser.SetToolTip(treeViewProject, string.Empty);
                }

                lastObject = obj;
            }
		}

		private void toolBarToolbar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			TreeNode node = this.treeViewProject.SelectedNode;
			
			if (e.Button == toolBarButtonSave)
			{
				this.Save();
			}
			else if (e.Button == toolBarButtonSaveAs)
			{
				SaveAs();
			}
			else if (e.Button == toolBarButtonExecute)
			{
				Execute();
			}
			else if (e.Button == toolBarButtonView) 
			{
				if ( (node is ProjectTreeNode) || (node is ModuleTreeNode) )
				{
					formEditModule.Module = node.Tag as ZeusModule;
					if (formEditModule.ShowDialog() == DialogResult.OK) 
					{
						this._isDirty = true;
						node.Text = formEditModule.Module.Name;
					}

					if (node is ProjectTreeNode) 
					{
						this.Text = "Project: " + formEditModule.Module.Name;
						this.TabText = formEditModule.Module.Name;
					}
				}
				else if (node is SavedObjectTreeNode)
				{
					SavedTemplateInput input = node.Tag as SavedTemplateInput;
					ZeusModule parentMod = node.Parent.Tag as ZeusModule;
					this.formEditSavedObject.Module = parentMod;
					this.formEditSavedObject.SavedObject = input;
					if (this.formEditSavedObject.ShowDialog() == DialogResult.OK) 
					{
						this._isDirty = true;
						node.Text = input.SavedObjectName;
						SortedProjectTreeNode parentnode = node.Parent as SortedProjectTreeNode;
						if (parentnode != null) 
						{
							node.Remove();
							parentnode.AddSorted(node);
						}
					}
				}
			}
		}
		#endregion

		#region Main Menu Event Handlers
		private void menuItemExecute_Click(object sender, System.EventArgs e)
		{
			this.Execute();
		}

		private void menuItemSave_Click(object sender, System.EventArgs e)
		{
			Save();
		}

		private void menuItemSaveAs_Click(object sender, System.EventArgs e)
		{
			this.SaveAs();
		}

		private void menuItemClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
		#endregion

		#region ProjectBrowser Event Handlers
		private void ProjectBrowser_MouseLeave(object sender, System.EventArgs e)
		{
			this.toolTipProjectBrowser.SetToolTip(treeViewProject, string.Empty);
		}

		private void ProjectBrowser_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!this.CanClose(true)) 
			{
				e.Cancel = true;
			}
		}
		#endregion

		#region ContextMenu Event Handlers
		private void contextMenuTree_Popup(object sender, System.EventArgs e)
		{
			this.contextItemAddModule.Visible = 
				this.contextItemAddSavedObject.Visible =
				this.contextItemEdit.Visible = 
				this.contextItemExecute.Visible = 
				this.contextItemRemove.Visible = 
				this.contextItemSave.Visible =
				this.contextItemCopy.Visible =
				this.contextItemSaveAs.Visible =  
				this.contextItemCacheSettings.Visible = 
				this.menuItemSep01.Visible =
				this.menuItemSep02.Visible = 
				this.menuItemSep03.Visible = false;

			SortedProjectTreeNode node = this.treeViewProject.SelectedNode as SortedProjectTreeNode;
			if (node is ProjectTreeNode) 
			{
				this.contextItemSave.Visible = 
					this.contextItemSaveAs.Visible = 
					this.contextItemAddModule.Visible =
					this.contextItemAddSavedObject.Visible = 
					this.contextItemEdit.Visible = 
					this.contextItemExecute.Visible = 
					this.contextItemCacheSettings.Visible = true;

				this.menuItemSep01.Visible = true;
				this.menuItemSep02.Visible = true;
				this.menuItemSep03.Visible = true;
			}
			else if (node is ModuleTreeNode)
			{
				this.contextItemAddModule.Visible = 
					this.contextItemAddSavedObject.Visible = 
					this.contextItemEdit.Visible = 
					this.contextItemExecute.Visible = 
					this.contextItemRemove.Visible = 
					this.contextItemCacheSettings.Visible = true;

				this.menuItemSep01.Visible = true;
				this.menuItemSep02.Visible = true;
				this.menuItemSep03.Visible = false;
			}
			else if (node is SavedObjectTreeNode)
			{
				this.contextItemEdit.Visible =
					this.contextItemExecute.Visible =
					this.contextItemCopy.Visible =
					this.contextItemRemove.Visible = true;

				this.menuItemSep01.Visible = true;
				this.menuItemSep02.Visible = false;
				this.menuItemSep03.Visible = false;
			}
		}

		private void contextItemEdit_Click(object sender, System.EventArgs e)
		{
			SortedProjectTreeNode node = this.treeViewProject.SelectedNode as SortedProjectTreeNode;
			SortedProjectTreeNode parentnode;
			if ( (node is ModuleTreeNode) || (node is ProjectTreeNode) ) 
			{
				ZeusModule module = node.Tag as ZeusModule;
				this.formEditModule.Module = module;
				if (this.formEditModule.ShowDialog() == DialogResult.OK) 
				{
					this._isDirty = true;
					node.Text = module.Name;
					parentnode = node.Parent as SortedProjectTreeNode;
					if (parentnode != null) 
					{
						node.Remove();
						parentnode.AddSorted(node);
						this.treeViewProject.SelectedNode = node;
					}

					if (node is ProjectTreeNode) 
					{
						this.Text = "Project: " + module.Name;
						this.TabText = module.Name;
					}
				}
			}
			else if (node is SavedObjectTreeNode)
			{
				SavedTemplateInput input = node.Tag as SavedTemplateInput;
				ZeusModule parentMod = node.Parent.Tag as ZeusModule;
				this.formEditSavedObject.Module = parentMod;
				this.formEditSavedObject.SavedObject = input;
				if (this.formEditSavedObject.ShowDialog() == DialogResult.OK) 
				{
					this._isDirty = true;
					node.Text = input.SavedObjectName;
					parentnode = node.Parent as SortedProjectTreeNode;
					if (parentnode != null) 
					{
						node.Remove();
						parentnode.AddSorted(node);
						this.treeViewProject.SelectedNode = node;
					}
				}
			}
		}

		private void contextItemAddModule_Click(object sender, System.EventArgs e)
		{
			SortedProjectTreeNode node = this.treeViewProject.SelectedNode as SortedProjectTreeNode;
			if ( (node is ModuleTreeNode) || (node is ProjectTreeNode) ) 
			{
				ZeusModule module = node.Tag as ZeusModule;

				ZeusModule newmodule = new ZeusModule();

				this.formEditModule.Module = newmodule;
				if (this.formEditModule.ShowDialog() == DialogResult.OK) 
				{
					this._isDirty = true;
					module.ChildModules.Add(newmodule);

					ModuleTreeNode newNode = new ModuleTreeNode(newmodule);
					node.AddSorted(newNode);
					node.Expand();
				}
			}
		}

		private void contextItemAddSavedObject_Click(object sender, System.EventArgs e)
		{
			SortedProjectTreeNode node = this.treeViewProject.SelectedNode as SortedProjectTreeNode;
			if ( (node is ModuleTreeNode) || (node is ProjectTreeNode) ) 
			{
				ZeusModule module = node.Tag as ZeusModule;

				SavedTemplateInput savedInput = new SavedTemplateInput();
				this.formEditSavedObject.Module = module;
				this.formEditSavedObject.SavedObject = savedInput;
				if (this.formEditSavedObject.ShowDialog() == DialogResult.OK) 
				{
					this._isDirty = true;
					module.SavedObjects.Add(savedInput);

					SavedObjectTreeNode newNode = new SavedObjectTreeNode(savedInput);
					node.AddSorted(newNode);
					node.Expand();
					this.treeViewProject.SelectedNode = newNode;
				}
			}
		}

		private void contextItemCopy_Click(object sender, System.EventArgs e)
		{
			SortedProjectTreeNode node = this.treeViewProject.SelectedNode as SortedProjectTreeNode;
			if (node is SavedObjectTreeNode)
			{
				SavedTemplateInput input = node.Tag as SavedTemplateInput;
				SavedTemplateInput copy = input.Copy();

				SortedProjectTreeNode moduleNode = node.Parent as SortedProjectTreeNode;
		
				ZeusModule module = moduleNode.Tag as ZeusModule;

				string copyName = copy.SavedObjectName;
				string newName = copyName;
				int count = 1;
				bool found;
				do
				{
					found = false;
					foreach (SavedTemplateInput tmp in module.SavedObjects) 
					{
						if (tmp.SavedObjectName == newName) 
						{ 
							found = true;
							newName = copyName + " " + count++;
							break;
						}
					}
				} while (found);

				copy.SavedObjectName = newName;

				module.SavedObjects.Add(copy);

				SavedObjectTreeNode copiedNode = new SavedObjectTreeNode(copy);
				moduleNode.AddSorted(copiedNode);

				this._isDirty = true;
			}
		}

		private void contextItemCacheSettings_Click(object sender, System.EventArgs e)
		{
			SortedProjectTreeNode node = this.treeViewProject.SelectedNode as SortedProjectTreeNode;
			if ( (node is ModuleTreeNode) || (node is ProjectTreeNode) ) 
			{
				ZeusModule module = node.Tag as ZeusModule;
				ZeusContext context = new ZeusContext();
                DefaultSettings settings = DefaultSettings.Instance;
				settings.PopulateZeusContext(context);
				module.SavedItems.Add(context.Input);
			}
		}

		private void contextItemRemove_Click(object sender, System.EventArgs e)
		{
			SortedProjectTreeNode node = this.treeViewProject.SelectedNode as SortedProjectTreeNode;
			SortedProjectTreeNode parentnode;
			if (node is ModuleTreeNode)
			{
				parentnode = node.Parent as SortedProjectTreeNode;

				ZeusModule parentmodule = parentnode.Tag as ZeusModule;
				ZeusModule module = node.Tag as ZeusModule;

				parentmodule.ChildModules.Remove(module);
				parentnode.Nodes.Remove(node);
				this._isDirty = true;
			}
			else if (node is SavedObjectTreeNode)
			{
				parentnode = node.Parent as SortedProjectTreeNode;

				ZeusModule parentmodule = parentnode.Tag as ZeusModule;
				SavedTemplateInput savedobj = node.Tag as SavedTemplateInput;

				parentmodule.SavedObjects.Remove(savedobj);
				parentnode.Nodes.Remove(node);
				this._isDirty = true;
			}
		}

		private void contextItemSave_Click(object sender, System.EventArgs e)
		{
			Save();
		}

		private void contextItemSaveAs_Click(object sender, System.EventArgs e)
		{
			SaveAs();
		}

		private void contextItemExecute_Click(object sender, System.EventArgs e)
		{
			this.Execute();
		}
		#endregion

		#region Load Project Tree
		public void CreateNewProject() 
		{
			this.treeViewProject.Nodes.Clear();

			ZeusProject proj = new ZeusProject();
			proj.Name = "New Project";
			proj.Description = "New Zeus Project file";

			rootNode = new ProjectTreeNode(proj);
			rootNode.Expand();

			this.Text = "Project: " + proj.Name;
			this.TabText = proj.Name;

			this.treeViewProject.Nodes.Add(rootNode);
		}

		public void LoadProject(string filename) 
		{
			this.treeViewProject.Nodes.Clear();

			ZeusProject proj = new ZeusProject(filename);
			if (proj.Load()) 
			{
				this.Text = "Project: " + proj.Name;
				this.TabText = proj.Name;

				rootNode = new ProjectTreeNode(proj);
					
				foreach (ZeusModule module in proj.ChildModules) 
				{
					LoadModule(rootNode, module);
				}
		
				foreach (SavedTemplateInput input in proj.SavedObjects) 
				{
					rootNode.AddSorted( new SavedObjectTreeNode(input) );
				}
			}
			rootNode.Expand();
			

			this.treeViewProject.Nodes.Add(rootNode);
		}

		private void LoadModule(SortedProjectTreeNode parentNode, ZeusModule childModule) 
		{
			ModuleTreeNode childNode = new ModuleTreeNode(childModule);
			parentNode.AddSorted(childNode);

			foreach (ZeusModule grandchildModule in childModule.ChildModules) 
			{
				LoadModule(childNode, grandchildModule);
			}
		
			foreach (SavedTemplateInput input in childModule.SavedObjects) 
			{
				childNode.AddSorted( new SavedObjectTreeNode(input) );
			}
		}
		#endregion

		#region Execute, Save, SaveAs
		public void Execute() 
		{
			Cursor.Current = Cursors.WaitCursor;

			TreeNode node = this.treeViewProject.SelectedNode;
            DefaultSettings settings = DefaultSettings.Instance;

			ProjectExecuteStatus log = new ProjectExecuteStatus();
			log.Show();

			if ((node is ModuleTreeNode) || (node is ProjectTreeNode) )
			{
				ZeusModule module = node.Tag as ZeusModule;
				module.Execute(settings.ScriptTimeout, log);
			}
			else if (node is SavedObjectTreeNode)
			{
				SavedTemplateInput savedinput = node.Tag as SavedTemplateInput;
				savedinput.Execute(settings.ScriptTimeout, log);
			}

			log.Finished = true;

			Cursor.Current = Cursors.Default;
		}

		private void Save()
		{
			if (this.rootNode.Project.FilePath != null)
			{
				_isDirty = false;

				this.rootNode.Project.Save();
			}
			else 
			{
				this.SaveAs();
			}
		}

		private void SaveAs() 
		{
			Stream myStream;
			SaveFileDialog saveFileDialog = new SaveFileDialog();
       
			saveFileDialog.Filter = "Zeus Project (*.zprj)|*.zprj|All files (*.*)|*.*";
			saveFileDialog.FilterIndex = 0;
			saveFileDialog.RestoreDirectory = true;

			ZeusProject proj = this.rootNode.Project;
			if (proj.FilePath != null) 
			{
				saveFileDialog.FileName = proj.FilePath;
			}

			if(saveFileDialog.ShowDialog() == DialogResult.OK)
			{

				myStream = saveFileDialog.OpenFile();

				if(null != myStream) 
				{
					_isDirty = false;

					myStream.Close();
					proj.FilePath = saveFileDialog.FileName;
					proj.Save();
				}
			}
		}

		#endregion

        #region IMyGenDocument Members

        public string DocumentIndentity
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IMyGenContent Members

        public ToolStrip ToolStrip
        {
            get { return null; }
        }

        public void Alert(IMyGenContent sender, string command, params object[] args)
        {
            //
        }

        #endregion
	}

	#region Project Browser Tree Node Classes
	public abstract class SortedProjectTreeNode : TreeNode 
	{
		public int AddSorted(TreeNode newnode) 
		{
			int insertIndex = -1;
			for (int i = 0; i < this.Nodes.Count; i++)
			{
				TreeNode node = this.Nodes[i];
				if (node.GetType() == newnode.GetType()) 
				{
					if (newnode.Text.CompareTo(node.Text) <= 0) 
					{
						insertIndex = i;
						break;
					}
				}
				else if (newnode is SavedObjectTreeNode) 
				{
					continue;
				}
				else 
				{
					insertIndex = i;
					break;
				}
			}

			if (insertIndex == -1) 
			{
				insertIndex = this.Nodes.Add(newnode);
			}
			else 
			{
				this.Nodes.Insert(insertIndex, newnode);
			}

			return insertIndex;
		}
	}

	public class ProjectTreeNode : SortedProjectTreeNode 
	{
		public ProjectTreeNode(ZeusProject proj) 
		{
			this.Tag = proj;

			this.Text = proj.Name;
			this.ImageIndex = 1;
			this.SelectedImageIndex = 1;
		}

		public ZeusProject Project
		{
			get 
			{
				return this.Tag as ZeusProject;
			}
		}
	}
	
	public class ModuleTreeNode : SortedProjectTreeNode 
	{
		public ModuleTreeNode(ZeusModule module) 
		{
			this.Tag = module;

			this.Text = module.Name;
			this.ImageIndex = 0;
			this.SelectedImageIndex = 0;
		}

		public ZeusModule Module
		{
			get 
			{
				return this.Tag as ZeusModule;
			}
		}
	}

	public class SavedObjectTreeNode : SortedProjectTreeNode 
	{
		public SavedObjectTreeNode(SavedTemplateInput templateInput)
		{
			this.Tag = templateInput;

			this.ForeColor = Color.Blue;
			this.Text = templateInput.SavedObjectName;
			this.ImageIndex = 2;
			this.SelectedImageIndex = 2;
		}

		public SavedTemplateInput SavedObject
		{
			get 
			{
				return this.Tag as SavedTemplateInput;
			}
		}
	}
	#endregion
}
