using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TestingClientApp
{
    internal class SignupLoginVM : NotifyPropertyChangedHandler
    {
        #region Events
        /****************************************************************************************/
        public delegate void CurrentPageChanged(string page);
        public event CurrentPageChanged OnCurrentPageChanged;
        #endregion

        
        #region Properties
        /****************************************************************************************/
        private NetworkClient networkClient;
        public Quiz Quiz { get; private set; }
        public User CurrentUser { get; set; } = new User();

        private bool passwordIsVisible = false;
        public bool PasswordIsVisible
        {
            get { return passwordIsVisible; }
            set
            {
                if (passwordIsVisible != value)
                {
                    passwordIsVisible = value;
                    NotifyPropertyChanged(nameof(PasswordIsVisible));
                }
            }
        }
        #endregion


        #region Commands
        /****************************************************************************************/
        public ICommand LoginPageNavigationCommand { get; set; }
        public ICommand PasswordVisibilityChangeCommand { get; set; }
        public ICommand ResetPasswordVisibilityCommand { get; set; }
        public ICommand SignUpCommand { get; set; }
        public ICommand LogInCommand { get; set; }
        #endregion


        #region Constructor
        /****************************************************************************************/
        public SignupLoginVM(Quiz quiz, NetworkClient networkClient)
        {
            this.Quiz = quiz;
            this.networkClient = networkClient;
            InitCommands();
        }
        #endregion


        #region Methods
        /****************************************************************************************/
        private void InitCommands()
        {
            LoginPageNavigationCommand = new RelayCommand<string>(p => { OnCurrentPageChanged?.Invoke(p); PasswordIsVisible = false; });
            PasswordVisibilityChangeCommand = new RelayCommand(ChangePasswordVisibility);
            ResetPasswordVisibilityCommand = new RelayCommand(() => PasswordIsVisible = false);
            //SignUpCommand = new RelayCommand(SignUp);
            LogInCommand = new RelayCommand(LogIn);
        }

        private void ChangePasswordVisibility()
        {
            if (PasswordIsVisible)
            {
                PasswordIsVisible = false;
            }
            else
            {
                PasswordIsVisible = true;
            }
        }

        private async void LogIn()
        {
            if (await networkClient.Login(CurrentUser.Login, CurrentUser.Password))
            {
                StartNewQuiz();
            }
            else
            {
                ShowMessageWindow("Wrong login or password");
            }
        }
        private void ShowMessageWindow(string message)
        {
            MessageShowWindow messageShowWindow = new MessageShowWindow(message);
            try
            {
                messageShowWindow.Owner = Application.Current.MainWindow;
            }
            catch { }
            messageShowWindow.ShowDialog();
        }
        private async void StartNewQuiz()
        {
            Quiz.UserLogin = CurrentUser.Login;
            Quiz.TestsList = await networkClient.GetAllowedTestsInfo();
            OnCurrentPageChanged?.Invoke("QuizMenu.xaml");
        }
        #endregion
    }
}
