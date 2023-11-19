using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TestingServerApp.Utilites;
using TestingServerApp.Viewes;

namespace TestingServerApp
{
    public enum RequestCode { Login, GetTests, GetAttempts, StartTest, GetResult, GetStatistic, Logout }
    public enum UserAnswerType { Correct, Incorrect, Partial } // also used in Client app

    public class ClientObject
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        private BinaryReader reader;
        private BinaryWriter writer;
        private User? currentUser;
        private Random random = new Random(Environment.TickCount);
        private ShortResult? currentShortResult;
        private List<int> userAnswersType = new List<int>();

        private TcpClient client;
        private Server server;

        public ClientObject(TcpClient tcpClient, Server serverObject)
        {
            client = tcpClient;
            server = serverObject;

            var stream = client.GetStream();

            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
        }

        public async Task ProcessAsync()
        {
            try
            {
                string message = string.Empty;
                while (true)
                {
                    var code = reader.ReadInt32();
                    ProcessRequest(code);
                }
            }
            catch (Exception e)
            {
                LogWriter.Write($"{e.Message}\r\n{e.InnerException}");
            }
            finally
            {
                CloseConnections();
                server.RemoveConnection(Id);
            }
        }

        private void ProcessRequest(int code)
        {
            switch (code)
            {
                case ((int)RequestCode.Login):
                    ProcessLogin();
                    break;
                case ((int)RequestCode.GetTests):
                    SendAllowedTestsInfo();
                    break;
                case ((int)RequestCode.GetAttempts):
                    SendAttemptLeftAmount();
                    break;
                case ((int)RequestCode.StartTest):
                    StartTest();
                    break;
                case ((int)RequestCode.GetResult):
                    ProcessTestResult();
                    break;
                case ((int)RequestCode.GetStatistic):
                    break;
                case ((int)RequestCode.Logout):
                    break;
            }
        }
        private void ProcessLogin()
        {
            try
            {
                currentUser = null;

                string login = reader.ReadString();
                string passowrd = reader.ReadString();

                using (Context context = new Context())
                {
                    currentUser = context.Users.Where((u) => u.Login == login).Include((u) => u.UserGroup).FirstOrDefault();
                    if (currentUser != null)
                    {
                        string checkedPasswordHash = PasswordEncryption.GetPasswordHash(passowrd, currentUser.PasswordSalt);

                        if (checkedPasswordHash == currentUser.PasswordHash)
                        {
                            writer.Write(true);
                        }
                        else
                        {
                            writer.Write(false);
                        }
                    }
                }
            }
            catch (Exception e) { LogWriter.Write($"{e.Message}\r\n{e.InnerException}"); }
        }
        private void SendAllowedTestsInfo()
        {
            try
            {
                using (Context context = new Context())
                {
                    if (currentUser != null)
                    {
                        List<TestInfo> testsInfo = context.IssuedTests
                            .Where((it) => it.UserGroup == currentUser.UserGroup)
                            .Select((it) => new TestInfo()
                            {
                                Id = it.Test.Id,
                                Name = it.Test.Name,
                                Image = it.Test.ImagePath != null ? File.ReadAllBytes(it.Test.ImagePath) : null,
                                QuestionsAmountForTest = it.Test.QuestionsAmountForTest,
                                MinutsForTest = it.Test.MinutsForTest,
                                MaxTestScores = it.Test.MaxTestScores,
                                TestCategory = it.Test.TestCategory.Name,
                                AttemptsAmount = it.AttemptsAmount,
                                ExiredDate = it.ExpiredDate
                            })
                            .ToList();
                        string testsInfoAsJson = JsonConvert.SerializeObject(testsInfo);

                        if (!string.IsNullOrEmpty(testsInfoAsJson))
                        {
                            writer.Write(testsInfoAsJson);
                        }
                    }
                }
            }
            catch { }
        }
        private void SendAttemptLeftAmount()
        {
            try
            {
                if (currentUser != null)
                {
                    int testId = reader.ReadInt32();
                    writer.Write(GetAttemptsLeftAmount(testId));
                }
            }
            catch (Exception e) { LogWriter.Write($"{e.Message}\r\n{e.InnerException}"); }
        }
        private int GetAttemptsLeftAmount(int testId)
        {
            using (Context context = new Context())
            {
                int allAttempts = context.IssuedTests.Where((it) => it.UserGroup == currentUser.UserGroup).Select((it) => it.AttemptsAmount).FirstOrDefault();
                int usedAttempts = context.ShortResults.Count((it) => it.Test.Id == testId);
                return allAttempts - usedAttempts;
            }
        }
        private void StartTest()
        {
            try
            {
                if (currentUser != null)
                {
                    currentShortResult = null;

                    int testId = reader.ReadInt32();

                    SendTestQuestions(testId);

                    WriteUserAttempt(testId);
                }
            }
            catch (Exception e) { LogWriter.Write($"{e.Message}\r\n{e.InnerException}"); }
        }
        private void SendTestQuestions(int testId)
        {
            if (GetAttemptsLeftAmount(testId) > 0)
            {
                using (Context context = new Context())
                {
                    List<Question> allQuestions = context.Questions.Where((q) => q.Test.Id == testId).Include((q) => q.Answers).ToList();
                    int questionsNeeded = context.Tests.Where((t) => t.Id == testId).Select((t) => t.QuestionsAmountForTest).FirstOrDefault();

                    if (questionsNeeded <= allQuestions.Count)
                    {
                        List<Question> questionsToSend = GetRandomQuestionsList(allQuestions, questionsNeeded);

                        string questionsToSendAsJson = JsonConvert.SerializeObject(questionsToSend, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                        if (!string.IsNullOrEmpty(questionsToSendAsJson))
                        {
                            writer.Write(questionsToSendAsJson);
                        }
                    }
                }
            }
        }
        private List<Question> GetRandomQuestionsList(List<Question> allQuestions, int questionsNeeded)
        {

            if (allQuestions.Count >= questionsNeeded)
            {
                List<Question> randomOrderedQuestions = new List<Question>();

                int index;

                for (int i = 0; i < questionsNeeded; ++i)
                {
                    index = random.Next(0, allQuestions.Count);
                    Question question = allQuestions[index];
                    // Load image to question
                    if (question.ImagePath != null) question.Image = File.ReadAllBytes(question.ImagePath);

                    randomOrderedQuestions.Add(allQuestions[index]);

                    //remove question from temp list to avoid it reselection
                    allQuestions.RemoveAt(index);
                }
                return randomOrderedQuestions;
            }
            return allQuestions;
        }
        private void WriteUserAttempt(int testId)
        {
            using (Context context = new Context())
            {
                Test test = context.Tests.Where((t) => t.Id == testId).FirstOrDefault()!;
                // Prevent error when context try to save UserGroup that is placed inside the current user
                User user = context.Users.Where((u) => u == currentUser).FirstOrDefault()!;

                currentShortResult = new ShortResult()
                {
                    Date = DateTime.Now,
                    Test = test,
                    TestMaxScores = test.MaxTestScores,
                    User = user!,
                    // Initial value that will be updated when user passed the test
                    UserScores = 0
                };

                context.Add(currentShortResult);
                context.SaveChanges();
            }
        }

        private void ProcessTestResult()
        {
            try
            {
                if (currentUser != null && currentShortResult != null)
                {
                    // Read question with answers
                    string answeredQuestionsAsJson = reader.ReadString();
                    List<Question> answeredQuestions = JsonConvert.DeserializeObject<List<Question>>(answeredQuestionsAsJson, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                    if (answeredQuestions != null)
                    {
                        userAnswersType.Clear();

                        // Get and send test results
                        double userTestScore = EstimateAnswers(answeredQuestions!);
                        // Send test score
                        writer.Write(userTestScore);
                        // Send answers type (correct/incorrect/partial)
                        string userAnswersAsJson = JsonConvert.SerializeObject(userAnswersType);
                        writer.Write(userAnswersAsJson);

                        // Save results to db
                        SaveTestResults(answeredQuestions, userTestScore);
                    }
                }
            }
            catch (Exception e) { LogWriter.Write($"{e.Message}\r\n{e.InnerException}"); }
        }

        private double EstimateAnswers(List<Question> answeredQuestions)
        {
            double userScore = 0;
            double testScore = 0;

            foreach (Question question in answeredQuestions)
            {
                testScore += question.QuestionWeight;

                double userCorrectAnswers = question.Answers.Count((a) => a.IsCorrect && a.IsUserAnswered);

                if (userCorrectAnswers == 0)
                {
                    userAnswersType.Add((int)UserAnswerType.Incorrect);
                    continue;
                }

                double userWrongAnswers = question.Answers.Count((a) => a.IsCorrect != a.IsUserAnswered);

                if (userWrongAnswers == 0)
                {
                    userAnswersType.Add((int)UserAnswerType.Correct);
                    userScore += question.QuestionWeight;
                }
                else if (question.MultipleAnswersAllowed == true)
                {
                    userAnswersType.Add((int)UserAnswerType.Partial);
                    userScore += userCorrectAnswers / (userCorrectAnswers + userWrongAnswers) * question.QuestionWeight;
                }
            }

            return userScore / testScore * currentShortResult!.TestMaxScores;
        }
        private void SaveTestResults(List<Question> answeredQuestions, double testScore)
        {
            using (Context context = new Context())
            {
                // Save user score to short result table
                ShortResult? shortResult = context.ShortResults.Where((i) => i.Id == currentShortResult.Id).FirstOrDefault();
                if (shortResult != null)
                {
                    shortResult.UserScores = testScore;

                    // Save detailed results...

                    context.SaveChanges();
                }
            }
        }


        private void ProcessLogin2()
        {
            try
            {


            }
            catch { }
        }
        public void CloseConnections()
        {
            writer.Close();
            reader.Close();
            client.Close();
        }

    }
}
