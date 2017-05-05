using System;
using System.Net;
using System.Net.Sockets;

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
    [-listen]    listening, if not specified is check");
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

        //https://msdn.microsoft.com/en-us/library/system.net.sockets.tcplistener(v=vs.110).aspx
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

                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Waiting for a connection... ");
                    Console.ResetColor();

                    var client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    
                    var stream = client.GetStream();

                    int i;
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        data = data.ToUpper();

                        var msg = System.Text.Encoding.ASCII.GetBytes(data);

                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }
                    client.Close();
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
