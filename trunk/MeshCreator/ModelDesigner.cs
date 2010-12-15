using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Box2CS;

namespace MeshCreator
{
	public partial class ModelDesigner : Form
	{
		public static Random random = new Random();
		public ModelDesigner()
		{
			InitializeComponent();
		}

		public List<Mesh> _meshes = new List<Mesh>();
		public Mesh _currentMesh;

		public Vec2 placingVec = new Vec2();
		public Bitmap RefImage;
		public int MovingVertex = -1;
		public Point ClickPos = new Point(), LastMovePos = new Point();

		public void AddMesh(Mesh mesh)
		{
			_meshes.Add(mesh);
			treeView1.Nodes.Add("Mesh");
		}

		public void RemoveMesh(Mesh mesh)
		{
			treeView1.Nodes.RemoveAt(_meshes.IndexOf(mesh));
			_meshes.Remove(mesh);
		}

		public void DrawGrid(Graphics e)
		{
			if (GridSizeIndex == 0)
				return;

			Color semiGray = Color.FromArgb(75, Color.Gray);

			for (int x = 0; x < pictureBox1.Width; x += GridSizes[GridSizeIndex])
			{
				e.DrawLine(new Pen(semiGray, 1), new Point(x, 0), new Point(x, pictureBox1.Height));
			}

			for (int x = 0; x < pictureBox1.Width; x += GridSizes[GridSizeIndex])
			{
				e.DrawLine(new Pen(semiGray, 1), new Point(0, x), new Point(pictureBox1.Width, x));
			}
		}

		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{
			if (RefImage != null)
				e.Graphics.DrawImage(RefImage, new Point(RefImage.Width / 2, RefImage.Height / 2));

			DrawGrid(e.Graphics);

			foreach (var mesh in _meshes)
			{
				var vecs = mesh.Vertices;
				var lineColor = (mesh == _currentMesh) ? Color.Blue : Color.Black;
				try
				{
					ConvexDecomposition.Polygon p = new ConvexDecomposition.Polygon();

					foreach (var b in vecs)
					{
						p.x.Add(b.x);
						p.y.Add(b.y);
					}

					List<ConvexDecomposition.Polygon> results = new List<ConvexDecomposition.Polygon>();
					ConvexDecomposition.Statics.DecomposeConvex(p, results, 24);

					foreach (var res in results)
					{
						List<Vec2> v = res.GetVertexVecs();

						for (int i = 0; i < v.Count; ++i)
						{
							e.Graphics.FillRectangle(Brushes.Black, v[i].x - 1, v[i].y - 1, 3, 3);

							int nextVec = i + 1;
							if (nextVec == v.Count)
								nextVec = 0;
							e.Graphics.DrawLine(new Pen(Color.Goldenrod, 2), v[i].x, v[i].y, v[nextVec].x, v[nextVec].y);
						}
					}

					for (int i = 0; i < vecs.Count; ++i)
					{
						e.Graphics.FillRectangle(Brushes.Black, vecs[i].x - 1, vecs[i].y - 1, 3, 3);

						int nextVec = i + 1;
						if (nextVec == vecs.Count)
							nextVec = 0;
						e.Graphics.DrawLine(new Pen(lineColor, 2), vecs[i].x, vecs[i].y, vecs[nextVec].x, vecs[nextVec].y);
					}
				}
				catch
				{
					for (int i = 0; i < vecs.Count; ++i)
					{
						e.Graphics.FillRectangle((SolidBrush)Brushes.Black, vecs[i].x - 1, vecs[i].y - 1, 3, 3);

						int nextVec = i + 1;
						if (nextVec == vecs.Count)
							nextVec = 0;
						e.Graphics.DrawLine(new Pen(Color.Red, 2), vecs[i].x, vecs[i].y, vecs[nextVec].x, vecs[nextVec].y);
					}
				}
			}
		}

		public static int[] GridSizes = new int[]
		{
			1,
			2,
			4,
			8,
			16,
			32,
			64
		};

		public static int GridSizeIndex = 4;

		public static float Snap(float Value)
		{
			if (GridSizeIndex == 0)
				return Value;

			if (Math.Abs(Math.Floor(Value/GridSizes[GridSizeIndex])-(Value/GridSizes[GridSizeIndex]))<Math.Abs(Math.Ceiling(Value/GridSizes[GridSizeIndex])-(Value/GridSizes[GridSizeIndex])))
				return (float)Math.Floor((Value/GridSizes[GridSizeIndex]))*GridSizes[GridSizeIndex];
			else
				return (float)Math.Ceiling((Value/GridSizes[GridSizeIndex]))*GridSizes[GridSizeIndex];
		}

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				_currentMesh.Vertices.Add(new Vec2(e.Location.X, e.Location.Y));
				MovingVertex = _currentMesh.Vertices.Count - 1;
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				Rectangle b = new Rectangle(e.Location.X - 2, e.Location.Y - 2, 7, 7);
				for (int i = 0; i < _currentMesh.Vertices.Count; ++i)
				{
					if (b.IntersectsWith(new Rectangle((int)_currentMesh.Vertices[i].x, (int)_currentMesh.Vertices[i].y, 1, 1)))
					{
						MovingVertex = i;
						break;
					}
				}
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
			{
				Rectangle b = new Rectangle(e.Location.X - 2, e.Location.Y - 2, 7, 7);
				for (int i = 0; i < _currentMesh.Vertices.Count; ++i)
				{
					if (b.IntersectsWith(new Rectangle((int)_currentMesh.Vertices[i].x, (int)_currentMesh.Vertices[i].y, 1, 1)))
					{
						_currentMesh.Vertices.RemoveAt(i);
						break;
					}
				}
			}

			pictureBox1.Invalidate();

