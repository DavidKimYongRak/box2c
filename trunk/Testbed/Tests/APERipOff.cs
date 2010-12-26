using System;
using Box2CS;

namespace Testbed.Tests
{
	public class Capsule
	{
		Body _body;

		public Body Body
		{
			get { return _body; }
		}

		public Capsule(World world, Vec2 pos, float width, float height, float density)
		{
			BodyDef bd = new BodyDef(EBodyType.b2_dynamicBody, Vec2.Empty);
			_body = world.CreateBody(bd);

			{
				PolygonShape poly = APERipOff.MakeShape(pos.x, pos.y, width, height, 0);
				_body.CreateFixture(new FixtureDef(poly, density));
			}

			{
				var p = pos + new Vec2(width / 2, 0);
				CircleShape circle = APERipOff.MakeCircle(p.x, p.y, height / 2);
				_body.CreateFixture(new FixtureDef(circle, density));

				p = pos - new Vec2(width / 2, 0);
				circle = APERipOff.MakeCircle(p.x, p.y, height / 2);
				_body.CreateFixture(new FixtureDef(circle, density));
			}
		}
	}

	public class APERipOff : Test
	{
		public static Vec2 WorldScale = new Vec2(0.05f, 0.05f);

		public static PolygonShape MakeShape(float x, float y, float w, float h, float angle)
		{
			PolygonShape sh = new PolygonShape();
			sh.AutoReverse = true;
			sh.SetAsBox(w / 2, h / 2, new Vec2(x, y), (float)Math.PI + angle);
			return sh;
		}

		public static CircleShape MakeCircle(float x, float y, float r)
		{
			return new CircleShape(new Vec2(x, y), r);
		}

