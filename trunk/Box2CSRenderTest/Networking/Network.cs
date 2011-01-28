using System;
using System.Collections.Generic;
using System.IO;

namespace Box2DSharpRenderTest.Networking
{
	public struct BinaryWrapper
	{
		byte[] _buffer;

		public Stream Stream
		{
			get;
			set;
		}

		public BinaryWrapper(Stream stream) :
			this()
		{
			Stream = stream;
			_buffer = new byte[32];
		}

		void SetBuffer(int amount)
		{
			Stream.Read(_buffer, 0, amount);
		}

		public bool ReadBoolean()
		{
			SetBuffer(1);
			return BitConverter.ToBoolean(_buffer, 0);
		}

		public byte ReadByte()
		{
			SetBuffer(1);
			return _buffer[0];
		}

		public char ReadChar()
		{
			SetBuffer(1);
			return BitConverter.ToChar(_buffer, 0);
		}

		public short ReadShort()
		{
			SetBuffer(2);
			return BitConverter.ToInt16(_buffer, 0);
		}

		public ushort ReadUShort()
		{
			SetBuffer(2);
			return BitConverter.ToUInt16(_buffer, 0);
		}

		public int ReadInt()
		{
			SetBuffer(4);
			return BitConverter.ToInt32(_buffer, 0);
		}

		public uint ReadUInt()
		{
			SetBuffer(4);
			return BitConverter.ToUInt32(_buffer, 0);
		}

		public long ReadLong()
		{
			SetBuffer(8);
			return BitConverter.ToInt64(_buffer, 0);
		}

		public ulong ReadULong()
		{
			SetBuffer(8);
			return BitConverter.ToUInt16(_buffer, 0);
		}

		public string ReadString()
		{
			return StringWriter.Read(this);
		}
	}

	public static class ClientPacketTypeBase
	{
		/// <summary>
		/// Got connection request, setting up player structure,
		/// now request more info (UDP address, name, etc).
		/// Also gives him time to precache.
		/// </summary>
		public const byte ConnectionAck = 0;

		/// <summary>
		/// Tells the player he has been spawned
		/// </summary>
		public const byte SpawnAck = 1;

		/// <summary>
		/// Player data packet.
		/// </summary>
		public const byte PlayerData = 2;

		public static class PlayerDataBits
		{
			public const int ActiveBit = (1 << 0);
			public const int NameBit = (1 << 1);
		}
	}

	public static class ServerPacketTypeBase
	{
		/// <summary>
		/// Received ConnectionAck; sending back UDP data + anything else
		/// </summary>
		public const byte ConnectionAck = 0;

		/// <summary>
		/// Change name
		/// </summary>
		public const byte ChangeName = 1;

		/// <summary>
		/// Disconnected from server
		/// </summary>
		public const byte Disconnected = 2;
	}

	public static class PacketTypeBase
	{
		/// <summary>
		/// End of a TCP message.
		/// </summary>
		public const byte EndOfMessage = 255;
	}

	/// <summary>
	/// Custom ASCII string writer
	/// </summary>
	public static class StringWriter
	{
		public static string Read(System.IO.BinaryReader reader)
		{
			string str = string.Empty;

			int length = reader.ReadInt32();

			for (int i = 0; i < length; ++i)
				str += reader.ReadChar();

			return str;
		}

		public static string Read(BinaryWrapper reader)
		{
			string str = string.Empty;

			int length = reader.ReadInt();

			for (int i = 0; i < length; ++i)
				str += reader.ReadChar();

			return str;
		}

		public static void Write(string str, System.IO.BinaryWriter writer)
		{
			writer.Write(str.Length);

			foreach (var x in str)
				writer.Write((byte)(unchecked((byte)x)));
		}
	}

	public delegate void NetworkReceivePacket(byte packetType, BinaryWrapper reader, System.Net.IPEndPoint endPoint);
	public delegate void TCPNetworkReceivePacket(byte packetType, BinaryWrapper reader, Player player);
	public delegate void ServerSendPlayerData(Player player, BinaryWriter writer);
	public delegate void ClientRecievePlayerData(ConnectedPlayer player, BinaryWrapper reader);
}
