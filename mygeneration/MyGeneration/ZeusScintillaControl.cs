using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;

using Scintilla;
using Scintilla.Forms;
using Scintilla.Enums;
using Zeus;
using Zeus.Templates;

namespace MyGeneration
{
	public interface IEditControl 
	{
		string Mode { get; }
        string Language { get; }
        string Text { get; set; }
        /*string LastSearchText { get; }
        bool LastSearchIsCaseSensitive { get; }
        bool LastSearchIsRegex { get; }

        int ReplaceNext(string find, string replace, bool isCaseSensitive, bool isRegex);
        int ReplaceAll(string find, string replace, bool isCaseSensitive, bool isRegex);
        bool FindNextAndHighlight(string text, bool isCaseSensitive, bool isRegex);
        void ReplaceHighlightedText(string replaceText);*/
        void GrabFocus();
        void Activate();
	}

	/// <summary>
	/// Summary description for JScriptScintillaControl.
	/// </summary>
	public class ZeusScintillaControl : ScintillaControl, IEditControl
	{
        private const int WM_KEYDOWN = 0x0100;
        private const int DEFAULT_CODE_PAGE = 65001;

        private string _language;
        private string _mode;
        private string _fontFamilyOverride;
        private int _codePageOverride = DEFAULT_CODE_PAGE;

		private static string LastSearchTextStatic = string.Empty;
		private static bool LastSearchIsCaseSensitiveStatic = true;
		private static bool LastSearchIsRegexStatic = false;
        private Hashtable ignoredKeys = new Hashtable();
        
        public static FindForm FindDialog = new FindForm();
        public static ReplaceForm ReplaceDialog = new ReplaceForm();

		public ZeusScintillaControl() : base() 
		{
            Scintilla.Forms.SearchHelper.Translations.MessageBoxTitle = "MyGeneration";
            this.SmartIndentingEnabled = true;
            this.GotFocus += new EventHandler(ZeusScintillaControl_GotFocus);
        }

        void ZeusScintillaControl_GotFocus(object sender, EventArgs e)
        {
            InitializeFindReplace();
        }

        public void InitializeFindReplace()
        {
            FindDialog.Initialize(this);
            ReplaceDialog.Initialize(this);
        }

        #region Adding Shortcuts from Form
        public override bool PreProcessMessage(ref Message msg)
        {
            switch (msg.Msg)
            {
                case WM_KEYDOWN:
                {
                    if (ignoredKeys.ContainsKey((int)Control.ModifierKeys + (int)msg.WParam))
                        return base.PreProcessMessage(ref msg);
                }
                break;
            }
            return false;
        }

        protected virtual void addShortcuts(Menu m)
        {
            foreach (MenuItem mi in m.MenuItems)
            {
                if (mi.Shortcut != Shortcut.None)
                    AddIgnoredKey(mi.Shortcut);
                if (mi.MenuItems.Count > 0)
                    addShortcuts(mi);
            }
        }

        public virtual void AddShortcutsFromForm(Form parentForm)
        {
            if ((parentForm != null) && (parentForm.Menu != null))
            {
                addShortcuts(parentForm.Menu);
            }
        }

        public virtual void AddIgnoredKey(Shortcut shortcutkey)
        {
            int key = (int)shortcutkey;
            this.ignoredKeys.Add(key, key);
        }

        public virtual void AddIgnoredKey(System.Windows.Forms.Keys key, System.Windows.Forms.Keys modifier)
        {
            this.ignoredKeys.Add((int)key + (int)modifier, (int)key + (int)modifier);
        }
#endregion

        private void ZeusScintillaControl_MarginClick(object sender, MarginClickEventArgs e)
        {
            /*if (margin == 2)
            {
                int line = LineFromPosition();
                sender.ToggleFold(line);
            }*/
        }


