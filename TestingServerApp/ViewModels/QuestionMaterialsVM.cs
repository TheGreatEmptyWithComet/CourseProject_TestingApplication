using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TestingServerApp.Viewes;
using static TestingServerApp.TestMaterialsVM;

namespace TestingServerApp
{
    public class QuestionMaterialsVM : NotifyPropertyChangeHandler
    {
        #region Delegates & Events
        /****************************************************************************************/
        public event CurrentPageChanged OnCurrentPageChanged;
        #endregion

        #region Properties
        /****************************************************************************************/
        private readonly Context context;
        private bool editDataMode = false;

        // DB row data
        private List<Question> allQuestions;
        // Data for WPF
        public ObservableCollection<QuestionVM> Questions { get { return new ObservableCollection<QuestionVM>(allQuestions.Select(i => new QuestionVM(i))); } }

        private int currentQuestionPosition;
        public int CurrentQuestionPosition
        {
            get
            {
                return currentQuestionPosition;
            }
            set
            {
                if (currentQuestionPosition != value)
                {
                    currentQuestionPosition = value;
                    NotifyPropertyChanged(nameof(CurrentQuestionPosition));
                }
            }
        }

        private Test currentTest;
        public Test CurrentTest
        {
            get => currentTest;
            set
            {
                currentTest = value;
                LoadDataFromDB();
            }
        }

        private QuestionVM selectedQuestion;
        public QuestionVM SelectedQuestion
        {
            get
            {
                return selectedQuestion;
            }
            set
            {
                if (selectedQuestion != value)
                {
                    selectedQuestion = value;
                    NotifyPropertyChanged(nameof(SelectedQuestion));
                }
            }
        }

        private QuestionVM currentQuestion;
        public QuestionVM CurrentQuestion
        {
            get
            {
                return currentQuestion;
            }
            set
            {
                if (currentQuestion != value)
                {
                    currentQuestion = value;
                    NotifyPropertyChanged(nameof(CurrentQuestion));
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
        public ICommand PreviousQuestionCommand { get; private set; }
        public ICommand NewOrNextQuestionCommand { get; private set; }
        public ICommand ExitWithoutSavingCommang { get; private set; }

        #endregion


        #region Constructor
        /****************************************************************************************/
        public QuestionMaterialsVM(Context context)
        {
            this.context = context;

            InitCommands();
        }

        #endregion


        #region Methods
        /****************************************************************************************/
        private void InitCommands()
        {
            AddCommand = new RelayCommand(AddNewQuestion);
            EditCommand = new RelayCommand(EditQuestion);
            DeleteCommand = new RelayCommand(DeleteTest);
            ExitWithoutSavingCommang = new RelayCommand(ExitWithoutSaving);
            PreviousQuestionCommand = new RelayCommand(PreviousQuestion);
            NewOrNextQuestionCommand = new RelayCommand(NewOrNextQuestion);
        }

        private void ExitWithoutSaving()
        {
            if (!editDataMode)
            {
                // remove the new item from collection
                allQuestions.Remove(allQuestions.Last());
            }
            OpenPage("QuestionsDirectoryPage.xaml");
        }

        private void AddNewQuestion()
        {
            editDataMode = false;

            // Create new entity
            allQuestions.Add(new Question());
            CurrentQuestion = Questions.Last(); ;
            ErrorMessage = string.Empty;

            // Set navigation index
            currentQuestionPosition = allQuestions.Count - 1;

            OpenPage("QuestionDataPage.xaml");
        }
        private void EditQuestion()
        {
            editDataMode = true;

            // Create edited user
            Question editedQuestion = new Question()
            {
                Test = SelectedQuestion.Model.Test,
                Answers = SelectedQuestion.Model.Answers,
                MultipleAnswersAllowed = SelectedQuestion.MultipleAnswersAllowed,
                PartialAnswersAllowed = SelectedQuestion.PartialAnswersAllowed,
                QuestionWeight = SelectedQuestion.QuestionWeight,
                Image = SelectedQuestion.Model.Image,
                Text = SelectedQuestion.Text
            };
            CurrentQuestion = new QuestionVM(editedQuestion);
            ErrorMessage = string.Empty;

            // Set navigation index
            currentQuestionPosition = allQuestions.IndexOf(SelectedQuestion.Model);

            // Open test data page
            OpenPage("QuestionDataPage.xaml");
        }
        private void DeleteTest()
        {
            // remove record
            context.Remove(SelectedQuestion.Model);

            // update db
            SaveChanges();
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


        private void LoadDataFromDB()
        {
            allQuestions = context.Questions.Where((q) => q.Test == CurrentTest).Include((q) => q.Answers).ToList();
            NotifyPropertyChanged(nameof(Tests));
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

        private void OpenPage(string pageName)
        {
            OnCurrentPageChanged?.Invoke(pageName);
        }
        private void PreviousQuestion()
        {
            if (CurrentQuestionPosition > 0)
            {
                --CurrentQuestionPosition;
                CurrentQuestion = Questions[CurrentQuestionPosition];
            }
        }
        private void NewOrNextQuestion()
        {
            if (CurrentQuestionPosition < allQuestions.Count - 1)
            {
                ++CurrentQuestionPosition;
                CurrentQuestion = Questions[CurrentQuestionPosition];
            }
            else if (CurrentQuestionPosition == allQuestions.Count - 1)
            {
                AddNewQuestion();
            }
        }

        #endregion

    }
}
