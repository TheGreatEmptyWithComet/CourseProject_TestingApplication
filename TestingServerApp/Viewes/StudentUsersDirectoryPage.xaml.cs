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
    /// Interaction logic for StudentUsersDirectoryPage.xaml
    /// </summary>
    public partial class StudentUsersDirectoryPage : Page
    {
        public StudentUsersDirectoryPage()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (DataContext as BaseVM).UserMaterialsVM.EditUserGroupCommand.Execute(null);
        }

        private void DataGrid1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (DataContext as BaseVM).UserMaterialsVM.EditUserCommand.Execute(null);
        }
    }
}
