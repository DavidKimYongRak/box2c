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
		int width = 640;
		int height = 480;
		float settingsHz = 60.0f;
		float viewZoom = 1.0f;
		Vec2 viewCenter = new Vec2(0.0f, 20.0f);
		int tx, ty, tw, th;
		bool rMouseDown;
		Vec2 lastp;
		World _world;
		Thread simulationThread;
		TestDebugDraw debugDraw;
		WorldXmlDeserializer deserializer;
		BodyObject HoverBody = null, SelectedBody = null;
        FixtureDefSerialized SelectedFixture = null;
		List<BodyObject> bodies = new List<BodyObject>();
        public ShapeSerialized SelectedShape = null;
        CirclePanel circlePanel = new CirclePanel();

		public class BodyObject
		{
			public BodyDef Body
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			MassData _mass;
			public MassData Mass
			{
				get { return _mass; }
				set { _mass = value; }
			}

			bool _autoCalculate = true;

			public bool AutoMassRecalculate
			{
				get { return _autoCalculate; }
				set { _autoCalculate = value; }
			}

			NotifyList<FixtureDefSerialized> _fixtures = new NotifyList<FixtureDefSerialized>();
			public NotifyList<FixtureDefSerialized> Fixtures
			{
				get { return _fixtures; }
			}

			public IEnumerable<FixtureDef> OnlyFixtures
			{
				get
				{
					foreach (var x in _fixtures)
						yield return x.Fixture;
				}
			}

			void _fixtures_ObjectsRemoved(object sender, EventArgs e)
			{
				_mass = Body.ComputeMass(OnlyFixtures);
			}

			void _fixtures_ObjectsAdded(object sender, EventArgs e)
			{
				_mass = Body.ComputeMass(OnlyFixtures);
			}

			public BodyObject(WorldXmlDeserializer deserializer, BodyDefSerialized x)
			{
				Body = x.Body;
				Name = x.Name;

				// FIXME: name
				for (int i = 0; i < x.FixtureIDs.Count; ++i)
				{
					var fixture = deserializer.FixtureDefs[x.FixtureIDs[i]];
					fixture.Fixture.Shape = deserializer.Shapes[deserializer.FixtureDefs[x.FixtureIDs[i]].ShapeID].Shape;
					_fixtures.Add(fixture);
				}

				_mass = Body.ComputeMass(OnlyFixtures);
				_fixtures.ObjectsAdded += new EventHandler(_fixtures_ObjectsAdded);
				_fixtures.ObjectsRemoved += new EventHandler(_fixtures_ObjectsRemoved);
			}
		}

		bool _testing = false;

		public float ViewZoom
		{
			get { return viewZoom; }
			set
			{
				viewZoom = value;
				OnGLResize(width, height);
			}
		}

		void StartTest()
		{
			_testing = true;
			_world = new World(new Vec2(0, -10.0f), true);
			_world.DebugDraw = debugDraw;

			System.Collections.Generic.List<Body> bodies = new System.Collections.Generic.List<Body>();

			foreach (var x in this.bodies)
			{
				var body = _world.CreateBody(x.Body);

				bodies.Add(body);

				foreach (var f in x.Fixtures)
					body.CreateFixture(f.Fixture);

				body.MassData = x.Mass;
			}

			foreach (var j in deserializer.Joints)
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

		public void DrawString(int x, int y, string str)
		{
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glPushMatrix();
			Gl.glLoadIdentity();
			int w = (int)Main.GLWindow.Width;
			int h = (int)Main.GLWindow.Height;
			Glu.gluOrtho2D(0, w, h, 0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glPushMatrix();
			Gl.glLoadIdentity();

			Gl.glColor3f(0.9f, 0.6f, 0.6f);
			Gl.glRasterPos2i(x, y);

			foreach (var c in str)
				Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_HELVETICA_12, c);

			Gl.glPopMatrix();
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glPopMatrix();
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
		}

		public void DrawStringFollow(float x, float y, string str)
		{
			Gl.glColor3f(0.9f, 0.6f, 0.6f);
			Gl.glRasterPos2f(x, y);

			foreach (var c in str)
				Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_HELVETICA_12, c);
		}

		public Main()
		{
			StartPosition = FormStartPosition.Manual;
			InitializeComponent();

			//OnGLResize(_glcontrol.Width, _glcontrol.Height);
			Text = "Box2CS Level Editor - Box2D version " + Box2DVersion.Version.ToString();

			renderWindow = new RenderWindow(panel1.Handle, new ContextSettings(32, 0, 12));
			renderWindow.Resized += new EventHandler<SizeEventArgs>(render_Resized);
			renderWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(renderWindow_MouseButtonPressed);
			renderWindow.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(renderWindow_MouseButtonReleased);
			renderWindow.MouseMoved += new EventHandler<MouseMoveEventArgs>(renderWindow_MouseMoved);
			renderWindow.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(renderWindow_KeyPressed);
			renderWindow.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(renderWindow_KeyReleased);
			renderWindow.Show(true);

			OnGLResize((int)renderWindow.Width, (int)renderWindow.Height);
			Tao.FreeGlut.Glut.glutInit();

			updateDraw = UpdateDraw;

			Box2CS.Parsers.XMLFragmentParser.LoadFromFile("out.xml");

			deserializer = new WorldXmlDeserializer();
			using (System.IO.FileStream fs = new System.IO.FileStream("out.xml", System.IO.FileMode.Open))
				deserializer.Deserialize(fs);

            for (int i = 0; i < deserializer.Bodies.Count; ++i)
            {
                var x = deserializer.Bodies[i];
                x.Name = "Body " + i.ToString();
                bodyListBox.Items.Add(x.Name);

                bodies.Add(new BodyObject(deserializer, x));
            }

            for (int i = 0; i < deserializer.FixtureDefs.Count; ++i)
            {
                var x = deserializer.FixtureDefs[i];
                x.Name = "Fixture " + i.ToString();
                fixtureListBox.Items.Add(x.Name);
                
                bodyFixtureSelect.Items.Add(("Fixture " + i.ToString() + ((string.IsNullOrEmpty(x.Name)) ? "" : " (" + x.Name + ")")));
            }

			for (int i = 0; i < deserializer.Joints.Count; ++i)
			{
				var x = deserializer.Joints[i];
                x.Name = "Joint " + i.ToString();
                listBox4.Items.Add(x.Name);
			}
            for (int i = 0; i < deserializer.Shapes.Count; ++i)
            {
                var x = deserializer.Shapes[i];
                x.Name = "Shape " + i.ToString();
                shapeListBox.Items.Add(x.Name);
                fixtureShape.Items.Add(x.Name);
            }

			debugDraw = new TestDebugDraw();
			debugDraw.Flags = DebugFlags.Shapes | DebugFlags.Joints | DebugFlags.CenterOfMasses;
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

            Gl.glClearColor((float)BackColor.R / 255.0f, (float)BackColor.G / 255.0f, (float)BackColor.B / 255.0f, 1);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();

			// Draw
			//test.m_world.DrawDebugData();

			//DrawTest();

			if (!_testing)
			{
				foreach (var x in bodies)
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
					DrawStringFollow(p.X, p.Y, HoverBody.Name);
				}

				foreach (var x in deserializer.Joints)
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
					debugDraw.DrawJoint(x, a1, a2, deserializer.Bodies[x.BodyAIndex].Body, deserializer.Bodies[x.BodyBIndex].Body);
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

				Invoke(updateDraw);

				// Sleep it off
				Thread.Sleep(5);
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
				
			case KeyCode.T:
				if (!_testing)
					StartTest();
				else
					EndTest();
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
			foreach (var b in bodies)
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
				{
					SelectedBody = moused;
					//propertyGrid1.SelectedObject = moused;
					//propertyGrid1.Refresh();
				}
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
				OnGLResize(width, height);
				lastp = ConvertScreenToWorld(x, y);
			}

			CheckSelect(x, y);
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
			OnGLResize(width, height);
			NextGameTick = System.Environment.TickCount;
		}

		private void Main_Load(object sender, EventArgs e)
		{
			simulationThread = new Thread(SimulationLoop);
			simulationThread.Start();
            bodyListBox.SelectedIndex = 0;
            fixtureListBox.SelectedIndex = 0;
            shapeListBox.SelectedIndex = 0;
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
			SelectedBody = bodies[bodyListBox.SelectedIndex];
            LoadBodyObjectSettings();
		}

        public void LoadBodyObjectSettings()
        {
            bodyActive.SelectedIndex = Convert.ToInt32(SelectedBody.Body.Active);
            bodyAllowSleep.SelectedIndex = Convert.ToInt32(SelectedBody.Body.AllowSleep);
            bodyAngle.Value = Convert.ToDecimal(SelectedBody.Body.Angle);
            bodyAngularDamping.Value = Convert.ToDecimal(SelectedBody.Body.AngularDamping);
            bodyAngularVelocity.Value = Convert.ToDecimal(SelectedBody.Body.AngularVelocity);
            bodyAwake.SelectedIndex = Convert.ToInt32(SelectedBody.Body.Awake);
            bodyType.SelectedIndex = (int)SelectedBody.Body.BodyType;
            bodyBullet.SelectedIndex = Convert.ToInt32(SelectedBody.Body.Bullet);
            bodyFixedRotation.SelectedIndex = Convert.ToInt32(SelectedBody.Body.FixedRotation);
            bodyInertiaScale.Value = Convert.ToDecimal(SelectedBody.Body.InertiaScale);
            bodyLinearDamping.Value = Convert.ToDecimal(SelectedBody.Body.LinearDamping);
            bodyLinearVelX.Value = Convert.ToDecimal(SelectedBody.Body.LinearVelocity.X);
            bodyLinearVelY.Value = Convert.ToDecimal(SelectedBody.Body.LinearVelocity.Y);
            bodyPositionX.Value = Convert.ToDecimal(SelectedBody.Body.Position.X);
            bodyPositionY.Value = Convert.ToDecimal(SelectedBody.Body.Position.Y);

            bodyName.Text = SelectedBody.Name;

            bodyAutoMassRecalculate.SelectedIndex = Convert.ToInt32(SelectedBody.AutoMassRecalculate);

            bodyCenterX.Value = Convert.ToDecimal(SelectedBody.Mass.Center.X);
            bodyCenterY.Value = Convert.ToDecimal(SelectedBody.Mass.Center.Y);
            bodyMass.Value = Convert.ToDecimal(SelectedBody.Mass.Mass);
            bodyInertia.Value = Convert.ToDecimal(SelectedBody.Mass.Inertia);
        
            bodyFixtureListBox.Items.Clear();
            for (int i = 0; i < SelectedBody.Fixtures.Count; i ++) {
                FixtureDefSerialized fixDefSer = SelectedBody.Fixtures[i];
                bodyFixtureListBox.Items.Add(fixDefSer.Name);
            }

            int prevIndex = bodyFixtureSelect.SelectedIndex;

            bodyFixtureSelect.Items.Clear();
            for (int i = 0; i < deserializer.FixtureDefs.Count; i++)
            {
                FixtureDefSerialized fixDefSer = deserializer.FixtureDefs[i];
                bodyFixtureSelect.Items.Add(fixDefSer.Name);
            }
            if (prevIndex < 0 || prevIndex >= deserializer.FixtureDefs.Count)
            {
                prevIndex = 0;
            }
            bodyFixtureSelect.SelectedIndex = prevIndex;
        }

		private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox4.SelectedIndex == -1)
				return;

			propertyGrid4.SelectedObject = deserializer.Joints[listBox4.SelectedIndex].Joint;
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
            /**
			BodyForPropertyGrid bpg = (BodyForPropertyGrid)propertyGrid1.SelectedObject;

			if (bpg.AutoMassRecalculate && 
				e.ChangedItem.PropertyDescriptor.ComponentType.GetProperty(e.ChangedItem.PropertyDescriptor.Name)
				.GetCustomAttributes(typeof(RecalculateMassAttribute), true)
				.Length > 0)
			{
				var calc = bpg.Body.ComputeMass(bpg.Fixtures);

				if (calc != bpg.Mass)
				{
					bpg.Mass = calc;
					propertyGrid1.Refresh();
				}
			}
             * */
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
            bodies.Add(new BodyObject(deserializer, new BodyDefSerialized(null, new BodyDef(), new List<int> { }, "Body " + (bodies.Count).ToString())));
			bodyListBox.Items.Add("Body "+(bodies.Count-1).ToString());
		}

        private void propertyGrid2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FixtureDefSerialized fixDefSer = deserializer.FixtureDefs[bodyFixtureSelect.SelectedIndex];
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
            SelectedBody.AutoMassRecalculate = Convert.ToBoolean(bodyAutoMassRecalculate.SelectedIndex);
        }

        private void bodyActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedBody.Body.Active = Convert.ToBoolean(bodyActive.SelectedIndex);
        }

        private void bodyAllowSleep_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedBody.Body.AllowSleep = Convert.ToBoolean(bodyAllowSleep.SelectedIndex);
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
            SelectedBody.Body.Awake = Convert.ToBoolean(bodyAwake.SelectedIndex);
        }

        private void bodyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedBody.Body.BodyType = (BodyType)bodyType.SelectedIndex;
        }

        private void bodyBullet_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedBody.Body.Bullet = Convert.ToBoolean(bodyBullet.SelectedIndex);
        }

        private void bodyFixedRotation_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedBody.Body.FixedRotation = Convert.ToBoolean(bodyFixedRotation.SelectedIndex);
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
            SelectedFixture = deserializer.FixtureDefs[fixtureListBox.SelectedIndex];
            LoadFixtureObjectSettings();
        }
        public void LoadFixtureObjectSettings()
        {
            fixtureName.Text = SelectedFixture.Name;
            fixtureDensity.Value = Convert.ToDecimal(SelectedFixture.Fixture.Density);
            fixtureFriction.Value = Convert.ToDecimal(SelectedFixture.Fixture.Friction);
            fixtureIsSensor.SelectedIndex = Convert.ToInt32(SelectedFixture.Fixture.IsSensor);
            fixtureRestitution.Value = Convert.ToDecimal(SelectedFixture.Fixture.Restitution);
            ShapeSerialized shape = new ShapeSerialized(SelectedFixture.Fixture.Shape,"");
            fixtureShape.SelectedIndex = SelectedFixture.ShapeID;
            fixtureCategoryBits.Value = SelectedFixture.Fixture.Filter.CategoryBits;
            fixtureGroupIndex.Value = SelectedFixture.Fixture.Filter.GroupIndex;
            fixtureMaskBits.Value = SelectedFixture.Fixture.Filter.MaskBits;
        }
        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            SelectedFixture.Name = fixtureName.Text;
        }

        private void fixtureIsSensor_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedFixture.Fixture.IsSensor = Convert.ToBoolean(fixtureIsSensor.SelectedIndex);
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
            SelectedFixture.ShapeID = fixtureShape.SelectedIndex;
        }

        private void shapeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fixtureListBox.SelectedIndex == -1)
                return;

            SelectedShape = deserializer.Shapes[shapeListBox.SelectedIndex];

            LoadShapeObjectSettings();
        }
        public void LoadShapeObjectSettings()
        {
            shapeName.Text = SelectedShape.Name;
            shapePanel.Controls.Clear();
            if (SelectedShape.Shape.ShapeType == ShapeType.Circle)
            {
                shapeType.SelectedIndex = 0;
                shapePanel.Controls.Add(circlePanel);
                CircleShape shape = (CircleShape)SelectedShape.Shape;
                circlePanel.circleRadius.Value = Convert.ToDecimal(shape.Radius);
                circlePanel.circlePositionX.Value = Convert.ToDecimal(shape.Position.X);
                circlePanel.circlePositionY.Value = Convert.ToDecimal(shape.Position.Y);
            }
            if (SelectedShape.Shape.ShapeType == ShapeType.Polygon)
            {
                shapeType.SelectedIndex = 1;
            }
        }
        public void circleRadius_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            CircleShape shape = (CircleShape)SelectedShape.Shape;
            shape.Radius = (float)e.NewValue;
            SelectedShape.Shape = shape;
        }
        public void circlePositionX_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            CircleShape shape = (CircleShape)SelectedShape.Shape;
            shape.Position = new Vec2( (float)e.NewValue ,shape.Position.Y);
            SelectedShape.Shape = shape;
        }
        public void circlePositionY_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            CircleShape shape = (CircleShape)SelectedShape.Shape;
            shape.Position = new Vec2(shape.Position.X, (float)e.NewValue);
            SelectedShape.Shape = shape;
        }

        private void shapeName_TextChanged(object sender, EventArgs e)
        {
            SelectedShape.Name = shapeName.Text;
        }

        private void shapeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (shapeType.SelectedIndex == 0 && !(SelectedShape.Shape is CircleShape))
                SelectedShape.Shape = new CircleShape();
            else if (shapeType.SelectedIndex == 1 && !(SelectedShape.Shape is PolygonShape))
                SelectedShape.Shape = new PolygonShape();

            LoadShapeObjectSettings();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            CircleShape shape = new CircleShape();
            ShapeSerialized newShape = new ShapeSerialized(shape,"Shape " + (deserializer.Shapes.Count).ToString());
            deserializer.Shapes.Add(newShape);
            shapeListBox.Items.Add("Shape " + (deserializer.Shapes.Count-1).ToString());
            fixtureShape.Items.Add("Shape " + (deserializer.Shapes.Count - 1).ToString());
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            FixtureDef fixture = new FixtureDef();
            FixtureDefSerialized newFixture = new FixtureDefSerialized(fixture, 0, "Fixture " + (deserializer.FixtureDefs.Count).ToString());
            deserializer.FixtureDefs.Add(newFixture);
            fixtureListBox.Items.Add("Fixture " + (deserializer.FixtureDefs.Count - 1).ToString());
            bodyFixtureSelect.Items.Add("Fixture " + (deserializer.FixtureDefs.Count - 1).ToString());
        }

        private void bodyFixtureDelete_Click(object sender, EventArgs e)
        {
            SelectedBody.Fixtures.RemoveAt(bodyFixtureListBox.SelectedIndex);
            LoadBodyObjectSettings();
        }
	}

	public class HolyCrapControl : Control
	{
	}
}