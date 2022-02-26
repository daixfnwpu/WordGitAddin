using System;
using System.Drawing;
using System.Windows.Forms;

namespace GitUI.CommandsDialogs
{
    partial class WordGitForm
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
            this.ToolStripMain = new GitUI.ToolStripEx();
            this._NO_TRANSLATE_WorkingDir = new System.Windows.Forms.ToolStripSplitButton();
            this.branchSelect = new System.Windows.Forms.ToolStripSplitButton();
            this.MainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.repoObjectsTree = new GitUI.BranchTreePanel.RepoObjectsTree();
            this.RightSplitContainer = new System.Windows.Forms.SplitContainer();
            this.RevisionsSplitContainer = new System.Windows.Forms.ContainerControl();
            this.RevisionGridContainer = new System.Windows.Forms.Panel();
            this.RevisionGrid = new GitUI.RevisionGridControl();
            this.notificationBarBisectInProgress = new GitUI.UserControls.InteractiveGitActionControl();
            this.notificationBarGitActionInProgress = new GitUI.UserControls.InteractiveGitActionControl();
            this.CommitInfoTabControl = new GitUI.CommandsDialogs.FullBleedTabControl();
            this.TreeTabPage = new System.Windows.Forms.TabPage();
            this.fileTree = new GitUI.CommandsDialogs.RevisionFileTreeControl();
            this.DiffTabPage = new System.Windows.Forms.TabPage();
            this.tsbShowRepos = new System.Windows.Forms.ToolStripButton();
            this.revisionDiff = new GitUI.CommandsDialogs.RevisionDiffControl();
            this.revisionGpgInfo1 = new GitUI.CommandsDialogs.RevisionGpgInfoControl();
            this.FilterToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFavouriteRepositories = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRecentRepositories = new System.Windows.Forms.ToolStripMenuItem();
            this.clearRecentRepositoriesListToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRecentRepositoriesClear = new System.Windows.Forms.ToolStripMenuItem();
            this.checkoutBranchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            applyPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            archiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            bisectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            checkoutBranchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            checkoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            cherryPickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            cleanupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            cloneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            commitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            branchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deleteBranchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deleteTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            formatPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            initNewRepositoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mergeBranchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pullToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pushToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            rebaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            runMergetoolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            stashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            patchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            undoLastCommitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator45 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemReflog = new System.Windows.Forms.ToolStripMenuItem();
            tsmiFavouriteRepositories = new System.Windows.Forms.ToolStripMenuItem();


