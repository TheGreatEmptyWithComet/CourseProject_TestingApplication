using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using TestingServerApp.Viewes;

namespace TestingServerApp
{
    public class TestCategoriesVM : NotifyPropertyChangeHandler
    {
        #region Properties
        /****************************************************************************************/
        private readonly Context context;
        // variable is used to prevent some data checking while they are edited
        private bool editDataMode = false;
        private TestCategoryDataWindow testCategoryDataWindow;
        // View model for window binding
        public TestCategoryVM CurrentTestCategory { get; private set; }

        // DB row data
        private List<TestCategory> allTestCategories;
        // Data for WPF
        public ObservableCollection<TestCategoryVM> TestCategories { get { return new ObservableCollection<TestCategoryVM>(allTestCategories.Select(i => new TestCategoryVM(i))); } }

        private TestCategoryVM selectedTestCategory;
        public TestCategoryVM SelectedTestCategory
        {
            get
            {
                return selectedTestCategory;
            }
            set
            {
                if (selectedTestCategory != value)
                {
                    selectedTestCategory = value;
                    NotifyPropertyChanged(nameof(SelectedTestCategory));
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
        public ICommand SaveCommand { get; private set; }
        #endregion


        #region Constructor
        /****************************************************************************************/
        public TestCategoriesVM(Context context)
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
            AddCommand = new RelayCommand(AddNewTestCategory);
            SaveCommand = new RelayCommand(CheckData);
            EditCommand = new RelayCommand(EditTestCategory);
            DeleteCommand = new RelayCommand(DeleteTestCategory);
        }
        private void AddNewTestCategory()
        {
            // Create new entity
            TestCategory newTestCategory = new TestCategory();
            CurrentTestCategory = new TestCategoryVM(newTestCategory);
            ErrorMessage = string.Empty;

            // Create and show window
            testCategoryDataWindow = new TestCategoryDataWindow();
            testCategoryDataWindow.Owner = Application.Current.MainWindow;

            if (testCategoryDataWindow.ShowDialog() == true)
            {
                context.Add(newTestCategory);
                //allTestCategories.Add(newTestCategory);
                SaveChanges();
            }
        }
        private void EditTestCategory()
        {
            editDataMode = true;

            // Create edited user
            TestCategory editedTestCategory = new TestCategory()
            {
                Name = SelectedTestCategory.Name
            };
            CurrentTestCategory = new TestCategoryVM(editedTestCategory);
            ErrorMessage = string.Empty;

            // Create and show window
            testCategoryDataWindow = new TestCategoryDataWindow();
            testCategoryDataWindow.Owner = Application.Current.MainWindow;

            if (testCategoryDataWindow.ShowDialog() == true)
            {
                // change data
                SelectedTestCategory.Name = editedTestCategory.Name;

                editDataMode = false;

                // update db
                SaveChanges();
            }
        }
        private void DeleteTestCategory()
        {
            // remove record
            context.Remove(SelectedTestCategory.Model);
            allTestCategories.Remove(SelectedTestCategory.Model);

            // update db
            SaveChanges();
        }
        private void CheckData()
        {
            // check first name
            if (string.IsNullOrEmpty(CurrentTestCategory.Name))
            {
                ErrorMessage = "Name must not be empty";
                return;
            }
            testCategoryDataWindow.DialogResult = true;
            testCategoryDataWindow.Close();
        }

        public void LoadDataFromDB()
        {
            allTestCategories = context.TestCategories.ToList();
            NotifyPropertyChanged(nameof(TestCategories));
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
