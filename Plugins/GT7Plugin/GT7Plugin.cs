using GT7Plugin.Properties;

using PDTools.SimulatorInterface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Threading;

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
        private Vector3 _accel_vector = new Vector3(0, 0, 0);
        private float slip_angle = 0f;

        private float _previous_mph = 0;

        private float _sampling_rate = 1f / 60f;

        private int _previous_gear = 0;
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
          "AccX",
          "AccY",
          "AccZ",
          "SlipAngle",
          "AngularVelocityX",
          "AngularVelocityY",
          "AngularVelocityZ",
          "GearShift",
          "Surge",
          "Sway",
          "YRotation"
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
            {https://www.amazon.ca/gp/product/B08YJPRJVB/ref=ewc_pr_img_2?smid=A2AO8XL38SU3ST&psc=1
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

            SimulatorInterfaceClient simInterface = new SimulatorInterfaceClient("192.168.50.164", SimulatorInterfaceGameType.GT7);
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

            var Q = new System.Numerics.Quaternion(packet.Rotation, packet.RelativeOrientationToNorth);
            var local_velocity = Maths.WorldVelocity_to_LocalVelocity_Quat(Q, packet.Velocity);

            var surge = 0f;
            var sway = 0f;
            var yrot = 0f;


            if (_seenPacket)
            {
                var delta_velocity = local_velocity - _previous_local_velocity;
                


                // divide by sampling rate to get acceleration
                var acc_vector = delta_velocity / _sampling_rate;
                _accel_vector = acc_vector * 9.81f; // convert to m/s^2

                if(local_velocity.Z != 0 && local_velocity.X != 0)
                {
                    slip_angle = Maths.RadianToDegree((float) Math.Atan(local_velocity.X / Math.Abs(local_velocity.Z)));
                }


                (surge, sway, yrot) = CalculateLateralForces(packet.MetersPerSecond, packet.CalculatedMaxSpeed, packet.AngularVelocity.Y);


            }

            _previous_mph = packet.MetersPerSecond;
            _previous_local_velocity = local_velocity;

            // Calculate roll/pitch/yaw based on quaternion
            var (roll, pitch, yaw) = Maths.roll_pitch_yawge(Q);


            _profileManager.SetInput(0, yaw);
            _profileManager.SetInput(1, pitch);
            _profileManager.SetInput(2, roll);
            _profileManager.SetInput(3, _accel_vector.X);
            _profileManager.SetInput(4, _accel_vector.Y);
            _profileManager.SetInput(5, _accel_vector.Z);
            _profileManager.SetInput(6, slip_angle);
            _profileManager.SetInput(7, packet.AngularVelocity.X);
            _profileManager.SetInput(8, packet.AngularVelocity.Y);
            _profileManager.SetInput(9, packet.AngularVelocity.Z);

            _previous_gear = packet.CurrentGear;
            var deltaGear = packet.CurrentGear - _previous_gear;

            _profileManager.SetInput(10, deltaGear);
            _profileManager.SetInput(11, surge);
            _profileManager.SetInput(12, sway);
            _profileManager.SetInput(13, yrot);
            
            
        }

        double radtodeg = 180 / Math.PI;

        double degtorad = 180 / Math.PI;

        public (float surge, float sway, float yrot) CalculateLateralForces(float speed, float speedmax, float angularVelocityY)
        {
            var delta_speed = speed - _previous_mph;

            var surge = delta_speed * 10; // 0 - 1.5 range


            float speedPct = speed / speedmax;   // + (speedmax > 0 ? speedmax : 400);

            var anglePct = (radtodeg * angularVelocityY) / 90.0f;



            //var v = (1 / (1 / 60f)) * 9.8f;

            //var vx = (float)(speed * Math.Cos(angularVelocityY));


            //var vy = (float)(speedPct * 100 * Math.Sin(angularVelocityY));

            //var sway = speedPct * angularVelocityY * 10;


            //var force = 100;

            //var speedWeight = .7f * force;
            //var angleWeight = .3f * force;

            //var aa = anglePct >= .002 ? 1 : 0;

            

            //sway = (float) (anglePct * angleWeight +  speedPct * speedWeight);

            //var speedfactor = speedWeight * speedPct * aa;
            //var anglefactor = anglePct * angleWeight;

            var sway = speedPct * anglePct;

            return (-surge, (float) -sway, (float)anglePct);
        }
    }

    
}
