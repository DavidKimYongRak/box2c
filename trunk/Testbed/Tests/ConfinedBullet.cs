using System;
using Box2CS;

namespace Testbed
{
	public class ConfinedBullet : Test
	{
		public static string Name
		{
			get { return "Confined Bullet"; }
		}
		const int e_count = 24;

		public override void BeginContact(Contact contact)
		{
		}

		public override void EndContact(Contact contact)
		{
		}

		public override void PostSolve(Contact contact, ContactImpulse impulse)
		{
		}

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
					bd.BodyType = EBodyType.b2_dynamicBody;
					bd.Position = new Vec2(0, 10.0f);
					using (MeshShape ms = new MeshShape("stool.bmesh", 1, false))
					{
						var body = m_world.CreateBody(bd);
						body.IsBullet = true;

						var fixtures = ms.AddToBody(body, 2);

						foreach (var s in fixtures)
						{
							s.Restitution = 10;
							s.Friction = 0.1f;
						}

						bd.Position = new Vec2(0, 5.0f);
						body = m_world.CreateBody(bd);
						body.IsBullet = true;

						fixtures = ms.AddToBody(body, 2);

						foreach (var s in fixtures)
						{
							s.Restitution = 10;
							s.Friction = 0.1f;
						}

						bd.Position = new Vec2(0, 15.0f);
						body = m_world.CreateBody(bd);
						body.IsBullet = true;

						fixtures = ms.AddToBody(body, 2);

						foreach (var s in fixtures)
						{
							s.Restitution = 10;
							s.Friction = 0.1f;
						}
					}
				}

			}
		}
	};
}
