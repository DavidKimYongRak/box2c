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
				vertices[i] = (new Vec2(((pictureBox1.Width / 2) - PolyData.Vertices[i].X) / pictureBox1.Width, ((pictureBox1.Height / 2) - PolyData.Vertices[i].Y) / pictureBox1.Height)) * (float)PolyData.Scale;
			}

			SelectedShape.Vertices = vertices;
		}

		public PolygonPanelData PolyData
		{
			get { return Program.MainForm.SelectedNode.ShapeNode.Data as PolygonPanelData; }
		}

		int _movingVertice = -1, _hoverVertice = -1;

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (_hoverVertice != -1)
					_movingVertice = _hoverVertice;
				else
				{
					PolyData.Vertices.Add(new Vec2(Snap(e.X), Snap(pictureBox1.Height - e.Y)));
					_movingVertice = PolyData.Vertices.Count - 1;

					DataGridViewRow row = new DataGridViewRow();
					//dataGridView1.Rows.Add(row);
					//dataGridView1.Rows[_movingVertice].SetValues(PolyData.Vertices[_movingVertice].X / pictureBox1.Width, PolyData.Vertices[_movingVertice].Y / pictureBox1.Height);
				}
			}

			pictureBox1.Invalidate();
		}

		private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
		{
			if (_movingVertice != -1)
			{
				PolyData.Vertices[_movingVertice] = new Vec2(Snap(e.X), Snap(pictureBox1.Height - e.Y));
				//dataGridView1.Rows[_movingVertice].SetValues(PolyData.Vertices[_movingVertice].X / pictureBox1.Width, PolyData.Vertices[_movingVertice].Y / pictureBox1.Height);
				pictureBox1.Invalidate();
			}
			else
			{
				int oldVert = _hoverVertice;

				_hoverVertice = -1;
				int i = 0;
				foreach (var x in PolyData.Vertices)
				{
					Rectangle rect = new Rectangle(e.X - 3, e.Y - 3, 7, 7);

					if (rect.Contains((int)x.X, (int)(pictureBox1.Height - x.Y)))
					{
						_hoverVertice = i;
						break;
					}

					i++;
				}

				if (oldVert != _hoverVertice)
					pictureBox1.Invalidate();
			}
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

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			_movingVertice = -1;

			// validate
			ValidatePolygon();
		}

		void ShowError(string text, string tooltip)
		{
			statusStrip1.ShowItemToolTips = true;
			toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			toolStripStatusLabel1.Text = text;
			toolStripStatusLabel1.AutoToolTip = true;
			toolStripStatusLabel1.ToolTipText = tooltip;
		}

		void HideError()
		{
			toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.Text;
			toolStripStatusLabel1.Text = "";
		}

		const float VertSize = 4.0f;
		void DrawVert(Graphics g, Vec2 pos, bool hover)
		{
			g.FillRectangle((hover) ? Brushes.Tomato : Brushes.White, new RectangleF(pos.X - (VertSize / 2), pictureBox1.Height - pos.Y - (VertSize / 2), VertSize, VertSize));
		}

		void DrawVertConnect(Graphics g, Vec2 l, Vec2 r, bool last)
		{
			g.DrawLine(new Pen((last) ? Color.ForestGreen : Color.Aqua, 2), new PointF(l.X, pictureBox1.Height - l.Y), new PointF(r.X, pictureBox1.Height - r.Y));
		}

		float _gridSize = 2;
		void DrawGrid(Graphics g, Color c)
		{
			var pen = new Pen(c, 1);

			for (int x = 0; x < pictureBox1.Width; x += (int)_gridSize)
				g.DrawLine(pen, new Point(x, 0), new Point(x, pictureBox1.Height));

			for (int x = 0; x < pictureBox1.Height; x += (int)_gridSize)
				g.DrawLine(pen, new Point(0, x), new Point(pictureBox1.Width, x));
		}

		float Snap(float Value)
		{
			if (Value < 0)
				Value = 0;
			if (Value > pictureBox1.Width)
				Value = pictureBox1.Width;
			if (_gridSize == 0)
				return Value;

			if (Math.Abs(Math.Floor(Value/_gridSize)-(Value/_gridSize))<Math.Abs(Math.Ceiling(Value/_gridSize)-(Value/_gridSize)))
				return (float)Math.Floor((Value/_gridSize))*_gridSize;
			return (float)Math.Ceiling((Value/_gridSize))*_gridSize;
		}

		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{
			DrawGrid(e.Graphics, Color.FromArgb(127, Color.Chocolate));

			var g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			if (PolyData.Vertices.Count > 2)
			{
				PointF[] polygon = new PointF[PolyData.Vertices.Count];

				for (int i = 0; i < PolyData.Vertices.Count; ++i)
					polygon[i] = new PointF(PolyData.Vertices[i].X, pictureBox1.Height - PolyData.Vertices[i].Y);

				using (GraphicsPath path = new GraphicsPath())
				{
					path.AddPolygon(polygon);
					g.FillPath(new SolidBrush(Color.FromArgb(128, Color.ForestGreen)), path);
				}
			}

			for (int i = 0; i < PolyData.Vertices.Count; ++i)
			{
				DrawVert(g, PolyData.Vertices[i], (i == _hoverVertice));

				if (i == PolyData.Vertices.Count - 1)
				{
					if (PolyData.Vertices.Count != 2)
						DrawVertConnect(g, PolyData.Vertices[i], PolyData.Vertices[0], true);
				}
				else
					DrawVertConnect(g, PolyData.Vertices[i], PolyData.Vertices[i + 1], false);
			}
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