			ClickPos = e.Location;
			LastMovePos = e.Location;
		}

		private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
		{
			if (MovingVertex != -1)
			{
				_currentMesh.Vertices[MovingVertex] = new Vec2(_currentMesh.Vertices[MovingVertex].x - (LastMovePos.X - e.Location.X),
								_currentMesh.Vertices[MovingVertex].y - (LastMovePos.Y - e.Location.Y));
				LastMovePos = e.Location;
				pictureBox1.Invalidate();
			}
		}

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			if (MovingVertex != -1)
			{
				_currentMesh.Vertices[MovingVertex] = new Vec2(Snap(_currentMesh.Vertices[MovingVertex].x), Snap(_currentMesh.Vertices[MovingVertex].y));
				MovingVertex = -1;
				pictureBox1.Invalidate();
			}
		}

		private void increaseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GridSizeIndex++;
			if (GridSizeIndex > GridSizes.Length - 1)
				GridSizeIndex = GridSizes.Length - 1;
			pictureBox1.Invalidate();
		}

		private void decreaseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GridSizeIndex--;
			if (GridSizeIndex < 0)
				GridSizeIndex = 0;
			pictureBox1.Invalidate();
		}

		private void exportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog sf = new SaveFileDialog())
			{
				sf.RestoreDirectory = true;
				sf.Filter = "All Files (*)|*";
				sf.DefaultExt = "bmesh";

				if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					using (System.IO.FileStream fs = new System.IO.FileStream(sf.FileName, System.IO.FileMode.Create))
					{
						List<PolygonShape> shapes = new List<PolygonShape>();

						float largestX = 0, largestY = 0;
						foreach (var mesh in _meshes)
						{
							for (int i = 0; i < mesh.Vertices.Count; ++i)
							{
								if (Math.Abs(mesh.Vertices[i].x) > largestX)
									largestX = Math.Abs(mesh.Vertices[i].x);
								if (Math.Abs(mesh.Vertices[i].y) > largestY)
									largestY = Math.Abs(mesh.Vertices[i].y);
							}

							if (largestY > largestX)
								largestX = largestY;
							else
								largestY = largestX;
						}

						foreach (var mesh in _meshes)
						{
							ConvexDecomposition.Polygon p = new ConvexDecomposition.Polygon();
							foreach (var b in mesh.Vertices)
							{
								p.x.Add(b.x);
								p.y.Add(b.y);
							}

							List<Vec2> newVecs = new List<Vec2>();

							for (int i = 0; i < mesh.Vertices.Count; ++i)
								newVecs.Add(new Vec2(mesh.Vertices[i].x / largestX, mesh.Vertices[i].y / largestY));

							List<ConvexDecomposition.Polygon> results = new List<ConvexDecomposition.Polygon>();
							ConvexDecomposition.Statics.DecomposeConvex(p, results, 24);

							for (int i = 0; i < results.Count; ++i)
							{
								for (int x = 0; x < results[i].x.Count; ++x)
									results[i].x[x] /= largestX;
								for (int x = 0; x < results[i].y.Count; ++x)
									results[i].y[x] /= largestY;
							}

							foreach (var res in results)
							{
								List<Vec2> v = res.GetVertexVecs();

								//sw.WriteLine(
								//	"{");

								//for (int i = 0; i < v.Count; ++i)
								//	sw.WriteLine("\t" + v[i].x.ToString() + " " + v[i].y.ToString());

								//sw.WriteLine("}");

								shapes.Add(new PolygonShape(v.ToArray()));
							}
						}

						new MeshShape(shapes.ToArray()).SaveBinary(fs);
					}
				}
			}
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_meshes.Clear();
			_meshes.Add(_currentMesh = new Mesh());
			pictureBox1.Invalidate();
		}

		private void toolStripMenuItem1_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dg = new OpenFileDialog())
			{
				dg.Filter = "All Files (*)|*";
				dg.DefaultExt = "txt";
				dg.RestoreDirectory = true;

				if (dg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					_meshes.Clear();
					_meshes.Add(_currentMesh = new Mesh());

					pictureBox1.Invalidate();
				}
			}
		}

		private void ModelDesigner_Load(object sender, EventArgs e)
		{
			AddMesh(_currentMesh = new Mesh());
		}

		private void referenceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog())
			{
				dlg.Filter = "Supported Images (*.png;*.bmp;*.jpg;*.gif)|*.png;*.gif;*.jpg;*.bmp";
				dlg.RestoreDirectory = true;

				if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					RefImage = new Bitmap(dlg.FileName);
				else
					RefImage = null;

				pictureBox1.Invalidate();
			}
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}

		private void pruneVerticesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < _currentMesh.Vertices.Count; ++i)
			{
				int start = i;
				int middle = i + 1;

				if (middle >= _currentMesh.Vertices.Count)
					middle = 0;

				int end = middle + 1;

				if (end >= _currentMesh.Vertices.Count)
					end = 0;

				var len_sm = (_currentMesh.Vertices[start] - _currentMesh.Vertices[middle]).Length();
				var len_me = (_currentMesh.Vertices[middle] - _currentMesh.Vertices[end]).Length();
				var len_se = (_currentMesh.Vertices[start] - _currentMesh.Vertices[end]).Length();

				if ((len_sm + len_me) == len_se)
				{
					_currentMesh.Vertices.RemoveAt(middle);
					i = -1;
					continue;
				}
			}

			Invalidate();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			_currentMesh = _meshes[e.Node.Index];
			pictureBox1.Invalidate();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			AddMesh(new Mesh());
		}
	}

	public class Mesh
	{
		List<Vec2> _vertices = new List<Vec2>();

		public List<Vec2> Vertices
		{
			get { return _vertices; }
		}
	}
}