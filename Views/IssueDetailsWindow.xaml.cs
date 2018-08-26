using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KalymnosBT.Models;
using KalymnosBT.Settings;
using KalymnosBT.ViewModels;

namespace KalymnosBT.Views
{
    /// <summary>
    /// Interaction logic for IssueDetailsWindow.xaml
    /// </summary>
    public partial class IssueDetailsWindow : Window
    {
        public bool WasDelBtnClicked { get; private set; }
        public bool WasRestoreBtnClicked { get; private set; }
        public IssueDetailsWindow()
        {
            InitializeComponent();
        }

        private void OnBtnSave(object sender, RoutedEventArgs e)
        {
            var viewModel = (DataContext as IssueDetailsViewModel);
            viewModel.AcceptChanges();
            viewModel.Model.AcceptChanges();
            DialogResult = true;
        }

        private void OnBtnDelete(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                throw new NullReferenceException("DataCotext is null");
            }

            if (!(DataContext is IssueDetailsViewModel))
            {
                throw new InvalidDataException("DataContext is not Vote!");
            }

            var viewModel = (DataContext as IssueDetailsViewModel);

            var issue = viewModel.Model;

            var message = viewModel.IsTrashed
                ? $"Are you sure you want to permanently delte the issue {issue.DisplayId}? This operation cannot be undone."
                : $"Are you sure you want to move the issue {issue.DisplayId} to trash?";
            
            if (MessageBoxResult.Yes == 
                MessageBox.Show(this, message, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No))
            {
                WasDelBtnClicked = true;
                viewModel.AcceptChanges();
                viewModel.Model.AcceptChanges();
                DialogResult = true;
            }

        }

        private void OnBtnRestore(object sender, RoutedEventArgs e)
        {
            WasDelBtnClicked = false;
            WasRestoreBtnClicked = true;
            var viewModel = (DataContext as IssueDetailsViewModel);
            viewModel.AcceptChanges();
            viewModel.Model.AcceptChanges();
            DialogResult = true;
        }

        private void OnWindowContentRendered(object sender, EventArgs e)
        {
            detailsGridColumn.Width = new GridLength(DesktopSettings.Default.IssueWindowDetailsPanelWidth);
            var viewModel = (DataContext as IssueDetailsViewModel);
            if (viewModel != null)
            {
                viewModel.AcceptChanges();
                viewModel.Model.AcceptChanges();
            }
            
        }

    

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DesktopSettings.Default.IssueWindowDetailsPanelWidth = detailsGridColumn.ActualWidth;


            var viewModel = (DataContext as IssueDetailsViewModel);

            if (viewModel.WasModified())
            {
                var result =
                    MessageBox.Show(this,
                        "Changes not saved, do you want to save them?",
                        "Confirmation", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (result == MessageBoxResult.No)
                {
                    e.Cancel = false;
                    DialogResult = false;
                }
                else
                {
                    DialogResult = true;
                }
            }
        }
    }
}
