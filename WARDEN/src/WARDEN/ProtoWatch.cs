using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace WARDEN
{
    class ProtoWatch
    {
        static Thread PingThread;
        static IPEndPoint GameServer; 
        static  UdpClient UPing;

        static int PingDelay;
        static int PingTimeout;
        static int FailedPings;
        static int MaxFailedPings;
        static int StartupDelay; 

        public ProtoWatch(KroConfig Configuration)
        {

            string enabled = Configuration.getValue("PW_Enable", "false");
            string ip = Configuration.getValue("PW_IP", "127.0.0.1");
            string port = Configuration.getValue("PW_PORT", "27015");

            PingDelay = Convert.ToInt16(Configuration.getValue("PW_PingDelay", "10"));
            PingTimeout = Convert.ToInt16(Configuration.getValue("PW_PingTimeout", "2"));
            MaxFailedPings = Convert.ToInt16(Configuration.getValue("PW_MaxFailedPings", "12"));
            StartupDelay = Convert.ToInt16(Configuration.getValue("PW_StartupDelay", "60"));

            if (enabled=="true")
            {
                try
                {
                    GameServer = new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt16(port));
                    UPing = new UdpClient();
                    PingThread = new Thread(new ThreadStart(pingSpin));
                    PingThread.Start(); 

                } catch (Exception E)
                {
                    Console.Write(DateTime.Now + " ");
                    Console.WriteLine("Something bad happened to ProtoWatch!");
                    Console.WriteLine(E.ToString());
                    Console.WriteLine("ProtoWatch is disabled for this session!");
                }

            }
        }
        static void pingSpin()
        {
            Console.Write(DateTime.Now + " ");
            Console.WriteLine("PROTOWATCH: Waiting for startup....");
            waitStartup();
            Console.Write(DateTime.Now + " ");
            Console.WriteLine("PROTOWATCH: Startup wait time elapsed -- Starting to monitor server.");

            while (true)
            {
                bool success = SRCDSPing(); 
                if (!success)
                {
                    if (DateTime.Now.Subtract(WARDEN.LastStart).TotalSeconds < StartupDelay )
                    {
                        Console.Write(DateTime.Now + " ");
                        Console.WriteLine("PROTOWATCH: The server didn't respond to the ping thread, but the main thread says it's already starting a new instance.");
                        waitStartup(); 
                    }
                    Console.Write(DateTime.Now + " ");
                    Console.WriteLine("PROTOWATCH: Server didn't respond to rules request...");
                    FailedPings++; 

                    if (FailedPings==MaxFailedPings)
                    {
                        FailedPings = 0;

                        Console.Write(DateTime.Now + " ");
                        Console.WriteLine("PROTOWATCH: No response for {0} requests. Restarting server. ",MaxFailedPings);
                        WARDEN.StopServer();
                        waitStartup(); 
                    }
                   

                } else
                {
                    FailedPings = 0; 
                }
                Thread.Sleep(PingDelay * 1000);
            }

        }

        static void waitStartup()
        {
            Thread.Sleep(StartupDelay * 1000);
        }
        static bool SRCDSPing()
        {
            try
            {
                UPing.Connect(GameServer);
                UPing.Send(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x56, 0xFF, 0xFF, 0xFF, 0xFF }, 9);
                UPing.Client.ReceiveTimeout = PingTimeout * 1000;

                byte[] resp = UPing.Receive(ref GameServer);
                return true;
            } catch (Exception E)
            {
               // Console.WriteLine(E);
                return false;
            }

        }
    }
}
