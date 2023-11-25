using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TestingServerApp;

namespace TestingClientApp
{
    internal class Quiz : NotifyPropertyChangedHandler
    {
        #region Properties
        /****************************************************************************************/
        private NetworkClient networkClient;
        public string UserLogin { get; set; }
        public ListCollectionView QuestionForQuiz { get; private set; }                                         
        private List<Question> storedQuestions;                                                                 

        private double userScore;
        public double UserScore
        {
            get => userScore;
            private set
            {
                userScore = value;
                NotifyPropertyChanged(nameof(UserScore));
            }
        }
        public List<TestInfo> TestsList { get; set; } = new List<TestInfo>();                                           
        public TestInfo CurrentTest { get; set; }                                                                      


        public string QuestionCategoryName { get; set; }


        private int topResultsAmount;
        public int TopResultsAmount
        {
            get { return topResultsAmount; }
            set
            {
                if (topResultsAmount != value)
                {
                    topResultsAmount = value;
                    NotifyPropertyChanged(nameof(TopResultsAmount));
                }
            }
        }
        #endregion


        #region Constructor
        /****************************************************************************************/
        public Quiz(NetworkClient networkClient)
        {
            this.networkClient = networkClient;
            this.TopResultsAmount = 20;
            //SetCategoriesList();
        }
        #endregion


        #region Methods
        /****************************************************************************************/
        // NEW
        public void SetCurretnTest(int testId)
        {
            CurrentTest = TestsList.Where((t) => t.Id == testId).FirstOrDefault();
        }

        public async Task PrepareQuizQuestion()
        {
            storedQuestions = null;
            UserScore = 0;

            // get question from server
            storedQuestions = await networkClient.GetTestQuestions(CurrentTest.Id);

            // Add references to question that were removed from answers while converting to json
            storedQuestions.ForEach((q) =>
            {
                foreach (Answer answer in q.Answers)
                {
                    answer.Question = q;
                }
            });

            List<QuestionVM> questionsVM = storedQuestions.Select((q) => new QuestionVM(q)).ToList();

            ICollectionView tempView = CollectionViewSource.GetDefaultView(questionsVM);
            QuestionForQuiz = (ListCollectionView)tempView;
        }
        public async void FinishQuiz()
        {
            Tuple<double, List<int>> result = await networkClient.GetTestResult(storedQuestions);
            UserScore = result.Item1;
            WriteUserAnswerTypeToQuestions(result.Item2);
        }
        private void WriteUserAnswerTypeToQuestions(List<int> answers)
        {
            for (int i = 0; i < storedQuestions.Count; ++i)
            {
                storedQuestions[i].UserAnswerType = (UserAnswerType)answers[i];
            }
        }

        #endregion

    }
}

