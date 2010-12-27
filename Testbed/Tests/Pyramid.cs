using System;
using Box2CS;

namespace Testbed.Tests
{
	public class Pyramid : Test
	{
		public static string Name
		{
			get { return "Pyramid"; }
		}

		const int e_count = 35;

		public Pyramid()
		{
			{
				BodyDef bd = new BodyDef();
				{
					Body ground = m_world.CreateBody(bd);

					PolygonShape shape = new PolygonShape();
					{
						shape.SetAsEdge(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
						ground.CreateFixture(shape, 0.0f);
					}
				}
			}

			{
				float a = 0.5f;
				PolygonShape shape = new PolygonShape();
				{
					shape.SetAsBox(a, a);

					Vec2 x = new Vec2(-7.0f, 0.75f), y;
					Vec2 deltaX = new Vec2(0.5625f, 1.25f);
					Vec2 deltaY = new Vec2(1.125f, 0.0f);

					System.Collections.Generic.List<Body> bodies = new System.Collections.Generic.List<Body>();

					BodyDef bd = new BodyDef();
					{
						for (int i = 0; i < e_count; ++i)
						{
							y = x;

							for (int j = i; j < e_count; ++j)
							{
								bd.BodyType = BodyType.Dynamic;
								bd.Position = y;

								Body body = m_world.CreateBody(bd);
								body.CreateFixture(shape, 5.0f);

								bodies.Add(body);

								y += deltaY;
							}
							x += deltaX;
						}

					}
				}
			}
		}
	};
}
