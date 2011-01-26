using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Box2DSharpRenderTest.Networking
{
	public enum PlayerState
	{
		/// <summary>
		/// Empty slot
		/// </summary>
		Disconnected,

		/// <summary>
		/// Is connected to the server, but has not
		/// been assigned a game state yet (gives time to precache, etc)
		/// </summary>
		Connecting,

		/// <summary>
		/// Inside the game
		/// </summary>
		Spawned
	}

	public class PlayerList
	{
		List<Player> _players = new List<Player>();

		public Player GetFreePlayer()
		{
			foreach (var x in _players)
			{
				if (x.State == PlayerState.Disconnected)
					return x;
			}

			var player = new Player();
			_players.Add(player);

			return player;
		}

		public Player this[int index]
		{
			get { return _players[index]; }
		}

		public Player PlayerFromEndPoint(bool tcp, IPEndPoint endPoint)
		{
			foreach (var x in _players)
			{
				if (tcp && ((IPEndPoint)x.Tcp.Client.Client.RemoteEndPoint).Address.Equals(endPoint) &&
						((IPEndPoint)x.Tcp.Client.Client.RemoteEndPoint).Port == endPoint.Port)
					return x;
				else if (!tcp && x.Udp.EndPoint.Address.Equals(endPoint) && x.Udp.EndPoint.Port == endPoint.Port)
					return x;
			}
			
			return null;
		}

		public void Remove(Player player)
		{
			// FIXME: do it if needed.
		}
	}

	public class Player
	{
		public class UDP : Paril.Helpers.EasyDispose
		{
			Player _player;
			MemoryStream _memory;

			public BinaryWriter Stream
			{
				get;
				set;
			}

			public IPEndPoint EndPoint
			{
				get;
				set;
			}

			public UDP(Player player)
			{
				_player = player;
				_memory = new MemoryStream();
				Stream = new BinaryWriter(_memory);
			}

			public void Close()
			{
				_memory.Dispose();
				_memory = null;
			}

			protected override void ReleaseManagedResources()
			{
				if (_memory != null)
					Close();
			}

			protected override void ReleaseUnmanagedResources()
			{
			}
		}

		public class TCP : Paril.Helpers.EasyDispose
		{
			Player _player;
			MemoryStream _memory;

			internal MemoryStream Memory
			{
				get { return _memory; }
			}

			public BinaryWriter BatchStream
			{
				get;
				set;
			}

			TcpClient _client;
			public TcpClient Client
			{
				get { return _client; }
				set
				{
					_client = value;

					if (_client != null)
					{
						NetStream = _client.GetStream();
						NetStreamBinary = new BinaryWriter(NetStream);
					}
				}
			}

			public NetworkStream NetStream
			{
				get;
				set;
			}

			public BinaryWriter NetStreamBinary
			{
				get;
				set;
			}

			public event NetworkReceivePacket PacketReceived;

			internal void OnPacketReceived(byte packetType, BinaryWrapper reader, IPEndPoint endPoint)
			{
				if (PacketReceived != null)
					PacketReceived(packetType, reader, endPoint);
			}

			public TCP(Player player)
			{
				_player = player;
				_memory = new MemoryStream();
				BatchStream = new BinaryWriter(_memory);
			}

			public void Close()
			{
				BatchStream.BaseStream.SetLength(0);

				NetStreamBinary.Close();
				Client.Close();
				Client = null;
				NetStreamBinary = null;
				NetStream = null;
			}

			protected override void ReleaseManagedResources()
			{
				if (Client != null)
					Close();
			}

			protected override void ReleaseUnmanagedResources()
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Server we're connected to
		/// </summary>
		public Server Server
		{
			get;
			set;
		}

		/// <summary>
		/// The UDP backend
		/// </summary>
		public UDP Udp
		{
			get;
			set;
		}

		/// <summary>
		/// The TCP backend
		/// </summary>
		public TCP Tcp
		{
			get;
			set;
		}

		/// <summary>
		/// Index of this client
		/// </summary>
		public int Index
		{
			get;
			set;
		}

		/// <summary>
		/// Last time we received a message from this player
		/// </summary>
		public DateTime LastMessage
		{
			get;
			set;
		}

		/// <summary>
		/// Player's state
		/// </summary>
		public PlayerState State
		{
			get;
			set;
		}

		public Player()
		{
			Udp = new UDP(this);
			Tcp = new TCP(this);
		}

		public void Connect(TcpClient client)
		{
			State = PlayerState.Connecting;
			Tcp.Client = client;
			Tcp.NetStreamBinary.Write((byte)ClientPacketTypeBase.ConnectionAck);
		}

		public void Check()
		{
			while (Tcp.NetStream.DataAvailable)
			{
				var reader = new BinaryWrapper(Tcp.NetStream);

				while (true)
				{
					byte packetType = reader.ReadByte();

					if (packetType == PacketTypeBase.EndOfMessage)
						break;

					Tcp.OnPacketReceived(packetType, reader, (IPEndPoint)Tcp.Client.Client.RemoteEndPoint);
				}
			}

			if (Tcp.BatchStream.BaseStream.Length != 0)
			{
				Tcp.NetStreamBinary.Write(Tcp.Memory.ToArray(), 0, (int)Tcp.Memory.Length);
				Tcp.Memory.SetLength(0);
			}
		}

		public void Disconnect()
		{
			Udp.Close();
			Tcp.Close();
		}
	}

	public class Server
	{
		public class UDPBackend
		{
			public const int Port = 10101;

			public UdpClient Listener
			{
				get;
				set;
			}

			public Server Server
			{
				get;
				set;
			}

			public UDPBackend(Server server)
			{
				Server = server;
			}

			public void Host()
			{
				Listener = new UdpClient(Port);
			}

			public void Check()
			{
				while (Listener.Available != 0)
				{
					IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
					var dgram = Listener.Receive(ref endPoint);

					using (var stream = new MemoryStream(dgram))
					{
						var reader = new BinaryWrapper(stream);

						while (stream.Length != stream.Position)
						{
							byte packetType = reader.ReadByte();
							Server.OnReceiveUDPData(packetType, reader, endPoint);
						}
					}
				}
			}

			public void Close()
			{
				Listener.Close();
			}
		}

		public class TCPBackend
		{
			public const int Port = 01010;

			public TcpListener Listener
			{
				get;
				set;
			}
			
			public Server Server
			{
				get;
				set;
			}

			public TCPBackend(Server server)
			{
				Server = server;
			}

			public static IPAddress GetLocalAddress(string hostname)
			{
				IPHostEntry host = Dns.GetHostEntry(hostname);

				foreach (IPAddress ip in host.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetworkV6)
						continue;

					return ip;
				}

				throw new Exception("No IPv4 address found?");
			}

			public void Host()
			{
				Listener = new TcpListener(GetLocalAddress("localhost"), Port);
				Listener.Start();
			}

			public void Close()
			{
				Listener.Stop();
			}

			public void Check()
			{
				while (Listener.Pending())
				{
					var client = Listener.AcceptTcpClient();

					Player player = Server.Players.GetFreePlayer();
					player.Connect(client);
					player.State = PlayerState.Connecting;
				}
			}
		}

		public UDPBackend Udp
		{
			get;
			set;
		}

		public PlayerList Players
		{
			get;
			private set;
		}

		public TCPBackend Tcp
		{
			get;
			set;
		}

		public event NetworkReceivePacket ReceiveUDPData;

		private void OnReceiveUDPData(byte packetType, BinaryWrapper reader, IPEndPoint endPoint)
		{
			if (ReceiveUDPData != null)
				ReceiveUDPData(packetType, reader, endPoint);
		}

		public Server()
		{
			Players = new PlayerList();
		}

		public void Check()
		{
			Tcp.Check();
			Udp.Check();
		}

		public void Host()
		{
			Udp = new UDPBackend(this);
			Tcp = new TCPBackend(this);

			Tcp.Host();
			Udp.Host();
		}

		public void Close()
		{
			Udp.Close();
			Tcp.Close();

			Udp = null;
			Tcp = null;
		}
	}
}
