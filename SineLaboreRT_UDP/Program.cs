using System;
using System.Collections.Generic;
using System.Text;

//serial port
using System.IO.Ports;

//UDP stuff
using System.Net;
using System.Net.Sockets;

namespace ConsoleApplication1
{
    class Program
    {
        private static byte COMport;
        private static int UDPPort;
        private static IPAddress IP;

        static void Main(string[] args)
        {
            SerialPort BoardSerial = new SerialPort();

            Console.WriteLine(" SinelaboreRT Serial to UPD Converter ");
            Console.WriteLine("###### justin.bauer@microchip.com ####");
            Console.WriteLine("");

            COMport = GetCOMPort();
            UDPPort = GetUPDPort(); //default is 4445
            IP = GetIP();

            //InitSerialPort();
            BoardSerial.PortName = "COM" + COMport;
            BoardSerial.BaudRate = 9600;
            BoardSerial.DataBits = 8; 
            BoardSerial.Parity = Parity.None;
            BoardSerial.StopBits = StopBits.One;
            BoardSerial.Handshake = Handshake.None;
            BoardSerial.DiscardNull = false; //important...don't discard this

            BoardSerial.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            try
            {
                BoardSerial.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.Write("Listening....");
            while (true)
            {
                ///@todo put a routine here that indicates it is waiting for INPUT
                //for (int i = 0; i < 100; ++i)
                //{
                //    Console.Write("\r{0}%   ", i);
                //}

            }
        }

        private static byte GetCOMPort()
        {
            string line;
            byte port;

            Console.Write("Enter COM port(0-255): ");
            while (true)
            {
                line = Console.ReadLine();
                if (!byte.TryParse(line, out port))
                {
                    Console.WriteLine("Not a valid input");
                }
                else
                {
                    Console.WriteLine("###### You Entered: " + line + "######"); // Report output
                    return port;
                }
            }

            
        }

        private static int GetUPDPort()
        {
            string line;
            int port;

          
            Console.Write("Enter UDP port (int - Default is 4445): ");
            while (true)
            {
                line = Console.ReadLine();
                if (!int.TryParse(line, out port))
                {
                    Console.WriteLine("Not a valid input");
                }
                else
                {
                    Console.WriteLine("###### You Entered: " + line + "######"); // Report output
                    return port;
                }
            }


        }

        private static IPAddress GetIP()
        {
            string line;
            IPAddress IP;

            Console.Write("Enter IP to send UPP packets (ex: 'localhost'):  ");
            while (true)
            {
                line = Console.ReadLine();
                if (line == "localhost")
                    return GetIPFromLocalHost();
                else
                {
                    if (!IPAddress.TryParse(line, out IP))
                    {
                        Console.WriteLine("Not a valid input");
                    }
                    else
                    {
                        Console.WriteLine("###### You Entered: " + line + "######"); // Report output
                        return IP;
                    }
                }
            }

        }

        private static IPAddress GetIPFromLocalHost()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return IPAddress.Parse(localIP);
        }

        static byte[] buffer = new byte[4096];

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            
            sp.NewLine = "\n";
            string s = sp.ReadLine();
            s.ToCharArray();


            byte[] buffer = Encoding.ASCII.GetBytes(s);
            Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                        ProtocolType.Udp);

            IPEndPoint sending_end_point = new IPEndPoint(IP, UDPPort);

            sending_socket.SendTo(buffer, sending_end_point);

            Console.WriteLine("\r\nData Received: ");
            Console.Write(s);
           // sp.DiscardInBuffer();
        }

    }
}
