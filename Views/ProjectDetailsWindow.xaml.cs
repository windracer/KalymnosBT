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
    /// Interaction logic for ProjectDetailsWindow.xaml
    /// </summary>
    public partial class ProjectDetailsWindow : Window
    {
        public bool WasDelButtonPressed { get; private set; } = false;
        public bool ShowDelButton { get; set; } = true;
        public ProjectDetailsWindow()
        {
            InitializeComponent();
        }

        private void OnBtnDelete(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                throw new NullReferenceException("DataCotext is null");
            }

            if (!(DataContext is Project))
            {
                throw new InvalidDataException("DataContext is not Project!");
            }

            var project = (Project)DataContext;

            if (MessageBoxResult.Yes == MessageBox.Show(this, $"Are you sure you want to delete project \"{project.Name}\" ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No))
            {
                WasDelButtonPressed = true;
                DialogResult = true;
            }

        }

        private void OnBtnOk(object sender, RoutedEventArgs e)
        {

            DialogResult = true;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (!ShowDelButton)
                btnDelete.Visibility = Visibility.Collapsed;
        }

    }
}
