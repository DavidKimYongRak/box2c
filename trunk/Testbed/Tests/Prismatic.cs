using System;
using Box2CS;

namespace Testbed.Tests
{
	public class Prismatic : Test
	{
		public Prismatic()
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
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(2.0f, 0.5f);

				BodyDef bd = new BodyDef();
				bd.BodyType = EBodyType.b2_dynamicBody;
				bd.Position = new Vec2(-10.0f, 10.0f);
				bd.Angle = 0.5f * (float)Math.PI;
				bd.AllowSleep = false;
				Body body = m_world.CreateBody(bd);
				body.CreateFixture(shape, 5.0f);

				PrismaticJointDef pjd = new PrismaticJointDef();

				// Bouncy limit
				Vec2 axis = new Vec2(2.0f, 1.0f);
				axis.Normalize();
				pjd.Initialize(ground, body, new Vec2(0.0f, 0.0f), axis);

				// Non-bouncy limit
				//pjd.Initialize(ground, body, new Vec2(-10.0f, 10.0f), new Vec2(1.0f, 0.0f));

				pjd.MotorSpeed = 10.0f;
				pjd.MaxMotorForce = 10000.0f;
				pjd.EnableMotor = true;
				pjd.LowerTranslation = 0.0f;
				pjd.UpperTranslation = 20.0f;
				pjd.EnableLimit = false;

				m_joint = (PrismaticJoint)m_world.CreateJoint(pjd);
			}
		}

		public override void  Keyboard(SFML.Window.KeyCode key)
		{
			switch (char.ToLower((char)key))
			{
			case 'l':
				m_joint.IsLimitEnabled = !m_joint.IsLimitEnabled;
				break;

			case 'm':
				m_joint.IsMotorEnabled = !m_joint.IsMotorEnabled;
				break;

			case 's':
				m_joint.MotorSpeed = -m_joint.MotorSpeed;
				break;
			}
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "Keys: (l) limits, (m) motors, (s) speed");
			m_textLine += 15;
			float force = m_joint.MotorForce;
			m_debugDraw.DrawString(5, m_textLine, "Motor Force = "+force.ToString() + ", Limits = "+m_joint.IsLimitEnabled.ToString());
			m_textLine += 15;	
		}

		PrismaticJoint m_joint;
	};
}
