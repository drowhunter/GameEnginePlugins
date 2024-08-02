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
using System.Linq;
using System.IO;

namespace YawVR_Game_Engine.Plugin
{
    [Export(typeof(Game))]
    [ExportMetadata("Name", "Overload")] // Name that will appear in the plugin list.
    [ExportMetadata("Version", "1.1")]

    public class OverloadPlugin : Game
    {
        public int STEAM_ID => 0;// 448850; // Will start this game on steam based on Steam ID

        public string PROCESS_NAME => string.Empty; //"olmod"; // Put here the exe name (without .exe) monitored by GE to maintain the plugin active.

        public bool PATCH_AVAILABLE => false; // Needs patch

        public string AUTHOR => "PhunkaeG,Drowhunter";

        public Image Logo => Resources.logo;

        public Image SmallLogo => Resources.small;

        public Image Background => Resources.background;

        public string Description => @"<font color=""green"">v1.1.1
    &quot;Usage:<br>
    1. Install OLMOD <a href=""https://olmod.overloadmaps.com/"">click here</a><br>
    2. Install GameMod.dll with telemetry into your overload directory (<a href=""https://github.com/drowhunter/olmod/releases/latest"">click here</a>)<br>
    3. Launch Olmod.exe to start the game ('Telemetry' must appear on the upper right corner of the game's main menu).&quot;;
</font>";


        private Thread readThread;
        private volatile bool running = false;
        private IProfileManager controller;
        private IMainFormDispatcher dispatcher;
        private UdpClient udpClient;
        private IPEndPoint endPoint;

        private static readonly string[] inputs = new string[]
        {
            "pitch","yaw", "roll",
            "sway", "heave", "surge",
            "pitch_speed","yaw_speed", "roll_speed",
            "g_sway", "g_heave", "g_surge",
            "boosting", "primary_fire", "secondary_fire", "picked_up_item", "damage_taken"
        };

        /// <summary>
        /// Text of the inputs that appear in GE's dropdown
        /// </summary>
        /// <returns></returns>
        public string[] GetInputData() => inputs;

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

        private class PlayerData
        {
            /// <summary>
            /// pitch (x), yaw (y), roll (z)
            /// </summary>
            public Vector3 Rotation = Vector3.Zero;

            /// <summary>
            /// pitch (x), yaw (y), roll (z)
            /// </summary>
            public Vector3 AngularVelocity = Vector3.Zero;

            /// <summary>
            /// sway (x), heave (y), surge (z)
            /// </summary>
            public Vector3 GForce = Vector3.Zero;

            /// <summary>
            /// pitch (x), yaw (y), roll (z)
            /// </summary>
            public Vector3 LocalAngularVelocity = Vector3.Zero;

            /// <summary>
            /// sway (x), heave (y), surge (z)
            /// </summary>
            public Vector3 LocalVelocity = Vector3.Zero;

            /// <summary>
            /// sway (x), heave (y), surge (z)
            /// </summary>
            public Vector3 LocalGForce = Vector3.Zero;

            public float EventBoosting;
            public float EventPrimaryFire;
            public float EventSecondaryFire;
            public float EventItemPickup;
            public float EventDamageTaken;

            public static PlayerData Parse(string packetString)
            {
                var playerData = new PlayerData();
                float[] parts = packetString.Split(';').Select(s => float.Parse(s)).ToArray();
                int expectedLength = 23;

                playerData.Rotation = new Vector3(parts[1], parts[2], parts[0]);
                playerData.AngularVelocity = new Vector3(parts[5], parts[4], parts[3]);
                playerData.GForce = new Vector3(parts[6], parts[7], parts[8]);
                playerData.EventBoosting = parts[9];
                playerData.EventPrimaryFire = parts[10];
                playerData.EventSecondaryFire = parts[11];
                playerData.EventItemPickup = parts[12];
                playerData.EventDamageTaken = parts[13];
                playerData.LocalGForce = new Vector3(parts[14], parts[15], parts[16]);
                playerData.LocalAngularVelocity = new Vector3(parts[17], parts[18], parts[19]);
                playerData.LocalVelocity = new Vector3(parts[20], parts[21], parts[22]);

                return playerData;
            }
        };

        private void ProcessTelemetry(string telemetry)
        {
            PlayerData pd = null;
            try
            {
                pd = PlayerData.Parse(telemetry);
                if(pd != null) { 

                    controller.SetInput(0, pd.Rotation.X);
                    controller.SetInput(1, pd.Rotation.Y);
                    controller.SetInput(2, pd.Rotation.Z);

                    controller.SetInput(3, pd.LocalVelocity.X);
                    controller.SetInput(4, pd.LocalVelocity.Y);
                    controller.SetInput(5, pd.LocalVelocity.Z);

                    controller.SetInput(6, pd.LocalAngularVelocity.X);
                    controller.SetInput(7, pd.LocalAngularVelocity.Y);
                    controller.SetInput(8, pd.LocalAngularVelocity.Z);

                    controller.SetInput(9, pd.LocalGForce.X);
                    controller.SetInput(10, pd.LocalGForce.Y);
                    controller.SetInput(11, pd.LocalGForce.Z);

                    controller.SetInput(12, pd.EventBoosting);
                    controller.SetInput(13, pd.EventPrimaryFire);
                    controller.SetInput(14, pd.EventSecondaryFire);
                    controller.SetInput(15, pd.EventItemPickup);
                    controller.SetInput(16, pd.EventDamageTaken);                    
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
            var defProfile = string.Empty;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OverloadPlugin.Profiles.DefaultProfile.yawgeprofile"))
            {
                TextReader tr = new StreamReader(stream);
                defProfile = tr.ReadToEnd();
            }

            var MyComponentsList = new List<Profile_Component>();
            MyComponentsList = dispatcher.JsonToComponents(defProfile);
            return MyComponentsList;
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
