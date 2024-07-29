using GT7Plugin.Properties;

using PDTools.SimulatorInterface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Threading;
using Quaternion = System.Numerics.Quaternion;

using YawGEAPI;

namespace YawVR_Game_Engine.Plugin
{
    [Export(typeof(Game))]
    [ExportMetadata("Name", "Gran Turismo 7")]
    [ExportMetadata("Version", "1.0")]
    public class GT7Plugin : Game
    {
        private volatile bool _running = false;
        private IMainFormDispatcher _dispatcher;
        private IProfileManager _profileManager;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private bool _seenPacket = false;
        
        private Vector3 _previous_local_velocity = new Vector3(0, 0, 0);
        
        
        private float _sampling_rate = 1f / 60f;

        
        public int STEAM_ID => 0;

        public string PROCESS_NAME => string.Empty;

        public bool PATCH_AVAILABLE => false;

        public string AUTHOR => "Drowhunter";

        public string Description => Resources.description;

        public Image Logo => Resources.logo;

        public Image SmallLogo => Resources.small;

        public Image Background => Resources.background;



        private static readonly string[] inputs = new string[]
        {
          "Yaw",
          "Pitch",
          "Roll",          
          "Sway",
          "Surge",
          "Heave"         
        };

        public LedEffect DefaultLED()
        {
            return new LedEffect(EFFECT_TYPE.KNIGHT_RIDER_2, 7, new YawColor[4]
            {
                new YawColor((byte) 66, (byte) 135, (byte) 245),
                new YawColor((byte) 80, (byte) 80, (byte) 80),
                new YawColor((byte) 128, (byte) 3, (byte) 117),
                new YawColor((byte) 110, (byte) 201, (byte) 12)
            }, 25f);
        }

        public List<Profile_Component> DefaultProfile()
        {
            return new List<Profile_Component>()
            {
                new Profile_Component(4, 1, 1f, 0.0f, 0.0f, false, false, -1f, 1f),
                new Profile_Component(7, 1, 0.5f, 0.0f, 0.0f, false, true, -1f, 0.3f),
                new Profile_Component(1, 2, 0.1f, 0.0f, 0.0f, false, false, -1f, 0.05f),
                new Profile_Component(5, 2, 1f, 0.0f, 0.0f, false, true, -1f, 1f),
                new Profile_Component(0, 1, 0.1f, 0.0f, 0.0f, false, false, -1f, 1f),
                new Profile_Component(3, 0, 1f, 0.0f, 0.0f, false, true, -1f, 1f)
            };
        }

        public void Exit()
        {
            this._running = false;
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
        }

        public Dictionary<string, ParameterInfo[]> GetFeatures()
        {
            return null;
        }

        public string[] GetInputData()
        {
            return GT7Plugin.inputs;
        }

        public async void Init()
        {
            this._running = true;

            SimulatorInterfaceClient simInterface = new SimulatorInterfaceClient(IPAddress.Broadcast.ToString(), SimulatorInterfaceGameType.GT7);
            simInterface.OnReceive += SimInterface_OnReceive;

            _cts = new CancellationTokenSource();

            // Cancel token from outside source to end simulator

            var task = simInterface.Start(_cts.Token);

            try
            {
                await task;
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

        public void PatchGame()
        {
            
        }

        public void SetReferences(IProfileManager controller, IMainFormDispatcher dispatcher)
        {
            this._dispatcher = dispatcher;
            this._profileManager = controller;
        }

        private void SimInterface_OnReceive(SimulatorPacket packet)
        {
            // Print the packet contents to the console
            //Console.SetCursorPosition(0, 0);
            try
            {
                packet.PrintPacket(false);

            } 
            catch (Exception e)
            {
                Console.WriteLine($"Error printing packet: {e.Message}");
            }

            // Get the game type the packet was issued from
            SimulatorInterfaceGameType gameType = packet.GameType;

            // Check on flags for whether the simulation is active
            if (packet.Flags.HasFlag(SimulatorFlags.CarOnTrack) && !packet.Flags.HasFlag(SimulatorFlags.Paused) && !packet.Flags.HasFlag(SimulatorFlags.LoadingOrProcessing))
            {
                //if(packet.PacketId)
                ReadFunction(packet);
            }

            _seenPacket = _seenPacket || true;
        }

        private void ReadFunction(SimulatorPacket packet)
        {
            // Calculate local velocity based on quaternion (Q is assumed to be normalized)

            var Q = new Quaternion(packet.Rotation, packet.RelativeOrientationToNorth);
            var local_velocity = Maths.WorldtoLocal(Q, packet.Velocity);

            var sway = CalculateCentrifugalAcceleration(local_velocity, packet.AngularVelocity);

            var surge = 0f;

            if (_seenPacket)
            {
                var delta_velocity = local_velocity - _previous_local_velocity;                

                surge = delta_velocity.Z * 100;
            }
            
            
            
            _previous_local_velocity = local_velocity;

            
            var (yaw, pitch, roll) = Maths.ToEuler(Q);


            _profileManager.SetInput(0, yaw);
            _profileManager.SetInput(1, pitch);
            _profileManager.SetInput(2, roll);
            _profileManager.SetInput(3, sway);
            _profileManager.SetInput(4, surge);
            
        }


        public float CalculateCentrifugalAcceleration(Vector3 velocity, Vector3 angularVelocity)
        {
            var Fc = velocity.Length() * angularVelocity.Length();            

            return Fc * (angularVelocity.Y >= 0 ? 1 : -1);
            
        }

        
    }

    
}
