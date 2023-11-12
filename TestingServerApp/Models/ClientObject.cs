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

namespace TestingServerApp
{
    public enum RequestCode { Login, GetTests, GetAttempts, GetQuestions, SaveResults, GetStatistic, Logout }

    public class ClientObject
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        private BinaryReader reader;
        private BinaryWriter writer;
        private int userId;
        private User? user;
        private Random random = new Random(Environment.TickCount);

        TcpClient client;
        Server server;

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
                case ((int)RequestCode.GetQuestions):
                    SendTestQuestions();
                    break;
                case ((int)RequestCode.SaveResults):
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
                string login = reader.ReadString();
                string passowrd = reader.ReadString();

                using (Context context = new Context())
                {
                    user = context.Users.Where((u) => u.Login == login).Include((u) => u.UserGroup).FirstOrDefault();
                    if (user != null)
                    {
                        string checkedPasswordHash = PasswordEncryption.GetPasswordHash(passowrd, user.PasswordSalt);

                        if (checkedPasswordHash == user.PasswordHash)
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
            catch { }
        }
        private void SendAllowedTestsInfo()
        {
            try
            {
                using (Context context = new Context())
                {
                    if (user != null)
                    {
                        List<TestInfo> testsInfo = context.IssuedTests
                            .Where((it) => it.UserGroup == user.UserGroup)
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
                if (user != null)
                {
                    int testId = reader.ReadInt32();
                    writer.Write(GetAttemptsLeftAmount(testId));
                }
            }
            catch { }
        }
        private int GetAttemptsLeftAmount(int testId)
        {
            using (Context context = new Context())
            {
                int allAttempts = context.IssuedTests.Where((it) => it.UserGroup == user.UserGroup).Select((it) => it.AttemptsAmount).FirstOrDefault();
                int usedAttempts = context.ShortResults.Count((it) => it.Test.Id == testId);
                return allAttempts - usedAttempts;
            }
        }
        private void SendTestQuestions()
        {
            try
            {
                if (user != null)
                {
                    int testId = reader.ReadInt32();

                    if (GetAttemptsLeftAmount(testId) > 0)
                    {
                        using (Context context = new Context())
                        {
                            List<Question> allQuestions = context.Questions.Where((q) => q.Test.Id == testId).Include((q) => q.Answers).ToList();
                            int questionsNeeded = context.Tests.Where((t) => t.Id == testId).Select((t) => t.QuestionsAmountForTest).FirstOrDefault();

                            if (questionsNeeded <= allQuestions.Count)
                            {
                                //List<Question> questionsToSend = GetRandomQuestionsList(allQuestions, questionsNeeded);

                                string questionsToSendAsJson = JsonConvert.SerializeObject(allQuestions);
                                int i = 5;

                                if (!string.IsNullOrEmpty(questionsToSendAsJson))
                                {
                                    writer.Write(questionsToSendAsJson);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e) { }
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
                    randomOrderedQuestions.Add(allQuestions[index]);
                    //remove question from temp list to avoid it reselection
                    allQuestions.RemoveAt(index);

                }
                return randomOrderedQuestions;
            }
            return allQuestions;
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
