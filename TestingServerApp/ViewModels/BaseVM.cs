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

            InitCommands();

            LoadAppConfig();

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

            AppConfig.DbExternalFilesPath.TestImagesPath = config.GetSection("DbExternalFilesPath:TestImagesPath").Value;
            AppConfig.DbExternalFilesPath.QuestionImagesPath = config.GetSection("DbExternalFilesPath:QuestionImagesPath").Value;
        }

        #endregion

    }
}
