using Dirtrally2Plugin.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Xml;
using YawGEAPI;

namespace YawVR_Game_Engine.Plugin
{
	[Export(typeof(Game))]
	[ExportMetadata("Name", "Dirt Rally 2")]
	[ExportMetadata("Version", "1.0")]
	internal class DirtRally2Plugin : Game
	{
		private UdpClient udpClient;

		private Thread readThread;

		private IPEndPoint remoteIP;

		private IProfileManager controller;

		private IMainFormDispatcher dispatcher;

		private bool running = false;

		public string PROCESS_NAME => "dirtrally2";

		public int STEAM_ID => 690790;

		public string AUTHOR => "YawVR";

		public bool PATCH_AVAILABLE => true;

		public string Description => Resources.description;

		public Image Logo => Resources.logo;

		public Image SmallLogo => Resources.small;

		public Image Background => Resources.background;

		public List<Profile_Component> DefaultProfile()
		{
			return dispatcher.JsonToComponents(Resources.defProfile);
		}

		public LedEffect DefaultLED()
		{
			return dispatcher.JsonToLED(Resources.defProfile);
		}

		public void Exit()
		{
			udpClient.Close();
			udpClient = null;
			running = false;
		}

		public string[] GetInputData()
		{
			return new string[19]
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
				"VelocityZ"
			};
		}

		public void SetReferences(IProfileManager controller, IMainFormDispatcher dispatcher)
		{
			this.controller = controller;
			this.dispatcher = dispatcher;
		}

		public void Init()
		{
			udpClient = new UdpClient(20777);
			udpClient.Client.ReceiveTimeout = 2000;
			running = true;
			readThread = new Thread(ReadFunction);
			readThread.Start();
		}

		private void ReadFunction()
		{
			try
			{
				while (running)
				{
					try
					{
						byte[] data = udpClient.Receive(ref remoteIP);
						float speed = BitConverter.ToSingle(data, 28);
						float rpm = BitConverter.ToSingle(data, 148) / 30f;
						float velocityX = (float)(Math.Asin(BitConverter.ToSingle(data, 32)) * 57.3);
						float velocityY = (float)(Math.Asin(BitConverter.ToSingle(data, 36)) * 57.3);
						float velocityZ = (float)(Math.Asin(BitConverter.ToSingle(data, 40)) * 57.3);
						float steer = BitConverter.ToSingle(data, 120);
						float Gforce_lon = BitConverter.ToSingle(data, 140);
						float Gforce_lat = BitConverter.ToSingle(data, 136);
						float pitchX = BitConverter.ToSingle(data, 56);
						float pitchY = BitConverter.ToSingle(data, 60);
						float pitchZ = BitConverter.ToSingle(data, 64);
						float rollX = BitConverter.ToSingle(data, 44);
						float rollY = BitConverter.ToSingle(data, 48);
						float rollZ = BitConverter.ToSingle(data, 52);
						float Susp_pos_bl = BitConverter.ToSingle(data, 68);
						float Susp_pos_br = BitConverter.ToSingle(data, 72);
						float Susp_pos_fl = BitConverter.ToSingle(data, 76);
						float Susp_pos_fr = BitConverter.ToSingle(data, 80);
						float Susp_vel_bl = BitConverter.ToSingle(data, 84);
						float Susp_vel_br = BitConverter.ToSingle(data, 88);
						float Susp_vel_fl = BitConverter.ToSingle(data, 92);
						float Susp_vel_fr = BitConverter.ToSingle(data, 96);
						float Wheel_speed_bl = BitConverter.ToSingle(data, 100);
						float Wheel_speed_br = BitConverter.ToSingle(data, 104);
						float pitch = (float)(Math.Asin(0f - pitchY) * 57.3);
						float roll = 0f - (float)(Math.Asin(0f - rollY) * 57.3);
						float yaw = (float)Math.Atan2(pitchY + pitchX, pitchZ) * 57.3f;
						controller.SetInput(0, speed);
						controller.SetInput(1, rpm);
						controller.SetInput(2, steer);
						controller.SetInput(3, Gforce_lon);
						controller.SetInput(4, Gforce_lat);
						controller.SetInput(5, pitch);
						controller.SetInput(6, roll);
						controller.SetInput(7, yaw);
						controller.SetInput(8, Susp_pos_bl);
						controller.SetInput(9, Susp_pos_br);
						controller.SetInput(10, Susp_pos_fl);
						controller.SetInput(11, Susp_pos_fr);
						controller.SetInput(12, Susp_vel_bl);
						controller.SetInput(13, Susp_vel_br);
						controller.SetInput(14, Susp_vel_fl);
						controller.SetInput(15, Susp_vel_fr);
						controller.SetInput(16, velocityX);
						controller.SetInput(17, velocityY);
						controller.SetInput(18, velocityZ);
					}
					catch (Exception)
					{
					}
				}
			}
			catch (ThreadAbortException)
			{
			}
		}

		public void PatchGame()
		{
			byte b = 0;
			string[] array = new string[2]
			{
				Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/My Games/DiRT Rally 2.0/hardwaresettings/hardware_settings_config.xml",
				Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/My Games/DiRT Rally 2.0/hardwaresettings/hardware_settings_config_vr.xml"
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (File.Exists(array[i]))
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(array[1]);
					XmlNode documentElement = xmlDocument.DocumentElement;
					XmlNode xmlNode = documentElement.SelectSingleNode("motion_platform");
					xmlNode.SelectSingleNode("dbox").Attributes["enabled"].Value = "true";
					xmlNode.SelectSingleNode("udp").Attributes["enabled"].Value = "true";
					xmlNode.SelectSingleNode("udp").Attributes["ip"].Value = "127.0.0.1";
					xmlNode.SelectSingleNode("udp").Attributes["extradata"].Value = "1";
					xmlNode.SelectSingleNode("udp").Attributes["port"].Value = "20777";
					xmlNode.SelectSingleNode("udp").Attributes["delay"].Value = "1";
					xmlDocument.Save(array[i]);
					b = (byte)(b + 1);
					dispatcher.ShowNotification(NotificationType.INFO, array[i] + " patched!");
				}
			}
			if (b == 0)
			{
				dispatcher.DialogShow("Could not patch dirt rally 2. Make sure to start the game at least once before patching!", DIALOG_TYPE.INFO);
			}
		}

		public Dictionary<string, ParameterInfo[]> GetFeatures()
		{
			return null;
		}
	}
}
