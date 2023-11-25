using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestingServerApp;

namespace TestingClientApp
{
    public class NetworkClient
    {
        private string serverHost;
        private int serverPort;
        private TcpClient client;
        private BinaryReader reader;
        private BinaryWriter writer;


        public NetworkClient()
        {
            LoadConfig();
            TryConnectToServer();
        }

        public async Task<bool> Login(string login, string password)
        {
            bool result = false;

            await Task.Run(() =>
            {
                try
                {
                    writer.Write((int)RequestCode.Login);
                    writer.Write(login);
                    writer.Write(password);

                    result = reader.ReadBoolean();
                }
                catch (Exception e)
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate { ShowServerConnectionError(); });
                }
            });

            return result;
        }

        public async Task<List<TestInfo>> GetAllowedTestsInfo()
        {
            List<TestInfo> testInfos = new List<TestInfo>();

            await Task.Run(() =>
            {
                try
                {
                    writer.Write((int)RequestCode.GetTests);

                    string testsAsJson = reader.ReadString();
                    testInfos = JsonConvert.DeserializeObject<List<TestInfo>>(testsAsJson);

                    if (testInfos == null)
                    {
                        testInfos = new List<TestInfo>();
                    }
                }
                catch
                {
                    Application.Current.Dispatcher.Invoke(() => ShowServerConnectionError());
                }
            });

            return testInfos;
        }
        public async Task<int> GetTestAttemptsLeftAmount(int testId)
        {
            int amount = 0;

            await Task.Run(() =>
            {
                try
                {
                    writer.Write((int)RequestCode.GetAttempts);
                    writer.Write(testId);
                    amount = reader.ReadInt32();
                }
                catch
                {
                    Application.Current.Dispatcher.Invoke(() => ShowServerConnectionError());
                }
            });

            return amount;
        }
        public async Task<List<Question>> GetTestQuestions(int testId)
        {
            List<Question> questions = new List<Question>();

            await Task.Run(() =>
            {
                try
                {
                    writer.Write((int)RequestCode.StartTest);
                    writer.Write(testId);

                    string questionsAsJson = reader.ReadString();
                    questions = JsonConvert.DeserializeObject<List<Question>>(questionsAsJson, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                    if (questions == null)
                    {
                        questions = new List<Question>();
                    }
                }
                catch
                {
                    Application.Current.Dispatcher.Invoke(() => ShowServerConnectionError());
                }
            });

            return questions;
        }

        public async Task<Tuple<double,List<int>>> GetTestResult(List<Question> questions)
        {
            double testResult = 0;
            List<int> userAnswerType = new List<int>();

            await Task.Run(() =>
            {
                try
                {
                    writer.Write((int)RequestCode.GetResult);

                    // Send questions with user answers
                    string questionsAsJson = JsonConvert.SerializeObject(questions, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                    if (!string.IsNullOrEmpty(questionsAsJson))
                    {
                        writer.Write(questionsAsJson);
                    }

                    testResult = reader.ReadDouble();
                    string userAnswersAsJson = reader.ReadString();
                    var buffer = JsonConvert.DeserializeObject<List<int>>(userAnswersAsJson);
                    if (buffer != null)
                    {
                        userAnswerType = buffer;
                    }

                }
                catch
                {
                    Application.Current.Dispatcher.Invoke(() => ShowServerConnectionError());
                }
            });

            return new Tuple<double, List<int>>(testResult, userAnswerType);
        }









        private void TryConnectToServer()
        {
            try
            {
                client = new TcpClient(serverHost, serverPort);

                reader = new BinaryReader(client.GetStream());
                writer = new BinaryWriter(client.GetStream());
            }
            catch
            {
                MessageShowDialogWindow messageShowDialogWindow = new MessageShowDialogWindow($"Server connection error.\nApplication can't start");
                messageShowDialogWindow.ShowDialog();
                Application.Current.Shutdown();
            }
        }

        private void LoadConfig()
        {
            var builder = new ConfigurationBuilder();
            // Set path to the current directory
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // Get config from file Appconfig.json
            builder.AddJsonFile("Appconfig.json");
            // Create config
            var config = builder.Build();

            // Server settings
            serverHost = config.GetSection("ServerIpAdress").Value;
            int.TryParse(config.GetSection("ServerPort").Value, out int port);
            serverPort = port;
        }
        public static void ShowServerConnectionError()
        {
            MessageShowDialogWindow messageShowDialogWindow = new MessageShowDialogWindow($"Server connection error.");
            messageShowDialogWindow.ShowDialog();
        }

        public void CloseConnections()
        {
            try
            {
                reader?.Close();
                writer?.Close();
                client?.Close();
            }
            catch { }
        }
    }
}
