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
using Microsoft.EntityFrameworkCore;

namespace TestingServerApp
{
    public class TestCategoriesVM : NotifyPropertyChangeHandler
    {
        #region Properties
        /****************************************************************************************/
        private readonly Context context;
        private TestCategoryDataWindow testCategoryDataWindow;

        public ObservableCollection<TestCategoryVM> TestCategories
        {
            get => new ObservableCollection<TestCategoryVM>(context.TestCategories.Select(i => new TestCategoryVM(i)));
        }

        // Entity for data binding
        public TestCategoryVM CurrentTestCategory { get; set; }

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

            InitCommands();

            // Load data from DB
            context.TestCategories.Load();
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

            // Create and show data window
            testCategoryDataWindow = new TestCategoryDataWindow();
            testCategoryDataWindow.Owner = Application.Current.MainWindow;

            if (testCategoryDataWindow.ShowDialog() == true)
            {
                context.Add(newTestCategory);
                SaveChanges();
            }
        }

        private void EditTestCategory()
        {
            ErrorMessage = string.Empty;

            // Create and show window
            testCategoryDataWindow = new TestCategoryDataWindow();
            testCategoryDataWindow.Owner = Application.Current.MainWindow;

            if (testCategoryDataWindow.ShowDialog() == true)
            {
                SaveChanges();
            }
            else
            {
                // Discard changes
                Context.DiscardChanges<TestCategory>(context);
            }
        }

        private void DeleteTestCategory()
        {
            var result = MessageBox.Show("Delete test ctaegory?\r\nAll related tests will be deleted!\r\rData recovery will be impossible", "Attention!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {

                context.Remove(CurrentTestCategory.Model);

                SaveChanges();
            }
        }

        private void CheckData()
        {
            // Name must not be empty
            if (string.IsNullOrEmpty(CurrentTestCategory.Name))
            {
                ErrorMessage = "Error: name must not be empty";
                return;
            }
            // Name must be unique
            else if (context.TestCategories.Any((category) => category.Name == CurrentTestCategory.Name && category.Id != CurrentTestCategory.Id))
            {
                ErrorMessage = "Error: name must be unique";
                return;
            }

            testCategoryDataWindow.DialogResult = true;
            testCategoryDataWindow.Close();
        }

        private void SaveChanges()
        {
            try
            {
                context.SaveChanges();
                NotifyPropertyChanged(nameof(TestCategories));
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