		/*private void ScintillaControlMarginClick(ScintillaControl sender, int modifiers, int position, int margin)
		{
			if(margin == 2)
			{
				int line = LineFromPosition(position);    
				sender.ToggleFold(line);
			}
		}*/

		private void EnableFolding()
		{
//			//============== folding options....
//			this.Property("fold", "1");
//			this.Property("fold.compact", "0");
//			this.Property("fold.comment", "1");
//			this.Property("fold.html", "1");
//
//			//============== control properties
//			this.SetMarginTypeN(2, (int) Scintilla.Enums.MarginType.symbol);
//			this.MarginWidthN(2, 20);
//		    this.MarginSensitiveN(2, true);
//
//			this.SetMarginMaskN(2, -33554431);
//			this.MarkerDefine((int) MarkerOutline.folderopen, MarkerSymbol.boxminus);
//			this.MarkerDefine((int) MarkerOutline.folder, MarkerSymbol.boxplus);
//			this.MarkerDefine((int) MarkerOutline.foldersub, MarkerSymbol.vline);
//			this.MarkerDefine((int) MarkerOutline.foldertail, MarkerSymbol.lcorner);
//			this.MarkerDefine((int) MarkerOutline.folderend, MarkerSymbol.boxplusconnected);
//			this.MarkerDefine((int) MarkerOutline.folderopenmid, MarkerSymbol.boxminusconnected);
//			this.MarkerDefine((int) MarkerOutline.foldermidtail,MarkerSymbol.tcorner);
        }


        public string FontFamilyOverride
        {
            get
            {
                return this._fontFamilyOverride;
            }
            set
            {
                if (value != _fontFamilyOverride)
                {
                    _fontFamilyOverride = value;
                    UpdateCurrentStyles();
                }
                
            }
        }

        public int CodePageOverride
        {
            get
            {
                return this._codePageOverride;
            }
            set
            {
                int tmp = (value == -1) ? DEFAULT_CODE_PAGE : value;
                if (tmp != _codePageOverride)
                {
                    _codePageOverride = tmp;
                    UpdateCurrentStyles();
                }
            }
        }


        public string Mode
        {
            get
            {
                return this._mode;
            }
        }

		public string Language 
		{
			get 
			{
				return this._language;
			}
		}

		public string LastSearchText { get { return ZeusScintillaControl.LastSearchTextStatic; } }
		public bool LastSearchIsCaseSensitive { get { return ZeusScintillaControl.LastSearchIsCaseSensitiveStatic; } }
		public bool LastSearchIsRegex { get { return ZeusScintillaControl.LastSearchIsRegexStatic; } }

		public override string Text
		{
			get
			{
                return base.Text;
			}
			set
			{
				if ((value != null) && (value.Length > 0))
					base.Text = value;
			}
		}

        public void SpecialRefresh()
        {
            UpdateCurrentStyles();
        }

