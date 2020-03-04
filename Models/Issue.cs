using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using KalymnosBT.MVVMBase;
using KalymnosBT.Properties;
using Newtonsoft.Json;

namespace KalymnosBT.Models
{
    public enum IssueStatus : int
    {
        Unknown = 0,
        Active = 1,
        Closed = 2
    }

    public class IssueIsFixedToTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TextDecorations.Strikethrough : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }

    public class IssueIsFixedToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.DarkGray: Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return false;
        }
    }

    public class IssueStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IssueStatus)) { return null; }

            var status = (IssueStatus) value;
            switch (status)
            {
                case IssueStatus.Unknown:
                    return "Unknown";
                case IssueStatus.Active:
                    return "Active";
                case IssueStatus.Closed:
                    return "Closed";
            }
            throw new InvalidOperationException("Enum value is unknown.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return IssueStatus.Unknown;

            var status = (string) value;
            if (string.Compare("Active", status, StringComparison.InvariantCultureIgnoreCase) == 0)
                return IssueStatus.Active;
            if (string.Compare("Closed", status, StringComparison.InvariantCultureIgnoreCase) == 0)
                return IssueStatus.Closed;

            throw new InvalidOperationException("Wrong enum value");

        }
    }

    public class Issue : ObservableObject
    {
        #region Member variables

        private ulong _issueId;
        private string _title = string.Empty;
        private string _details = string.Empty;
        private IssueStatus _status = IssueStatus.Active;

        private DateTime _created = DateTime.UtcNow;
        private DateTime _modified = DateTime.UtcNow;
        private DateTime _fixed = DateTime.MaxValue;

        private bool _starred;
        private bool _important;

      #endregion

        #region Properties
        public ulong IssueId { get => _issueId; set => SetProperty(ref _issueId, value, "DisplayId"); }
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Details { get => _details; set => SetProperty(ref _details, value); }
        public IssueStatus Status { get => _status; set { SetProperty(ref _status, value); OnPropertyChanged("IsFixed");} }

        public string DisplayId => this.Project != null ? $"{this.Project.Prefix}-{IssueId}" : IssueId.ToString();

        public DateTime Created { get => _created; set => SetProperty(ref _created, value); }
        public string CreatedAsString { get => Created.ToString("d", CultureInfo.CurrentCulture); }
        public DateTime Modified { get => _modified; set => SetProperty(ref _modified, value); }
        public string ModifiedAsString { get => Modified.ToString("d", CultureInfo.CurrentCulture); }
        public DateTime Fixed { get => _fixed; set => SetProperty(ref _fixed, value); }

        public bool Important { get => _important; set => SetProperty(ref _important, value); }
        public bool Starred { get => _starred; set => SetProperty(ref _starred, value); }

        [JsonIgnore]
        public bool IsBacklogged
        {
            get => Project?.Backlog.Contains(this) ?? false;
            set
            {
                if (value && !IsBacklogged)
                {
                    Project?.Backlog.Add(this);
                }

                if (!value)
                {
                    Project?.Backlog.Remove(this);
                }
                OnPropertyChanged();
            }
        }


        public bool IsFixed => Status == IssueStatus.Closed;

        public Project Project { get; set; }

        public ObservableCollection<Comment> Comments { get; } = new ObservableCollection<Comment>();
        public ObservableCollection<Vote> Votes { get; } = new ObservableCollection<Vote>();

        #endregion

        public Issue()
        {
            
        }

        public Issue(Issue source)
        {
            if (source != null)
                CopyFrom(source);
        }

        public bool Match(string query)
        {
            query = query.ToUpperInvariant();

            if (Title != null && Title.ToUpperInvariant().Contains(query))
                return true;

            if (Details != null && Details.ToUpperInvariant().Contains(query))
                return true;

            if (DisplayId != null && DisplayId.ToUpperInvariant().Contains(query))
                return true;

            if (Comments != null)
            {
                foreach (var comment in Comments)
                {
                    if (comment.Text.ToUpperInvariant().Contains(query))
                        return true;
                }
            }

            if (Votes != null)
            {
                foreach (var vote in Votes)
                {
                    if (vote.Name != null && vote.Name.ToUpperInvariant().Contains(query))
                        return true;

                    if (vote.Email != null && vote.Email.ToUpperInvariant().Contains(query))
                        return true;
                }
            }
            return false;
        }

        public void CopyFrom(Issue src)
        {
            if (src == null)
                throw new NullReferenceException("Source of the issue is null");

            IssueId = src.IssueId;
            Title = src.Title;
            Details = src.Details;

            Created = src.Created;
            Modified = src.Modified;
            Fixed = src.Fixed;

            Status = src.Status;
            Important = src.Important;
            Starred = src.Starred;

            Project = src.Project;

            Comments.Clear();
            Votes.Clear();

            
            foreach (var comment in src.Comments)
            {
                
                Comments.Add(new Comment{Text = comment.Text, Date = comment.Date});
            }

            foreach (var vote in src.Votes)
            {
                Votes.Add(new Vote{Name = vote.Name, Email = vote.Email, When = vote.When});
            }
        }
    }
}
