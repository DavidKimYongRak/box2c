using System;
using Box2CS;
using System.Collections.Generic;

namespace Testbed.Tests
{
	// Adapted from SpiritWalkers by darkzerox
	public class Car : Test
	{
		public static string Name
		{
			get { return "Car"; }
		}

		List<Vec2> _points = new List<Vec2>();

		public Car()
		{
			{	// car body
				PolygonShape poly1 = new PolygonShape(), poly2 = new PolygonShape();

				// bottom half
				poly1.Vertices = new Vec2[]
				{
					new Vec2(2.2f,-0.74f),
					new Vec2(2.2f,-0.2f),
					new Vec2(1.0f,0),
					new Vec2(-2.2f,0),
					new Vec2(-2.2f,-0.74f),

					//new Vec2(-2.2f,-0.74f),
					//new Vec2(-2.2f,0),
					//new Vec2(1.0f,0),
					//new Vec2(2.2f,-0.2f),
					//new Vec2(2.2f,-0.74f)
				};

				FixtureDef fixture1 = new FixtureDef();

				fixture1.Filter.GroupIndex = -1;
				fixture1.Shape = poly1;
				fixture1.Density		= 20.0f;
				fixture1.Friction		= 0.68f;

				// top half
				poly2.Vertices = new Vec2[]
				{
					new Vec2(1.0f,0),
					new Vec2(0.5f,0.74f),
					new Vec2(-1.3f,0.7f),
					new Vec2(-1.7f,0),
				};

				FixtureDef fixture2 = new FixtureDef();

				fixture2.Filter.GroupIndex = -1;
				fixture2.Shape = poly2;
				fixture2.Density		= 5.0f;
				fixture2.Friction		= 0.68f;

				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(-35.0f, 2.8f);

				m_vehicle = m_world.CreateBody(bd);
				m_vehicle.CreateFixture(fixture1);
				m_vehicle.CreateFixture(fixture2);
			}

			{	// vehicle wheels
				CircleShape circ = new CircleShape();
				circ.Radius = 0.58608f;

				FixtureDef wheelFix = new FixtureDef();
				wheelFix.Shape = circ;
				wheelFix.Density = 40.0f;
				wheelFix.Friction = 0.8f;
				wheelFix.Filter.GroupIndex = -1;

				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.AllowSleep = false;
				bd.Position = new Vec2(-33.8f, 2.0f);

				m_rightWheel = m_world.CreateBody(bd);
				m_rightWheel.CreateFixture(wheelFix);

				bd.Position = new Vec2(-36.2f, 2.0f);
				m_leftWheel = m_world.CreateBody(bd);
				m_leftWheel.CreateFixture(wheelFix);
			}

			{	// join wheels to chassis
				RevoluteJointDef jd = new RevoluteJointDef();
				jd.Initialize(m_vehicle, m_leftWheel, m_leftWheel.WorldCenter);
				jd.CollideConnected = false;
				jd.EnableMotor = true;
				jd.MaxMotorTorque = 10.0f;
				jd.MotorSpeed = 0.0f;
				m_leftJoint = (RevoluteJoint)m_world.CreateJoint(jd);

				jd.Initialize(m_vehicle, m_rightWheel, m_rightWheel.WorldCenter);
				jd.CollideConnected = false;
				m_rightJoint = (RevoluteJoint)m_world.CreateJoint(jd);
			}
		}

		bool _paused = false;

		public override void Step()
		{
			base.Step();

			if (!_paused)
			{
				TestSettings.pause = true;
				_paused = true;
			}
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "Keys: left = a, brake = s, right = d, attempt flip = w");
			m_textLine += 15;

			m_debugDraw.DrawString(5, m_textLine, "Keys: start chain/add point to cursor = t, finish chain = y");
			m_textLine += 15;

			foreach (var p in _points)
				m_debugDraw.DrawPoint(p, 4, new ColorF(1, 0, 0));
		}

		public override void Keyboard(SFML.Window.KeyCode key)
		{
			switch (char.ToLower((char)key))
			{
			case 'a':
				m_leftJoint.MaxMotorTorque = 800.0f;
				m_leftJoint.MotorSpeed = 36.0f;
				break;

			case 's':
				m_leftJoint.MaxMotorTorque = 100.0f;
				m_leftJoint.MotorSpeed = 0.0f;
				break;

			case 'd':
				m_leftJoint.MaxMotorTorque = 1200.0f;
				m_leftJoint.MotorSpeed = -36.0f;
				break;

			case 'w':
				m_vehicle.ApplyLinearImpulse(new Vec2(0.0f, 455.0f), m_vehicle.WorldCenter);
				m_vehicle.ApplyAngularImpulse(66);
				break;

			case 't':
				_points.Add(Program.MainForm.ConvertScreenToWorld(Main.GLWindow.Input.GetMouseX(), Main.GLWindow.Input.GetMouseY()));
				break;

			case 'y':
				{
					var chain = new EdgeChain(_points.ToArray());
					chain.GenerateBodies(m_world, new Vec2(0, 0), new FixtureDef(null, 0, 0, 0.65f));
					_points.Clear();
				}
				break;		
			}
		}

		Body m_leftWheel;
		Body m_rightWheel;
		Body m_vehicle;
		RevoluteJoint m_leftJoint;
		RevoluteJoint m_rightJoint;
	}
}