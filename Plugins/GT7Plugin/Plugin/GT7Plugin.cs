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
using YawGEAPI;

namespace YawVRYawVR_Game_Engine.Plugin
{
  [Export(typeof (Game))]
  [ExportMetadata("Name", "Gran Turismo 7")]
  [ExportMetadata("Version", "1.0")]
  public class GT7Plugin : Game
  {
    private volatile bool running = false;
    private Thread readthread;
    private UdpClient receivingUdpClient;
    private IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
    private IMainFormDispatcher dispatcher;
    private IProfileManager controller;

    public int STEAM_ID => 241560;

    public bool PATCH_AVAILABLE => true;

    public string AUTHOR => "YawVR";

    public string PROCESS_NAME => "";

    public string Description => Resources.description;

    public Image Logo => (Image) Resources.logo;

    public Image SmallLogo => (Image) Resources.small;

    public Image Background => (Image) Resources.background;

    public LedEffect DefaultLED() => new LedEffect((EFFECT_TYPE) 1, 7, new YawColor[4]
    {
      new YawColor((byte) 66, (byte) 135, (byte) 245),
      new YawColor((byte) 80, (byte) 80, (byte) 80),
      new YawColor((byte) 128, (byte) 3, (byte) 117),
      new YawColor((byte) 110, (byte) 201, (byte) 12)
    }, 25f);

    public List<Profile_Component> DefaultProfile()
    {
      List<Profile_Component> profileComponentList = new List<Profile_Component>();
      profileComponentList.Add(new Profile_Component(4, 1, 1f, 0.0f, 0.0f, false, false, -1f, 1f, true, (ObservableCollection<ProfileMath>) null, (ProfileSpikeflatter) null, 0.0f, (ProfileComponentType) 0, (ObservableCollection<ProfileCondition>) null));
      profileComponentList.Add(new Profile_Component(7, 1, 0.5f, 0.0f, 0.0f, false, true, -1f, 0.3f, true, (ObservableCollection<ProfileMath>) null, (ProfileSpikeflatter) null, 0.0f, (ProfileComponentType) 0, (ObservableCollection<ProfileCondition>) null));
      profileComponentList.Add(new Profile_Component(1, 2, 0.1f, 0.0f, 0.0f, false, false, -1f, 0.05f, true, (ObservableCollection<ProfileMath>) null, (ProfileSpikeflatter) null, 0.0f, (ProfileComponentType) 0, (ObservableCollection<ProfileCondition>) null));
      profileComponentList.Add(new Profile_Component(5, 2, 1f, 0.0f, 0.0f, false, true, -1f, 1f, true, (ObservableCollection<ProfileMath>) null, (ProfileSpikeflatter) null, 0.0f, (ProfileComponentType) 0, (ObservableCollection<ProfileCondition>) null));
      profileComponentList.Add(new Profile_Component(0, 1, 0.1f, 0.0f, 0.0f, false, false, -1f, 1f, true, (ObservableCollection<ProfileMath>) null, (ProfileSpikeflatter) null, 0.0f, (ProfileComponentType) 0, (ObservableCollection<ProfileCondition>) null));
      profileComponentList.Add(new Profile_Component(3, 0, 1f, 0.0f, 0.0f, false, true, -1f, 1f, true, (ObservableCollection<ProfileMath>) null, (ProfileSpikeflatter) null, 0.0f, (ProfileComponentType) 0, (ObservableCollection<ProfileCondition>) null));
      return profileComponentList;
    }

    public void Exit()
    {
      this.receivingUdpClient.Close();
      this.receivingUdpClient = (UdpClient) null;
      this.running = false;
    }

    public string[] GetInputData() => new string[15]
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
        byte[] numArray = this.receivingUdpClient.Receive(ref this.RemoteIpEndPoint);
        float single1 = BitConverter.ToSingle(numArray, 4);
        float single2 = BitConverter.ToSingle(numArray, 8);
        float single3 = BitConverter.ToSingle(numArray, 12);
        float single4 = BitConverter.ToSingle(numArray, 16);
        float single5 = BitConverter.ToSingle(numArray, 20);
        float single6 = BitConverter.ToSingle(numArray, 24);
        float single7 = BitConverter.ToSingle(numArray, 28);
        float single8 = BitConverter.ToSingle(numArray, 32);
        float single9 = BitConverter.ToSingle(numArray, 36);
        float single10 = BitConverter.ToSingle(numArray, 40);
        float single11 = BitConverter.ToSingle(numArray, 44);
        float single12 = BitConverter.ToSingle(numArray, 48);
        float single13 = BitConverter.ToSingle(numArray, 52);
        float single14 = BitConverter.ToSingle(numArray, 56);
        float single15 = BitConverter.ToSingle(numArray, 60);
        BitConverter.ToSingle(numArray, 64);
        this.controller.SetInput(0, single1 * 57.295f);
        this.controller.SetInput(1, single2 * 57.295f);
        this.controller.SetInput(2, single3 * 57.295f);
        this.controller.SetInput(3, single4 * 57.295f);
        this.controller.SetInput(4, single5 * 57.295f);
        this.controller.SetInput(5, single6 * 57.295f);
        this.controller.SetInput(6, single7);
        this.controller.SetInput(7, single8);
        this.controller.SetInput(8, single9);
        this.controller.SetInput(9, single10);
        this.controller.SetInput(10, single11);
        this.controller.SetInput(11, single12);
        this.controller.SetInput(12, single13);
        this.controller.SetInput(13, single14);
        this.controller.SetInput(14, single15);
      }
    }

    public void PatchGame()
    {
      try
      {
        string str = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/The Crew";
        Console.WriteLine(str);
        string tempFileName = Path.GetTempFileName();
        this.dispatcher.GetInstallPath(str);
        using (WebClient webClient = new WebClient())
        {
          webClient.DownloadFile("http://yaw.one/gameengine/Plugins/The_Crew_2/GT72.zip", tempFileName);
          Console.WriteLine(str);
          this.dispatcher.ExtractToDirectory(tempFileName, str, true);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("{0}", (object) ex.ToString());
      }
    }

    public Dictionary<string, ParameterInfo[]> GetFeatures() => (Dictionary<string, ParameterInfo[]>) null;
  }
}
