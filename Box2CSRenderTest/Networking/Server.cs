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

	public class PlayerList : IEnumerable<Player>, System.Collections.IEnumerable
	{
		Server _server;

		List<Player> _players = new List<Player>();

		public Player GetFreePlayer()
		{
			foreach (var x in _players)
			{
				if (x.State == PlayerState.Disconnected)
					return x;
			}

			var player = new Player(_server);
			player.Index = _players.Count;
			_players.Add(player);

			return player;
		}

		public int Count
		{
			get { return _players.Count; }
		}

		public IEnumerator<Player> GetEnumerator()
		{
			return _players.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
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

		public PlayerList(Server server)
		{
			_server = server;
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

			public string Name
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

		/// <summary>
		/// Player's name
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Whether this player is active or not.
		/// </summary>
		public bool Active
		{
			get;
			set;
		}

		/// <summary>
		/// User data
		/// </summary>
		public object Data
		{
			get;
			set;
		}

		public Player(Server server)
		{
			Server = server;

			Udp = new UDP(this);
			Tcp = new TCP(this);
		}

		public void Connect(TcpClient client)
		{
			State = PlayerState.Connecting;
			Tcp.Client = client;
			Tcp.NetStreamBinary.Write((byte)ClientPacketTypeBase.ConnectionAck);
			Tcp.NetStreamBinary.Write((byte)PacketTypeBase.EndOfMessage);
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
					else if (packetType == ServerPacketTypeBase.ConnectionAck)
					{
						Active = true;
						int port = reader.ReadInt();
						Udp.EndPoint = new IPEndPoint(((IPEndPoint)Tcp.Client.Client.RemoteEndPoint).Address, port);
						string playerName = StringWriter.Read(reader);

						Name = playerName;
					}
					else if (packetType == ServerPacketTypeBase.Disconnected)
					{
						Active = false;

						foreach (var x in Server.Players)
						{
							if (x == this)
								continue;

							SendData(x, ClientPacketTypeBase.PlayerDataBits.ActiveBit);
						}

						Server.Players.Remove(this);
					}

					Tcp.OnPacketReceived(packetType, reader, (IPEndPoint)Tcp.Client.Client.RemoteEndPoint);
					Server.OnReceiveTCPData(packetType, reader, this);
				}
			}

			if (Tcp.BatchStream.BaseStream.Length != 0)
			{
				Tcp.NetStreamBinary.Write(Tcp.Memory.ToArray(), 0, (int)Tcp.Memory.Length);
				Tcp.Memory.SetLength(0);
			}
		}

		/// <summary>
		/// Sends this player's data to x
		/// </summary>
		/// <param name="x"></param>
		internal void SendData(Player x, byte bits)
		{
			x.Tcp.BatchStream.Write(ClientPacketTypeBase.PlayerData);
			x.Tcp.BatchStream.Write((byte)Index);
			x.Tcp.BatchStream.Write(bits);

			if ((bits & ClientPacketTypeBase.PlayerDataBits.ActiveBit) != 0)
				x.Tcp.BatchStream.Write(Active);

			if ((bits & ClientPacketTypeBase.PlayerDataBits.NameBit) != 0)
				StringWriter.Write(Name, x.Tcp.BatchStream);

			Server.OnSendPlayerData(this, x.Tcp.BatchStream);

			x.Tcp.BatchStream.Write(PacketTypeBase.EndOfMessage);
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
			MemoryStream _memory;

			public BinaryWriter BatchStream
			{
				get;
				set;
			}

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
				_memory = new MemoryStream();
				BatchStream = new BinaryWriter(_memory);
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

				byte[] batchArray;

				if (BatchStream.BaseStream.Length != 0)
					batchArray = _memory.ToArray();
				else
					batchArray = new byte[0];

				foreach (var c in Server.Players)
				{
					if (c.Udp.Stream.BaseStream.Length != 0 ||
						BatchStream.BaseStream.Length != 0)
					{
						var arr = ((MemoryStream)c.Udp.Stream.BaseStream).ToArray().Concat<byte>(batchArray).ToArray<byte>();

						Listener.Send(arr, arr.Length, c.Udp.EndPoint);

						c.Udp.Stream.BaseStream.SetLength(0);
					}
				}

				_memory.SetLength(0);
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
				Listener = new TcpListener(IPAddress.Any, Port);
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

				foreach (var c in Server.Players)
				{
					c.Check();

					if (c.Tcp.BatchStream.BaseStream.Length != 0)
					{
						var arr = ((MemoryStream)c.Tcp.BatchStream.BaseStream).ToArray();
						c.Tcp.NetStreamBinary.Write(arr);

						c.Udp.Stream.BaseStream.SetLength(0);
					}
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

		public event TCPNetworkReceivePacket ReceiveTCPData;

		public void OnReceiveTCPData(byte packetType, BinaryWrapper reader, Player player)
		{
			if (ReceiveTCPData != null)
				ReceiveTCPData(packetType, reader, player);
		}

		public event ServerSendPlayerData SendPlayerData;

		internal void OnSendPlayerData(Player player, BinaryWriter writer)
		{
			if (SendPlayerData != null)
				SendPlayerData(player, writer);
		}

		/// <summary>
		/// Accepts a player's connection and finalizes it.
		/// Do this after you have all the information you need.
		/// </summary>
		/// <param name="endPoint">Endpoint of theplayer</param>
		public void AcceptPlayer(Player player)
		{
			player.State = PlayerState.Spawned;
			player.Tcp.NetStreamBinary.Write((byte)ClientPacketTypeBase.SpawnAck);
			player.Tcp.NetStreamBinary.Write(Players.Count);
			player.Tcp.NetStreamBinary.Write(player.Index);
			player.Tcp.NetStreamBinary.Write((byte)PacketTypeBase.EndOfMessage);

			// send this player everybody's data
			foreach (var x in Players)
			{
				//if (x == player)
				//	continue;

				if (x != player)
				{
					x.SendData(player,
								ClientPacketTypeBase.PlayerDataBits.ActiveBit |
								ClientPacketTypeBase.PlayerDataBits.NameBit);
				}

				player.SendData(x,
						ClientPacketTypeBase.PlayerDataBits.ActiveBit |
						ClientPacketTypeBase.PlayerDataBits.NameBit);
			}
		}

		public Server()
		{
			Players = new PlayerList(this);
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
