using System;
using Box2CS;

namespace Testbed.Tests
{
	public class Revolute : Test
	{
		public Revolute()
		{
			Body ground = null;
			{
				BodyDef bd = new BodyDef();
				ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			{
				CircleShape shape = new CircleShape();
				shape.Radius = 0.5f;

				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;

				RevoluteJointDef rjd = new RevoluteJointDef();

				bd.Position = new Vec2(0.0f, 20.0f);
				Body body = m_world.CreateBody(bd);
				body.CreateFixture(shape, 5.0f);

				float w = 100.0f;
				body.AngularVelocity = w;
				body.LinearVelocity = new Vec2(-8.0f * w, 0.0f);

				rjd.Initialize(ground, body, new Vec2(0.0f, 12.0f));
				rjd.MotorSpeed = 1.0f * (float)Math.PI;
				rjd.MaxMotorTorque = 10000.0f;
				rjd.EnableMotor = false;
				rjd.LowerAngle = -0.25f * (float)Math.PI;
				rjd.UpperAngle = 0.5f * (float)Math.PI;
				rjd.EnableLimit = true;
				rjd.CollideConnected = true;

				m_joint = (RevoluteJoint)m_world.CreateJoint(rjd);
			}
		}

		public override void  Keyboard(SFML.Window.KeyCode key)
		{
			switch (char.ToLower((char)key))
			{
			case 'l':
				m_joint.IsLimitEnabled = !m_joint.IsLimitEnabled;
				break;

			case 's':
				m_joint.IsMotorEnabled = !m_joint.IsMotorEnabled;
				break;
			}
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "Keys: (l) limits, (a) left, (s) motor, (d) right");
			m_textLine += 15;
		}

		RevoluteJoint m_joint;
	};
}
