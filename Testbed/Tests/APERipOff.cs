using System;
using Box2CS;

namespace Testbed.Tests
{
	public class Capsule
	{
		Body _body;

		public Capsule(World world, Vec2 pos, float width, float height)
		{
			BodyDef bd = new BodyDef(EBodyType.b2_dynamicBody, Vec2.Empty);
			_body = world.CreateBody(bd);

			{
				PolygonShape poly = APERipOff.MakeShape(pos.x, pos.y, width, height, 0);
				_body.CreateFixture(new FixtureDef(poly, 1.0f));
			}

			{
				var p = pos + new Vec2(width / 2, 0);
				CircleShape circle = APERipOff.MakeCircle(p.x, p.y, height / 2);
				_body.CreateFixture(new FixtureDef(circle, 1.0f));

				p = pos - new Vec2(width / 2, 0);
				circle = APERipOff.MakeCircle(p.x, p.y, height / 2);
				_body.CreateFixture(new FixtureDef(circle, 1.0f));
			}
		}
	}

	public class APERipOff : Test
	{
		public static Vec2 WorldScale = new Vec2(0.05f, 0.05f);

		public static PolygonShape MakeShape(float x, float y, float w, float h, float angle)
		{
			return new PolygonShape(w / 2, h / 2, new Vec2(x, y), angle);
		}

		public static CircleShape MakeCircle(float x, float y, float r)
		{
			return new CircleShape(new Vec2(x, y), r);
		}

		public APERipOff()
		{
			TestSettings.pause = true;

			new Capsule(m_world, new Vec2(300, 10), 16, 26);

			Body ground = m_world.CreateBody(new BodyDef());

			{
				ground.CreateFixture(new FixtureDef(MakeShape(340, 327, 550, 50, 0)));

				ground.CreateFixture(new FixtureDef(MakeShape(325, -33, 649, 80, 0)));

				ground.CreateFixture(new FixtureDef(MakeShape(375, 220, 390, 20, 0.405f)));

				ground.CreateFixture(new FixtureDef(MakeShape(90, 200, 102, 20, -.7f)));

				ground.CreateFixture(new FixtureDef(MakeShape(96, 129, 102, 20, -.7f)));

				ground.CreateFixture(new FixtureDef(MakeCircle(175, 190, 60)));

				ground.CreateFixture(new FixtureDef(MakeCircle(600, 660, 400)));

				ground.CreateFixture(new FixtureDef(MakeShape(30, 370, 32, 60, 0), 0.0f, 2.0f));

				ground.CreateFixture(new FixtureDef(MakeShape(1, 99, 30, 500, 0)));

				ground.CreateFixture(new FixtureDef(MakeShape(54, 300, 20, 150, 0)));

				ground.CreateFixture(new FixtureDef(MakeShape(54, 122, 20, 94, 0)));

				ground.CreateFixture(new FixtureDef(MakeShape(75, 65, 60, 25, -0.7f)));

				ground.CreateFixture(new FixtureDef(MakeShape(23, 11, 65, 40, -0.7f)));

				ground.CreateFixture(new FixtureDef(MakeShape(654, 230, 50, 500, 0)));

				ground.CreateFixture(new FixtureDef(MakeShape(127, 49, 75, 25, 0)));

				ground.CreateFixture(new FixtureDef(MakeShape(483, 55, 100, 15, 0)));
			}
		}
	}
}
