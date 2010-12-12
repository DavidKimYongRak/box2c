using System;
using System.Collections;
using Box2CS;
using Tao.OpenGl;
using Tao.Platform.Windows;
using System.Windows.Forms;

namespace Testbed
{
	public partial class Main : Form
	{
		int testIndex = 0;
		int testSelection = 0;
		TestEntries entries = new TestEntries();
		TestEntry entry;
		Test test;
		int width = 640;
		int height = 480;
		int framePeriod = 15;
		float settingsHz = 60.0f;
		float viewZoom = 1.0f;
		Vec2 viewCenter = new Vec2(0.0f, 20.0f);
		int tx, ty, tw, th;
		bool rMouseDown;
		Vec2 lastp;
		SimpleOpenGlControl _glcontrol;

		public SimpleOpenGlControl GLWindow
		{
			get { return _glcontrol; }
		}

		public Main()
		{
			InitializeComponent();

			_glcontrol = new SimpleOpenGlControl();
			_glcontrol.Dock = DockStyle.Fill;
			_glcontrol.AutoCheckErrors = true;
			_glcontrol.AutoFinish = true;
			_glcontrol.AutoMakeCurrent = true;
			_glcontrol.Paint += new PaintEventHandler(_glcontrol_Paint);
			_glcontrol.Resize += new EventHandler(_glcontrol_Resize);
			_glcontrol.MouseDown += new MouseEventHandler(_glcontrol_MouseDown);
			_glcontrol.MouseUp += new MouseEventHandler(_glcontrol_MouseUp);
			_glcontrol.MouseMove += new MouseEventHandler(_glcontrol_MouseMove);
			_glcontrol.KeyDown += new KeyEventHandler(_glcontrol_KeyDown);
			splitContainer1.Panel1.Controls.Add(_glcontrol);

			_glcontrol.InitializeContexts();
			Tao.FreeGlut.Glut.glutInit();
			OnGLResize(_glcontrol.Width, _glcontrol.Height);

			//Gl.glEnable(Gl.GL_LINE_SMOOTH);
			//Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);

			System.Timers.Timer simulationTimer = new System.Timers.Timer();
			simulationTimer.Interval = framePeriod;
			simulationTimer.SynchronizingObject = this;
			simulationTimer.Elapsed += new System.Timers.ElapsedEventHandler(simulationTimer_Elapsed);
			simulationTimer.Start();

			Text = "Box2CS Testbed - Box2D version " + Box2DVersion.Version.ToString();

			testIndex = b2Math.b2Clamp(testIndex, 0, entries.Entries.Count-1);
			testSelection = testIndex;

			entry = entries.Entries[testIndex];
			test = entry.Construct();

			foreach (var e in entries.Entries)
				comboBox1.Items.Add(e.Name);

			comboBox1.SelectedIndex = 0;
			comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);

			numericUpDown1.Value = TestSettings.velocityIterations;
			numericUpDown2.Value = TestSettings.positionIterations;
			numericUpDown3.Value = (decimal)TestSettings.hz;
		}

