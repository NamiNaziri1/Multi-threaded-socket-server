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
            

            master = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ipE = new IPEndPoint(IPAddress.Parse("fe80::8df6:2969:45f4:3946%10"), 12345);


            try{
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
                    Console.Write(":: >");
                    Console.ForegroundColor = c;
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