        private void UpdateCurrentStyles()
        {
            UpdateModeAndLanguage(_language, _mode);
        }
		public void UpdateModeAndLanguage(string language, string mode) 
		{
			this.StyleClearAll();
			this.StyleResetDefault();
			this.ClearDocumentStyle();
		
			this._language = language;
			this._mode     = mode;			

			switch (this._language)
			{
				case ZeusConstants.Languages.VB:
				case ZeusConstants.Languages.VBNET:
					if (this.Mode == ZeusConstants.Modes.MARKUP) this.ConfigureControlForTaggedVBNet();
					else this.ConfigureControlForVBNet();
					break;

				case ZeusConstants.Languages.JSCRIPT:
				case ZeusConstants.Languages.JAVA:
					if (this.Mode == ZeusConstants.Modes.MARKUP) this.ConfigureControlForTaggedJScript();
					else this.ConfigureControlForJScript();
					break;

				case ZeusConstants.Languages.VBSCRIPT:
					if (this.Mode == ZeusConstants.Modes.MARKUP) this.ConfigureControlForTaggedVBScript();
					else this.ConfigureControlForVBScript();
					break;

				case ZeusConstants.Languages.CSHARP:
					if (this.Mode == ZeusConstants.Modes.MARKUP) this.ConfigureControlForTaggedCSharp();
					else this.ConfigureControlForCSharp();
					break;

				case ZeusConstants.Languages.TSQL:	
					this.ConfigureControlForMSSQL();
					break;

				case ZeusConstants.Languages.SQL:
				case ZeusConstants.Languages.PLSQL:
					this.ConfigureControlForSQL();
					break;

				case ZeusConstants.Languages.JETSQL:
					this.ConfigureControlForJetSQL();
					break;

				case ZeusConstants.Languages.PHP:
					this.ConfigureControlForPHP();
					break;

				default:
					this.ConfigureControlForUnsupported();
					break;
			}

			DefaultSettings settings = new DefaultSettings();
			this.TabWidth = settings.Tabs;

            if (!string.IsNullOrEmpty(this.FontFamilyOverride))
            {
                Font f = new Font(this.FontFamilyOverride, this.Font.Size);
                for (int i = 0; i < 128; i++)
                {
                    this.StyleSetFont(i, f.Name);
                }
            }
            this.CodePage = this.CodePageOverride;

			this.Colorize(0, -1);
            //this.Colourise(0, -1);
            
			this.Refresh();
		}

        public void Activate()
        {
            this.InitializeFindReplace();
            this.GrabFocus();
            this.Focus();
        }