		void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			testSelection = comboBox1.SelectedIndex;
		}

		void _glcontrol_KeyDown(object sender, KeyEventArgs e)
		{
			OnGLKeyboard(e.KeyCode, _glcontrol.PointToClient(Cursor.Position).X,
				_glcontrol.PointToClient(Cursor.Position).Y);
		}

		void _glcontrol_MouseMove(object sender, MouseEventArgs e)
		{
			OnGLMouseMotion(e.Location.X, e.Location.Y);
		}

		void _glcontrol_MouseUp(object sender, MouseEventArgs e)
		{
			OnGLMouse(e.Button, EMouseButtonState.Up, e.Location.X, e.Location.Y);
		}

		void _glcontrol_MouseDown(object sender, MouseEventArgs e)
		{
			OnGLMouse(e.Button, EMouseButtonState.Down, e.Location.X, e.Location.Y);
		}

		void _glcontrol_Resize(object sender, EventArgs e)
		{
			OnGLResize(_glcontrol.Width, _glcontrol.Height);
		}

		void _glcontrol_Paint(object sender, PaintEventArgs e)
		{
			test.m_world.DrawDebugData();
		}

		void simulationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			SimulationLoop();
		}

		void OnGLResize(int w, int h)
		{
			width = w;
			height = h;

			tx = ty = 0;
			tw = w;
			th = h;
			Gl.glViewport(tx, ty, tw, th);

			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			float ratio = (float)tw / (float)th;

			Vec2 extents = new Vec2(ratio * 25.0f, 25.0f);
			extents *= viewZoom;

			Vec2 lower = viewCenter - extents;
			Vec2 upper = viewCenter + extents;

			// L/R/B/T
			Glu.gluOrtho2D(lower.x, upper.x, lower.y, upper.y);
		}

		public Vec2 ConvertScreenToWorld(int x, int y)
		{
			float u = x / (float)tw;
			float v = (th - y) / (float)th;

			float ratio = (float)tw / (float)th;
			Vec2 extents = new Vec2(ratio * 25.0f, 25.0f);
			extents *= viewZoom;

			Vec2 lower = viewCenter - extents;
			Vec2 upper = viewCenter + extents;

			Vec2 p = new Vec2();
			p.x = (1.0f - u) * lower.x + u * upper.x;
			p.y = (1.0f - v) * lower.y + v * upper.y;
			return p;
		}

		// This is used to control the frame rate (60Hz).
		void SimulationLoop()
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();

			if (test != null)
			{
				test.SetTextLine(30);
				TestSettings.hz = settingsHz;
				if (!TestSettings.pause)
					test.Step();

				test.DrawTitle(5, 15, entry.Name);

				if (testSelection != testIndex)
				{
					testIndex = testSelection;
					entry = entries.Entries[testIndex];
					test.Dispose();
					test = entry.Construct();
					viewZoom = 1.0f;
					viewCenter = new Vec2(0.0f, 20.0f);
					OnGLResize(width, height);
				}
			}
		}

		void OnGLKeyboard(Keys key, int x, int y)
		{
			switch (key)
			{
			case Keys.Escape:
				Application.Exit();
				break;

				// Press 'z' to zoom out.
			case Keys.Z:
				viewZoom = Math.Min(1.1f * viewZoom, 20.0f);
				OnGLResize(width, height);
				break;

				// Press 'x' to zoom in.
			case Keys.X:
				viewZoom = Math.Max(0.9f * viewZoom, 0.02f);
				OnGLResize(width, height);
				break;

				// Press 'r' to reset.
			case Keys.R:
				test.Dispose();
				test = entry.Construct();
				break;

				// Press space to launch a bomb.
			case Keys.Space:
				if (test != null)
					test.LaunchBomb();
				break;
 
			case Keys.P:
				TestSettings.pause = !TestSettings.pause;
				break;

				// Press [ to prev test.
			case Keys.OemOpenBrackets:
				--testSelection;
				if (testSelection < 0)
				{
					testSelection = entries.Entries.Count - 1;
				}
				break;

				// Press ] to next test.
			case Keys.OemCloseBrackets:
				++testSelection;
				if (testSelection == entries.Entries.Count)
				{
					testSelection = 0;
				}
				break;

				// Press left to pan left.
			case Keys.Left:
				viewCenter.x -= 0.5f;
				OnGLResize(width, height);
				break;

				// Press right to pan right.
			case Keys.Right:
				viewCenter.x += 0.5f;
				OnGLResize(width, height);
				break;

				// Press down to pan down.
			case Keys.Down:
				viewCenter.y -= 0.5f;
				OnGLResize(width, height);
				break;

				// Press up to pan up.
			case Keys.Up:
				viewCenter.y += 0.5f;
				OnGLResize(width, height);
				break;

				// Press home to reset the view.
			case Keys.Home:
				viewZoom = 1.0f;
				viewCenter = new Vec2(0.0f, 20.0f);
				OnGLResize(width, height);
				break;
		
			default:
				if (test != null)
					test.Keyboard(key);
				break;
			}
		}

		enum EMouseButtonState
		{
			Down,
			Up
		}

		void OnGLMouse(MouseButtons button, EMouseButtonState state, int x, int y)
		{
			// Use the mouse to move things around.
			if (button == System.Windows.Forms.MouseButtons.Left)
			{
				Vec2 p = ConvertScreenToWorld(x, y);
				if (state == EMouseButtonState.Down)
				{
					if ((ModifierKeys & Keys.Shift) != 0)
					{
						test.ShiftMouseDown(p);
					}
					else
					{
						test.MouseDown(p);
					}
				}
		
				if (state == EMouseButtonState.Up)
				{
					test.MouseUp(p);
				}
			}
			else if (button == System.Windows.Forms.MouseButtons.Right)
			{
				if (state == EMouseButtonState.Down)
				{	
					lastp = ConvertScreenToWorld(x, y);
					rMouseDown = true;
				}

				if (state == EMouseButtonState.Up)
				{
					rMouseDown = false;
				}
			}
		}

		void OnGLMouseMotion(int x, int y)
		{
			Vec2 p = ConvertScreenToWorld(x, y);
			test.MouseMove(p);
	
			if (rMouseDown)
			{
				Vec2 diff = p - lastp;
				viewCenter.x -= diff.x;
				viewCenter.y -= diff.y;
				OnGLResize(width, height);
				lastp = ConvertScreenToWorld(x, y);
			}
		}

		void OnGLMouseWheel(int wheel, int direction, int x, int y)
		{
			if (direction > 0)
			{
				viewZoom /= 1.1f;
			}
			else
			{
				viewZoom *= 1.1f;
			}
			OnGLResize(width, height);
		}

		void OnGLRestart()
		{
			test.Dispose();
			entry = entries.Entries[testIndex];
			test = entry.Construct();
			OnGLResize(width, height);
		}

		void Pause()
		{
			TestSettings.pause = !TestSettings.pause;
		}

		void SingleStep()
		{
			TestSettings.pause = true;
			TestSettings.singleStep = true;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			TestSettings.enableWarmStarting = checkBox1.Checked;
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			TestSettings.enableContinuous = checkBox2.Checked;
		}

		private void checkBox3_CheckedChanged(object sender, EventArgs e)
		{
			TestSettings.drawShapes = checkBox3.Checked;
		}

		private void checkBox4_CheckedChanged(object sender, EventArgs e)
		{
			TestSettings.drawJoints = checkBox4.Checked;
		}

		private void checkBox5_CheckedChanged(object sender, EventArgs e)
		{
			TestSettings.drawAABBs = checkBox5.Checked;
		}

		private void checkBox6_CheckedChanged(object sender, EventArgs e)
		{
			TestSettings.drawPairs = checkBox6.Checked;
		}

		private void checkBox7_CheckedChanged(object sender, EventArgs e)
		{
			TestSettings.drawContactPoints = checkBox7.Checked;
		}

		private void checkBox8_CheckedChanged(object sender, EventArgs e)
		{
			TestSettings.drawContactNormals = checkBox8.Checked;
		}

		private void checkBox9_CheckedChanged(object sender, EventArgs e)
		{
			TestSettings.drawContactForces = checkBox9.Checked;
		}

		private void checkBox10_CheckedChanged(object sender, EventArgs e)
		{
			TestSettings.drawFrictionForces = checkBox10.Checked;
		}

		private void checkBox11_CheckedChanged(object sender, EventArgs e)
		{
			TestSettings.drawCOMs = checkBox11.Checked;
		}

		private void checkBox12_CheckedChanged(object sender, EventArgs e)
		{
			TestSettings.drawStats = checkBox12.Checked;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Pause();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			SingleStep();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			OnGLRestart();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void numericUpDown3_ValueChanged(object sender, EventArgs e)
		{
			settingsHz = (float)numericUpDown3.Value;
		}

		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{
			TestSettings.positionIterations = (int)numericUpDown2.Value;
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			TestSettings.velocityIterations = (int)numericUpDown1.Value;
		}
	}
}
