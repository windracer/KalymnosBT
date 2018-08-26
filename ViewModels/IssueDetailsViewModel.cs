using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using KalymnosBT.Models;
using KalymnosBT.MVVMBase;
using KalymnosBT.Views;

namespace KalymnosBT.ViewModels
{

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }

    public class IssueDetailsViewModel : ViewModelBase<Issue>
    {
        private Issue _originalIssueRef;
        public IssueDetailsViewModel(Issue source)
        {
            Model = new Issue(source);
            _originalIssueRef = source;

            //PropertyChanged += ThisPropertyChanged;
        }

        public Window TheView { get; set; }

        //void ThisPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {
        //        case "CurrentStatus":
        //            Model.Fixed = DateTime.UtcNow;
        //            break;
        //    }
        //}

        public bool IsOriginalIssueBackloged
        {
            get => _originalIssueRef.IsBacklogged;
            set 
            {
                _originalIssueRef.IsBacklogged = value;
                OnPropertyChanged();
            }
        }

        public bool WasModified()
        {
            return IsChanged || Model.IsChanged;
        }

        private bool _isEditing;
        public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

        private bool _isTrashed;
        public bool IsTrashed { get => _isTrashed; set => SetProperty(ref _isTrashed, value); }

        public IEnumerable<IssueStatus> IssueStatusSecection
        {
            get
            {
                var status = new[]
                {
                    IssueStatus.Active,
                    IssueStatus.Closed 
                };
                return status;
            }
        }

        public string DeleteBtnTooltip => IsTrashed ? "Delete forever" : "Move to trash";

        public IssueStatus CurrentStatus
        {
            get => Model.Status;
            set
            {
                if (value != Model.Status)
                {
                    Model.Status = value;
                    Model.Fixed = DateTime.UtcNow;
                    OnPropertyChanged();
                }
            }
        }

        private Vote _selectedVote;

        public Vote SelectedVote
        {
            get => _selectedVote;
            set => SetProperty(ref _selectedVote, value);
        }

        private Comment _selectedComment;
        public Comment SelectedComment
        {
            get => _selectedComment;
            set => SetProperty(ref _selectedComment, value);
        }

        private ICommand _addVoteCommand;
        public ICommand AddVoteCommand
        {
            get
            {
                return _addVoteCommand ?? (_addVoteCommand = new RelayCommand(
                    () =>
                    {
                        var voteViewModel = new VoteEditViewModel {Model = new Vote {When = DateTime.UtcNow}};

                        dialogShow:


                        var dlg = new VoteEditWindow
                        {
                            Owner = TheView,
                            DataContext = voteViewModel,
                            ShowDelButton = false
                        };

                        
                        if (dlg.ShowDialog() == true)
                        {
                            voteViewModel.Model.Name = voteViewModel.Model.Name.Trim();
                            voteViewModel.Model.Email = voteViewModel.Model.Email.Trim();

                            var entriesCount = Model.Votes.Count(
                                vote => string.Compare(vote.Name, voteViewModel.Model.Name,
                                            StringComparison.CurrentCultureIgnoreCase) == 0
                                        || string.Compare(vote.Email, voteViewModel.Model.Email,
                                            StringComparison.CurrentCultureIgnoreCase) == 0);

                            if (entriesCount > 0)
                            {
                                MessageBox.Show(TheView,
                                    $"The voter {voteViewModel.CombinedName} already voted for this issue", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                goto dialogShow;
                            }

                            Model.Votes.Add(voteViewModel.Model);
                            IsChanged = true;
                        }
                    }));
            }
        }

        private ICommand _editVoteCommand;
        public ICommand EditVoteCommand
        {
            get
            {
                return _editVoteCommand ?? (_editVoteCommand = new RelayCommand(
                           () =>
                           {
                               var voteViewModel = new VoteEditViewModel { Model = new Vote { When = SelectedVote.When, Name = SelectedVote.Name, Email = SelectedVote.Email}};
                               var dlg = new VoteEditWindow { DataContext = voteViewModel, Owner = TheView};

                               if (dlg.ShowDialog() == true)
                               {
                                   if (dlg.WasDelBtnClicked)
                                   {
                                       Model.Votes.Remove(SelectedVote);
                                   }
                                   else
                                   {
                                       SelectedVote.Name = voteViewModel.Model.Name;
                                       SelectedVote.Email = voteViewModel.Model.Email;
                                       SelectedVote.When = voteViewModel.Model.When;
                                   }
                                   IsChanged = true;
                               }
                           }));
            }
        }

        private ICommand _addCommentCommand;
        public ICommand AddCommentCommand
        {
            get
            {
                return _addCommentCommand ?? (_addCommentCommand = new RelayCommand(
                           () =>
                           {
                               var comment = new Comment();
                               var dlg = new CommentEditWindow{DataContext = comment, ShowDelButton = false, Owner = TheView};
                               dlg.DataContext = comment;
                               if (dlg.ShowDialog() == true)
                               {
                                   comment.Date = DateTime.UtcNow;
                                   Model.Comments.Add(comment);
                                   IsChanged = true;
                               }
                           }));
            }
        }

        private ICommand _editCommentCommand;
        public ICommand EditCommentCommand
        {
            get
            {
                return _editCommentCommand ?? (_editCommentCommand = new RelayCommand(
                           () =>
                           {
                               var comment = new Comment {Text = SelectedComment?.Text};
                               var dlg = new CommentEditWindow {DataContext = comment, Owner = TheView};
                               if (SelectedComment != null && dlg.ShowDialog() == true)
                               {
                                   if (dlg.WasDelPressed)
                                   {
                                       Model.Comments.Remove(SelectedComment);
                                   }
                                   else
                                   {
                                       SelectedComment.Text = comment.Text;
                                   }
                                   IsChanged = true;

                               }
                           }));
            }
        }
    }
}
