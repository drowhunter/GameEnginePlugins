using OverloadPlugin.Properties;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using Quaternion = System.Numerics.Quaternion;
using YawGEAPI;
using PluginHelper;

namespace YawVR_Game_Engine.Plugin
{
    [Export(typeof(Game))]
    [ExportMetadata("Name", "Overload")] // Name that will appear in the plugin list.
    [ExportMetadata("Version", "1.0")]

    public class OverloadPlugin : Game
    {
        public int STEAM_ID => 0;// 448850; // Will start this game on steam based on Steam ID

        public string PROCESS_NAME => "olmod"; // Put here the exe name (without .exe) monitored by GE to maintain the plugin active.

        public bool PATCH_AVAILABLE => false; // Needs patch

        public string AUTHOR => "PhunkaeG";

        public Image Logo => Resources.logo;

        public Image SmallLogo => Resources.small;

        public Image Background => Resources.background;

        public string Description => "Usage:<br>1. Install OLMOD (https://olmod.overloadmaps.com/)<br>2. Install gamemod.dll with telemetry (https://github.com/overload-development-community/olmod/issues/323)<br>3. Launch Olmod.exe to start the game ('Telemetry' must appear on the upper right corner of the game's main menu)."; // No title here, the name of the plugin is added automatically.

        private Thread readThread;
        private volatile bool running = false;
        private IProfileManager controller;
        private IMainFormDispatcher dispatcher;
        private UdpClient udpClient;
        private IPEndPoint endPoint;

        private void ReadTelemetry()
        {
            try
            {
                while (running)
                {
                    if (udpClient.Available > 0)
                    {
                        byte[] data = udpClient.Receive(ref endPoint);
                        string telemetryData = Encoding.ASCII.GetString(data);
                        ProcessTelemetry(telemetryData);
                    }
                    // Thread.Sleep(20); // Reduce CPU usage - NB: Overloads sends UDP packets much too fast !!!
                }
            }
            catch (Exception ex)
            {
                // Handle or log exceptions
                Console.WriteLine("Error reading telemetry data: " + ex.Message);
            }
        }
        private void ProcessTelemetry(string telemetry)
        {
            try
            {
                string[] parts = telemetry.Split(';');
                if (parts.Length >= 14) // Make sure all parts are present
                {
                    float roll = float.Parse(parts[0]);
                    float pitch = float.Parse(parts[1]);
                    float yaw = float.Parse(parts[2]);
                    float AngularVelocityZ = float.Parse(parts[3]);
                    float AngularVelocityX = float.Parse(parts[4]);
                    float AngularVelocityY = float.Parse(parts[5]);
                    float gForceX = float.Parse(parts[6]);
                    float gForceY = float.Parse(parts[7]);
                    float gForceZ = float.Parse(parts[8]);

                    


                    var qrot = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
                    ///var ypr = qrot.ToEuler();

                    var gforce = new Vector3(gForceX, gForceY, gForceZ);
                    var angV = Maths.EulerToDirection(AngularVelocityX, AngularVelocityY, AngularVelocityZ, false);

                    var local_av = Maths.rotate_vector_by_quaternion(Quaternion.Conjugate(qrot), angV);

                    //var qn = Quaternion.Normalize(q);
                    //var local_gforce = (Quaternion.Conjugate(qn) * new Quaternion(gforce, 0)).Vector();
                    var local_gforce = Maths.rotate_vector_by_quaternion(Quaternion.Conjugate(qrot), gforce);

                    

                    var datapoints = new List<float>()
                    {
                        yaw, pitch, roll,
                        AngularVelocityX, AngularVelocityY, AngularVelocityZ,
                        gForceX, gForceY, gForceZ,
                        0, 0, 0, 0, 0,
                        local_gforce.X, local_gforce.Y, local_gforce.Z,
                        local_av.X, local_av.Y, local_av.Z
                    };

                    for (int i = 0; i < datapoints.Count; i++)
                    {
                        float dp = datapoints[i];
                        controller.SetInput(i, dp);
                    }                   
                   

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing telemetry data: {ex.Message}");
            }
        }

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
            return new List<Profile_Component>();
        }

        public void Exit()
        {
            running = false;
            readThread.Join();
            udpClient.Close();
        }
        public Dictionary<string, ParameterInfo[]> GetFeatures()
        {
            return new Dictionary<string, ParameterInfo[]>(); // Return empty if no features to report
        }

        public string[] GetInputData()
        {
            return new string[] { 
                "Yaw", "Pitch", "Roll", 
                "AngularVelocityX", "AngularVelocityY", "AngularVelocityZ", 
                "gForceX", "gForceY", "gForceZ",
                "boosting", "primary_fire", "secondary_fire", "picked_up_item", "damage_taken",
                "LocalGX", "LocalGY", "LocalGZ",
                "LocalAVX", "LocalAVY", "LocalAVZ"
            }; // Text of the inputs that appear in GE's dropdown

        }

        public void Init()
        {
            if (udpClient != null)
                udpClient.Close();

            if (readThread != null && readThread.IsAlive)
                Exit();

            endPoint = new IPEndPoint(IPAddress.Any, 4123); // Use the correct port for Overload
            udpClient = new UdpClient(endPoint);

            running = true;
            readThread = new Thread(ReadTelemetry);
            readThread.Start();
        }

        public void PatchGame()
        {
            // should add code to download the olmod and extract it to the game folder
            // just like TheCrew2 plugin does
        }

        public void SetReferences(IProfileManager controller, IMainFormDispatcher dispatcher)
        {
            //we need to save these references
            this.controller = controller;
            this.dispatcher = dispatcher;
        }
    }
}
