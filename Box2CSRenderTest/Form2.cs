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
using Box2CS;
using Tao.DevIl;
//using SFML.Graphics;
using SFML.Window;

namespace Box2DSharpRenderTest
{
	public partial class Form2 : Form
	{
		public Form2()
		{
			InitializeComponent();
		}

		class GDIDebugThing : DebugDraw
		{
			public override void DrawPolygon(Vec2[] vertices, int vertexCount, ColorF color)
			{
				Gl.glColor3f(color.R, color.G, color.B);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				for (int i = 0; i < vertexCount; ++i)
				{
					Gl.glVertex2f(vertices[i].X, vertices[i].Y);
				}
				Gl.glEnd();
			}

			public override void DrawSolidPolygon(Vec2[] vertices, int vertexCount, ColorF color)
			{
				Gl.glColor4f(0.5f * color.R, 0.5f * color.G, 0.5f * color.B, 0.5f);
				Gl.glBegin(Gl.GL_TRIANGLE_FAN);
				for (int i = 0; i < vertexCount; ++i)
				{
					Gl.glVertex2f(vertices[i].X, vertices[i].Y);
				}
				Gl.glEnd();

				Gl.glColor4f(color.R, color.G, color.B, 1.0f);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				for (int i = 0; i < vertexCount; ++i)
				{
					Gl.glVertex2f(vertices[i].X, vertices[i].Y);
				}
				Gl.glEnd();
			}

			public override void DrawCircle(Vec2 center, float radius, ColorF color)
			{
				const float k_segments = 16.0f;
				const float k_increment = (float)(2.0f * System.Math.PI / k_segments);
				float theta = 0.0f;
				Gl.glColor3f(color.R, color.G, color.B);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				for (int i = 0; i < k_segments; ++i)
				{
					Vec2 v = center + radius * new Vec2((float)System.Math.Cos(theta), (float)System.Math.Sin(theta));
					Gl.glVertex2f(v.X, v.Y);
					theta += k_increment;
				}
				Gl.glEnd();
			}

			public override void DrawSolidCircle(Vec2 center, float radius, Vec2 axis, ColorF color)
			{
				const float k_segments = 16.0f;
				const float k_increment = (float)(2.0f * System.Math.PI / k_segments);
				float theta = 0.0f;
				Gl.glColor4f(0.5f * color.R, 0.5f * color.G, 0.5f * color.B, 0.5f);
				Gl.glBegin(Gl.GL_TRIANGLE_FAN);
				for (int i = 0; i < k_segments; ++i)
				{
					Vec2 v = center + radius * new Vec2((float)System.Math.Cos(theta), (float)System.Math.Sin(theta));
					Gl.glVertex2f(v.X, v.Y);
					theta += k_increment;
				}
				Gl.glEnd();

				theta = 0.0f;
				Gl.glColor4f(color.R, color.G, color.B, 1.0f);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				for (int i = 0; i < k_segments; ++i)
				{
					Vec2 v = center + radius * new Vec2((float)System.Math.Cos(theta), (float)System.Math.Sin(theta));
					Gl.glVertex2f(v.X, v.Y);
					theta += k_increment;
				}
				Gl.glEnd();

				Vec2 p = center + radius * axis;
				Gl.glBegin(Gl.GL_LINES);
				Gl.glVertex2f(center.X, center.Y);
				Gl.glVertex2f(p.X, p.Y);
				Gl.glEnd();
			}

			public override void DrawSegment(Vec2 p1, Vec2 p2, ColorF color)
			{
				Gl.glColor3f(color.R, color.G, color.B);
				Gl.glBegin(Gl.GL_LINES);
				Gl.glVertex2f(p1.X, p1.Y);
				Gl.glVertex2f(p2.X, p2.Y);
				Gl.glEnd();
			}

			public override void DrawTransform(Transform xf)
			{
				Vec2 p1 = xf.Position, p2;
				const float k_axisScale = 0.4f;
				Gl.glBegin(Gl.GL_LINES);

				Gl.glColor3f(1.0f, 0.0f, 0.0f);
				Gl.glVertex2f(p1.X, p1.Y);
				p2 = p1 + k_axisScale * xf.R.Col1;
				Gl.glVertex2f(p2.X, p2.Y);

				Gl.glColor3f(0.0f, 1.0f, 0.0f);
				Gl.glVertex2f(p1.X, p1.Y);
				p2 = p1 + k_axisScale * xf.R.Col2;
				Gl.glVertex2f(p2.X, p2.Y);

				Gl.glEnd();
			}

