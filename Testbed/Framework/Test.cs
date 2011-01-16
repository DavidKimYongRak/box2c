using System;
using System.Collections.Generic;
using Box2CS;
using Tao.OpenGl;
using System.Windows.Forms;
using System.Reflection;

namespace Testbed
{
	public delegate Test TestCreateFcn();

	public static class Rand
	{
		static Random rand = new Random();
		const int RAND_LIMIT	= 32767;

		/// Random number in range [-1,1]
		public static float RandomFloat()
		{
			return 1 - (float)(rand.NextDouble() * 2);
		}

		/// Random floating point number in range [lo, hi]
		public static float RandomFloat(float lo, float hi)
		{
			return (float)((rand.NextDouble() * hi) + (rand.NextDouble() * lo));
		}
	}

	/// Test settings. Some can be controlled in the GUI.
	public static class TestSettings
	{
		public static float hz = 60;
		public static int velocityIterations = 8;
		public static int positionIterations = 3;
		public static bool drawShapes = true;
		public static bool drawJoints = true;
		public static bool drawAABBs;
		public static bool drawPairs;
		public static bool drawContactPoints;
		public static bool drawContactNormals;
		public static bool drawContactForces;
		public static bool drawFrictionForces;
		public static bool drawCOMs;
		public static bool drawStats;
		public static bool enableWarmStarting = true;
		public static bool enableContinuous = true;
		public static bool pause;
		public static bool singleStep;
		public static bool restart;
	};

	public class TestEntry
	{
		public string Name;
		public Type Type;

		public Test Construct()
		{
			return (Test)Activator.CreateInstance(Type);//(Test)Type.GetConstructor(new System.Type[] { }).Invoke(null);
		}
	}

	public class TestEntries
	{
		List<TestEntry> _entries = new List<TestEntry>();

		public TestEntries()
		{
			Enumerate();
		}

		void Enumerate()
		{
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.BaseType == typeof(Test))
				{
					TestEntry newEntry = new TestEntry();
					newEntry.Type = type;
					newEntry.Name = type.Name;
					_entries.Add(newEntry);
				}
			}

