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

namespace KalymnosBT.Views
{
    /// <summary>
    /// Interaction logic for VoteEditWindow.xaml
    /// </summary>
    public partial class VoteEditWindow : Window
    {
        public VoteEditWindow()
        {
            InitializeComponent();

        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (!ShowDelButton)
                btnDelete.Visibility = Visibility.Hidden;
        }

        private void OnBtnOk(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnBtnDelete(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                throw new NullReferenceException("DataCotext is null");
            }

            if (!(DataContext is Vote))
            {
                throw new InvalidDataException("DataContext is not Vote!");
            }

            var vote = (Vote) DataContext;

            if (MessageBoxResult.Yes == MessageBox.Show(this, $"Are you sure you want to delete the vote from {vote.Name} <{vote.Email}>?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No))
            {
                WasDelBtnClicked = true;
                DialogResult = true;
            }

        }

        public bool WasDelBtnClicked { get; private set; } = false;
        public bool ShowDelButton { get; set; } = true;
    }
}
