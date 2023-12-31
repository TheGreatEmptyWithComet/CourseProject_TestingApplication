﻿using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static CommunityToolkit.Mvvm.ComponentModel.__Internals.__TaskExtensions.TaskAwaitableWithoutEndValidation;

namespace TestingServerApp
{
    public class BaseVM : NotifyPropertyChangedHandler
    {
        #region Properties
        /****************************************************************************************/
        private readonly Context context;
        private Server server;

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
        private bool menuIsVisibile = false;
        public bool MenuIsVisibile
        {
            get { return menuIsVisibile; }
            set
            {
                menuIsVisibile = value;
                NotifyPropertyChanged(nameof(MenuIsVisibile));
            }
        }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        #endregion


        #region Inner view models
        /****************************************************************************************/
        public TestCategoriesVM TestCategoriesVM { get; private set; }
        public TestMaterialsVM TestMaterialsVM { get; private set; }
        public UserMaterialsVM UserMaterialsVM { get; private set; }
        public IssueTestsVM IssueTestsVM { get; private set; }
        public TestResultsVM TestResultsVM { get; private set; }
        #endregion


        #region Commands
        /****************************************************************************************/
        public ICommand PageNavigationCommand { get; private set; }
        public ICommand LoginCommand { get; private set; }
        public ICommand AtExitCommand { get; private set; }

        #endregion


        #region Constructor
        /****************************************************************************************/
        public BaseVM()
        {
            context = new Context();

            // Set the start page
            CurrentPage = "StartPage.xaml";

            // Init inner view models
            TestCategoriesVM = new TestCategoriesVM(context);
            TestMaterialsVM = new TestMaterialsVM(context);
            TestMaterialsVM.OnCurrentPageChanged += (page) => CurrentPage = page;
            UserMaterialsVM = new UserMaterialsVM(context);
            IssueTestsVM = new IssueTestsVM(context);
            TestResultsVM = new TestResultsVM(context);

            InitCommands();

            LoadAppConfig();
            CreateDbFilesStorageIfNotExist();

            // start server
            server = new Server(new IPEndPoint(IPAddress.Parse(AppConfig.ServerIpAdress), AppConfig.ServerPort));
            server.ListenAsync();

        }
        #endregion


        #region Methods
        /****************************************************************************************/
        private void InitCommands()
        {
            PageNavigationCommand = new RelayCommand<string>(p => CurrentPage = p);
            AtExitCommand = new RelayCommand(AtExit);
            LoginCommand = new RelayCommand(LoginMethod);
        }

        private void LoginMethod()
        {
            using (Context context = new Context())
            {
                var currentUser = context.Users.Where((u) => u.Login == Login && u.UserGroup.Name == "Administrator").FirstOrDefault();
                if (currentUser != null)
                {
                    string checkedPasswordHash = PasswordEncryption.GetPasswordHash(Password, currentUser.PasswordSalt);

                    if (checkedPasswordHash == currentUser.PasswordHash)
                    {
                        MenuIsVisibile = true;
                        CurrentPage = "UsersMenuPage.xaml";
                    }
                }
            }
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

            // Database storage path for files
            AppConfig.DbExternalFiles.TestImagesPath = GetFullPath(config.GetSection("DbExternalFiles:TestImagesPath").Value);
            AppConfig.DbExternalFiles.QuestionImagesPath = GetFullPath(config.GetSection("DbExternalFiles:QuestionImagesPath").Value);
            int.TryParse(config.GetSection("DbExternalFiles:ImageMaxSize").Value, out int size);
            AppConfig.DbExternalFiles.ImageMaxSize = size;

            // Log file path
            AppConfig.LogFileName = config.GetSection("LogFileName").Value;

            // Server settings
            AppConfig.ServerIpAdress = config.GetSection("ServerIpAdress").Value;
            int.TryParse(config.GetSection("ServerPort").Value, out int port);
            AppConfig.ServerPort = port;
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
