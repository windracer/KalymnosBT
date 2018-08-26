using System;
using System.Collections.ObjectModel;
using System.Linq;
using KalymnosBT.MVVMBase;

namespace KalymnosBT.Models
{
    public class Project : ObservableObject
    {
        private string _name;
        private string _prefix;
        private ulong _lastIssueNumber;

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Prefix { get => _prefix; set => SetProperty(ref _prefix, value); }
        public ulong LastIssueNumber { get => _lastIssueNumber; set => SetProperty(ref _lastIssueNumber, value); }

        public int FixedIssuesCount { get => Issues.Count(i => i.IsFixed); } 

        public ObservableCollection<Issue> Issues { get; } = new ObservableCollection<Issue>();
        public ObservableCollection<Issue> Backlog { get; } = new ObservableCollection<Issue>();
        public ObservableCollection<Issue> Trash { get; } = new ObservableCollection<Issue>();
    }
}
