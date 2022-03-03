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
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using GitCommands;
using GitUIPluginInterfaces;
//using ResourceManager;
using GitExtUtils;
using GitUI;
using GitCommands.Patches;

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

        private void OpenInWord(string filename)
        {
            var activeDocument = Application.ActiveDocument;
            // var activeDocumentHost = Globals.Factory.GetVstoObject(activeDocument);
            // activeDocumentHost.BeforeSave += ActiveDocumentHost_BeforeSave;
            // Application.DocumentBeforeSave += WordApp_DocumentBeforeSave;
            if (activeDocument is not null)
            {
                activeDocument.Close();
            }
            var newDoc = Application.Documents.Open(filename);
            var newDocHost = Globals.Factory.GetVstoObject(newDoc);
            newDocHost.BeforeSave += NewDocHost_BeforeSave;
            newDocHost.BeforeClose += NewDocHost_BeforeClose;
        }
        private void SaveDocumentToFile(WordTools.Document doc, string saveFile = null)
        {
            // var Doc = sender as WordTools.Document;
            if (saveFile == null)
                saveFile = @"C:\Users\Administrator\Desktop\tmp\tmpnew.docx");
            doc.SaveAs2(saveFile);
        }
        private void NewDocHost_BeforeClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var Doc = sender as WordTools.Document;
            SaveDocumentToFile(Doc);
            e.Cancel = true;
        }

        private void NewDocHost_BeforeSave(object sender, SaveEventArgs e)
        {
            var Doc = sender as WordTools.Document;
            SaveDocumentToFile(Doc);
            e.Cancel = true;
        }

        private void OpenInWordReadOnly(string filename)
        {
            Application.Documents.Open(filename, ReadOnly: true);
        }
        private void ActiveDocumentHost_BeforeSave(object sender, SaveEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }
            else
            {
                Console.WriteLine(Application.ActiveDocument.Path);
            }
        }

        public Task FileOpenInWordAsync(string filename)
        {
            OpenInWord(filename);
            return Task.CompletedTask;
        }
        public Task WordFileDiff(FileStatusItem item)
        {
            //string defaultText = "";
            //System.Action? openWithDiffTool = null;
            //if (item?.Item.IsStatusOnly ?? false)
            //{
            //    // Present error (e.g. parsing Git)
            //    return fileViewer.ViewTextAsync(item.Item.Name, item.Item.ErrorMessage ?? "");
            //}

            //if (item?.Item is null || item.SecondRevision?.ObjectId is null)
            //{
            //    if (!string.IsNullOrWhiteSpace(defaultText))
            //    {
            //        return fileViewer.ViewTextAsync(item?.Item?.Name, defaultText);
            //    }

            //    fileViewer.Clear();
            //    return Task.CompletedTask;
            //}

            //var firstId = item.FirstRevision?.ObjectId ?? item.SecondRevision.FirstParentId;

            //openWithDiffTool ??= OpenWithDiffTool;

            //if (item.Item.IsNew || firstId is null || FileHelper.IsImage(item.Item.Name))
            //{
            //    // View blob guid from revision, or file for worktree
            //    return fileViewer.ViewGitItemRevisionAsync(item.Item, item.SecondRevision.ObjectId, openWithDiffTool);
            //}

            //if (item.Item.IsRangeDiff)
            //{
            //    // This command may take time, give an indication of what is going on
            //    // The sha are incorrect if baseA/baseB is set, to simplify the presentation
            //    fileViewer.ViewText("range-diff.sh", $"git range-diff {firstId}...{item.SecondRevision.ObjectId}");

            //    string output = fileViewer.Module.GetRangeDiff(
            //            firstId,
            //            item.SecondRevision.ObjectId,
            //            item.BaseA,
            //            item.BaseB,
            //            fileViewer.GetExtraDiffArguments(isRangeDiff: true));

            //    // Try set highlighting from first found filename
            //    var match = new Regex(@"\n\s*(@@|##)\s+(?<file>[^#:\n]+)").Match(output ?? "");
            //    var filename = match.Groups["file"].Success ? match.Groups["file"].Value : item.Item.Name;

            //    return fileViewer.ViewRangeDiffAsync(filename, output ?? defaultText);
            //}

            //string selectedPatch = GetSelectedPatch(fileViewer, firstId, item.SecondRevision.ObjectId, item.Item)
            //    ?? defaultText;

            //return item.Item.IsSubmodule
            //    ? fileViewer.ViewTextAsync(item.Item.Name, text: selectedPatch, openWithDifftool: openWithDiffTool)
            //    : fileViewer.ViewPatchAsync(item, text: selectedPatch, openWithDifftool: openWithDiffTool);

            //void OpenWithDiffTool()
            //{
            //    fileViewer.Module.OpenWithDifftool(
            //        item.Item.Name,
            //        item.Item.OldName,
            //        firstId?.ToString(),
            //        item.SecondRevision.ToString(),
            //        isTracked: item.Item.IsTracked);
            //}

            //static string? GetSelectedPatch(
            //    GitUI.Editor.FileViewer fileViewer,
            //    ObjectId firstId,
            //    ObjectId selectedId,
            //    GitItemStatus file)
            //{
            //    if (firstId == ObjectId.CombinedDiffId)
            //    {
            //        var diffOfConflict = fileViewer.Module.GetCombinedDiffContent(selectedId, file.Name,
            //            fileViewer.GetExtraDiffArguments(), fileViewer.Encoding);

            //        return string.IsNullOrWhiteSpace(diffOfConflict)
            //            ? TranslatedStrings.UninterestingDiffOmitted
            //            : diffOfConflict;
            //    }

            //    if (file.IsSubmodule)
            //    {
            //        var status = ThreadHelper.JoinableTaskFactory.Run(file.GetSubmoduleStatusAsync!);
            //        return status is not null
            //            ? LocalizationHelpers.ProcessSubmoduleStatus(fileViewer.Module, status)
            //            : $"Failed to get status for submodule \"{file.Name}\"";
            //    }

            //    var patch = GetItemPatch(fileViewer.Module, file, firstId, selectedId,
            //        fileViewer.GetExtraDiffArguments(), fileViewer.Encoding);

            //    return file.IsSubmodule
            //        ? LocalizationHelpers.ProcessSubmodulePatch(fileViewer.Module, file.Name, patch)
            //        : patch?.Text;

            //    static Patch? GetItemPatch(
            //        GitModule module,
            //        GitItemStatus file,
            //        ObjectId? firstId,
            //        ObjectId? secondId,
            //        string diffArgs,
            //        Encoding encoding)
            //    {
            //        // Files with tree guid should be presented with normal diff
            //        var isTracked = file.IsTracked || (file.TreeGuid is not null && secondId is not null);

            //        return module.GetSingleDiff(firstId, secondId, file.Name, file.OldName, diffArgs, encoding, true, isTracked);
            //    }
            //}

            Word.Application wordApp = new Word.Application();
            wordApp.DocumentBeforeSave += WordApp_DocumentBeforeSave;
            wordApp.Visible = false;
            object wordTrue = (object)true;
            object wordFalse = (object)false;
            object fileToOpen = @"C:\Temp\1.docx";
            object missing = Type.Missing;
            Word.Document doc1 = wordApp.Documents.Open(ref fileToOpen,
                   ref missing, ref wordFalse, ref wordFalse, ref missing,
                   ref missing, ref missing, ref missing, ref missing,
                   ref missing, ref missing, ref wordTrue, ref missing,
                   ref missing, ref missing, ref missing);

            object fileToOpen1 = @"C:\Temp\2.docx";
            Word.Document doc2 = wordApp.Documents.Open(ref fileToOpen1,
                   ref missing, ref wordFalse, ref wordFalse, ref missing,
                   ref missing, ref missing, ref missing, ref missing,
                   ref missing, ref missing, ref missing, ref missing,
                   ref missing, ref missing, ref missing);

            Word.Document doc = wordApp.CompareDocuments(doc1, doc2, Word.WdCompareDestination.wdCompareDestinationNew, Word.WdGranularity.wdGranularityWordLevel,
                true, true, true, true, true, true, true, true, true, true, "", true);

            doc1.Close(ref missing, ref missing, ref missing);
            doc2.Close(ref missing, ref missing, ref missing);
            wordApp.Visible = true;
            return Task.CompletedTask;
        }

        private void WordApp_DocumentBeforeSave(Word.Document Doc, ref bool SaveAsUI, ref bool Cancel)
        {
            Doc.SaveAs2(@"C:\Users\Administrator\Desktop\tmp\tmp.docx");
            SaveAsUI = false;
            Cancel = true;
            //throw new NotImplementedException();
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
