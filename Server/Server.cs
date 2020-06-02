using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerData;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;

namespace Server
{
    class Server
    {

        static Socket listenerSocket;
        static List<ClientData> clients;

        static void Main(string[] args)
        {

            //Console.WriteLine("Starting Server On " + Packet.GetIP4Address());
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clients = new List<ClientData>();
            IPEndPoint ip= new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);

            listenerSocket.Bind(ip);


            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();


        }


        static void ListenThread()
        {
            while(true)
            {
                listenerSocket.Listen(0);

                

                if (clients.Count == 4)
                {
                    Packet p = new Packet(PacketType.busy, "");
                    Socket acc = listenerSocket.Accept();
                    acc.Send(p.ToBytes());
                    acc.Close();
                }
                else
                {
                    Console.WriteLine("Number Of Current Client: " + clients.Count);
                    clients.Add(new ClientData(listenerSocket.Accept()));
                }

                /////////////////////////
            }
        }


        public static void DataIN(object ob)
        {
            ClientData cd = (ClientData)ob;
            Socket clientSocket = cd.clientSocket;
            
            byte[] Buffer;
            int readBytes;

            while(true)
            {
                try
                {
                    Buffer = new byte[clientSocket.SendBufferSize];

                    readBytes = clientSocket.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        Packet packet = new Packet(Buffer);
                        DataManager(packet, cd);
                    }
                }
                catch(SocketException ex)
                {
                    
                        

                    Console.WriteLine("A Client disconnected!");
                    clientSocket.Close();
                    clients.Remove(cd);
                    Console.WriteLine("Number Of Current Client: " + clients.Count);

                    break;
                }
            }
        }
        public static void DataManager(Packet p, ClientData cd)
        {
            switch (p.packetType)
            {
                case PacketType.firstNumber:
                    cd.number = p.number;
                    break;
                case PacketType.secondNumber:
                    int x = p.number + cd.number;
                    Socket s = cd.clientSocket;
                    p.number = x;
                    p.packetType = PacketType.answer;
                    cd.clientSocket.Send(p.ToBytes());
                    break;
            }
        }

    }



    class ClientData
    {
        public int number;
        public Socket clientSocket;
        public Thread clientThread;
        public string id;


        public ClientData()
        {

            /////////////
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.DataIN);
            clientThread.Start(this);
        }
        public ClientData(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.DataIN);
            clientThread.Start(this);
        }


    }

}