			public void DrawPoint(Vec2 p, float size, ColorF color)
			{
				Gl.glPointSize(size);
				Gl.glBegin(Gl.GL_POINTS);
				Gl.glColor3f(color.R, color.G, color.B);
				Gl.glVertex2f(p.X, p.Y);
				Gl.glEnd();
				Gl.glPointSize(1.0f);
			}

			public void DrawAABB(AABB aabb, ColorF c)
			{
				Gl.glColor3f(c.R, c.G, c.B);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				Gl.glVertex2f(aabb.LowerBound.X, aabb.LowerBound.Y);
				Gl.glVertex2f(aabb.UpperBound.X, aabb.LowerBound.Y);
				Gl.glVertex2f(aabb.UpperBound.X, aabb.UpperBound.Y);
				Gl.glVertex2f(aabb.LowerBound.X, aabb.UpperBound.Y);
				Gl.glEnd();
			}

			static Vec2[] drawVertices = new Vec2[8];
			public void DrawSolidPolygon(Shape shape, Transform xf, ColorF color)
			{
				switch (shape.ShapeType)
				{
				case ShapeType.Circle:
					{
						CircleShape circle = (CircleShape)shape;

						Vec2 center = (xf * circle.Position);
						float radius = circle.Radius;
						Vec2 axis = xf.R.Col1;

						DrawSolidCircle(center, radius, axis, color);
					}
					break;

				case ShapeType.Polygon:
					{
						PolygonShape poly = (PolygonShape)shape;
						int vertexCount = poly.VertexCount;
						//b2Assert(vertexCount <= b2_maxPolygonVertices);
						for (int i = 0; i < vertexCount; ++i)
							drawVertices[i] = (xf * poly.Vertices[i]);

						DrawSolidPolygon(drawVertices, vertexCount, color);
					}
					break;
				}
			}
		}

		static World world;

		void MakeBox(Vec2 pos, float w, float h)
		{
			// Define the dynamic body. We set its position and call the body factory.
			BodyDef bodyDef = new BodyDef();
			{
				bodyDef.BodyType = BodyType.Dynamic;
				bodyDef.Position = pos;
				var body = world.CreateBody(bodyDef);

				// Define another box shape for our dynamic body.
				PolygonShape dynamicBox = new PolygonShape();
				{
					//dynamicBox.SetAsBox(w, h);

					dynamicBox.Vertices = new Vec2[]
					{
						new Vec2(-0.5f, 0.0f),
						new Vec2(0.5f, 0.0f),
						new Vec2(0.0f, 1.5f),
					};

					// Define the dynamic body fixture.
					FixtureDef fixtureDef = new FixtureDef();
					{
						fixtureDef.Shape = dynamicBox;

						// Set the box density to be non-zero, so it will be dynamic.
						fixtureDef.Density = 1.0f;

						// Override the default friction.
						fixtureDef.Friction = 0.3f;

						// Add the shape to the body.
						body.CreateFixture(fixtureDef);
					}
				}
			}
		}

		static Body MakeCircle(Vec2 pos, float radius)
		{
			// Define the dynamic body. We set its position and call the body factory.
			BodyDef bodyDef = new BodyDef();
			{
				bodyDef.BodyType = BodyType.Dynamic;
				bodyDef.Position = pos;
				var body = world.CreateBody(bodyDef);

				// Define another box shape for our dynamic body.
				CircleShape dynamicBox = new CircleShape();
				{
					dynamicBox.Radius = radius;

					// Define the dynamic body fixture.
					FixtureDef fixtureDef = new FixtureDef();
					{
						fixtureDef.Shape = dynamicBox;

						// Set the box density to be non-zero, so it will be dynamic.
						fixtureDef.Density = 1.0f;

						// Override the default friction.
						fixtureDef.Friction = 0.3f;

						// Add the shape to the body.
						body.CreateFixture(fixtureDef);

						return body;
					}
				}
			}
		}

		public class Filterer : ContactFilter
		{
			public override bool ShouldCollide(Fixture fixtureA, Fixture fixtureB)
			{
				if (fixtureA.Body.UserData == null ||
					fixtureB.Body.UserData == null)
					return base.ShouldCollide(fixtureA, fixtureB);

				if ((fixtureA.Body.UserData is int && fixtureB.Body.UserData is int) && 
					((((int)fixtureA.Body.UserData) == 1 &&
					((int)fixtureB.Body.UserData) == 2) ||
					(((int)fixtureB.Body.UserData) == 1 &&
					((int)fixtureA.Body.UserData) == 2)))
					return false;

				return base.ShouldCollide(fixtureA, fixtureB);
			}
		}

