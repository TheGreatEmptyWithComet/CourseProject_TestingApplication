﻿using System;
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

namespace TestingServerApp.Viewes
{
    /// <summary>
    /// Interaction logic for TestIssuesDirectoryPage.xaml
    /// </summary>
    public partial class TestIssuesDirectoryPage : Page
    {
        public TestIssuesDirectoryPage()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (DataContext as BaseVM).IssueTestsVM.EditIssuedTestCommand.Execute(null);
        }
    }
}
