using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Box2CS.Parsers;
using FarseerPhysics.Common;
using Box2CS;
using System.Drawing.Drawing2D;

namespace Editor
{
	public partial class Decomposer : Form
	{
		public Decomposer()
		{
			InitializeComponent();
		}

		private void Decomposer_Load(object sender, EventArgs e)
		{
			listBox1.SelectedIndex = 0;
		}

		const float VertSize = 2.5f;
		void DrawVert(Graphics g, Vec2 pos)
		{
			g.FillRectangle(Brushes.White, new RectangleF(pos.X - (VertSize / 2), pictureBox1.Height - pos.Y - (VertSize / 2), VertSize, VertSize));
		}

		void DrawVertConnect(Graphics g, Vec2 l, Vec2 r)
		{
			g.DrawLine(new Pen(Color.Aqua, 2), new PointF(l.X, pictureBox1.Height - l.Y), new PointF(r.X, pictureBox1.Height - r.Y));
		}

		PolyParser polygon;

		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{
			if (polygon == null)
				return;

			var g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			for (int i = 0; i < polygon.PolyChains.Count; ++i)
			{
				using (GraphicsPath path = new GraphicsPath())
				{
					List<PointF> points = new List<PointF>();

					for (int x = 0; x < polygon.PolyChains[i].Connections.Count; ++x)
					{
						/*DrawVert(g, verts[i]);

						if (i == verts.Count - 1)
						{
							if (verts.Count != 2)
								DrawVertConnect(g, verts[i], verts[0]);
						}
						else
							DrawVertConnect(g, verts[i], verts[i + 1]);*/
						var p = polygon.PolyChains[i].Vertices[polygon.PolyChains[i].Connections[x] - 1];
						p.X *= pictureBox1.Width;
						p.Y *= pictureBox1.Height;
						p.Y = pictureBox1.Height - p.Y;
						points.Add(p);
					}

					path.AddPolygon(points.ToArray());
					g.DrawPath(Pens.Aqua, path);
				}
			}

			if (_decomposedVerts != null)
			{
				foreach (var v in _decomposedVerts)
				{
					using (GraphicsPath path = new GraphicsPath())
					{
						List<PointF> points = new List<PointF>();

						for (int x = 0; x < v.Count; ++x)
						{
							var p = v[x];
							p.X *= pictureBox1.Width;
							p.Y *= pictureBox1.Height;
							p.Y = pictureBox1.Height - p.Y;
							points.Add(new PointF(p.X, p.Y));
						}

						path.AddPolygon(points.ToArray());
						g.DrawPath(Pens.Blue, path);
					}
				}
			}
		}

		public static void OpenDialog()
		{
			using (Decomposer poser = new Decomposer())
				poser.ShowDialog();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = "Polygons|*.poly";

				if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					polygon = PolyParser.LoadFromFile(ofd.FileName);
					pictureBox1.Invalidate();
				}
				else
					return;
			}

