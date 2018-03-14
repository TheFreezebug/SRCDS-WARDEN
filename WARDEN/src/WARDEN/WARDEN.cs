using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Drawing;

namespace WARDEN
{
    static class WARDEN
    {
        static KroConfig Configuration;
        static Process Server;

        static Thread ProcessWatcherThread; 

        public static DateTime InitialStart;
        public static DateTime LastStart;

        static bool ServerRunning = false;

        static ProtoWatch NetWatcher;

        static StreamWriter ConOut;

        private static object lockobj = new object();

        private static int QuickRestartCount = 0;
        private static int QuickRestartTime;
        private static int QuickRestartMax;
        private static int QuickRestartWait; 

        static void Main(string[] args)
        {
            Console.WriteLine("SRCDS WARDEN (C) XAYRGA (wwww.xayr.ga) 2018.");
            Console.WriteLine("Opening config.wa...");
            Configuration = new KroConfig("config.wa");
            InitialStart = DateTime.Now;

            ConsoleEx.SetScreenColors(Color.FromArgb(255, 255, 255), Color.FromArgb(0, 25, 75));

            QuickRestartTime = Convert.ToInt32(Configuration.getValue("QuickRestartTime", "20"));
            QuickRestartMax = Convert.ToInt32(Configuration.getValue("QuickRestartMax", "5"));
            QuickRestartWait = Convert.ToInt32(Configuration.getValue("QuickRestartWait", "20"));

            NetWatcher = new ProtoWatch(Configuration);

            ProcessWatcherThread = new Thread(new ThreadStart(ProcessSpin));
            ProcessWatcherThread.Start(); 

            while ( true )
            {

                Console.Write(">");
                string inp = Console.ReadLine(); 


            }


        }
        static void ProcessSpin()
        {


            while (true)
            {
                LastStart = DateTime.Now;

                try
                {
                    Warning("Starting server...");
                    StartServer();
                    Server.WaitForExit();
                    Warning("Server crashed!");

                }
                catch (Exception E)
                {
                    Err("Problem starting the server!");
                    Err(E.ToString());
                }

                if (DateTime.Now.Subtract(LastStart).TotalSeconds < QuickRestartTime)
                {
                    QuickRestartCount++;
                    Warning("Server restarted abnormally fast! {0}/{1}", QuickRestartCount, QuickRestartMax);

                    if (QuickRestartCount == QuickRestartMax)
                    {
                        Err("Restart loop detected! Waiting for {0} seconds!", QuickRestartWait);
                        Thread.Sleep(QuickRestartWait * 1000);
                        QuickRestartCount = 0;
                    }
                }
                else
                {
                    QuickRestartCount = 0;
                }

            }
        }

        static void Warning(string msg, params object[] acx)
        {
            Console.Write(DateTime.Now + " ");
            ConsoleColor asd = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("WARDEN: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg ,acx);
            Console.ForegroundColor = asd;  
        }


        static void Err(string msg, params object[] acx)
        {
            Console.Write(DateTime.Now + " ");
            ConsoleColor asd = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("!!WARDEN: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg, acx);
            Console.ForegroundColor = asd;
        }

        public static void StopServer()
        {
            ServerRunning = false;
            try
            {
                if (Server != null)
                {
                    Server.Kill();
                }
            } catch
            {

            }

        }

        static void StartServer()
        {
            string exe = Configuration.getValue("EXECUTABLE", "srcds.exe");
            string para = Configuration.getValue("parameters","");
            
            ProcessStartInfo ServerStart = new ProcessStartInfo(exe);
            ServerStart.UseShellExecute = false; 
            if (Configuration.getValue("HideWindow","false")=="true")
            {
                ServerStart.WindowStyle = ProcessWindowStyle.Hidden;

                ServerStart.RedirectStandardError = true;
                ServerStart.RedirectStandardInput = true;
                ServerStart.RedirectStandardInput = true;

               
            }
            ServerStart.Arguments = para;
  
            Server = new Process();
            Server.StartInfo = ServerStart;
    
            Server.Start();

            if (Configuration.getValue("HideWindow", "false") == "true")
            {
                Server.OutputDataReceived += sv_STDOUT;
                Server.ErrorDataReceived += sv_STDERR;

            }

        }

        private static void sv_STDOUT(object Sender,DataReceivedEventArgs ev)
        {
         
                ConsoleColor asd = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(ev.Data.ToString());

                Console.ForegroundColor = asd;
        

        }

        private static void sv_STDERR(object Sender, DataReceivedEventArgs ev)
        {

     

            ConsoleColor asd = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(ev.Data.ToString());
       
  
            Console.ForegroundColor = asd;
        }


    }
}
