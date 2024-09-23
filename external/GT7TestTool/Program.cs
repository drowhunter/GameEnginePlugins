using PDTools.SimulatorInterface;

using PluginHelper;

using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GT7TestTool
{
    internal class Program
    {
        private static bool _showUnknown = false;

        private static UdpClient udpClient;

        static void Main(string[] args)
        {
            /* Mostly a test sample for using the Simulator Interface library */

            Console.WriteLine("Simulator Interface GT7/GTSport/GT6 - Nenkai#9075");
            Console.WriteLine();

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: SimulatorInterface.exe <IP address of PS4/PS5> ('--gtsport' for GT Sport support, --gt6 for GT6 support, optional: '--debug' to show unknown values)");
                return;
            }

            _showUnknown = args.Contains("--debug");
            bool gtsport = args.Contains("--gtsport");
            bool gt6 = args.Contains("--gt6");

            if (gtsport && gt6)
            {
                Console.WriteLine("Error: Both GT6 and GT Sport arguments are present.");
                return;
            }

            Console.WriteLine("Starting interface..");

            SimulatorInterfaceGameType type = SimulatorInterfaceGameType.GT7;
            if (gtsport)
                type = SimulatorInterfaceGameType.GTSport;
            else if (gt6)
                type = SimulatorInterfaceGameType.GT6;

            
            
            var simInterface = new SimulatorInterfaceClient(args[0], type);

            var udpserver = new UdpServer(SimulatorInterfaceClient.ReceivePortGT7);
            udpserver.OnClientConnected += (connected) =>
            {
                Console.WriteLine($"{DateTime.Now:hh:mm:ss} Client connected: {connected}");
            };

            

            // Connect to simhub udp forwarded telemetry port
            //var simInterface = new SimulatorInterfaceClient(IPAddress.Loopback.ToString(), type, 33741);

            simInterface.OnReceive += SimInterface_OnReceive;

            simInterface.OnRawData += async (data) =>
            {
                if (udpserver.ClientConnected)
                {
                    await udpserver.SendAsync(data, 33741);
                }
            };

            var cts = new CancellationTokenSource();

            // Cancel token from outside source to end simulator
            
            var forwarderTask = udpserver.StartListenerAsync(cts.Token);
            var task = simInterface.Start(cts.Token);
            
            try
            {
                //await task;
                Task.WaitAll(task, forwarderTask);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"Simulator Interface ending..");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Errored during simulation: {e.Message}");
            }
            finally
            {
                // Important to clear up underlaying socket
                simInterface.Dispose();
            }
        }

        
        private static void SimInterface_OnReceive(SimulatorPacket packet)
        {
            // Print the packet contents to the console
            Console.SetCursorPosition(0, 0);
            packet.PrintPacket(_showUnknown);

            // Get the game type the packet was issued from
            SimulatorInterfaceGameType gameType = packet.GameType;
            
            // Check on flags for whether the simulation is active
            if (packet.Flags.HasFlag(SimulatorFlags.CarOnTrack) && !packet.Flags.HasFlag(SimulatorFlags.Paused) && !packet.Flags.HasFlag(SimulatorFlags.LoadingOrProcessing))
            {
                // Do stuff with packet
            }
        }
    }
}