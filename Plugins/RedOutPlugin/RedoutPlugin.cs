// Decompiled with JetBrains decompiler
// Type: YawVR_Game_Engine.Plugin.RedoutPlugin
// Assembly: RedOutPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C05644C-29E2-4C25-BA06-FC759BF669E2
// Assembly location: C:\src\yawge\Gameplugins\RedOutPlugin.dll

using RedOutPlugin.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using YawGEAPI;


namespace YawVR_Game_Engine.Plugin
{
  [Export(typeof (Game))]
  [ExportMetadata("Name", "Redout")]
  [ExportMetadata("Version", "1.0")]
  public class RedoutPlugin : Game
  {
    private MemoryMappedFile mmf;
    private Thread readThread;
    private IMainFormDispatcher dispatcher;
    private IProfileManager controller;
    private volatile bool running = false;
    private static readonly string[] inputs = new string[6]
    {
      "Yaw",
      "Pitch",
      "Roll",
      "AccX",
      "AccY",
      "AccZ"
    };

    public int STEAM_ID => 517710;

    public string PROCESS_NAME => "redout";

    public string AUTHOR => "YawVR";

    public bool PATCH_AVAILABLE => false;

    public string Description => Resources.description;

    public Image Logo => (Image) Resources.logo;

    public Image SmallLogo => (Image) Resources.small;

    public Image Background => (Image) Resources.background;

    public void Exit() => this.running = false;

    public void PatchGame()
    {
    }

    public string[] GetInputData() => RedoutPlugin.inputs;

    public LedEffect DefaultLED() => this.dispatcher.JsonToLED(Resources.defProfile);

    public List<Profile_Component> DefaultProfile()
    {
      return this.dispatcher.JsonToComponents(Resources.defProfile);
    }

    public void SetReferences(IProfileManager controller, IMainFormDispatcher dispatcher)
    {
      this.dispatcher = dispatcher;
      this.controller = controller;
    }

    public void Init()
    {
      this.running = true;
      this.readThread = new Thread(new ThreadStart(this.ReadThread));
      this.readThread.Start();
    }

    private void ReadThread()
    {
      while (!this.OpenMMF() && this.running)
      {
        this.dispatcher.ShowNotification(NotificationType.WARNING, "Cant reach MMF, trying again..");
        Console.WriteLine("Cant connect to MMF, trying again..");
        Thread.Sleep(4000);
      }
      while (this.running)
      {
        RedoutPlugin.Packet packet = this.ReadPacket();
        Quaternion quaternion = new Quaternion((double) packet.QuatX, (double) packet.QuatY, (double) packet.QuatZ, (double) packet.QuatZ);
        float num1 = (float) RedoutPlugin.RadianToDegree(quaternion.toYawFromYUp()) * -1f;
        float num2 = (float) RedoutPlugin.RadianToDegree(quaternion.toPitchFromYUp()) * -1f;
        this.controller.SetInput(0, (float) RedoutPlugin.RadianToDegree(quaternion.toRollFromYUp()));
        this.controller.SetInput(1, num2);
        this.controller.SetInput(2, num1);
        this.controller.SetInput(3, packet.AccX);
        this.controller.SetInput(4, packet.AccY);
        this.controller.SetInput(5, packet.AccZ);
        Thread.Sleep(20);
      }
    }

    public RedoutPlugin.Packet ReadPacket()
    {
      using (MemoryMappedViewStream viewStream = this.mmf.CreateViewStream())
      {
        using (BinaryReader binaryReader = new BinaryReader((Stream) viewStream))
        {
          int count = Marshal.SizeOf(typeof (RedoutPlugin.Packet));
          GCHandle gcHandle = GCHandle.Alloc((object) binaryReader.ReadBytes(count), GCHandleType.Pinned);
          RedoutPlugin.Packet structure = (RedoutPlugin.Packet) Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof (RedoutPlugin.Packet));
          gcHandle.Free();
          return structure;
        }
      }
    }

    public static double RadianToDegree(double angle) => angle * (180.0 / Math.PI);

    private bool OpenMMF()
    {
      try
      {
        this.mmf = MemoryMappedFile.OpenExisting("Global\\RedoutSimulatorParams");
        return true;
      }
      catch (UnauthorizedAccessException ex)
      {
        this.dispatcher.DialogShow("Admin privileges are needed to read Redout's MMF. Restart GameEngine in admin mode?", DIALOG_TYPE.QUESTION, (Action<bool>) (_param1 => this.dispatcher.RestartApp(true)));
        this.dispatcher.ExitGame();
        return false;
      }
      catch (FileNotFoundException ex)
      {
        return false;
      }
    }

    public Dictionary<string, ParameterInfo[]> GetFeatures()
    {
      return (Dictionary<string, ParameterInfo[]>) null;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct Packet
    {
      public float QuatX;
      public float QuatY;
      public float QuatZ;
      public float QuatW;
      public float AccWorldX;
      public float AccWorldY;
      public float AccWorldZ;
      public float AccX;
      public float AccY;
      public float AccZ;
    }
  }
}
