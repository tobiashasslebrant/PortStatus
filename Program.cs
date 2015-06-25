using System;
using System.Net.Sockets;

namespace portstatus
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length <= 1)
			{
				Console.WriteLine("== Checks if a port is open on an address ==");
				Console.WriteLine("portstatus.exe <address> <port>");
				return;
			}

			var address = args[0];
			var port = int.Parse(args[1]);
			var tcpClient = new TcpClient();

			try
			{

				tcpClient.Connect(address, port);
				tcpClient.Close();
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("OPEN: Port {0} is open on address {1}", port, address);
			}
			catch (Exception e)	
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("CLOSED: Port {0} is closed or not in use on address {1}", port, address);
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine("Details: {0}", e.Message);
			}
			Console.ResetColor();

		}
	}
}
