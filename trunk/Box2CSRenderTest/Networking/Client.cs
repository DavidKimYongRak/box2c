using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Box2DSharpRenderTest.Networking
{
	public class ConnectedPlayer
	{
		public int Index
		{
			get;
			set;
		}

		public bool Active
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public ConnectedPlayer(int index)
		{
			Index = index;
		}
	}

	public class ConnectedPlayerList : IEnumerable<ConnectedPlayer>, System.Collections.IEnumerable
	{
		List<ConnectedPlayer> _players = new List<ConnectedPlayer>();

		public ConnectedPlayer this[int index]
		{
			get
			{
				if (index >= _players.Count)
				{
					if (index != _players.Count)
						throw new Exception(); // shouldn't happen!

					var x = new ConnectedPlayer(_players.Count);
					_players.Add(x);
					return x;
				}

				return _players[index];
			}
		}

		public IEnumerator<ConnectedPlayer> GetEnumerator()
		{
			return _players.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void SetPlayers(int count)
		{
			for (int i = 0; i < count; ++i)
				_players.Add(new ConnectedPlayer(i));
		}

		public void Clear()
		{
			_players.Clear();
		}
	}

	public class Client
	{
		public class TCPBackend
		{
			Client _client;
			MemoryStream _memory;

			TcpClient _tcpClient;
			public TcpClient Client
			{
				get { return _tcpClient; }
				set { _tcpClient = value; }
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

			public BinaryWriter BatchStream
			{
				get;
				set;
			}

			public TCPBackend(Client client)
			{
				_client = client;
			}

			public bool Connected
			{
				get;
				private set;
			}

			public void Connect(string hostName)
			{
				Client = new TcpClient();
				Client.BeginConnect(hostName, Server.TCPBackend.Port, 
					delegate(IAsyncResult ar)
					{
						Client.EndConnect(ar);
						NetStream = Client.GetStream();
						NetStreamBinary = new BinaryWriter(NetStream);
						Connected = true;
					}
					, null);

				_memory = new MemoryStream();
				BatchStream = new BinaryWriter(_memory);
			}

			public event NetworkReceivePacket PacketReceived;

			internal void OnPacketReceived(byte packetType, BinaryWrapper reader, IPEndPoint endPoint)
			{
				if (PacketReceived != null)
					PacketReceived(packetType, reader, endPoint);
			}

			public void Check()
			{
				if (!Connected)
					return;

				while (Client.Available != 0)
				{
					var reader = new BinaryWrapper(NetStream);

					while (true)
					{
						byte packetType = reader.ReadByte();

						if (packetType == PacketTypeBase.EndOfMessage)
							break;
						else if (packetType == ClientPacketTypeBase.SpawnAck)
						{
							int count = reader.ReadInt();
							_client.Players.SetPlayers(count);
							_client.PlayerIndex = reader.ReadInt();

							_client.Players[_client.PlayerIndex].Name = _client.Name;

							_client.Spawned();
						}
						else if (packetType == ClientPacketTypeBase.ConnectionAck)
							_client.Connected();
						else if (packetType == ClientPacketTypeBase.PlayerData)
						{
							int index = reader.ReadByte();
							byte bits = reader.ReadByte();
							var player = _client.GetPlayer(index);

							if ((bits & ClientPacketTypeBase.PlayerDataBits.ActiveBit) != 0)
								player.Active = reader.ReadBoolean();

							if ((bits & ClientPacketTypeBase.PlayerDataBits.NameBit) != 0)
								player.Name = StringWriter.Read(reader);

							_client.OnPlayerDataReceived(player, reader);
						}

						OnPacketReceived(packetType, reader, (IPEndPoint)Client.Client.RemoteEndPoint);
					}			
				}

				if (BatchStream.BaseStream.Length != 0)
				{
					NetStreamBinary.Write(_memory.ToArray(), 0, (int)_memory.Length);
					_memory.SetLength(0);
				}
			}

			public void Close()
			{
				NetStreamBinary.Close();
				Client.Close();
				Client = null;
				BatchStream.Close();
			}
		}

		public class UDPBackend
		{
			Client _client;
			MemoryStream _memory;

			public UdpClient Client
			{
				get;
				set;
			}

			public BinaryWriter Stream
			{
				get;
				set;
			}

			public UDPBackend(Client client)
			{
				_client = client;
				_memory = new MemoryStream();
				Stream = new BinaryWriter(_memory);
			}

			public void Connect(string hostName)
			{
				Client = new UdpClient(hostName, Server.UDPBackend.Port);
			}

			public event NetworkReceivePacket PacketReceived;

			internal void OnPacketReceived(byte packetType, BinaryWrapper reader, IPEndPoint endPoint)
			{
				if (PacketReceived != null)
					PacketReceived(packetType, reader, endPoint);
			}

			public void Check()
			{
				while (Client.Available != 0)
				{
					IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
					var dgram = Client.Receive(ref endPoint);

					using (var stream = new MemoryStream(dgram))
					{
						var reader = new BinaryWrapper(stream);

						while (stream.Length != stream.Position)
						{
							byte packetType = reader.ReadByte();
							OnPacketReceived(packetType, reader, endPoint);
						}
					}
				}

				if (Stream.BaseStream.Length != 0)
				{
					var arr = _memory.ToArray();
					Client.Send(arr, arr.Length);

					_memory.SetLength(0);
				}
			}

			public void Close()
			{
				Stream.Close();
				Client.Close();
			}
		}

		public TCPBackend Tcp
		{
			get;
			set;
		}

		public UDPBackend Udp
		{
			get;
			set;
		}

		/// <summary>
		/// FIXME: remove soon
		/// </summary>
		string _name;
		public string Name
		{
			get { return _name; }
			
			set
			{
				_name = value;

				if (Tcp.Connected)
				{
				}
			}
		}

		public IPAddress Address
		{
			get;
			set;
		}

		public ConnectedPlayerList Players
		{
			get;
			set;
		}

		public int PlayerIndex
		{
			get;
			set;
		}

		public event ClientRecievePlayerData PlayerDataReceived;

		internal void OnPlayerDataReceived(ConnectedPlayer player, BinaryWrapper reader)
		{
			if (PlayerDataReceived != null)
				PlayerDataReceived(player, reader);
		}
	
		public Client()
		{
			Tcp = new TCPBackend(this);
			Udp = new UDPBackend(this);

			Players = new ConnectedPlayerList();
		}

		public void Check()
		{
			Tcp.Check();
			Udp.Check();
		}

		public void Spawned()
		{
		}

		public ConnectedPlayer GetPlayer(int index)
		{
			return Players[index];
		}

		/// <summary>
		/// Pushes UDP address requirement into the buffer
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		public void SendUdpData(BinaryWriter writer)
		{
			writer.Write(((IPEndPoint)Udp.Client.Client.LocalEndPoint).Port);
		}
		
		public void Connected()
		{
			Tcp.NetStreamBinary.Write(ServerPacketTypeBase.ConnectionAck);
			SendUdpData(Tcp.NetStreamBinary);
			StringWriter.Write(Name, Tcp.NetStreamBinary);
		}

		public void Connect(string hostName, string name)
		{
			Name = name;

			Tcp.Connect(hostName);
			Udp.Connect(hostName);
		}

		public void Close()
		{
			if (Tcp.Connected)
			{
				Tcp.BatchStream.Write(ServerPacketTypeBase.Disconnected);
				Tcp.BatchStream.Write(PacketTypeBase.EndOfMessage);
				Tcp.Check();
			}

			Udp.Close();
			Tcp.Close();

			Udp = null;
			Tcp = null;
		}
	}
}