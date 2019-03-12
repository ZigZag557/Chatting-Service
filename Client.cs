using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Net.NetworkInformation;

namespace Chatting_application
{
    class Server
    {
        public static TcpListener tcpListener;
        public static List<TcpClient> clients;

        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("");

            try
            {
                clients = new List<TcpClient>();

                tcpListener = new TcpListener(IPAddress.Any, 1401);

                tcpListener.Start();
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There was a problem with the server, you might be trying to open multiple servers which is not possible.");
                Console.WriteLine("Press 'ENTER' to exit.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadLine();
                Environment.Exit(0);

            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Connection established with the port '1401'.");
            Console.WriteLine("If you are having trouble with the connection, check your port '1401'.");
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.White;
            while (true)
            {
                TcpClient client = null;

                client = tcpListener.AcceptTcpClient();

                clients.Add(client);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("A new connection has established.");
                Console.ForegroundColor = ConsoleColor.White;
                Thread t = new Thread(() => Listen(client));
                t.Start();
            }

        }

        public static void Listen(object obj)
        {
            bool shouldStop = false;
            TcpClient tcpClient = (TcpClient)obj;
            StreamReader sReader = new StreamReader(tcpClient.GetStream());

            while (!shouldStop)
            {
                try
                {
                    string memberName = sReader.ReadLine();
                    string message = sReader.ReadLine();
                    WriteAll(memberName,message);
                }
                catch
                {
                    shouldStop = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Connection to a person is lost.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }



        public static void WriteAll(string name, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(name + ": ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(message);
            Console.WriteLine("");

            foreach (var client in clients.ToArray())
            {
                try
                {
                    NetworkStream ns = client.GetStream();

                    StreamWriter sw = new StreamWriter(ns);
                    sw.WriteLine(name);
                    sw.WriteLine(message);
                    sw.Flush();

                }
                catch
                {
                    clients.Remove(client);
                };
            }
        }
    }
}
