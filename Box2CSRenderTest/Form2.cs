using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Tao.OpenGl;
using Tao.Platform.Windows;

using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;

using Tao.DevIl;
using SFML.Window;
using Tao.FreeGlut;

namespace Box2DSharpRenderTest
{
	public partial class Form2 : Form
	{
		public Form2()
		{
			InitializeComponent();
		}

		class GDIDebugThing
		{
			public void DrawPolygon(Vector2[] vertices, int vertexCount, Color color)
			{
				Gl.glColor3ub(color.R, color.G, color.B);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				for (int i = 0; i < vertexCount; ++i)
				{
					Gl.glVertex2f(vertices[i].X, vertices[i].Y);
				}
				Gl.glEnd();
			}

			public void DrawSolidPolygon(Vector2[] vertices, int vertexCount, Color color)
			{
				Gl.glColor4ub((byte)(color.R / 2), (byte)(color.G / 2), (byte)(color.B / 2), 127);
				Gl.glBegin(Gl.GL_TRIANGLE_FAN);
				for (int i = 0; i < vertexCount; ++i)
				{
					Gl.glVertex2f(vertices[i].X, vertices[i].Y);
				}
				Gl.glEnd();

				Gl.glColor4ub(color.R, color.G, color.B, 255);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				for (int i = 0; i < vertexCount; ++i)
				{
					Gl.glVertex2f(vertices[i].X, vertices[i].Y);
				}
				Gl.glEnd();
			}

			public void DrawCircle(Vector2 center, float radius, Color color)
			{
				const float k_segments = 16.0f;
				const float k_increment = (float)(2.0f * System.Math.PI / k_segments);
				float theta = 0.0f;
				Gl.glColor3ub(color.R, color.G, color.B);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				for (int i = 0; i < k_segments; ++i)
				{
					Vector2 v = center + radius * new Vector2((float)System.Math.Cos(theta), (float)System.Math.Sin(theta));
					Gl.glVertex2f(v.X, v.Y);
					theta += k_increment;
				}
				Gl.glEnd();
			}

			public void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color)
			{
				const float k_segments = 16.0f;
				const float k_increment = (float)(2.0f * System.Math.PI / k_segments);
				float theta = 0.0f;
				Gl.glColor4ub((byte)(color.R / 2), (byte)(color.G / 2), (byte)(color.B / 2), 127);
				Gl.glBegin(Gl.GL_TRIANGLE_FAN);
				for (int i = 0; i < k_segments; ++i)
				{
					Vector2 v = center + radius * new Vector2((float)System.Math.Cos(theta), (float)System.Math.Sin(theta));
					Gl.glVertex2f(v.X, v.Y);
					theta += k_increment;
				}
				Gl.glEnd();

				theta = 0.0f;
				Gl.glColor4ub(color.R, color.G, color.B, 255);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				for (int i = 0; i < k_segments; ++i)
				{
					Vector2 v = center + radius * new Vector2((float)System.Math.Cos(theta), (float)System.Math.Sin(theta));
					Gl.glVertex2f(v.X, v.Y);
					theta += k_increment;
				}
				Gl.glEnd();

				Vector2 p = center + radius * axis;
				Gl.glBegin(Gl.GL_LINES);
				Gl.glVertex2f(center.X, center.Y);
				Gl.glVertex2f(p.X, p.Y);
				Gl.glEnd();
			}

			public void DrawSegment(Vector2 p1, Vector2 p2, Color color)
			{
				Gl.glColor3ub(color.R, color.G, color.B);
				Gl.glBegin(Gl.GL_LINES);
				Gl.glVertex2f(p1.X, p1.Y);
				Gl.glVertex2f(p2.X, p2.Y);
				Gl.glEnd();
			}

			public void DrawTransform(Transform xf)
			{
				Vector2 p1 = xf.Position, p2;
				const float k_axisScale = 0.4f;
				Gl.glBegin(Gl.GL_LINES);

				Gl.glColor3ub(255, 0, 0);
				Gl.glVertex2f(p1.X, p1.Y);
				p2 = p1 + k_axisScale * xf.R.Col1;
				Gl.glVertex2f(p2.X, p2.Y);

				Gl.glColor3ub(0, 255, 0);
				Gl.glVertex2f(p1.X, p1.Y);
				p2 = p1 + k_axisScale * xf.R.Col2;
				Gl.glVertex2f(p2.X, p2.Y);

				Gl.glEnd();
			}

