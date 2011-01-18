using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Box2CS;
using System.Drawing.Drawing2D;
using FarseerPhysics.Common.Decomposition;

namespace Editor
{
	public partial class PolygonPanel : UserControl
	{
		public PolygonPanel()
		{
			InitializeComponent();
			HideError();
		}

		public PolygonShape SelectedShape
		{
			get { return (PolygonShape)Program.MainForm.SelectedNode.ShapeNode.Shape; }
		}

		public void Added()
		{
			LoadPolygonShapeData();
		}

		public void LoadPolygonShapeData()
		{
			//dataGridView1.Rows.Clear();

			floatNumericUpDown1.Value = PolyData.Scale;

			for (int i = 0; i < PolyData.Vertices.Count; ++i)
			{
				//DataGridViewRow row = new DataGridViewRow();
				//dataGridView1.Rows.Add(row);
				//dataGridView1.Rows[i].SetValues(PolyData.Vertices[i].X / pictureBox1.Width, PolyData.Vertices[i].Y / pictureBox1.Height);
			}

			pictureBox1.VerticeList = PolyData.Vertices;
			pictureBox1.Invalidate();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(toolStripStatusLabel1.Text))
			{
				MessageBox.Show("You have errors in your polygon. Please fix these before applying:\n\n"+toolStripStatusLabel1.ToolTipText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			Vec2[] vertices = new Vec2[PolyData.Vertices.Count];

			for (int i = 0; i < PolyData.Vertices.Count; ++i)
			{
				vertices[i] = (new Vec2((PolyData.Vertices[i].X - (pictureBox1.Width / 2)) / pictureBox1.Width, (PolyData.Vertices[i].Y - (pictureBox1.Height / 2)) / pictureBox1.Height)) * (float)PolyData.Scale;
			}

			SelectedShape.Vertices = vertices;

			if ((Program.MainForm.SelectedNode.Node.Parent as FixtureNode).OwnedBody != null &&
				(Program.MainForm.SelectedNode.Node.Parent as FixtureNode).OwnedBody.AutoMassRecalculate)
				(Program.MainForm.SelectedNode.Node.Parent as FixtureNode).OwnedBody.RecalculateMass();
		}

		public PolygonPanelData PolyData
		{
			get { return Program.MainForm.SelectedNode.ShapeNode.Data as PolygonPanelData; }
		}

		void ValidatePolygon()
		{
			HideError();

			if (PolyData.Vertices.Count <= 1 || PolyData.Vertices.Count > Box2DSettings.b2_maxPolygonVertices)
				ShowError("Polygon vertice count", "Polygons can only contain between 2 and 8 vertices.\nNote that a polygon of 2 vertices is called an edge (Box2D 2.1.2).");
			else if (PolyData.Vertices.Count > 2)
			{
				// Compute normals. Ensure the edges have non-zero length.
				for (int i = 0; i < PolyData.Vertices.Count; ++i)
				{
					int i1 = i;
					int i2 = i + 1 < PolyData.Vertices.Count ? i + 1 : 0;
					Vec2 edge = PolyData.Vertices[i2] - PolyData.Vertices[i1];

					if (!(edge.LengthSquared() > float.Epsilon * float.Epsilon))
					{
						ShowError("Edge length", "An edge has an incredibly small length. This may be due\nto vertices too close together.");
						return;
					}
				}

				for (int i = 0; i < PolyData.Vertices.Count; ++i)
				{
					int i1 = i;
					int i2 = i + 1 < PolyData.Vertices.Count ? i + 1 : 0;
					Vec2 edge = PolyData.Vertices[i2] - PolyData.Vertices[i1];

					for (int j = 0; j < PolyData.Vertices.Count; ++j)
					{
						// Don't check vertices on the current edge.
						if (j == i1 || j == i2)
						{
							continue;
						}

						Vec2 r = PolyData.Vertices[j] - PolyData.Vertices[i1];

						// Your polygon is non-convex (it has an indentation) or
						// has colinear edges.
						float s = edge.Cross(r);
						if (!(s > 0.0f))
						{
							ShowError("Non-convex/Collinear", "Your polygon is non-convex or has collinear edges.\nTry using the \"Reverse\" button; if that doesn't work,\nthe problem lies within the spacing of your vertices.");
							return;
						}
					}
				}

				Vec2 centroid;
				try
				{
					PolygonShape.ComputeCentroid(out centroid, PolyData.Vertices);
				}
				catch
				{
					ShowError("Polygon area", "The area of your polygon is incredibly small.");
				}
			}
		}

		void ShowError(string text, string tooltip)
		{
			statusStrip1.ShowItemToolTips = true;
			toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			toolStripStatusLabel1.Text = text;
			toolStripStatusLabel1.AutoToolTip = true;
			toolStripStatusLabel1.ToolTipText = tooltip;
		}

		void pictureBox1_ValidatePolygon(object sender, System.EventArgs e)
		{
			ValidatePolygon();
		}

		void HideError()
		{
			toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.Text;
			toolStripStatusLabel1.Text = "";
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			PolyData.Vertices.Reverse();

			//for (int i = 0; i < PolyData.Vertices.Count; ++i)
				//dataGridView1.Rows[i].SetValues((PolyData.Vertices[i].X) / pictureBox1.Width, (PolyData.Vertices[i].Y) / pictureBox1.Height);

			pictureBox1.Invalidate();
			ValidatePolygon();
		}

		private void toolStripButton5_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < PolyData.Vertices.Count; ++i)
			{
				PolyData.Vertices[i] = new Vec2(pictureBox1.Width - PolyData.Vertices[i].X, PolyData.Vertices[i].Y);
				//dataGridView1.Rows[i].SetValues((PolyData.Vertices[i].X) / pictureBox1.Width, (PolyData.Vertices[i].Y) / pictureBox1.Height);
			}

