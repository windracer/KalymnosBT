using System;
using System.Collections.Generic;
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

        public void SortIssues()
        {
            if (_viewModel?.SelectedProject?.Issues == null)
                return;

            var view = CollectionViewSource.GetDefaultView(_viewModel.SelectedProject.Issues);
            view.SortDescriptions.Clear();

            if (sortFieldCombo.SelectedIndex == 0)
            {
                view.SortDescriptions.Add(new SortDescription("IssueId", ListSortDirection.Ascending));
                var liveView = (ICollectionViewLiveShaping)view;
                liveView.IsLiveFiltering = true;

            }
            else
            {
                view.SortDescriptions.Add(new SortDescription("Votes.Count", ListSortDirection.Descending));
                var liveView = (ICollectionViewLiveShaping)view;
                liveView.IsLiveFiltering = true;
            }

        }

        public void FilterIssues()
        {
            var view = CollectionViewSource.GetDefaultView(_viewModel.SelectedProject.Issues);

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
    }
}
