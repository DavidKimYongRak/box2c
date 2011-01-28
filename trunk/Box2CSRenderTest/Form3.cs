using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Box2DSharpRenderTest
{
	public partial class Form3 : Form
	{
		static class MyClientPackets
		{
			public const byte ServerChat = 3;
			public const byte ClientChat = 3;
		}

		public Form3()
		{
			InitializeComponent();
		}

		Networking.Server server;
		Networking.Client client;
		System.Timers.Timer timer = new System.Timers.Timer();

		private void Form3_Load(object sender, EventArgs e)
		{
			timer.Interval = 20;
			timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
			timer.SynchronizingObject = this;
			timer.Start();
		}

		void Udp_PacketReceived(byte packetType, Networking.BinaryWrapper reader, System.Net.IPEndPoint endPoint)
		{
			textBox2.Text += "(DEBUG: Client: Received UDP "+packetType.ToString()+")"+Environment.NewLine;
		}

		void Tcp_PacketReceived(byte packetType, Networking.BinaryWrapper reader, System.Net.IPEndPoint endPoint)
		{
			textBox2.Text += "(DEBUG: Client: Received TCP "+packetType.ToString()+")"+Environment.NewLine;

			if (packetType == Networking.ClientPacketTypeBase.ConnectionAck)
			{
				client.Tcp.NetStreamBinary.Write(Networking.PacketTypeBase.EndOfMessage);
			}
			else if (packetType == MyClientPackets.ClientChat)
			{
				var player = client.Players[reader.ReadInt()];
				textBox2.Text += player.Name+": " + Networking.StringWriter.Read(reader)+Environment.NewLine;
			}
			else if (packetType == Networking.ClientPacketTypeBase.PlayerData)
			{
				foreach (var p in client.Players)
				{
					if (listBox1.Items.Count <= p.Index)
						listBox1.Items.Add("");

					if (!p.Active)
						listBox1.Items[p.Index] = "(Disconnected)";
					else
						listBox1.Items[p.Index] = p.Name;
				}
			}
		}

		void server_ReceiveUDPData(byte packetType, Networking.BinaryWrapper reader, System.Net.IPEndPoint endPoint)
		{
			textBox2.Text += "(DEBUG: Server: Received UDP "+packetType.ToString()+")"+Environment.NewLine;
		}

		void server_ReceiveTCPData(byte packetType, Networking.BinaryWrapper reader, Networking.Player player)
		{
			if (packetType == Networking.ServerPacketTypeBase.ConnectionAck)
			{
				server.AcceptPlayer(player);
			}
			else if (packetType == MyClientPackets.ServerChat)
			{
				foreach (var p in server.Players)
				{
					// if p == player continue?
					p.Tcp.BatchStream.Write(MyClientPackets.ClientChat);
					p.Tcp.BatchStream.Write(player.Index);
					Networking.StringWriter.Write(Networking.StringWriter.Read(reader), p.Tcp.BatchStream);
					p.Tcp.BatchStream.Write(Networking.PacketTypeBase.EndOfMessage);
				}
			}

			textBox2.Text += "(DEBUG: Server: Received TCP "+packetType.ToString()+")"+Environment.NewLine;
		}

		void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (server != null)
				server.Check();

			if (client != null)
				client.Check();
		}

		private void Form3_FormClosing(object sender, FormClosingEventArgs e)
		{
			timer.Stop();
			
			if (server != null)
				server.Close();

			if (client != null)
				client.Close();
		}

		private void hostToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (server != null)
			{
				server.Close();
				server = null;
				return;
			}

			server = new Networking.Server();
			server.Host();

			server.ReceiveTCPData += new Networking.TCPNetworkReceivePacket(server_ReceiveTCPData);
			server.ReceiveUDPData += new Networking.NetworkReceivePacket(server_ReceiveUDPData);
		}

		private void connectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (client != null)
			{
				client.Close();
				client = null;
				return;
			}

			using (IPDlg dlg = new IPDlg())
			{
				if (dlg.ShowDialog() != DialogResult.OK)
					return;

				if (string.IsNullOrEmpty(dlg.IP) || string.IsNullOrEmpty(dlg.PlayerName))
					return;

				client = new Networking.Client();

				client.Tcp.PacketReceived += new Networking.NetworkReceivePacket(Tcp_PacketReceived);
				client.Udp.PacketReceived += new Networking.NetworkReceivePacket(Udp_PacketReceived);

				client.Connect(dlg.IP, dlg.PlayerName);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBox1.Text))
				return;

			client.Tcp.BatchStream.Write(MyClientPackets.ServerChat);
			Networking.StringWriter.Write(textBox1.Text, client.Tcp.BatchStream);
			client.Tcp.BatchStream.Write(Networking.PacketTypeBase.EndOfMessage);

			textBox1.Clear();
		}
	}
}
