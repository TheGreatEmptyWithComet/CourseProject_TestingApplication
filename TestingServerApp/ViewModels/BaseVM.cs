using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestingServerApp
{
    public class BaseVM : NotifyPropertyChangeHandler
    {
        #region Properties
        /****************************************************************************************/
        private readonly Context context;

        private string currentPage;
        public string CurrentPage
        {
            get { return currentPage; }
            set
            {
                currentPage = value;
                NotifyPropertyChanged(nameof(CurrentPage));
            }
        }
        #endregion


        #region Inner view models
        /****************************************************************************************/
        public TestCategoriesVM TestCategoriesVM { get; private set; }
        public TestMaterialsVM TestMaterialsVM { get; private set; }
        public UserMaterialsVM UserMaterialsVM { get; private set; }
        public IssueTestsVM IssueTestsVM { get; private set; }
        #endregion


        #region Commands
        /****************************************************************************************/
        public ICommand PageNavigationCommand { get; private set; }
        public ICommand AtExitCommand { get; private set; }

        #endregion


        #region Constructor
        /****************************************************************************************/
        public BaseVM()
        {
            context = new Context();

            // Set the start page
            CurrentPage = "UsersMenuPage.xaml";

            // Init inner view models
            TestCategoriesVM = new TestCategoriesVM(context);
            TestMaterialsVM = new TestMaterialsVM(context);
            TestMaterialsVM.OnCurrentPageChanged += (page) => CurrentPage = page;
            UserMaterialsVM = new UserMaterialsVM(context);
            IssueTestsVM = new IssueTestsVM(context);

            InitCommands();

            LoadAppConfig();
            CreateDbFilesStorageIfNotExist();

        }
        #endregion


        #region Methods
        /****************************************************************************************/
        private void InitCommands()
        {
            PageNavigationCommand = new RelayCommand<string>(p => CurrentPage = p);
            AtExitCommand = new RelayCommand(AtExit);
        }


        // release context resources
        private void AtExit()
        {
            context.Dispose();
        }

        private void LoadAppConfig()
        {
            var builder = new ConfigurationBuilder();
            // Set path to the current directory
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // Get config from file Appconfig.json
            builder.AddJsonFile("Appconfig.json");
            // Create config
            var config = builder.Build();

            AppConfig.DbExternalFiles.TestImagesPath = GetFullPath(config.GetSection("DbExternalFiles:TestImagesPath").Value);
            AppConfig.DbExternalFiles.QuestionImagesPath = GetFullPath(config.GetSection("DbExternalFiles:QuestionImagesPath").Value);
            
            int.TryParse(config.GetSection("DbExternalFiles:ImageMaxSize").Value, out int size);
            AppConfig.DbExternalFiles.ImageMaxSize = size;

            AppConfig.LogFileName = config.GetSection("LogFileName").Value;
        }
        private void CreateDbFilesStorageIfNotExist()
        {
            Directory.CreateDirectory(AppConfig.DbExternalFiles.TestImagesPath);
            Directory.CreateDirectory(AppConfig.DbExternalFiles.QuestionImagesPath);
        }
        private string GetFullPath(string path)
        {
            string fullPath = path;

            if (!Path.IsPathFullyQualified(path))
            {
                fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            }

            return fullPath;
        }


        #endregion

    }
}
