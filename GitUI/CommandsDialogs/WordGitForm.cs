using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConEmu.WinForms;
using GitCommands;
using GitCommands.Config;
using GitCommands.Git;
using GitCommands.Git.Commands;
using GitCommands.Gpg;
using GitCommands.Submodules;
using GitCommands.UserRepositoryHistory;
using GitCommands.Utils;
using GitExtUtils;
using GitExtUtils.GitUI;
using GitExtUtils.GitUI.Theming;
using GitUI.BranchTreePanel;
using GitUI.CommandsDialogs.BrowseDialog;
using GitUI.CommandsDialogs.BrowseDialog.DashboardControl;
using GitUI.CommandsDialogs.WorktreeDialog;
using GitUI.HelperDialogs;
using GitUI.Hotkey;
using GitUI.Infrastructure.Telemetry;
using GitUI.NBugReports;
using GitUI.Properties;
using GitUI.Script;
using GitUI.Shells;
using GitUI.UserControls;
using GitUI.UserControls.RevisionGrid;
using GitUIPluginInterfaces;
using GitUIPluginInterfaces.RepositoryHosts;
using Microsoft;
using Microsoft.VisualStudio.Threading;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using ResourceManager;

namespace GitUI.CommandsDialogs
{
    public sealed partial class WordGitForm: GitModuleForm, IBrowseRepo
    {
        #region Translation

        private readonly TranslationString _noSubmodulesPresent = new("No submodules");
        private readonly TranslationString _topProjectModuleFormat = new("Top project: {0}");
        private readonly TranslationString _superprojectModuleFormat = new("Superproject: {0}");
        private readonly TranslationString _goToSuperProject = new("Go to superproject");

        private readonly TranslationString _indexLockCantDelete = new("Failed to delete index.lock");

        private readonly TranslationString _loading = new("Loading...");

        private readonly TranslationString _noReposHostPluginLoaded = new("No repository host plugin loaded.");
        private readonly TranslationString _noReposHostFound = new("Could not find any relevant repository hosts for the currently open repository.");

        private readonly TranslationString _configureWorkingDirMenu = new("Configure this menu");

        private readonly TranslationString _updateCurrentSubmodule = new("Update current submodule");

        private readonly TranslationString _pullFetch = new("Fetch");
        private readonly TranslationString _pullFetchAll = new("Fetch all");
        private readonly TranslationString _pullFetchPruneAll = new("Fetch and prune all");
        private readonly TranslationString _pullMerge = new("Pull - merge");
        private readonly TranslationString _pullRebase = new("Pull - rebase");
        private readonly TranslationString _pullOpenDialog = new("Open pull dialog");

        private readonly TranslationString _buildReportTabCaption = new("Build Report");
        private readonly TranslationString _noWorkingFolderText = new("No working directory");
        private readonly TranslationString _commitButtonText = new("Commit");

        private readonly TranslationString _undoLastCommitText = new("You will still be able to find all the commit's changes in the staging area\n\nDo you want to continue?");
        private readonly TranslationString _undoLastCommitCaption = new("Undo last commit");

        #endregion

        private readonly SplitterManager _splitterManager = new(new AppSettingsPath("FormBrowse"));
        private readonly GitStatusMonitor _gitStatusMonitor;
        private readonly FormBrowseMenus _formBrowseMenus;
        private readonly IFormBrowseController _controller;
        private readonly ICommitDataManager _commitDataManager;
        private readonly IAppTitleGenerator _appTitleGenerator;
        private readonly IAheadBehindDataProvider? _aheadBehindDataProvider;
        private readonly WindowsJumpListManager _windowsJumpListManager;
        private readonly ISubmoduleStatusProvider _submoduleStatusProvider;
        private List<ToolStripItem>? _currentSubmoduleMenuItems;
        private BuildReportTabPageExtension? _buildReportTabPageExtension;
        private readonly ShellProvider _shellProvider = new();
        private ConEmuControl? _terminal;
        private Dashboard? _dashboard;

        private TabPage? _consoleTabPage;

        [Flags]
        private enum UpdateTargets
        {
            None = 1,
            DiffList = 2,
            FileTree = 4,
            CommitInfo = 8
        }

        private UpdateTargets _selectedRevisionUpdatedTargets = UpdateTargets.None;

        public override RevisionGridControl RevisionGridControl { get => RevisionGrid; }

        [Obsolete("For VS designer and translation test only. Do not remove.")]
#pragma warning disable CS8618               
        private WordGitForm()
#pragma warning restore CS8618               
        {
            InitializeComponent();
            InitializeComplete();
        }

        public WordGitForm(GitUICommands commands, string filter, ObjectId? selectedId = null, ObjectId? firstId = null)
            : base(commands)
        {
            InitializeComponent();

            MainSplitContainer.Visible = false;
            MainSplitContainer.SplitterDistance = DpiUtil.Scale(260);

            CommitInfoTabControl.ImageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = DpiUtil.Scale(new Size(16, 16)),
                Images =
                {
                    { nameof(Images.FileTree), Images.FileTree },
                    { nameof(Images.Diff), Images.Diff }
                }
            };

            DiffTabPage.ImageKey = nameof(Images.Diff);
            TreeTabPage.ImageKey = nameof(Images.FileTree);
            repoObjectsTree.Initialize(_aheadBehindDataProvider, null, RevisionGrid, RevisionGrid, RevisionGrid);
            revisionDiff.Bind(RevisionGrid, fileTree, () => RequestRefresh());
            fileTree.Bind(() => RequestRefresh());

            var repositoryDescriptionProvider = new RepositoryDescriptionProvider(new GitDirectoryResolver());
            _appTitleGenerator = new AppTitleGenerator(repositoryDescriptionProvider);
            _windowsJumpListManager = new WindowsJumpListManager(repositoryDescriptionProvider);

            InitCountArtificial(out _gitStatusMonitor);

            if (!EnvUtils.RunningOnWindows())
            {
            }

            RevisionGrid.SelectionChanged += (sender, e) =>
            {
                _selectedRevisionUpdatedTargets = UpdateTargets.None;
                RefreshSelection();
            };
            RevisionGrid.RevisionGraphLoaded += (sender, e) =>
            {
                if (sender is null || MainSplitContainer.Panel1Collapsed)
                {
                    return;
                }

                bool isFiltering = !AppSettings.ShowReflogReferences
                                && (AppSettings.ShowCurrentBranchOnly || AppSettings.BranchFilterEnabled);
                repoObjectsTree.ToggleFilterMode(isFiltering);
            };

            HotkeysEnabled = true;
            Hotkeys = HotkeySettingsManager.LoadHotkeys(HotkeySettingsName);
            UICommandsChanged += (a, e) =>
            {
                var oldCommands = e.OldCommands;
                if (oldCommands is not null)
                {
                    oldCommands.PostRepositoryChanged -= UICommands_PostRepositoryChanged;
                    oldCommands.BrowseRepo = null;
                }

                UICommands.PostRepositoryChanged += UICommands_PostRepositoryChanged;
                UICommands.BrowseRepo = this;
            };

            UICommands.PostRepositoryChanged += UICommands_PostRepositoryChanged;
            UICommands.BrowseRepo = this;
            _controller = new FormBrowseController(new GitGpgController(() => Module), new RepositoryCurrentBranchNameProvider(), new InvalidRepositoryRemover());
            _commitDataManager = new CommitDataManager(() => Module);

            _submoduleStatusProvider = SubmoduleStatusProvider.Default;
            _submoduleStatusProvider.StatusUpdating += SubmoduleStatusProvider_StatusUpdating;
            _submoduleStatusProvider.StatusUpdated += SubmoduleStatusProvider_StatusUpdated;

            FillBuildReport(revision: null);     
            RevisionGrid.ShowBuildServerInfo = true;

            _formBrowseMenus = new FormBrowseMenus(ToolStripMain);
            RevisionGrid.MenuCommands.MenuChanged += (sender, e) => _formBrowseMenus.OnMenuCommandsPropertyChanged();
            SystemEvents.SessionEnding += (sender, args) => SaveApplicationSettings();

            ManageWorktreeSupport();

            WorkaroundToolbarLocationBug();
            WorkaroundPaddingIncreaseBug();

