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
	public partial class IPDlg : Form
	{
		public IPDlg()
		{
			InitializeComponent();
		}

		public string IP
		{
			get { return textBox1.Text; }
		}

		public string PlayerName
		{
			get { return textBox2.Text; }
		}

		private void IPDlg_Load(object sender, EventArgs e)
		{

		}
	}
}
