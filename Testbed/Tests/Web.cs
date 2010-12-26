using System;
using Box2CS;

namespace Testbed.Tests
{
	// This tests distance joints, body destruction, and joint destruction.
	public class Web : Test
	{
		public Web()
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
				shape.SetAsBox(0.5f, 0.5f);

				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;

				bd.Position = new Vec2(-5.0f, 5.0f);
				m_bodies[0] = m_world.CreateBody(bd);
				m_bodies[0].CreateFixture(shape, 5.0f);

				bd.Position = new Vec2(5.0f, 5.0f);
				m_bodies[1] = m_world.CreateBody(bd);
				m_bodies[1].CreateFixture(shape, 5.0f);

				bd.Position = new Vec2(5.0f, 15.0f);
				m_bodies[2] = m_world.CreateBody(bd);
				m_bodies[2].CreateFixture(shape, 5.0f);

				bd.Position = new Vec2(-5.0f, 15.0f);
				m_bodies[3] = m_world.CreateBody(bd);
				m_bodies[3].CreateFixture(shape, 5.0f);

				DistanceJointDef jd = new DistanceJointDef();
				Vec2 p1, p2, d;

				jd.FrequencyHz = 4.0f;
				jd.DampingRatio = 0.5f;

				jd.BodyA = ground;
				jd.BodyB = m_bodies[0];
				jd.LocalAnchorA = new Vec2(-10.0f, 0.0f);
				jd.LocalAnchorB = new Vec2(-0.5f, -0.5f);
				p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
				p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
				d = p2 - p1;
				jd.Length = d.Length();
				m_joints[0] = m_world.CreateJoint(jd);

				jd.BodyA = ground;
				jd.BodyB = m_bodies[1];
				jd.LocalAnchorA = new Vec2(10.0f, 0.0f);
				jd.LocalAnchorB = new Vec2(0.5f, -0.5f);
				p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
				p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
				d = p2 - p1;
				jd.Length = d.Length();
				m_joints[1] = m_world.CreateJoint(jd);

				jd.BodyA = ground;
				jd.BodyB = m_bodies[2];
				jd.LocalAnchorA = new Vec2(10.0f, 20.0f);
				jd.LocalAnchorB = new Vec2(0.5f, 0.5f);
				p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
				p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
				d = p2 - p1;
				jd.Length = d.Length();
				m_joints[2] = m_world.CreateJoint(jd);

				jd.BodyA = ground;
				jd.BodyB = m_bodies[3];
				jd.LocalAnchorA = new Vec2(-10.0f, 20.0f);
				jd.LocalAnchorB = new Vec2(-0.5f, 0.5f);
				p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
				p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
				d = p2 - p1;
				jd.Length = d.Length();
				m_joints[3] = m_world.CreateJoint(jd);

				jd.BodyA = m_bodies[0];
				jd.BodyB = m_bodies[1];
				jd.LocalAnchorA = new Vec2(0.5f, 0.0f);
				jd.LocalAnchorB = new Vec2(-0.5f, 0.0f);;
				p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
				p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
				d = p2 - p1;
				jd.Length = d.Length();
				m_joints[4] = m_world.CreateJoint(jd);

				jd.BodyA = m_bodies[1];
				jd.BodyB = m_bodies[2];
				jd.LocalAnchorA = new Vec2(0.0f, 0.5f);
				jd.LocalAnchorB = new Vec2(0.0f, -0.5f);
				p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
				p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
				d = p2 - p1;
				jd.Length = d.Length();
				m_joints[5] = m_world.CreateJoint(jd);

				jd.BodyA = m_bodies[2];
				jd.BodyB = m_bodies[3];
				jd.LocalAnchorA = new Vec2(-0.5f, 0.0f);
				jd.LocalAnchorB = new Vec2(0.5f, 0.0f);
				p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
				p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
				d = p2 - p1;
				jd.Length = d.Length();
				m_joints[6] = m_world.CreateJoint(jd);

				jd.BodyA = m_bodies[3];
				jd.BodyB = m_bodies[0];
				jd.LocalAnchorA = new Vec2(0.0f, -0.5f);
				jd.LocalAnchorB = new Vec2(0.0f, 0.5f);
				p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
				p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
				d = p2 - p1;
				jd.Length = d.Length();
				m_joints[7] = m_world.CreateJoint(jd);
			}
		}

		public override void  Keyboard(SFML.Window.KeyCode key)
		{
			switch (char.ToLower((char)key))
			{
			case 'b':
				for (int i = 0; i < 4; ++i)
				{
					if (m_bodies[i] != null)
					{
						m_world.DestroyBody(m_bodies[i]);
						m_bodies[i] = null;
						break;
					}
				}
				break;

			case 'j':
				for (int i = 0; i < 8; ++i)
				{
					if (m_joints[i] != null)
					{
						m_world.DestroyJoint(m_joints[i]);
						m_joints[i] = null;
						break;
					}
				}
				break;
			}
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "This demonstrates a soft distance joint.");
			m_textLine += 15;
			m_debugDraw.DrawString(5, m_textLine, "Press: (b) to delete a body, (j) to delete a joint");
			m_textLine += 15;		
		}

		public override void JointDestroyed(Joint joint)
		{
			for (int i = 0; i < 8; ++i)
			{
				if (m_joints[i] == joint)
				{
					m_joints[i] = null;
					break;
				}
			}
		}

		Body[] m_bodies = new Body[4];
		Joint[] m_joints = new Joint[8];
	};
}
