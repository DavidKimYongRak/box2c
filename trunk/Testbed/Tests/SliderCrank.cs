using System;
using Box2CS;

namespace Testbed.Tests
{
	public class SliderCrank : Test
	{
		public SliderCrank()
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
				Body prevBody = ground;

				// Define crank.
				{
					PolygonShape shape = new PolygonShape();
					shape.SetAsBox(0.5f, 2.0f);

					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic;
					bd.Position = new Vec2(0.0f, 7.0f);
					Body body = m_world.CreateBody(bd);
					body.CreateFixture(shape, 2.0f);

					RevoluteJointDef rjd = new RevoluteJointDef();
					rjd.Initialize(prevBody, body, new Vec2(0.0f, 5.0f));
					rjd.MotorSpeed = 1.0f * (float)Math.PI;
					rjd.MaxMotorTorque = 10000.0f;
					rjd.EnableMotor = true;
					m_joint1 = (RevoluteJoint)m_world.CreateJoint(rjd);

					prevBody = body;
				}

				// Define follower.
				{
					PolygonShape shape = new PolygonShape();
					shape.SetAsBox(0.5f, 4.0f);

					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic;
					bd.Position = new Vec2(0.0f, 13.0f);
					Body body = m_world.CreateBody(bd);
					body.CreateFixture(shape, 2.0f);

					RevoluteJointDef rjd = new RevoluteJointDef();
					rjd.Initialize(prevBody, body, new Vec2(0.0f, 9.0f));
					rjd.EnableMotor = false;
					m_world.CreateJoint(rjd);

					prevBody = body;
				}

				// Define piston
				{
					PolygonShape shape = new PolygonShape();
					shape.SetAsBox(1.5f, 1.5f);

					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic;
					bd.Position = new Vec2(0.0f, 17.0f);
					Body body = m_world.CreateBody(bd);
					body.CreateFixture(shape, 2.0f);

					RevoluteJointDef rjd = new RevoluteJointDef();
					rjd.Initialize(prevBody, body, new Vec2(0.0f, 17.0f));
					m_world.CreateJoint(rjd);

					PrismaticJointDef pjd = new PrismaticJointDef();
					pjd.Initialize(ground, body, new Vec2(0.0f, 17.0f), new Vec2(0.0f, 1.0f));

					pjd.MaxMotorForce = 1000.0f;
					pjd.EnableMotor = true;

					m_joint2 = (PrismaticJoint)m_world.CreateJoint(pjd);
				}

				// Create a payload
				{
					PolygonShape shape = new PolygonShape();
					shape.SetAsBox(1.5f, 1.5f);

					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic;
					bd.Position = new Vec2(0.0f, 23.0f);
					Body body = m_world.CreateBody(bd);
					body.CreateFixture(shape, 2.0f);
				}
			}
		}

		public override void  Keyboard(SFML.Window.KeyCode key)
		{
			switch (char.ToLower((char)key))
			{
			case 'f':
				m_joint2.IsMotorEnabled = !m_joint2.IsMotorEnabled;
				m_joint2.BodyB.IsAwake = true;
				break;

			case 'm':
				m_joint1.IsMotorEnabled = !m_joint1.IsMotorEnabled;
				m_joint1.BodyB.IsAwake = true;
				break;
			}
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "Keys: (f) toggle friction, (m) toggle motor");
			m_textLine += 15;
			m_debugDraw.DrawString(5, m_textLine, "Motor Torque = "+m_joint1.MotorTorque.ToString());
			m_textLine += 15;
		}

		RevoluteJoint m_joint1;
		PrismaticJoint m_joint2;
	};
}
