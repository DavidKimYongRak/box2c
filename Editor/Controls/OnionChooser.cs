using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Box2CS;

namespace Editor.Controls
{
	public partial class OnionChooser : Form
	{
		class OnionIndex
		{
			public int Index;
			public string Name;
			public PolygonPanelData Data;

			public override string  ToString()
			{
 					return Name;
			}

			public OnionIndex(string name, int index, PolygonPanelData data)
			{
				Name = name;
				Index = index;
				Data = data;
			}
		}

		public int SelectedFixtureIndex
		{
			get
			{
				return (listBox1.SelectedIndex == -1) ? -1 : (listBox1.Items[listBox1.SelectedIndex] as OnionIndex).Index;
			}
		}

		public OnionChooser()
		{
			InitializeComponent();

			listBox1.Items.Add(new OnionIndex("None", -1, null));

			for (int i = 0; i < Main.WorldObject.Fixtures.Count; ++i)
			{
				var fixture = Main.WorldObject.Fixtures[i];

				if (fixture.ShapeNode.Shape is PolygonShape)
					listBox1.Items.Add(new OnionIndex(fixture.Name, i, (fixture.ShapeNode.Data as PolygonPanelData)));
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex == -1 ||
				(listBox1.Items[listBox1.SelectedIndex] as OnionIndex).Index == -1)
			{
				pictureBox1.VerticeList = null;
				pictureBox1.Invalidate();
				return;
			}

			pictureBox1.VerticeList = (listBox1.Items[listBox1.SelectedIndex] as OnionIndex).Data.Vertices;
			pictureBox1.Invalidate();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
	}
}
