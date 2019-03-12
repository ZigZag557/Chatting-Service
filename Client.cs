using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace Chatting_App
{
    class Client
    {
        public static string memberName;
        
        public static TcpClient clientTcp;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            string adress;
            bool availableName = false;
            bool availableIp = false;

            Console.WriteLine("Welcome, please enter an IP adress or domain name to connect.");

            string serverIP = Console.ReadLine();
            adress = serverIP;
            availableIp = true;
            
            Console.WriteLine("Please enter your name before we connect you to the chat service.");
            
            while (!availableName)
            {
                string name = Console.ReadLine();
                if (!isAlphanumbeic(name))
                {
                    Console.WriteLine("Your name must be alphanumeric.");
                    continue;
                }

                if (name.Length < 4)
                {
                    Console.WriteLine("Your name is too short.");
                    continue;
                }
                if (name.Length > 11)
                {
                    Console.WriteLine("Your name is too long!");
                    continue;
                }

                availableName = true;
                memberName = name;
            }

            NetworkStream ns = null;
            
            try
            {
                clientTcp = new TcpClient(serverIP, 1401);
                ns = clientTcp.GetStream();

                Thread t = new Thread(() => ReadMessages(ns));
                t.Start();

                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You have successfully connected to the server as '" + memberName + "'. Enjoy!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not connect to the host!");
                Console.WriteLine("Press 'ENTER' to leave.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadLine();
                Environment.Exit(0);
            }

            while (true)
            {
                int top = Console.CursorTop;
                string message = Console.ReadLine();

                Console.SetCursorPosition(0, top + 1);
                Console.WriteLine("");
                Console.SetCursorPosition(0, top + 1);

                SendMessage(message, ns);
            }

        }
        public static bool isAlphanumbeic(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if(!char.IsLetterOrDigit(str[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static void SendMessage(string message, NetworkStream ns)
        {
            StreamWriter streamWriter = new StreamWriter(ns);

            streamWriter.WriteLine(memberName + ": " + message);
            streamWriter.Flush();
        }

        public static void ReadMessages(NetworkStream ns)
        {
            while(true)
            {

                StreamReader sw = new StreamReader(ns);

                var readLine = sw.ReadLine();

                Console.WriteLine(readLine);

            }
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            try
            {
                clientTcp.Close();
            }
            catch { }
        }
    }
}
