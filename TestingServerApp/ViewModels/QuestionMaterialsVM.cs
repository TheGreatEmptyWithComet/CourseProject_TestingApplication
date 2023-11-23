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
using TestingServerApp.Models;
using TestingServerApp.Utilites;
using TestingServerApp.Viewes;
using static TestingServerApp.TestMaterialsVM;

namespace TestingServerApp
{
    public class QuestionMaterialsVM : NotifyPropertyChangedHandler
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

        private List<Question> allQuestions;

        public ObservableCollection<QuestionVM> Questions
        {
            get => new ObservableCollection<QuestionVM>(allQuestions.Select(i => new QuestionVM(i)));
        }

        // Entity for data binding while create or edit
        private QuestionVM currentQuestion;
        public QuestionVM CurrentQuestion
        {
            get => currentQuestion;
            set
            {
                currentQuestion = value;
                NotifyPropertyChanged(nameof(CurrentQuestion));
            }
        }

        private int currentQuestionPosition;
        public int CurrentQuestionPosition
        {
            get => currentQuestionPosition;
            set
            {
                currentQuestionPosition = value;
                NotifyPropertyChanged(nameof(CurrentQuestionPosition));
            }
        }

        // property is needed because Questions.Count property isn't correctly updated for binding when neq item is added
        private int questionsCount;
        public int QuestionsCount
        {
            get => questionsCount;
            set
            {
                questionsCount = value;
                NotifyPropertyChanged(nameof(QuestionsCount));
            }
        }

        // Test these questions are belong to
        private Test currentTest;
        public Test CurrentTest
        {
            get => currentTest;
            set
            {
                currentTest = value;
                // Load test's questions when test is set
                LoadDataFromDB();
                // Clean error message as it can remain after previous page error
                ErrorMessage = string.Empty;
            }
        }

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