			public void DrawPoint(Vector2 p, float size, Color color)
			{
				Gl.glPointSize(size);
				Gl.glBegin(Gl.GL_POINTS);
				Gl.glColor3ub(color.R, color.G, color.B);
				Gl.glVertex2f(p.X, p.Y);
				Gl.glEnd();
				Gl.glPointSize(1.0f);
			}

			public void DrawAABB(AABB aabb, Color c)
			{
				Gl.glColor3ub(c.R, c.G, c.B);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				Gl.glVertex2f(aabb.LowerBound.X, aabb.LowerBound.Y);
				Gl.glVertex2f(aabb.UpperBound.X, aabb.LowerBound.Y);
				Gl.glVertex2f(aabb.UpperBound.X, aabb.UpperBound.Y);
				Gl.glVertex2f(aabb.LowerBound.X, aabb.UpperBound.Y);
				Gl.glEnd();
			}

			static Vector2[] drawVertices = new Vector2[8];
			public void DrawSolidPolygon(Shape shape, Transform xf, Color color)
			{
				switch (shape.ShapeType)
				{
				case ShapeType.Circle:
					{
						CircleShape circle = (CircleShape)shape;

						Vector2 center = MathUtils.Multiply(ref xf, circle.Position);
						float radius = circle.Radius;
						Vector2 axis = xf.R.Col1;

						DrawSolidCircle(center, radius, axis, color);
					}
					break;

				case ShapeType.Polygon:
					{
						PolygonShape poly = (PolygonShape)shape;
						int vertexCount = poly.Vertices.Count;
						//b2Assert(vertexCount <= b2_maxPolygonVertices);
						for (int i = 0; i < vertexCount; ++i)
							drawVertices[i] = MathUtils.Multiply(ref xf, poly.Vertices[i]);

						DrawSolidPolygon(drawVertices, vertexCount, color);
					}
					break;
				}
			}
		}

		static World world;

		bool _slowmotion = false;

		GDIDebugThing _debugDraw;

		public class Player
		{
			Biped _biped;

			public Biped Biped
			{
				get { return _biped; }
			}

			public Player(World world, Vector2 position, float xScale, float yScale)
			{
				_biped = new Biped(this, world, position, xScale, yScale);
			}

			bool _stickingLegs, _stickingHands;

			public bool StickingLegs
			{
				get { return _stickingLegs; }
				set { _stickingLegs = value; }
			}

			public bool StickingHands
			{
				get { return _stickingHands; }
				set { _stickingHands = value; }
			}

			RevoluteJoint[] _welds = new RevoluteJoint[4];

			public RevoluteJoint[] Welds
			{
				get { return _welds; }
			}

			bool[] _weldDefs = new bool[4];

			public bool[] WeldDefs
			{
				get { return _weldDefs; }
			}
		}

		static Player[] players = new Player[2];

		struct NetworkOptions
		{
			public string Name;
			public string IP;
			public bool Hosting;
		}

		Networking.NetworkServer server;
		Networking.NetworkClient client;
		NetworkOptions networkOptions = new NetworkOptions();

		float _currentZoom = 7;

		SFML.Graphics.RenderWindow renderWindow;
		System.Threading.Thread simulationThread;

		RevoluteJoint EasyRevoluteJoint (Body bodyA, Body bodyB, Vector2 worldPos)
		{
			return new RevoluteJoint(bodyA, bodyB, bodyA.GetLocalPoint(ref worldPos), bodyB.GetLocalPoint(ref worldPos));
		}

