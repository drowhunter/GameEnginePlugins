using FormHelper;
using FormHelper.Storage;

using PluginHelper;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using TDUSCPlugin;
using TDUSCPlugin.Properties;

using YawGEAPI;


namespace YawVR_Game_Engine.Plugin
{
    [Export(typeof(Game))]
    [ExportMetadata("Name", "Test Drive Unlimited Solar Crown")]
    [ExportMetadata("Version", "0.4")]
    public class TDUSCPlugin : Game, IDisposable
    {
        Stopwatch stopwatch = new Stopwatch();

        private const string FORWARDING_ENABLED = "forwardingEnabled";
        private const string FORWARDING_PORT = "forwardingPort";
        private const string INCOMING_PORT = "incomingPort";
        private UserSettingsManager<RegistryStorage> _settings;

        private Thread readThread;
        private IPEndPoint remoteIP;
        

        private CancellationTokenSource cancellationTokenSource;

        long packetid = 0;
        private int gameport;

        bool testing = false;
        private UdpClient socket;
        private UdpClient forwardingSocket;
        private bool _clientConnected = false;

        private const float _sample_rate = 1 / 60f;
        private const float RAD_2_DEG = 57.29578f;
        private TDUSCPacket _lastpacket;


        private Vector3 _previous_local_velocity;

        /// <summary>
        /// used to compute angular velocity
        /// </summary>
        Vector3 _lastRotation = Vector3.Zero;

        public TDUSCPlugin()
        {
            _settings = new UserSettingsManager<RegistryStorage>(this.GetType().Name);
            cancellationTokenSource = new CancellationTokenSource();

        }


        public TDUSCPlugin(IProfileManager controller, IMainFormDispatcher dispatcher)
        {
            this.controller = controller;
            this.dispatcher = dispatcher;
        }


        private bool _isStarted = false;
        private bool isStarted
        {
            get
            {
                return _isStarted;
            }
            set
            {
                if (_isStarted != value)
                {
                    _isStarted = value;
                    if (value)
                    {
                        Log("S");
                    }
                    else
                    {
                        Log("s");
                    }
                }
            }
        }

