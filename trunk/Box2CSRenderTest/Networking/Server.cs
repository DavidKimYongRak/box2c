using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Box2DSharpRenderTest.Networking
{
	public static class NetworkSettings
	{
		public const int Port = 10101;
	}

	public abstract class EasyDispose : IDisposable
	{
		public bool Disposed
		{
			get;
			private set;
		}

		protected abstract void ReleaseUnmanagedResources();
		protected abstract void ReleaseManagedResources();

		void Dispose(bool disposing)
		{
			if (!Disposed)
			{
				if (disposing)
					ReleaseManagedResources();

				ReleaseUnmanagedResources();
				Disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~EasyDispose()
		{
			Dispose(false);
		}
	}

	public abstract class DataPacket : EasyDispose
	{
		protected BinaryReader Memory
		{
			get;
			private set;
		}

		public IPEndPoint EndPoint
		{
			get;
			private set;
		}

		public DataPacket(byte[] data, IPEndPoint endPoint)
		{
			Memory = new BinaryReader(new MemoryStream(data));
			EndPoint = endPoint;
		}

		public abstract void Parse();

		protected override void ReleaseManagedResources()
		{
			Memory.BaseStream.Dispose();
			Memory = null;
		}

		protected override void ReleaseUnmanagedResources()
		{
		}
	}

	public enum EServerDataPacketType
	{
		ConnectPacket,
		DisconnectPacket,
		ChatPacket,
		ClientCmd
	}

	public class ServerDataPacket : DataPacket
	{
		public NetworkServer Server
		{
			get;
			set;
		}

		public ServerDataPacket(NetworkServer server, byte[] data, IPEndPoint endPoint) :
			base(data, endPoint)
		{
			Server = server;
		}

		public override void Parse()
		{
			while (Memory.BaseStream.Length != Memory.BaseStream.Position)
			{
				EServerDataPacketType type = (EServerDataPacketType)Memory.ReadByte();

				switch (type)
				{
				case EServerDataPacketType.ConnectPacket:
					{
						foreach (var p in Server.Clients)
						{
							if (p.EndPoint.Address.Equals(EndPoint.Address))
								throw new Exception();
						}

						var client = new ConnectedClient(Server);
						client.Name = Memory.ReadString();
						client.State = ClientState.Connected;
						client.EndPoint = EndPoint;

						Server.Clients.Add(client);

						client.Stream.Write((byte)EClientDataPacketType.ConnectedPacket);
						client.Stream.Write((byte)(Server.Clients.Count - 1));

						Server.Stream.Write((byte)EClientDataPacketType.ChatPacket);
						Server.Stream.Write("Player "+client.Name+" connected");
					}
					break;
				case EServerDataPacketType.DisconnectPacket:
					{
						var p = Server.ClientFromEndPoint(EndPoint);

						if (p == null)
							throw new Exception();

						Server.Clients.Remove(p);

						Server.Stream.Write((byte)EClientDataPacketType.ChatPacket);
						Server.Stream.Write("Player "+p.Name+" disconnected");
					}
					break;
				case EServerDataPacketType.ChatPacket:
					{
						var p = Server.ClientFromEndPoint(EndPoint);
						var index = Memory.ReadByte();

						var finalString = p.Name + ": " + Memory.ReadString();

						Server.Stream.Write((byte)EClientDataPacketType.ChatPacket);
						Server.Stream.Write(finalString);
						break;
					}
				case EServerDataPacketType.ClientCmd:
					{
						var p = Server.ClientFromEndPoint(EndPoint);
						p.Keys = (Form2.GameKeys)Memory.ReadInt32();
						break;
					}
				}
			}
		}
	}

	public enum ClientState
	{
		Connecting,
		Connected,
		Disconnected
	}

	public class ConnectedClient
	{
		public IPEndPoint EndPoint
		{
			get;
			set;
		}

		public ClientState State
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
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

		public NetworkServer Server
		{
			get;
			set;
		}

		public Form2.GameKeys Keys
		{
			get;
			set;
		}

		public ConnectedClient(NetworkServer server)
		{
			Server = server;
			Stream = new BinaryWriter(Memory = new MemoryStream());
		}

		public void SendData()
		{
			if (Stream.BaseStream.Length != 0)
			{
				Server.Server.Send(Memory.GetBuffer(), (int)Memory.Length, EndPoint);
				Memory.SetLength(0);
				Memory.Position = 0;
			}
		}
	}

	public class NetworkServer
	{
		public List<ConnectedClient> Clients
		{
			get;
			private set;
		}

		public long Frame
		{
			get;
			set;
		}

		public ConnectedClient ClientFromEndPoint(IPEndPoint endPoint)
		{
			foreach (var p in Clients)
			{
				if (p.EndPoint.Address.Equals(endPoint.Address))
					return p;
			}

			return null;
		}

		UdpClient _udpServer;

		public UdpClient Server
		{
			get { return _udpServer; }
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

		public NetworkServer()
		{
			_udpServer = new UdpClient(NetworkSettings.Port);
			Clients = new List<ConnectedClient>();
			Memory = new MemoryStream();
			Stream = new BinaryWriter(Memory);
		}

		public void Close()
		{
			Memory.SetLength(0);
			Memory.Position = 0;
			Stream.Write((byte)EClientDataPacketType.DisconnectedPacket);
			Check(true);

			_udpServer.Close();
		}

		public void Check(bool _skipCheck = false)
		{
			while (!_skipCheck && _udpServer.Available != 0)
			{
				IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
				var data = new ServerDataPacket(this, _udpServer.Receive(ref endPoint), endPoint);
				data.Parse();
			}

			if (Memory.Length != 0)
			{
				foreach (var p in Clients)
					_udpServer.Send(Memory.GetBuffer(), (int)Memory.Length, p.EndPoint);
				Memory.SetLength(0);
				Memory.Position = 0;
			}
		}
	}
}
