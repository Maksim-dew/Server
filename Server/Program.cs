using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Serilog;
using Serilog.Core;

namespace ClientServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File("FileLog.log").CreateLogger();

            Log.Information("The is a sample information");

            try
            {
                TcpListener serverSocket = new TcpListener(IPAddress.Any, 7000);
                Console.WriteLine("Server started");
                serverSocket.Start();

                while (true)
                {
                    TcpClient clientSocket = serverSocket.AcceptTcpClient();
                    NetworkStream stream = clientSocket.GetStream();

                    byte[] bytes = new byte[256];
                    int length = stream.Read(bytes, 0, bytes.Length);
                    string request = Encoding.ASCII.GetString(bytes, 0, length);
                    Console.WriteLine("Got request: " + request);

                    string message = "Length of yuor request: " + request.Length;
                    bytes = Encoding.ASCII.GetBytes(message);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();

                    Console.WriteLine("Sent message: " + message);
                    clientSocket.Close();
                }

                serverSocket.Stop();
                Console.WriteLine("Server stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                Log.Error(ex, "Some error occurred");
            }
            Console.ReadLine();

            Log.CloseAndFlush();
            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