		#region Find/Replace stuff
		/*public int ReplaceNext(string find, string replace, bool isCaseSensitive, bool isRegex) 
		{
            int i = 0;
			if (FindNextAndHighlight(find, isCaseSensitive, isRegex)) 
			{
				ReplaceHighlightedText(replace);
				++i;
			}
			return i;
		}
		
		public int ReplaceAll(string find, string replace, bool isCaseSensitive, bool isRegex) 
		{
			int i = 0;
			int savedPos = this.CurrentPos;

			this.CurrentPos = 0;
			while (FindNextAndHighlight(find, isCaseSensitive, isRegex)) 
			{
				ReplaceHighlightedText(replace);
				++i;
			}

			this.CurrentPos = savedPos;
			this.SelectionStart = savedPos;
			this.SelectionEnd = savedPos;
			return i;
		}
		
		public bool FindNextAndHighlight(string find, bool isCaseSensitive, bool isRegex) 
		{
			LastSearchIsRegexStatic = isRegex;
			if (isRegex) 
			{
				return FindNextAndHighlightRegex(find, isCaseSensitive);
			}
			else 
			{
				return FindNextAndHighlight(find, isCaseSensitive);
			}
		}

		public void ReplaceHighlightedText(string replaceText) 
		{
            if (GetSelectedText().Length > 0) 
			{
				int startPos = this.SelectionStart;
				this.ReplaceSelection(replaceText);

				CurrentPos = startPos + replaceText.Length;
				SelectionStart = CurrentPos;
				SelectionEnd   = CurrentPos;
			}
		}

		private bool FindNextAndHighlight(string find, bool isCaseSensitive)
		{
			if ((find == null) || (find == string.Empty)) return false;

			LastSearchTextStatic = find;
			LastSearchIsCaseSensitiveStatic = isCaseSensitive;

			int x = -1, y = -1;
			int visline = this.FirstVisibleLine;;
			int curLineIndex = this.LineFromPosition( this.CurrentPos );
			int offset = this.PositionFromLine(curLineIndex);
			int origPos = this.CurrentPos;

			offset = CurrentPos - offset;

			for (int i = curLineIndex;  i < this.LineCount; i++) 
			{
				this.GotoLine(i);
                string line = this.GetCurLine();// (this.LineLength(i)); //(this.PositionFromLine(i), this.LineLength(i));
				int index = -1;
				if (isCaseSensitive) 
				{
					index = line.IndexOf(find, offset);
				}
				else 
				{
					index = line.ToLower().IndexOf(find.ToLower(), offset);
				}

				if (index >= 0) 
				{
					if ( !((curLineIndex == i) && (index < offset)) )
					{
						x = index;
						y = i;
						break;
					}
				}
				offset = 0;
			}

			if ((x >= 0) && (y >= 0))
			{
                this.GrabFocus();
				this.GotoLine(y);
				this.EnsureVisibleEnforcePolicy(y);

				int selStart = this.CurrentPos + x;
				int selEnd = selStart + find.Length;

				this.CurrentPos = selEnd;
				this.SelectionStart = selStart;
				this.SelectionEnd   = selEnd;

				return true;
			}
			else 
			{
                this.GrabFocus();
				this.GotoLine(curLineIndex);
				this.EnsureVisibleEnforcePolicy(visline);
				
				this.CurrentPos		= origPos;
				this.SelectionStart = origPos;
				this.SelectionEnd   = origPos;

				return false;
			}
		}

		private bool FindNextAndHighlightRegex(string regexp, bool isCaseSensitive) 
		{
			if ((regexp== null) || (regexp == string.Empty)) return false;

			LastSearchTextStatic = regexp;
			LastSearchIsCaseSensitiveStatic = isCaseSensitive;
	
			try 
			{
				RegexOptions reFlags = RegexOptions.Singleline | RegexOptions.Compiled;
				if (isCaseSensitive) reFlags |= RegexOptions.IgnoreCase;
				Regex re = new Regex(regexp, reFlags);

				int x = -1, y = -1;
				int visline = this.FirstVisibleLine;
				int curLineIndex = this.LineFromPosition( this.CurrentPos );
				int offset = this.PositionFromLine(curLineIndex);
				int origPos = this.CurrentPos;
				int matchLen = -1;

				offset = CurrentPos - offset;

				for (int i = curLineIndex;  i < this.LineCount; i++) 
				{
					this.GotoLine(i);
					string line = this.GetCurLine();//this.LineLength(i)); //(this.PositionFromLine(i), this.LineLength(i));
					int index = -1;
					
					Match match = re.Match(line, offset);
					if (match.Success) 
					{
						index = match.Index;
						matchLen = match.Length;
					}

					if (index >= 0) 
					{
						if ( !((curLineIndex == i) && (index < offset)) )
						{
							x = index;
							y = i;
							break;
						}
					}
					offset = 0;
				}

				if ((x >= 0) && (y >= 0))
				{
                    this.GrabFocus();
					this.GotoLine(y);
					this.EnsureVisibleEnforcePolicy(y);

					int selStart = this.CurrentPos + x;
					int selEnd = selStart + matchLen;

					this.CurrentPos = selEnd;
					this.SelectionStart = selStart;
					this.SelectionEnd   = selEnd;

					return true;
				}
				else 
				{
                    this.GrabFocus();
					this.GotoLine(curLineIndex);
					this.EnsureVisibleEnforcePolicy(visline);
				
					this.CurrentPos		= origPos;
					this.SelectionStart = origPos;
					this.SelectionEnd   = origPos;

					return false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Regex Error!");
				return false;
			}
		}*/
		#endregion
/*
		#region Not-so-smart indenting support
		public void ZeusScintillaControl_CharAdded(ScintillaControl ctrl, int ch)
		{
			if(ch == '\n')
			{
				int curLine = this.CurrentPos;
				curLine = this.LineFromPosition(curLine);

				int previousIndent = this.GetLineIndentation(curLine-1);
				this.IndentLine(curLine, previousIndent);
			}
		}

		public void IndentLine(int line, int indent)
		{
			if (indent < 0)
			{
				return;
			}

			int selStart = this.SelectionStart;
			int selEnd = this.SelectionEnd;

			int posBefore = this.LineIndentPosition(line);
			this.SetLineIndentation(line, indent);
			int posAfter = LineIndentPosition(line);
			int posDifference = posAfter - posBefore;

			if (posAfter > posBefore)
			{
				// Move selection on
				if (selStart >= posBefore)
				{
					selStart += posDifference;
				}

				if (selEnd >= posBefore)
				{
					selEnd += posDifference;
				}
			}
			else if (posAfter < posBefore)
			{
				// Move selection back
				if (selStart >= posAfter)
				{
					if (selStart >= posBefore)
						selStart += posDifference;
					else
						selStart = posAfter;
				}
				if (selEnd >= posAfter)
				{
					if (selEnd >= posBefore)
						selEnd += posDifference;
					else
						selEnd = posAfter;
				}
			}

			this.SetSel(selStart, selEnd);
		}
		#endregion
*/
		#region Base Scintilla Settings
		protected void ConfigureBaseSettings() 
		{
			//SC_CP_UTF8 (65001)
			this.CodePage = this.CodePageOverride;

			this.Anchor_ = 0;
	//		this.BackSpaceUnIndents = false;
	//		this.BufferedDraw = true;
	//		this.CaretForeground = 0;
	//		this.CaretLineBackground = 65535;
	//		this.CaretLineVisible = false;
			this.CaretPeriod = 500;
			this.CaretWidth = 1;
			this.LegacyConfiguration = null;
			this.LegacyConfigurationLanguage = "";
			this.ControlCharSymbol = 0;
			this.CurrentPos = 0;
			this.Cursor_ = -1;
			this.Dock = System.Windows.Forms.DockStyle.Fill;
			//this.EdgeColour = 12632256;
            this.EdgeColor = 12632256;
			this.EdgeColumn = 0;
			this.EdgeMode = 0;
	//		this.EndAtLastLine = true;
	//		this.EOLCharactersVisible = false;
            this.EndOfLineMode = EndOfLine.Crlf;
	//		this.focus = false;
			this.HighlightGuide = 0;
	//		this.HorizontalScrollBarVisible = true;
			this.Indent = 0;
	//		this.IndentationGuidesVisible = false;
			this.LayoutCache = 1;
			this.Lexer = 0;
	//		this.LexerLanguage = null;
			this.Location = new System.Drawing.Point(0, 0);
			this.MarginLeft = 1;
			this.MarginRight = 1;
			this.ModEventMask = 3959;
	//		this.MouseDownCaptures = true;
			this.MouseDwellTime = 10000000;
			this.Name = "scintillaControl";
	//		this.Overtype = false;
			this.PrintColorMode = 0;
			this.PrintMagnification = 0;
			this.PrintWrapMode = 1;
	//		this.ReadOnly = false;
			this.ScrollWidth = 2000;
			this.SearchFlags = 0;
			this.SelectionEnd = 0;
			this.SelectionStart = 0;
			this.Size = new System.Drawing.Size(736, 721);
			this.Status = 0;
			this.StyleBits = 5;
	//		this.TabIndents = true;
			this.TabIndex = 0;
			this.TabWidth = 8;
			this.TargetEnd = 0;
			this.TargetStart = 0;
	//		this.UsePalette = false;
	//		this.UseTabs = true;
	//		this.VerticalScrollBarVisible = true;
	//		this.WhitespaceVisibleState = 0;
			this.WrapMode = 0;
			this.XOffset = 0;
			this.Zoom = 0;
			this.Visible = true;
			this.Width = 200;
			this.Height = 200;
			this.Dock = DockStyle.Fill;
	//		this.TabIndents = true;

			this.SetSelectionBackground(true, 65535);  // yellow
			this.SetSelectionForeground(true, 0);
		}
		#endregion
		
