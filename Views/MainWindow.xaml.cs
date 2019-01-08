using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KalymnosBT.Models;
using KalymnosBT.Settings;
using KalymnosBT.ViewModels;

namespace KalymnosBT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel { TheView = this, Model = KalymnosDbHelper.Load() };
            DataContext = _viewModel;

            if (!string.IsNullOrEmpty(DesktopSettings.Default.LastSelectedProject))
            {
                _viewModel.SelectedProject =
                    _viewModel.Model.Projects.FirstOrDefault(
                        p => p.Prefix == DesktopSettings.Default.LastSelectedProject);

                if (_viewModel.SelectedProject == null && _viewModel.Model.Projects.Count > 0)
                    _viewModel.SelectedProject = _viewModel.Model.Projects[0];
            }
            else
            {
                _viewModel.SelectedProject = _viewModel.Model.Projects.First();
            }

            _viewModel.SelectedIssue = _viewModel.SelectedProject?.Issues.FirstOrDefault();
            _viewModel.PropertyChanged += ViewModelPropertyChanged;
            FilterIssues();
            SortIssues();
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ShowFixedIssues":
                    FilterIssues();
                    break;
            }
        }

        ObservableCollection<Issue> GetSelectedDataSource()
        {
            if (1 /*Backlog*/ == _viewModel?.SelectedDataSource)
                return _viewModel?.SelectedProject?.Backlog;

            if (2 /*Trash*/ == _viewModel?.SelectedDataSource)
                return _viewModel?.SelectedProject?.Trash;

            return _viewModel?.SelectedProject?.Issues;
        }

        public void SortIssues()
        {
            var selectedDataSource = GetSelectedDataSource();
            if (null == selectedDataSource)
                return;

            var view = CollectionViewSource.GetDefaultView(selectedDataSource);
            view.SortDescriptions.Clear();

            if (sortFieldCombo.SelectedIndex == 0) // Issue Id
            {
                view.SortDescriptions.Add(new SortDescription("IssueId", ListSortDirection.Ascending));
                var liveView = (ICollectionViewLiveShaping)view;
                liveView.IsLiveFiltering = true;

            }
            else if (sortFieldCombo.SelectedIndex == 1) // Votes Count
            {
                view.SortDescriptions.Add(new SortDescription("Votes.Count", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("Starred", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("Important", ListSortDirection.Descending));
                var liveView = (ICollectionViewLiveShaping)view;
                liveView.IsLiveFiltering = true;
            }
            else if (sortFieldCombo.SelectedIndex == 2) // Stars -> Id
            {
                view.SortDescriptions.Add(new SortDescription("Starred", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("Important", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("IssueId", ListSortDirection.Ascending));

                var liveView = (ICollectionViewLiveShaping) view;
                liveView.IsLiveFiltering = true;
            }
            else if (sortFieldCombo.SelectedIndex == 3) // Importance -> Id
            {
                view.SortDescriptions.Add(new SortDescription("Important", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("Starred", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("IssueId", ListSortDirection.Ascending));

                var liveView = (ICollectionViewLiveShaping) view;
                liveView.IsLiveFiltering = true;
            }
            else if (sortFieldCombo.SelectedIndex == 4) // Stars -> Votes
            {
                view.SortDescriptions.Add(new SortDescription("Starred", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("Votes.Count", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("Important", ListSortDirection.Descending));

                var liveView = (ICollectionViewLiveShaping) view;
                liveView.IsLiveFiltering = true;
            }
            else if (sortFieldCombo.SelectedIndex == 5) // Importance -> Votes
            {
                view.SortDescriptions.Add(new SortDescription("Important", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("Votes.Count", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("Starred", ListSortDirection.Descending));

                var liveView = (ICollectionViewLiveShaping) view;
                liveView.IsLiveFiltering = true;
            }

        }

        public void FilterIssues()
        {
            var selectedDataSource = GetSelectedDataSource();
            if (null == selectedDataSource)
                return;

            var view = CollectionViewSource.GetDefaultView(selectedDataSource);

            if (string.IsNullOrWhiteSpace(filterText.Text))
            {
                view.Filter = null;
                if (!_viewModel.ShowFixedIssues)
                {
                    view.Filter = obj => !(obj as Issue).IsFixed;
                    var liveView = (ICollectionViewLiveShaping) view;
                    liveView.IsLiveFiltering = true;
                }
            }
            else
            {
                view.Filter = obj => (obj as Issue).Match(filterText.Text)
                                     && (_viewModel.ShowFixedIssues || !(obj as Issue).IsFixed);

                var liveView = (ICollectionViewLiveShaping)view;
                liveView.IsLiveFiltering = true;

            }
        }

        private void OnBtnFilter(object sender, RoutedEventArgs e)
        {
            FilterIssues();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            DesktopSettings.Default.Save();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            var w = detailsPanelGridColumn.ActualWidth;
            DesktopSettings.Default.MainWindowDetailsPanelWidth = w;

            if (_viewModel.SelectedProject != null)
                DesktopSettings.Default.LastSelectedProject = _viewModel.SelectedProject.Prefix;

            DesktopSettings.Default.ShowFixedIssuesSelected = _viewModel.ShowFixedIssues;
        }

        private void OnWindowContentRendered(object sender, EventArgs e)
        {
            var width = new GridLength(DesktopSettings.Default.MainWindowDetailsPanelWidth);
            detailsPanelGridColumn.Width = width;

            if (DesktopSettings.Default.ShowFixedIssuesSelected)
                checkBoxShowFixedIssues.IsChecked = true;

        }

        private void OnComboSortOrderSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortIssues();
        }

        private void OnComboDataSourceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterIssues();
            SortIssues();
        }
    }
}
