using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace TestingClientApp
{
    internal class QuizPerformVM : NotifyPropertyChangedHandler
    {
        #region Events
        /****************************************************************************************/
        public delegate void CurrentPageChanged(string page);
        public event CurrentPageChanged OnCurrentPageChanged;
        #endregion


        #region Properties
        /****************************************************************************************/
        private DispatcherTimer Timer = new DispatcherTimer();
        private NetworkClient networkClient;

        public Quiz Quiz { get; private set; }
        public int TimeProgressMaxValueInSecunds { get; private set; }

        private int timeProgressInSecunds;
        public int TimeProgressInSecunds
        {
            get { return timeProgressInSecunds; }
            set
            {
                if (timeProgressInSecunds != value)
                {
                    timeProgressInSecunds = value;
                    NotifyPropertyChanged(nameof(TimeProgressInSecunds));
                }
            }
        }

        private TimeSpan timeLeft;
        public TimeSpan TimeLeft
        {
            get { return timeLeft; }
            set
            {
                if (timeLeft != value)
                {
                    timeLeft = value;
                    NotifyPropertyChanged(nameof(TimeLeft));
                }
            }
        }
        #endregion


        #region Commands
        /****************************************************************************************/
        public ICommand StartTestCommand { get; set; }
        public ICommand NextQuestionCommand { get; set; }
        public ICommand PreviousQuestionCommand { get; set; }
        public ICommand FinishQuizCommand { get; set; }
        #endregion


        #region Constructor
        /****************************************************************************************/
        public QuizPerformVM(Quiz quiz, NetworkClient networkClient)
        {
            this.Quiz = quiz;
            this.networkClient = networkClient;
            InitTimer();
            InitCommands();
        }
        #endregion


        #region Methods
        /****************************************************************************************/
        private void InitCommands()
        {
            StartTestCommand = new RelayCommand(StartTest);
            NextQuestionCommand = new RelayCommand(NextQuizQuestion);
            PreviousQuestionCommand = new RelayCommand(PreviousQuizQuestion);
            FinishQuizCommand = new RelayCommand(() => FinishQuiz());
        }
        private void InitTimer()
        {
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += (s, e) => AtTimerTick();
        }

        private async void StartTest()
        {
            if (Quiz.CurrentTest.AttemptsLeft > 0)
            {
                TimeProgressMaxValueInSecunds = Quiz.CurrentTest.MinutsForTest * 60;
                TimeProgressInSecunds = 0;

                await Quiz.PrepareQuizQuestion();

                Quiz.QuestionForQuiz.MoveCurrentToFirst();
                TimeLeft = TimeSpan.FromMinutes(Quiz.CurrentTest.MinutsForTest);
                Timer.Start();
                OnCurrentPageChanged?.Invoke("QuizPerform.xaml");
            }
        }
        private void NextQuizQuestion()
        {
            Quiz.QuestionForQuiz.MoveCurrentToNext();
            if (Quiz.QuestionForQuiz.IsCurrentAfterLast)
            {
                Quiz.QuestionForQuiz.MoveCurrentToLast();
            }
        }
        private void PreviousQuizQuestion()
        {
            Quiz.QuestionForQuiz.MoveCurrentToPrevious();
            if (Quiz.QuestionForQuiz.IsCurrentBeforeFirst)
            {
                Quiz.QuestionForQuiz.MoveCurrentToFirst();
            }
        }
        private void FinishQuiz(bool dialogResult = false)//+++++++++++++++++++++++++++++++++++++++++++++++++++++
        {
            if (dialogResult != true)
            {
                MessageShowDialogWindow messageShowDialogWindow = new MessageShowDialogWindow("Finish the quiz?");
                messageShowDialogWindow.Owner = Application.Current.MainWindow;
                dialogResult = messageShowDialogWindow.ShowDialog() ?? false;
            }

            if (dialogResult)
            {
                Quiz.FinishQuiz();
                TimeProgressInSecunds = 0;
                Quiz.QuestionForQuiz.MoveCurrentToFirst();
                OnCurrentPageChanged?.Invoke("QuizRezults.xaml");
            }
        }
        private void AtTimerTick()
        {
            TimeLeft -= TimeSpan.FromSeconds(1);
            ++TimeProgressInSecunds;

            // Automatic finish the quiz when time is over
            if (timeLeft.TotalSeconds <= 0)
            {
                Timer.Stop();
                FinishQuiz(true);
            }
        }
        #endregion
    }
}
