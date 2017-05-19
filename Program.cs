using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace portstatus
{
	class Program
	{
		static void Main(string[] args)
		{
         	if (args.Length < 1)
			{
				Console.Write($@"
DESCRIPTION: Checks if a port is open on an address 
SYNTAX: portstatus.exe address port [-listen]
ARGUMENTS:  
    address      Address to check or listen to (can be * when listening)  
    port         Port to check or listen to
    [-listen]    listening, if not specified is check

EXAMPLES:
    portstatus.exe 1.2.3.4 90       Check if port 90 is open on ip address 1.2.3.4
    portstatus.exe * 88 -listen     Listens for incoming calls to current machine to port 88 on all ip addresses

");
                return;
			}

			var address = args[0];
		    var port = int.Parse(args[1]);

            if(args.Length == 2)
                CheckPort(address, port);
            else if(args[2] == "-listen")
                ListenToPort(address, port);
            else
                Console.WriteLine("wrong arguments");
        }

	    static void CheckPort(string address, int port)
	    {
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

        static object _lock = new object();
        //http://stackoverflow.com/questions/19387086/how-to-set-up-tcplistener-to-always-listen-and-accept-multiple-connections
        static void ListenToPort(string address, int port)
        {
            TcpListener server = null;
            try
            {
                var ipAddress = address == "*"
                    ? IPAddress.Any
                    : IPAddress.Parse(address);
                
                server = new TcpListener(ipAddress, port);
                server.Start();

                var bytes = new byte[256];

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Waiting for a connection... ");
                Console.ResetColor();

                while (true)
                {
                    var client = server.AcceptSocket();
                    var childSocketThread = new Thread(() =>
                    {
                        lock (_lock)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"----- Connection accepted at UTC {DateTime.UtcNow:T}. -----");
                            Console.ResetColor();

                            var data = new byte[1000];
                            var size = client.Receive(data);
                            var message = "";
                            Console.WriteLine("Recieved data: ");
                            for (var i = 0; i < size; i++)
                                message += Convert.ToChar(data[i]);

                            Console.WriteLine(message);
                        }
                        client.Close();
                    });
                    childSocketThread.Start();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}
