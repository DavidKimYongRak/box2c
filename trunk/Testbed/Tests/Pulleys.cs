using System;
using Box2CS;

namespace Testbed.Tests
{
	public class Pulleys : Test
	{
		public Pulleys()
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
				float a = 2.0f;
				float b = 4.0f;
				float y = 16.0f;
				float L = 12.0f;

				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(a, b);

				BodyDef bd = new BodyDef();
				bd.BodyType = EBodyType.b2_dynamicBody;

				bd.Position = new Vec2(-10.0f, y);
				Body body1 = m_world.CreateBody(bd);
				body1.CreateFixture(shape, 5.0f);

				bd.Position = new Vec2(10.0f, y);
				Body body2 = m_world.CreateBody(bd);
				body2.CreateFixture(shape, 5.0f);

				PulleyJointDef pulleyDef = new PulleyJointDef();
				Vec2 anchor1 = new Vec2(-10.0f, y + b);
				Vec2 anchor2 = new Vec2(10.0f, y + b);
				Vec2 groundAnchor1 = new Vec2(-10.0f, y + b + L);
				Vec2 groundAnchor2 = new Vec2(10.0f, y + b + L);
				pulleyDef.Initialize(body1, body2, groundAnchor1, groundAnchor2, anchor1, anchor2, 2.0f);

				m_joint1 = (PulleyJoint)m_world.CreateJoint(pulleyDef);
			}
		}

		public override void Draw()
		{
			base.Draw();

			float ratio = m_joint1.Ratio;
			float L = m_joint1.LengthA + ratio * m_joint1.LengthB;
			m_debugDraw.DrawString(5, m_textLine, "L1 + "+ratio.ToString()+" * L2 = "+L.ToString());
			m_textLine += 15;	
		}

		PulleyJoint m_joint1;
	};
}