		#region Default Control Setup
		protected void ConfigureControlForUnsupported() 
		{
			this.ConfigureBaseSettings();
		}
		#endregion

		#region C# Setup
		protected void ConfigureControlForCSharp()
		{
			this.ConfigureBaseSettings();

			this.LegacyConfiguration = MDIParent.scintillaXmlConfig.scintilla;
			this.LegacyConfigurationLanguage = "C#";

			this.EnableFolding();
		}
		#endregion

		#region VBNet Setup
		protected void ConfigureControlForVBNet()
		{
			this.ConfigureBaseSettings();

			this.LegacyConfiguration = MDIParent.scintillaXmlConfig.scintilla;
			this.LegacyConfigurationLanguage = "VB.Net";

			this.CaretFore = 0xffffff;

		//	this.EnableFolding();
		}
		#endregion

		#region PHP Setup
		protected void ConfigureControlForPHP()
		{
			this.ConfigureBaseSettings();

			this.LegacyConfiguration = MDIParent.scintillaXmlConfig.scintilla;
			this.LegacyConfigurationLanguage = "PHP";

		//	this.EnableFolding();
		}
		#endregion
		
		#region JScript Setup
		protected void ConfigureControlForJScript()
		{
			this.ConfigureBaseSettings();

			this.LegacyConfiguration = MDIParent.scintillaXmlConfig.scintilla;
			this.LegacyConfigurationLanguage = "JScript";

			this.EnableFolding();
		}
		#endregion

