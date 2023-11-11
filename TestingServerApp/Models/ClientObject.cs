using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class ClientObject
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        private BinaryReader reader;
        private BinaryWriter writer;
        private int userId;
        private User? user;

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
                case ((int)RequestCode.GetQuestions):
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
            catch (Exception e) { }
        }
        private void SendAllowedTestsInfo()
        {
            try
            {
                using (Context context = new Context())
                {
                    if (user != null)
                    {
                        List<TestInfo> testsInfo = context.IssuedTests.Where((it) => it.UserGroup == user.UserGroup).Select((it) => new TestInfo(it)).ToList();
                        string testsInfoAsJson = JsonConvert.SerializeObject(testsInfo);
                        
                        if (!string.IsNullOrEmpty(testsInfoAsJson))
                        {
                            writer.Write(testsInfoAsJson);
                        }
                    }
                }
            }
            catch (Exception e) { }
        }

        private void ProcessLogin2()
        {
            try
            {


            }
            catch (Exception e) { }
        }
        public void CloseConnections()
        {
            writer.Close();
            reader.Close();
            client.Close();
        }

    }
}
