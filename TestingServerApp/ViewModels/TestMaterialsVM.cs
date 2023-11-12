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
using static TestingServerApp.TestMaterialsVM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Win32;
using System.IO;
using TestingServerApp.Utilites;

namespace TestingServerApp
{
    public delegate void CurrentPageChanged(string page);
    public class TestMaterialsVM : NotifyPropertyChangedHandler
    {
        #region Delegates & Events
        /****************************************************************************************/
        public event CurrentPageChanged OnCurrentPageChanged;
        #endregion


        #region Properties
        /****************************************************************************************/
        private readonly Context context;

        public ObservableCollection<TestVM> Tests
        {
            get => new ObservableCollection<TestVM>(context.Tests.Select(i => new TestVM(i)));
        }

        // Entity for data binding
        public TestVM CurrentTest { get; set; }

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


        #region Inner view models
        /****************************************************************************************/
        public QuestionMaterialsVM QuestionMaterialsVM { get; private set; }
        #endregion


        #region Commands
        /****************************************************************************************/
        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SaveTestEndExitCommand { get; private set; }
        public ICommand ExitTestWithoutSavingCommand { get; private set; }
        public ICommand GoToQuestionsCommand { get; private set; }
        public ICommand LoadImageCommand { get; private set; }
        public ICommand DeleteImageCommand { get; private set; }
        #endregion


        #region Constructor
        /****************************************************************************************/
        public TestMaterialsVM(Context context)
        {
            this.context = context;

            // Init inner view models
            QuestionMaterialsVM = new QuestionMaterialsVM(context);
            QuestionMaterialsVM.OnCurrentPageChanged += (page) => OpenPage(page);

            // Load data from DB
            context.Tests.Include((t) => t.Questions).Load();

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
            GoToQuestionsCommand = new RelayCommand(GoToQuestions);
            ExitTestWithoutSavingCommand = new RelayCommand(ExitTestWithoutSaving);
            LoadImageCommand = new RelayCommand(LoadImage);
            DeleteImageCommand = new RelayCommand(DeleteImage);
        }

        private void AddNewTest()
        {
            Test newTest = new Test();
            context.Add(newTest);
            CurrentTest = new TestVM(newTest);
            ErrorMessage = string.Empty;

            OpenPage("TestDataPage.xaml");
        }

        private void EditTest()
        {
            ErrorMessage = string.Empty;

            // Open test data page
            OpenPage("TestDataPage.xaml");
        }

        private void DeleteTest()
        {
            var result = MessageBox.Show("Delete test?\r\nData recovery will be impossible!", "Attention!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                context.Remove(CurrentTest.Model);

                SaveChanges();
            }
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

        private void SaveTestEndExit()
        {
            if (TestDataIsCorrect())
            {
                SaveChanges();

                OpenPage("TestsDirectoryPage.xaml");
            }
        }

        private void ExitTestWithoutSaving()
        {
            // Discard changes
            Context.DiscardChanges<Test>(context);

            OpenPage("TestsDirectoryPage.xaml");
        }

        private void LoadImage()
        {
            ErrorMessage = string.Empty;
            
            string destinFilePath = Path.Combine(AppConfig.DbExternalFiles.TestImagesPath, CurrentTest.Id.ToString());
            
            if (File.Exists(destinFilePath))
            {
                DeleteImage();
            }

            if (ImageHandler.LoadImage(destinFilePath, out string errorMessage) == true)
            {
                CurrentTest.ImagePath = destinFilePath;
            }
            else
            {
                ErrorMessage = errorMessage;
            }
        }
        private void DeleteImage()
        {
            var fileName = CurrentTest.ImagePath;
            CurrentTest.ImagePath = null;

            ImageHandler.DeleteImageAsync(fileName);
        }

        private void SaveChanges()
        {
            try
            {
                context.SaveChanges();
                NotifyPropertyChanged(nameof(Tests));
            }
            catch (Exception ex)
            {
                string innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                MessageBox.Show(ex.Message + "\n" + innerMessage, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoToQuestions()
        {
            if (TestDataIsCorrect())
            {
                QuestionMaterialsVM.CurrentTest = CurrentTest.Model;
                SaveChanges();
                OpenPage("QuestionsDirectoryPage.xaml");
            }
        }

        private void OpenPage(string pageName)
        {
            OnCurrentPageChanged?.Invoke(pageName);
        }
        #endregion
    }
}