			pictureBox1.Invalidate();
			ValidatePolygon();
		}

		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < PolyData.Vertices.Count; ++i)
			{
				PolyData.Vertices[i] = new Vec2(PolyData.Vertices[i].X, pictureBox1.Height - PolyData.Vertices[i].Y);
				//dataGridView1.Rows[i].SetValues((PolyData.Vertices[i].X) / pictureBox1.Width, (PolyData.Vertices[i].Y) / pictureBox1.Height);
			}

			pictureBox1.Invalidate();
			ValidatePolygon();
		}

		private void floatNumericUpDown1_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			PolyData.Scale = e.NewValue;
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			{
				var oldFirst = PolyData.Vertices[0];
				PolyData.Vertices.RemoveAt(0);
				PolyData.Vertices.Add(oldFirst);
			}
			{
				//var oldFirst = dataGridView1.Rows[0];
				//dataGridView1.Rows.RemoveAt(0);
				//dataGridView1.Rows.Add(oldFirst);
			}

			pictureBox1.Invalidate();
		}

		private void toolStripButton6_Click(object sender, EventArgs e)
		{
			{
				var oldLast = PolyData.Vertices[PolyData.Vertices.Count - 1];
				PolyData.Vertices.RemoveAt(PolyData.Vertices.Count - 1);
				PolyData.Vertices.Insert(0, oldLast);
			}
			{
				//var oldLast = dataGridView1.Rows[dataGridView1.Rows.Count - 1];
				//dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1);
				//dataGridView1.Rows.Insert(0, oldLast);
			}

			pictureBox1.Invalidate();
		}

		private void toolStripButton7_Click(object sender, EventArgs e)
		{
			using (Editor.Controls.OnionChooser chooser = new Controls.OnionChooser())
			{
				if (chooser.ShowDialog() == DialogResult.OK)
				{
					var index = chooser.SelectedFixtureIndex;

					if (index == -1)
					{
					}
				}
			}
		}
	}

	public class PolygonPlotter : Control
	{
		public PolygonPlotter()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserMouse |
				ControlStyles.UserPaint |
				ControlStyles.OptimizedDoubleBuffer,
				true);

			GridSize = 2;
			VertSize = 4.0f;
			MovingVertice = HoverVertice = -1;
		}

		public IList<Vec2> VerticeList
		{
			get;
			set;
		}

		int VertAtPos(Point pt)
		{
			int vertice = -1;
			int i = 0;

			foreach (var x in VerticeList)
			{
				Rectangle rect = new Rectangle(pt.X - 3, pt.Y - 3, 7, 7);

				if (rect.Contains((int)x.X, (int)(Height - x.Y)))
				{
					vertice = i;
					break;
				}

				i++;
			}

			return vertice;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (VerticeList == null || !Enabled)
				return;

			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (HoverVertice != -1)
					MovingVertice = HoverVertice;
				else
				{
					VerticeList.Add(new Vec2(Snap(e.X), Snap(Height - e.Y)));
					MovingVertice = VerticeList.Count - 1;

					DataGridViewRow row = new DataGridViewRow();
					//dataGridView1.Rows.Add(row);
					//dataGridView1.Rows[_movingVertice].SetValues(PolyData.Vertices[_movingVertice].X / pictureBox1.Width, PolyData.Vertices[_movingVertice].Y / pictureBox1.Height);
				}
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				if (HoverVertice != -1)
				{
					VerticeList.RemoveAt(HoverVertice);
					HoverVertice = VertAtPos(e.Location);
				}
			}

			Invalidate();
		}

		public event EventHandler ValidatePolygon;

		protected virtual void OnValidatePolygon()
		{
			if (ValidatePolygon != null)
				ValidatePolygon(this, EventArgs.Empty);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (VerticeList == null || !Enabled)
				return;

			MovingVertice = -1;

			// validate
			OnValidatePolygon();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (VerticeList == null || !Enabled)
				return;

			if (MovingVertice != -1)
			{
				VerticeList[MovingVertice] = new Vec2(Snap(e.X), Snap(Height - e.Y));
				Invalidate();
			}
			else
			{
				int oldVert = HoverVertice;

				HoverVertice = VertAtPos(e.Location);

				if (oldVert != HoverVertice)
					Invalidate();
			}
		}

		public float GridSize
		{
			get;
			set;
		}

		public float VertSize
		{
			get;
			set;
		}

		public int HoverVertice
		{
			get;
			set;
		}

		public int MovingVertice
		{
			get;
			set;
		}

		void DrawVert(Graphics g, Vec2 pos, bool hover)
		{
			g.FillRectangle((hover) ? Brushes.Tomato : Brushes.White, new RectangleF(pos.X - (VertSize / 2), Height - pos.Y - (VertSize / 2), VertSize, VertSize));
		}

		void DrawVertConnect(Graphics g, Vec2 l, Vec2 r, bool last)
		{
			g.DrawLine(new Pen((last) ? Color.ForestGreen : Color.Aqua, 2), new PointF(l.X, Height - l.Y), new PointF(r.X, Height - r.Y));
		}

		void DrawGrid(Graphics g, Color c)
		{
			var pen = new Pen(c, 1);

			for (float x = 0; x < Width; x += GridSize)
				g.DrawLine(pen, new PointF(x, 0), new PointF(x, Height));

			for (float x = 0; x < Height; x += GridSize)
				g.DrawLine(pen, new PointF(0, x), new PointF(Width, x));
		}

		float Snap(float Value)
		{
			if (Value < 0)
				Value = 0;
			if (Value > Width)
				Value = Width;
			if (GridSize == 0)
				return Value;

			if (Math.Abs(Math.Floor(Value/GridSize)-(Value/GridSize))<Math.Abs(Math.Ceiling(Value/GridSize)-(Value/GridSize)))
				return (float)Math.Floor((Value/GridSize))*GridSize;
			return (float)Math.Ceiling((Value/GridSize))*GridSize;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			DrawGrid(e.Graphics, Color.FromArgb(127, Color.Chocolate));

			if (VerticeList == null)
				return;

			var g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			if (VerticeList.Count > 2)
			{
				PointF[] polygon = new PointF[VerticeList.Count];

				for (int i = 0; i < VerticeList.Count; ++i)
					polygon[i] = new PointF(VerticeList[i].X, Height - VerticeList[i].Y);

				using (GraphicsPath path = new GraphicsPath())
				{
					path.AddPolygon(polygon);
					g.FillPath(new SolidBrush(Color.FromArgb(128, Color.ForestGreen)), path);
				}
			}

			for (int i = 0; i < VerticeList.Count; ++i)
			{
				DrawVert(g, VerticeList[i], (i == HoverVertice));

				if (i == VerticeList.Count - 1)
				{
					if (VerticeList.Count != 2)
						DrawVertConnect(g, VerticeList[i], VerticeList[0], true);
				}
				else
					DrawVertConnect(g, VerticeList[i], VerticeList[i + 1], false);
			}
		}
	}

	public class PolygonPanelData
	{
		public List<Vec2> Vertices
		{
			get;
			internal set;
		}

		public decimal Scale
		{
			get;
			set;
		}

		public PolygonPanelData()
		{
			Vertices = new List<Vec2>();
			Scale = 1;
		}
	}
}
