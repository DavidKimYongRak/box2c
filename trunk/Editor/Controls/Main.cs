using System;
using System.Collections;
using Box2CS;
using Tao.OpenGl;
using System.Windows.Forms;
using System.Threading;
using SFML.Window;
using SFML.Graphics;
using Tao.FreeGlut;
using Box2CS.Serialize;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ComponentModel;
using Paril.Collections;

namespace Editor
{
	public partial class Main : Form
	{
		float settingsHz = 60.0f;
		float viewZoom = 1.0f;
		Vec2 viewCenter = new Vec2(0.0f, 0.0f);
		int tx, ty, tw, th;
		bool rMouseDown;
		Vec2 lastp;
		World _world;
		Thread simulationThread;
		TestDebugDraw debugDraw;
		BodyObject HoverBody = null, SelectedBody = null;
        FixtureDefSerialized SelectedFixture = null;
        CirclePanel circlePanel = new CirclePanel();
		public static WorldObject WorldObject = new WorldObject();
		bool _testing = false;

		public float ViewZoom
		{
			get { return viewZoom; }
			set
			{
				viewZoom = value;
				OnGLResize();
			}
		}

		void StartTest()
		{
			_testing = true;
			_world = new World(new Vec2(0, -10.0f), true);
			_world.DebugDraw = debugDraw;

			System.Collections.Generic.List<Body> bodies = new System.Collections.Generic.List<Body>();

			foreach (var x in WorldObject.Bodies)
			{
				var body = _world.CreateBody(x.Body);

				bodies.Add(body);

				foreach (var f in x.Fixtures)
					body.CreateFixture(f.Fixture);

				body.MassData = x.Mass;
			}

			foreach (var j in WorldObject.Joints)
			{
				j.Joint.BodyA = bodies[j.BodyAIndex];
				j.Joint.BodyB = bodies[j.BodyBIndex];

				var joint = _world.CreateJoint(j.Joint);
			}
		}

		void EndTest()
		{
			_testing = false;
			_world = null;
			//simulationThread.Abort();
			//simulationThread = null;
		}

		public Main()
		{
			StartPosition = FormStartPosition.Manual;
			InitializeComponent();

			Text = "Box2CS Level Editor - Box2D version " + Box2DVersion.Version.ToString();

			renderWindow = new RenderWindow(panel1.Handle, new ContextSettings(32, 0, 12));
			renderWindow.Resized += new EventHandler<SizeEventArgs>(render_Resized);
			renderWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(renderWindow_MouseButtonPressed);
			renderWindow.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(renderWindow_MouseButtonReleased);
			renderWindow.MouseMoved += new EventHandler<MouseMoveEventArgs>(renderWindow_MouseMoved);
			renderWindow.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(renderWindow_KeyPressed);
			renderWindow.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(renderWindow_KeyReleased);
			renderWindow.Show(true);

			OnGLResize();
			Tao.FreeGlut.Glut.glutInit();

			debugDraw = new TestDebugDraw();
			debugDraw.Flags = DebugFlags.Shapes | DebugFlags.Joints | DebugFlags.CenterOfMasses;
		}

