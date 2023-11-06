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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestingServerApp.Styles
{
    /// <summary>
    /// Interaction logic for TestsListPage.xaml
    /// </summary>
    public partial class TestsListPage : Page
    {
        public TestsListPage()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (DataContext as BaseVM).TestMaterialsVM.EditCommand.Execute(null);
        }
    }
}
