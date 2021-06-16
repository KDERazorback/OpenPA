﻿
using OpenPA;
using OpenPA.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestOpenPA
{
    class Program
    {
#if DEBUG
        static string? addr = "tcp:10.1.10.12";        
#else
        //static string addr = "unix:/run/user/1000/pulse/native";
        static string? addr = null;
#endif
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Connecting to {0}", addr);

            Console.WriteLine("PULSE_SERVER: {0}", Environment.GetEnvironmentVariable("PULSE_SERVER"));

            Task t = program();
            t.Wait();

            Console.WriteLine("Good-bye");
        }

        static async Task program()
        {
            PulseAudio.Init();            

            PAContext context = new(MainLoop.Instance, "TestApp");
            var t = await context.ConnectAsync(addr);
            

            Console.WriteLine("Connection succeeded: {0}", t);
            if (!t)
            {
                MainLoop.Instance.Stop();
                MainLoop.Instance.Dispose();
                return;
            }

            ServerInfo? serverInfo = await context.GetServerInfoAsync();
            if (serverInfo != null)
            {
                Console.WriteLine("Host Name: {0}", serverInfo.HostName);
                Console.WriteLine("User Name: {0}", serverInfo.UserName);
                Console.WriteLine("Server Name: {0}", serverInfo.ServerName);
                Console.WriteLine("Server Version: {0}", serverInfo.ServerVersion);
                Console.WriteLine("Default Source Name: {0}", serverInfo.DefaultSourceName);
                Console.WriteLine("Default Sink Name: {0}", serverInfo.DefaultSinkName);
                Console.WriteLine("Sample Spec: {0} {1} {2}", serverInfo.SampleSpec?.Format, serverInfo.SampleSpec?.Channels, serverInfo.SampleSpec?.Rate);

                if (serverInfo.ChannelMap != null && serverInfo.ChannelMap.Map != null)
                {
                    Console.Write("Channels:");
                    foreach (ChannelPosition channelPosition in serverInfo.ChannelMap.Map)
                    {
                        Console.Write(" {0}", channelPosition);
                    }
                    Console.WriteLine();
                }

                if (!String.IsNullOrWhiteSpace(serverInfo.DefaultSinkName))
                {
                    //var sink = await context.GetSinkInfoAsync(serverInfo.DefaultSinkName);
                    var sinks = await context.GetSinkInfoListAsync();
                    if (sinks != null)
                    {
                        foreach(var sink in sinks)
                        {
                            Console.WriteLine("{0}", sink?.Name);
                        }
                    }
                    
                }

                var s = await context.GetSinkInfoAsync(0);
                var s1 = await context.GetSinkInfoAsync("output");

                var sources = await context.GetSourceInfoListAsync();
                var src = await context.GetSourceInfoAsync(0);
                var src1 = await context.GetSourceInfoAsync(serverInfo.DefaultSourceName ?? String.Empty);
                //Console.WriteLine("Got Source");
            }
            context.Disconnect();            
            context.Dispose();

            MainLoop.Instance.Stop();
            MainLoop.Instance.Dispose();

        }
    }
}
