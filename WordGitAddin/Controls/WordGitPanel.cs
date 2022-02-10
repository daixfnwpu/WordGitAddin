using GitCommands;
using GitUI;
using GitUI.CommandsDialogs;
using GitUI.CommandsDialogs.SettingsDialog;
using GitUI.CommandsDialogs.SettingsDialog.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordGitAddin.Controls
{
    public partial class WordGitPanel : UserControl
    {
        public WordGitPanel()
        {
            InitializeComponent();
        }

        public void EmbedForm(Form frm)
        {
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Visible = true;
            frm.Dock = DockStyle.Fill;   // optional
            this.Controls.Add(frm);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GitUIExtensions.UISynchronizationContext = SynchronizationContext.Current;
            GitUICommands uiCommands = new GitUICommands(string.Empty);
            var commonLogic = new CommonLogic(uiCommands.Module);
            var checkSettingsLogic = new CheckSettingsLogic(commonLogic, uiCommands.Module);
            using (var checklistSettingsPage = new ChecklistSettingsPage(commonLogic, checkSettingsLogic, uiCommands.Module, null))
            {
                if (!checklistSettingsPage.CheckSettings())
                {
                    checkSettingsLogic.AutoSolveAllSettings();
                    uiCommands.StartSettingsDialog();
                }
            }
            string[] cmdArgs = Environment.GetCommandLineArgs();
            GitUICommands uCommands = new GitUICommands(GetWorkingDir(cmdArgs));

           // uCommands.StartBrowseDialog();


            var formBrowser = new FormBrowse(uCommands, "");
          //  Application.Run(form);

           // InvokeEvent(owner, PostBrowse);
            //Application.Run(formBrowser);
            EmbedForm(formBrowser);

        }


        private static string GetWorkingDir(string[] args)
        {
            string workingDir = string.Empty;
            if (args.Length >= 3)
            {
                if (Directory.Exists(args[2]))
                    workingDir = GitModule.FindGitWorkingDir(args[2]);
                else
                {
                    workingDir = Path.GetDirectoryName(args[2]);
                    workingDir = GitModule.FindGitWorkingDir(workingDir);
                }

                //Do not add this working dir to the recent repositories. It is a nice feature, but it
                //also increases the startup time
                //if (Module.ValidWorkingDir())
                //    Repositories.RepositoryHistory.AddMostRecentRepository(Module.WorkingDir);
            }

            if (args.Length <= 1 && string.IsNullOrEmpty(workingDir) && Settings.StartWithRecentWorkingDir)
            {
                if (GitModule.IsValidGitWorkingDir(Settings.RecentWorkingDir))
                    workingDir = Settings.RecentWorkingDir;
            }

            if (string.IsNullOrEmpty(workingDir))
            {
                string findWorkingDir = GitModule.FindGitWorkingDir(Directory.GetCurrentDirectory());
                if (GitModule.IsValidGitWorkingDir(findWorkingDir))
                    workingDir = findWorkingDir;
            }

            return workingDir;
        }
    }
}