		bool BeginContact(Contact contact)
		{
			if (contact.FixtureA.Body.UserData == null ||
					contact.FixtureB.Body.UserData == null)
				return true;

			if (contact.FixtureA.Body.UserData is BipedBodyDescriptor ||
					contact.FixtureB.Body.UserData is BipedBodyDescriptor)
			{
				var bipedBody = (contact.FixtureA.Body.UserData is BipedBodyDescriptor) ? contact.FixtureA.Body : contact.FixtureB.Body;
				var biped = (BipedBodyDescriptor)bipedBody.UserData;
				var other = (bipedBody == contact.FixtureB.Body) ? contact.FixtureA.Body : contact.FixtureB.Body;
				Vector2 wmNormal;
				FixedArray2<Vector2> wmPoints;

				contact.GetWorldManifold(out wmNormal, out wmPoints);

				switch (biped.FixtureIndex)
				{
				case BipedFixtureIndex.LFoot:
					if (biped.Biped.OwnedPlayer.StickingLegs &&
							biped.Biped.OwnedPlayer.Welds[0] == null &&
							biped.Biped.OwnedPlayer.WeldDefs[0] == false)
					{
						var jd = biped.Biped.OwnedPlayer.Welds[0] = EasyRevoluteJoint(biped.Biped.Bodies[(int)biped.FixtureIndex], other, wmPoints[0]);
						jd.LowerLimit = -0.5f * (float)Math.PI; // -90 degrees
						jd.UpperLimit = 0.25f * (float)Math.PI; // 45 degrees
						jd.LimitEnabled = true;
						jd.CollideConnected = true;

						biped.Biped.OwnedPlayer.WeldDefs[0] = true;
					}
					break;
				case BipedFixtureIndex.RFoot:
					if (biped.Biped.OwnedPlayer.StickingLegs && 
							biped.Biped.OwnedPlayer.Welds[1] == null &&
							biped.Biped.OwnedPlayer.WeldDefs[1] == false)
					{
						var jd = biped.Biped.OwnedPlayer.Welds[1] = EasyRevoluteJoint(biped.Biped.Bodies[(int)biped.FixtureIndex], other, wmPoints[0]);
						jd.LowerLimit = -0.5f * (float)Math.PI; // -90 degrees
						jd.UpperLimit = 0.25f * (float)Math.PI; // 45 degrees
						jd.LimitEnabled = true;
						jd.CollideConnected = true;

						biped.Biped.OwnedPlayer.WeldDefs[1] = true;
					}
					break;
				case BipedFixtureIndex.LHand:
					if (biped.Biped.OwnedPlayer.StickingHands &&
							biped.Biped.OwnedPlayer.Welds[2] == null &&
							biped.Biped.OwnedPlayer.WeldDefs[2] == false)
					{
						var jd = biped.Biped.OwnedPlayer.Welds[2] = EasyRevoluteJoint(biped.Biped.Bodies[(int)biped.FixtureIndex], other, wmPoints[0]);
						jd.LowerLimit = -0.5f * (float)Math.PI; // -90 degrees
						jd.UpperLimit = 0.25f * (float)Math.PI; // 45 degrees
						jd.LimitEnabled = true;
						jd.CollideConnected = true;

						biped.Biped.OwnedPlayer.WeldDefs[2] = true;
					}
					break;
				case BipedFixtureIndex.RHand:
					if (biped.Biped.OwnedPlayer.StickingHands && 
							biped.Biped.OwnedPlayer.Welds[3] == null &&
							biped.Biped.OwnedPlayer.WeldDefs[3] == false)
					{
						var jd = biped.Biped.OwnedPlayer.Welds[3] = EasyRevoluteJoint(biped.Biped.Bodies[(int)biped.FixtureIndex], other, wmPoints[0]);
						jd.LowerLimit = -0.5f * (float)Math.PI; // -90 degrees
						jd.UpperLimit = 0.25f * (float)Math.PI; // 45 degrees
						jd.LimitEnabled = true;
						jd.CollideConnected = true;

						biped.Biped.OwnedPlayer.WeldDefs[3] = true;
					}
					break;
				}
			}

			return true;
		}

		struct GroundBodyDescriptor
		{
		}