		static Body ground;

		int _glidCrate;
		int _glidChain1, _glidChain2;
		int _glidChainball;
		static MouseJoint _mouseJoint;

		class MyDestructionListener : DestructionListener
		{
			public override void SayGoodbye(Joint joint)
			{
				if (joint == _mouseJoint)
					_mouseJoint = null;
			}

			public override void SayGoodbye(Fixture fixture)
			{
			}
		}

		public class Crate
		{
			Body _body;
			float _w, _h;

			public float Width
			{
				get { return _w; }
			}

			public float Height
			{
				get { return _h; }
			}

			public Crate(float w, float h, Vec2 position)
			{
				_w = w;
				_h = h;

				BodyDef bd = new BodyDef();
				{
					bd.BodyType = BodyType.Dynamic;
					bd.Position = position;
					bd.UserData = 1;

					PolygonShape sh = new PolygonShape();
					{
						sh.SetAsBox(w, h);

						FixtureDef fx = new FixtureDef();
						{
							fx.Shape = sh;
							fx.Density = 1.0f;
							fx.Friction = 0.3f;

							_body = world.CreateBody(bd);
							_body.CreateFixture(fx);
						}
					}
				}
			}

			public Body Body
			{
				get { return _body; }
			}
		}

		public class Chain
		{
			List<Body> _bodies = new List<Body>();
			Body _wrecker;
			float _w, _h;

			public float Width
			{
				get { return _w; }
			}

			public float Height
			{
				get { return _h; }
			}

			public Chain(float w, float h, int count, Vec2 position)
			{
				_w = w;
				_h = h;

				PolygonShape shape = new PolygonShape();
				{
					shape.SetAsBox(_w, _h);

					FixtureDef fd = new FixtureDef();
					{
						fd.Shape = shape;
						fd.Density = 20.0f;
						fd.Friction = 0.2f;

						RevoluteJointDef jd = new RevoluteJointDef();
						{
							jd.CollideConnected = false;

							const float y = 0;
							Body prevBody = ground;
							float _x = position.X;
							for (int i = 0; i < count; ++i)
							{
								BodyDef bd = new BodyDef();
								{
									bd.BodyType = BodyType.Dynamic;
									bd.Position = new Vec2(_x + w, position.Y + y);
									bd.UserData = 2;
									Body body = world.CreateBody(bd);
									body.CreateFixture(fd);

									_bodies.Add(body);

									Vec2 anchor = new Vec2(_x, position.Y + y);
									jd.Initialize(prevBody, body, anchor);

									world.CreateJoint(jd);

									_x += w * 2;
									prevBody = body;
								}
							}

							_wrecker = MakeCircle(new Vec2(_x + 1, position.Y), 1);
							_wrecker.Mass = 50;

							WeldJointDef wd = new WeldJointDef();
							{
								wd.Initialize(prevBody, _wrecker, new Vec2(_x, position.Y));

								world.CreateJoint(wd);
							}
						}
					}
				}
			}

			public List<Body> Bodies
			{
				get { return _bodies; }
			}

			public Body Wrecker
			{
				get { return _wrecker; }
			}
		}

		public class Bullet
		{
			Body _body;
			bool _remove;

			public bool Remove
			{
				get { return _remove; }
				set { _remove = value; }
			}

			public Body Body
			{
				get { return _body; }
			}

			public Bullet(Biped biped)
			{
				var gunHand = biped.Bodies[(int)BipedFixtureIndex.LHand];

				//var pos = gunHand.WorldCenter + new Vec2((float)(cos * 0.5f), (float)(-sin * 0.5f)) + new Vec2((float)(sin * 2), (float)(-cos * 2));
				Vec2 test = gunHand.WorldCenter;
				Vec2 Vel = new Vec2((float)Math.Sin(gunHand.Angle), (float)-Math.Cos(gunHand.Angle));
				Vec2 ShootPoint = new Vec2(2.0f, -0.17f);

				test.X += (0.5f + ShootPoint.Y) * (float)Math.Sin(gunHand.Angle + (90.0f).ToRad());
				test.Y += (0.5f + ShootPoint.Y) * (float)-Math.Cos(gunHand.Angle + (90.0f).ToRad());

				test.X += (ShootPoint.X) * (float)Math.Sin(gunHand.Angle);
				test.Y += (ShootPoint.X) * (float)-Math.Cos(gunHand.Angle);

				var bulletBodyDef = new BodyDef(BodyType.Dynamic, test);
				var bulletBodyShape = new PolygonShape(0.5f, 0.15f, gunHand.Angle + ((float)Math.PI / 2));
				_body = world.CreateBody(bulletBodyDef);
				_body.CreateFixture(new FixtureDef(bulletBodyShape, 0.01f, 0.0f, 0.2f, gunHand.FixtureList.FilterData));
				_body.IsBullet = true;
				_body.LinearVelocity = new Vec2((float)(Vel.X * 45), (float)(Vel.Y * 45));
				_body.UserData = (biped == players[0].Biped) ? 4 : 5;
			}
		}