		#region VBScript Setup
		protected void ConfigureControlForVBScript()
		{
			this.ConfigureBaseSettings();

			this.LegacyConfiguration = MDIParent.scintillaXmlConfig.scintilla;
			this.LegacyConfigurationLanguage = "VBScript";

			this.EnableFolding();
		}
		#endregion

		#region MSSQL Setup
		protected void ConfigureControlForMSSQL()
		{
			this.ConfigureBaseSettings();
			this.LegacyConfiguration = MDIParent.scintillaXmlConfig.scintilla;
			this.LegacyConfigurationLanguage = "MSSQL";
		}
		#endregion
		
		#region SQL Setup
		protected void ConfigureControlForSQL()
		{
			this.ConfigureBaseSettings();
			this.LegacyConfiguration = MDIParent.scintillaXmlConfig.scintilla;
			this.LegacyConfigurationLanguage = "SQL";
		}
		#endregion

		#region JetSQL Setup
		protected void ConfigureControlForJetSQL()
		{
			this.ConfigureBaseSettings();
			this.LegacyConfiguration = MDIParent.scintillaXmlConfig.scintilla;
			this.LegacyConfigurationLanguage = "JetSQL";
		}
		#endregion

		#region Tagged C# Setup
		protected void ConfigureControlForTaggedCSharp()
		{
			this.ConfigureBaseSettings();

			this.LegacyConfiguration = MDIParent.scintillaXmlConfig.scintilla;
			this.LegacyConfigurationLanguage = "TaggedC#";
		}
		#endregion

		#region Tagged JScript Setup
		protected void ConfigureControlForTaggedJScript()
		{
			this.ConfigureBaseSettings();

			this.LegacyConfiguration = MDIParent.scintillaXmlConfig.scintilla;
			this.LegacyConfigurationLanguage = "TaggedJScript";
		}
		#endregion

		#region Tagged VBScript Setup
		protected void ConfigureControlForTaggedVBScript()
		{
			this.ConfigureBaseSettings();

			this.LegacyConfiguration = MDIParent.scintillaXmlConfig.scintilla;
			this.LegacyConfigurationLanguage = "TaggedVBScript";

			this.CaretFore = 0x000000;
		}
		#endregion

		#region Tagged VBNet Setup
		protected void ConfigureControlForTaggedVBNet()
		{
			this.ConfigureBaseSettings();

			this.LegacyConfiguration = MDIParent.scintillaXmlConfig.scintilla;
			this.LegacyConfigurationLanguage = "TaggedVB.Net";

			this.CaretFore = 0x000000;
		}
		#endregion
	}
}
