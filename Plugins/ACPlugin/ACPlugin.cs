// Decompiled with JetBrains decompiler
// Type: YawVR_Game_Engine.Plugin.ACPlugin
// Assembly: ACPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4B94A2DB-8D35-4A0B-BF8B-FF6A3CB7A173
// Assembly location: E:\_Apps\GameEngine_2_6\Gameplugins\ACPlugin.dll

using ACPlugin.Properties;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using YawGEAPI;


namespace YawVR_Game_Engine.Plugin
{
  [Export(typeof (Game))]
  [ExportMetadata("Name", "Assetto Corsa")]
  [ExportMetadata("Version", "1.2")]
  internal class ACPlugin : Game
  {
    private IProfileManager controller;
    private IMainFormDispatcher dispatcher;
    private UdpClient udpClient;
    private IPEndPoint remoteIP = new IPEndPoint(IPAddress.Loopback, 9996);
    private Thread readThread;
    private bool running = false;
    private string input = "127.0.0.1";

    public string PROCESS_NAME => string.Empty;

    public int STEAM_ID => 244210;

    public string AUTHOR => "YawVR";

    public bool PATCH_AVAILABLE => false;

    public Image Logo => (Image) Resources.logo;

    public Image SmallLogo => (Image) Resources.small;

    public Image Background => (Image) Resources.background;

    public string Description => string.Empty;

    public void PatchGame()
    {
    }

    public void Exit()
    {
      this.udpClient.Close();
      this.udpClient = (UdpClient) null;
      this.running = false;
    }

    public void SetReferences(IProfileManager controller, IMainFormDispatcher dispatcher)
    {
      this.controller = controller;
      this.dispatcher = dispatcher;
    }

    public void Init()
    {
      this.input = Interaction.InputBox("Enter the host address of Assetto Corsa\nLeave default value if its running on this PC", "Endpoint", "127.0.0.1");
      this.HandShake();
      this.running = true;
      this.readThread = new Thread(new ThreadStart(this.ReadFunction));
      this.readThread.Start();
    }

    private void ReadFunction()
    {
      try
      {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 9996);
        while (this.running)
        {
          try
          {
            byte[] numArray = this.udpClient.Receive(ref remoteEP);
            float single1 = BitConverter.ToSingle(numArray, 8);
            float single2 = BitConverter.ToSingle(numArray, 68);
            float num = (float) ((double) BitConverter.ToSingle(numArray, 72) / 41.0 * ((double) single1 / 100.0));
            float single3 = BitConverter.ToSingle(numArray, 28);
            float single4 = BitConverter.ToSingle(numArray, 32);
            float single5 = BitConverter.ToSingle(numArray, 36);
            this.controller.SetInput(0, single1);
            this.controller.SetInput(1, single2);
            this.controller.SetInput(2, num);
            this.controller.SetInput(3, single3);
            this.controller.SetInput(4, single4);
            this.controller.SetInput(5, single5);
          }
          catch (SocketException ex)
          {
            for (int index = 0; index < this.GetInputData().Length; ++index)
              this.controller.SetInput(index, 0.0f);
            Thread.Sleep(1000);
            this.HandShake();
          }
        }
      }
      catch (ThreadAbortException ex)
      {
      }
    }

    private void HandShake()
    {
      this.udpClient = new UdpClient();
      this.udpClient.Client.ReceiveTimeout = 1000;
      this.udpClient.Connect(this.input, 9996);
      byte[] dgram1 = new YawVR_Game_Engine.Plugin.ACPlugin.Handshaker(1, 1, 0).toByte();
      Thread.Sleep(250);
      this.udpClient.Send(dgram1, dgram1.Length);
      byte[] dgram2 = new YawVR_Game_Engine.Plugin.ACPlugin.Handshaker(1, 1, 1).toByte();
      this.udpClient.Send(dgram2, dgram2.Length);
    }

    public string[] GetInputData()
    {
      return new string[6]
      {
        "Speed",
        "RPM",
        "Steer",
        "Acceleration_vert",
        "Acceleration_horiz",
        "Acceleration_lon"
      };
    }

    public LedEffect DefaultLED()
    {
      return new LedEffect(EFFECT_TYPE.FLOW_LEFTRIGHT, 2, new YawColor[4]
      {
        new YawColor(byte.MaxValue, (byte) 40, (byte) 0),
        new YawColor((byte) 80, (byte) 80, (byte) 80),
        new YawColor(byte.MaxValue, (byte) 100, (byte) 0),
        new YawColor((byte) 140, (byte) 0, byte.MaxValue)
      }, -20f);
    }

    public List<Profile_Component> DefaultProfile()
    {
      return this.dispatcher.JsonToComponents(Resources.defProfile);
    }

    public Dictionary<string, ParameterInfo[]> GetFeatures()
    {
      return (Dictionary<string, ParameterInfo[]>) null;
    }

    private struct Handshaker
    {
      private int identifier;
      private int version;
      private int operationId;

      public Handshaker(int identifier, int version, int operationId)
      {
        this.identifier = identifier;
        this.version = version;
        this.operationId = operationId;
      }

      public byte[] toByte()
      {
        List<byte> byteList = new List<byte>();
        byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(this.identifier));
        byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(this.version));
        byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(this.operationId));
        return byteList.ToArray();
      }
    }
  }
}
