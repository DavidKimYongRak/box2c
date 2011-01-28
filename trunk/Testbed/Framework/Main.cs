using System;
using System.Collections;
using Box2CS;
using Tao.OpenGl;
using Tao.Platform.Windows;
using System.Windows.Forms;
using System.Threading;
using SFML.Window;
using SFML.Graphics;

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
		float settingsHz = 60.0f;
		float viewZoom = 1.0f;
		Vec2 viewCenter = new Vec2(0.0f, 20.0f);
		int tx, ty, tw, th;
		bool rMouseDown;
		Vec2 lastp;
		Thread simulationThread;
		long _ts = 0, _min = long.MaxValue, _max = long.MinValue;

		public float ViewZoom
		{
			get { return viewZoom; }
			set
			{
				viewZoom = value;
				OnGLResize(width, height);
			}
		}

		public Main()
		{
			StartPosition = FormStartPosition.Manual;
			InitializeComponent();

			Tao.FreeGlut.Glut.glutInit();
			//OnGLResize(_glcontrol.Width, _glcontrol.Height);

			simulationThread = new Thread(SimulationLoop);

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

			renderWindow = new RenderWindow(panel1.Handle, new ContextSettings(32, 0, 12));
			renderWindow.Resized += new EventHandler<SizeEventArgs>(render_Resized);
			renderWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(renderWindow_MouseButtonPressed);
			renderWindow.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(renderWindow_MouseButtonReleased);
			renderWindow.MouseMoved += new EventHandler<MouseMoveEventArgs>(renderWindow_MouseMoved);
			renderWindow.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(renderWindow_KeyPressed);
			renderWindow.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(renderWindow_KeyReleased);
			renderWindow.Show(true);

			OnGLResize((int)renderWindow.Width, (int)renderWindow.Height);

			updateDraw = UpdateDraw;

			simulationThread.Start();
		}

		void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			testSelection = comboBox1.SelectedIndex;
		}

		void OnGLResize(int w, int h)
		{
			if (InvokeRequired)
			{
				Invoke((Action)delegate() { OnGLResize(w, h); });
				return;
			}

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
			Glu.gluOrtho2D(lower.X, upper.X, lower.Y, upper.Y);
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
			p.X = (1.0f - u) * lower.X + u * upper.X;
			p.Y = (1.0f - v) * lower.Y + v * upper.Y;
			return p;
		}

		long _ts_b2, _ts_min_, _ts_max_;

		[System.Runtime.InteropServices.DllImport(Box2DSettings.Box2CDLLName)]
		extern static uint b2world_getelapsed();
		[System.Runtime.InteropServices.DllImport(Box2DSettings.Box2CDLLName)]
		extern static uint b2world_getmax();
		[System.Runtime.InteropServices.DllImport(Box2DSettings.Box2CDLLName)]
		extern static uint b2world_getmin();

		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		void Simulate()
		{
			if (test != null)
			{
				if (TestSettings.restart)
				{
					OnGLRestart();
					TestSettings.restart = false;
				}

				sw.Reset();
				TestSettings.hz = settingsHz;
				test.Step();
				sw.Stop();
				_ts = sw.ElapsedMilliseconds;

				if (_ts < _min)
					_min = _ts;
				if (_ts > _max)
					_max= _ts;

				_ts_b2 = b2world_getelapsed();
				_ts_max_ = b2world_getmax();
				_ts_min_ = b2world_getmin();

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

		void DrawTest()
		{
			if (test != null)
			{
				test.SetTextLine(30);
				test.DrawTitle(5, 15, entry.Name);
				test.m_debugDraw.DrawString(5, 100, "Time: "+_ts.ToString()+" (B2: "+_ts_b2.ToString()+")");
				test.m_debugDraw.DrawString(5, 115, "MinTime: " + _min.ToString()+" (B2: "+_ts_min_.ToString()+")");
				test.m_debugDraw.DrawString(5, 130, "MaxTime: " + _max.ToString()+" (B2: "+_ts_max_.ToString()+")");
				test.Draw();
			}
		}

		// This is used to control the frame rate (60Hz).
		static long NextGameTick = System.Environment.TickCount;
		static int gameFrame = 0;
		public static System.Drawing.Point CursorPos = new System.Drawing.Point();
		static RenderWindow renderWindow;

		public static RenderWindow GLWindow
		{
			get { return renderWindow; }
		}

		delegate void UpdateDrawDelegate();
		UpdateDrawDelegate updateDraw;
		void UpdateDraw()
		{
			// Process events
			renderWindow.DispatchEvents();

			// Clear the window
			renderWindow.Clear();

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();

			// Draw
			test.m_world.DrawDebugData();

			DrawTest();

			renderWindow.Display();
		}

		void SimulationLoop()
		{
			while (true)
			{
				CursorPos = new System.Drawing.Point(renderWindow.Input.GetMouseX(), renderWindow.Input.GetMouseY());

				int TICKS_PER_SECOND = (int)settingsHz;
				int SKIP_TICKS = 1000 / TICKS_PER_SECOND;
				const int MAX_FRAMESKIP = 5;

				int loops = 0;
				while (System.Environment.TickCount > NextGameTick && loops < MAX_FRAMESKIP)
				{					
					Simulate();

					NextGameTick += SKIP_TICKS;
					loops++;
					gameFrame++;
				}

				// Sleep it off
				Thread.Sleep(5);

				Invoke(updateDraw);
			}
		}

		void renderWindow_KeyReleased(object sender, SFML.Window.KeyEventArgs e)
		{
		}

		void renderWindow_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
		{
			OnGLKeyboard(e.Code, renderWindow.Input.GetMouseX(),
				renderWindow.Input.GetMouseY());
		}

		void renderWindow_MouseMoved(object sender, MouseMoveEventArgs e)
		{
			OnGLMouseMotion(e.X, e.Y);
		}

		void renderWindow_MouseButtonReleased(object sender, MouseButtonEventArgs e)
		{
			OnGLMouse(e.Button, MouseButtonState.Up, e.X, e.Y);
		}

		void renderWindow_MouseButtonPressed(object sender, MouseButtonEventArgs e)
		{
			OnGLMouse(e.Button, MouseButtonState.Down, e.X, e.Y);
		}

		void render_Resized(object sender, SizeEventArgs e)
		{
			OnGLResize((int)e.Width, (int)e.Height);
		}

		void OnGLKeyboard(KeyCode key, int x, int y)
		{
			switch (key)
			{
			case KeyCode.Escape:
				Application.Exit();
				break;

				// Press 'z' to zoom out.
			case KeyCode.Z:
				viewZoom = Math.Min(1.1f * viewZoom, 20.0f);
				OnGLResize(width, height);
				break;

				// Press 'x' to zoom in.
			case KeyCode.X:
				viewZoom = Math.Max(0.9f * viewZoom, 0.02f);
				OnGLResize(width, height);
				break;

				// Press 'r' to reset.
			case KeyCode.R:
				test.Dispose();
				test = entry.Construct();
				break;

				// Press space to launch a bomb.
			case KeyCode.Space:
				if (test != null)
					test.LaunchBomb();
				break;

			case KeyCode.P:
				TestSettings.pause = !TestSettings.pause;
				break;

				// Press [ to prev test.
			case KeyCode.LBracket:
				--testSelection;
				if (testSelection < 0)
				{
					testSelection = entries.Entries.Count - 1;
				}
				break;

				// Press ] to next test.
			case KeyCode.RBracket:
				++testSelection;
				if (testSelection == entries.Entries.Count)
				{
					testSelection = 0;
				}
				break;

				// Press left to pan left.
			case KeyCode.Left:
				viewCenter.X -= 0.5f;
				OnGLResize(width, height);
				break;

				// Press right to pan right.
			case KeyCode.Right:
				viewCenter.X += 0.5f;
				OnGLResize(width, height);
				break;

				// Press down to pan down.
			case KeyCode.Down:
				viewCenter.Y -= 0.5f;
				OnGLResize(width, height);
				break;

				// Press up to pan up.
			case KeyCode.Up:
				viewCenter.Y += 0.5f;
				OnGLResize(width, height);
				break;

				// Press home to reset the view.
			case KeyCode.Home:
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

		enum MouseButtonState
		{
			Down,
			Up
		}

		void OnGLMouse(MouseButton button, MouseButtonState state, int x, int y)
		{
			// Use the mouse to move things around.
			if (button == MouseButton.Left)
			{
				Vec2 p = ConvertScreenToWorld(x, y);
				if (state == MouseButtonState.Down)
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
		
				if (state == MouseButtonState.Up)
				{
					test.MouseUp(p);
				}
			}
			else if (button == MouseButton.Right)
			{
				if (state == MouseButtonState.Down)
				{	
					lastp = ConvertScreenToWorld(x, y);
					rMouseDown = true;
				}

				if (state == MouseButtonState.Up)
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
				viewCenter.X -= diff.X;
				viewCenter.Y -= diff.Y;
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

			_min = long.MaxValue;
			_max = long.MinValue;
			NextGameTick = System.Environment.TickCount;
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
			TestSettings.restart = true;
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

		private void Main_Load(object sender, EventArgs e)
		{

		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			//simulationTimer.Stop();
			//simulationTimer.Dispose();
			//simulationTimer = null;
			simulationThread.Abort();
		}

		private void panel1_Click(object sender, EventArgs e)
		{
			panel1.Focus();
		}
	}

	public class HolyCrapControl : Control
	{
	}
}
