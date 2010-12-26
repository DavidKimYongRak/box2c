using System;
using Box2CS;

namespace Testbed.Tests
{
	// A line joint with a limit and friction.
	public class LineJoint : Test
	{
		public LineJoint()
		{
			Body ground = null;
			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));

				BodyDef bd = new BodyDef();
				ground = m_world.CreateBody(bd);
				ground.CreateFixture(shape, 0.0f);
			}

			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.5f, 2.0f);

				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(0.0f, 7.0f);
				Body body = m_world.CreateBody(bd);
				body.CreateFixture(shape, 1.0f);

				LineJointDef jd = new LineJointDef();
				Vec2 axis = new Vec2(2.0f, 1.0f);
				axis.Normalize();
				jd.Initialize(ground, body, new Vec2(0.0f, 8.5f), axis);
				jd.MotorSpeed = 0.0f;
				jd.MaxMotorForce = 100.0f;
				jd.EnableMotor = true;
				jd.LowerTranslation = -4.0f;
				jd.UpperTranslation = 4.0f;
				jd.EnableLimit = true;
				m_world.CreateJoint(jd);
			}
		}
	};
}
