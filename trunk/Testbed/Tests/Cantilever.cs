using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	public class Cantilever : Test
	{
		public static string Name
		{
			get { return "Cantilever"; }
		}

		public const int e_count = 8;

		public Cantilever()
		{
			Body ground;
			{
				ground = m_world.CreateBody(new BodyDef());

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.5f, 0.125f);

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 20.0f;

				WeldJointDef jd = new WeldJointDef();

				Body prevBody = ground;
				for (int i = 0; i < e_count; ++i)
				{
					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic;
					bd.Position = new Vec2(-14.5f + 1.0f * i, 5.0f);
					Body body = m_world.CreateBody(bd);
					body.CreateFixture(fd);

					Vec2 anchor = new Vec2(-15.0f + 1.0f * i, 5.0f);
					jd.Initialize(prevBody, body, anchor);
					m_world.CreateJoint(jd);

					prevBody = body;
				}
			}

			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.5f, 0.125f);

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 20.0f;

				WeldJointDef jd = new WeldJointDef();

				Body prevBody = ground;
				for (int i = 0; i < e_count; ++i)
				{
					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic;
					bd.Position = new Vec2(-14.5f + 1.0f * i, 15.0f);
					bd.InertiaScale = 10.0f;
					Body body = m_world.CreateBody(bd);
					body.CreateFixture(fd);

					Vec2 anchor = new Vec2(-15.0f + 1.0f * i, 15.0f);
					jd.Initialize(prevBody, body, anchor);
					m_world.CreateJoint(jd);

					prevBody = body;
				}
			}

			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.5f, 0.125f);

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 20.0f;

				WeldJointDef jd = new WeldJointDef();

				Body prevBody = ground;
				for (int i = 0; i < e_count; ++i)
				{
					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic;
					bd.Position = new Vec2(-4.5f + 1.0f * i, 5.0f);
					Body body = m_world.CreateBody(bd);
					body.CreateFixture(fd);

					if (i > 0)
					{
						Vec2 anchor = new Vec2(-5.0f + 1.0f * i, 5.0f);
						jd.Initialize(prevBody, body, anchor);
						m_world.CreateJoint(jd);
					}

					prevBody = body;
				}
			}

			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.5f, 0.125f);

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 20.0f;

				WeldJointDef jd = new WeldJointDef();

				Body prevBody = ground;
				for (int i = 0; i < e_count; ++i)
				{
					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic;
					bd.Position = new Vec2(5.5f + 1.0f * i, 10.0f);
					bd.InertiaScale = 10.0f;
					Body body = m_world.CreateBody(bd);
					body.CreateFixture(fd);

					if (i > 0)
					{
						Vec2 anchor = new Vec2(5.0f + 1.0f * i, 10.0f);
						jd.Initialize(prevBody, body, anchor);
						m_world.CreateJoint(jd);
					}

					prevBody = body;
				}
			}

			for (int i = 0; i < 2; ++i)
			{
				Vec2[] vertices = new Vec2[]
				{
					new Vec2(-0.5f, 0.0f),
					new Vec2(0.5f, 0.0f),
					new Vec2(0.0f, 1.5f)
				};

				PolygonShape shape = new PolygonShape();
				shape.Vertices = vertices;

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 1.0f;

				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(-8.0f + 8.0f * i, 12.0f);
				Body body = m_world.CreateBody(bd);
				body.CreateFixture(fd);
			}

			for (int i = 0; i < 2; ++i)
			{
				CircleShape shape = new CircleShape();
				shape.Radius = 0.5f;

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 1.0f;

				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(-6.0f + 6.0f * i, 10.0f);
				Body body = m_world.CreateBody(bd);
				body.CreateFixture(fd);
			}
		}
	};
}