		static List<Body> BodiesToRemove = new List<Body>();
		bool _slowmotion = false;

		public class MyListener : ContactListener
		{
			public override void BeginContact(Contact contact)
			{
				if (
					(contact.FixtureA.Body.UserData is int && ((int)contact.FixtureA.Body.UserData == 4 || (int)contact.FixtureA.Body.UserData == 5) ||
					(contact.FixtureB.Body.UserData is int && ((int)contact.FixtureB.Body.UserData == 4 || (int)contact.FixtureB.Body.UserData == 5))))
				{
					var bullet = (contact.FixtureA.Body.UserData is int && ((int)contact.FixtureA.Body.UserData == 4 || (int)contact.FixtureA.Body.UserData == 5)) ? contact.FixtureA.Body : contact.FixtureB.Body;
					var body = (contact.FixtureB.Body == bullet) ? contact.FixtureA.Body : contact.FixtureB.Body;

					contact.Enabled = false;
					if (!BodiesToRemove.Contains(bullet))
					{
						body.ApplyLinearImpulse(-(bullet.Position - body.Position) * 45, contact.WorldManifold.GetPoint(0));
						BodiesToRemove.Add(bullet);
						return;
					}
				}

				if (contact.FixtureA.Body.UserData == null ||
					contact.FixtureB.Body.UserData == null)
					return;

				if (contact.FixtureA.Body.UserData is BipedBodyDescriptor ||
					contact.FixtureB.Body.UserData is BipedBodyDescriptor)
				{
					var bipedBody = (contact.FixtureA.Body.UserData is BipedBodyDescriptor) ? contact.FixtureA.Body : contact.FixtureB.Body;
					var biped = (BipedBodyDescriptor)bipedBody.UserData;
					var other = (bipedBody == contact.FixtureB.Body) ? contact.FixtureA.Body : contact.FixtureB.Body;

					switch (biped.FixtureIndex)
					{
					case BipedFixtureIndex.LFoot:
						if (biped.Biped.OwnedPlayer.StickingLegs &&
							biped.Biped.OwnedPlayer.Welds[0] == null &&
							biped.Biped.OwnedPlayer.WeldDefs[0] == null)
						{
							var jd = new RevoluteJointDef();
							jd.Initialize(biped.Biped.Bodies[(int)biped.FixtureIndex], other, contact.WorldManifold.GetPoint(0));
							jd.LowerAngle = -0.5f * (float)Math.PI; // -90 degrees
							jd.UpperAngle = 0.25f * (float)Math.PI; // 45 degrees
							jd.EnableLimit = true;
							jd.MaxMotorTorque = 10.0f;
							jd.CollideConnected = true;
							jd.MotorSpeed = 0.0f;
							jd.EnableMotor = true;

							biped.Biped.OwnedPlayer.WeldDefs[0] = jd;
						}
						break;
					case BipedFixtureIndex.RFoot:
						if (biped.Biped.OwnedPlayer.StickingLegs && 
							biped.Biped.OwnedPlayer.Welds[1] == null &&
							biped.Biped.OwnedPlayer.WeldDefs[1] == null)
						{
							var jd = new RevoluteJointDef();
							jd.Initialize(biped.Biped.Bodies[(int)biped.FixtureIndex], other, contact.WorldManifold.GetPoint(0));
							jd.LowerAngle = -0.5f * (float)Math.PI; // -90 degrees
							jd.UpperAngle = 0.25f * (float)Math.PI; // 45 degrees
							jd.EnableLimit = true;
							jd.MaxMotorTorque = 10.0f;
							jd.CollideConnected = true;
							jd.MotorSpeed = 0.0f;
							jd.EnableMotor = true;

							biped.Biped.OwnedPlayer.WeldDefs[1] = jd;
						}
						break;
					/*case BipedFixtureIndex.LHand:
						if (biped.Biped.OwnedPlayer.StickingHands &&
							biped.Biped.OwnedPlayer.Welds[2] == null &&
							biped.Biped.OwnedPlayer.WeldDefs[2] == null)
						{
							var jd = new RevoluteJointDef();
							jd.Initialize(biped.Biped.Bodies[(int)biped.FixtureIndex], other, contact.WorldManifold.Points[0]);
							jd.LowerAngle = -0.5f * (float)Math.PI; // -90 degrees
							jd.UpperAngle = 0.25f * (float)Math.PI; // 45 degrees
							jd.EnableLimit = true;
							jd.MaxMotorTorque = 10.0f;
							jd.CollideConnected = true;
							jd.MotorSpeed = 0.0f;
							jd.EnableMotor = true;

							biped.Biped.OwnedPlayer.WeldDefs[2] = jd;
						}
						break;*/
					case BipedFixtureIndex.RHand:
						if (biped.Biped.OwnedPlayer.StickingHands && 
							biped.Biped.OwnedPlayer.Welds[3] == null &&
							biped.Biped.OwnedPlayer.WeldDefs[3] == null)
						{
							var jd = new RevoluteJointDef();
							jd.Initialize(biped.Biped.Bodies[(int)biped.FixtureIndex], other, contact.WorldManifold.GetPoint(0));
							jd.LowerAngle = -0.5f * (float)Math.PI; // -90 degrees
							jd.UpperAngle = 0.25f * (float)Math.PI; // 45 degrees
							jd.EnableLimit = true;
							jd.MaxMotorTorque = 10.0f;
							jd.CollideConnected = true;
							jd.MotorSpeed = 0.0f;
							jd.EnableMotor = true;

							biped.Biped.OwnedPlayer.WeldDefs[3] = jd;
						}
						break;
					}
				}
			}