		void OnGLResize()
		{
			if (InvokeRequired)
			{
				Invoke((Action)delegate() { OnGLResize(); });
				return;
			}

			tx = ty = 0;
			tw = (int)renderWindow.Width;
			th = (int)renderWindow.Height;
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

		// This is used to control the frame rate (60Hz).
		static long NextGameTick = System.Environment.TickCount;
		static int gameFrame = 0;
		public static System.Drawing.Point CursorPos = new System.Drawing.Point();
		static RenderWindow renderWindow;

		public static RenderWindow GLWindow
		{
			get { return renderWindow; }
		}

		void UpdateDraw()
		{
			// Process events
			renderWindow.DispatchEvents();

			// Clear the window
			renderWindow.Clear();

            Gl.glClearColor((float)BackColor.R / 255.0f, (float)BackColor.G / 255.0f, (float)BackColor.B / 255.0f, 1);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();

			// Draw
			//test.m_world.DrawDebugData();

			//DrawTest();

			if (!_testing)
			{
				foreach (var x in WorldObject.Bodies)
				{
					var xf = new Transform(x.Body.Position, new Mat22(x.Body.Angle));

					foreach (var fixture in x.Fixtures)
					{
						ColorF color = new ColorF(0.9f, 0.7f, 0.7f);

						if (SelectedBody != null && x.Body == SelectedBody.Body)
							color = new ColorF(1, 1, 0);
						else if (HoverBody != null && x.Body == HoverBody.Body)
							color = new ColorF(1, 0, 0);
						else if (!x.Body.Active)
							color = new ColorF(0.5f, 0.5f, 0.3f);
						else if (x.Body.BodyType == BodyType.Static)
							color = new ColorF(0.5f, 0.9f, 0.5f);
						else if (x.Body.BodyType == BodyType.Kinematic)
							color = new ColorF(0.5f, 0.5f, 0.9f);
						else if (x.Body.Awake)
							color = new ColorF(0.6f, 0.6f, 0.6f);

						debugDraw.DrawShape(fixture.Fixture, xf, color);
					}

					debugDraw.DrawTransform(xf);
				}

				if (HoverBody != null && !string.IsNullOrEmpty(HoverBody.Name))
				{
					Vec2 p = ConvertScreenToWorld(CursorPos.X, CursorPos.Y);
					TestDebugDraw.DrawStringFollow(p.X, p.Y, HoverBody.Name);
				}

				foreach (var x in WorldObject.Joints)
				{
					Vec2 a1 = Vec2.Empty, a2 = Vec2.Empty;

					switch (x.Joint.JointType)
					{
					case JointType.Revolute:
						a1 = ((RevoluteJointDef)x.Joint).LocalAnchorA;
						a2 = ((RevoluteJointDef)x.Joint).LocalAnchorB;
						break;
					case JointType.Friction:
						a1 = ((FrictionJointDef)x.Joint).LocalAnchorA;
						a2 = ((FrictionJointDef)x.Joint).LocalAnchorB;
						break;
					case JointType.Line:
						a1 = ((LineJointDef)x.Joint).LocalAnchorA;
						a2 = ((LineJointDef)x.Joint).LocalAnchorB;
						break;
					case JointType.Prismatic:
						a1 = ((PrismaticJointDef)x.Joint).LocalAnchorA;
						a2 = ((PrismaticJointDef)x.Joint).LocalAnchorB;
						break;
					case JointType.Weld:
						a1 = ((WeldJointDef)x.Joint).LocalAnchorA;
						a2 = ((WeldJointDef)x.Joint).LocalAnchorB;
						break;
					}
					debugDraw.DrawJoint(x, a1, a2, WorldObject.Bodies[x.BodyAIndex].Body, WorldObject.Bodies[x.BodyBIndex].Body);
				}
			}
			else
				_world.DrawDebugData();

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
					//Simulate();
					if (_world != null)
						_world.Step(1.0f / settingsHz, 8, 3);

					NextGameTick += SKIP_TICKS;
					loops++;
					gameFrame++;
				}

				Invoke((Action)delegate() { UpdateDraw(); });

				// Sleep it off
				Thread.Sleep(5);
			}
		}

		void renderWindow_KeyReleased(object sender, SFML.Window.KeyEventArgs e)
		{
		}

