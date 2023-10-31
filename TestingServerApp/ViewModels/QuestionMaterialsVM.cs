using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
        private bool newQuestionIsAdded = false;
        private Question editedQuestion;

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
        // property is needed because Questions.Count property isn't correctly updated for binding when neq item is added
        private int questionsCount;
        public int QuestionsCount
        {
            get
            {
                return questionsCount;
            }
            set
            {
                if (currentQuestionPosition != value)
                {
                    questionsCount = value;
                    NotifyPropertyChanged(nameof(QuestionsCount));
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
                // Load test's questions when test is set
                LoadDataFromDB();
            }
        }

        // Item that is selected in view's item control
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

        public ICommand LoadImageCommand { get; private set; }
        public ICommand DeleteImageCommand { get; private set; }

        public ICommand AddAnswerCommand { get; private set; }
        public ICommand DeleteAnswerCommand { get; private set; }

        public ICommand PreviousQuestionCommand { get; private set; }
        public ICommand NewOrNextQuestionCommand { get; private set; }

        public ICommand ExitWithoutSavingCommang { get; private set; }
        public ICommand SaveQuestionEndExitCommand { get; private set; }
        public ICommand SaveQuestionWithoutExitingCommand { get; private set; }

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
        // GENERAL 
        /****************************************************************************************/
        private void InitCommands()
        {
            AddCommand = new RelayCommand(AddNewQuestion);
            EditCommand = new RelayCommand(EditQuestion);
            DeleteCommand = new RelayCommand(DeleteQuestion);

            LoadImageCommand = new RelayCommand(LoadImage);
            DeleteImageCommand = new RelayCommand(DeleteImage);

            AddAnswerCommand = new RelayCommand(AddAnswer);
            DeleteAnswerCommand = new RelayCommand<object>(DeleteAnswer);

            PreviousQuestionCommand = new RelayCommand(PreviousQuestion);
            NewOrNextQuestionCommand = new RelayCommand(NewOrNextQuestion);

            ExitWithoutSavingCommang = new RelayCommand(ExitWithoutSaving);
            SaveQuestionEndExitCommand = new RelayCommand(SaveQuestionEndExit);
            SaveQuestionWithoutExitingCommand = new RelayCommand(SaveQuestionWithoutExiting);
        }
        private void SaveChangesAndLoadFromContext()
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
        private void LoadDataFromDB()
        {
            allQuestions = context.Questions.Where((q) => q.Test == CurrentTest).Include((q) => q.Answers).ToList();
            NotifyPropertyChanged(nameof(Tests));
        }
        private void SaveQuestionEndExit()
        {
            SaveQuestionWithoutExiting();
            OpenPage("QuestionsDirectoryPage.xaml");
        }
        private void SaveQuestionWithoutExiting()
        {
            if (QuestionDataIsCorrect())
            {
                SaveChangesAndLoadFromContext();
            }
        }
        private void ExitWithoutSaving()
        {
            if (newQuestionIsAdded)
            {
                // remove the new item from collection
                allQuestions.Remove(allQuestions.Last());
            }
            OpenPage("QuestionsDirectoryPage.xaml");
        }
        private void OpenPage(string pageName)
        {
            OnCurrentPageChanged?.Invoke(pageName);
        }

        // QUESTION
        /****************************************************************************************/
        private void AddNewQuestion()
        {
            newQuestionIsAdded = true;

            // Create new entity
            Question newQuestion = new Question() { Test = CurrentTest, Answers=new ObservableCollection<Answer>()};

            context.Add(newQuestion);

            //Update questions list();
            SaveChangesAndLoadFromContext();
            CurrentQuestion = Questions.Last(); ;
            ErrorMessage = string.Empty;

            // Set navigation index
            CurrentQuestionPosition = allQuestions.Count - 1;

            // Update count property
            QuestionsCount = allQuestions.Count;

            NotifyPropertyChanged(nameof(Questions));

            OpenPage("QuestionDataPage.xaml");
        }
        private void EditQuestion()
        {
            newQuestionIsAdded = false;

            currentQuestionPosition = allQuestions.IndexOf(SelectedQuestion.Model);
            CurrentQuestion = Questions[currentQuestionPosition];
            ErrorMessage = string.Empty;

            // Update count property
            QuestionsCount = allQuestions.Count;

            // Open test data page
            OpenPage("QuestionDataPage.xaml");
        }
        private void DeleteQuestion()
        {
            // remove record
            context.Remove(SelectedQuestion.Model);

            // update db
            SaveChangesAndLoadFromContext();
            // notify property changed
            NotifyPropertyChanged(nameof(Questions));
        }
        private bool QuestionDataIsCorrect()
        {
            // Question text
            if (string.IsNullOrEmpty(CurrentQuestion.Text))
            {
                ErrorMessage = "Question text must not be empty";
                return false;
            } // Question weight
            else if (CurrentQuestion.QuestionWeight <= 0)
            {
                ErrorMessage = "Question weight must be greater than 0";
                return false;
            } // Min 2 answers available
            else if (CurrentQuestion.Answers.Count < 2)
            {
                ErrorMessage = "There are must be at least two answers";
                return false;
            }
            else if (!CurrentQuestion.MultipleAnswersAllowed && CurrentQuestion.Answers.Where((a) => a.IsCorrect == true).Count() != 1)
            {
                ErrorMessage = "There must be one correct answer";
                return false;
            }
            else if (CurrentQuestion.MultipleAnswersAllowed && CurrentQuestion.Answers.Where((a) => a.IsCorrect == true).Count() < 2)
            {
                ErrorMessage = "At least two answers must be checked as correct";
                return false;
            } // all answers hat text
            else if (CurrentQuestion.Answers.Any((a) => string.IsNullOrEmpty(a.Text)))
            {
                ErrorMessage = "Answer text must not be empty";
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }
        
        // QUESTION IMAGE
        private void LoadImage()
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                CurrentQuestion.Image = File.ReadAllBytes(op.FileName);
            }
        }
        private void DeleteImage()
        {
            CurrentQuestion.Image = null;
        }

        // NAVIGATION
        private void NewOrNextQuestion()
        {
            // don't go further when data isn't correct
            if (!QuestionDataIsCorrect())
            {
                return;
            }

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
        private void PreviousQuestion()
        {
            if (QuestionDataIsCorrect() && CurrentQuestionPosition > 0)
            {
                --CurrentQuestionPosition;
                CurrentQuestion = Questions[CurrentQuestionPosition];
            }
        }

        // ANSWER
        /****************************************************************************************/
        private void AddAnswer()
        {
            Answer newAnswer = new Answer() { Question = CurrentQuestion.Model };
            if (newQuestionIsAdded && allQuestions.Last().Answers == null)
            {
                allQuestions.Last().Answers = new ObservableCollection<Answer>();
            }
            CurrentQuestion.Model.Answers.Add(newAnswer);
            CurrentQuestion.NotifyAnswersChanged();
        }
        private void DeleteAnswer(object item)
        {
            if (item is AnswerVM answer)
            {
                CurrentQuestion.Model.Answers.Remove(answer.Model);
                CurrentQuestion.NotifyAnswersChanged();
            }
        }

        #endregion

    }
}
