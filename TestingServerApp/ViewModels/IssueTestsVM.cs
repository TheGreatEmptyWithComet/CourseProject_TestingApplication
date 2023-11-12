using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TestingServerApp.Viewes;

namespace TestingServerApp
{
    public class IssueTestsVM : NotifyPropertyChangeHandler
    {
        #region Properties
        /****************************************************************************************/
        private readonly Context context;
        private TestIssuesDataWindow TestIssuesDataWindow;

        public ObservableCollection<TestVM> Tests
        {
            get
            {
                if (CurrentTestCategory != null)
                {
                    return new ObservableCollection<TestVM>(context.Tests.Where((t) => t.TestCategory == CurrentTestCategory.Model).Select(i => new TestVM(i)));
                }
                else
                {
                    return new ObservableCollection<TestVM>();
                }
            }
        }

        public ObservableCollection<IssuedTestVM> IssuedTests
        {
            get
            {
                if (CurrentTest != null && OnlyActualIssuedTestsIsShown == false)
                {
                    return new ObservableCollection<IssuedTestVM>(context.IssuedTests.Where((it) => it.Test == CurrentTest.Model).Select(i => new IssuedTestVM(i)));
                }
                else if (CurrentTest != null && OnlyActualIssuedTestsIsShown == true)
                {
                    return new ObservableCollection<IssuedTestVM>(context.IssuedTests.Where((it) => it.Test == CurrentTest.Model && (DateTime.Now <= it.ExpiredDate)).Select(i => new IssuedTestVM(i)));
                }
                else
                {
                    return new ObservableCollection<IssuedTestVM>();
                }
            }
        }

        private TestCategoryVM currentTestCategory;
        public TestCategoryVM CurrentTestCategory
        {
            get => currentTestCategory;
            set
            {
                if (value != null)
                {
                    currentTestCategory = value;
                    NotifyPropertyChanged(nameof(Tests));
                    // Clear issued tests list:
                    CurrentTest = new TestVM(new Test());
                }
            }
        }

        private TestVM currentTest;
        public TestVM CurrentTest
        {
            get => currentTest;
            set
            {
                if (value != null)
                {
                    currentTest = value;
                    NotifyPropertyChanged(nameof(IssuedTests));
                }
            }
        }

        // Show issued tests that are accessible for students by expired date value
        private bool onlyActualIssuedTestsIsShown = false;
        public bool OnlyActualIssuedTestsIsShown
        {
            get => onlyActualIssuedTestsIsShown;
            set
            {
                onlyActualIssuedTestsIsShown = value;
                NotifyPropertyChanged(nameof(IssuedTests));
            }
        }

        public IssuedTestVM CurrentIssuedTest { get; set; }

        // Error message for data window
        private string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                NotifyPropertyChanged(nameof(ErrorMessage));
            }
        }
        #endregion

        #region Commands
        /****************************************************************************************/
        public ICommand AddIssuedTestCommand { get; private set; }
        public ICommand EditIssuedTestCommand { get; private set; }
        public ICommand DeleteIssuedTestCommand { get; private set; }
        public ICommand SaveIssuedTestCommand { get; private set; }

        #endregion


        #region Constructor
        /****************************************************************************************/
        public IssueTestsVM(Context context)
        {
            this.context = context;

            InitCommands();

            // Load data from DB
            context.UserGroups.Include((ug) => ug.Users).Load();

        }
        #endregion


        #region Methods
        // GENERAL 
        /****************************************************************************************/
        private void InitCommands()
        {
            AddIssuedTestCommand = new RelayCommand(AddIssuedTest);
            EditIssuedTestCommand = new RelayCommand(EditIssuedTest);
            DeleteIssuedTestCommand = new RelayCommand(DeleteIssuedTest);
            SaveIssuedTestCommand = new RelayCommand(CheckIssuedTestData);
        }
        private void SaveChanges()
        {
            try
            {
                context.SaveChanges();
                NotifyPropertyChanged(nameof(IssuedTests));
            }
            catch (Exception ex)
            {
                string innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                MessageBox.Show(ex.Message + "\n" + innerMessage, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // USER GROUPS
        /****************************************************************************************/
        private void AddIssuedTest()
        {
            // Create new entity
            IssuedTest newIssuedTest = new IssuedTest()
            {
                IssueDate = DateTime.Now,
                Test = CurrentTest.Model,
                ExpiredDate = DateTime.Now,
            };
            CurrentIssuedTest = new IssuedTestVM(newIssuedTest);
            ErrorMessage = string.Empty;

            //Create and show data window
            TestIssuesDataWindow = new TestIssuesDataWindow();
            TestIssuesDataWindow.Owner = Application.Current.MainWindow;

            if (TestIssuesDataWindow.ShowDialog() == true)
            {
                context.Add(newIssuedTest);
                SaveChanges();
            }
        }
        private void EditIssuedTest()
        {
            ErrorMessage = string.Empty;

            //Create and show window
            TestIssuesDataWindow = new TestIssuesDataWindow();
            TestIssuesDataWindow.Owner = Application.Current.MainWindow;

            if (TestIssuesDataWindow.ShowDialog() == true)
            {
                SaveChanges();
            }
            else
            {
                // Discard changes
                Context.DiscardChanges<IssuedTest>(context);
                NotifyPropertyChanged(nameof(IssuedTests));
            }
        }
        private void DeleteIssuedTest()
        {
            var result = MessageBox.Show("Delete issued test?\r\nAll related data will be deleted!\r\rData recovery will be impossible", "Attention!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                context.Remove(CurrentIssuedTest.Model);

                SaveChanges();
            }
        }
        private void CheckIssuedTestData()
        {
            // Group must not be empty
            if (CurrentIssuedTest.UserGroup == null)
            {
                ErrorMessage = "Error: select students group";
                return;
            }
            // Expired date muts be greater or equal today date
            else if (CurrentIssuedTest.ExpiredDate < DateTime.Now)
            {
                ErrorMessage = "Error: Expired date muts be greater or equal today date";
                return;
            }
            // Attempts amount > 0
            else if (CurrentIssuedTest.AttemptsAmount <= 0)
            {
                ErrorMessage = "Error: Attempts amount must be greater than 0";
                return;
            }

            TestIssuesDataWindow.DialogResult = true;
            TestIssuesDataWindow.Close();
        }
        #endregion
    }
}