            var toolBackColor = SystemColors.Window;
            var toolForeColor = SystemColors.WindowText;
            BackColor = toolBackColor;
            ForeColor = toolForeColor;
            toolPanel.TopToolStripPanel.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    _formBrowseMenus.ShowToolStripContextMenu(Cursor.Position);
                }
            };

            foreach (var control in this.FindDescendants())
            {
                control.AllowDrop = true;
                control.DragEnter += FormBrowse_DragEnter;
                control.DragDrop += FormBrowse_DragDrop;
            }

            RevisionGrid.SelectedId = selectedId;
            RevisionGrid.FirstId = firstId;

            InitializeComplete();
            UpdateCommitButtonAndGetBrush(null, AppSettings.ShowGitStatusInBrowseToolbar);
            RestorePosition();

            RevisionGrid.ToggledBetweenArtificialAndHeadCommits += (s, e) => FocusRevisionDiffFileStatusList();

            toolPanel.TopToolStripPanel.BackColor = Color.Transparent;
            ToolStripMain.BackColor = Color.Transparent;
            BackColor = OtherColors.BackgroundColor;

            return;

            void FocusRevisionDiffFileStatusList()
            {
                if (!revisionDiff.Visible)
                {
                    CommitInfoTabControl.SelectedTab = DiffTabPage;
                }

                if (revisionDiff.Visible)
                {
                    revisionDiff.SwitchFocus(alreadyContainedFocus: false);
                }
            }

            void ManageWorktreeSupport()
            {
                if (!GitVersion.Current.SupportWorktree)
                {
                }

                if (!GitVersion.Current.SupportWorktreeList)
                {
                }
            }

            void InitCountArtificial(out GitStatusMonitor gitStatusMonitor)
            {
                Brush? lastBrush = null;

                gitStatusMonitor = new GitStatusMonitor(this);
                if (!NeedsGitStatusMonitor())
                {
                    gitStatusMonitor.Active = false;
                }

                gitStatusMonitor.GitStatusMonitorStateChanged += (s, e) =>
                {
                    var status = e.State;
                    if (status == GitStatusMonitorState.Stopped)
                    {
                        UpdateCommitButtonAndGetBrush(null, showCount: false);
                        RevisionGrid.UpdateArtificialCommitCount(null);
                        if (EnvUtils.RunningOnWindowsWithMainWindow())
                        {
                            TaskbarManager.Instance.SetOverlayIcon(null, "");
                        }

                        lastBrush = null;
                    }
                };

                gitStatusMonitor.GitWorkingDirectoryStatusChanged += (s, e) =>
                {
                    IReadOnlyList<GitItemStatus>? status = e?.ItemStatuses;

                    bool countToolbar = AppSettings.ShowGitStatusInBrowseToolbar;
                    bool countArtificial = AppSettings.ShowGitStatusForArtificialCommits && AppSettings.RevisionGraphShowWorkingDirChanges;

                    var brush = UpdateCommitButtonAndGetBrush(status, countToolbar);

                    RevisionGrid.UpdateArtificialCommitCount(countArtificial ? status : null);
                    if (countToolbar || countArtificial)
                    {
                        if (!ReferenceEquals(brush, lastBrush)
                            && EnvUtils.RunningOnWindowsWithMainWindow())
                        {
                            lastBrush = brush;

                            const int imgDim = 32;
                            const int dotDim = 15;
                            const int pad = 2;
                            using (var bmp = new Bitmap(imgDim, imgDim))
                            {
                                using (var g = Graphics.FromImage(bmp))
                                {
                                    g.SmoothingMode = SmoothingMode.AntiAlias;
                                    g.Clear(Color.Transparent);
                                    g.FillEllipse(brush, new Rectangle(imgDim - dotDim - pad, imgDim - dotDim - pad, dotDim, dotDim));
                                }

                                using var overlay = Icon.FromHandle(bmp.GetHicon());
                                TaskbarManager.Instance.SetOverlayIcon(overlay, "");
                            }

                            var repoStateVisualiser = new RepoStateVisualiser();
                            var (image, _) = repoStateVisualiser.Invoke(status);
                            _windowsJumpListManager.UpdateCommitIcon(image);
                        }

                        if (AppSettings.ShowSubmoduleStatus)
                        {
                            Validates.NotNull(_submoduleStatusProvider);

                            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                            {
                                try
                                {
                                    await TaskScheduler.Default;
                                    await _submoduleStatusProvider.UpdateSubmodulesStatusAsync(Module.WorkingDir, status);
                                }
                                catch (GitConfigurationException ex)
                                {
                                    await this.SwitchToMainThreadAsync();
                                    MessageBoxes.ShowGitConfigurationExceptionMessage(this, ex);
                                }
                            });
                        }
                    }
                };
            }

           Brush UpdateCommitButtonAndGetBrush(IReadOnlyList<GitItemStatus>? status, bool showCount)
            {
                var repoStateVisualiser = new RepoStateVisualiser();
                var (image, brush) = repoStateVisualiser.Invoke(status);

                return brush;
            }

            void WorkaroundToolbarLocationBug()
            {
                toolPanel.TopToolStripPanel.Controls.Clear();

                ToolStrip[] toolStrips = new[] {ToolStripMain };
                foreach (ToolStrip toolStrip in toolStrips)
                {
                    toolPanel.TopToolStripPanel.Controls.Add(toolStrip);
                }

#if DEBUG
                foreach (ToolStrip toolStrip in toolStrips)
                {
                    Debug.Assert(toolStrip.Top == 0, $"{toolStrip.Name} must be placed on the 1st row");
                }

                for (int i = toolStrips.Length - 1; i > 0; i--)
                {
                    Debug.Assert(toolStrips[i].Left < toolStrips[i - 1].Left, $"{toolStrips[i - 1].Name} must be placed before {toolStrips[i].Name}");
                }
#endif
            }

            void WorkaroundPaddingIncreaseBug()
            {
                MainSplitContainer.Panel1.Padding = new Padding(1);
                RevisionsSplitContainer.Padding = new Padding(1);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _formBrowseMenus?.Dispose();
                components?.Dispose();
                _gitStatusMonitor?.Dispose();
                _windowsJumpListManager?.Dispose();

            }

            base.Dispose(disposing);
        }

        protected override void OnApplicationActivated()
        {
            if (AppSettings.RefreshArtificialCommitOnApplicationActivated && CommitInfoTabControl.SelectedTab == DiffTabPage)
            {
                revisionDiff.RefreshArtificial();
            }

            base.OnApplicationActivated();
        }

        protected override void OnLoad(EventArgs e)
        {
            _formBrowseMenus.CreateToolbarsMenus(ToolStripMain);

            HideVariableMainMenuItems();
            RefreshSplitViewLayout();
            InternalInitialize(false);

            if (!Module.IsValidGitWorkingDir())
            {
                base.OnLoad(e);
                return;
            }

            RevisionGrid.Load();
            ActiveControl = RevisionGrid;
            RevisionGrid.IndexWatcher.Reset();

            RevisionGrid.IndexWatcher.Changed += (_, args) =>
            {
                bool indexChanged = args.IsIndexChanged;
                this.InvokeAsync(
                        () =>
                        {
                        })
                    .FileAndForget();
            };
            UpdateSubmodulesStructure();
            UpdateStashCount();

            base.OnLoad(e);

            SetSplitterPositions();
        }

        protected override void OnActivated(EventArgs e)
        {
            this.InvokeAsync(OnActivate).FileAndForget();
            base.OnActivated(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveApplicationSettings();

            foreach (var control in this.FindDescendants())
            {
                control.DragEnter -= FormBrowse_DragEnter;
                control.DragDrop -= FormBrowse_DragDrop;
            }

            base.OnFormClosing(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _splitterManager.SaveSplitters();
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            PluginRegistry.Unregister(UICommands);
            base.OnClosed(e);
        }

        public override void AddTranslationItems(ITranslation translation)
        {
            base.AddTranslationItems(translation);
        }

        public override void TranslateItems(ITranslation translation)
        {
            base.TranslateItems(translation);
        }

        public override void CancelButtonClick(object sender, EventArgs e)
        {
            if (RevisionGrid.FilterIsApplied(false))
            {
            }

            else if (RevisionGrid.FilterIsApplied(true) || AppSettings.BranchFilterEnabled)
            {
                RevisionGrid.ShowAllBranches();
            }
        }

        private bool NeedsGitStatusMonitor()
        {
            return AppSettings.ShowGitStatusInBrowseToolbar || (AppSettings.ShowGitStatusForArtificialCommits && AppSettings.RevisionGraphShowWorkingDirChanges);
        }

        private void UICommands_PostRepositoryChanged(object sender, GitUIEventArgs e)
        {
            this.InvokeAsync(RefreshRevisions).FileAndForget();
            UpdateSubmodulesStructure();
            UpdateStashCount();
            revisionDiff.UICommands_PostRepositoryChanged(sender, e);
        }

        private void RefreshRevisions()
        {
            if (RevisionGrid.IsDisposed || IsDisposed || Disposing)
            {
                return;
            }

            _gitStatusMonitor.InvalidateGitWorkingDirectoryStatus();
            RequestRefresh();

            if (_dashboard is null || !_dashboard.Visible)
            {
                revisionDiff.RefreshArtificial();
                RevisionGrid.ForceRefreshRevisions();
                InternalInitialize(true);
            }

        }

        private void RequestRefresh() => _gitStatusMonitor?.RequestRefresh();

        private void RefreshSelection()
        {
            var selectedRevisions = RevisionGrid.GetSelectedRevisions();
            var selectedRevision = RevisionGrid.GetSelectedRevisions().FirstOrDefault();

            FillFileTree(selectedRevision);
            FillDiff(selectedRevisions);

            var oldBody = selectedRevision?.Body;
            if (selectedRevision is not null && selectedRevision.HasMultiLineMessage && oldBody != selectedRevision.Body)
            {
                RevisionGrid.Refresh();
            }

            FillBuildReport(selectedRevision);
            repoObjectsTree.SelectionChanged(selectedRevisions);
        }

        #region IBrowseRepo

        public void GoToRef(string refName, bool showNoRevisionMsg, bool toggleSelection = false)
        {
            using (WaitCursorScope.Enter())
            {
                RevisionGrid.GoToRef(refName, showNoRevisionMsg, toggleSelection);
            }
        }

        #endregion

        private void ShowDashboard()
        {
            toolPanel.SuspendLayout();
            toolPanel.TopToolStripPanelVisible = false;
            toolPanel.BottomToolStripPanelVisible = false;
            toolPanel.LeftToolStripPanelVisible = false;
            toolPanel.RightToolStripPanelVisible = false;
            toolPanel.ResumeLayout();

            MainSplitContainer.Visible = false;

            if (_dashboard is null)
            {
                _dashboard = new Dashboard { Dock = DockStyle.Fill };
                _dashboard.GitModuleChanged += SetGitModule;
                toolPanel.ContentPanel.Controls.Add(_dashboard);
            }

            Text = _appTitleGenerator.Generate(branchName: TranslatedStrings.NoBranch);

            _dashboard.RefreshContent();
            _dashboard.Visible = true;
            _dashboard.BringToFront();

            DiagnosticsClient.TrackPageView("Dashboard");
        }

        private void HideDashboard()
        {
            MainSplitContainer.Visible = true;
            if (_dashboard is null || !_dashboard.Visible)
            {
                return;
            }

            _dashboard.Visible = false;
            toolPanel.SuspendLayout();
            toolPanel.TopToolStripPanelVisible = true;
            toolPanel.BottomToolStripPanelVisible = true;
            toolPanel.LeftToolStripPanelVisible = true;
            toolPanel.RightToolStripPanelVisible = true;
            toolPanel.ResumeLayout();

            DiagnosticsClient.TrackPageView("Revision graph");
        }

        private void UpdatePluginMenu(bool validWorkingDir)
        {
        }

        private void HideVariableMainMenuItems()
        {
            _formBrowseMenus.RemoveRevisionGridMainMenuItems();
        }

        private void InternalInitialize(bool hard)
        {
            toolPanel.SuspendLayout();
            toolPanel.TopToolStripPanel.SuspendLayout();

            using (WaitCursorScope.Enter())
            {
                if (AppSettings.CheckForUpdates && AppSettings.LastUpdateCheck.AddDays(7) < DateTime.Now)
                {
                    AppSettings.LastUpdateCheck = DateTime.Now;
                    var updateForm = new FormUpdates(AppSettings.AppVersion);
                    updateForm.SearchForUpdatesAndShow(ownerWindow: this, alwaysShow: false);
                }

                bool hasWorkingDir = !string.IsNullOrEmpty(Module.WorkingDir);
                if (hasWorkingDir)
                {
                    HideDashboard();
                }
                else
                {
                    ShowDashboard();
                }

                bool bareRepository = Module.IsBareRepository();
                bool isDashboard = _dashboard is not null && _dashboard.Visible;
                bool validBrowseDir = !isDashboard && Module.IsValidGitWorkingDir();

                if (hard && hasWorkingDir)
                {
                    ShowRevisions();
                }

                RefreshWorkingDirComboText();
                var branchName = !string.IsNullOrEmpty(branchSelect.Text) ? branchSelect.Text : TranslatedStrings.NoBranch;
                Text = _appTitleGenerator.Generate(Module.WorkingDir, validBrowseDir, branchName);

                OnActivate();

                if (validBrowseDir)
                {
                    _windowsJumpListManager.AddToRecent(Module.WorkingDir);

                    _formBrowseMenus.ResetMenuCommandSets();
                }
                else
                {
                    _windowsJumpListManager.DisableThumbnailToolbar();
                }

                UICommands.RaisePostBrowseInitialize(this);
            }

            toolPanel.TopToolStripPanel.ResumeLayout();
            toolPanel.ResumeLayout();

            return;

            void ShowRevisions()
            {
                if (RevisionGrid.IndexWatcher.IndexChanged)
                {
                    RefreshSelection();
                }

                RevisionGrid.IndexWatcher.Reset();
            }
        }

        private void OnActivate()
        {
            notificationBarBisectInProgress.RefreshBisect();

            notificationBarGitActionInProgress.RefreshGitAction();
        }

        private void UpdateStashCount()
        {
            if (AppSettings.ShowStashCount)
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                {
                    await Task.Delay(500);
                    await TaskScheduler.Default;

                    var result = Module.GetStashes(noLocks: true).Count;

                    await this.SwitchToMainThreadAsync();

                }).FileAndForget();
            }
            else
            {
            }
        }

        #region Working directory combo box

        private void RefreshWorkingDirComboText()
        {
            var path = Module.WorkingDir;

            if (string.IsNullOrWhiteSpace(path))
            {
                _NO_TRANSLATE_WorkingDir.Text = _noWorkingFolderText.Text;
                return;
            }

            var recentRepositoryHistory = ThreadHelper.JoinableTaskFactory.Run(
                () => RepositoryHistoryManager.Locals.AddAsMostRecentAsync(path));

            var mostRecentRepos = new List<RecentRepoInfo>();
            using var graphics = CreateGraphics();
            var splitter = new RecentRepoSplitter
            {
                MeasureFont = _NO_TRANSLATE_WorkingDir.Font,
                Graphics = graphics
            };

            splitter.SplitRecentRepos(recentRepositoryHistory, mostRecentRepos, mostRecentRepos);

            var ri = mostRecentRepos.Find(e => e.Repo.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase));

            _NO_TRANSLATE_WorkingDir.Text = PathUtil.GetDisplayPath(ri?.Caption ?? path);

            if (AppSettings.RecentReposComboMinWidth > 0)
            {
                _NO_TRANSLATE_WorkingDir.AutoSize = false;
                var captionWidth = graphics.MeasureString(_NO_TRANSLATE_WorkingDir.Text, _NO_TRANSLATE_WorkingDir.Font).Width;
                captionWidth = captionWidth + _NO_TRANSLATE_WorkingDir.DropDownButtonWidth + 5;
                _NO_TRANSLATE_WorkingDir.Width = Math.Max(AppSettings.RecentReposComboMinWidth, (int)captionWidth);
            }
            else
            {
                _NO_TRANSLATE_WorkingDir.AutoSize = true;
            }
        }

        private void WorkingDirDropDownOpening(object sender, EventArgs e)
        {
            _NO_TRANSLATE_WorkingDir.DropDownItems.Clear();

            var tsmiCategorisedRepos = new ToolStripMenuItem(tsmiFavouriteRepositories.Text, tsmiFavouriteRepositories.Image);
            PopulateFavouriteRepositoriesMenu(tsmiCategorisedRepos);
            if (tsmiCategorisedRepos.DropDownItems.Count > 0)
            {
                _NO_TRANSLATE_WorkingDir.DropDownItems.Add(tsmiCategorisedRepos);
            }

            PopulateRecentRepositoriesMenu(_NO_TRANSLATE_WorkingDir);

            _NO_TRANSLATE_WorkingDir.DropDownItems.Add(new ToolStripSeparator());

            var mnuOpenLocalRepository = new ToolStripMenuItem(openToolStripMenuItem.Text, openToolStripMenuItem.Image) { ShortcutKeys = openToolStripMenuItem.ShortcutKeys };
            mnuOpenLocalRepository.Click += OpenToolStripMenuItemClick;
            _NO_TRANSLATE_WorkingDir.DropDownItems.Add(mnuOpenLocalRepository);

            var mnuRecentReposSettings = new ToolStripMenuItem(_configureWorkingDirMenu.Text);
            mnuRecentReposSettings.Click += (hs, he) =>
            {
                using (var frm = new FormRecentReposSettings())
                {
                    frm.ShowDialog(this);
                }

                RefreshWorkingDirComboText();
            };

            _NO_TRANSLATE_WorkingDir.DropDownItems.Add(mnuRecentReposSettings);

            PreventToolStripSplitButtonClosing((ToolStripSplitButton)sender);
        }

        private void WorkingDirClick(object sender, EventArgs e)
        {
            _NO_TRANSLATE_WorkingDir.ShowDropDown();
        }

        private void _NO_TRANSLATE_WorkingDir_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                OpenToolStripMenuItemClick(sender, e);
            }
        }

        #endregion

        private void FillFileTree(GitRevision revision)
        {
            var showFileTreeTab = revision?.IsArtificial != true;

            if (showFileTreeTab)
            {
                if (TreeTabPage.Parent is null)
                {
                    var index = CommitInfoTabControl.TabPages.IndexOf(DiffTabPage);
                    Debug.Assert(index != -1, "TabControl should contain diff tab page");
                    CommitInfoTabControl.TabPages.Insert(index + 1, TreeTabPage);
                }
            }
            else
            {
                TreeTabPage.Parent = null;
            }

            if (CommitInfoTabControl.SelectedTab != TreeTabPage || _selectedRevisionUpdatedTargets.HasFlag(UpdateTargets.FileTree))
            {
                return;
            }

            _selectedRevisionUpdatedTargets |= UpdateTargets.FileTree;
            fileTree.LoadRevision(revision);
        }

        private void FillDiff(IReadOnlyList<GitRevision> revisions)
        {
            if (CommitInfoTabControl.SelectedTab != DiffTabPage)
            {
                return;
            }

            if (_selectedRevisionUpdatedTargets.HasFlag(UpdateTargets.DiffList))
            {
                return;
            }

            _selectedRevisionUpdatedTargets |= UpdateTargets.DiffList;
            revisionDiff.DisplayDiffTab(revisions);
        }

        private void FillBuildReport(GitRevision? revision)
        {
            _buildReportTabPageExtension ??= new BuildReportTabPageExtension(() => Module, CommitInfoTabControl, _buildReportTabCaption.Text);

            _buildReportTabPageExtension.FillBuildReport(revision);
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            GitModule? module = FormOpenDirectory.OpenModule(this, Module);
            if (module is not null)
            {
                SetGitModule(this, new GitModuleEventArgs(module));
            }
        }

        private void CheckoutToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartCheckoutRevisionDialog(this);
        }

        private void CloneToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartCloneDialog(this, string.Empty, false, SetGitModule);
        }

        private void CommitToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartCommitDialog(this);
        }

        private void InitNewRepositoryToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartInitializeDialog(this, gitModuleChanged: SetGitModule);
        }

        private void PushToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartPushDialog(this, pushOnShow: ModifierKeys.HasFlag(Keys.Shift));
        }

        private void RefreshToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.RepoChangedNotifier.Notify();
        }

        private void RefreshDashboardToolStripMenuItemClick(object sender, EventArgs e)
        {
            _dashboard?.RefreshContent();
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            using var frm = new FormAbout();
            frm.ShowDialog(this);
        }

        private void PatchToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartViewPatchDialog(this);
        }

        private void ApplyPatchToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartApplyPatchDialog(this);
        }

        private void GitGuiToolStripMenuItemClick(object sender, EventArgs e)
        {
            Module.RunGui();
        }

        private void FormatPatchToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartFormatPatchDialog(this);
        }

        private void GitcommandLogToolStripMenuItemClick(object sender, EventArgs e)
        {
            FormGitCommandLog.ShowOrActivate(this);
        }

        private void CheckoutBranchToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartCheckoutBranch(this);
        }

        private void StashToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartStashDialog(this);
            UpdateStashCount();
        }

        private void ResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UICommands.StartResetChangesDialog(this);
            RequestRefresh();
            revisionDiff.RefreshArtificial();
        }

        private void RunMergetoolToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartResolveConflictsDialog(this);
        }

        private void CurrentBranchClick(object sender, EventArgs e)
        {
            branchSelect.ShowDropDown();
        }

        private void DeleteBranchToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartDeleteBranchDialog(this, string.Empty);
        }

        private void DeleteTagToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartDeleteTagDialog(this, null);
        }

        private void CherryPickToolStripMenuItemClick(object sender, EventArgs e)
        {
            var revisions = RevisionGrid.GetSelectedRevisions(SortDirection.Descending);

            UICommands.StartCherryPickDialog(this, revisions);
        }

        private void MergeBranchToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartMergeBranchDialog(this, null);
        }

        private void OnShowSettingsClick(object sender, EventArgs e)
        {
            var translation = AppSettings.Translation;
            var commitInfoPosition = AppSettings.CommitInfoPosition;

            UICommands.StartSettingsDialog(this);

            if (translation != AppSettings.Translation)
            {
                Translator.Translate(this, AppSettings.CurrentTranslation);
            }

            if (commitInfoPosition != AppSettings.CommitInfoPosition)
            {
            }

            Hotkeys = HotkeySettingsManager.LoadHotkeys(HotkeySettingsName);
            RevisionGrid.ReloadHotkeys();
            RevisionGrid.ReloadTranslation();
            fileTree.ReloadHotkeys();
            revisionDiff.ReloadHotkeys();
            new CustomDiffMergeToolProvider().Clear();
            revisionDiff.LoadCustomDifftools();
            RevisionGrid.LoadCustomDifftools();

            _dashboard?.RefreshContent();

            _gitStatusMonitor.Active = NeedsGitStatusMonitor() && Module.IsValidGitWorkingDir();

        }

        private void TagToolStripMenuItemClick(object sender, EventArgs e)
        {
            var revision = RevisionGrid.LatestSelectedRevision;

            UICommands.StartCreateTagDialog(this, revision);
        }

        private void KGitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Module.RunGitK();
        }

        private void DonateToolStripMenuItemClick(object sender, EventArgs e)
        {
            using var frm = new FormDonate();
            frm.ShowDialog(this);
        }

        private static void SaveApplicationSettings()
        {
            AppSettings.SaveSettings();
        }

        private void EditGitignoreToolStripMenuItem1Click(object sender, EventArgs e)
        {
            UICommands.StartEditGitIgnoreDialog(this, false);
        }

        private void EditGitInfoExcludeToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartEditGitIgnoreDialog(this, true);
        }

        private void ArchiveToolStripMenuItemClick(object sender, EventArgs e)
        {
            var revisions = RevisionGrid.GetSelectedRevisions();
            if (revisions.Count < 1 || revisions.Count > 2)
            {
                MessageBoxes.SelectOnlyOneOrTwoRevisions(this);
                return;
            }

            GitRevision mainRevision = revisions.First();
            GitRevision? diffRevision = null;
            if (revisions.Count == 2)
            {
                diffRevision = revisions.Last();
            }

            UICommands.StartArchiveDialog(this, mainRevision, diffRevision);
        }

        private void EditMailMapToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartMailMapDialog(this);
        }

        private void EditLocalGitConfigToolStripMenuItemClick(object sender, EventArgs e)
        {
            var fileName = Path.Combine(Module.ResolveGitInternalPath("config"));
            UICommands.StartFileEditorDialog(fileName, true);
        }

        private void CompressGitDatabaseToolStripMenuItemClick(object sender, EventArgs e)
        {
            FormProcess.ReadDialog(this, process: null, arguments: "gc", Module.WorkingDir, input: null, useDialogSettings: true);
        }

        private void recoverLostObjectsToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartVerifyDatabaseDialog(this);
        }

        private void ManageRemoteRepositoriesToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartRemotesDialog(this);
        }

        private void RebaseToolStripMenuItemClick(object sender, EventArgs e)
        {
            var revisions = RevisionGrid.GetSelectedRevisions();

            if (revisions.Count == 0)
            {
                return;
            }

            if (revisions.Count == 2)
            {
                string? to = null;
                string? from = null;

                string currentBranch = Module.GetSelectedBranch();
                var currentCheckout = RevisionGrid.CurrentCheckout;

                if (revisions[0].ObjectId == currentCheckout)
                {
                    from = revisions[1].ObjectId.ToShortString();
                    to = currentBranch;
                }
                else if (revisions[1].ObjectId == currentCheckout)
                {
                    from = revisions[0].ObjectId.ToShortString();
                    to = currentBranch;
                }

                UICommands.StartRebaseDialog(this, from, to, null, interactive: false, startRebaseImmediately: false);
            }
            else
            {
                UICommands.StartRebaseDialog(this, revisions.First().Guid);
            }
        }

        private void StartAuthenticationAgentToolStripMenuItemClick(object sender, EventArgs e)
        {
            new Executable(AppSettings.Pageant, Module.WorkingDir).Start();
        }

        private void GenerateOrImportKeyToolStripMenuItemClick(object sender, EventArgs e)
        {
            new Executable(AppSettings.Puttygen, Module.WorkingDir).Start();
        }

        private void CommitInfoTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelection();
            if (CommitInfoTabControl.SelectedTab == DiffTabPage)
            {
                revisionDiff.SwitchFocus(alreadyContainedFocus: false);
            }
        }

        private void ChangelogToolStripMenuItemClick(object sender, EventArgs e)
        {
            using var frm = new FormChangeLog();
            frm.ShowDialog(this);
        }

        private void ToolStripButtonPushClick(object sender, EventArgs e)
        {
            PushToolStripMenuItemClick(sender, e);
        }

        private void ManageSubmodulesToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartSubmodulesDialog(this);
            UpdateSubmodulesStructure();
        }

        private void UpdateSubmoduleToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem toolStripMenuItem)
            {
                var submodule = toolStripMenuItem.Tag as string;
                Validates.NotNull(Module.SuperprojectModule);
                FormProcess.ShowDialog(this, process: null, arguments: GitCommandHelpers.SubmoduleUpdateCmd(submodule), Module.SuperprojectModule.WorkingDir, input: null, useDialogSettings: true);
            }

            UICommands.RepoChangedNotifier.Notify();
        }

        private void UpdateAllSubmodulesToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartUpdateSubmodulesDialog(this);
            UpdateSubmodulesStructure();
        }

        private void SynchronizeAllSubmodulesToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartSyncSubmodulesDialog(this);
            UpdateSubmodulesStructure();
        }

        private void ToolStripSplitStashButtonClick(object sender, EventArgs e)
        {
            UICommands.StartStashDialog(this);
            UpdateStashCount();
        }

        private void StashChangesToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StashSave(this, AppSettings.IncludeUntrackedFilesInManualStash);
            UpdateStashCount();
        }

        private void StashPopToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StashPop(this);
            UpdateStashCount();
        }

        private void ManageStashesToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartStashDialog(this);
            UpdateStashCount();
        }

        private void CreateStashToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartStashDialog(this, false);
            UpdateStashCount();
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        private void tsmiFavouriteRepositories_DropDownOpening(object sender, EventArgs e)
        {
            tsmiFavouriteRepositories.DropDownItems.Clear();
            PopulateFavouriteRepositoriesMenu(tsmiFavouriteRepositories);
        }

        private void tsmiRecentRepositories_DropDownOpening(object sender, EventArgs e)
        {
            tsmiRecentRepositories.DropDownItems.Clear();
            PopulateRecentRepositoriesMenu(tsmiRecentRepositories);
            if (tsmiRecentRepositories.DropDownItems.Count < 1)
            {
                return;
            }

            tsmiRecentRepositories.DropDownItems.Add(clearRecentRepositoriesListToolStripMenuItem);
            TranslateItem(tsmiRecentRepositoriesClear.Name, tsmiRecentRepositoriesClear);
            tsmiRecentRepositories.DropDownItems.Add(tsmiRecentRepositoriesClear);
        }

        private void tsmiRecentRepositoriesClear_Click(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                var repositoryHistory = Array.Empty<Repository>();
                await RepositoryHistoryManager.Locals.SaveRecentHistoryAsync(repositoryHistory);

                await this.SwitchToMainThreadAsync();
                _dashboard?.RefreshContent();
            });
        }

        private void PluginSettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartPluginSettingsDialog(this);
        }

        private void RepoSettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartRepoSettingsDialog(this);
        }

        private void CloseToolStripMenuItemClick(object sender, EventArgs e)
        {
            SetWorkingDir("");
        }

        private void UserManualToolStripMenuItemClick(object sender, EventArgs e)
        {
            OsShellUtil.OpenUrlInDefaultBrowser(AppSettings.DocumentationBaseUrl);
        }

        private void CleanupToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartCleanupRepositoryDialog(this);
        }

        private void PopulateFavouriteRepositoriesMenu(ToolStripDropDownItem container)
        {
            var repositoryHistory = ThreadHelper.JoinableTaskFactory.Run(() => RepositoryHistoryManager.Locals.LoadFavouriteHistoryAsync());
            if (repositoryHistory.Count < 1)
            {
                return;
            }

            PopulateFavouriteRepositoriesMenu(container, repositoryHistory);
        }

        private void PopulateFavouriteRepositoriesMenu(ToolStripDropDownItem container, in IList<Repository> repositoryHistory)
        {
            var mostRecentRepos = new List<RecentRepoInfo>();
            var lessRecentRepos = new List<RecentRepoInfo>();

            using (var graphics = CreateGraphics())
            {
                var splitter = new RecentRepoSplitter
                {
                    MeasureFont = container.Font,
                    Graphics = graphics
                };

                splitter.SplitRecentRepos(repositoryHistory, mostRecentRepos, lessRecentRepos);
            }

            foreach (var repo in mostRecentRepos.Union(lessRecentRepos).GroupBy(k => k.Repo.Category).OrderBy(k => k.Key))
            {
                AddFavouriteRepositories(repo.Key, repo.ToList());
            }

            void AddFavouriteRepositories(string? category, IList<RecentRepoInfo> repos)
            {
                ToolStripMenuItem menuItemCategory;
                if (!container.DropDownItems.ContainsKey(category))
                {
                    menuItemCategory = new ToolStripMenuItem(category);
                    container.DropDownItems.Add(menuItemCategory);
                }
                else
                {
                    menuItemCategory = (ToolStripMenuItem)container.DropDownItems[category];
                }

                foreach (var r in repos)
                {
                    _controller.AddRecentRepositories(menuItemCategory, r.Repo, r.Caption, SetGitModule);
                }
            }
        }

        private void PopulateRecentRepositoriesMenu(ToolStripDropDownItem container)
        {
            var mostRecentRepos = new List<RecentRepoInfo>();
            var lessRecentRepos = new List<RecentRepoInfo>();

            var repositoryHistory = ThreadHelper.JoinableTaskFactory.Run(() => RepositoryHistoryManager.Locals.LoadRecentHistoryAsync());
            if (repositoryHistory.Count < 1)
            {
                return;
            }

            using (var graphics = CreateGraphics())
            {
                var splitter = new RecentRepoSplitter
                {
                    MeasureFont = container.Font,
                    Graphics = graphics
                };

                splitter.SplitRecentRepos(repositoryHistory, mostRecentRepos, lessRecentRepos);
            }

            foreach (var repo in mostRecentRepos)
            {
                _controller.AddRecentRepositories(container, repo.Repo, repo.Caption, SetGitModule);
            }

            if (lessRecentRepos.Count > 0)
            {
                if (mostRecentRepos.Count > 0 && (AppSettings.SortMostRecentRepos || AppSettings.SortLessRecentRepos))
                {
                    container.DropDownItems.Add(new ToolStripSeparator());
                }

                foreach (var repo in lessRecentRepos)
                {
                    _controller.AddRecentRepositories(container, repo.Repo, repo.Caption, SetGitModule);
                }
            }
        }

        public void SetWorkingDir(string? path, ObjectId? selectedId = null, ObjectId? firstId = null)
        {
            RevisionGrid.SelectedId = selectedId;
            RevisionGrid.FirstId = firstId;
            SetGitModule(this, new GitModuleEventArgs(new GitModule(path)));
        }

        private void SetGitModule(object sender, GitModuleEventArgs e)
        {
            var module = e.GitModule;
            HideVariableMainMenuItems();
            PluginRegistry.Unregister(UICommands);
            RevisionGrid.InvalidateCount();
            _gitStatusMonitor.InvalidateGitWorkingDirectoryStatus();
            _submoduleStatusProvider.Init();
            UICommands = new GitUICommands(module);
            if (Module.IsValidGitWorkingDir())
            {
                var path = Module.WorkingDir;
                ThreadHelper.JoinableTaskFactory.Run(() => RepositoryHistoryManager.Locals.AddAsMostRecentAsync(path));
                AppSettings.RecentWorkingDir = path;
#if DEBUG
                Debug.WriteLine("Encodings for " + path);
                Debug.WriteLine("Files content encoding: " + module.FilesEncoding.EncodingName);
                Debug.WriteLine("Commit encoding: " + module.CommitEncoding.EncodingName);
                if (module.LogOutputEncoding.CodePage != module.CommitEncoding.CodePage)
                {
                    Debug.WriteLine("Log output encoding: " + module.LogOutputEncoding.EncodingName);
                }
#endif

                HideDashboard();
                UICommands.RepoChangedNotifier.Notify();
                RevisionGrid.IndexWatcher.Reset();
            }
            else
            {
                RevisionGrid.IndexWatcher.Reset();
                MainSplitContainer.Visible = false;
                ShowDashboard();
            }

        }

        private void TranslateToolStripMenuItemClick(object sender, EventArgs e)
        {
            OsShellUtil.OpenUrlInDefaultBrowser(@"https://github.com/gitextensions/gitextensions/wiki/Translations");
        }

        private void FileExplorerToolStripMenuItemClick(object sender, EventArgs e)
        {
            OsShellUtil.OpenWithFileExplorer(Module.WorkingDir);
        }

        private void CreateBranchToolStripMenuItemClick(object sender, EventArgs e)
        {
            UICommands.StartCreateBranchDialog(this, RevisionGrid.LatestSelectedRevision?.ObjectId);
        }

        private void editGitAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UICommands.StartEditGitAttributesDialog(this);
        }

        public static void CopyFullPathToClipboard(FileStatusList diffFiles, GitModule module)
        {
            if (!diffFiles.SelectedItems.Any())
            {
                return;
            }

            var fileNames = new StringBuilder();
            foreach (var item in diffFiles.SelectedItems)
            {
                var path = PathUtil.Combine(module.WorkingDir, item.Item.Name);
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                if (fileNames.Length > 0)
                {
                    fileNames.AppendLine();
                }

                fileNames.Append(path.ToNativePath());
            }

            ClipboardUtil.TrySetText(fileNames.ToString());
        }

        private void deleteIndexLockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Module.UnlockIndex(true);
            }
            catch (FileDeleteException ex)
            {
                ThreadHelper.AssertOnUIThread();
                throw new UserExternalOperationException(_indexLockCantDelete.Text,
                    new ExternalOperationException(command: null, arguments: ex.FileName, Module.WorkingDir, ex));
            }
        }

        private void BisectClick(object sender, EventArgs e)
        {
            using (var frm = new FormBisect(RevisionGrid))
            {
                frm.ShowDialog(this);
            }

            UICommands.RepoChangedNotifier.Notify();
        }

        private void CurrentBranchDropDownOpening(object sender, EventArgs e)
        {
            branchSelect.DropDownItems.Clear();

            AddCheckoutBranchMenuItem();
            branchSelect.DropDownItems.Add(new ToolStripSeparator());
            AddBranchesMenuItems();

            PreventToolStripSplitButtonClosing(sender as ToolStripSplitButton);

            void AddCheckoutBranchMenuItem()
            {
                var checkoutBranchItem = new ToolStripMenuItem(checkoutBranchToolStripMenuItem.Text, Images.BranchCheckout)
                {
                    ShortcutKeys = checkoutBranchToolStripMenuItem.ShortcutKeys,
                    ShortcutKeyDisplayString = checkoutBranchToolStripMenuItem.ShortcutKeyDisplayString
                };

                branchSelect.DropDownItems.Add(checkoutBranchItem);
                checkoutBranchItem.Click += CheckoutBranchToolStripMenuItemClick;
            }

            void AddBranchesMenuItems()
            {
                foreach (IGitRef branch in GetBranches())
                {
                    Validates.NotNull(branch.ObjectId);
                    bool isBranchVisible = ((ICheckRefs)RevisionGridControl).Contains(branch.ObjectId);

                    ToolStripItem toolStripItem = branchSelect.DropDownItems.Add(branch.Name);
                    toolStripItem.ForeColor = isBranchVisible ? branchSelect.ForeColor : Color.Silver.AdaptTextColor();
                    toolStripItem.Image = isBranchVisible ? Images.Branch : Images.EyeClosed;
                    toolStripItem.Click += (s, e) => UICommands.StartCheckoutBranch(this, toolStripItem.Text);
                    toolStripItem.AdaptImageLightness();
                }

                IEnumerable<IGitRef> GetBranches()
                {
                    return Module
                        .GetRefs(tags: false, branches: true)
                        .Take(100);
                }
            }
        }

        private void _forkCloneMenuItem_Click(object sender, EventArgs e)
        {
            if (PluginRegistry.GitHosters.Count > 0)
            {
                UICommands.StartCloneForkFromHoster(this, PluginRegistry.GitHosters[0], SetGitModule);
                UICommands.RepoChangedNotifier.Notify();
            }
            else
            {
                MessageBox.Show(this, _noReposHostPluginLoaded.Text, TranslatedStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _viewPullRequestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TryGetRepositoryHost(out var repoHost))
            {
                return;
            }

            UICommands.StartPullRequestsDialog(this, repoHost);
        }

        private void _createPullRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TryGetRepositoryHost(out var repoHost))
            {
                return;
            }

            UICommands.StartCreatePullRequest(this, repoHost);
        }

        private void _addUpstreamRemoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                if (!TryGetRepositoryHost(out var repoHost))
                {
                    return;
                }

                var remoteName = await repoHost.AddUpstreamRemoteAsync();
                if (!string.IsNullOrEmpty(remoteName))
                {
                    UICommands.StartPullDialogAndPullImmediately(this, null, remoteName, AppSettings.PullAction.Fetch);
                }
            }).FileAndForget();
        }

        private bool TryGetRepositoryHost([NotNullWhen(returnValue: true)] out IRepositoryHostPlugin? repoHost)
        {
            repoHost = PluginRegistry.TryGetGitHosterForModule(Module);
            if (repoHost is null)
            {
                MessageBox.Show(this, _noReposHostFound.Text, TranslatedStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        #region Hotkey commands

        public static readonly string HotkeySettingsName = "Browse";

        internal enum Command
        {
            GitBash = 0,
            GitGui = 1,
            GitGitK = 2,
            FocusRevisionGrid = 3,
            FocusCommitInfo = 4,
            FocusDiff = 5,
            FocusFileTree = 6,
            Commit = 7,
            AddNotes = 8,
            FindFileInSelectedCommit = 9,
            CheckoutBranch = 10,
            QuickFetch = 11,
            QuickPull = 12,
            QuickPush = 13,

            CloseRepository = 15,
            Stash = 16,
            StashPop = 17,
            FocusFilter = 18,
            OpenWithDifftool = 19,
            OpenSettings = 20,
            ToggleBranchTreePanel = 21,
            EditFile = 22,
            OpenAsTempFile = 23,
            OpenAsTempFileWith = 24,
            FocusBranchTree = 25,
            FocusGpgInfo = 26,
            GoToSuperproject = 27,
            GoToSubmodule = 28,
            FocusBuildServerStatus = 30,
            FocusNextTab = 31,
            FocusPrevTab = 32,
            OpenWithDifftoolFirstToLocal = 33,
            OpenWithDifftoolSelectedToLocal = 34,
            OpenCommitsWithDifftool = 35,
            ToggleBetweenArtificialAndHeadCommits = 36,
            GoToChild = 37,
            GoToParent = 38
        }

        internal Keys GetShortcutKeys(Command cmd)
        {
            return GetShortcutKeys((int)cmd);
        }

        private void AddNotes()
        {
            var revision = RevisionGrid.GetSelectedRevisions().FirstOrDefault();
            var objectId = revision?.ObjectId;

            if (objectId is null || objectId.IsArtificial)
            {
                return;
            }

            Module.EditNotes(objectId);
        }

        private void FindFileInSelectedCommit()
        {
            CommitInfoTabControl.SelectedTab = TreeTabPage;

            AppSettings.ShowSplitViewLayout = true;
            RefreshSplitViewLayout();

            fileTree.InvokeFindFileDialog();
        }

        private void QuickFetch()
        {
            bool success = ScriptManager.RunEventScripts(this, ScriptEvent.BeforeFetch);
            if (!success)
            {
                return;
            }

            success = FormProcess.ShowDialog(this, process: null, arguments: Module.FetchCmd(string.Empty, string.Empty, string.Empty), Module.WorkingDir, input: null, useDialogSettings: true);
            if (!success)
            {
                return;
            }

            ScriptManager.RunEventScripts(this, ScriptEvent.AfterFetch);
            UICommands.RepoChangedNotifier.Notify();
        }

        protected override CommandStatus ExecuteCommand(int cmd)
        {
            switch ((Command)cmd)
            {
                case Command.GitGui: Module.RunGui(); break;
                case Command.GitGitK: Module.RunGitK(); break;
                case Command.FocusBranchTree: FocusBranchTree(); break;
                case Command.FocusRevisionGrid: RevisionGrid.Focus(); break;
                case Command.FocusDiff: FocusTabOf(revisionDiff, (c, alreadyContainedFocus) => c.SwitchFocus(alreadyContainedFocus)); break;
                case Command.FocusFileTree: FocusTabOf(fileTree, (c, alreadyContainedFocus) => c.SwitchFocus(alreadyContainedFocus)); break;
                case Command.FocusGpgInfo when AppSettings.ShowGpgInformation.Value: FocusTabOf(revisionGpgInfo1, (c, alreadyContainedFocus) => c.Focus()); break;
                case Command.FocusBuildServerStatus: FocusTabOf(_buildReportTabPageExtension?.Control, (c, alreadyContainedFocus) => c.Focus()); break;
                case Command.FocusNextTab: FocusNextTab(); break;
                case Command.FocusPrevTab: FocusNextTab(forward: false); break;
                case Command.Commit: CommitToolStripMenuItemClick(this, EventArgs.Empty); break;
                case Command.AddNotes: AddNotes(); break;
                case Command.FindFileInSelectedCommit: FindFileInSelectedCommit(); break;
                case Command.CheckoutBranch: CheckoutBranchToolStripMenuItemClick(this, EventArgs.Empty); break;
                case Command.QuickFetch: QuickFetch(); break;
                case Command.QuickPull: mergeToolStripMenuItem_Click(this, EventArgs.Empty); break;
                case Command.QuickPush: UICommands.StartPushDialog(this, true); break;
                case Command.CloseRepository: CloseToolStripMenuItemClick(this, EventArgs.Empty); break;
                case Command.Stash: UICommands.StashSave(this, AppSettings.IncludeUntrackedFilesInManualStash); break;
                case Command.StashPop: UICommands.StashPop(this); break;
                case Command.OpenCommitsWithDifftool: RevisionGrid.DiffSelectedCommitsWithDifftool(); break;
                case Command.OpenWithDifftool: OpenWithDifftool(); break;
                case Command.OpenWithDifftoolFirstToLocal: OpenWithDifftoolFirstToLocal(); break;
                case Command.OpenWithDifftoolSelectedToLocal: OpenWithDifftoolSelectedToLocal(); break;
                case Command.OpenSettings: OnShowSettingsClick(this, EventArgs.Empty); break;
                case Command.ToggleBranchTreePanel: toggleBranchTreePanel_Click(this, EventArgs.Empty); break;
                case Command.EditFile: EditFile(); break;
                case Command.OpenAsTempFile when fileTree.Visible: fileTree.ExecuteCommand(RevisionFileTreeControl.Command.OpenAsTempFile); break;
                case Command.OpenAsTempFileWith when fileTree.Visible: fileTree.ExecuteCommand(RevisionFileTreeControl.Command.OpenAsTempFileWith); break;
                case Command.ToggleBetweenArtificialAndHeadCommits: RevisionGrid?.ExecuteCommand(RevisionGridControl.Command.ToggleBetweenArtificialAndHeadCommits); break;
                case Command.GoToChild: RestoreFileStatusListFocus(() => RevisionGrid?.ExecuteCommand(RevisionGridControl.Command.GoToChild)); break;
                case Command.GoToParent: RestoreFileStatusListFocus(() => RevisionGrid?.ExecuteCommand(RevisionGridControl.Command.GoToParent)); break;
                default: return base.ExecuteCommand(cmd);
            }

            return true;

            void FocusBranchTree()
            {
                if (!MainSplitContainer.Panel1Collapsed)
                {
                    repoObjectsTree.Focus();
                }
            }

            void FocusTabOf<T>(T? control, Action<T, bool> switchFocus) where T : Control
            {
                if (control is not null)
                {
                    var tabPage = control.Parent as TabPage;
                    if (CommitInfoTabControl.TabPages.IndexOf(tabPage) >= 0)
                    {
                        bool alreadyContainedFocus = control.ContainsFocus;

                        if (CommitInfoTabControl.SelectedTab != tabPage)
                        {
                            CommitInfoTabControl.SelectedTab = tabPage;
                        }

                        switchFocus(control, alreadyContainedFocus);
                    }
                }
            }

            void FocusNextTab(bool forward = true)
            {
                int tabIndex = CommitInfoTabControl.SelectedIndex;
                tabIndex += forward ? 1 : (CommitInfoTabControl.TabCount - 1);
                CommitInfoTabControl.SelectedIndex = tabIndex % CommitInfoTabControl.TabCount;
            }

            void OpenWithDifftool()
            {
                if (revisionDiff.Visible)
                {
                    revisionDiff.ExecuteCommand(RevisionDiffControl.Command.OpenWithDifftool);
                }
                else if (fileTree.Visible)
                {
                    fileTree.ExecuteCommand(RevisionFileTreeControl.Command.OpenWithDifftool);
                }
            }

            void OpenWithDifftoolFirstToLocal()
            {
                if (revisionDiff.Visible)
                {
                    revisionDiff.ExecuteCommand(RevisionDiffControl.Command.OpenWithDifftoolFirstToLocal);
                }
            }

            void OpenWithDifftoolSelectedToLocal()
            {
                if (revisionDiff.Visible)
                {
                    revisionDiff.ExecuteCommand(RevisionDiffControl.Command.OpenWithDifftoolSelectedToLocal);
                }
            }

            void EditFile()
            {
                if (revisionDiff.Visible)
                {
                    revisionDiff.ExecuteCommand(RevisionDiffControl.Command.EditFile);
                }
                else if (fileTree.Visible)
                {
                    fileTree.ExecuteCommand(RevisionFileTreeControl.Command.EditFile);
                }
            }

            void RestoreFileStatusListFocus(Action action)
            {
                bool restoreFocus = revisionDiff.ContainsFocus;

                action();

                if (restoreFocus)
                {
                    revisionDiff.SwitchFocus(alreadyContainedFocus: false);
                }
            }
        }

        internal CommandStatus ExecuteCommand(Command cmd)
        {
            return ExecuteCommand((int)cmd);
        }

        #endregion

        public static void OpenContainingFolder(FileStatusList diffFiles, GitModule module)
        {
            if (!diffFiles.SelectedItems.Any())
            {
                return;
            }

            foreach (var item in diffFiles.SelectedItems)
            {
                string? filePath = PathUtil.Combine(module.WorkingDir, item.Item.Name.ToNativePath());

                if (!Strings.IsNullOrWhiteSpace(filePath))
                {
                    FormBrowseUtil.ShowFileOrParentFolderInFileExplorer(filePath);
                }
            }
        }

        private void SetSplitterPositions()
        {
            _splitterManager.AddSplitter(MainSplitContainer, nameof(MainSplitContainer));
            _splitterManager.AddSplitter(RightSplitContainer, nameof(RightSplitContainer));

            revisionDiff.InitSplitterManager(_splitterManager);
            fileTree.InitSplitterManager(_splitterManager);

            _splitterManager.RestoreSplitters();
            RefreshLayoutToggleButtonStates();
        }

        private void CommandsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var selectedRevisions = RevisionGrid.GetSelectedRevisions();
            bool singleNormalCommit = selectedRevisions.Count == 1 && !selectedRevisions[0].IsArtificial;

        }

        private void PullToolStripMenuItemClick(object sender, EventArgs e)
        {
            DoPull(pullAction: AppSettings.FormPullAction, isSilent: false);
        }

        private void ToolStripButtonPullClick(object sender, EventArgs e)
        {
            bool isSilent = AppSettings.DefaultPullAction != AppSettings.PullAction.None;
            var pullAction = AppSettings.DefaultPullAction != AppSettings.PullAction.None ?
                AppSettings.DefaultPullAction : AppSettings.FormPullAction;
            DoPull(pullAction: pullAction, isSilent: isSilent);
        }

        private void pullToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DoPull(pullAction: AppSettings.FormPullAction, isSilent: false);
        }

        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoPull(pullAction: AppSettings.PullAction.Merge, isSilent: true);
        }

        private void rebaseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DoPull(pullAction: AppSettings.PullAction.Rebase, isSilent: true);
        }

        private void fetchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoPull(pullAction: AppSettings.PullAction.Fetch, isSilent: true);
        }

        private void fetchAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoPull(pullAction: AppSettings.PullAction.FetchAll, isSilent: true);
        }

        private void fetchPruneAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoPull(pullAction: AppSettings.PullAction.FetchPruneAll, isSilent: true);
        }

        private void DoPull(AppSettings.PullAction pullAction, bool isSilent)
        {
            if (isSilent)
            {
                UICommands.StartPullDialogAndPullImmediately(this, pullAction: pullAction);
            }
            else
            {
                UICommands.StartPullDialog(this, pullAction: pullAction);
            }
        }

        private void branchSelect_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                CheckoutBranchToolStripMenuItemClick(sender, e);
            }
        }

        private void RevisionInfo_CommandClicked(object sender, ResourceManager.CommandEventArgs e)
        {
            switch (e.Command)
            {
                case "gotocommit":
                    Validates.NotNull(e.Data);
                    var found = Module.TryResolvePartialCommitId(e.Data, out var revision);

                    if (found)
                    {
                        found = RevisionGrid.SetSelectedRevision(revision);
                    }

                    if (AppSettings.ShowFirstParent && !found)
                    {
                        RevisionGrid.SetSelectedRevision(revision);
                    }

                    break;
                case "gotobranch":
                case "gototag":
                    Validates.NotNull(e.Data);
                    CommitData? commit = _commitDataManager.GetCommitData(e.Data, out _);
                    if (commit is not null)
                    {
                        RevisionGrid.SetSelectedRevision(commit.ObjectId);
                    }

                    break;
                case "navigatebackward":
                    RevisionGrid.NavigateBackward();
                    break;
                case "navigateforward":
                    RevisionGrid.NavigateForward();
                    break;
                default:
                    throw new InvalidOperationException($"unexpected internal link: {e.Command}/{e.Data}");
            }
        }

        private void SubmoduleToolStripButtonClick(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuSender)
            {
                SetWorkingDir(menuSender.Tag as string);
            }
        }

        private void PreventToolStripSplitButtonClosing(ToolStripSplitButton? control)
        {
            if (control is null    )
            {
                return;
            }

            control.Tag = this.FindFocusedControl();
            control.DropDownClosed += ToolStripSplitButtonDropDownClosed;
        }

        private static void ToolStripSplitButtonDropDownClosed(object sender, EventArgs e)
        {
            if (sender is ToolStripSplitButton control)
            {
                control.DropDownClosed -= ToolStripSplitButtonDropDownClosed;

                if (control.Tag is Control controlToFocus)
                {
                    controlToFocus.Focus();
                    control.Tag = null;
                }
            }
        }

        private void toolStripButtonLevelUp_DropDownOpening(object sender, EventArgs e)
        {
            PreventToolStripSplitButtonClosing(sender as ToolStripSplitButton);
        }

        #region Submodules

        private ToolStripItem CreateSubmoduleMenuItem(SubmoduleInfo info, string textFormat = "{0}")
        {
            var item = new ToolStripMenuItem(string.Format(textFormat, info.Text))
            {
                Width = 200,
                Tag = info.Path,
                Image = Images.FolderSubmodule
            };

            if (info.Bold)
            {
                item.Font = new Font(item.Font, FontStyle.Bold);
            }

            item.Click += SubmoduleToolStripButtonClick;

            return item;
        }

        private void UpdateSubmoduleMenuItemStatus(ToolStripItem item, SubmoduleInfo info, string textFormat = "{0}")
        {
            if (info.Detailed is not null)
            {
                item.Image = GetSubmoduleItemImage(info.Detailed);
                item.Text = string.Format(textFormat, info.Text + info.Detailed.AddedAndRemovedText);
            }

            return;

            static Image GetSubmoduleItemImage(DetailedSubmoduleInfo details)
            {
                return (details.Status, details.IsDirty) switch
                {
                    (null, _) => Images.FolderSubmodule,
                    (SubmoduleStatus.FastForward, true) => Images.SubmoduleRevisionUpDirty,
                    (SubmoduleStatus.FastForward, false) => Images.SubmoduleRevisionUp,
                    (SubmoduleStatus.Rewind, true) => Images.SubmoduleRevisionDownDirty,
                    (SubmoduleStatus.Rewind, false) => Images.SubmoduleRevisionDown,
                    (SubmoduleStatus.NewerTime, true) => Images.SubmoduleRevisionSemiUpDirty,
                    (SubmoduleStatus.NewerTime, false) => Images.SubmoduleRevisionSemiUp,
                    (SubmoduleStatus.OlderTime, true) => Images.SubmoduleRevisionSemiDownDirty,
                    (SubmoduleStatus.OlderTime, false) => Images.SubmoduleRevisionSemiDown,
                    (_, true) => Images.SubmoduleDirty,
                    (_, false) => Images.FileStatusModified
                };
            }
        }

        private void UpdateSubmodulesStructure()
        {
            var updateStatus = AppSettings.ShowSubmoduleStatus && _gitStatusMonitor.Active;

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    await TaskScheduler.Default;
                    await _submoduleStatusProvider.UpdateSubmodulesStructureAsync(Module.WorkingDir, TranslatedStrings.NoBranch, updateStatus);
                }
                catch (GitConfigurationException ex)
                {
                    await this.SwitchToMainThreadAsync();
                    MessageBoxes.ShowGitConfigurationExceptionMessage(this, ex);
                }
            });
        }

        private void SubmoduleStatusProvider_StatusUpdating(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await this.SwitchToMainThreadAsync();
            }).FileAndForget();
        }

        private void SubmoduleStatusProvider_StatusUpdated(object sender, SubmoduleStatusEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                if (e.StructureUpdated || _currentSubmoduleMenuItems is null)
                {
                    _currentSubmoduleMenuItems = await PopulateToolbarAsync(e.Info, e.Token);
                }

                await UpdateSubmoduleMenuStatusAsync(e.Info, e.Token);
            }).FileAndForget();
        }

        private async Task<List<ToolStripItem>> PopulateToolbarAsync(SubmoduleInfoResult result, CancellationToken cancelToken)
        {
            await this.SwitchToMainThreadAsync(cancelToken);

            var newItems = result.OurSubmodules
                .Select(submodule => CreateSubmoduleMenuItem(submodule))
                .ToList();

            if (result.OurSubmodules.Count == 0)
            {
                newItems.Add(new ToolStripMenuItem(_noSubmodulesPresent.Text));
            }

            if (result.SuperProject is not null)
            {
                newItems.Add(new ToolStripSeparator());

                if (result.TopProject is not null && result.TopProject != result.SuperProject)
                {
                    newItems.Add(CreateSubmoduleMenuItem(result.TopProject, _topProjectModuleFormat.Text));
                }

                newItems.Add(CreateSubmoduleMenuItem(result.SuperProject, _superprojectModuleFormat.Text));
                newItems.AddRange(result.AllSubmodules.Select(submodule => CreateSubmoduleMenuItem(submodule)));
            }

            newItems.Add(new ToolStripSeparator());

            if (result.CurrentSubmoduleName is not null)
            {
                var item = new ToolStripMenuItem(_updateCurrentSubmodule.Text)
                {
                    Width = 200,
                    Tag = Module.WorkingDir,
                    Image = Images.FolderSubmodule
                };
                item.Click += UpdateSubmoduleToolStripMenuItemClick;
                newItems.Add(item);
            }

            return newItems;
        }

        private async Task UpdateSubmoduleMenuStatusAsync(SubmoduleInfoResult result, CancellationToken cancelToken)
        {
            if (_currentSubmoduleMenuItems is null)
            {
                return;
            }

            await this.SwitchToMainThreadAsync(cancelToken);

            Validates.NotNull(result.TopProject);
            var infos = result.AllSubmodules.ToDictionary(info => info.Path, info => info);
            infos[result.TopProject.Path] = result.TopProject;
            foreach (var item in _currentSubmoduleMenuItems)
            {
                var path = item.Tag as string;
                if (GitExtUtils.Strings.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                if (infos.ContainsKey(path))
                {
                    UpdateSubmoduleMenuItemStatus(item, infos[path]);
                }
                else
                {
                    Debug.Fail($"Status info for {path} ({1 + result.AllSubmodules.Count} records) has no match in current nodes ({_currentSubmoduleMenuItems.Count})");
                    break;
                }
            }
        }

        #endregion

        private void reportAnIssueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserEnvironmentInformation.CopyInformation();
            OsShellUtil.OpenUrlInDefaultBrowser(@"https://github.com/gitextensions/gitextensions/issues");
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var updateForm = new FormUpdates(AppSettings.AppVersion);
            updateForm.SearchForUpdatesAndShow(Owner, true);
        }

        private void toolStripButtonPull_DropDownOpened(object sender, EventArgs e)
        {
            PreventToolStripSplitButtonClosing(sender as ToolStripSplitButton);
        }

        private void menuitemSparseWorkingCopy_Click(object sender, EventArgs e)
        {
            UICommands.StartSparseWorkingCopyDialog(this);
        }

        private void toolStripMenuItemReflog_Click(object sender, EventArgs e)
        {
            using var formReflog = new FormReflog(UICommands);
            formReflog.ShowDialog();
        }

        #region Layout management

        private void toggleSplitViewLayout_Click(object sender, EventArgs e)
        {
            AppSettings.ShowSplitViewLayout = !AppSettings.ShowSplitViewLayout;
            DiagnosticsClient.TrackEvent("Layout change",
                new Dictionary<string, string> { { nameof(AppSettings.ShowSplitViewLayout), AppSettings.ShowSplitViewLayout.ToString() } });

            RefreshSplitViewLayout();
        }

        private void toggleBranchTreePanel_Click(object sender, EventArgs e)
        {
            MainSplitContainer.Panel1Collapsed = !MainSplitContainer.Panel1Collapsed;
            DiagnosticsClient.TrackEvent("Layout change",
                new Dictionary<string, string> { { "ShowLeftPanel", MainSplitContainer.Panel1Collapsed.ToString() } });

            RefreshLayoutToggleButtonStates();
        }

        private void CommitInfoBelowClick(object sender, EventArgs e) =>
            SetCommitInfoPosition(CommitInfoPosition.BelowList);

        private void CommitInfoLeftwardClick(object sender, EventArgs e) =>
            SetCommitInfoPosition(CommitInfoPosition.LeftwardFromList);

        private void CommitInfoRightwardClick(object sender, EventArgs e) =>
            SetCommitInfoPosition(CommitInfoPosition.RightwardFromList);

        private void SetCommitInfoPosition(CommitInfoPosition position)
        {
            AppSettings.CommitInfoPosition = position;
            DiagnosticsClient.TrackEvent("Layout change",
                new Dictionary<string, string> { { nameof(AppSettings.CommitInfoPosition), AppSettings.CommitInfoPosition.ToString() } });

            RefreshLayoutToggleButtonStates();
        }

        private void RefreshSplitViewLayout()
        {
            RightSplitContainer.Panel2Collapsed = !AppSettings.ShowSplitViewLayout;
            DiagnosticsClient.TrackEvent("Layout change",
                new Dictionary<string, string> { { nameof(AppSettings.ShowSplitViewLayout), AppSettings.ShowSplitViewLayout.ToString() } });

            RefreshLayoutToggleButtonStates();
        }

        private void RefreshLayoutToggleButtonStates()
        {
        }

        #endregion

        private void manageWorktreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var formManageWorktree = new FormManageWorktree(UICommands);
            formManageWorktree.ShowDialog(this);
        }

        private void createWorktreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var formCreateWorktree = new FormCreateWorktree(UICommands);
            var dialogResult = formCreateWorktree.ShowDialog(this);
            if (dialogResult == DialogResult.OK && formCreateWorktree.OpenWorktree)
            {
                var newModule = new GitModule(formCreateWorktree.WorktreeDirectory);
                SetGitModule(this, new GitModuleEventArgs(newModule));
            }
        }

        private void toolStripSplitStash_DropDownOpened(object sender, EventArgs e)
        {
            PreventToolStripSplitButtonClosing(sender as ToolStripSplitButton);
        }

        private void undoLastCommitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AppSettings.DontConfirmUndoLastCommit || MessageBox.Show(this, _undoLastCommitText.Text, _undoLastCommitCaption.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var args = GitCommandHelpers.ResetCmd(ResetMode.Soft, "HEAD~1");
                Module.GitExecutable.GetOutput(args);
                RequestRefresh();
            }
        }
        private void FormBrowse_DragDrop(object sender, DragEventArgs e)
        {
            HandleDrop(e);
        }

        private void HandleDrop(DragEventArgs e)
        {
            if (TreeTabPage.Parent is null)
            {
                return;
            }

            var itemPath = (e.Data.GetData(DataFormats.Text) ?? e.Data.GetData(DataFormats.UnicodeText)) as string;
            if (IsFileExistingInRepo(itemPath))
            {
                CommitInfoTabControl.SelectedTab = TreeTabPage;
                fileTree.SelectFileOrFolder(itemPath);
                return;
            }

            if (e.Data.GetData(DataFormats.FileDrop) is not string[] paths)
            {
                return;
            }

            foreach (string path in paths)
            {
                if (!IsFileExistingInRepo(path))
                {
                    continue;
                }

                if (CommitInfoTabControl.SelectedTab != TreeTabPage)
                {
                    CommitInfoTabControl.SelectedTab = TreeTabPage;
                }

                if (fileTree.SelectFileOrFolder(path))
                {
                    return;
                }
            }

            bool IsPathExists([NotNullWhen(returnValue: true)] string? path) => path is not null && (File.Exists(path) || Directory.Exists(path));

            bool IsFileExistingInRepo([NotNullWhen(returnValue: true)] string? path) => IsPathExists(path) && path.StartsWith(Module.WorkingDir, StringComparison.InvariantCultureIgnoreCase);
        }

        private void FormBrowse_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)
                || e.Data.GetDataPresent(DataFormats.Text)
                || e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void TsmiTelemetryEnabled_Click(object sender, EventArgs e)
        {
            UICommands.StartGeneralSettingsDialog(this);
        }

        private void HelpToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
        }

        private void tsbShowRepos_Click(object sender, EventArgs e)
        {
            // TODO: throw new NotImplementedException();
            if (MainSplitContainer.Panel1Collapsed == false)
            {
                MainSplitContainer.Panel1Collapsed = true;
                MainSplitContainer.Panel1.Hide();
            }
            else
            {
                MainSplitContainer.Panel1Collapsed = false;
                MainSplitContainer.Panel1.Show();
            }
        }
    }
}
