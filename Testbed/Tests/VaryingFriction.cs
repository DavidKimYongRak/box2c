using System;
using Box2CS;

namespace Testbed.Tests
{
	public class VaryingFriction : Test
	{
		public VaryingFriction()
		{
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(13.0f, 0.25f);

				BodyDef bd = new BodyDef();
				bd.Position = new Vec2(-4.0f, 22.0f);
				bd.Angle = -0.25f;

				Body ground = m_world.CreateBody(bd);
				ground.CreateFixture(shape, 0.0f);
			}

			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.25f, 1.0f);

				BodyDef bd = new BodyDef();
				bd.Position = new Vec2(10.5f, 19.0f);

				Body ground = m_world.CreateBody(bd);
				ground.CreateFixture(shape, 0.0f);
			}

			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(13.0f, 0.25f);

				BodyDef bd = new BodyDef();
				bd.Position = new Vec2(4.0f, 14.0f);
				bd.Angle = 0.25f;

				Body ground = m_world.CreateBody(bd);
				ground.CreateFixture(shape, 0.0f);
			}

			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.25f, 1.0f);

				BodyDef bd = new BodyDef();
				bd.Position = new Vec2(-10.5f, 11.0f);

				Body ground = m_world.CreateBody(bd);
				ground.CreateFixture(shape, 0.0f);
			}

			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(13.0f, 0.25f);

				BodyDef bd = new BodyDef();
				bd.Position = new Vec2(-4.0f, 6.0f);
				bd.Angle = -0.25f;

				Body ground = m_world.CreateBody(bd);
				ground.CreateFixture(shape, 0.0f);
			}

			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.5f, 0.5f);

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 25.0f;

				float[] friction = {0.75f, 0.5f, 0.35f, 0.1f, 0.0f};

				for (int i = 0; i < 5; ++i)
				{
					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic;
					bd.Position = new Vec2(-15.0f + 4.0f * i, 28.0f);
					Body body = m_world.CreateBody(bd);

					fd.Friction = friction[i];
					body.CreateFixture(fd);
				}
			}
		}
	};
}