        #region Commands
        /****************************************************************************************/
        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand LoadFromJsonCommand { get; private set; }

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
            LoadFromJsonCommand = new RelayCommand(LoadQuestionsFromJson);

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
        }
        private void SaveQuestionEndExit()
        {
            if (QuestionDataIsCorrect())
            {
                SaveChangesAndLoadFromContext();
                OpenPage("QuestionsDirectoryPage.xaml");
            }
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
            // Discard changes
            Context.DiscardChanges<Question>(context);
            Context.DiscardChanges<Answer>(context);

            //SaveChangesAndLoadFromContext();
            LoadDataFromDB();

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
            // Create new entity
            Question newQuestion = new Question()
            {
                Test = CurrentTest,
                Answers = new ObservableCollection<Answer>()
            };
            allQuestions.Add(newQuestion);
            context.Add(newQuestion);
            CurrentQuestion = new QuestionVM(newQuestion);

            ErrorMessage = string.Empty;

            // Set navigation index
            CurrentQuestionPosition = allQuestions.Count() - 1;

            // Update count property
            QuestionsCount = allQuestions.Count();

            OpenPage("QuestionDataPage.xaml");
        }
        private void EditQuestion()
        {
            // Can be out of range error
            try
            {
                currentQuestionPosition = allQuestions.IndexOf(CurrentQuestion.Model);
                CurrentQuestion = Questions[currentQuestionPosition];

                // Update count property
                QuestionsCount = allQuestions.Count();

                ErrorMessage = string.Empty;

                // Open test data page
                OpenPage("QuestionDataPage.xaml");
            }
            catch (Exception ex) { }
        }
        private void DeleteQuestion()
        {
            // remove record
            context.Remove(CurrentQuestion.Model);
            // update db
            SaveChangesAndLoadFromContext();
            // notify property changed
            NotifyPropertyChanged(nameof(Questions));
        }
        private void LoadQuestionsFromJson()
        {
            ErrorMessage = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a json text file";
            openFileDialog.Filter = "Json text files|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Read Json file
                    string questionsAsJson = File.ReadAllText(openFileDialog.FileName);
                    List<StoredJsonQuestion>? storedJsonQuestions = JsonConvert.DeserializeObject<List<StoredJsonQuestion>>(questionsAsJson);

                    // Check if file format is correct
                    if (storedJsonQuestions == null)
                    {
                        ErrorMessage = "Error: wrong json file";
                        return;
                    }

                    // Iterate through readed data
                    foreach (var jsonQuestion in storedJsonQuestions)
                    {
                        // Create new question
                        Question newQuestion = new Question()
                        {
                            Text = jsonQuestion.Text,
                            MultipleAnswersAllowed = jsonQuestion.CorrectAnswerNumbers.Count > 1 ? true : false,
                            PartialAnswersAllowed = jsonQuestion.PartialAnswersAllowed,
                            QuestionWeight = jsonQuestion.QuestionWeight,
                            Test = CurrentTest,
                            Answers = new ObservableCollection<Answer>()
                        };

                        // Create answers to new question
                        foreach (string jsonAnswer in jsonQuestion.Answers)
                        {
                            Answer newAnswer = new Answer()
                            {
                                Text = jsonAnswer,
                                IsCorrect = jsonQuestion.CorrectAnswerNumbers.Contains((jsonQuestion.Answers.IndexOf(jsonAnswer) + 1)) ? true : false,
                                Question = newQuestion
                            };

                            newQuestion.Answers.Add(newAnswer);
                        }

                        // Check data and save ( CurrentQuestion property is assigned because it is checked by QuestionDataIsCorrect method)
                        CurrentQuestion = new QuestionVM(newQuestion);
                        if (QuestionDataIsCorrect())
                        {
                            allQuestions.Add(newQuestion);
                            context.Add(newQuestion);
                        }
                        else
                        {
                            return;
                        }
                    }

                    // Save changes to db
                    SaveChangesAndLoadFromContext();
                    NotifyPropertyChanged(nameof(Questions));
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error: wrong json file.\r\n{ex.Message}";
                }
            }
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
            } // One correct answer when one answer is allowed
            else if (!CurrentQuestion.MultipleAnswersAllowed && CurrentQuestion.Answers.Where((a) => a.IsCorrect == true).Count() != 1)
            {
                ErrorMessage = "There must be one correct answer";
                return false;
            } // Min two correct answers when multiple answers is allowed
            else if (CurrentQuestion.MultipleAnswersAllowed && CurrentQuestion.Answers.Where((a) => a.IsCorrect == true).Count() < 2)
            {
                ErrorMessage = "At least two answers must be checked as correct";
                return false;
            } // All answers has text
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
            ErrorMessage = string.Empty;

            string destinFilePath = Path.Combine(AppConfig.DbExternalFiles.QuestionImagesPath, CurrentQuestion.Id.ToString());

            if (File.Exists(destinFilePath))
            {
                DeleteImage();
            }

            if (ImageHandler.LoadImage(destinFilePath, out string errorMessage) == true)
            {
                CurrentQuestion.ImagePath = destinFilePath;
            }
            else
            {
                ErrorMessage = errorMessage;
            }
        }
        private void DeleteImage()
        {
            var fileName = CurrentQuestion.ImagePath;
            CurrentQuestion.ImagePath = null;

            ImageHandler.DeleteImageAsync(fileName);
        }

        // NAVIGATION
        private void NewOrNextQuestion()
        {
            // don't go further when data isn't correct
            if (!QuestionDataIsCorrect())
            {
                return;
            }

            // go to the next question
            if (CurrentQuestionPosition < allQuestions.Count - 1)
            {
                ++CurrentQuestionPosition;
                CurrentQuestion = Questions[CurrentQuestionPosition];
            }
            // create new question
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
            CurrentQuestion.Model.Answers.Add(newAnswer);
            CurrentQuestion.NotifyAnswersChanged();
        }
        private void DeleteAnswer(object item)
        {
            if (item is AnswerVM answer)
            {
                CurrentQuestion.Model.Answers.Remove(answer.Model);
                CurrentQuestion.NotifyAnswersChanged();

                // deleted answer can't be normally restored untill the app restart. 
                context.SaveChanges();
            }
        }

        #endregion

    }
}
