using System;
using Box2CS;

namespace Testbed.Tests
{
	public class CompoundShapes : Test
	{
		public static string Name
		{
			get { return "CompoundShapes"; }
		}

		public override void BeginContact(Contact contact)
		{
		}

		public override void EndContact(Contact contact)
		{
		}

		public override void PostSolve(Contact contact, ContactImpulse impulse)
		{
		}

		public CompoundShapes()
		{
			{
				BodyDef bd = new BodyDef();
				{
					bd.Position = new Vec2(0.0f, 0.0f);
					Body body = m_world.CreateBody(bd);

					PolygonShape shape = new PolygonShape();
					shape.SetAsEdge(new Vec2(50.0f, 0.0f), new Vec2(-50.0f, 0.0f));

					body.CreateFixture(shape, 0.0f);
				}
			}

			{
				CircleShape circle1 = new CircleShape();
				circle1.Radius = 0.5f;
				circle1.Position = new Vec2(-0.5f, 0.5f);

				CircleShape circle2 = new CircleShape();
				circle2.Radius = 0.5f;
				circle2.Position = new Vec2(0.5f, 0.5f);

				for (int i = 0; i < 10; ++i)
				{
					float x = Rand.RandomFloat(-0.1f, 0.1f);
					BodyDef bd = new BodyDef();
					{
						bd.BodyType = EBodyType.b2_dynamicBody;
						bd.Position = new Vec2(x + 5.0f, 1.05f + 2.5f * i);
						bd.Angle = Rand.RandomFloat(-(float)Math.PI, (float)Math.PI);
						Body body = m_world.CreateBody(bd);
						body.CreateFixture(circle1, 2.0f);
						body.CreateFixture(circle2, 0.0f);
					}
				}
			}

			{
				var mesh = new MeshShape("mine.bmesh", 1.5f, false);

				for (int i = 0; i < 10; ++i)
				{
					float x = Rand.RandomFloat(-0.1f, 0.1f);
					BodyDef bd = new BodyDef();
					{
						bd.BodyType = EBodyType.b2_dynamicBody;
						bd.Position = new Vec2(x - 5.0f, 1.05f + 2.5f * i);
						bd.Angle = Rand.RandomFloat((float)-Math.PI, (float)Math.PI);
						Body body = m_world.CreateBody(bd);
						mesh.AddToBody(body, 2);
						body.UserData = "thingies";
					}
				}
			}

			{
				Transform xf1 = new Transform();
				xf1.R.Set(0.3524f * (float)Math.PI);
				xf1.position = xf1.R * new Vec2(1.0f, 0.0f);

				Vec2[] vertices = new Vec2[3];

				PolygonShape triangle1 = new PolygonShape();
				vertices[0] = xf1 * new Vec2(-1.0f, 0.0f);
				vertices[1] = xf1 * new Vec2(1.0f, 0.0f);
				vertices[2] = xf1 * new Vec2(0.0f, 0.5f);
				triangle1.Vertices = vertices;

				Transform xf2 = new Transform();
				xf2.R.Set(-0.3524f * (float)Math.PI);
				xf2.position = xf2.R * new Vec2(-1.0f, 0.0f);

				PolygonShape triangle2 = new PolygonShape();
				vertices[0] = xf2 * new Vec2(-1.0f, 0.0f);
				vertices[1] = xf2 * new Vec2(1.0f, 0.0f);
				vertices[2] = xf2 * new Vec2(0.0f, 0.5f);
				triangle2.Vertices = vertices;

				for (int i = 0; i < 10; ++i)
				{
					float x = Rand.RandomFloat(-0.1f, 0.1f);
					BodyDef bd = new BodyDef();
					{
						bd.BodyType = EBodyType.b2_dynamicBody;
						bd.Position = new Vec2(x, 2.05f + 2.5f * i);
						bd.Angle = 0.0f;
						Body body = m_world.CreateBody(bd);
						body.CreateFixture(triangle1, 2.0f);
						body.CreateFixture(triangle2, 2.0f);
						body.UserData = "ship";
					}
				}
			}

			{
				PolygonShape bottom = new PolygonShape();
				bottom.SetAsBox(1.5f, 0.15f);

				PolygonShape left = new PolygonShape();
				left.SetAsBox(0.15f, 2.7f, new Vec2(-1.45f, 2.35f), 0.2f);

				PolygonShape right = new PolygonShape();
				right.SetAsBox(0.15f, 2.7f, new Vec2(1.45f, 2.35f), -0.2f);

				BodyDef bd = new BodyDef();
				bd.BodyType = EBodyType.b2_dynamicBody;
				bd.Position = new Vec2(0.0f, 2.0f);
				Body body = m_world.CreateBody(bd);
				body.CreateFixture(bottom, 4.0f);
				body.CreateFixture(left, 4.0f);
				body.CreateFixture(right, 4.0f);

				body.UserData = "stool";
			}
		}
	};
}
