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
    /// Interaction logic for CommentEditWindow.xaml
    /// </summary>
    public partial class CommentEditWindow : Window
    {
        public CommentEditWindow()
        {
            InitializeComponent();
        }

        private void OnBtnOk(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnBtnDelete(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show(this, $"Are you sure you want to delete this comment?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No))
            {
                WasDelPressed = true;
                DialogResult = true;
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (!ShowDelButton)
                btnDelete.Visibility = Visibility.Hidden;
        }

        public bool ShowDelButton { get; set; } = true;
        public bool WasDelPressed { get; private set; }
    }
}
