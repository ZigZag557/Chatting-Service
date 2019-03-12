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

namespace Chat_server
{
    class Program
    {
        public static TcpListener tcpListener;
        public static List<TcpClient> clients;

        static void Main(string[] args)
        {
            try
            {
                clients = new List<TcpClient>();

                tcpListener = new TcpListener(IPAddress.Any, 1401);

                tcpListener.Start();
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There was a problem with server.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(ex);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Connection established with the port '1401'.");
            Console.WriteLine("If you are having trouble with the connection, check your port '1401'.");
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.White;
            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
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

            while(!shouldStop)
            {
                try
                {
                    string message = sReader.ReadLine();
                    WriteAll(message);
                }
                catch
                {
                    shouldStop = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Connection to a person is lost.");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
            }
        }



        public static void WriteAll(string str)
        {
            Console.WriteLine(str);

            foreach (var client in clients.ToArray())
            {
                try
                {
                    NetworkStream ns = client.GetStream();

                    StreamWriter sw = new StreamWriter(ns);
                    sw.WriteLine(str);
                    sw.Flush();

                }
                catch
                {
                    clients.Remove(client);
                };
            }
           
        }

        public static void RemoveDisconnected()
        {

        }

    }
}
