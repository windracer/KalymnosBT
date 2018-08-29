using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using KalymnosBT.Models;
using KalymnosBT.MVVMBase;
using KalymnosBT.Settings;
using KalymnosBT.Views;

namespace KalymnosBT.ViewModels
{
    public class MainViewModel : ViewModelBase<KalymnosDb>
    {
        private enum DataSources : int
        {
            Issues = 0,
            Backlog = 1,
            Trash = 2
        }

        private enum IssueSortFields : int
        {
            IssueId = 0,
            NumberOfVotes = 1
        }

        private string _mainWndTitle;

        public string MainWndTitle => string.IsNullOrEmpty(_mainWndTitle) ? (_mainWndTitle = $"KalymnosBT - Bug tracker for indi-(e)-viduals // {KalymnosDbHelper.DbFile}") : _mainWndTitle;

        public DesktopSettings WindowsSettings => DesktopSettings.Default; 
        
        public Window TheView { get; set; }

        private Project _selectedProject;

        public Project SelectedProject { get => _selectedProject; set => SetProperty(ref _selectedProject, value); }

        private bool _showFixedIssues;
        public bool ShowFixedIssues { get => _showFixedIssues; set => SetProperty(ref _showFixedIssues, value); }

        private Issue _selectedIssue;
        public Issue SelectedIssue { get => _selectedIssue; set { SetProperty(ref _selectedIssue, value); OnPropertyChanged("SelectedIssueCommentsCount"); OnPropertyChanged("SelectedIssueVotesCount"); } }

        public int SelectedIssueCommentsCount => SelectedIssue?.Comments.Count() ?? 0;
        public int SelectedIssueVotesCount => SelectedIssue?.Votes.Count() ?? 0;

        public bool ListOfIssuesChanged => true;

        private DataSources _selectedDataSource; //0 - issues, 1 - backlog, 2 - trash
        public int SelectedDataSource { get => (int) _selectedDataSource; set => SetProperty(ref _selectedDataSource, (DataSources) value); }

        private IssueSortFields _issueSortField; //0 - issue id, 1 - number of votes
        public int IssueSortField { get => (int) _issueSortField; set => SetProperty(ref _issueSortField, (IssueSortFields) value); }

        private ICommand _showBackstageWindowCommand;
        public ICommand ShowBackstageWindowCommand
        {
            get
            {
                return _showBackstageWindowCommand ??
                       (
                           _showBackstageWindowCommand = new RelayCommand(
                               () =>
                               {
                                   var backstageViewModel = new BackstageViewModel {TheView = this.TheView, Model = this.Model};
                                   var dlg = new BackstageWindow{Owner = TheView, DataContext = backstageViewModel};

                                   dlg.ShowDialog();

                                   var listBox = ((MainWindow) TheView).listBoxIssues;
                                   var tmpDataContext = listBox.DataContext;
                                   listBox.DataContext = null;
                                   listBox.DataContext = tmpDataContext;

                                   var tmpIssue = SelectedIssue;
                                   SelectedIssue = null;
                                   SelectedIssue = tmpIssue;
                               })
                       );
            }
        }

        private ICommand _editIssueCommand;
        public ICommand EditIssueCommand
        {
            get
            {
                return _editIssueCommand ??
                       (_editIssueCommand = new RelayCommand(
                           () =>
                           {
                               var dlg = new IssueDetailsWindow{Owner = TheView};
                               var issueViewModel = new IssueDetailsViewModel(SelectedIssue);
                               issueViewModel.TheView = dlg;
                               issueViewModel.IsEditing = true;
                               issueViewModel.IsTrashed = (SelectedDataSource == (int) DataSources.Trash);


                               dlg.DataContext = issueViewModel;
                               if (dlg.ShowDialog() == true)
                               {
                                   if (SelectedIssue != null)
                                   {
                                       SelectedIssue.CopyFrom(issueViewModel.Model);
                                       SelectedIssue.Modified = DateTime.UtcNow;
                                   }

                                   if (dlg.WasDelBtnClicked)
                                   {
                                       if (issueViewModel.IsTrashed)
                                       {
                                           // Delete permanently:
                                           SelectedProject.Trash.Remove(SelectedIssue);
                                           SelectedProject.Backlog.Remove(SelectedIssue);
                                       }
                                       else
                                       {
                                           // Move to trash:
                                           SelectedProject.Trash.Add(SelectedIssue);
                                           SelectedProject.Backlog.Remove(SelectedIssue);
                                           SelectedProject.Issues.Remove(SelectedIssue);
                                       }
                                       
                                   }
                                   else if (dlg.WasRestoreBtnClicked)
                                   {
                                       // Restore from trash:
                                       SelectedProject.Issues.Add(SelectedIssue);
                                       SelectedProject.Trash.Remove(SelectedIssue);
                                   }

                                   KalymnosDbHelper.Save(Model);
                                   
                                   OnPropertyChanged("SelectedIssueCommentsCount");
                                   OnPropertyChanged("SelectedIssueVotesCount");
                                   OnPropertyChanged("ShowFixedIssues");
                               }
                           }));
            }
        }

        private ICommand _createIssueCommand;
        public ICommand CreateIssueCommand
        {
            get
            {
                return _createIssueCommand ??
                       (_createIssueCommand = new RelayCommand(
                           () =>
                           {

                               var issue = new Issue
                               {
                                   IssueId = SelectedProject.LastIssueNumber + 1,
                                   Created = DateTime.UtcNow,
                                   Modified = DateTime.UtcNow,
                                   Project = SelectedProject,
                                   
                               };
                               
                               var issueViewModel = new IssueDetailsViewModel(issue);
                               issueViewModel.IsEditing = false;
                               issueViewModel.IsTrashed = false;

                               var dlg = new IssueDetailsWindow{Owner = TheView, DataContext = issueViewModel};
                               issueViewModel.TheView = dlg;

                               if (dlg.ShowDialog() == true)
                               {
                                   issue.CopyFrom(issueViewModel.Model);

                                   if (issue.IssueId > SelectedProject.LastIssueNumber)
                                       SelectedProject.LastIssueNumber = issue.IssueId;

                                   SelectedProject.Issues.Add(issue);
                                   SelectedIssue = issue;
                                   KalymnosDbHelper.Save(Model);

                                   OnPropertyChanged("ShowFixedIssues");
                               }
                           }));
            }
        }

        public UndoManager UndoManager { get; } = new UndoManager();

        private ICommand _undoCommand;

        public ICommand UndoCommand
        {
            get
            {
                return _undoCommand ?? (_undoCommand = new RelayCommand(
                           () => UndoManager.Undo(),
                           () => UndoManager.CanUndo));
            }
        }

    }
}