			_entries.Sort(delegate(TestEntry t1, TestEntry t2)
			{
				return t1.Name.CompareTo(t2.Name);
			});
		}

		public IList<TestEntry> Entries
		{
			get { return _entries; }
		}
	}

	// This is called when a joint in the world is implicitly destroyed
	// because an attached body is destroyed. This gives us a chance to
	// nullify the mouse joint.
	public class TestDestructionListener : DestructionListener
	{
		public override void SayGoodbye(Fixture fixture)
		{

		}
		public override void SayGoodbye(Joint joint)
		{
			if (test.m_mouseJoint == joint)
			{
				test.m_mouseJoint = null;
			}
			else
			{
				test.JointDestroyed(joint);
			}
		}

		public Test test;
	};

	public struct ContactPoint
	{
		public const int k_maxContactPoints = 2048;

		public Fixture fixtureA;
		public Fixture fixtureB;
		public Vec2 normal;
		public Vec2 position;
		public PointState state;
	};

	public abstract class Test : ContactListener, IDisposable
	{
		public Test()
		{
			Vec2 gravity;
			gravity = new Vec2(0.0f, -10.0f);
			bool doSleep = true;
			m_world = new World(gravity, doSleep);
			m_bomb = null;
			m_textLine = 30;
			m_mouseJoint = null;
			m_pointCount = 0;

			m_destructionListener.test = this;
			m_world.DestructionListener = m_destructionListener;
			m_world.ContactListener = this;
			m_world.DebugDraw = m_debugDraw;

			m_bombSpawning = false;

			m_stepCount = 0;

			BodyDef bodyDef = new BodyDef();
			m_groundBody = m_world.CreateBody(bodyDef);
			m_groundBody.UserData = "Ground";
		}

		public virtual void DisposeTest() { }

		bool _disposed = false;
		public new void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				DisposeTest();
			}

			m_world.Dispose();
			base.Dispose();
		}

		public void SetTextLine(int line) { m_textLine = line; }
		public void DrawTitle(int x, int y, string str)
		{
			m_debugDraw.DrawString(x, y, str);
		}

		Fixture _selectedFixture;

		public virtual void Step()
		{
			float timeStep = TestSettings.hz > 0.0f ? 1.0f / TestSettings.hz : 0.0f;

			if (TestSettings.pause)
			{
				if (TestSettings.singleStep)
				{
					TestSettings.singleStep = false;
				}
				else
				{
					timeStep = 0.0f;
				}
			}

			DebugFlags flags = 0;
			if (TestSettings.drawShapes) flags |= DebugFlags.Shapes;
			if (TestSettings.drawJoints) flags |= DebugFlags.Joints;
			if (TestSettings.drawAABBs) flags |= DebugFlags.AABBs;
			if (TestSettings.drawPairs) flags |= DebugFlags.Pairs;
			if (TestSettings.drawCOMs) flags |= DebugFlags.CenterOfMasses;
			m_debugDraw.Flags = flags;

			m_world.WarmStarting = TestSettings.enableWarmStarting;
			m_world.ContinuousPhysics = TestSettings.enableContinuous;

			m_pointCount = 0;

			m_world.Step(timeStep, TestSettings.velocityIterations, TestSettings.positionIterations);

			//m_world.DrawDebugData();

			if (timeStep > 0.0f)
			{
				++m_stepCount;
			}

			// Make a small box.
			_selectedFixture = null;
			{
				AABB aabb = new AABB();
				Vec2 d = new Vec2(0.001f, 0.001f);
				var p2 = Main.CursorPos;
				var p = Program.MainForm.ConvertScreenToWorld((int)p2.X, (int)p2.Y);
				aabb.LowerBound = p - d;
				aabb.UpperBound = p + d;

				m_world.QueryAABB(
				delegate(Fixture fixture)
				{
					Body body = fixture.Body;
					if (body.BodyType == BodyType.Dynamic)
					{
						bool inside = fixture.TestPoint(p);
						if (inside)
						{
							_selectedFixture = fixture;

							// We are done, terminate the query.
							return false;
						}
					}

					// Continue the query.
					return true;
				},
				aabb);
			}
		}

		public virtual void Keyboard(SFML.Window.KeyCode key) { }
		public void ShiftMouseDown(Vec2 p)
		{
			m_mouseWorld = p;

			if (m_mouseJoint != null)
				return;

			SpawnBomb(p);
		}
		public virtual void MouseDown(Vec2 p)
		{
			m_mouseWorld = p;

			if (m_mouseJoint != null)
				return;

			// Make a small box.
			AABB aabb = new AABB();
			Vec2 d = new Vec2(0.001f, 0.001f);
			aabb.LowerBound = p - d;
			aabb.UpperBound = p + d;

			// Query the world for overlapping shapes.
			Fixture m_fixture = null;

			m_world.QueryAABB(
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
				Body body = m_fixture.Body;
				MouseJointDef md = new MouseJointDef();
				{
					md.BodyA = m_groundBody;
					md.BodyB = body;
					md.Target = p;
					md.MaxForce = 1000.0f * body.Mass;
					m_mouseJoint = (MouseJoint)m_world.CreateJoint(md);
				}
				body.IsAwake = true;
			}
		}
		public virtual void MouseUp(Vec2 p)
		{
			if (m_mouseJoint != null)
			{
				m_world.DestroyJoint(m_mouseJoint);
				m_mouseJoint = null;
			}

			if (m_bombSpawning)
				CompleteBombSpawn(p);
		}
		public virtual void MouseMove(Vec2 p)
		{
			m_mouseWorld = p;

			if (m_mouseJoint != null)
			{
				m_mouseJoint.Target = p;

				if ((Control.ModifierKeys & Keys.Shift) != 0)
				{
					m_mouseJoint.BodyB.IsFixedRotation = true;
					m_mouseJoint.BodyB.AngularVelocity = 0;
				}
				else
					m_mouseJoint.BodyB.IsFixedRotation = false;
			}
		}
		public void LaunchBomb()
		{
			Vec2 p = new Vec2(Rand.RandomFloat(-15.0f, 15.0f), 30.0f);
			Vec2 v = -5.0f * p;
			LaunchBomb(p, v);
		}

		public void LaunchBomb(Vec2 position, Vec2 velocity)
		{
			if (m_bomb != null)
			{
				m_world.DestroyBody(m_bomb);
				m_bomb = null;
			}

			m_bomb = m_world.CreateBody(new BodyDef(BodyType.Dynamic, position, 0.0f, velocity, true));
			m_bomb.Mass = 5;

			m_bomb.CreateFixture(new FixtureDef(new CircleShape(0.3f), 20.0f));
		}

		public void SpawnBomb(Vec2 worldPt)
		{
			m_bombSpawnPoint = worldPt;
			m_bombSpawning = true;
		}

		public void CompleteBombSpawn(Vec2 p)
		{
			if (m_bombSpawning == false)
				return;

			const float multiplier = 30.0f;
			Vec2 vel = m_bombSpawnPoint - p;
			vel *= multiplier;
			LaunchBomb(m_bombSpawnPoint, vel);
			m_bombSpawning = false;
		}

		// Let derived tests know that a joint was destroyed.
		public virtual void JointDestroyed(Joint joint) { }

		public override void BeginContact(Contact contact)
		{
		}

		public override void EndContact(Contact contact)
		{
		}

		public override void PostSolve(Contact contact, ContactImpulse impulse)
		{
		}

		public override void PreSolve(Contact contact, Manifold oldManifold)
		{
			Manifold manifold = contact.Manifold;

			if (manifold.PointCount == 0)
				return;

			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;

			PointState[] state1 = new PointState[Box2DSettings.b2_maxManifoldPoints],
				state2 = new PointState[Box2DSettings.b2_maxManifoldPoints];
			Manifold.GetPointStates(ref state1, ref state2, ref oldManifold, ref manifold);

			WorldManifold worldManifold = contact.WorldManifold;

			for (int i = 0; i < manifold.PointCount && m_pointCount < ContactPoint.k_maxContactPoints; ++i)
			{
				m_points[m_pointCount].fixtureA = fixtureA;
				m_points[m_pointCount].fixtureB = fixtureB;
				m_points[m_pointCount].position = worldManifold.GetPoint(i);
				m_points[m_pointCount].normal = worldManifold.Normal;
				m_points[m_pointCount].state = state2[i];
				++m_pointCount;
			}
		}

		// Callbacks for derived classes.

		public Body m_groundBody;
		public AABB m_worldAABB;
		public ContactPoint[] m_points = new ContactPoint[ContactPoint.k_maxContactPoints];
		public int m_pointCount;
		public TestDestructionListener m_destructionListener = new TestDestructionListener();
		public TestDebugDraw m_debugDraw = new TestDebugDraw();
		public int m_textLine;
		public World m_world;
		public Body m_bomb;
		public MouseJoint m_mouseJoint;
		public Vec2 m_bombSpawnPoint;
		public bool m_bombSpawning;
		public Vec2 m_mouseWorld;
		public int m_stepCount;

		public virtual void Draw()
		{
			if (TestSettings.pause)
				m_debugDraw.DrawString(5, m_textLine, "****PAUSED****");
			m_textLine += 15;

			if (TestSettings.drawStats)
			{
				m_debugDraw.DrawString(5, m_textLine, String.Format("bodies/contacts/joints/proxies = {0}/{1}/{2}/{3}",
					m_world.BodyCount, m_world.ContactCount, m_world.JointCount, m_world.ProxyCount));
				m_textLine += 15;
			}

			if (m_mouseJoint != null)
			{
				Vec2 p1 = m_mouseJoint.AnchorB;
				Vec2 p2 = m_mouseJoint.Target;

				Gl.glPointSize(4.0f);
				Gl.glColor3f(0.0f, 1.0f, 0.0f);
				Gl.glBegin(Gl.GL_POINTS);
				Gl.glVertex2f(p1.X, p1.Y);
				Gl.glVertex2f(p2.X, p2.Y);
				Gl.glEnd();
				Gl.glPointSize(1.0f);

				Gl.glColor3f(0.8f, 0.8f, 0.8f);
				Gl.glBegin(Gl.GL_LINES);
				Gl.glVertex2f(p1.X, p1.Y);
				Gl.glVertex2f(p2.X, p2.Y);
				Gl.glEnd();
			}

			if (m_bombSpawning)
			{
				Gl.glPointSize(4.0f);
				Gl.glColor3f(0.0f, 0.0f, 1.0f);
				Gl.glBegin(Gl.GL_POINTS);
				Gl.glColor3f(0.0f, 0.0f, 1.0f);
				Gl.glVertex2f(m_bombSpawnPoint.X, m_bombSpawnPoint.Y);
				Gl.glEnd();

				Gl.glColor3f(0.8f, 0.8f, 0.8f);
				Gl.glBegin(Gl.GL_LINES);
				Gl.glVertex2f(m_mouseWorld.X, m_mouseWorld.Y);
				Gl.glVertex2f(m_bombSpawnPoint.X, m_bombSpawnPoint.Y);
				Gl.glEnd();
			}

			if (_selectedFixture != null)
			{
				var body = _selectedFixture.Body;

				if (_selectedFixture.UserData != null)
				{
					m_debugDraw.DrawString(5, m_textLine, "Fixture data: " + _selectedFixture.UserData.ToString());
					m_textLine += 15;
				}

				if (body.UserData != null)
				{
					m_debugDraw.DrawString(5, m_textLine, "Body data: " + body.UserData.ToString());
					m_textLine += 15;
				}

				m_debugDraw.DrawString(5, m_textLine, "Position: "+body.Position);
				m_textLine += 15;
			}

			if (TestSettings.drawContactPoints)
			{
				//const float32 k_impulseScale = 0.1f;
				const float k_axisScale = 0.6f;

				for (int i = 0; i < m_pointCount; ++i)
				{
					if (m_points[i].state == PointState.Add)
					{
						// Add
						m_debugDraw.DrawPoint(m_points[i].position, 10.0f, new ColorF(0.3f, 0.95f, 0.3f));
					}
					else if (m_points[i].state == PointState.Persist)
					{
						// Persist
						m_debugDraw.DrawPoint(m_points[i].position, 5.0f, new ColorF(0.3f, 0.3f, 0.95f));
					}

					if (TestSettings.drawContactNormals)
					{
						Vec2 p1 = m_points[i].position;
						Vec2 p2 = p1 + k_axisScale * m_points[i].normal;
						m_debugDraw.DrawSegment(p1, p2, new ColorF(0.9f, 0, 0));
					}

					else if (TestSettings.drawContactForces)
					{
						//b2Vec2 p1 = m_points[i].position;
						//b2Vec2 p2 = p1 + k_forceScale * m_points[i].normalForce * m_points[i].normal;
						//DrawSegment(p1, p2, b2Color(0.9f, 0.9f, 0.3f));
					}

					if (TestSettings.drawFrictionForces)
					{
						//Vec2 tangent = m_points[i].normal.Cross (1.0f);
						//Vec2 p1 = m_points[i].position;
						//Vec2 p2 = p1 + k_forceScale * m_points[i].tangentForce * tangent;
						//DrawSegment(p1, p2, b2Color(0.9f, 0.9f, 0.3f));
					}
				}
			}
		}
	};
}