			label2.Text = "Vertices: " + polygon.VertCount.ToString();
		}

		List<Vertices> _decomposedVerts;
		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		
		private void decomposeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Vertices verts = new Vertices();

			for (int i = 0; i < polygon.PolyChains.Count; ++i)
			{
				for (int x = 0; x < polygon.PolyChains[i].Connections.Count; ++x)
				{
					var p = polygon.PolyChains[i].Vertices[polygon.PolyChains[i].Connections[x] - 1];
					verts.Add(new Microsoft.Xna.Framework.Vector2(p.X, p.Y));
				}
			}

			sw.Reset();
			sw.Start();
			switch (listBox1.SelectedIndex)
			{
			case 0:
				{
					_decomposedVerts = FarseerPhysics.Common.Decomposition.CDTDecomposer.ConvexPartition(verts);
					
					FarseerPhysics.Common.Decomposition.Triangle[] triangles = new FarseerPhysics.Common.Decomposition.Triangle[_decomposedVerts.Count];

					for (int x = 0; x < triangles.Length; ++x)
						triangles[x] = new FarseerPhysics.Common.Decomposition.Triangle(_decomposedVerts[x][0].X, _decomposedVerts[x][0].Y, _decomposedVerts[x][1].X, _decomposedVerts[x][1].Y, _decomposedVerts[x][2].X, _decomposedVerts[x][2].Y);

					_decomposedVerts = FarseerPhysics.Common.Decomposition.EarclipDecomposer.PolygonizeTriangles(triangles.ToList(), 100000, 0.001f);
				}
				break;
			case 1:
				{
					_decomposedVerts = FarseerPhysics.Common.Decomposition.BayazitDecomposer.ConvexPartition(verts);
				}
				break;
			case 2:
				{
					_decomposedVerts = FarseerPhysics.Common.Decomposition.EarclipDecomposer.ConvexPartition(verts);
				}
				break;
			case 3:
				{
					_decomposedVerts = FarseerPhysics.Common.Decomposition.FlipcodeDecomposer.ConvexPartition(verts);
				}
				break;
			case 4:
				{
					_decomposedVerts = FarseerPhysics.Common.Decomposition.SeidelDecomposer.ConvexPartition(verts, 0.001f);

					FarseerPhysics.Common.Decomposition.Triangle[] triangles = new FarseerPhysics.Common.Decomposition.Triangle[_decomposedVerts.Count];

					for (int x = 0; x < triangles.Length; ++x)
						triangles[x] = new FarseerPhysics.Common.Decomposition.Triangle(_decomposedVerts[x][0].X, _decomposedVerts[x][0].Y, _decomposedVerts[x][1].X, _decomposedVerts[x][1].Y, _decomposedVerts[x][2].X, _decomposedVerts[x][2].Y);

					_decomposedVerts = FarseerPhysics.Common.Decomposition.EarclipDecomposer.PolygonizeTriangles(triangles.ToList(), 100000, 0.001f);
				}
				break;
			}
			sw.Stop();

			label3.Text = "Num Polygons: " + _decomposedVerts.Count.ToString();

			int count = 0;
			foreach (var x in _decomposedVerts)
				count += x.Count;

			label4.Text = "Total Vertices: "+count.ToString();
			label1.Text = "Time: " + sw.ElapsedMilliseconds.ToString() + "ms";

			pictureBox1.Invalidate();
		}
	}

	public class PolyChain
	{
		public List<PointF> Vertices = new List<PointF>();
		public List<int> Connections = new List<int>();
		public bool In;
	}

	public struct BoundsF
	{
		public PointF Mins
		{
			get;
			set;
		}

		public PointF Maxs
		{
			get;
			set;
		}

		public void Merge(PointF point)
		{
			if (point.X < Mins.X)
				Mins = new PointF(point.X, Mins.Y);
			if (point.Y < Mins.Y)
				Mins = new PointF(Mins.X, point.Y);

			if (point.X > Maxs.X)
				Maxs = new PointF(point.X, Maxs.Y);
			if (point.Y > Maxs.Y)
				Maxs = new PointF(Maxs.X, point.Y);
		}
	}

	public class PolyParser
	{
		FileBuffer _buffer;
		static char[] _punctuation = new char[] { };
		static char lineComment = '#';

		public PolyParser(Stream stream)
		{
			Load(stream);
		}

		public PolyParser(string fileName)
		{
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				Load(fs);
		}

		public void Load(Stream stream)
		{
			_buffer = new FileBuffer(stream);
		}

		public static PolyParser LoadFromFile(string fileName)
		{
			var x = new PolyParser(fileName);
			x.Parse();
			return x;
		}

		public static PolyParser LoadFromStream(Stream stream)
		{
			var x = new PolyParser(stream);
			x.Parse();
			return x;
		}

		string NextToken()
		{
			string str = "";
			bool _done = false;

			while (true)
			{
				char c = (char)_buffer.Next;

				if (lineComment == c)
				{
					ReadUntil('\n');
					_buffer.Position++;
					continue;
				}
				else if (_punctuation.Contains<char>(c))
				{
					if (str != "")
					{
						_buffer.Position--;
						break;
					}

					_done = true;
				}
				else if (char.IsWhiteSpace(c))
				{
					if (str != "")
						break;
					else
						continue;
				}

				str += c;

				if (_done)
					break;
			}

			str = TrimControl(str);

			// Trim quotes from start and end
			if (str[0] == '\"')
				str = str.Remove(0, 1);

			if (str[str.Length - 1] == '\"')
				str = str.Remove(str.Length - 1, 1);

			return str;
		}

		string PeekToken()
		{
			var oldPos = _buffer.Position;
			var str = NextToken();
			_buffer.Position = oldPos;
			return str;
		}

		string NextLine()
		{
			string line = ReadUntil('\n');
			_buffer.Position++;
			return line;
		}

		string ReadUntil(char c)
		{
			string str = "";

			while (true)
			{
				char ch = (char)_buffer.Next;

				if (ch == c)
				{
					if (string.IsNullOrEmpty(str))
						continue;

					_buffer.Position--;
					break;
				}

				str += ch;
			}

			// Trim quotes from start and end
			if (str[0] == '\"')
				str = str.Remove(0, 1);

			if (str[str.Length - 1] == '\"')
				str = str.Remove(str.Length - 1, 1);

			return str;
		}

		string TrimControl(string str)
		{
			string newStr = str;

			// Trim control characters
			int i = 0;
			while (true)
			{
				if (i == newStr.Length)
					break;

				if (char.IsControl(newStr[i]))
					newStr = newStr.Remove(i, 1);
				else
					i++;
			}

			return newStr;
		}

		public List<PolyChain> PolyChains = new List<PolyChain>();
		public int VertCount;

		public void Parse()
		{
			int polychains = int.Parse(NextToken());
			BoundsF bounds = new BoundsF();
			VertCount = 0;

			for (int i = 0; i < polychains; ++i)
			{
				var chain = new PolyChain();
				
				int verts = int.Parse(NextToken());
				VertCount += verts;
				chain.In = (NextToken() == "in");

				for (int x = 0; x < verts; ++x)
				{
					var vert = new PointF(float.Parse(NextToken()), float.Parse(NextToken()));
					chain.Vertices.Add(vert);

					bounds.Merge(vert);
				}

				for (int x = 0; x < verts; ++x)
				{
					var vert = chain.Vertices[x];
					vert.X -= bounds.Mins.X;
					vert.Y -= bounds.Mins.Y;

					vert.X /= bounds.Maxs.X - bounds.Mins.X;
					vert.Y /= bounds.Maxs.Y - bounds.Mins.X;

					if (vert.X < 0 || vert.Y < 0 || vert.X > 1 || vert.X > 1)
						throw new Exception();
					chain.Vertices[x] = vert;
					chain.Connections.Add(int.Parse(NextToken()));
				}

				PolyChains.Add(chain);
			}
		}
	}
}