			public override void EndContact(Contact contact)
			{
			}

			public override void PostSolve(Contact contact, ContactImpulse impulse)
			{
			}

			public override void PreSolve(Contact contact, Manifold oldManifold)
			{
			}
		}

		List<Crate> _crates = new List<Crate>();
		List<Chain> _chains = new List<Chain>();

		GDIDebugThing _debugDraw;
		ContactFilter _filterer;
		MyListener _listener;

		public class Player
		{
			Biped _biped;

			public Biped Biped
			{
				get { return _biped; }
			}

			public Player(World world, Vec2 position, float xScale, float yScale)
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

			RevoluteJointDef[] _weldDefs = new RevoluteJointDef[4];

			public RevoluteJointDef[] WeldDefs
			{
				get { return _weldDefs; }
			}
		}

		static Player[] players = new Player[2];

		public struct GroundBodyDescriptor
		{
			Body Body;

			public GroundBodyDescriptor(Body body)
			{
				Body = body;
			}
		}

		struct NetworkOptions
		{
			public string Name;
			public string IP;
			public bool Hosting;
		}

		Networking.NetworkServer server;
		Networking.NetworkClient client;
		NetworkOptions networkOptions = new NetworkOptions();

		static BipedDef player1Def = new BipedDef(9, 9);
		static BipedDef player2Def = new BipedDef(-9, 9);

		float _currentZoom = 10;

		SFML.Graphics.RenderWindow renderWindow;
		System.Threading.Thread simulationThread;

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
			
			Il.ilInit();
			Ilut.ilutInit();

			_debugDraw = new GDIDebugThing();
			_debugDraw.Flags = DebugFlags.Shapes;

