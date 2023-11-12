using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
   

    public class Server
    {
        private IPEndPoint iPEndPoint;
        private TcpListener tcpListener;
        private List<ClientObject> clients = new List<ClientObject>();


        public Server(IPEndPoint iPEndPoint)
        {
            this.iPEndPoint = iPEndPoint;
            tcpListener = new TcpListener(iPEndPoint);
        }


        protected internal async Task ListenAsync()
        {
            try
            {
                tcpListener.Start();

                while (true)
                {
                    // Wait for connection
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                    // Create client and process connection
                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    clients.Add(clientObject);
                    Task.Run(clientObject.ProcessAsync);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Disconnect();
            }
        }


        private void Disconnect()
        {
            // close conenctions in all clients
            foreach (var client in clients)
            {
                client.CloseConnections();
            }
            // stop server
            tcpListener.Stop();
        }

        public void RemoveConnection(string id)
        {
            ClientObject? client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null) clients.Remove(client);
            //client?.CloseConnections();
        }

    }
}