		private void Form2_Load(object sender, EventArgs e)
		{
			using (NetworkDialog dialog = new NetworkDialog())
			{
				dialog.ShowDialog();

				if (!dialog.checkBox1.Checked && (string.IsNullOrEmpty(dialog.textBox2.Text) ||
					string.IsNullOrEmpty(dialog.textBox1.Text)))
				{
					Close();
					return;
				}

				networkOptions.Name = dialog.textBox1.Text;
				networkOptions.IP = dialog.textBox2.Text;
				networkOptions.Hosting = dialog.checkBox1.Checked;
			}

			if (networkOptions.Hosting)
				server = new Networking.NetworkServer();
			else
				client = new Networking.NetworkClient(System.Net.IPAddress.Parse(networkOptions.IP), networkOptions.Name);

			renderWindow = new SFML.Graphics.RenderWindow(pictureBox1.Handle, new ContextSettings(32, 0, 12));
			renderWindow.Resized += new EventHandler<SizeEventArgs>(render_Resized);
			//renderWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(renderWindow_MouseButtonPressed);
			//renderWindow.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(renderWindow_MouseButtonReleased);
			//renderWindow.MouseMoved += new EventHandler<MouseMoveEventArgs>(renderWindow_MouseMoved);
			renderWindow.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(renderWindow_KeyPressed);
			renderWindow.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(renderWindow_KeyReleased);
			renderWindow.Show(true);

			InitOpenGL(pictureBox1.Size, _currentZoom, PointF.Empty);
			Tao.FreeGlut.Glut.glutInit();
			
			Il.ilInit();
			Ilut.ilutInit();

			_debugDraw = new GDIDebugThing();

			if (networkOptions.Hosting)
			{
				// Define the gravity vector.
				Vector2 gravity = new Vector2(0.0f, -17.0f);

				// Construct a world object, which will hold and simulate the rigid bodies.
				FarseerPhysics.Settings.UseFPECollisionCategories = true;
				FarseerPhysics.Settings.PositionIterations = 8;
				FarseerPhysics.Settings.VelocityIterations = 16;

				world = new World(gravity);
				world.ContactManager.BeginContact += BeginContact;

				{
					float bottom = (float)(86);
					float left = (float)(86);

					FixtureFactory.CreateEdge(world, new Vector2(-left, -bottom), new Vector2(left, -bottom)).Body.UserData = new GroundBodyDescriptor();
					FixtureFactory.CreateEdge(world, new Vector2(-left, bottom), new Vector2(-left, -bottom)).Body.UserData = new GroundBodyDescriptor();
					FixtureFactory.CreateEdge(world, new Vector2(left, bottom), new Vector2(left, -bottom)).Body.UserData = new GroundBodyDescriptor();
					FixtureFactory.CreateEdge(world, new Vector2(left, bottom), new Vector2(-left, bottom)).Body.UserData = new GroundBodyDescriptor();
				}

				players[0] = new Player(world, new Vector2(-24, 0), 9, 9);
				players[1] = new Player(world, new Vector2(24, 0), -9, 9);
			}

			simulationThread = new System.Threading.Thread(SimulationLoop);
			simulationThread.Start();
		}

		Point CursorPos;
		public const float settingsHz = 25;
		public const int settingsHzInMs = (int)(1000.0f / settingsHz);