		public APERipOff()
		{
			TestSettings.pause = true;

			Body ground = m_world.CreateBody(new BodyDef());

			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(-26, 40), new Vec2(32, 40))));
			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(-26, 40), new Vec2(-26 - 5, 40 - 4))));
			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(-26 - 5, 40 - 4), new Vec2(-26 - 5, 40 - 44))));
			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(32, 40), new Vec2(32, 40 - 44))));
			ground.CreateFixture(new FixtureDef(new CircleShape(new Vec2(27, 40 - 58), 25)));
			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(24, 2), new Vec2(-3 - 5, 19))));
			ground.CreateFixture(new FixtureDef(new CircleShape(new Vec2(-7.25f - 5, 14.75f), 6)));

			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(-12 - 5, 18), new Vec2(-26 - 5 + 5, 11))));
			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(-26 - 5 + 5, 11), new Vec2(-26 - 5 + 5, 40 - 44))));
			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(-26 - 5, 40 - 44), new Vec2(-26 - 5 + 5, 40 - 44)), 0.0f, 2.0f));

			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(-26 + 5, 40 - 5), new Vec2(-26 - 5 + 5, 40 - 4 - 5))));
			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(-26 - 5 + 5, 40 - 4 - 5), new Vec2(-26 - 5 + 5, 40 - 22))));
			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(-12 - 5, 18 + 7), new Vec2(-26 - 5 + 5, 11 + 7))));
			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(-26 + 5, 40 - 5), new Vec2(-26 + 15, 40 - 5))));
			ground.CreateFixture(new FixtureDef(new PolygonShape(new Vec2(-26 + 35, 40 - 5), new Vec2(-26 + 35 + 12, 40 - 5))));

			//Capsule cantilever = new Capsule(m_world, new Vec2(-26 + 52.8f, 40 - 5 - (1.40f / 2)), 8.8f, 1.40f, 1.0f);

			BodyDef bd = new BodyDef(EBodyType.b2_dynamicBody, Vec2.Empty);
			var cantilever = m_world.CreateBody(bd);
			var cantpos = new Vec2(-26 + 52.8f, 40 - 5 - (1.40f / 2));

			{
				PolygonShape poly = APERipOff.MakeShape(cantpos.x, cantpos.y, 8.8f, 1.40f, 0);
				cantilever.CreateFixture(new FixtureDef(poly, 0.05f));
			}

			{
				var p = cantpos + new Vec2(8.8f / 2, 0);
				CircleShape circle = APERipOff.MakeCircle(p.x, p.y, 1.40f / 2);
				var tempbody = m_world.CreateBody(bd);
				tempbody.CreateFixture(new FixtureDef(circle, 0.05f));

				p = cantpos - new Vec2(8.8f / 2, 0);
				circle = APERipOff.MakeCircle(p.x, p.y, 1.40f / 2);
				cantilever.CreateFixture(new FixtureDef(circle, 0.05f));

				WeldJointDef wjd = new WeldJointDef();
				wjd.Initialize(tempbody, ground, tempbody.WorldCenter);
				wjd.CollideConnected = false;
				m_world.CreateJoint(wjd);

				WeldJointDef rjd = new WeldJointDef();
				rjd.Initialize(tempbody, cantilever, tempbody.WorldCenter);
				rjd.CollideConnected = false;
				m_world.CreateJoint(rjd);
			}

			new Capsule(m_world, new Vec2(0, 24), 5, 4, 0.2f);

			{
				Body squareThing;
				bd = new BodyDef(EBodyType.b2_kinematicBody, new Vec2(23, 17));

				squareThing = m_world.CreateBody(bd);

				squareThing.CreateFixture(new FixtureDef(new CircleShape(new Vec2(-1, 4), 0.35f), 1));
				squareThing.CreateFixture(new FixtureDef(new CircleShape(new Vec2(1, 4), 0.35f), 1));
				squareThing.CreateFixture(new FixtureDef(new CircleShape(new Vec2(-1, -4.5f), 0.35f), 1));
				squareThing.CreateFixture(new FixtureDef(new CircleShape(new Vec2(1, -4.5f), 0.35f), 1));

				squareThing.CreateFixture(new FixtureDef(new PolygonShape(1, 0.35f, new Vec2(0, -4.5f), 0), 1));
				squareThing.CreateFixture(new FixtureDef(new PolygonShape(1, 0.35f, new Vec2(0, 4.0f), 0), 1));
				squareThing.CreateFixture(new FixtureDef(new PolygonShape(0.35f, 4.25f, new Vec2(-1, -0.25f), 0), 1));
				squareThing.CreateFixture(new FixtureDef(new PolygonShape(0.35f, 4.25f, new Vec2(1, -0.25f), 0), 1));

				squareThing.AngularVelocity = -0.50f;

				m_world.CreateBody(new BodyDef(EBodyType.b2_dynamicBody, new Vec2(23, 19)))
					.CreateFixture(new FixtureDef(new CircleShape(0.625f), 1));

				Body squareOne = m_world.CreateBody(new BodyDef(EBodyType.b2_dynamicBody, squareThing.WorldCenter + new Vec2(1, 8.0f)));
				squareOne.CreateFixture(new PolygonShape(0.50f, 0.50f), 500);

				RevoluteJointDef rjd = new RevoluteJointDef();
				rjd.Initialize(squareThing, squareOne, squareThing.WorldCenter + new Vec2(1, 4));
				rjd.CollideConnected = true;
				m_world.CreateJoint(rjd);

				Body squareTwo = m_world.CreateBody(new BodyDef(EBodyType.b2_dynamicBody, squareThing.WorldCenter + new Vec2(-1, -8.5f)));
				squareTwo.CreateFixture(new PolygonShape(0.50f, 0.50f), 500);

				rjd = new RevoluteJointDef();
				rjd.Initialize(squareThing, squareTwo, squareThing.WorldCenter + new Vec2(-1, -4.5f));
				rjd.CollideConnected = true;
				m_world.CreateJoint(rjd);
			}
		}
	}
}
