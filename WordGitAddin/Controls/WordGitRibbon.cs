using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using WordGitAddin.Properties;
using Office = Microsoft.Office.Core;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new WordGitRibbon();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace WordGitAddin.Controls
{
    [ComVisible(true)]
    public class WordGitRibbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;
        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        public bool ShowWordGitPaneTogglePressed { get; private set; }

        public WordGitRibbon()
        {
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("WordGitAddin.Controls.WordGitRibbon.xml");
        }

        #endregion


        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }


        public bool GetButtonPressed(Office.IRibbonControl control)
        {
            switch (control.Id)
            {
                case "ShowWordGitPane":
                    return ShowWordGitPaneTogglePressed;
            }
            return false;
        }
        public object GetControlImage(Office.IRibbonControl control)
        {
            switch (control.Id)
            {
                case "ShowWordGitPane":
                    return Resources.MySQLforExcel_Logo_48x48; 
            }

            return null;
        }

        /// <summary>
        /// Callback method specified within the onAction attribute of a ribbon control declared in the Ribbon.xml.
        /// </summary>
        /// <param name="control">A ribbon control.</param>
        /// <param name="buttonPressed">Flag indicating whether the toggle button is depressed.</param>
        public void OnClickWordGit(Office.IRibbonControl control, bool buttonPressed)
        {
            ShowWordGitPaneTogglePressed = buttonPressed;
            var taskPane = Globals.ThisAddIn.GetOrCreateActiveCustomPane();
            if (taskPane == null)
            {
                log.Error(string.Format(Resources.CustomTaskPaneGetOrCreateError, Globals.ThisAddIn.WordVersionNumber));
                return;
            }

            taskPane.Visible = buttonPressed;
            if (!buttonPressed)
            {
                Globals.ThisAddIn.CloseWordGitPane(taskPane.Control as WordGitPanel);
            }
        }



        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion


        internal void changeShowWordGitPanelToggleState(bool pressed)
        {
            ShowWordGitPaneTogglePressed = pressed;
            ribbon.InvalidateControl("ShowWordGitPane");
        }
    }
}