			if (networkOptions.Hosting)
			{
				// Define the gravity vector.
				Vec2 gravity = new Vec2(0.0f, -17.0f);

				// Do we want to let bodies sleep?
				bool doSleep = true;

				// Construct a world object, which will hold and simulate the rigid bodies.
				world = new World(gravity, doSleep);
				world.WarmStarting = true;
				world.ContinuousPhysics = true;

				{
					float bottom = (float)(pictureBox1.Height / 2) / _currentZoom;
					float left = (float)(pictureBox1.Width / 2) / _currentZoom;
					BodyDef bd = new BodyDef();
					{
						ground = world.CreateBody(bd);

						PolygonShape shape = new PolygonShape();
						{
							shape.SetAsEdge(new Vec2(-left, -bottom), new Vec2(left, -bottom));
							ground.CreateFixture(shape, 0.0f);
							ground.UserData = new GroundBodyDescriptor(ground);
						}

						ground = world.CreateBody(bd);

						{
							shape.SetAsEdge(new Vec2(-left, bottom), new Vec2(-left, -bottom));
							ground.CreateFixture(shape, 0.0f);
							ground.UserData = new GroundBodyDescriptor(ground);
						}

						ground = world.CreateBody(bd);

						{
							shape.SetAsEdge(new Vec2(left, bottom), new Vec2(left, -bottom));
							ground.CreateFixture(shape, 0.0f);
							ground.UserData = new GroundBodyDescriptor(ground);
						}

						ground = world.CreateBody(bd);

						{
							shape.SetAsEdge(new Vec2(left, bottom), new Vec2(-left, bottom));
							ground.CreateFixture(shape, 0.0f);
							ground.UserData = new GroundBodyDescriptor(ground);
						}
					}
				}

				world.DebugDraw = _debugDraw;

				_filterer = new Filterer();
				world.ContactFilter = _filterer;

				_listener = new MyListener();
				world.ContactListener = _listener;
	

				_glidCrate = Ilut.ilutGLLoadImage("crate1.jpg");
				_glidChain1 = Ilut.ilutGLLoadImage("chain_top.png");
				_glidChain2 = Ilut.ilutGLLoadImage("chain_bottom.png");
				_glidChainball = Ilut.ilutGLLoadImage("chainball.png");

				players[0] = new Player(world, new Vec2(-24, 0), 9, 9);
				players[1] = new Player(world, new Vec2(24, 0), -9, 9);
			}

			simulationThread = new System.Threading.Thread(SimulationLoop);
			simulationThread.Start();
		}

		Point CursorPos;
		public const float settingsHz = 25;
		public const int settingsHzInMs = (int)(1000.0f / settingsHz);

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

