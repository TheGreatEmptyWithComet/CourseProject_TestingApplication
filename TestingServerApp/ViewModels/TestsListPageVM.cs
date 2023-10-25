using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using TestingServerApp.Viewes;
using static TestingServerApp.TestsListPageVM;

namespace TestingServerApp
{
    public class TestsListPageVM : NotifyPropertyChangeHandler
    {
        #region Properties
        /****************************************************************************************/
        public delegate void CurrentPageChanged(string page);
        public event CurrentPageChanged OnCurrentPageChanged;
        #endregion

        #region Properties
        /****************************************************************************************/
        private readonly Context context;
        // variable is used to prevent some data checking while they are edited
        private bool editDataMode = false;
        private TestDataWindow TestDataWindow;
        // View model for window binding
        public TestVM CurrentTest { get; private set; }

        // DB row data
        private List<Test> allTests;
        // Data for WPF
        public ObservableCollection<TestVM> Tests { get { return new ObservableCollection<TestVM>(allTests.Select(i => new TestVM(i))); } }

        private TestVM selectedTest;
        public TestVM SelectedTest
        {
            get
            {
                return selectedTest;
            }
            set
            {
                if (selectedTest != value)
                {
                    selectedTest = value;
                    NotifyPropertyChanged(nameof(SelectedTest));
                }
            }
        }

        // Error message for data window

        private string errorMessage;
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                errorMessage = value;
                NotifyPropertyChanged(nameof(ErrorMessage));
            }
        }

        #endregion


        #region Commands
        /****************************************************************************************/
        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SaveTestEndExitCommand { get; private set; }
        #endregion


        #region Constructor
        /****************************************************************************************/
        public TestsListPageVM(Context context)
        {
            this.context = context;
            LoadDataFromDB();
            InitCommands();
        }
        #endregion


        #region Methods
        /****************************************************************************************/
        private void InitCommands()
        {
            AddCommand = new RelayCommand(AddNewTest);
            SaveTestEndExitCommand = new RelayCommand(SaveTestEndExit);
            EditCommand = new RelayCommand(EditTest);
            DeleteCommand = new RelayCommand(DeleteTest);
        }

        private void SaveTestEndExit()
        {
            if (TestDataIsCorrect())
            {
                context.Add(CurrentTest.Model);
                SaveChanges();
                OnCurrentPageChanged?.Invoke("TestsListPage.xaml");
            }
            else
            {
                return;
            }
        }

        private void AddNewTest()
        {
            // Create new entity
            Test newTest = new Test();
            CurrentTest = new TestVM(newTest);
            ErrorMessage = string.Empty;

            OnCurrentPageChanged?.Invoke("TestDataPage.xaml");
        }
        private void EditTest()
        {
            editDataMode = true;

            // Create edited user
            Test editedTest = new Test()
            {
                TestCategory = SelectedTest.Model.TestCategory,
                Name = SelectedTest.Name,
                QuestionsAmountForTest = SelectedTest.QuestionsAmountForTest,
                MaxTestScores = SelectedTest.MaxTestScores,
                MinutsForTest = SelectedTest.MinutsForTest,
                Questions = SelectedTest.Model.Questions
            };
            CurrentTest = new TestVM(editedTest);
            ErrorMessage = string.Empty;

            // Create and show window
            TestDataWindow = new TestDataWindow();
            TestDataWindow.Owner = Application.Current.MainWindow;

            if (TestDataWindow.ShowDialog() == true)
            {
                // change data
                SelectedTest.Name = editedTest.Name;

                editDataMode = false;

                // update db
                SaveChanges();
            }
        }
        private void DeleteTest()
        {
            // remove record
            context.Remove(SelectedTest.Model);
            allTests.Remove(SelectedTest.Model);

            // update db
            SaveChanges();
        }
        private bool TestDataIsCorrect()
        {
            // check first name
            if (string.IsNullOrEmpty(CurrentTest.Name))
            {
                ErrorMessage = "Name must not be empty";
                return false;
            }
            else if (CurrentTest.TestCategory == null)
            {
                ErrorMessage = "Select a category";
                return false;
            }
            else if (CurrentTest.QuestionsAmountForTest <= 0)
            {
                ErrorMessage = "Input correct questions amount for test";
                return false;
            }
            else if (CurrentTest.MinutsForTest <= 0)
            {
                ErrorMessage = "Input correct minutes amount for test";
                return false;
            }
            else if (CurrentTest.MaxTestScores <= 0)
            {
                ErrorMessage = "Input correct max test scores";
                return false;
            }

            return true;
           
        }

        public void LoadDataFromDB()
        {
            allTests = context.Tests.ToList();
            NotifyPropertyChanged(nameof(Tests));
        }
        private void SaveChanges()
        {
            try
            {
                context.SaveChanges();
                LoadDataFromDB();
            }
            catch (Exception ex)
            {
                string innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                MessageBox.Show(ex.Message + "\n" + innerMessage, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
