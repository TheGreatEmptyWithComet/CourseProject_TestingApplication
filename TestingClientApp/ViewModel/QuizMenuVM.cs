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
    public enum StatisticType { User, Category, General }
    internal class QuizMenuVM : NotifyPropertyChangedHandler
    {
        #region Events
        /****************************************************************************************/
        public delegate void CurrentPageChanged(string page);
        public event CurrentPageChanged OnCurrentPageChanged;
        #endregion


        #region Properties
        /****************************************************************************************/
        public Quiz Quiz { get; private set; }
        private NetworkClient networkClient;

        private string currentPage = string.Empty;
        public string CurrentPage
        {
            get { return currentPage; }
            set
            {
                // the comparisson of old and new value doesn't permorm itntentionally
                currentPage = value;
                NotifyPropertyChanged(nameof(CurrentPage));
            }
        }

        private bool currentPageIsStatistic = false;
        public bool CurrentPageIsStatistic
        {
            get { return currentPageIsStatistic; }
            set
            {

                if (currentPageIsStatistic != value)
                {
                    currentPageIsStatistic = value;
                    NotifyPropertyChanged(nameof(CurrentPageIsStatistic));
                }
            }
        }

        private StatisticType quizStatisticType;
        public StatisticType QuizStatisticType
        {
            get { return quizStatisticType; }
            set
            {
                if (quizStatisticType != value)
                {
                    quizStatisticType = value;
                    NotifyPropertyChanged(nameof(QuizStatisticType));
                }
            }
        }
        #endregion


        #region Commands
        /****************************************************************************************/
        public ICommand TestSelectionCommand { get; private set; }
        public ICommand PageNavigationCommand { get; private set; }
        public ICommand StatisticPageNavigationCommand { get; private set; }
        public ICommand ExitQuizCommand { get; private set; }
        #endregion


        #region Constructor
        /****************************************************************************************/
        public QuizMenuVM(Quiz quiz, NetworkClient networkClient)
        {
            this.Quiz = quiz;
            this.networkClient = networkClient;
            CurrentPage = "QuizMenuCategorySelection.xaml";
            InitCommands();
        }
        #endregion


        #region Methods
        /****************************************************************************************/
        private void InitCommands()
        {
            TestSelectionCommand = new RelayCommand<object>(AtTestWasSelected);
            PageNavigationCommand = new RelayCommand<string>(p => { CurrentPage = p; CurrentPageIsStatistic = false; });
            StatisticPageNavigationCommand = new RelayCommand<StatisticType>(NavigateStatistic);
            ExitQuizCommand = new RelayCommand(ExitQuiz);
        }

        private async void AtTestWasSelected(object testId)
        {
            //Quiz.QuestionCategoryName = testId.ToString();        -----------------------------------------------------
            Quiz.SetCurretnTest((int)testId);

            if (!currentPageIsStatistic)
            {
                Quiz.CurrentTest.AttemptsLeft = await networkClient.GetTestAttemptsLeftAmount((int)testId);
                OnCurrentPageChanged?.Invoke("QuizPreStart.xaml");//++++++++++++++++++
            }
            else
            {
                //Quiz.SetCategoryStatistic();
                CurrentPage = "QuizMenuCategoryStatistic.xaml";
            }
        }

        private void NavigateStatistic(StatisticType type)
        {
            CurrentPageIsStatistic = true;
            QuizStatisticType = type;

            switch (type)
            {
                case StatisticType.User:
                    //Quiz.SetUserStatistic();
                    CurrentPage = "QuizMenuUserStatistic.xaml";
                    break;

                case StatisticType.Category:
                    CurrentPage = "QuizMenuCategorySelection.xaml";
                    break;

                case StatisticType.General:
                    //Quiz.SetGeneralStatistic();
                    CurrentPage = "QuizMenuGeneralStatistic.xaml";
                    break;
            }
        }

        private void ExitQuiz()
        {
            MessageShowDialogWindow messageShowDialogWindow = new MessageShowDialogWindow("Exit the quiz?");
            messageShowDialogWindow.Owner = Application.Current.MainWindow;
            bool dialogResult = messageShowDialogWindow.ShowDialog() ?? false;

            if (dialogResult)
            {
                Application.Current.MainWindow.Close();
            }
        }
        #endregion
    }
}