		void renderWindow_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
		{
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

		/*Point _downPos;
		void sogc_MouseUp(object sender, MouseEventArgs e)
		{
			if (_mouseJoint != null)
			{
				world.DestroyJoint(_mouseJoint);
				_mouseJoint = null;
			}
		}

		void sogc_MouseDown(object sender, MouseEventArgs e)
		{
			return;

			_downPos = e.Location;

			if (e.Button == System.Windows.Forms.MouseButtons.Middle)
			{
				var p = new Vec2((float)((pictureBox1.Width / 2) - (pictureBox1.Width - _downPos.X)) / 14.0f, (float)((pictureBox1.Height / 2) - _downPos.Y) / 14.0f);

				if (_mouseJoint != null)
					return;

				// Make a small box.
				AABB aabb = new AABB();
				Vec2 d = new Vec2(0.001f, 0.001f);
				aabb.LowerBound = p - d;
				aabb.UpperBound = p + d;

				// Query the world for overlapping shapes.
				Fixture? m_fixture = null;

				world.QueryAABB(
				delegate(Fixture fixture)
				{
					Body body = fixture.Body;
					if (body.BodyType == BodyType.Dynamic)
					{
						bool inside = fixture.TestPoint(p);
						if (inside)
						{
							m_fixture = fixture;

							// We are done, terminate the query.
							return false;
						}
					}

					// Continue the query.
					return true;
				},
				aabb);

				if (m_fixture != null)
				{
					Body body = m_fixture.Value.Body;
					MouseJointDef md = new MouseJointDef();
					{
						md.BodyA = ground;
						md.BodyB = body;
						md.Target = p;
						md.MaxForce = 1000.0f * body.Mass;
						_mouseJoint = (MouseJoint)world.CreateJoint(md);
					}
					body.IsAwake = true;
				}
			}
		}

		Point _oldPos;
		int _moveCount = 2;
		void sogc_MouseMove(object sender, MouseEventArgs e)
		{
			var p = new Vec2((float)((pictureBox1.Width / 2) - (pictureBox1.Width - e.Location.X)) / 14.0f, (float)((pictureBox1.Height / 2) - e.Location.Y) / 14.0f);
			if (_mouseJoint != null)
				_mouseJoint.Target = p;

			if (_moveCount == 2)
			{
				_moveCount = 0;
				_oldPos = e.Location;
			}
			else
				_moveCount++;
		}


		void sogc_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			return;
			
			sogc_MouseClick(sender, e);
		}

		void sogc_MouseClick(object sender, MouseEventArgs e)
		{
			return;

			if (e.Button == System.Windows.Forms.MouseButtons.Middle)
				return;

			var pos = new Vec2((float)((pictureBox1.Width / 2) - (pictureBox1.Width - _downPos.X)) / 14.0f, (float)((pictureBox1.Height / 2) - _downPos.Y) / 14.0f);

			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				Crate cr = new Crate(1, 1, pos);
				_crates.Add(cr);

				var delta = e.Location;
				delta.X -= _downPos.X;
				delta.Y -= _downPos.Y;

				cr.Body.LinearVelocity = new Vec2(delta.X / 14, -delta.Y / 14);
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				Chain ch = new Chain(0.25f, 0.16f, 14, pos);
				_chains.Add(ch);
			}

			_moveCount = 0;
			_oldPos = e.Location;
		}*/

		void Simulate()
		{
			if (networkOptions.Hosting)
			{
				// Prepare for simulation. Typically we use a time step of 1/60 of a
				// second (60Hz) and 10 iterations. This provides a high quality simulation
				// in most game scenarios.
				float timeStep = 1.0f / settingsHz;
				int velocityIterations = 14;
				int positionIterations = 8;

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
								players[i].Biped.Bodies[(int)x].ApplyForce(new Vec2(0, MoveSpeed * speedMultiplier), players[i].Biped.Bodies[(int)x].WorldCenter);
						}
						if ((server.Clients[i].Keys & GameKeys.Down) != 0)
						{
							foreach (var x in fixturesToMove)
								players[i].Biped.Bodies[(int)x].ApplyForce(new Vec2(0, (-MoveSpeed / 2) * speedMultiplier), players[i].Biped.Bodies[(int)x].WorldCenter);
						}
						if ((server.Clients[i].Keys & GameKeys.Left) != 0)
						{
							foreach (var x in fixturesToMove)
								players[i].Biped.Bodies[(int)x].ApplyForce(new Vec2((-MoveSpeed / 2) * speedMultiplier, 0), players[i].Biped.Bodies[(int)x].WorldCenter);
						}
						if ((server.Clients[i].Keys & GameKeys.Right) != 0)
						{
							foreach (var x in fixturesToMove)
								players[i].Biped.Bodies[(int)x].ApplyForce(new Vec2((MoveSpeed / 2) * speedMultiplier, 0), players[i].Biped.Bodies[(int)x].WorldCenter);
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
				world.Step(timeStep, velocityIterations, positionIterations);

				foreach (var x in BodiesToRemove)
					world.DestroyBody(x);
				BodiesToRemove.Clear();

				for (int i = 0; i < players[0].WeldDefs.Length; ++i)
				{
					if (players[0].WeldDefs[i] != null)
					{
						players[0].Welds[i] = (RevoluteJoint)world.CreateJoint(players[0].WeldDefs[i]);
						players[0].WeldDefs[i] = null;
					}

					if (i < 2 && !players[0].StickingLegs ||
					i >= 2 && !players[0].StickingHands)
					{
						if (players[0].Welds[i] != null)
						{
							world.DestroyJoint(players[0].Welds[i]);
							players[0].Welds[i] = null;
						}
					}

					if (players[1].WeldDefs[i] != null)
					{
						players[1].Welds[i] = (RevoluteJoint)world.CreateJoint(players[1].WeldDefs[i]);
						players[1].WeldDefs[i] = null;
					}

					if (i < 2 && !players[1].StickingLegs ||
					i >= 2 && !players[1].StickingHands)
					{
						if (players[1].Welds[i] != null)
						{
							world.DestroyJoint(players[1].Welds[i]);
							players[1].Welds[i] = null;
						}
					}
				}

				server.Stream.Write((byte)Networking.EClientDataPacketType.FramePacket);
				server.Stream.Write(server.Frame);
				for (int i = 0; i < (int)BipedFixtureIndex.Max; ++i)
				{
					server.Stream.Write(players[0].Biped.Bodies[(int)i].Position.X);
					server.Stream.Write(players[0].Biped.Bodies[(int)i].Position.Y);
					server.Stream.Write(players[0].Biped.Bodies[(int)i].Angle);

					server.Stream.Write(players[1].Biped.Bodies[(int)i].Position.X);
					server.Stream.Write(players[1].Biped.Bodies[(int)i].Position.Y);
					server.Stream.Write(players[1].Biped.Bodies[(int)i].Angle);
				}
			}
			else
			{
				if (_oldGameKeys != _gameKeys)
				{
					client.Stream.Write((byte)Networking.EServerDataPacketType.ClientCmd);
					client.Stream.Write((int)_gameKeys);

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

			Gl.glTranslatef(pictureBox1.Width / 2, pictureBox1.Height / 2, 0);
			Gl.glScalef(zoom, -zoom, zoom);

			if (_drawDebug && networkOptions.Hosting)
			{
				Gl.glPushMatrix();
				world.DrawDebugData();
				Gl.glPopMatrix();
			}
			else if (!networkOptions.Hosting)
			{
				float frac = 1.0f - (float)((DateTime.Now.TimeOfDay.TotalMilliseconds - client.NextFrameTime) / (double)settingsHzInMs);

				if (client.CurFrame.Transforms != null)
				{
					var color = new ColorF(0.9f, 0.7f, 0.7f);

					for (int i = 0; i < (int)BipedFixtureIndex.Max; ++i)
					{
						if (client.OldFrame.Transforms == null)
						{
							_debugDraw.DrawSolidPolygon(player1Def.Fixtures[(int)i].Shape, client.CurFrame.Transforms[i, 0].ToTransform(), color);
							_debugDraw.DrawSolidPolygon(player2Def.Fixtures[(int)i].Shape, client.CurFrame.Transforms[i, 1].ToTransform(), color);
						}
						else
						{
							for (int z = 0; z < 2; ++z)
							{
								var transform1_new = client.CurFrame.Transforms[i, z];
								var transform1_old = client.OldFrame.Transforms[i, z];
								transform1_new.Position = transform1_old.Position + ((transform1_old.Position - transform1_new.Position) * frac);
								transform1_new.Angle = transform1_old.Angle + ((transform1_old.Angle - transform1_new.Angle) * frac);
								_debugDraw.DrawSolidPolygon(((z == 0) ? player1Def : player2Def).Fixtures[(int)i].Shape, transform1_new.ToTransform(), color);
							}
						}
					}
				}
			}

			Gl.glPushMatrix();
			Gl.glColor4ub(255, 255, 255, 255);
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, _glidCrate);
			foreach (var c in _crates)
			{
				Gl.glPushMatrix();
				Gl.glTranslatef(c.Body.Position.X, c.Body.Position.Y, 0);
				Gl.glRotatef((float)(c.Body.Angle * 180 / Math.PI), 0, 0, 1);
				Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0, 0);
				Gl.glVertex2f(-c.Width, -c.Height);
				Gl.glTexCoord2f(1, 0);
				Gl.glVertex2f(c.Width, -c.Height);
				Gl.glTexCoord2f(1, 1);
				Gl.glVertex2f(c.Width, c.Height);
				Gl.glTexCoord2f(0, 1);
				Gl.glVertex2f(-c.Width, c.Height);
				Gl.glEnd();
				Gl.glPopMatrix();
			}

			bool _top = true;
			foreach (var c in _chains)
			{
				foreach (var b in c.Bodies)
				{
					if (_top)
						Gl.glBindTexture(Gl.GL_TEXTURE_2D, _glidChain1);
					else
						Gl.glBindTexture(Gl.GL_TEXTURE_2D, _glidChain2);

					_top = !_top;

					Gl.glPushMatrix();
					Gl.glTranslatef(b.Position.X, b.Position.Y, 0);
					Gl.glRotatef((float)(b.Angle * 180 / Math.PI), 0, 0, 1);
					Gl.glBegin(Gl.GL_QUADS);
					Gl.glTexCoord2f(0, 0);
					Gl.glVertex2f(-(c.Width + .2f), -c.Height);
					Gl.glTexCoord2f(0, 1);
					Gl.glVertex2f((c.Width + .2f), -c.Height);
					Gl.glTexCoord2f(1, 1);
					Gl.glVertex2f((c.Width + .2f), c.Height);
					Gl.glTexCoord2f(1, 0);
					Gl.glVertex2f(-(c.Width + .2f), c.Height);
					Gl.glEnd();
					Gl.glPopMatrix();
				}

				Gl.glBindTexture(Gl.GL_TEXTURE_2D, _glidChainball);
				Gl.glPushMatrix();
				Gl.glTranslatef(c.Wrecker.Position.X, c.Wrecker.Position.Y, 0);
				Gl.glRotatef((float)(c.Wrecker.Angle * 180 / Math.PI), 0, 0, 1);
				Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0, 0);
				Gl.glVertex2f(-1, -1);
				Gl.glTexCoord2f(0, 1);
				Gl.glVertex2f(1, -1);
				Gl.glTexCoord2f(1, 1);
				Gl.glVertex2f(1, 1);
				Gl.glTexCoord2f(1, 0);
				Gl.glVertex2f(-1, 1);
				Gl.glEnd();
				Gl.glPopMatrix();
			}

			Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

			Gl.glPopMatrix();
		
			renderWindow.Display();

			_renderFrame++;

			mutex.ReleaseMutex();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			client.SendText(textBox2.Text);
			textBox2.Clear();
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
