using System;
using System.Collections.Generic;
using Box2CS;

namespace Testbed.Tests
{
	public class BreakableJoint
	{
		WeldJoint _joint;
		float _maxForce;

		public WeldJoint Joint
		{
			get { return _joint; }
			set { _joint = value; }
		}

		public float MaxForce
		{
			get { return _maxForce; }
			set { _maxForce = value; }
		}

		public BreakableJoint(WeldJoint joint, float maxForce)
		{
			_joint = joint;
			_maxForce = maxForce;
		}
	}

	public class Breakable : Test
	{
		public static string Name
		{
			get { return "Breakable"; }
		}

		public Breakable()
		{
			// Ground body
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			// Breakable dynamic body
			/*{
				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(0.0f, 40.0f);
				bd.Angle = (float)(0.25 * Math.PI);
				var m_body1 = m_world.CreateBody(bd);

				m_shape1.SetAsBox(0.5f, 0.5f, new Vec2(-0.5f, 0.0f), 0.0f);
				var m_piece1 = m_body1.CreateFixture(m_shape1, 1.0f);

				m_shape2.SetAsBox(0.5f, 0.5f, new Vec2(0.5f, 0.0f), 0.0f);
				var m_piece2 = m_body1.CreateFixture(m_shape2, 1.0f);

				m_breaks.Add(new BodyBreaker(m_world, m_body1, m_piece1, m_piece2));
			}*/
			float y = 2.0f;

			for (int i = 0; i < 3; ++i)
			{
				PolygonShape bottom = new PolygonShape();
				bottom.SetAsBox(1.5f, 0.15f);

				PolygonShape left = new PolygonShape();
				left.SetAsBox(0.15f, 2.7f, new Vec2(-1.45f, 2.35f), 0.2f);

				PolygonShape right = new PolygonShape();
				right.SetAsBox(0.15f, 2.7f, new Vec2(1.45f, 2.35f), -0.2f);

				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(0.0f, y += 3.5f);
				Body bottom_body = m_world.CreateBody(bd);
				var bf = bottom_body.CreateFixture(bottom, 4.0f);
				Body left_body = m_world.CreateBody(bd);
				var lf = left_body.CreateFixture(left, 4.0f);
				Body right_body = m_world.CreateBody(bd);
				var rf = right_body.CreateFixture(right, 4.0f);

				WeldJointDef wjd = new WeldJointDef();
				wjd.Initialize(bottom_body, left_body, bd.Position);
				wjd.CollideConnected = false;

				WeldJoint wj1 = (WeldJoint)m_world.CreateJoint(wjd);

				wjd.Initialize(bottom_body, right_body, bd.Position);
				WeldJoint wj2 = (WeldJoint)m_world.CreateJoint(wjd);

				m_breaks.Add(new BreakableJoint(wj1, 100));
				m_breaks.Add(new BreakableJoint(wj2, 100));
			}
		}

		public override void  PostSolve(Contact contact, ContactImpulse impulse)
		{
			foreach (var x in m_breaks)
			{
				if (_jointsToRemove.Contains(x) || x.MaxForce == 0)
				{
					// The body already broke.
					continue;
				}

				bool hasAnyFixtures = false;

				foreach (var f in x.Joint.BodyA.Fixtures)
				{
					if (f == contact.FixtureA || f == contact.FixtureB)
					{
						hasAnyFixtures = true;
						break;
					}
				}

				if (!hasAnyFixtures)
				{
					foreach (var f in x.Joint.BodyB.Fixtures)
					{
						if (f == contact.FixtureA || f == contact.FixtureB)
						{
							hasAnyFixtures = true;
							break;
						}
					}

					if (!hasAnyFixtures)
						continue;
				}

				// Should the body break?
				int count = contact.Manifold.PointCount;

				float maxImpulse = 0.0f;
				for (int i = 0; i < count; ++i)
					maxImpulse = Math.Max(maxImpulse, impulse.GetNormalImpulse(i));

				if (maxImpulse > x.MaxForce)
				{
					// Flag the body for breaking.
					_jointsToRemove.Add(x);
				}
			}
		}

		public override void  Step()
		{
			foreach (var x in _jointsToRemove)
			{
				m_breaks.Remove(x);
				m_world.DestroyJoint(x.Joint);
			}

			_jointsToRemove.Clear();

			base.Step();
		}

		enum BreakableState
		{
			Welded,
			Breaking,
			Broke
		}

		//Body m_body1;
		List<BreakableJoint> m_breaks = new List<BreakableJoint>();
		List<BreakableJoint> _jointsToRemove = new List<BreakableJoint>();
		PolygonShape m_shape1 = new PolygonShape();
		PolygonShape m_shape2 = new PolygonShape();
	};
}
