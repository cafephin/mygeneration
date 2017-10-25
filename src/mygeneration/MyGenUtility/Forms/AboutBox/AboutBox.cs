using System.Reflection;
using System.Windows.Forms;
using MyMeta;
using WeifenLuo.WinFormsUI.Docking;

namespace MyGeneration
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class AboutBox : Form
    {
        private const string MYGENERATION_HOMEPAGE = "https://github.com/cafephin/mygeneration";

		private ListBox _productsListBox;
        private TextBox _productInfoTextBox;
		private LinkLabel _productUrlLinkLabel;
        private AboutBoxLogo _aboutBoxLogo;

        private readonly System.Collections.Generic.Dictionary<int, IMyMetaPlugin> _myMetaPlugins = new System.Collections.Generic.Dictionary<int, IMyMetaPlugin>();
        private readonly System.Collections.Generic.Dictionary<int, IEditorManager> _editorManagerPlugins = new System.Collections.Generic.Dictionary<int, IEditorManager>();
        private readonly System.Collections.Generic.Dictionary<int, IContentManager> _contentManagerPlugins = new System.Collections.Generic.Dictionary<int, IContentManager>();
        private readonly System.Collections.Generic.Dictionary<int, ISimplePluginManager> _simplePluginManagers = new System.Collections.Generic.Dictionary<int, ISimplePluginManager>();
        
		public AboutBox()
		{
			InitializeComponent();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this._productsListBox = new System.Windows.Forms.ListBox();
            this._productInfoTextBox = new System.Windows.Forms.TextBox();
            this._productUrlLinkLabel = new System.Windows.Forms.LinkLabel();
            this._aboutBoxLogo = new MyGeneration.AboutBoxLogo();
            this.SuspendLayout();
            // 
            // _productsListBox
            // 
            this._productsListBox.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._productsListBox.ItemHeight = 16;
            this._productsListBox.Location = new System.Drawing.Point(1, 67);
            this._productsListBox.Name = "_productsListBox";
            this._productsListBox.Size = new System.Drawing.Size(449, 148);
            this._productsListBox.TabIndex = 0;
            this._productsListBox.SelectedValueChanged += new System.EventHandler(this.ProductsListBox_OnSelectedValueChanged);
            // 
            // _productInfoTextBox
            // 
            this._productInfoTextBox.AcceptsReturn = true;
            this._productInfoTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._productInfoTextBox.Location = new System.Drawing.Point(1, 247);
            this._productInfoTextBox.Multiline = true;
            this._productInfoTextBox.Name = "_productInfoTextBox";
            this._productInfoTextBox.ReadOnly = true;
            this._productInfoTextBox.Size = new System.Drawing.Size(449, 136);
            this._productInfoTextBox.TabIndex = 1;
            // 
            // lnkURL
            // 
            this._productUrlLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._productUrlLinkLabel.Location = new System.Drawing.Point(1, 218);
            this._productUrlLinkLabel.Name = "_productUrlLinkLabel";
            this._productUrlLinkLabel.Size = new System.Drawing.Size(449, 26);
            this._productUrlLinkLabel.TabIndex = 3;
            this._productUrlLinkLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._productUrlLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ProductUrlLinkLabel_OnClicked);
            // 
            // _aboutBoxLogo
            // 
            this._aboutBoxLogo.BackColor = System.Drawing.Color.White;
            this._aboutBoxLogo.Location = new System.Drawing.Point(1, 1);
            this._aboutBoxLogo.Name = "_aboutBoxLogo";
            this._aboutBoxLogo.Size = new System.Drawing.Size(449, 68);
            this._aboutBoxLogo.TabIndex = 5;
            this._aboutBoxLogo.Text = "fun1";
            // 
            // AboutBox
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(451, 382);
            this.Controls.Add(this._productUrlLinkLabel);
            this.Controls.Add(this._productInfoTextBox);
            this.Controls.Add(this._productsListBox);
            this.Controls.Add(this._aboutBoxLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About MyGeneration";
            this.Load += new System.EventHandler(this.AboutBox_OnLoading);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void AboutBox_OnLoading(object sender, System.EventArgs e)
		{
            _aboutBoxLogo.Start();
            Assembly asmblyMyGen = Assembly.GetEntryAssembly();
			Assembly asmblyZeus = Assembly.GetAssembly(typeof(Zeus.ZeusTemplate));
			Assembly asmblyPlugins = Assembly.GetAssembly(typeof(Zeus.IZeusCodeSegment));
			Assembly asmblyMyMeta = Assembly.GetAssembly(typeof(MyMeta.Database));
			Assembly asmblyScintilla = Assembly.GetAssembly(typeof(Scintilla.ScintillaControl));
			Assembly asmblyWinFormsUI = Assembly.GetAssembly(typeof(DockContent));

            _productsListBox.Items.Add("MyGeneration".PadRight(29) + asmblyMyGen.GetName().Version);
			_productsListBox.Items.Add("MyMeta".PadRight(29) + asmblyMyMeta.GetName().Version);
			_productsListBox.Items.Add("Zeus Parser".PadRight(29) + asmblyZeus.GetName().Version);
			_productsListBox.Items.Add("Plug-in Interface".PadRight(29) + asmblyPlugins.GetName().Version);
			_productsListBox.Items.Add("ScintillaNet".PadRight(29) + asmblyScintilla.GetName().Version);
			_productsListBox.Items.Add("Scintilla".PadRight(29) + "1.60");
			_productsListBox.Items.Add("DockPanel Suite".PadRight(29) + asmblyWinFormsUI.GetName().Version);
			_productsListBox.Items.Add("Npgsql".PadRight(29) + GetAssemblyVersion("Npgsql", "1.0.0.0"));
			_productsListBox.Items.Add("Firebird .Net Data Provider".PadRight(29) + GetAssemblyVersion("FirebirdSql.Data.Firebird", "1.7.1.0"));
			_productsListBox.Items.Add("System.Data.SQLite".PadRight(29) + GetAssemblyVersion("System.Data.SQLite", "1.0.38.0"));
			_productsListBox.Items.Add("VistaDB 2.0 ADO.NET Provider".PadRight(29) + "2.0.16");
			_productsListBox.Items.Add("Dnp.Utils".PadRight(29) + GetAssemblyVersion("Dnp.Utils", "1.0.0.0"));

            foreach (string pluginName in MyMeta.dbRoot.Plugins.Keys)
            {
                var plugin = dbRoot.Plugins[pluginName] as IMyMetaPlugin;
                var index = _productsListBox.Items.Add(plugin.ProviderName.PadRight(29) + plugin.GetType().Assembly.GetName().Version);
                _myMetaPlugins[index] = plugin;
            }

            foreach (var pluginName in PluginManager.ContentManagers.Keys)
            {
                IContentManager plugin = PluginManager.ContentManagers[pluginName];
                var index = _productsListBox.Items.Add(plugin.Name.PadRight(29) + plugin.GetType().Assembly.GetName().Version);
                _contentManagerPlugins[index] = plugin;
            }

            foreach (var pluginName in PluginManager.SimplePluginManagers.Keys)
            {
                ISimplePluginManager plugin = PluginManager.SimplePluginManagers[pluginName];
                var index = _productsListBox.Items.Add(plugin.Name.PadRight(29) + plugin.GetType().Assembly.GetName().Version);
                _simplePluginManagers[index] = plugin;
            }

            foreach (var pluginName in PluginManager.EditorManagers.Keys)
            {
                IEditorManager plugin = PluginManager.EditorManagers[pluginName];
                var index = _productsListBox.Items.Add(plugin.Name.PadRight(29) + plugin.GetType().Assembly.GetName().Version);
                _editorManagerPlugins[index] = plugin;
            }

			_productsListBox.SelectedIndex = 0;
		}

		private string GetAssemblyVersion(string assemblyName, string fallbackVersion) 
		{
			string assemblyVersion;
			try
            {
                var an = assemblyName;
                if (an.ToLower().EndsWith(".dll"))
                    an = an.Substring(0, an.Length - 4);
                var name = new AssemblyName(an);
                Assembly assembly = Assembly.Load(name);

				assemblyVersion = assembly.GetName().Version.ToString();
			}
			catch 
			{
				assemblyVersion = fallbackVersion;
			}
			return assemblyVersion;
		}

		private void ProductsListBox_OnSelectedValueChanged(object sender, System.EventArgs e)
		{
			var product = _productsListBox.SelectedIndex;

		    switch (product)
		    {
		        case 0:
		            _productInfoTextBox.Text = "MyGeneration combines database metadata with the power " +
		                                       "of scripting to generate stored procedures, data objects, " +
		                                       "business objects, user interfaces, and more.";
		            _productUrlLinkLabel.Text = MYGENERATION_HOMEPAGE;
		            break;
		        case 1:
		            _productInfoTextBox.Text = "MyMeta is C# COM object that serves up database meta-data with plug-in support.";
		            _productUrlLinkLabel.Text = MYGENERATION_HOMEPAGE;
		            break;
		        case 2:
		            _productInfoTextBox.Text = "The Zeus Parser contains the template parser and interpreter for MyGeneration.";
		            _productUrlLinkLabel.Text = MYGENERATION_HOMEPAGE;
		            break;
		        case 3:
		            _productInfoTextBox.Text = "The plug-in Interface contains interfaces allowing third parties to develop MyGeneration plug-ins.";
		            _productUrlLinkLabel.Text = MYGENERATION_HOMEPAGE;
		            break;

		        case 4:
		            _productInfoTextBox.Text = "ScintillaNET is an encapsulation of Scintilla for use within the .NET framework";
		            _productUrlLinkLabel.Text = "http://sourceforge.net/projects/scide/";
		            break;
		        case 5:
		            _productInfoTextBox.Text = "Scintilla is a free source code editing component. It comes with complete " +
		                                       "source code and a license that permits use in any free project or commercial product.";
		            _productUrlLinkLabel.Text = "http://www.scintilla.org";
		            break;
		        case 6:
		            _productInfoTextBox.Text = "DockPanel suite is designed to achieve docking capability for MDI forms. " +
		                                       "It can be used to develop applications with Visual Studio .Net style user interface. " +
		                                       "DockPanel suite is a 100%-native .NET Windows Forms control, written in C#.";
		            _productUrlLinkLabel.Text = "http://sourceforge.net/projects/dockpanelsuite/";
		            break;
		        case 7:
		            _productInfoTextBox.Text = "Npgsql is a .Net data provider for Postgresql. It allows any program developed " +
		                                       "for the .Net framework to access the Postgresql versions 7.x and 8.x. It is " +
		                                       "implemented in 100% C# code.";
		            _productUrlLinkLabel.Text = "http://pgfoundry.org/projects/npgsql";
		            break;
		        case 8:
		            _productInfoTextBox.Text = "The .NET Data provider/Driver is written in C# and provides a high-performance, " +
		                                       "native implementation of the Firebird API.";
		            _productUrlLinkLabel.Text = "http://www.firebirdsql.org/index.php?op=files&id=netprovider";
		            break;
		        case 9:
		            _productInfoTextBox.Text = "System.Data.SQLite is an enhanced version of the original SQLite database engine. " +
		                                       "It is a complete drop-in replacement for the original sqlite3.dll (you can even " +
		                                       "rename it to sqlite3.dll).  It has no linker dependency on the .NET runtime so it " +
		                                       "can be distributed independently of .NET, yet embedded in the binary is a complete " +
		                                       "ADO.NET 2.0 provider for full managed development. The C# provider, the very minor C " +
		                                       "code modifications to SQLite, documentation and etc were written by Robert Simpson.";
		            _productUrlLinkLabel.Text = "http://sqlite.phxsoftware.com/";
		            break;
		        case 10:
		            _productInfoTextBox.Text = "VistaDB is a true RDBMS specifically designed for .NET to give developers a robust, " +
		                                       "high-speed embedded database solution with minimal overhead.";
		            _productUrlLinkLabel.Text = "http://www.vistadb.net/";
		            break;
		        case 11:
		            _productInfoTextBox.Text = "The DnpUtils plug-in for MyGeneration by David Parsons (dnparsons). This plug-in " +
		                                       "contains all kinds of useful functionality. See the Help menu for more details.";
		            _productUrlLinkLabel.Text = MYGENERATION_HOMEPAGE;
		            break;
		        default:
		            if (_myMetaPlugins.ContainsKey(product))
		            {
		                _productInfoTextBox.Text = _myMetaPlugins[product].ProviderAuthorInfo;
		                _productUrlLinkLabel.Text = _myMetaPlugins[product].ProviderAuthorUri != null
		                                                ? _myMetaPlugins[product].ProviderAuthorUri.ToString()
		                                                : MYGENERATION_HOMEPAGE;
		            }
		            else if (_contentManagerPlugins.ContainsKey(product))
		            {
		                _productInfoTextBox.Text = _contentManagerPlugins[product].Description;
		                _productUrlLinkLabel.Text = _contentManagerPlugins[product].AuthorUri != null
		                                                ? _contentManagerPlugins[product].AuthorUri.ToString()
		                                                : MYGENERATION_HOMEPAGE;
		            }
		            else if (_editorManagerPlugins.ContainsKey(product))
		            {
		                _productInfoTextBox.Text = _editorManagerPlugins[product].Description;
		                _productUrlLinkLabel.Text = _editorManagerPlugins[product].AuthorUri != null
		                                                ? _editorManagerPlugins[product].AuthorUri.ToString()
		                                                : MYGENERATION_HOMEPAGE;
		            }
		            else if (_simplePluginManagers.ContainsKey(product))
		            {
		                _productInfoTextBox.Text = _simplePluginManagers[product].Description;
		                _productUrlLinkLabel.Text = _simplePluginManagers[product].AuthorUri != null
		                                                ? _simplePluginManagers[product].AuthorUri.ToString()
		                                                : MYGENERATION_HOMEPAGE;
		            }
		            break;
		    }
		}

		private void ProductUrlLinkLabel_OnClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			LaunchBrowser(_productUrlLinkLabel.Text);
		}

		private void LaunchBrowser(string url)
		{
			try 
			{
                Zeus.WindowsTools.LaunchBrowser(url);
			}
			catch 
			{
				Help.ShowHelp(this, url);
			}
		}
	}
}