		void renderWindow_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
		{
			int x = renderWindow.Input.GetMouseX();
			int y = renderWindow.Input.GetMouseY();

			switch (e.Code)
			{
			case KeyCode.Escape:
				Application.Exit();
				break;

			case KeyCode.T:
				if (!_testing)
					StartTest();
				else
					EndTest();
				break;

			// Press 'z' to zoom out.
			case KeyCode.Z:
				viewZoom = Math.Min(1.1f * viewZoom, 20.0f);
				OnGLResize();
				break;

			// Press 'x' to zoom in.
			case KeyCode.X:
				viewZoom = Math.Max(0.9f * viewZoom, 0.02f);
				OnGLResize();
				break;

			// Press left to pan left.
			case KeyCode.Left:
				viewCenter.X -= 0.5f;
				OnGLResize();
				break;

			// Press right to pan right.
			case KeyCode.Right:
				viewCenter.X += 0.5f;
				OnGLResize();
				break;

			// Press down to pan down.
			case KeyCode.Down:
				viewCenter.Y -= 0.5f;
				OnGLResize();
				break;

			// Press up to pan up.
			case KeyCode.Up:
				viewCenter.Y += 0.5f;
				OnGLResize();
				break;

			// Press home to reset the view.
			case KeyCode.Home:
				viewZoom = 1.0f;
				viewCenter = new Vec2(0.0f, 20.0f);
				OnGLResize();
				break;
			}	
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
			OnGLResize();
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
				CheckSelect(x, y);
				Vec2 p = ConvertScreenToWorld(x, y);
				if (state == MouseButtonState.Down)
				{
					if ((ModifierKeys & Keys.Shift) != 0)
					{
					}
					else
					{
					}
				}
		
				if (state == MouseButtonState.Up)
				{
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

		void CheckSelect(int x, int y)
		{
			Vec2 p = ConvertScreenToWorld(x, y);

			BodyObject moused = null;
			foreach (var b in WorldObject.Bodies)
			{
				foreach (var f in b.Fixtures)
				{
					if (f.Fixture.Shape.TestPoint(new Transform(b.Body.Position, new Mat22(b.Body.Angle)), p))
						moused = b;
				}
			}

			if (MouseButtons == System.Windows.Forms.MouseButtons.Left)
			{
				if (moused != null)
					SelectedBody = moused;
			}

			HoverBody = moused;
		}

		void OnGLMouseMotion(int x, int y)
		{
			Vec2 p = ConvertScreenToWorld(x, y);

			if (rMouseDown)
			{
				Vec2 diff = p - lastp;
				viewCenter.X -= diff.X;
				viewCenter.Y -= diff.Y;
				OnGLResize();
				lastp = ConvertScreenToWorld(x, y);
			}

			CheckSelect(x, y);
		}

		void OnGLMouseWheel(int wheel, int direction, int x, int y)
		{
			if (direction > 0)
				viewZoom /= 1.1f;
			else
				viewZoom *= 1.1f;
			OnGLResize();
		}

		void OnGLRestart()
		{
			OnGLResize();
			NextGameTick = System.Environment.TickCount;
		}

		private void Main_Load(object sender, EventArgs e)
		{
			ImageList list = new ImageList();
			list.ColorDepth = ColorDepth.Depth32Bit;
			list.ImageSize = new System.Drawing.Size(16, 16);

			System.Drawing.Bitmap b = new System.Drawing.Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Green, 3), new System.Drawing.Rectangle(3, 3, 16 - 6, 16 - 6));
			}
			list.Images.Add(b);

			b = new System.Drawing.Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

