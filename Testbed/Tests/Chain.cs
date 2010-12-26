using System;
using Box2CS;

namespace Testbed.Tests
{
	public class Chain : Test
	{
		public Chain()
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
				shape.SetAsBox(0.6f, 0.075f);

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 20.0f;
				fd.Friction = 0.2f;

				RevoluteJointDef jd = new RevoluteJointDef();
				jd.CollideConnected = false;

				const float y = 25.0f;
				Body prevBody = ground;
				for (int i = 0; i < 30; ++i)
				{
					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic;
					bd.Position = new Vec2(0.5f + i, y);
					Body body = m_world.CreateBody(bd);
					body.CreateFixture(fd);

					Vec2 anchor = new Vec2(i, y);
					jd.Initialize(prevBody, body, anchor);
					m_world.CreateJoint(jd);

					prevBody = body;
				}
			}
		}
	};

}
