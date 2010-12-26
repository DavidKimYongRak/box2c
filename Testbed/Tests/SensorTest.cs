using System;
using Box2CS;

namespace Testbed.Tests
{
	// This is used to test sensor shapes.
	public class SensorTest : Test
	{
		const int e_count = 14;

		public SensorTest()
		{
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				{
					PolygonShape shape = new PolygonShape();
					shape.SetAsEdge(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
					ground.CreateFixture(shape, 0.0f);
				}

				{
					CircleShape shape = new CircleShape();
					shape.Radius = 5.0f;
					shape.Position = new Vec2(0.0f, 10.0f);

					FixtureDef fd = new FixtureDef();
					fd.Shape = shape;
					fd.IsSensor = true;
					m_sensor = ground.CreateFixture(fd);
				}
			}

			{
				CircleShape shape = new CircleShape();

				for (int i = 0; i < e_count; ++i)
				{
					shape.Radius = Rand.RandomFloat(0.85f, 1.0f);

					BodyDef bd = new BodyDef();
					bd.BodyType = EBodyType.b2_dynamicBody;
					bd.Position = new Vec2(-10.0f + 3.0f * i, 20.0f);
					bd.UserData = i;

					m_touching[i] = false;
					m_bodies[i] = m_world.CreateBody(bd);

					m_bodies[i].CreateFixture(shape, 1.0f);
				}
			}
		}

		// Implement contact listener.
		public override void  BeginContact(Contact contact)
		{
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;

			if (fixtureA == m_sensor)
			{
				object userData = fixtureB.Body.UserData;
				if (userData != null && userData is int)
					m_touching[(int)fixtureB.Body.UserData] = true;
			}

			if (fixtureB == m_sensor)
			{
				object userData = fixtureA.Body.UserData;
				if (userData != null && userData is int)
					m_touching[(int)fixtureA.Body.UserData] = true;
			}
		}

		// Implement contact listener.
		public override void  EndContact(Contact contact)
		{
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;

			if (fixtureA == m_sensor)
			{
				object userData = fixtureB.Body.UserData;
				if (userData != null && userData is int)
					m_touching[(int)fixtureB.Body.UserData] = false;

			}

			if (fixtureB == m_sensor)
			{
				object userData = fixtureA.Body.UserData;
				if (userData != null && userData is int)
					m_touching[(int)fixtureA.Body.UserData] = false;
			}
		}

		public override void  Step()
		{
			base.Step();

			// Traverse the contact results. Apply a force on shapes
			// that overlap the sensor.
			for (int i = 0; i < e_count; ++i)
			{
				if (m_touching[i] == false)
					continue;

				Body body = m_bodies[i];
				Body ground = m_sensor.Body;

				CircleShape circle = (CircleShape)m_sensor.Shape;
				Vec2 center = ground.GetWorldPoint(circle.Position);

				Vec2 position = body.Position;

				Vec2 d = center - position;
				if (d.LengthSquared() < float.Epsilon * float.Epsilon)
					continue;

				d.Normalize();
				Vec2 F = (body.Mass * 32) * d;
				body.ApplyForce(F, position);
			}
		}

		Fixture m_sensor;
		Body[] m_bodies = new Body[e_count];
		bool[] m_touching = new bool[e_count];
	};
}