				System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
				path.AddPolygon(new System.Drawing.Point[]
				{
					new System.Drawing.Point(4, 7),
					new System.Drawing.Point(13, 3),
					new System.Drawing.Point(7, 15),
					new System.Drawing.Point(1, 13)
				}
				);
				g.FillPath(System.Drawing.Brushes.Red, path);
			}
			list.Images.Add(b);

			b = new System.Drawing.Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
				TextRenderer.DrawText(g, "F", Font, new System.Drawing.Point(2, 2), System.Drawing.Color.Black);
			}
			list.Images.Add(b);

			b = new System.Drawing.Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
				TextRenderer.DrawText(g, "B", Font, new System.Drawing.Point(2, 2), System.Drawing.Color.Black);
			}
			list.Images.Add(b);

			b = new System.Drawing.Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
				TextRenderer.DrawText(g, "G", Font, new System.Drawing.Point(2, 2), System.Drawing.Color.Black);
			}
			list.Images.Add(b);

			b = new System.Drawing.Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
				TextRenderer.DrawText(g, "W", Font, new System.Drawing.Point(2, 2), System.Drawing.Color.Black);
			}
			list.Images.Add(b);

			b = new System.Drawing.Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

				g.DrawCurve(System.Drawing.Pens.Black, new System.Drawing.Point[] {
				new System.Drawing.Point(2, 2),
				new System.Drawing.Point(4, 7),
				new System.Drawing.Point(12, 7),
				new System.Drawing.Point(14, 2)
				});
			}
			list.Images.Add(b);

			simulationThread = new Thread(SimulationLoop);
			simulationThread.Start();
			{
				treeView1.ImageList = list;
				var node = new Paril.Windows.Forms.TreeNodeEx("World");
				node.ImageIndex = node.SelectedImageIndex = 5;

				node.Nodes.Add(new Paril.Windows.Forms.TreeNodeEx("Ground", 4, 4));
				node.Nodes[0].Nodes.Add(new Paril.Windows.Forms.TreeNodeEx("Fixture 0 (Ramp piece)", 2, 2));
				node.Nodes[0].Nodes[0].Nodes.Add(new Paril.Windows.Forms.TreeNodeEx("Edge", 6, 6));
				node.Nodes[0].Nodes.Add(new Paril.Windows.Forms.TreeNodeEx("Fixture 1 (Ramp piece)", 2, 2));
				node.Nodes[0].Nodes[1].Nodes.Add(new Paril.Windows.Forms.TreeNodeEx("Polygon", 1, 1));

				node.Nodes.Add(new Paril.Windows.Forms.TreeNodeEx("Elevator", 4, 4));
				node.Nodes[1].Nodes.Add(new Paril.Windows.Forms.TreeNodeEx("Fixture 0 (Left pulley track)", 2, 2));
				node.Nodes[1].Nodes[0].Nodes.Add(new Paril.Windows.Forms.TreeNodeEx("Circle", 0, 0));
				node.Nodes[1].Nodes.Add(new Paril.Windows.Forms.TreeNodeEx("Fixture 1 (Right pulley track)", 2, 2));
				node.Nodes[1].Nodes[1].Nodes.Add(new Paril.Windows.Forms.TreeNodeEx("Circle", 0, 0));
				node.Nodes[1].Nodes.Add(new Paril.Windows.Forms.TreeNodeEx("Fixture 1 (Pulley track)", 2, 2));
				node.Nodes[1].Nodes[2].Nodes.Add(new Paril.Windows.Forms.TreeNodeEx("Circle", 0, 0));

				treeView1.Nodes.Add(node);
				node.Expand();
			}
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			simulationThread.Abort();
		}

		private void panel1_MouseMove(object sender, MouseEventArgs e)
		{
		}

		private void panel1_KeyPress(object sender, KeyPressEventArgs e)
		{

		}

		private void panel1_Click(object sender, EventArgs e)
		{
			panel1.Focus();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bodyListBox.SelectedIndex == -1)
				return;

			//propertyGrid1.SelectedObject = bodies[listBox1.SelectedIndex];
			SelectedBody = WorldObject.Bodies[bodyListBox.SelectedIndex];
            LoadBodyObjectSettings();
		}

        public void LoadBodyObjectSettings()
        {
			bodyActive.Checked = SelectedBody.Body.Active;
			bodyAllowSleep.Checked = SelectedBody.Body.AllowSleep;
            bodyAngle.Value = Convert.ToDecimal(SelectedBody.Body.Angle);
            bodyAngularDamping.Value = Convert.ToDecimal(SelectedBody.Body.AngularDamping);
            bodyAngularVelocity.Value = Convert.ToDecimal(SelectedBody.Body.AngularVelocity);
			bodyAwake.Checked = SelectedBody.Body.Awake;
            bodyType.SelectedIndex = (int)SelectedBody.Body.BodyType;
			bodyBullet.Checked = SelectedBody.Body.Bullet;
			bodyFixedRotation.Checked = SelectedBody.Body.FixedRotation;
            bodyInertiaScale.Value = Convert.ToDecimal(SelectedBody.Body.InertiaScale);
            bodyLinearDamping.Value = Convert.ToDecimal(SelectedBody.Body.LinearDamping);
            bodyLinearVelX.Value = Convert.ToDecimal(SelectedBody.Body.LinearVelocity.X);
            bodyLinearVelY.Value = Convert.ToDecimal(SelectedBody.Body.LinearVelocity.Y);
            bodyPositionX.Value = Convert.ToDecimal(SelectedBody.Body.Position.X);
            bodyPositionY.Value = Convert.ToDecimal(SelectedBody.Body.Position.Y);

            bodyName.Text = SelectedBody.Name;

			bodyAutoMassRecalculate.Checked = SelectedBody.AutoMassRecalculate;

            bodyCenterX.Value = Convert.ToDecimal(SelectedBody.Mass.Center.X);
            bodyCenterY.Value = Convert.ToDecimal(SelectedBody.Mass.Center.Y);
            bodyMass.Value = Convert.ToDecimal(SelectedBody.Mass.Mass);
            bodyInertia.Value = Convert.ToDecimal(SelectedBody.Mass.Inertia);
        
            bodyFixtureListBox.Items.Clear();

            for (int i = 0; i < SelectedBody.Fixtures.Count; i ++)
			{
                FixtureDefSerialized fixDefSer = SelectedBody.Fixtures[i];
                bodyFixtureListBox.Items.Add(fixDefSer.Name);
            }

            int prevIndex = bodyFixtureSelect.SelectedIndex;

            bodyFixtureSelect.Items.Clear();

            if (WorldObject.Fixtures.Count > 0)
            {
                for (int i = 0; i < WorldObject.Fixtures.Count; i++)
                {
                    FixtureDefSerialized fixDefSer = WorldObject.Fixtures[i];
                    bodyFixtureSelect.Items.Add(fixDefSer.Name);
                }

                if (prevIndex < 0 || prevIndex >= WorldObject.Fixtures.Count)
                    prevIndex = 0;

                bodyFixtureSelect.SelectedIndex = prevIndex;
            }
        }

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			WorldObject.Bodies.Add(new BodyObject(WorldObject, new BodyDefSerialized(null, new BodyDef(), new List<int> { }, "Body " + (WorldObject.Bodies.Count).ToString())));
			bodyListBox.Items.Add("Body "+(WorldObject.Bodies.Count-1).ToString());
		}

        private void button1_Click(object sender, EventArgs e)
        {
			FixtureDefSerialized fixDefSer = WorldObject.Fixtures[bodyFixtureSelect.SelectedIndex];
            bool addFix = true;
            for (int i = 0; i < SelectedBody.Fixtures.Count; i ++ ) {
                if (SelectedBody.Fixtures[i] == fixDefSer) {
                    addFix = false;
                }
            }
            if (addFix) { SelectedBody.Fixtures.Add(fixDefSer); }

            LoadBodyObjectSettings();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SelectedBody.Name = bodyName.Text;
        }

        private void bodyAutoMassRecalculate_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedBody.AutoMassRecalculate = bodyAutoMassRecalculate.Checked;
        }

        private void bodyActive_SelectedIndexChanged(object sender, EventArgs e)
        {
			SelectedBody.Body.Active = bodyActive.Checked;
        }

        private void bodyAllowSleep_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedBody.Body.AllowSleep = bodyAllowSleep.Checked;
        }

        private void bodyAngle_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            SelectedBody.Body.Angle = (float)e.NewValue;
        }

        private void bodyAngularDamping_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            SelectedBody.Body.AngularDamping = (float)e.NewValue;
        }

        private void bodyAngularVelocity_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            SelectedBody.Body.AngularVelocity = (float)e.NewValue;
        }

        private void bodyAwake_SelectedIndexChanged(object sender, EventArgs e)
        {
			SelectedBody.Body.Awake = bodyAwake.Checked;
        }

        private void bodyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedBody.Body.BodyType = (BodyType)bodyType.SelectedIndex;
        }

        private void bodyBullet_SelectedIndexChanged(object sender, EventArgs e)
        {
			SelectedBody.Body.Bullet = bodyBullet.Checked;
        }

        private void bodyFixedRotation_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedBody.Body.FixedRotation = bodyFixedRotation.Checked;
        }

        private void bodyInertiaScale_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            SelectedBody.Body.InertiaScale = (float)e.NewValue;
        }

        private void numericUpDown5_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            SelectedBody.Body.LinearDamping = (float)e.NewValue;
        }

        private void bodyLinearVelX_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            SelectedBody.Body.LinearVelocity = new Vec2((float)e.NewValue, SelectedBody.Body.LinearVelocity.Y);
        }

        private void bodyLinearVelY_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            SelectedBody.Body.LinearVelocity = new Vec2(SelectedBody.Body.LinearVelocity.X, (float)e.NewValue);
        }

        private void bodyPositionX_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            SelectedBody.Body.Position =
                new Vec2((float)e.NewValue, SelectedBody.Body.Position.Y);
        }

        private void bodyPositionY_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            SelectedBody.Body.Position =
                new Vec2(SelectedBody.Body.Position.X, (float)e.NewValue);
        }

        private void bodyMass_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            MassData Mass = SelectedBody.Mass;
            Mass.Mass = (float)e.NewValue;
            SelectedBody.Mass = Mass;
        }

        private void bodyInertia_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            MassData Mass = SelectedBody.Mass;
            Mass.Inertia = (float)e.NewValue;
            SelectedBody.Mass = Mass;
        }

        private void bodyCenterX_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            MassData Mass = SelectedBody.Mass;
            Mass.Center = new Vec2((float)e.NewValue, Mass.Center.Y);
            SelectedBody.Mass = Mass;
        }

        private void bodyCenterY_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            MassData Mass = SelectedBody.Mass;
            Mass.Center = new Vec2(Mass.Center.X, (float)e.NewValue);
            SelectedBody.Mass = Mass;
        }    

        private void fixtureListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fixtureListBox.SelectedIndex == -1)
                return;

            //propertyGrid1.SelectedObject = bodies[listBox1.SelectedIndex];
			SelectedFixture = WorldObject.Fixtures[fixtureListBox.SelectedIndex];
            LoadFixtureObjectSettings();
        }

        public void LoadFixtureObjectSettings()
        {
            fixtureName.Text = SelectedFixture.Name;
            fixtureDensity.Value = Convert.ToDecimal(SelectedFixture.Fixture.Density);
            fixtureFriction.Value = Convert.ToDecimal(SelectedFixture.Fixture.Friction);
			fixtureIsSensor.Checked = SelectedFixture.Fixture.IsSensor;
            fixtureRestitution.Value = Convert.ToDecimal(SelectedFixture.Fixture.Restitution);
           // ShapeSerialized shape = new ShapeSerialized(SelectedFixture.Fixture.Shape,"");
            //if (WorldObject.Shapes.Count > 0)
            //{
                //fixtureShape.SelectedIndex = SelectedFixture.ShapeID;
            //}
            fixtureCategoryBits.Value = SelectedFixture.Fixture.Filter.CategoryBits;
            fixtureGroupIndex.Value = SelectedFixture.Fixture.Filter.GroupIndex;
            fixtureMaskBits.Value = SelectedFixture.Fixture.Filter.MaskBits;

            LoadShapeObjectSettings();
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            SelectedFixture.Name = fixtureName.Text;
        }

        private void fixtureIsSensor_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedFixture.Fixture.IsSensor = fixtureIsSensor.Checked;
        }

        private void fixtureDensity_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            SelectedFixture.Fixture.Density = (float)e.NewValue;
        }

        private void fixtureFriction_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            SelectedFixture.Fixture.Friction = (float)e.NewValue;
        }

        private void fixtureRestitution_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            SelectedFixture.Fixture.Restitution = (float)e.NewValue;
        }

        private void fixtureCategoryBits_ValueChanged(object sender, EventArgs e)
        {
            SelectedFixture.Fixture.Filter.CategoryBits = (ushort)fixtureCategoryBits.Value;
        }

        private void fixtureGroupIndex_ValueChanged(object sender, EventArgs e)
        {
            SelectedFixture.Fixture.Filter.CategoryBits = (ushort)fixtureCategoryBits.Value;
        }

        private void fixtureMaskBits_ValueChanged(object sender, EventArgs e)
        {
            SelectedFixture.Fixture.Filter.MaskBits = (ushort)fixtureMaskBits.Value;
        }

        private void fixtureShape_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        public void LoadShapeObjectSettings()
        {
            shapePanel.Controls.Clear();
            if (SelectedFixture.Fixture.Shape.ShapeType == ShapeType.Circle)
            {
                shapeType.SelectedIndex = 0;
                shapePanel.Controls.Add(circlePanel);
                CircleShape shape = (CircleShape)SelectedFixture.Fixture.Shape;
                circlePanel.circleRadius.Value = Convert.ToDecimal(shape.Radius);
                circlePanel.circlePositionX.Value = Convert.ToDecimal(shape.Position.X);
                circlePanel.circlePositionY.Value = Convert.ToDecimal(shape.Position.Y);
            }

            if (SelectedFixture.Fixture.Shape.ShapeType == ShapeType.Polygon)
                shapeType.SelectedIndex = 1;
        }

        public void circleRadius_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            CircleShape shape = (CircleShape)SelectedFixture.Fixture.Shape;
            shape.Radius = (float)e.NewValue;
            SelectedFixture.Fixture.Shape = shape;
        }

        public void circlePositionX_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            CircleShape shape = (CircleShape)SelectedFixture.Fixture.Shape;
            shape.Position = new Vec2( (float)e.NewValue ,shape.Position.Y);
            SelectedFixture.Fixture.Shape = shape;
        }

        public void circlePositionY_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            CircleShape shape = (CircleShape)SelectedFixture.Fixture.Shape;
            shape.Position = new Vec2(shape.Position.X, (float)e.NewValue);
            SelectedFixture.Fixture.Shape = shape;
        }

        private void shapeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (shapeType.SelectedIndex == 0 && !(SelectedFixture.Fixture.Shape is CircleShape))
                SelectedFixture.Fixture.Shape = new CircleShape();
            else if (shapeType.SelectedIndex == 1 && !(SelectedFixture.Fixture.Shape is PolygonShape))
                SelectedFixture.Fixture.Shape = new PolygonShape();

            LoadShapeObjectSettings();
        }

        private void bodyFixtureDelete_Click(object sender, EventArgs e)
        {
            SelectedBody.Fixtures.RemoveAt(bodyFixtureListBox.SelectedIndex);
            LoadBodyObjectSettings();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            FixtureDef fixture = new FixtureDef();
			FixtureDefSerialized newFixture = new FixtureDefSerialized(fixture, 0, "Fixture " + (WorldObject.Fixtures.Count).ToString());
			WorldObject.Fixtures.Add(newFixture);
			fixtureListBox.Items.Add("Fixture " + (WorldObject.Fixtures.Count - 1).ToString());
			bodyFixtureSelect.Items.Add("Fixture " + (WorldObject.Fixtures.Count - 1).ToString());
            newFixture.Fixture.Shape = new CircleShape();
        }

		void ClearListboxes()
		{
			bodyListBox.Items.Clear();
			fixtureListBox.Items.Clear();
			bodyFixtureListBox.Items.Clear();
		}

		private void loadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.RestoreDirectory = true;
				ofd.Filter = "Box2Scene XML Files|*.xml";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					ClearListboxes();
					WorldObject.LoadFromFile(ofd.FileName);

					for (int i = 0; i < WorldObject.Bodies.Count; ++i)
					{
						var body = WorldObject.Bodies[i];
						bodyListBox.Items.Add((string.IsNullOrEmpty(body.Name)) ? ("Body " + i.ToString()) : body.Name);
					}

					for (int i = 0; i < WorldObject.Fixtures.Count; ++i)
					{
						var fixture = WorldObject.Fixtures[i];
						fixtureListBox.Items.Add((string.IsNullOrEmpty(fixture.Name)) ? ("Fixture " + i.ToString()) : fixture.Name);
					}
				}
			}
		}

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (bodyListBox.SelectedIndex > -1 && bodyListBox.SelectedIndex < WorldObject.Bodies.Count)
            {
                WorldObject.Bodies.RemoveAt(bodyListBox.SelectedIndex);
                bodyListBox.Items.RemoveAt(bodyListBox.SelectedIndex);
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (bodyListBox.SelectedIndex > -1 && bodyListBox.SelectedIndex < WorldObject.Bodies.Count)
            {
                /**
                new BodyObject(WorldObject,
                    new BodyDefSerialized(WorldObject.Bodies[bodyListBox.SelectedIndex], new BodyDef(), new List<int> { }, "Body "))
                );

                BodyDefSerialized def = new BodyDefSerialized(WorldObject.Bodies[bodyListBox.SelectedIndex].Body, new BodyDef(), new List<int> { }, "Body "));
                BodyObject body = new BodyObject();

                WorldObject.Bodies.RemoveAt(bodyListBox.SelectedIndex);
                bodyListBox.Items.RemoveAt(bodyListBox.SelectedIndex);
                bodyListBox.SelectedIndex = WorldObject.Bodies.Count - 1;
                **/
            }
        }
	}
}