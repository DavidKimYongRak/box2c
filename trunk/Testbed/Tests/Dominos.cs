using System;
using Box2CS;

namespace Testbed.Tests
{
	public class Dominos : Test
	{
		public static string Name
		{
			get { return "Dominos"; }
		}

		public Dominos()
		{
			Body b1 = m_world.CreateBody(new BodyDef());
			b1.CreateFixture(new PolygonShape(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f)), 0.0f);
			b1.UserData = 1;

			{
				Body ground = m_world.CreateBody(new BodyDef(BodyType.Static, new Vec2(-1.5f, 10.0f)));
				ground.CreateFixture(new PolygonShape(6.0f, 0.25f), 0.0f);
				ground.UserData = "ground";
			}

			for (int i = 0; i < 10; ++i)
			{
				Body body = m_world.CreateBody(new BodyDef(BodyType.Dynamic, new Vec2(-6.0f + 1.0f * i, 11.25f), 0));
				body.CreateFixture(new FixtureDef(new PolygonShape(0.1f, 1.0f), 20.0f, 0.0f, 0.1f));
				body.UserData = "domino " + i.ToString();
			}

			{
				Body ground = m_world.CreateBody(new BodyDef(BodyType.Static, new Vec2(1.0f, 6.0f)));
				ground.CreateFixture(new PolygonShape(7.0f, 0.25f, Vec2.Empty, 0.3f), 0.0f);
				ground.UserData = "ground";
			}

			Body b2;
			{
				b2 = m_world.CreateBody(new BodyDef(BodyType.Static, new Vec2(-7.0f, 4.0f)));
				b2.CreateFixture(new PolygonShape(0.25f, 1.5f), 0.0f);
				b2.UserData = b2;
			}

			Body b3;
			{
				b3 = m_world.CreateBody(new BodyDef(BodyType.Dynamic, new Vec2(-0.9f, 1.0f), -0.15f));
				b3.CreateFixture(new PolygonShape(6.0f, 0.125f), 10.0f);
				b3.UserData = 3;
			}

			RevoluteJointDef jd = new RevoluteJointDef();
			Vec2 anchor = new Vec2(-2.0f, 1.0f);
			jd.Initialize(b1, b3, anchor);
			jd.CollideConnected = true;
			m_world.CreateJoint(jd);

			Body b4;
			{
				b4 = m_world.CreateBody(new BodyDef(BodyType.Dynamic, new Vec2(-10.0f, 15.0f)));
				b4.CreateFixture(new PolygonShape(0.25f, 0.25f), 10.0f);
				b4.UserData = 4;
			}

			anchor = new Vec2(-7.0f, 15.0f);
			jd.Initialize(b2, b4, anchor);
			m_world.CreateJoint(jd);

			Body b5;
			{
				b5 = m_world.CreateBody(new BodyDef(BodyType.Dynamic, new Vec2(6.5f, 3.0f)));

				b5.CreateFixture(new FixtureDef(new PolygonShape(1.0f, 0.1f, new Vec2(0.0f, -0.9f), 0.0f), 10.0f, 0.0f, 0.1f));
				b5.CreateFixture(new FixtureDef(new PolygonShape(0.1f, 1.0f, new Vec2(-0.9f, 0.0f), 0.0f), 10.0f, 0.0f, 0.1f));
				b5.CreateFixture(new FixtureDef(new PolygonShape(0.1f, 1.0f, new Vec2(0.9f, 0.0f), 0.0f), 10.0f, 0.0f, 0.1f));

				b5.UserData = 5;
			}

			anchor = new Vec2(6.0f, 2.0f);
			jd.Initialize(b1, b5, anchor);
			m_world.CreateJoint(jd);

			Body b6;
			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(1.0f, 0.1f);

				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(6.5f, 4.1f);
				b6 = m_world.CreateBody(bd);
				b6.CreateFixture(shape, 30.0f);
				b6.UserData = 6;
			}

			anchor = new Vec2(7.5f, 4.0f);
			jd.Initialize(b5, b6, anchor);
			m_world.CreateJoint(jd);

			Body b7;
			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.1f, 1.0f);

				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(7.4f, 1.0f);

				b7 = m_world.CreateBody(bd);
				b7.CreateFixture(shape, 10.0f);
				b7.UserData = 7;
			}

			DistanceJointDef djd = new DistanceJointDef();
			djd.BodyA = b3;
			djd.BodyB = b7;
			djd.LocalAnchorA = new Vec2(6.0f, 0.0f);
			djd.LocalAnchorB = new Vec2(0.0f, -1.0f);
			Vec2 d = djd.BodyB.GetWorldPoint(djd.LocalAnchorB) - djd.BodyA.GetWorldPoint(djd.LocalAnchorA);
			djd.Length = d.Length();
			m_world.CreateJoint(djd);

			{
				float radius = 0.2f;

				CircleShape shape = new CircleShape();
				shape.Radius = radius;

				for (int i = 0; i < 4; ++i)
				{
					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic;
					bd.Position = new Vec2(5.9f + 2.0f * radius * i, 2.4f);
					Body body = m_world.CreateBody(bd);
					body.CreateFixture(shape, 10.0f);
					body.UserData = "circle";
				}
			}
		}
	};
}