        private bool _isRunning = false;
        private bool isRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    if (value)
                    {
                        stopwatch.Stop();
                        Log("r");
                    }
                    else
                    {
                        stopwatch.Restart();
                        Log("X");
                        
                    }
                }
                
                    
            }
        }

        void Log(string message)
        {
            Console.Write(message);
        }

        void LogLine(string message)
        {
            Console.WriteLine(message);
        }

        public async void Init()
        {
            bool error = false;

            await PromptUser().ConfigureAwait(false);

            

            gameport = _settings.Get<int>(INCOMING_PORT);

            


            do
            {
                try
                {
                    

                    socket = new UdpClient(gameport);
                    socket.Client.ReceiveTimeout = 60000;
                    var thread = new Thread(ReadFunction);
                    thread.Name = "TDUSC Read Thread";
                    thread.Start();

                    isStarted = true;
                    error = false;
                }
                catch (SocketException ex)
                {
                    error = true;

                    Log("!");
                }
                catch (Exception ex)
                {
                    Log(ex.Message + Environment.NewLine);
                    isStarted = false;
                    return;
                }
            } while (error);
        }

        


        private async Task StartForwardingIfRequired(CancellationToken cancellationToken = default)
        {
            if (_settings.Get<bool>(FORWARDING_ENABLED))
            {
                forwardingSocket = new UdpClient(_settings.Get<int>(FORWARDING_PORT));

                var listenTask = await forwardingSocket.ReceiveAsync().WithCancellation(cancellationToken);
                _clientConnected = true;

                Log(Environment.NewLine + "A Client has connected = " + Environment.NewLine);
                

            }
        }
        public async Task PromptUser(CancellationToken cancellationToken = default)
        {
            await _settings.LoadAsync(defaultSettings, cancellationToken);

            await _settings.ShowFormAsync(cancellationToken: cancellationToken);

            

            
        }

        public async void ReadFunction()
        {
            int fwdPort = _settings.Get<int>(FORWARDING_PORT);
            while (isStarted)
            {
                try
                {
                    if(_lastpacket == null)
                    {
                        Log("s");
                    }
                    

                    var buffer = socket.Receive(ref remoteIP);

                    if (buffer.Length >= 280)
                    {
                        if (isRunning == false)
                        {
                            Log("R");
                            isRunning = true;
                            socket.Client.ReceiveTimeout = 900; // must be less than 1000 for GE to not to reset
                        }

                        if (_clientConnected && forwardingSocket != null)
                        {
                            var t = forwardingSocket.SendAsync(buffer, fwdPort).ConfigureAwait(false);
                            Log("f");

                        }

                        _lastpacket = new TDUSCPacket(buffer);
                        SetInput(isRunning);    
                    }
                    else
                    {
                        Log("!");
                        isRunning = false;
                        
                    }
                }
                catch (SocketException ex)
                {
                    if (stopwatch.Elapsed < TimeSpan.FromMinutes(10))
                    {
                        if (isRunning)
                        {
                            isRunning = false;
                        }

                        if (_lastpacket != null)
                        {

                            SetInput(isRunning);
                        }
                    }
                    else {
                        Log(Environment.NewLine + "Q");
                        dispatcher.ShowNotification(NotificationType.ERROR, "Connection to the game has been lost.");
                        isStarted = false;
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.Message+Environment.NewLine);
                    isStarted = false;
                    isRunning= false;
                }
                
            }
        }

        public float CalculateCentrifugalAcceleration(Vector3 velocity, Vector3 angularVelocity)
        {
            var Fc = velocity.Length() * angularVelocity.Length();

            return Fc * (angularVelocity.Y >= 0 ? 1 : -1);

        }

        

        public string[] GetInputData()
        {
            return new string[26]
            {
                "Speed",
                "RPM",
                "Steer",
                "Force_long",
                "Force_lat",
                "Pitch",
                "Roll",
                "Yaw",
                "suspen_pos_bl",
                "suspen_pos_br",
                "suspen_pos_fl",
                "suspen_pos_fr",
                "suspen_vel_bl",
                "suspen_vel_br",
                "suspen_vel_fl",
                "suspen_vel_fr",
                "VelocityX",
                "VelocityY",
                "VelocityZ",
                "IsAlive",
                "Surge",
                "Sway",
                "AccY",
                "AngularVelocityX",
                "AngularVelocityY",
                "AngularVelocityZ",
            };
        }
        public void SetInput(bool isAlive)
        {
            var packet = _lastpacket;

            var worldVelocity = new Vector3(packet.VelocityX, packet.VelocityY, packet.VelocityZ);

            var world_rollV = new Vector3(packet.RollX, packet.RollY, packet.RollZ);
            var world_pitchV = new Vector3(packet.PitchX, packet.PitchY, packet.PitchZ);

            Vector3 world_yawV = world_rollV.Cross(world_pitchV);

            var Q = System.Numerics.Quaternion.CreateFromRotationMatrix(
                new Matrix4x4(
                    world_rollV.X, world_pitchV.X, world_yawV.X, 0.0f, 
                    world_rollV.Y, world_pitchV.Y, world_yawV.Y, 0.0f, 
                    world_rollV.Z, world_pitchV.Z, world_yawV.Z, 0.0f, 
                    0.0f, 0.0f, 0.0f, 1f)
                );
            

            (float pitch, float yaw, float roll) = Q.ToEuler(false);
            

            Vector3 local_velocity = Maths.WorldtoLocal(Q, worldVelocity);

            float sway = 0.0f;
            float surge = 0.0f;
            float heave = 0.0f;


            Vector3 angular_velocity = Vector3.Zero;
            Vector3 rotation = new Vector3(pitch, yaw, roll);
            if (_lastRotation != null)
            {
                angular_velocity = (rotation - _lastRotation) * (Maths.GRAVITY /_sample_rate);
                sway = CalculateCentrifugalAcceleration(local_velocity, angular_velocity);

                _lastRotation = rotation;
            }

            Vector3 previousLocalVelocity = _previous_local_velocity;

            if (_previous_local_velocity != null)
            {
                Vector3 delta_velocity = local_velocity - _previous_local_velocity;

                surge = delta_velocity.Z / _sample_rate / 9.81f;
                
                heave = delta_velocity.Y / _sample_rate / 9.81f;
            }

            if (!testing)
            {
                this.controller.SetInput(0, packet.Speed);
                this.controller.SetInput(1, packet.RPM);  // not provided yet
                this.controller.SetInput(2, packet.Steer);
                this.controller.SetInput(3, packet.Gforce_lon);
                this.controller.SetInput(4, packet.Gforce_lat);
                this.controller.SetInput(5, -pitch * RAD_2_DEG);
                this.controller.SetInput(6, yaw * RAD_2_DEG);
                this.controller.SetInput(7, -roll * RAD_2_DEG);
                this.controller.SetInput(8, packet.Susp_pos_bl);
                this.controller.SetInput(9, packet.Susp_pos_br);
                this.controller.SetInput(10, packet.Susp_pos_fl);
                this.controller.SetInput(11, packet.Susp_pos_fr);
                this.controller.SetInput(12, packet.Susp_vel_bl);
                this.controller.SetInput(13, packet.Susp_vel_br);
                this.controller.SetInput(14, packet.Susp_vel_fl);
                this.controller.SetInput(15, packet.Susp_vel_fr);
                this.controller.SetInput(16, local_velocity.X);
                this.controller.SetInput(17, local_velocity.Y);
                this.controller.SetInput(18, local_velocity.Z);
                this.controller.SetInput(19, isAlive ? 1f : 0.0f);
                this.controller.SetInput(20, surge);
                this.controller.SetInput(21, sway);
                this.controller.SetInput(21, heave);
                this.controller.SetInput(21, angular_velocity.X);
                this.controller.SetInput(21, angular_velocity.Y);
                this.controller.SetInput(21, angular_velocity.Z);
            }

            Log(isRunning ? "." : "x");
            _previous_local_velocity = local_velocity;
        }

        

        

        public List<Profile_Component> DefaultProfile()
        {
            string json = string.Empty;
            using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TDUSCPlugin.Profiles.DefaultProfile.yawgeprofile"))
                json = new StreamReader(manifestResourceStream).ReadToEnd();
            List<Profile_Component> profileComponentList = new List<Profile_Component>();
            return dispatcher.JsonToComponents(json);
        }

        public Dictionary<string, ParameterInfo[]> GetFeatures()
        {
            return null;
        }

        public LedEffect DefaultLED()
        {
            return new LedEffect(EFFECT_TYPE.FLOW_LEFTRIGHT, 2, new YawColor[4]
            {
                new YawColor(byte.MaxValue,  40,  0),
                new YawColor( 80,  80,  80),
                new YawColor(byte.MaxValue,  100,  0),
                new YawColor( 140,  0, byte.MaxValue)
            }, -20f);
        }

        public void PatchGame()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\My Games\Test Drive Unlimited Solar Crown\UserSettings.cfg";
            if (File.Exists(path))
            {
                string input = File.ReadAllText(path);
                string contents = Regex.Replace(input, @"(?<=Rc2\.Telemetry\.EnableTelemetry\s*?=\s*?)false", "true", RegexOptions.Singleline);
                if (!(contents != input))
                    return;
                File.WriteAllText(path, contents);
                dispatcher.ShowNotification(NotificationType.INFO, path + " patched!");
            }
            else
                dispatcher.DialogShow("Could not find UserSetting.cfg, Make sure you run the game at least once first", DIALOG_TYPE.INFO);
        }

        public void Dispose()
        {
            try
            {
                socket.Close();
            }
            finally
            {
                socket = null;                
            }
        }

        List<UserSetting> defaultSettings = new List<UserSetting>
        {
            new UserSetting
            {
                DisplayName = "Udp Forwarding",
                Name = FORWARDING_ENABLED,
                Description = "Enable UDP Forwarding",
                SettingType = SettingType.Bool,
                Value = false
            },
            new UserSetting
            {
                DisplayName = $"Udp Forwarding Port",
                Name = FORWARDING_PORT,
                Description = "Port to forward UDP packets to.",
                SettingType = SettingType.NetworkPort,
                Value = 20771,
                ValidationEnabled = true,
                ValidationEnabledWhen = new Dictionary<string, string> { { "forwardingEnabled", "true" } },
                EnabledWhen = new Dictionary<string, string> { { "forwardingEnabled", "true" } }

            },
            new UserSetting
            {
                DisplayName = $"Incoming Data Port",
                Name = INCOMING_PORT,
                Description = "The default port for incoming data from the console. (Default: 33740)",
                SettingType = SettingType.NetworkPort,
                Value = 20777,
                ValidationEnabled = true

            }
        };

        #region Game Implementation Properties

        
        public string PROCESS_NAME => "TDUSC";
        public int STEAM_ID => 1249970;
        public string AUTHOR => "Drowhunter";
        public bool PATCH_AVAILABLE => true;
        public Image Logo => Resources.logo;
        public Image SmallLogo => Resources.small;
        public Image Background => Resources.background;
        public string Description => ResourceHelper.LoadEmbeddedResourceString("description.html");

        private IProfileManager controller;
        private IMainFormDispatcher dispatcher;

        public void Exit()
        {
            isStarted = false;
            isRunning = false;
            Dispose();
        }

        public void SetReferences(IProfileManager controller, IMainFormDispatcher dispatcher)
        {
            this.controller = controller;
            this.dispatcher = dispatcher;
        }
        #endregion
    }
}
