using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using WordGitAddin.Controls;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using OfficeTools = Microsoft.Office.Tools;
using WordTools = Microsoft.Office.Tools.Word;
using System.IO;
using GitUI.UserControls;

namespace WordGitAddin
{
    public partial class ThisAddIn
    {
        #region Constants

        /// <summary>
        /// The Add-In's maximum pane width in pixels.
        /// </summary>
        public const int ADD_IN_MAX_PANE_WIDTH = 460;

        /// <summary>
        /// The Add-In's minimum pane width in pixels.
        /// </summary>
        public const int ADD_IN_MIN_PANE_WIDTH = 266;

        /// <summary>
        /// The application name without spaces.
        /// </summary>
        public const string APP_NAME_NO_SPACES = "WordGit";

        /// <summary>
        /// The relative path of the stored connections file under the application data directory.
        /// </summary>
        public const string CONNECTIONS_FILE_RELATIVE_PATH = SETTINGS_DIRECTORY_RELATIVE_PATH + @"\connections.xml";

        /// <summary>
        /// The string representation of the Escape key.
        /// </summary>
        public const string ESCAPE_KEY = "{ESC}";

        /// <summary>
        /// The Excel major version number corresponding to Excel 2007.
        /// </summary>
        public const int WORD_2007_VERSION_NUMBER = 12;

        /// <summary>
        /// The Excel major version number corresponding to Excel 2010.
        /// </summary>
        public const int WORD_2010_VERSION_NUMBER = 14;

        /// <summary>
        /// The Excel major version number corresponding to Excel 2013.
        /// </summary>
        public const int WORD_2013_VERSION_NUMBER = 15;

        /// <summary>
        /// The Excel major version number corresponding to Excel 2016.
        /// </summary>
        public const int WORD_2016_VERSION_NUMBER = 16;

        /// <summary>
        /// The number of seconds in 1 hour.
        /// </summary>
        public const int MILLISECONDS_IN_HOUR = 3600000;

        /// <summary>
        /// The relative path of the passwords vault file under the application data directory.
        /// </summary>
        public const string PASSWORDS_VAULT_FILE_RELATIVE_PATH = SETTINGS_DIRECTORY_RELATIVE_PATH + @"\user_data.dat";

        /// <summary>
        /// The relative path of the settings directory under the application data directory.
        /// </summary>
        public const string SETTINGS_DIRECTORY_RELATIVE_PATH = @"\Daixf\WordGit";

        /// <summary>
        /// The relative path of the settings file under the application data directory.
        /// </summary>
        public const string SETTINGS_FILE_RELATIVE_PATH = SETTINGS_DIRECTORY_RELATIVE_PATH + @"\settings.config";

        #endregion


        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion


        #region Property
        public int WordVersionNumber { get; private set; }
        #endregion

        internal OfficeTools.CustomTaskPane GetOrCreateActiveCustomPane()
        {
            var activeCustomPane = ActiveCustomPane;

            // If there is no custom pane associated to the Excel Add-In in the active window, create one.
            if (activeCustomPane != null)
            {
                return activeCustomPane;
            }

            // Create a new custom task pane and initialize it.
            var wordGitPanel = new WordGitPanel();
            activeCustomPane = CustomTaskPanes.Add(wordGitPanel, "WordGit");
            activeCustomPane.VisibleChanged += CustomTaskPaneVisibleChanged;
            activeCustomPane.DockPosition = MsoCTPDockPosition.msoCTPDockPositionRight;
            activeCustomPane.Width = 500;
            activeCustomPane.DockPositionRestrict = MsoCTPDockPositionRestrict.msoCTPDockPositionRestrictNoHorizontal;
            return activeCustomPane;
        }

        private void CustomTaskPaneVisibleChanged(object sender, EventArgs e)
        {
            CustomWordGitRibbon.changeShowWordGitPanelToggleState(sender is OfficeTools.CustomTaskPane customTaskPanel && customTaskPanel.Visible);
        }


        internal void CloseWordGitPane(WordGitPanel wordAddInPane)
        {
            throw new NotImplementedException();
        }

        public OfficeTools.CustomTaskPane ActiveCustomPane
        {
            get
            {
                var addInPane = CustomTaskPanes.FirstOrDefault(ctp =>
                {
                    bool isParentWindowActiveWordWindow;
                    if (WordVersionNumber >= WORD_2013_VERSION_NUMBER)
                    {
                        // If running on Excel 2013 or later a MDI is used for the windows so the active custom pane is matched with its window and the application active window.
                        Window paneWindow = null;
                        try
                        {
                            // This assignment is intentionally inside a try block because when an Excel window has been previously closed this property (ActiveCustomPane)
                            // is called before the CustomTaskPane linked to the closed Excel window is removed from the collection, so the ctp.Window can throw an Exception.
                            // A null check is not enough.
                            paneWindow = ctp.Window as Window;
                        }
                        catch
                        {
                            // ignored
                        }

                        isParentWindowActiveWordWindow = paneWindow != null && Application.ActiveWindow != null && paneWindow.Hwnd == Application.ActiveWindow.Hwnd;
                    }
                    else
                    {
                        // If running on Excel 2007 or 2010 a SDI is used so the active custom pane is the first one of an Excel Add-In.
                        isParentWindowActiveWordWindow = true;
                    }

                    return isParentWindowActiveWordWindow && ctp.Control is WordGitPanel;
                });

                return addInPane;
            }
        }

        public WordGitRibbon CustomWordGitRibbon { get; private set; }
        /*  Override this method to provide the Microsoft Office application an implementation of the Microsoft.Office.Core.IRibbonExtensibility interface, or if you have multiple Ribbons in your project and you want to specify which Ribbons to display at run time.

            You do not have to override this method to return Ribbons that you add to the project by using the Ribbon(Visual Designer) item template.By default, this method returns a RibbonManager object that represents all Ribbon (Visual Designer) items in the project. For more information, see Ribbon Overview.

            You must override the CreateRibbonExtensibilityObject or RequestService method to return Ribbons in your project that you add by using the Ribbon(XML) item template.For more information about how to override the CreateRibbonExtensibilityObject method, see Ribbon XML.
         */
        protected override IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            CustomWordGitRibbon = new WordGitRibbon();
            return CustomWordGitRibbon;
        }

        public void WordFileDiff(FileStatusItem fileStatus)
        {
            throw new NotImplementedException(" wordFileDiff not implemented");
        }
        private bool DocConversToDocx(string filepath)
        {
            object oMissing = System.Reflection.Missing.Value;
            //  Word._Application oWord;
            Microsoft.Office.Interop.Word.Application oWord;
            Word._Document oDoc;
            oWord = new Word.Application();
            //oWord.Visible = true;
            oDoc = oWord.Documents.Open(filepath, ref oMissing, ref oMissing,
                                        ref oMissing, ref oMissing);
            // oWord.DefaultSaveFormat = Word.WdSaveFormat.wdWordDocument;
            var saveFilePath = Path.ChangeExtension(filepath, "docx");
            oWord.Documents.Save(filepath, WdOriginalFormat.wdWordDocument);
            return false;
        }
    }
}