		public static void DrawStringFollow(float x, float y, string str, bool center)
		{
			if (center)
			{
				int wid = 0;
				foreach (var c in str)
					wid += Glut.glutBitmapWidth(Glut.GLUT_BITMAP_HELVETICA_12, c);

				x -= (wid) / 14;
			}
			Gl.glColor3f(0.9f, 0.6f, 0.6f);
			Gl.glRasterPos2f(x, y);

			foreach (var c in str)
				Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_HELVETICA_12, c);
		}

		public static void DrawString(int x, int y, string str)
		{
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glPushMatrix();
			Gl.glLoadIdentity();
			int w = (int)Program.MainForm.pictureBox1.Width;
			int h = (int)Program.MainForm.pictureBox1.Height;
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

				Invoke((Action)delegate() { UpdateDraw(); });

				mutex.WaitOne();
				mutex.ReleaseMutex();

				// Sleep it off
				System.Threading.Thread.Sleep(2);
		
			}
		}

		Dictionary<KeyCode, bool> _keyRepeats = new Dictionary<KeyCode, bool>();

		void renderWindow_KeyReleased(object sender, SFML.Window.KeyEventArgs e)
		{
			switch (e.Code)
			{
			case KeyCode.W:
				_gameKeys &= ~GameKeys.Up;
				break;
			case KeyCode.S:
				_gameKeys &= ~GameKeys.Down;
				break;
			case KeyCode.A:
				_gameKeys &= ~GameKeys.Left;
				break;
			case KeyCode.D:
				_gameKeys &= ~GameKeys.Right;
				break;
			case KeyCode.L:
				_gameKeys &= ~GameKeys.Hands;
				break;
			case KeyCode.K:
				_gameKeys &= ~GameKeys.Legs;
				break;
			case KeyCode.M:
				_gameKeys &= ~GameKeys.StickLegs;
				break;
			case KeyCode.N:
				_gameKeys &= ~GameKeys.StickHands;
				break;
			}

			_keyRepeats.Remove(e.Code);
		}

		bool typing;
		string currentString = "";

		static char CharFromKeyCode(SFML.Window.KeyEventArgs e)
		{
			switch (e.Code)
			{
			case KeyCode.Period:
				return (e.Shift) ? '>' : '.';
			case KeyCode.BackSlash:
				return (e.Shift) ? '?' : '/';
			case KeyCode.Comma:
				return (e.Shift) ? '<' : ',';
			case KeyCode.Dash:
				return (e.Shift) ? '_' : '-';
			case KeyCode.Equal:
				return (e.Shift) ? '+' : '=';
			case KeyCode.LBracket:
				return (e.Shift) ? '{' : '[';
			case KeyCode.Quote:
				return (e.Shift) ? '"' : '\'';
			case KeyCode.RBracket:
				return (e.Shift) ? '}' : ']';
			case KeyCode.SemiColon:
				return (e.Shift) ? ':' : ';';
			case KeyCode.Slash:
				return (e.Shift) ? '|' : '\\';
			case KeyCode.Tilde:
				return '~';
			}

			return (char)e.Code;
		}

		void renderWindow_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
		{
			if (typing)
			{
				char cFromCode = CharFromKeyCode(e);
				if (e.Code == KeyCode.Back)
				{
					if (currentString.Length != 0)
						currentString = currentString.Remove(currentString.Length - 1);
				}
				else if (e.Code == KeyCode.Return)
				{
					typing = !typing;

					if (!string.IsNullOrEmpty(currentString))
						client.SendText(currentString);

					currentString = "";
					return;
				}
				else if (e.Code == KeyCode.Space)
					currentString += ' ';
				else if (char.IsPunctuation(cFromCode) || char.IsLetterOrDigit(cFromCode) || char.IsSymbol(cFromCode))
					currentString += cFromCode;

				return;
			}

			if (_keyRepeats.ContainsKey(e.Code))
				return;

			switch (e.Code)
			{
			case KeyCode.Z:
				_drawDebug = !_drawDebug;
				break;
			case KeyCode.W:
				_gameKeys |= GameKeys.Up;
				break;
			case KeyCode.S:
				_gameKeys |= GameKeys.Down;
				break;
			case KeyCode.A:
				_gameKeys |= GameKeys.Left;
				break;
			case KeyCode.D:
				_gameKeys |= GameKeys.Right;
				break;
			case KeyCode.L:
				_gameKeys |= GameKeys.Hands;
				break;
			case KeyCode.K:
				_gameKeys |= GameKeys.Legs;
				break;
			//case Keys.H:
			//	new Bullet(players[0].Biped);
			//	break;
			//case Keys.O:
			//	_slowmotion = !_slowmotion;
			//	break;
			case KeyCode.U:
				_gameKeys |= GameKeys.StiffToggle;
				break;
			case KeyCode.T:
				typing = !typing;
				break;
			case KeyCode.M:
				_gameKeys |= GameKeys.StickLegs;
				break;
			case KeyCode.N:
				_gameKeys |= GameKeys.StickHands;
				break;
			}

			_keyRepeats.Add(e.Code, true);
		}

		/*void renderWindow_MouseMoved(object sender, MouseMoveEventArgs e)
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
		}*/

		void render_Resized(object sender, SizeEventArgs e)
		{
			OnGLResize();
		}

		[Flags]
		public enum GameKeys
		{
			Up = 1,
			Right = 2,
			Down = 4,
			Left = 8,
			Legs = 16,
			Hands = 32,
			StickLegs = 64,
			StickHands = 128,
			StiffToggle = 256,
		}
		GameKeys _oldGameKeys = 0, _gameKeys = 0;

		BipedModel player1Def = new BipedModel(9, 9),
			player2Def = new BipedModel(-9, 9);

		void Simulate()
		{
			if (networkOptions.Hosting)
			{
				// Prepare for simulation. Typically we use a time step of 1/60 of a
				// second (60Hz) and 10 iterations. This provides a high quality simulation
				// in most game scenarios.
				float timeStep = 1.0f / settingsHz;

				const int MoveSpeed = 450 * 9;

				BipedFixtureIndex[] fixturesToMove;
				float speedMultiplier = 1;

				if (server.Clients.Count != 0)
				{
					for (int i = 0; i < 2; ++i)
					{
						if (server.Clients.Count == i)
							break;

						if ((server.Clients[i].Keys & GameKeys.Legs) != 0)
						{
							fixturesToMove = new BipedFixtureIndex[] { BipedFixtureIndex.LFoot, BipedFixtureIndex.RFoot };
							speedMultiplier = 0.25f;
						}
						else if ((server.Clients[i].Keys & GameKeys.Hands) != 0)
						{
							fixturesToMove = new BipedFixtureIndex[] { BipedFixtureIndex.LHand, BipedFixtureIndex.RHand };
							speedMultiplier = 0.25f;
						}
						else
							fixturesToMove = new BipedFixtureIndex[] { BipedFixtureIndex.Chest };

						if ((server.Clients[i].Keys & GameKeys.Up) != 0)
						{
							foreach (var x in fixturesToMove)
								players[i].Biped.Bodies[(int)x].ApplyForce(new Vector2(0, MoveSpeed * speedMultiplier), players[i].Biped.Bodies[(int)x].WorldCenter);
						}
						if ((server.Clients[i].Keys & GameKeys.Down) != 0)
						{
							foreach (var x in fixturesToMove)
								players[i].Biped.Bodies[(int)x].ApplyForce(new Vector2(0, (-MoveSpeed / 2) * speedMultiplier), players[i].Biped.Bodies[(int)x].WorldCenter);
						}
						if ((server.Clients[i].Keys & GameKeys.Left) != 0)
						{
							foreach (var x in fixturesToMove)
								players[i].Biped.Bodies[(int)x].ApplyForce(new Vector2((-MoveSpeed / 2) * speedMultiplier, 0), players[i].Biped.Bodies[(int)x].WorldCenter);
						}
						if ((server.Clients[i].Keys & GameKeys.Right) != 0)
						{
							foreach (var x in fixturesToMove)
								players[i].Biped.Bodies[(int)x].ApplyForce(new Vector2((MoveSpeed / 2) * speedMultiplier, 0), players[i].Biped.Bodies[(int)x].WorldCenter);
						}

						players[i].StickingHands = (server.Clients[i].Keys & GameKeys.StickHands) != 0;
						players[i].StickingLegs = (server.Clients[i].Keys & GameKeys.StickLegs) != 0;

						if ((server.Clients[i].Keys & GameKeys.StiffToggle) != 0)
						{
							players[i].Biped.StickBody();
							server.Clients[i].Keys &= ~GameKeys.StiffToggle;
						}
					}
				}

				// This is our little game loop.
				// Instruct the world to perform a single step of simulation.
				// It is generally best to keep the time step and iterations fixed.
				world.Step(timeStep);

				for (int z = 0; z < 2; ++z)
				for (int i = 0; i < players[z].WeldDefs.Length; ++i)
				{
					if (players[z].Welds[i] != null && players[z].WeldDefs[i])
					{
						world.AddJoint(players[z].Welds[i]);
						players[z].WeldDefs[i] = false;
					}

					if (i < 2 && !players[z].StickingLegs ||
					i >= 2 && !players[z].StickingHands)
					{
						if (players[z].Welds[i] != null)
						{
							world.RemoveJoint(players[z].Welds[i]);
							players[z].Welds[i] = null;
						}
					}
				}

				server.Stream.Write((byte)Networking.EClientDataPacketType.FramePacket);
				server.Stream.Write(server.Frame);
				for (int i = 0; i < (int)BipedFixtureIndex.Max; ++i)
				{
					server.Stream.Write(players[0].Biped.Bodies[(int)i].Position.X);
					server.Stream.Write(players[0].Biped.Bodies[(int)i].Position.Y);
					server.Stream.Write(players[0].Biped.Bodies[(int)i].Rotation);

					server.Stream.Write(players[1].Biped.Bodies[(int)i].Position.X);
					server.Stream.Write(players[1].Biped.Bodies[(int)i].Position.Y);
					server.Stream.Write(players[1].Biped.Bodies[(int)i].Rotation);
				}
			}
			else
			{
				if (_oldGameKeys != _gameKeys)
				{
					client.Udp.Stream.Write((byte)Networking.EServerDataPacketType.ClientCmd);
					client.Udp.Stream.Write((int)_gameKeys);

					_oldGameKeys = _gameKeys;
				}

				if ((_gameKeys & GameKeys.StiffToggle) != 0)
				{
					_gameKeys &= ~GameKeys.StiffToggle;
					_oldGameKeys &= ~GameKeys.StiffToggle;
				}
			}

			if (networkOptions.Hosting)
			{
				server.Check();

				server.Frame++;
			}
			else
			{
				client.Check();

				client.Frame++;
			}
		}

		void OnGLResize()
		{
			InitOpenGL(pictureBox1.Size, _currentZoom, PointF.Empty);
		}

		static long NextGameTick = System.Environment.TickCount;
		static int gameFrame = 0;

		PointF offset;
		float zoom = 1;
		public void InitOpenGL(Size WidthHeight, float viewZoom, PointF ViewOffset)
		{
			offset = ViewOffset;
			zoom = viewZoom;

			int Width = WidthHeight.Width;
			int Height = WidthHeight.Height;
			Height = Height > 0 ? Height : 1;

			Gl.glDisable(Gl.GL_CULL_FACE);
			Gl.glClearColor(.5f, .5f, .5f, 1);

			Gl.glDepthFunc(Gl.GL_LEQUAL);										// Type Of Depth Testing
			Gl.glEnable(Gl.GL_DEPTH_TEST);									// Enable Depth Testing
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);			// Enable Alpha Blending (disable alpha testing)
			Gl.glEnable(Gl.GL_BLEND);											// Enable Blending       (disable alpha testing)
			Gl.glEnable(Gl.GL_TEXTURE_2D);									// Enable Texture Mapping
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);				// Really Nice Perspective Calculations

			float AspectRatio = (float)Width / (float)Height;
			Gl.glViewport(0, 0, Width, Height);
			Gl.glMatrixMode(Gl.GL_PROJECTION);

			Gl.glLoadIdentity();
			Gl.glOrtho(0, Width, Height, 0, -1, 1);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			NextGameTick = System.Environment.TickCount;
		}

		bool _drawDebug = true;
		long _renderFrame = 0;
		System.Threading.Mutex mutex = new System.Threading.Mutex();

		public void UpdateDraw()
		{
			mutex.WaitOne();

			// Process events
			renderWindow.DispatchEvents();

			// Clear the window
			renderWindow.Clear();

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();

			if (_drawDebug && networkOptions.Hosting)
			{
				Gl.glPushMatrix();
				//world.DrawDebugData();
				Gl.glPopMatrix();
			}
			else if (!networkOptions.Hosting)
			{
				float frac = 1.0f - (float)((DateTime.Now.TimeOfDay.TotalMilliseconds - client.NextFrameTime) / (double)settingsHzInMs);
				Vector2 centerPos = new Vector2();
				Vector2 sub = new Vector2();
				float realZoom = zoom;

				if (client.OldFrame.Transforms != null)
				{
					centerPos -= client.OldFrame.Transforms[(int)BipedFixtureIndex.Stomach, client.ConnectedIndex].Position + ((client.OldFrame.Transforms[(int)BipedFixtureIndex.Stomach, client.ConnectedIndex].Position - client.CurFrame.Transforms[(int)BipedFixtureIndex.Stomach, client.ConnectedIndex].Position) * frac);
					//sub = client.OldFrame.Transforms[(int)BipedFixtureIndex.Head, client.ConnectedIndex].Position - client.CurFrame.Transforms[(int)BipedFixtureIndex.Head, client.ConnectedIndex].Position;
				}

				//realZoom -= sub.LengthSquared();

				centerPos.X += (pictureBox1.Width / 2) / realZoom;
				centerPos.Y -= (pictureBox1.Height / 2) / realZoom;

				Gl.glScalef(realZoom, -realZoom, realZoom);
				Gl.glTranslatef(centerPos.X, centerPos.Y, 0);

				{
					float bottom = (float)(86);
					float left = (float)(86);

					_debugDraw.DrawSegment(new Vector2(-left, -bottom), new Vector2(left, -bottom), Color.Green);
					_debugDraw.DrawSegment(new Vector2(-left, bottom), new Vector2(-left, -bottom), Color.Green);
					_debugDraw.DrawSegment(new Vector2(left, bottom), new Vector2(left, -bottom), Color.Green);
					_debugDraw.DrawSegment(new Vector2(left, bottom), new Vector2(-left, bottom), Color.Green);
				}

				if (client.CurFrame.Transforms != null)
				{
					var color = Color.FromArgb(229, 178, 178);

					for (int i = 0; i < (int)BipedFixtureIndex.Max; ++i)
					{
						if (client.OldFrame.Transforms == null)
						{
							_debugDraw.DrawSolidPolygon(player1Def[(BipedFixtureIndex)i], client.CurFrame.Transforms[i, 0].ToTransform(), color);
							_debugDraw.DrawSolidPolygon(player2Def[(BipedFixtureIndex)i], client.CurFrame.Transforms[i, 1].ToTransform(), color);
						}
						else
						{
							for (int z = 0; z < 2; ++z)
							{
								var transform1_new = client.CurFrame.Transforms[i, z];
								var transform1_old = client.OldFrame.Transforms[i, z];
								transform1_new.Position = transform1_old.Position + ((transform1_old.Position - transform1_new.Position) * frac);
								transform1_new.Angle = transform1_old.Angle + ((transform1_old.Angle - transform1_new.Angle) * frac);
								_debugDraw.DrawSolidPolygon(((z == 0) ? player1Def : player2Def)[(BipedFixtureIndex)i], transform1_new.ToTransform(), color);

								if (i == (int)BipedFixtureIndex.Head)
								{
									DrawStringFollow(transform1_new.Position.X, transform1_new.Position.Y + 2, "Player", true);
								}
							}
						}
					}
				}

				Gl.glPushMatrix();
				int y = 250;
				int start = client.Console.Messages.Count;
				for (int i = 0; i < 6; ++i)
				{
					if ((start - i) <= 0)
						break;

					DrawString(0, y, client.Console.Messages[start - i - 1]);
					y -= 13;
				}

				if (typing)
					DrawString(0, 263, currentString + ((((int)((DateTime.Now.TimeOfDay.TotalMilliseconds * 2) / 1000) & 1) != 0) ? "|" : ""));
				Gl.glPopMatrix();
			}
		
			renderWindow.Display();

			_renderFrame++;

			mutex.ReleaseMutex();
		}

		private void Form2_FormClosing(object sender, FormClosingEventArgs e)
		{
			simulationThread.Abort();

			if (networkOptions.Hosting)
				server.Close();
			else
				client.Close();
		}
	}

	public static class Extensions
	{
		public static float ToDeg(this float rad)
		{
			return (float)(180.0f * rad / Math.PI);
		}

		public static float ToRad(this float deg)
		{
			return (float)(Math.PI * deg / 180.0f);
		}
	}

	public class BlankControl : Control
	{
	}
}
