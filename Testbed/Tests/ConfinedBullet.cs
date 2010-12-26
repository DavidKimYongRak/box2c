using System;
using Box2CS;

namespace Testbed.Tests
{
	public class ConfinedBullet : Test
	{
		public static string Name
		{
			get { return "Confined Bullet"; }
		}
		const int e_count = 24;

		public ConfinedBullet()
		{
			{
				BodyDef bd = new BodyDef();
				{
					Body ground = m_world.CreateBody(bd);

					PolygonShape shape = new PolygonShape();
					{
						shape.SetAsEdge(new Vec2(-10.0f, 0.0f), new Vec2(10.0f, 0.0f));
						ground.CreateFixture(shape, 0.0f);
					}

					ground = m_world.CreateBody(bd);

					shape = new PolygonShape();
					{
						shape.SetAsEdge(new Vec2(-10.0f, 20.0f), new Vec2(10.0f, 20.0f));
						ground.CreateFixture(shape, 0.0f);
					}

					ground = m_world.CreateBody(bd);

					shape = new PolygonShape();
					{
						shape.SetAsEdge(new Vec2(-10.0f, 20.0f), new Vec2(-10.0f, 0.0f));
						ground.CreateFixture(shape, 0.0f);
					}

					ground = m_world.CreateBody(bd);

					shape = new PolygonShape();
					{
						shape.SetAsEdge(new Vec2(10.0f, 20.0f), new Vec2(10.0f, 0.0f));
						ground.CreateFixture(shape, 0.0f);
					}
				}

				bd = new BodyDef();
				{
					bd.BodyType = BodyType.Dynamic;
					bd.Position = new Vec2(0, 10.0f);
					{
						PolygonShape bottom = new PolygonShape(1.5f, 0.15f);
						PolygonShape left = new PolygonShape(0.15f, 2.7f, new Vec2(-1.45f, 2.35f), 0.2f);
						PolygonShape right = new PolygonShape(0.15f, 2.7f, new Vec2(1.45f, 2.35f), -0.2f);

						var body = m_world.CreateBody(bd);
						body.IsBullet = true;
						body.CreateFixture(new FixtureDef(bottom, 1.0f, 10, 0.1f));
						body.CreateFixture(new FixtureDef(left, 1.0f, 10, 0.1f));
						body.CreateFixture(new FixtureDef(right, 1.0f, 10, 0.1f));
					}
				}
			}
		}
	};
}
