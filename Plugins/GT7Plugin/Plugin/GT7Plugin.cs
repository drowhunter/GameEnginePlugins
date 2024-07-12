// Decompiled with JetBrains decompiler
// Type: YawVRYawVR_Game_Engine.Plugin.GT7Plugin
// Assembly: GT7, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4722EE52-F477-46B0-8A58-A26AC0A5A193
// Assembly location: G:\apps\GameEngine\Gameplugins\GT7Plugin.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

using GT7.Properties;

using PDTools.SimulatorInterface;

using YawGEAPI;

namespace YawVRYawVR_Game_Engine.Plugin
{
    [Export(typeof(Game))]
    [ExportMetadata("Name", "Gran Turismo 7")]
    [ExportMetadata("Version", "1.0")]
    public class GT7Plugin : Game
    {
        private volatile bool _running = false;
        private Thread _readthread;
        
       
        private IMainFormDispatcher _dispatcher;
        private IProfileManager _controller;

        private CancellationTokenSource _cts;
        private SimulatorInterfaceClient _simInterface;


        private const float _toDegree = 57.295f;
        public int STEAM_ID => 241560;

        public bool PATCH_AVAILABLE => true;

        public string AUTHOR => "Drowhunter";

        public string PROCESS_NAME => "";

        public string Description => Resources.description;

        public Image Logo => (Image)Resources.logo;

        public Image SmallLogo => (Image)Resources.small;

        public Image Background => (Image)Resources.background;

        public LedEffect DefaultLED() => new LedEffect((EFFECT_TYPE)1, 7, new YawColor[4]
        {
              new YawColor((byte) 66, (byte) 135, (byte) 245),
              new YawColor((byte) 80, (byte) 80, (byte) 80),
              new YawColor((byte) 128, (byte) 3, (byte) 117),
              new YawColor((byte) 110, (byte) 201, (byte) 12)
        }, 25f);

        public List<Profile_Component> DefaultProfile()
        {
            List<Profile_Component> profileComponentList = new List<Profile_Component>();
            profileComponentList.Add(new Profile_Component(4, 1, 1f, 0.0f, 0.0f, false, false, -1f, 1f, true, (ObservableCollection<ProfileMath>)null, (ProfileSpikeflatter)null, 0.0f, (ProfileComponentType)0, (ObservableCollection<ProfileCondition>)null));
            profileComponentList.Add(new Profile_Component(7, 1, 0.5f, 0.0f, 0.0f, false, true, -1f, 0.3f, true, (ObservableCollection<ProfileMath>)null, (ProfileSpikeflatter)null, 0.0f, (ProfileComponentType)0, (ObservableCollection<ProfileCondition>)null));
            profileComponentList.Add(new Profile_Component(1, 2, 0.1f, 0.0f, 0.0f, false, false, -1f, 0.05f, true, (ObservableCollection<ProfileMath>)null, (ProfileSpikeflatter)null, 0.0f, (ProfileComponentType)0, (ObservableCollection<ProfileCondition>)null));
            profileComponentList.Add(new Profile_Component(5, 2, 1f, 0.0f, 0.0f, false, true, -1f, 1f, true, (ObservableCollection<ProfileMath>)null, (ProfileSpikeflatter)null, 0.0f, (ProfileComponentType)0, (ObservableCollection<ProfileCondition>)null));
            profileComponentList.Add(new Profile_Component(0, 1, 0.1f, 0.0f, 0.0f, false, false, -1f, 1f, true, (ObservableCollection<ProfileMath>)null, (ProfileSpikeflatter)null, 0.0f, (ProfileComponentType)0, (ObservableCollection<ProfileCondition>)null));
            profileComponentList.Add(new Profile_Component(3, 0, 1f, 0.0f, 0.0f, false, true, -1f, 1f, true, (ObservableCollection<ProfileMath>)null, (ProfileSpikeflatter)null, 0.0f, (ProfileComponentType)0, (ObservableCollection<ProfileCondition>)null));
            return profileComponentList;
        }

        public void Exit()
        {
            _cts.Cancel();
            _simInterface.Dispose();
            
            this._running = false;
        }

        

        public string[] GetInputData() => new string[12]
        {
          "AngularVelocityP", "AngularVelocityR", "AngularVelocityY",
          "OrientationYaw", "OrientationPitch", "OrientationRoll",
          //"Acceleration_Lateral", "Acceleration_longitudinal", "Acceleration_vertical",
          "VelocityX", "VelocityY", "VelocityZ",
          "PositionX", "PositionY", "PositionZ"
        };

        public void SetReferences(IProfileManager controller, IMainFormDispatcher dispatcher)
        {
            this._dispatcher = dispatcher;
            this._controller = controller;
        }

        public void Init()
        {
            this._running = true;

            
            _simInterface = new SimulatorInterfaceClient("192.168.50.226", SimulatorInterfaceGameType.GT7);
            _simInterface.OnReceive += SimInterface_OnReceive;


            _cts = new CancellationTokenSource();

            // Cancel token from outside source to end simulator

            

            
            
            
            //this._readthread = new Thread(new ThreadStart(this.ReadFunction));
            //this._readthread.Start();
        }
        
        private void SimInterface_OnReceive(SimulatorPacket packet)
        {
            // Print the packet contents to the console
            Console.SetCursorPosition(0, 0);
            packet.PrintPacket(_running);

            // Get the game type the packet was issued from
            SimulatorInterfaceGameType gameType = packet.GameType;

            // Check on flags for whether the simulation is active
            if (packet.Flags.HasFlag(SimulatorFlags.CarOnTrack) && !packet.Flags.HasFlag(SimulatorFlags.Paused) && !packet.Flags.HasFlag(SimulatorFlags.LoadingOrProcessing))
            {
                // Do stuff with packet
                

                _controller.SetInput(0, packet.AngularVelocity.X * _toDegree);
                _controller.SetInput(1, packet.AngularVelocity.Y * _toDegree);
                _controller.SetInput(2, packet.AngularVelocity.Z * _toDegree);

                _controller.SetInput(3, packet.Rotation.X * _toDegree);
                _controller.SetInput(4, packet.Rotation.Y * _toDegree);
                _controller.SetInput(5, packet.Rotation.Z * _toDegree);

                //_controller.SetInput(6, packet..X);
                //_controller.SetInput(7, packet.Velocity.Y);
                //_controller.SetInput(8, packet.Velocity.Z);

                _controller.SetInput(6, packet.Velocity.X);
                _controller.SetInput(7, packet.Velocity.Y);
                _controller.SetInput(8, packet.Velocity.Z);

                _controller.SetInput(09, packet.Position.X);
                _controller.SetInput(10, packet.Position.Y);
                _controller.SetInput(11, packet.Position.Z);

            }
        }

        

        public void PatchGame()
        {
            
        }

        public Dictionary<string, ParameterInfo[]> GetFeatures() => (Dictionary<string, ParameterInfo[]>)null;
    }
}
