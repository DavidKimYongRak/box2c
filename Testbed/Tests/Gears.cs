using System;
using Box2CS;

namespace Testbed.Tests
{
	public class Gears : Test
	{
		public Gears()
		{
			Body ground = null;
			{
				BodyDef bd = new BodyDef();
				ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(50.0f, 0.0f), new Vec2(-50.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			{
				CircleShape circle1 = new CircleShape();
				circle1.Radius = 1.0f;

				CircleShape circle2 = new CircleShape();
				circle2.Radius = 2.0f;
			
				PolygonShape box = new PolygonShape();
				box.SetAsBox(0.5f, 5.0f);

				BodyDef bd1 = new BodyDef();
				bd1.BodyType = BodyType.Dynamic;
				bd1.Position = new Vec2(-3.0f, 12.0f);
				Body body1 = m_world.CreateBody(bd1);
				body1.CreateFixture(circle1, 5.0f);

				RevoluteJointDef jd1 = new RevoluteJointDef();
				jd1.BodyA = ground;
				jd1.BodyB = body1;
				jd1.LocalAnchorA = ground.GetLocalPoint(bd1.Position);
				jd1.LocalAnchorB = body1.GetLocalPoint(bd1.Position);
				jd1.ReferenceAngle = body1.Angle - ground.Angle;
				m_joint1 = (RevoluteJoint)m_world.CreateJoint(jd1);

				BodyDef bd2 = new BodyDef();
				bd2.BodyType = BodyType.Dynamic;
				bd2.Position = new Vec2(0.0f, 12.0f);
				Body body2 = m_world.CreateBody(bd2);
				body2.CreateFixture(circle2, 5.0f);

				RevoluteJointDef jd2 = new RevoluteJointDef();
				jd2.Initialize(ground, body2, bd2.Position);
				m_joint2 = (RevoluteJoint)m_world.CreateJoint(jd2);

				BodyDef bd3 = new BodyDef();
				bd3.BodyType = BodyType.Dynamic;
				bd3.Position = new Vec2(2.5f, 12.0f);
				Body body3 = m_world.CreateBody(bd3);
				body3.CreateFixture(box, 5.0f);

				PrismaticJointDef jd3 = new PrismaticJointDef();
				jd3.Initialize(ground, body3, bd3.Position, new Vec2(0.0f, 1.0f));
				jd3.LowerTranslation = -5.0f;
				jd3.UpperTranslation = 5.0f;
				jd3.EnableLimit = true;

				m_joint3 = (PrismaticJoint)m_world.CreateJoint(jd3);

				GearJointDef jd4 = new GearJointDef();
				jd4.BodyA = body1;
				jd4.BodyB = body2;
				jd4.JointA = m_joint1;
				jd4.JointB = m_joint2;
				jd4.Ratio = circle2.Radius / circle1.Radius;
				m_joint4 = (GearJoint)m_world.CreateJoint(jd4);

				GearJointDef jd5 = new GearJointDef();
				jd5.BodyA = body2;
				jd5.BodyB = body3;
				jd5.JointA = m_joint2;
				jd5.JointB = m_joint3;
				jd5.Ratio = -1.0f / circle2.Radius;
				m_joint5 = (GearJoint)m_world.CreateJoint(jd5);
			}
		}

		public override void Draw()
		{
			base.Draw();

			float ratio, value;

			ratio = m_joint4.Ratio;
			value = m_joint1.JointAngle + ratio * m_joint2.JointAngle;
			m_debugDraw.DrawString(5, m_textLine, "theta1 + "+ratio.ToString()+" * theta2 = "+value.ToString());
			m_textLine += 15;

			ratio = m_joint5.Ratio;
			value = m_joint2.JointAngle + ratio * m_joint3.JointTranslation;
			m_debugDraw.DrawString(5, m_textLine, "theta2 + "+ratio.ToString()+" * ratio = "+value.ToString());
			m_textLine += 15;
		}

		RevoluteJoint m_joint1;
		RevoluteJoint m_joint2;
		PrismaticJoint m_joint3;
		GearJoint m_joint4;
		GearJoint m_joint5;
	};
}
