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
	[Export(typeof(Game))]
	[ExportMetadata("Name", "Assetto Corsa")]
	[ExportMetadata("Version", "1.2")]
	internal class ACPlugin : Game
	{
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
				List<byte> list = new List<byte>();
				list.AddRange(BitConverter.GetBytes(identifier));
				list.AddRange(BitConverter.GetBytes(version));
				list.AddRange(BitConverter.GetBytes(operationId));
				return list.ToArray();
			}
		}

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

		public Image Logo => Resources.logo;

		public Image SmallLogo => Resources.small;

		public Image Background => Resources.background;

		public string Description => string.Empty;

		public void PatchGame()
		{
		}

		public void Exit()
		{
			udpClient.Close();
			udpClient = null;
			running = false;
		}

		public void SetReferences(IProfileManager controller, IMainFormDispatcher dispatcher)
		{
			this.controller = controller;
			this.dispatcher = dispatcher;
		}

		public void Init()
		{
			input = Interaction.InputBox("Enter the host address of Assetto Corsa\nLeave default value if its running on this PC", "Endpoint", "127.0.0.1");
			HandShake();
			running = true;
			readThread = new Thread(ReadFunction);
			readThread.Start();
		}

		private void ReadFunction()
		{
			try
			{
				IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 9996);
				while (running)
				{
					try
					{
						byte[] value = udpClient.Receive(ref remoteEP);
						float num = BitConverter.ToSingle(value, 8);
						float value2 = BitConverter.ToSingle(value, 68);
						float value3 = BitConverter.ToSingle(value, 72) / 41f * (num / 100f);
						float value4 = BitConverter.ToSingle(value, 28);
						float value5 = BitConverter.ToSingle(value, 32);
						float value6 = BitConverter.ToSingle(value, 36);
						controller.SetInput(0, num);
						controller.SetInput(1, value2);
						controller.SetInput(2, value3);
						controller.SetInput(3, value4);
						controller.SetInput(4, value5);
						controller.SetInput(5, value6);
					}
					catch (SocketException)
					{
						for (int i = 0; i < GetInputData().Length; i++)
						{
							controller.SetInput(i, 0f);
						}
						Thread.Sleep(1000);
						HandShake();
					}
				}
			}
			catch (ThreadAbortException)
			{
			}
		}

		private void HandShake()
		{
			udpClient = new UdpClient();
			udpClient.Client.ReceiveTimeout = 1000;
			udpClient.Connect(input, 9996);
			byte[] array = new Handshaker(1, 1, 0).toByte();
			Thread.Sleep(250);
			udpClient.Send(array, array.Length);
			array = new Handshaker(1, 1, 1).toByte();
			udpClient.Send(array, array.Length);
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
				new YawColor(byte.MaxValue, 40, 0),
				new YawColor(80, 80, 80),
				new YawColor(byte.MaxValue, 100, 0),
				new YawColor(140, 0, byte.MaxValue)
			}, -20f);
		}

		public List<Profile_Component> DefaultProfile()
		{
			return dispatcher.JsonToComponents(Resources.defProfile);
		}

		public Dictionary<string, ParameterInfo[]> GetFeatures()
		{
			return null;
		}
	}
}