            this.gitItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gitRevisionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.toolPanel = new System.Windows.Forms.ToolStripContainer();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.commandsToolStripMenuItem = new System.Windows.Forms.ToolStripSplitButton();
            this.ToolStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).BeginInit();
            this.MainSplitContainer.Panel1.SuspendLayout();
            this.MainSplitContainer.Panel2.SuspendLayout();
            this.MainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RightSplitContainer)).BeginInit();
            this.RightSplitContainer.Panel1.SuspendLayout();
            this.RightSplitContainer.Panel2.SuspendLayout();
            this.RightSplitContainer.SuspendLayout();
            this.RevisionsSplitContainer.SuspendLayout();
            this.RevisionsSplitContainer.SuspendLayout();
            this.RevisionGridContainer.SuspendLayout();
            this.CommitInfoTabControl.SuspendLayout();
            this.TreeTabPage.SuspendLayout();
            this.DiffTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gitItemBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gitRevisionBindingSource)).BeginInit();
            this.toolPanel.ContentPanel.SuspendLayout();
            this.toolPanel.TopToolStripPanel.SuspendLayout();
            this.toolPanel.SuspendLayout();
            this.SuspendLayout();
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(122, 22);
            toolStripMenuItem2.Text = "...";
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new System.Drawing.Size(83, 22);
            toolStripMenuItem4.Text = "...";
            this.ToolStripMain.ClickThrough = true;
            this.ToolStripMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.ToolStripMain.DrawBorder = false;
            this.ToolStripMain.GripMargin = new System.Windows.Forms.Padding(0);
            this.ToolStripMain.GripStyle = ToolStripGripStyle.Hidden;
            this.ToolStripMain.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tsbShowRepos.CheckOnClick = true;
            this.tsbShowRepos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbShowRepos.Image = global::GitUI.Properties.Images.BranchLocalRoot;
            this.tsbShowRepos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbShowRepos.Name = "tsbShowRepos";
            this.tsbShowRepos.Size = new System.Drawing.Size(23, 22);
            this.tsbShowRepos.Text = "&Repo";
            this.tsbShowRepos.Click += new System.EventHandler(this.tsbShowRepos_Click);
            this.ToolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._NO_TRANSLATE_WorkingDir,
            this.branchSelect,
            this.tsbShowRepos,
            this.commandsToolStripMenuItem
            });
            this.ToolStripMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.ToolStripMain.Location = new System.Drawing.Point(3, 0);
            this.ToolStripMain.Name = "ToolStripMain";
            this.ToolStripMain.Padding = new System.Windows.Forms.Padding(0);
            this.ToolStripMain.Size = new System.Drawing.Size(479, 25);
            this.ToolStripMain.TabIndex = 0;
            this.ToolStripMain.Text = "Standard";
            this._NO_TRANSLATE_WorkingDir.Image = global::GitUI.Properties.Resources.RepoOpen;
            this._NO_TRANSLATE_WorkingDir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._NO_TRANSLATE_WorkingDir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._NO_TRANSLATE_WorkingDir.Name = "_NO_TRANSLATE_WorkingDir";
            this._NO_TRANSLATE_WorkingDir.Size = new System.Drawing.Size(83, 22);
            this._NO_TRANSLATE_WorkingDir.Text = "WorkingDir";
            this._NO_TRANSLATE_WorkingDir.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._NO_TRANSLATE_WorkingDir.ToolTipText = "Change working directory";
            this._NO_TRANSLATE_WorkingDir.ButtonClick += new System.EventHandler(this.WorkingDirClick);
            this._NO_TRANSLATE_WorkingDir.DropDownOpening += new System.EventHandler(this.WorkingDirDropDownOpening);
            this._NO_TRANSLATE_WorkingDir.MouseUp += new System.Windows.Forms.MouseEventHandler(this._NO_TRANSLATE_WorkingDir_MouseUp);
            this.branchSelect.Image = global::GitUI.Properties.Resources.branch;
            this.branchSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.branchSelect.Name = "branchSelect";
            this.branchSelect.Size = new System.Drawing.Size(60, 22);
            this.branchSelect.Text = "Branch";
            this.branchSelect.ToolTipText = "Change current branch";
            this.branchSelect.ButtonClick += new System.EventHandler(this.CurrentBranchClick);
            this.branchSelect.DropDownOpening += new System.EventHandler(this.CurrentBranchDropDownOpening);
            this.branchSelect.MouseUp += new System.Windows.Forms.MouseEventHandler(this.branchSelect_MouseUp);
            this.MainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.MainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.MainSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.MainSplitContainer.Name = "MainSplitContainer";
            this.MainSplitContainer.Panel1.Controls.Add(this.repoObjectsTree);
            this.MainSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(1);
            this.MainSplitContainer.Panel1MinSize = 192;
            this.MainSplitContainer.Panel2.Controls.Add(this.RightSplitContainer);
            this.MainSplitContainer.Size = new System.Drawing.Size(923, 502);
            this.MainSplitContainer.SplitterWidth = 6;
            this.MainSplitContainer.TabIndex = 1;
            this.repoObjectsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.repoObjectsTree.Location = new System.Drawing.Point(0, 0);
            this.repoObjectsTree.MinimumSize = new System.Drawing.Size(190, 0);
            this.repoObjectsTree.Margin = new System.Windows.Forms.Padding(0);
            this.repoObjectsTree.Name = "repoObjectsTree";
            this.repoObjectsTree.Size = new System.Drawing.Size(267, 502);
            this.repoObjectsTree.TabIndex = 0;
            this.RightSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.RightSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.RightSplitContainer.Name = "RightSplitContainer";
            this.RightSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightSplitContainer.Panel1.Controls.Add(this.RevisionsSplitContainer);
            this.RightSplitContainer.Panel2.Controls.Add(this.CommitInfoTabControl);
            this.RightSplitContainer.Panel2MinSize = 200;
            this.RightSplitContainer.Size = new System.Drawing.Size(650, 502);
            this.RightSplitContainer.SplitterDistance = 209;
            this.RightSplitContainer.SplitterWidth = 6;
            this.RightSplitContainer.TabIndex = 1;
            this.RightSplitContainer.TabStop = false;
            this.RevisionsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RevisionsSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.RevisionsSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.RevisionsSplitContainer.Name = "RevisionsSplitContainer";
            this.RevisionsSplitContainer.Padding = new System.Windows.Forms.Padding(1);
            this.RevisionsSplitContainer.Controls.Add(this.RevisionGridContainer);
            this.RevisionsSplitContainer.TabIndex = 0;
            this.RevisionGridContainer.Controls.Add(this.RevisionGrid);
            this.RevisionGridContainer.Controls.Add(this.notificationBarBisectInProgress);
            this.RevisionGridContainer.Controls.Add(this.notificationBarGitActionInProgress);
            this.RevisionGridContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RevisionGridContainer.Location = new System.Drawing.Point(0, 0);
            this.RevisionGridContainer.Name = "RevisionGridContainer";
            this.RevisionGridContainer.Size = new System.Drawing.Size(606, 205);
            this.RevisionGridContainer.TabIndex = 2;
            this.RevisionGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RevisionGrid.Location = new System.Drawing.Point(0, 0);
            this.RevisionGrid.Name = "RevisionGrid";
            this.RevisionGrid.Size = new System.Drawing.Size(350, 209);
            this.RevisionGrid.TabIndex = 2;
            this.notificationBarBisectInProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.notificationBarBisectInProgress.Location = new System.Drawing.Point(0, 33);
            this.notificationBarBisectInProgress.MinimumSize = new System.Drawing.Size(0, 33);
            this.notificationBarBisectInProgress.Name = "notificationBarBisectInProgress";
            this.notificationBarBisectInProgress.Size = new System.Drawing.Size(561, 33);
            this.notificationBarBisectInProgress.TabIndex = 1;
            this.notificationBarBisectInProgress.Visible = false;
            this.notificationBarGitActionInProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.notificationBarGitActionInProgress.Location = new System.Drawing.Point(0, 0);
            this.notificationBarGitActionInProgress.MinimumSize = new System.Drawing.Size(0, 33);
            this.notificationBarGitActionInProgress.Name = "notificationBarGitActionInProgress";
            this.notificationBarGitActionInProgress.Size = new System.Drawing.Size(561, 33);
            this.notificationBarGitActionInProgress.TabIndex = 0;
            this.notificationBarGitActionInProgress.Visible = false;
            this.CommitInfoTabControl.Controls.Add(this.DiffTabPage);
            this.CommitInfoTabControl.Controls.Add(this.TreeTabPage);
            this.CommitInfoTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommitInfoTabControl.Location = new System.Drawing.Point(0, 0);
            this.CommitInfoTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.CommitInfoTabControl.Name = "CommitInfoTabControl";
            this.CommitInfoTabControl.Padding = new System.Drawing.Point(0, 0);
            this.CommitInfoTabControl.SelectedIndex = 0;
            this.CommitInfoTabControl.Size = new System.Drawing.Size(650, 287);
            this.CommitInfoTabControl.TabIndex = 0;
            this.CommitInfoTabControl.SelectedIndexChanged += new System.EventHandler(this.CommitInfoTabControl_SelectedIndexChanged);
            this.TreeTabPage.Controls.Add(this.fileTree);
            this.TreeTabPage.Location = new System.Drawing.Point(1, 21);
            this.TreeTabPage.Margin = new System.Windows.Forms.Padding(0);
            this.TreeTabPage.Name = "TreeTabPage";
            this.TreeTabPage.Size = new System.Drawing.Size(646, 264);
            this.TreeTabPage.TabIndex = 0;
            this.TreeTabPage.Text = "File tree";
            this.TreeTabPage.UseVisualStyleBackColor = true;
            this.fileTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTree.Location = new System.Drawing.Point(0, 0);
            this.fileTree.Margin = new System.Windows.Forms.Padding(0);
            this.fileTree.Name = "fileTree";
            this.fileTree.Size = new System.Drawing.Size(646, 264);
            this.fileTree.TabIndex = 0;
            this.DiffTabPage.Controls.Add(this.revisionDiff);
            this.DiffTabPage.Location = new System.Drawing.Point(1, 21);
            this.DiffTabPage.Margin = new System.Windows.Forms.Padding(0);
            this.DiffTabPage.Name = "DiffTabPage";
            this.DiffTabPage.Size = new System.Drawing.Size(646, 264);
            this.DiffTabPage.TabIndex = 1;
            this.DiffTabPage.Text = "Diff";
            this.DiffTabPage.UseVisualStyleBackColor = true;
            this.revisionDiff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.revisionDiff.Location = new System.Drawing.Point(0, 0);
            this.revisionDiff.Margin = new System.Windows.Forms.Padding(0);
            this.revisionDiff.Name = "revisionDiff";
            this.revisionDiff.Size = new System.Drawing.Size(646, 264);
            this.revisionDiff.TabIndex = 0;
            this.FilterToolTip.AutomaticDelay = 100;
            this.FilterToolTip.ShowAlways = true;
            this.FilterToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Error;
            this.FilterToolTip.ToolTipTitle = "RegEx";
            this.FilterToolTip.UseAnimation = false;
            this.FilterToolTip.UseFading = false;
            this.openToolStripMenuItem.Image = global::GitUI.Properties.Images.RepoOpen;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClick);
            this.tsmiFavouriteRepositories.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem4});
            this.tsmiFavouriteRepositories.Image = global::GitUI.Properties.Images.Star;
            this.tsmiFavouriteRepositories.Name = "tsmiFavouriteRepositories";
            this.tsmiFavouriteRepositories.Size = new System.Drawing.Size(198, 22);
            this.tsmiFavouriteRepositories.Text = "Favourite Repositories";
            this.tsmiFavouriteRepositories.DropDownOpening += new System.EventHandler(this.tsmiFavouriteRepositories_DropDownOpening);
            this.tsmiRecentRepositories.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem2,
            this.clearRecentRepositoriesListToolStripMenuItem,
            this.tsmiRecentRepositoriesClear});
            this.tsmiRecentRepositories.Image = global::GitUI.Properties.Images.RecentRepositories;
            this.tsmiRecentRepositories.Name = "tsmiRecentRepositories";
            this.tsmiRecentRepositories.Size = new System.Drawing.Size(198, 22);
            this.tsmiRecentRepositories.Text = "Recent Repositories";
            this.tsmiRecentRepositories.DropDownOpening += new System.EventHandler(this.tsmiRecentRepositories_DropDownOpening);

            this.clearRecentRepositoriesListToolStripMenuItem.Name = "clearRecentRepositoriesListToolStripMenuItem";
            this.clearRecentRepositoriesListToolStripMenuItem.Size = new System.Drawing.Size(119, 6);
            this.tsmiRecentRepositoriesClear.Name = "tsmiRecentRepositoriesClear";
            this.tsmiRecentRepositoriesClear.Size = new System.Drawing.Size(122, 22);
            this.tsmiRecentRepositoriesClear.Text = "Clear List";
            this.tsmiRecentRepositoriesClear.Click += new System.EventHandler(this.tsmiRecentRepositoriesClear_Click);
            this.checkoutBranchToolStripMenuItem.Image = global::GitUI.Properties.Images.BranchCheckout;
            this.checkoutBranchToolStripMenuItem.Name = "checkoutBranchToolStripMenuItem";
            this.checkoutBranchToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+.";
            this.checkoutBranchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemPeriod)));
            this.checkoutBranchToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.checkoutBranchToolStripMenuItem.Text = "Checkout branch...";
            this.checkoutBranchToolStripMenuItem.Click += new System.EventHandler(this.CheckoutBranchToolStripMenuItemClick);
            this.commandsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commitToolStripMenuItem,
            this.undoLastCommitToolStripMenuItem,
            this.pullToolStripMenuItem,
            this.pushToolStripMenuItem,
            this.toolStripSeparator21,
            this.stashToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.cleanupToolStripMenuItem,
            this.toolStripSeparator25,
            this.branchToolStripMenuItem,
            this.deleteBranchToolStripMenuItem,
            this.checkoutBranchToolStripMenuItem,
            this.mergeBranchToolStripMenuItem,
            this.rebaseToolStripMenuItem,
            this.runMergetoolToolStripMenuItem,
            this.toolStripSeparator45,
            this.tagToolStripMenuItem,
            this.deleteTagToolStripMenuItem,
            this.toolStripSeparator23,
            this.cherryPickToolStripMenuItem,
            this.archiveToolStripMenuItem,
            this.checkoutToolStripMenuItem,
            this.bisectToolStripMenuItem,
            this.toolStripMenuItemReflog,
            this.toolStripSeparator22,
            this.formatPatchToolStripMenuItem,
            this.applyPatchToolStripMenuItem,
            this.patchToolStripMenuItem});
            this.commandsToolStripMenuItem.Name = "commandsToolStripMenuItem";
            this.commandsToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.commandsToolStripMenuItem.Text = "&Commands";

             // 
            // commitToolStripMenuItem
            // 
            this.commitToolStripMenuItem.Image = global::GitUI.Properties.Images.RepoStateClean;
            this.commitToolStripMenuItem.Name = "commitToolStripMenuItem";
            this.commitToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.commitToolStripMenuItem.Text = "Commit...";
            this.commitToolStripMenuItem.Click += new System.EventHandler(this.CommitToolStripMenuItemClick);
            // 
            // undoLastCommitToolStripMenuItem
            // 
            this.undoLastCommitToolStripMenuItem.Image = global::GitUI.Properties.Images.ResetFileTo;
            this.undoLastCommitToolStripMenuItem.Name = "undoLastCommitToolStripMenuItem";
            this.undoLastCommitToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.undoLastCommitToolStripMenuItem.Text = "Undo last commit";
            this.undoLastCommitToolStripMenuItem.Click += new System.EventHandler(this.undoLastCommitToolStripMenuItem_Click);
            // 
            // pullToolStripMenuItem
            // 
            this.pullToolStripMenuItem.Image = global::GitUI.Properties.Images.Pull;
            this.pullToolStripMenuItem.Name = "pullToolStripMenuItem";
            this.pullToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.pullToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.pullToolStripMenuItem.Text = "Pull/Fetch...";
            this.pullToolStripMenuItem.Click += new System.EventHandler(this.PullToolStripMenuItemClick);
            // 
            // pushToolStripMenuItem
            // 
            this.pushToolStripMenuItem.Image = global::GitUI.Properties.Images.Push;
            this.pushToolStripMenuItem.Name = "pushToolStripMenuItem";
            this.pushToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.pushToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.pushToolStripMenuItem.Text = "Push...";
            this.pushToolStripMenuItem.Click += new System.EventHandler(this.PushToolStripMenuItemClick);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(210, 6);
            // 
            // stashToolStripMenuItem
            // 
            this.stashToolStripMenuItem.Image = global::GitUI.Properties.Images.Stash;
            this.stashToolStripMenuItem.Name = "stashToolStripMenuItem";
            this.stashToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.stashToolStripMenuItem.Text = "Manage stashes...";
            this.stashToolStripMenuItem.Click += new System.EventHandler(this.StashToolStripMenuItemClick);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Image = global::GitUI.Properties.Images.ResetWorkingDirChanges;
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.resetToolStripMenuItem.Text = "Reset changes...";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.ResetToolStripMenuItem_Click);
            // 
            // cleanupToolStripMenuItem
            // 
            this.cleanupToolStripMenuItem.Image = global::GitUI.Properties.Images.CleanupRepo;
            this.cleanupToolStripMenuItem.Name = "cleanupToolStripMenuItem";
            this.cleanupToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.cleanupToolStripMenuItem.Text = "Clean working directory...";
            this.cleanupToolStripMenuItem.Click += new System.EventHandler(this.CleanupToolStripMenuItemClick);
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            this.toolStripSeparator25.Size = new System.Drawing.Size(210, 6);
            // 
            // branchToolStripMenuItem
            // 
            this.branchToolStripMenuItem.Image = global::GitUI.Properties.Images.BranchCreate;
            this.branchToolStripMenuItem.Name = "branchToolStripMenuItem";
            this.branchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.branchToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.branchToolStripMenuItem.Text = "Create branch...";
            this.branchToolStripMenuItem.Click += new System.EventHandler(this.CreateBranchToolStripMenuItemClick);
            // 
            // deleteBranchToolStripMenuItem
            // 
            this.deleteBranchToolStripMenuItem.Image = global::GitUI.Properties.Images.BranchDelete;
            this.deleteBranchToolStripMenuItem.Name = "deleteBranchToolStripMenuItem";
            this.deleteBranchToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.deleteBranchToolStripMenuItem.Text = "Delete branch...";
            this.deleteBranchToolStripMenuItem.Click += new System.EventHandler(this.DeleteBranchToolStripMenuItemClick);
            // 
            // checkoutBranchToolStripMenuItem
            // 
            this.checkoutBranchToolStripMenuItem.Image = global::GitUI.Properties.Images.BranchCheckout;
            this.checkoutBranchToolStripMenuItem.Name = "checkoutBranchToolStripMenuItem";
            this.checkoutBranchToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+.";
            this.checkoutBranchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemPeriod)));
            this.checkoutBranchToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.checkoutBranchToolStripMenuItem.Text = "Checkout branch...";
            this.checkoutBranchToolStripMenuItem.Click += new System.EventHandler(this.CheckoutBranchToolStripMenuItemClick);
            // 
            // mergeBranchToolStripMenuItem
            // 
            this.mergeBranchToolStripMenuItem.Image = global::GitUI.Properties.Images.Merge;
            this.mergeBranchToolStripMenuItem.Name = "mergeBranchToolStripMenuItem";
            this.mergeBranchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.mergeBranchToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.mergeBranchToolStripMenuItem.Text = "Merge branches...";
            this.mergeBranchToolStripMenuItem.Click += new System.EventHandler(this.MergeBranchToolStripMenuItemClick);
            // 
            // rebaseToolStripMenuItem
            // 
            this.rebaseToolStripMenuItem.Image = global::GitUI.Properties.Images.Rebase;
            this.rebaseToolStripMenuItem.Name = "rebaseToolStripMenuItem";
            this.rebaseToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.rebaseToolStripMenuItem.Text = "Rebase...";
            this.rebaseToolStripMenuItem.Click += new System.EventHandler(this.RebaseToolStripMenuItemClick);
            // 
            // runMergetoolToolStripMenuItem
            // 
            this.runMergetoolToolStripMenuItem.Image = global::GitUI.Properties.Images.Conflict;
            this.runMergetoolToolStripMenuItem.Name = "runMergetoolToolStripMenuItem";
            this.runMergetoolToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.runMergetoolToolStripMenuItem.Text = "Solve merge conflicts...";
            this.runMergetoolToolStripMenuItem.Click += new System.EventHandler(this.RunMergetoolToolStripMenuItemClick);
            // 
            // toolStripSeparator45
            // 
            this.toolStripSeparator45.Name = "toolStripSeparator45";
            this.toolStripSeparator45.Size = new System.Drawing.Size(210, 6);
            // 
            // tagToolStripMenuItem
            // 
            this.tagToolStripMenuItem.Image = global::GitUI.Properties.Images.TagCreate;
            this.tagToolStripMenuItem.Name = "tagToolStripMenuItem";
            this.tagToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.tagToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.tagToolStripMenuItem.Text = "Create tag...";
            this.tagToolStripMenuItem.Click += new System.EventHandler(this.TagToolStripMenuItemClick);
            // 
            // deleteTagToolStripMenuItem
            // 
            this.deleteTagToolStripMenuItem.Image = global::GitUI.Properties.Images.TagDelete;
            this.deleteTagToolStripMenuItem.Name = "deleteTagToolStripMenuItem";
            this.deleteTagToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.deleteTagToolStripMenuItem.Text = "Delete tag...";
            this.deleteTagToolStripMenuItem.Click += new System.EventHandler(this.DeleteTagToolStripMenuItemClick);
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            this.toolStripSeparator23.Size = new System.Drawing.Size(210, 6);
            // 
            // cherryPickToolStripMenuItem
            // 
            this.cherryPickToolStripMenuItem.Image = global::GitUI.Properties.Images.CherryPick;
            this.cherryPickToolStripMenuItem.Name = "cherryPickToolStripMenuItem";
            this.cherryPickToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.cherryPickToolStripMenuItem.Text = "Cherry pick...";
            this.cherryPickToolStripMenuItem.Click += new System.EventHandler(this.CherryPickToolStripMenuItemClick);
            // 
            // archiveToolStripMenuItem
            // 
            this.archiveToolStripMenuItem.Image = global::GitUI.Properties.Images.ArchiveRevision;
            this.archiveToolStripMenuItem.Name = "archiveToolStripMenuItem";
            this.archiveToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.archiveToolStripMenuItem.Text = "Archive revision...";
            this.archiveToolStripMenuItem.Click += new System.EventHandler(this.ArchiveToolStripMenuItemClick);
            // 
            // checkoutToolStripMenuItem
            // 
            this.checkoutToolStripMenuItem.Image = global::GitUI.Properties.Images.Checkout;
            this.checkoutToolStripMenuItem.Name = "checkoutToolStripMenuItem";
            this.checkoutToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.checkoutToolStripMenuItem.Text = "Checkout revision...";
            this.checkoutToolStripMenuItem.Click += new System.EventHandler(this.CheckoutToolStripMenuItemClick);
            // 
            // bisectToolStripMenuItem
            // 
            this.bisectToolStripMenuItem.Image = global::GitUI.Properties.Images.Bisect;
            this.bisectToolStripMenuItem.Name = "bisectToolStripMenuItem";
            this.bisectToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.bisectToolStripMenuItem.Text = "Bisect...";
            this.bisectToolStripMenuItem.Click += new System.EventHandler(this.BisectClick);
            // 
            // toolStripMenuItemReflog
            // 
            this.toolStripMenuItemReflog.Image = global::GitUI.Properties.Images.Book;
            this.toolStripMenuItemReflog.Name = "toolStripMenuItemReflog";
            this.toolStripMenuItemReflog.Size = new System.Drawing.Size(213, 22);
            this.toolStripMenuItemReflog.Text = "Show reflog...";
            this.toolStripMenuItemReflog.Click += new System.EventHandler(this.toolStripMenuItemReflog_Click);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            this.toolStripSeparator22.Size = new System.Drawing.Size(210, 6);
            // 
            // formatPatchToolStripMenuItem
            // 
            this.formatPatchToolStripMenuItem.Image = global::GitUI.Properties.Images.PatchFormat;
            this.formatPatchToolStripMenuItem.Name = "formatPatchToolStripMenuItem";
            this.formatPatchToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.formatPatchToolStripMenuItem.Text = "Format patch...";
            this.formatPatchToolStripMenuItem.Click += new System.EventHandler(this.FormatPatchToolStripMenuItemClick);
            // 
            // applyPatchToolStripMenuItem
            // 
            this.applyPatchToolStripMenuItem.Image = global::GitUI.Properties.Images.PatchApply;
            this.applyPatchToolStripMenuItem.Name = "applyPatchToolStripMenuItem";
            this.applyPatchToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.applyPatchToolStripMenuItem.Text = "Apply patch...";
            this.applyPatchToolStripMenuItem.Click += new System.EventHandler(this.ApplyPatchToolStripMenuItemClick);
            // 
            // patchToolStripMenuItem
            // 
            this.patchToolStripMenuItem.Image = global::GitUI.Properties.Images.PatchView;
            this.patchToolStripMenuItem.Name = "patchToolStripMenuItem";
            this.patchToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.patchToolStripMenuItem.Text = "View patch file...";
            this.patchToolStripMenuItem.Click += new System.EventHandler(this.PatchToolStripMenuItemClick);
            //// 
            //// toolStripSeparator46
            //// 
            //this.toolStripSeparator46.Name = "toolStripSeparator46";
            //this.toolStripSeparator46.Size = new System.Drawing.Size(268, 6);
            //// 
            //// toolStripSeparator41
            //// 
            //this.toolStripSeparator41.Name = "toolStripSeparator41";
            //this.toolStripSeparator41.Size = new System.Drawing.Size(214, 6);
            //// 
            //// toolStripSeparator42
            //// 
            //this.toolStripSeparator42.Name = "toolStripSeparator42";
            //this.toolStripSeparator42.Size = new System.Drawing.Size(110, 6);
            //// 
            //// toolStripSeparator6
            //// 
            //this.toolStripSeparator6.Name = "toolStripSeparator6";
            //this.toolStripSeparator6.Size = new System.Drawing.Size(214, 6);
            //// 
            //// PuTTYToolStripMenuItem
            //// 
            //this.PuTTYToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            //this.startAuthenticationAgentToolStripMenuItem,
            //this.generateOrImportKeyToolStripMenuItem});
            //this.PuTTYToolStripMenuItem.Image = global::GitUI.Properties.Images.Putty;
            //this.PuTTYToolStripMenuItem.Name = "PuTTYToolStripMenuItem";
            //this.PuTTYToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            //this.PuTTYToolStripMenuItem.Text = "PuTTY";
            //// 
            //// startAuthenticationAgentToolStripMenuItem
            //// 
            //this.startAuthenticationAgentToolStripMenuItem.Image = global::GitUI.Properties.Images.Pageant16;
            //this.startAuthenticationAgentToolStripMenuItem.Name = "startAuthenticationAgentToolStripMenuItem";
            //this.startAuthenticationAgentToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            //this.startAuthenticationAgentToolStripMenuItem.Text = "Start authentication agent";
            //this.startAuthenticationAgentToolStripMenuItem.Click += new System.EventHandler(this.StartAuthenticationAgentToolStripMenuItemClick);
            //// 
            //// generateOrImportKeyToolStripMenuItem
            //// 
            //this.generateOrImportKeyToolStripMenuItem.Image = global::GitUI.Properties.Images.PuttyGen;
            //this.generateOrImportKeyToolStripMenuItem.Name = "generateOrImportKeyToolStripMenuItem";
            //this.generateOrImportKeyToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            //this.generateOrImportKeyToolStripMenuItem.Text = "Generate or import key";
            //this.generateOrImportKeyToolStripMenuItem.Click += new System.EventHandler(this.GenerateOrImportKeyToolStripMenuItemClick);
            //// 
            //// _repositoryHostsToolStripMenuItem
            //// 
            //this._repositoryHostsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            //this._forkCloneRepositoryToolStripMenuItem,
            //this._viewPullRequestsToolStripMenuItem,
            //this._createPullRequestsToolStripMenuItem,
            //this._addUpstreamRemoteToolStripMenuItem});
            //this._repositoryHostsToolStripMenuItem.Name = "_repositoryHostsToolStripMenuItem";
            //this._repositoryHostsToolStripMenuItem.Size = new System.Drawing.Size(114, 20);
            //this._repositoryHostsToolStripMenuItem.Text = "(Repository hosts)";
            //// 
            //// _forkCloneRepositoryToolStripMenuItem
            //// 
            //this._forkCloneRepositoryToolStripMenuItem.Name = "_forkCloneRepositoryToolStripMenuItem";
            //this._forkCloneRepositoryToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            //this._forkCloneRepositoryToolStripMenuItem.Text = "Fork/Clone repository...";
            //this._forkCloneRepositoryToolStripMenuItem.Click += new System.EventHandler(this._forkCloneMenuItem_Click);
            //// 
            //// _viewPullRequestsToolStripMenuItem
            //// 
            //this._viewPullRequestsToolStripMenuItem.Name = "_viewPullRequestsToolStripMenuItem";
            //this._viewPullRequestsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            //this._viewPullRequestsToolStripMenuItem.Text = "View pull requests...";
            //this._viewPullRequestsToolStripMenuItem.Click += new System.EventHandler(this._viewPullRequestsToolStripMenuItem_Click);
            //// 
            //// _createPullRequestsToolStripMenuItem
            //// 
            //this._createPullRequestsToolStripMenuItem.Name = "_createPullRequestsToolStripMenuItem";
            //this._createPullRequestsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            //this._createPullRequestsToolStripMenuItem.Text = "Create pull requests...";
            //this._createPullRequestsToolStripMenuItem.Click += new System.EventHandler(this._createPullRequestToolStripMenuItem_Click);
            //// 
            //// dashboardToolStripMenuItem
            //// 
            //this.dashboardToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            //this.refreshDashboardToolStripMenuItem,
            //this.toolStripSeparator42});
            //this.dashboardToolStripMenuItem.Name = "dashboardToolStripMenuItem";
            //this.dashboardToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            //this.dashboardToolStripMenuItem.Text = "&Dashboard";
            //// 
            //// pluginsToolStripMenuItem
            //// 
            //this.pluginsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            //this.toolStripSeparator15,
            //this.pluginSettingsToolStripMenuItem});
            //this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            //this.pluginsToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            //this.pluginsToolStripMenuItem.Text = "&Plugins";
            //// 
            //// toolStripSeparator15
            //// 
            //this.toolStripSeparator15.Name = "toolStripSeparator15";
            //this.toolStripSeparator15.Size = new System.Drawing.Size(150, 6);
            //// 
            //// pluginSettingsToolStripMenuItem
            //// 
            //this.pluginSettingsToolStripMenuItem.Image = global::GitUI.Properties.Images.Settings;
            //this.pluginSettingsToolStripMenuItem.Name = "pluginSettingsToolStripMenuItem";
            //this.pluginSettingsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            //this.pluginSettingsToolStripMenuItem.Text = "Plugin Settings";
            //this.pluginSettingsToolStripMenuItem.Click += new System.EventHandler(this.PluginSettingsToolStripMenuItemClick);
            //// 
            //// settingsToolStripMenuItem
            //// 
            //this.settingsToolStripMenuItem.Image = global::GitUI.Properties.Images.Settings;
            //this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            //this.settingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemcomma)));
            //this.settingsToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            //this.settingsToolStripMenuItem.Text = "Settings";
            //this.settingsToolStripMenuItem.Click += new System.EventHandler(this.OnShowSettingsClick);
            // 
            // toolStripMenuItemReflog
            // 
            this.toolStripMenuItemReflog.Image = global::GitUI.Properties.Images.Book;
            this.toolStripMenuItemReflog.Name = "toolStripMenuItemReflog";
            this.toolStripMenuItemReflog.Size = new System.Drawing.Size(213, 22);
            this.toolStripMenuItemReflog.Text = "Show reflog...";
            this.toolStripMenuItemReflog.Click += new System.EventHandler(this.toolStripMenuItemReflog_Click);

            this.toolPanel.BottomToolStripPanelVisible = false;
            this.toolPanel.ContentPanel.Controls.Add(this.MainSplitContainer);
            this.toolPanel.ContentPanel.Margin = new System.Windows.Forms.Padding(0);
            this.toolPanel.ContentPanel.Padding = new System.Windows.Forms.Padding(6);
            this.toolPanel.ContentPanel.Size = new System.Drawing.Size(1846, 1023);
            this.toolPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolPanel.LeftToolStripPanelVisible = false;
            this.toolPanel.Location = new System.Drawing.Point(0, 24);
            this.toolPanel.Margin = new System.Windows.Forms.Padding(0);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Padding = new System.Windows.Forms.Padding(0);
            this.toolPanel.TopToolStripPanel.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.toolPanel.RightToolStripPanelVisible = false;
            this.toolPanel.Size = new System.Drawing.Size(923, 527);
            this.toolPanel.TabIndex = 1;
            this.toolPanel.TopToolStripPanel.Controls.Add(this.ToolStripMain);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(923, 573);
            this.Controls.Add(this.toolPanel);
            this.Name = "WordGitForm";
            this.Text = "Git Extensions";
            this.ToolStripMain.ResumeLayout(false);
            this.ToolStripMain.PerformLayout();
            this.MainSplitContainer.Panel1.ResumeLayout(false);
            this.MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).EndInit();
            this.MainSplitContainer.ResumeLayout(false);
            this.RightSplitContainer.Panel1.ResumeLayout(false);
            this.RightSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RightSplitContainer)).EndInit();
            this.RightSplitContainer.ResumeLayout(false);
            this.RevisionsSplitContainer.ResumeLayout(false);
            this.RevisionsSplitContainer.ResumeLayout(false);
            this.RevisionGridContainer.ResumeLayout(false);
            this.CommitInfoTabControl.ResumeLayout(false);
            this.TreeTabPage.ResumeLayout(false);
            this.DiffTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gitItemBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gitRevisionBindingSource)).EndInit();
            this.toolPanel.ContentPanel.ResumeLayout(false);
            this.toolPanel.TopToolStripPanel.ResumeLayout(false);
            this.toolPanel.TopToolStripPanel.PerformLayout();
            this.toolPanel.ResumeLayout(false);
            this.toolPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

       

        #endregion

        internal SplitContainer MainSplitContainer;
        private SplitContainer RightSplitContainer;
        private ContainerControl RevisionsSplitContainer;

        private FullBleedTabControl CommitInfoTabControl;
        private TabPage DiffTabPage;
        private TabPage TreeTabPage;
        private BindingSource gitRevisionBindingSource;
        private BindingSource gitItemBindingSource;
        private GitUI.RevisionGridControl RevisionGrid;
        private GitUI.BranchTreePanel.RepoObjectsTree repoObjectsTree;
        private ToolTip FilterToolTip;
        private RevisionFileTreeControl fileTree;
        private RevisionDiffControl revisionDiff;
        private ToolStripContainer toolPanel;
        private RevisionGpgInfoControl revisionGpgInfo1;
        private ToolStripButton tsbShowRepos;
        private ToolStripEx ToolStripMain;
        private ToolStripSplitButton _NO_TRANSLATE_WorkingDir;
        private ToolStripSplitButton branchSelect;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem tsmiRecentRepositories;
        private ToolStripMenuItem tsmiRecentRepositoriesClear;
        private ToolStripSeparator clearRecentRepositoriesListToolStripMenuItem;

        private ToolStripSplitButton commandsToolStripMenuItem;
        private ToolStripMenuItem applyPatchToolStripMenuItem;
        private ToolStripMenuItem archiveToolStripMenuItem;
        private ToolStripMenuItem bisectToolStripMenuItem;
        private ToolStripMenuItem checkoutBranchToolStripMenuItem;
        private ToolStripMenuItem checkoutToolStripMenuItem;
        private ToolStripMenuItem cherryPickToolStripMenuItem;
        private ToolStripMenuItem cleanupToolStripMenuItem;
        private ToolStripMenuItem cloneToolStripMenuItem;
        private ToolStripMenuItem commitToolStripMenuItem;
        private ToolStripMenuItem branchToolStripMenuItem;
        private ToolStripMenuItem tagToolStripMenuItem;
        private ToolStripMenuItem deleteBranchToolStripMenuItem;
        private ToolStripMenuItem deleteTagToolStripMenuItem;
        private ToolStripMenuItem formatPatchToolStripMenuItem;
        private ToolStripMenuItem initNewRepositoryToolStripMenuItem;
        private ToolStripMenuItem mergeBranchToolStripMenuItem;
        private ToolStripMenuItem pullToolStripMenuItem;
        private ToolStripMenuItem pushToolStripMenuItem;
        private ToolStripMenuItem rebaseToolStripMenuItem;
        private ToolStripMenuItem runMergetoolToolStripMenuItem;
        private ToolStripMenuItem stashToolStripMenuItem;
        private ToolStripMenuItem patchToolStripMenuItem;
        private ToolStripMenuItem undoLastCommitToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator45;
        private ToolStripSeparator toolStripSeparator21;
        private ToolStripSeparator toolStripSeparator25;
        private ToolStripSeparator toolStripSeparator22;
        private ToolStripSeparator toolStripSeparator23;
        private ToolStripMenuItem resetToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemReflog;
        private ToolStripMenuItem tsmiFavouriteRepositories;
        private Panel RevisionGridContainer;
        private UserControls.InteractiveGitActionControl notificationBarBisectInProgress;
        private UserControls.InteractiveGitActionControl notificationBarGitActionInProgress;
    }
}
