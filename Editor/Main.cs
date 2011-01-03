﻿using System;
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
		BodyForPropertyGrid HoverBody = null, SelectedBody = null;
		List<BodyForPropertyGrid> bodies = new List<BodyForPropertyGrid>();

		public class BodyForPropertyGrid
		{
			[Description("The actual body.")]
			public BodyDef Body
			{
				get;
				set;
			}

			[Description("Name identifier for this body and connected fixtures.")]
			public string Name
			{
				get;
				set;
			}

			MassData _mass;
			[Description("Mass for this body. Empty the properties' contents to recalculate the mass data manually. Will automatically update if AutoMassRecalculate is true.")]
			public MassData Mass
			{
				get { return _mass; }
				set { if (value == MassData.Recalculate) _mass = Body.ComputeMass(Fixtures); else _mass = value; }
			}

			bool _autoCalculate = true;

			[DefaultValue(true)]
			[Description("Automatically recalculate the mass of the body if a fixture or shape is modified.")]
			public bool AutoMassRecalculate
			{
				get { return _autoCalculate; }
				set { _autoCalculate = value; }
			}

			FixtureDef[] _fixtures;
			[Description("List of fixtures attached to this body.")]
			public FixtureDef[] Fixtures
			{
				get { return _fixtures; }

				set
				{
					_fixtures = value;
					_mass = Body.ComputeMass(value);
				}
			}

			public BodyForPropertyGrid(WorldXmlDeserializer deserializer, BodyDefSerialized x)
			{
				Body = x.Body;
				Name = x.Name;

				var fixtures = new FixtureDef[x.FixtureIDs.Count];

				// FIXME: name
				for (int i = 0; i < x.FixtureIDs.Count; ++i)
				{
					fixtures[i] = deserializer.FixtureDefs[x.FixtureIDs[i]].Fixture.Clone();
					fixtures[i].Shape = deserializer.Shapes[deserializer.FixtureDefs[x.FixtureIDs[i]].ShapeID].Shape.Clone();
				}

				Fixtures = fixtures;
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
					body.CreateFixture(f);
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
				listBox1.Items.Add("Body "+i.ToString() + ((string.IsNullOrEmpty(x.Name)) ? "" : " ("+x.Name+")"));

				bodies.Add(new BodyForPropertyGrid(deserializer, x));
			}

			for (int i = 0; i < deserializer.Joints.Count; ++i)
			{
				var x = deserializer.Joints[i];
				listBox4.Items.Add(x.Joint.JointType.ToString() + " Joint "+i.ToString() + ((string.IsNullOrEmpty(x.Name)) ? "" : " ("+x.Name+")"));
			}

			deserializer.Shapes.Clear();
			deserializer.FixtureDefs.Clear();

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

						debugDraw.DrawShape(fixture, xf, color);
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

			BodyForPropertyGrid moused = null;
			foreach (var b in bodies)
			{
				foreach (var f in b.Fixtures)
				{
					if (f.Shape.TestPoint(new Transform(b.Body.Position, new Mat22(b.Body.Angle)), p))
						moused = b;
				}
			}

			if (MouseButtons == System.Windows.Forms.MouseButtons.Left)
				if (moused != null)
				{
					SelectedBody = moused;
					propertyGrid1.SelectedObject = moused;
					propertyGrid1.Refresh();
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
			if (listBox1.SelectedIndex == -1)
				return;

			propertyGrid1.SelectedObject = bodies[listBox1.SelectedIndex];
			SelectedBody = bodies[listBox1.SelectedIndex];
		}

		private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox4.SelectedIndex == -1)
				return;

			propertyGrid4.SelectedObject = deserializer.Joints[listBox4.SelectedIndex].Joint;
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
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
		}
	}

	public class HolyCrapControl : Control
	{
	}
}
