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
		public Form3()
		{
			InitializeComponent();
		}

		Networking.Server server;
		System.Timers.Timer timer = new System.Timers.Timer();

		private void Form3_Load(object sender, EventArgs e)
		{
			server = new Networking.Server();
			server.Host();

			timer.Interval = 20;
			timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
			timer.SynchronizingObject = this;
			timer.Start();
		}

		void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			server.Check();
		}

		private void Form3_FormClosing(object sender, FormClosingEventArgs e)
		{
			timer.Stop();
			server.Close();
		}
	}
}
