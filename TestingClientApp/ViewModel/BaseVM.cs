using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace TestingClientApp
{
    internal class BaseVM : NotifyPropertyChangedHandler
    {
         #region Properties
        /****************************************************************************************/
        public Quiz Quiz { get; set; }
        private NetworkClient networkClient = new NetworkClient();

        private string currentPage;
        public string CurrentPage
        {
            get { return currentPage; }
            set
            {
                // As far as page can be navigate also with hiperlink, that doesn't affect this property, it might be a case when navigation to the page that is already setted to the property should be performed and event PropertyChanged should be raisen. Therefore the checking new and old value for equality before assignment doesn't performed.
                currentPage = value;
                NotifyPropertyChanged(nameof(CurrentPage));
            }
        }
        #endregion


        #region Inner view models
        /****************************************************************************************/
        public QuizPerformVM QuizPerformVM { get; private set; }
        public SignupLoginVM SignupLoginVM { get; private set; }
        public QuizMenuVM QuizMenuVM { get; private set; }
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
            // set the start page
            CurrentPage = "Start.xaml";

            Quiz = new Quiz(networkClient);

            QuizPerformVM = new QuizPerformVM(Quiz, networkClient);
            QuizPerformVM.OnCurrentPageChanged += (p) => { CurrentPage = p; };

            SignupLoginVM = new SignupLoginVM(Quiz, networkClient);
            SignupLoginVM.OnCurrentPageChanged += (p) => { CurrentPage = p; };

            QuizMenuVM = new QuizMenuVM(Quiz, networkClient);
            QuizMenuVM.OnCurrentPageChanged += (p) => { CurrentPage = p; };

            InitCommands();
        }
        #endregion


        #region Methods
        /****************************************************************************************/
        private void InitCommands()
        {
            PageNavigationCommand = new RelayCommand<string>(p => CurrentPage = p);
            AtExitCommand = new RelayCommand(() => networkClient.CloseConnections());
        }
        #endregion
    }
}
