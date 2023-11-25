using System;
using System.Collections.Generic;
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

namespace TestingClientApp
{
    /// <summary>
    /// Interaction logic for MessageShowDialogWindow.xaml
    /// </summary>
    public partial class MessageShowDialogWindow : Window
    {
        public MessageShowDialogWindow(string messageText)
        {
            InitializeComponent();
            this.DataContext = messageText;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
