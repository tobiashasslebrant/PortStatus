﻿using System;
using System.Configuration;
using System.Net.Sockets;

namespace portstatus
{
	class Program
	{
		static void Main(string[] args)
		{
		    var defaultPort = int.Parse(ConfigurationManager.AppSettings.Get("defaultPort") ?? "80");

         	if (args.Length < 1)
			{
				Console.WriteLine("DESCRIPTION: Checks if a port is open on an address");
				Console.WriteLine("SYNTAX: portstatus.exe address [port]");
                Console.WriteLine("ARGUMENTS:\r\n" +
                                  "  address      Address to check\r\n" +
                                 $"  port         Port to check, Default {defaultPort}");

                return;
			}

			var address = args[0];
			var port = args.Length == 1 ? defaultPort : int.Parse(args[1]);
			
			try
			{
			    using (var tcpClient = new TcpClient())
			    {
			        tcpClient.Connect(address, port);
			        tcpClient.Close();
			    }
			    Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"OPEN: Port {port} is open on address {address}");
			}
			catch (Exception e)	
			{
                Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"CLOSED: Port {port} is closed or not in use on {address}");
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine($"Details: {e.Message}");
			}
			Console.ResetColor();
		}
	}
}
