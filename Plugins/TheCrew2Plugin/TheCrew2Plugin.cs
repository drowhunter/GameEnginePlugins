// Decompiled with JetBrains decompiler
// Type: YawVRYawVR_Game_Engine.Plugin.TheCrew2Plugin
// Assembly: TheCrew2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 57EA1049-7C99-4299-9B7E-8E15049A09F6
// Assembly location: C:\apps\yawge\Gameplugins\TheCrew2Plugin.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using TheCrew2.Properties;
using YawGEAPI;
using Syroot.BinaryData.Memory;


namespace YawVRYawVR_Game_Engine.Plugin
{
  [Export(typeof (Game))]
  [ExportMetadata("Name", "The Crew 2")]
  [ExportMetadata("Version", "1.0")]
  public class TheCrew2Plugin : Game
  {
    private volatile bool running = false;
    private Thread readthread;
    private UdpClient receivingUdpClient;
    private IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
    private IMainFormDispatcher dispatcher;
    private IProfileManager controller;

    public int STEAM_ID => 646910;

    public bool PATCH_AVAILABLE => true;

    public string AUTHOR => "YawVR";

    public string PROCESS_NAME => "";

    public string Description => Resources.description;

    public Image Logo => (Image) Resources.logo;

    public Image SmallLogo => (Image) Resources.small;

    public Image Background => (Image) Resources.background;

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
      this.receivingUdpClient.Close();
      this.receivingUdpClient = (UdpClient) null;
      this.running = false;
    }

    public string[] GetInputData()
    {
      return new string[15]
      {
        "AngularVelocityP",
        "AngularVelocityR",
        "AngularVelocityY",
        "OrientationYaw",
        "OrientationPitch",
        "OrientationRoll",
        "Acceleration_Lateral",
        "Acceleration_longitudinal",
        "Acceleration_vertical",
        "VelocityX",
        "VelocityY",
        "VelocityZ",
        "PositionX",
        "PositionY",
        "PositionZ"
      };
    }

    public void SetReferences(IProfileManager controller, IMainFormDispatcher dispatcher)
    {
      this.dispatcher = dispatcher;
      this.controller = controller;
    }

    public void Init()
    {
      this.running = true;
      this.receivingUdpClient = new UdpClient(1337);
      this.readthread = new Thread(new ThreadStart(this.ReadFunction));
      this.readthread.Start();
    }

    private void ReadFunction()
    {
      while (this.running)
      {
        var numArray = this.receivingUdpClient.Receive(ref this.RemoteIpEndPoint);

        var sr = new SpanReader(numArray);

        var data = new float[GetInputData().Length];

        for (int i = 0; i < data.Length; i++)
        {
          data[i] = sr.ReadSingle();
          this.controller.SetInput(i, RadToDeg(data[i]));
        }

      }
    }

    private float RadToDeg(float rad)
    {
      return (float) (rad * (180.0 / Math.PI));
    }

    public void PatchGame()
    {
      try
      {
        string str = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/The Crew 2";
        Console.WriteLine(str);
        string tempFileName = Path.GetTempFileName();
        this.dispatcher.GetInstallPath(str);
        using (WebClient webClient = new WebClient())
        {
          webClient.DownloadFile("http://yaw.one/gameengine/Plugins/The_Crew_2/TheCrew2.zip", tempFileName);
          Console.WriteLine(str);
          this.dispatcher.ExtractToDirectory(tempFileName, str, true);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("{0}", (object) ex.ToString());
      }
    }

    public Dictionary<string, ParameterInfo[]> GetFeatures()
    {
      return (Dictionary<string, ParameterInfo[]>) null;
    }
  }
}
