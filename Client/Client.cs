using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerData;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;

namespace Client
{
    class Client
    {

        public static Socket master;
        public static string id;

        static bool flag = true;
       

        static void Main(string[] args)
        {
            

            master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            


            Console.WriteLine("Which Port Do You Want to use?");
            Console.WriteLine("1.12345      2.12346");

            int x = int.Parse(Console.ReadLine());
            int port;
            if(x == 1)
            {
                port = 12345;
            }else if (x == 2)
            {
                port = 12346;
            }
            else
            {
                port = 12345;
            }

            IPEndPoint ipE = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            try
            {
                master.Connect(ipE);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Thread.Sleep(1000);
                
            }
            Thread t = new Thread(DataIN);
            t.Start();

            while(true)
            {
                try
                {
                    Console.Write(":: >");
                    string input = Console.ReadLine();
                    Packet p;
                    if (flag == true)
                    {
                        p = new Packet(PacketType.firstNumber, id);
                        flag = false;
                    }else
                    {
                        p = new Packet(PacketType.secondNumber, id);
                        flag = true;
                    }
                    p.number = int.Parse(input);
                    master.Send(p.ToBytes());
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Server has disconnected");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }

        static void DataIN()
        {
            byte[] Buffer;
            int readBytes;
            while(true)
            {
                Buffer = new byte[master.SendBufferSize];
                readBytes = master.Receive(Buffer);


                if(readBytes > 0)
                {
                    DataManager(new Packet(Buffer));
                }
            }
        }

        static void DataManager(Packet p)
        {
            switch(p.packetType)
            {
                case PacketType.answer:
                    ConsoleColor c = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(p.number);
                    Console.ForegroundColor = c;
                    Console.Write(":: >");
                    break;
                case PacketType.busy:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Server Is Busy");
                    Console.ReadLine();
                    Environment.Exit(0);
                    break;
            }
        }


    }
}
