using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;

namespace Box2DSharpRenderTest.Networking
{
	public enum EClientDataPacketType
	{
		ConnectedPacket,
		DisconnectedPacket,
		ChatPacket,
		FramePacket
	}

	public class ClientDataPacket : DataPacket
	{
		public NetworkClient Client
		{
			get;
			set;
		}

		public ClientDataPacket(NetworkClient client, byte[] data, IPEndPoint endPoint) :
			base(data, endPoint)
		{
			Client = client;
		}

		public override void Parse()
		{
			while (Memory.BaseStream.Length != Memory.BaseStream.Position)
			{
				EClientDataPacketType type = (EClientDataPacketType)Memory.ReadByte();

				switch (type)
				{
				case EClientDataPacketType.ConnectedPacket:
					{
						Client.ConnectedIndex = (int)Memory.ReadByte();
						break;
					}
				case EClientDataPacketType.ChatPacket:
					{
						Client.Console.AddMessage(Memory.ReadString());
						break;
					}
				case EClientDataPacketType.DisconnectedPacket:
					{
						System.Windows.Forms.MessageBox.Show("Forcibly disconnected");
						System.Windows.Forms.Application.Exit();
						break;
					}
				case EClientDataPacketType.FramePacket:
					{
						Client.OldFrame = Client.CurFrame;

						Frame frame = new Frame();

						frame.ServerFrame = Memory.ReadInt64();
						frame.ServerTime = (int)frame.ServerFrame * Form2.settingsHzInMs;

						frame.Transforms = new TransformHolder[(int)BipedFixtureIndex.Max, 2];

						for (int i = 0; i < (int)BipedFixtureIndex.Max; ++i)
						{
							frame.Transforms[i, 0] = new TransformHolder(new Vector2(Memory.ReadSingle(), Memory.ReadSingle()), Memory.ReadSingle());
							frame.Transforms[i, 1] = new TransformHolder(new Vector2(Memory.ReadSingle(), Memory.ReadSingle()), Memory.ReadSingle());
						}

						Client.CurFrame = frame;
						Client.NextFrameTime = DateTime.Now.TimeOfDay.TotalMilliseconds;

						break;
					}
				}
			}
		}
	}

	public struct TransformHolder
	{
		public Vector2 Position;
		public float Angle;

		public TransformHolder(Vector2 position, float angle)
		{
			Position = position;
			Angle = angle;
		}

		public Transform ToTransform()
		{
			var mat = new Mat22(Angle);
			return new Transform(ref Position, ref mat);
		}
	}

	public struct Frame
	{
		public long ServerFrame
		{
			get;
			set;
		}

		public int ServerTime
		{
			get;
			set;
		}

		public TransformHolder[,] Transforms
		{
			get;
			set;
		}
	}

	public class ClientConsole
	{
		public List<string> Messages
		{
			get;
			set;
		}

		int _displayRange;
		public int DisplayRange
		{
			get { return _displayRange; }
			
			set
			{
				if (_displayRange >= Messages.Count)
					_displayRange = -1;
				else
					_displayRange = value;
			}
		}

		public void AddMessage(string msg)
		{
			Messages.Add(msg);
		}

		public ClientConsole()
		{
			Messages = new List<string>();
			DisplayRange = -1;
		}
	}

	public class NetworkClient
	{
		public ClientConsole Console
		{
			get;
			set;
		}

		public int ConnectedIndex
		{
			get;
			set;
		}

		public class UDP
		{
			NetworkClient _client;

			public UdpClient Client
			{
				get;
				private set;
			}

			public MemoryStream Memory
			{
				get;
				private set;
			}

			public BinaryWriter Stream
			{
				get;
				private set;
			}

			public IPEndPoint EndPoint
			{
				get;
				private set;
			}

			public UDP(NetworkClient client)
			{
				_client = client;
			}

			public void Connect(IPEndPoint endPoint, string name)
			{
				EndPoint = endPoint;
				Client = new UdpClient();
				Client.Connect(EndPoint);

				Memory = new MemoryStream();
				Stream = new BinaryWriter(Memory);

				Stream.Write((byte)EServerDataPacketType.ConnectPacket);
				Stream.Write(name);
			}

			public void Close()
			{
				Memory.SetLength(0);
				Memory.Position = 0;
				Stream.Write((byte)EServerDataPacketType.DisconnectPacket);
				Check(true);

				Client.Close();
				Client = null;
			}

			public void Check(bool skipCheck = false)
			{
				while (!skipCheck && Client.Available != 0)
				{
					IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
					var data = new ClientDataPacket(_client, Client.Receive(ref endPoint), endPoint);
					data.Parse();
				}

				if (Memory.Length != 0)
				{
					Client.Send(Memory.GetBuffer(), (int)Memory.Length);
					Memory.SetLength(0);
					Memory.Position = 0;
				}
			}
		}

		public UDP Udp
		{
			get;
			private set;
		}


		public class TCP
		{
			NetworkClient _client;

			public TcpClient Client
			{
				get;
				private set;
			}

			public MemoryStream Memory
			{
				get;
				private set;
			}

			public BinaryWriter Stream
			{
				get;
				private set;
			}

			public IPEndPoint EndPoint
			{
				get;
				private set;
			}

			public NetworkStream NetStream
			{
				get;
				set;
			}

			public TCP(NetworkClient client)
			{
				_client = client;
			}

			public void Connect(IPEndPoint endPoint, string name)
			{
				EndPoint = endPoint;
				Client = new TcpClient();
				Client.Connect(EndPoint);

				NetStream = Client.GetStream();

				Memory = new MemoryStream();
				Stream = new BinaryWriter(Memory);

				Stream.Write((byte)EServerDataPacketType.ConnectPacket);
				Stream.Write(name);
			}

			public void Close()
			{
				Client.Close();
				Client = null;
			}

			public void Check(bool skipCheck = false)
			{
				while (!skipCheck && NetStream.DataAvailable)
				{
					List<byte> bytes = new List<byte>();
					
					while (true)
					{
						byte[] buffer = new byte[4096];
						int i = NetStream.Read(buffer, 0, buffer.Length);
					
						if (i <= 0)
							break;

						bytes.AddRange(buffer);
					}

					var data = new ClientDataPacket(_client, bytes.ToArray(), (IPEndPoint)Client.Client.RemoteEndPoint);
					data.Parse();
				}

				if (Memory.Length != 0)
				{
					NetStream.Write(Memory.GetBuffer(), 0, (int)Memory.Length);
					Memory.SetLength(0);
					Memory.Position = 0;
				}
			}
		}

		public TCP Tcp
		{
			get;
			private set;
		}

		public long Frame
		{
			get;
			set;
		}

		public Frame OldFrame
		{
			get;
			set;
		}

		public Frame CurFrame
		{
			get;
			set;
		}

		public int Time
		{
			get;
			set;
		}

		public double NextFrameTime
		{
			get;
			set;
		}

		public NetworkClient(IPAddress address, string name)
		{
			Udp = new NetworkClient.UDP(this);
			Udp.Connect(new IPEndPoint(address, NetworkSettings.UDPPort), name);

			Console = new ClientConsole();
		}

		public void Close()
		{
			Udp.Close();
		}

		public void Check()
		{
			Udp.Check();
			Tcp.Check();
		}

		public void SendText(string str)
		{
			Udp.Stream.Write((byte)EServerDataPacketType.ChatPacket);
			Udp.Stream.Write((byte)ConnectedIndex);
			Udp.Stream.Write(str);
		}
	}
}
