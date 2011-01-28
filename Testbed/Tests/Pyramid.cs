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
		int totalCount = 0;

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
								body.UserData = totalCount;
								totalCount++;
								m_bodies.Add(body);

								y += deltaY;
							}
							x += deltaX;
						}

						m_touching = new bool[totalCount];
					}
				}
			}

			{
				Body movableBody = m_world.CreateBody(new BodyDef());
				movableBody.Position = new Vec2(999, 999);
				CircleShape shape = new CircleShape();
				shape.Radius = 7.0f;
				shape.Position = new Vec2(0, 0);

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.IsSensor = true;
				m_sensor = movableBody.CreateFixture(fd);
			}
		}

		bool movingSensor = false;
		Vec2 cursorPos = new Vec2();

		public override void Keyboard(SFML.Window.KeyCode key)
		{
			switch (char.ToLower((char)key))
			{
			case 'e':
				PhysExplosion(cursorPos, 6, 12);
				break;
			}
			base.Keyboard(key);
		}

		public override void MouseDown(Vec2 p)
		{
			movingSensor = true;
			base.MouseDown(p);
		}

		public override void MouseMove(Vec2 p)
		{
			cursorPos = p;
			if (movingSensor)
			{
				m_sensor.Body.Position = p;	
			}
			base.MouseMove(p);
		}

		public override void MouseUp(Vec2 p)
		{
			movingSensor = false;
			base.MouseUp(p);
		}

		// Implement contact listener.
		public override void BeginContact(Contact contact)
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
		public override void EndContact(Contact contact)
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

		public override void Step()
		{
			base.Step();

			// Traverse the contact results. Apply a force on shapes
			// that overlap the sensor.
			for (int i = 0; i < m_bodies.Count; ++i)
			{
				if (m_touching[i] == false)
					continue;

				Body body = m_bodies[i];
				Body ground = m_sensor.Body;

				CircleShape circle = (CircleShape)m_sensor.Shape;
				Vec2 center = ground.WorldCenter;

				Vec2 position = body.Position;

				Vec2 d = center - position;
				if (d.LengthSquared() < float.Epsilon * float.Epsilon)
					continue;

				d.Normalize();
				Vec2 F = (body.Mass * 28) * d;
				body.ApplyForce(F, position);
			}
		}

		class PhysExplosionCallback : QueryCallback
		{
			public float Force, Radius;
			public Vec2 pos;

			/// Called for each fixture found in the query.
			/// @return false to terminate the query.
			public override bool ReportFixture(Fixture fixture)
			{
				Body Body = fixture.Body;

				//if (MyBody && (MyBody->Type == CT_BOX || MyBody->Type == CT_SHELLS))
				{
					Vec2 BodyPos = Body.WorldCenter;
					Vec2 HitVector = (BodyPos-pos);
					float Distance = HitVector.Normalize(); //Makes a 1 unit length vector from HitVector, while getting the length.
					if ((Body.BodyType == BodyType.Dynamic) && (Distance<=Radius))
					{
						float HitForce=(Radius-Distance)*Force; //TODO: This is linear, but that's not realistic.
						Body.ApplyLinearImpulse(HitForce * HitVector, Body.WorldCenter);
					};
				}
				return true;
			}
		};

		void PhysExplosion(Vec2 pos, float Radius, float Force)
		{
			AABB Sector = new AABB();
			Sector.LowerBound = new Vec2(pos.X-Radius, pos.Y-Radius);
			Sector.UpperBound = new Vec2(pos.X+Radius, pos.Y+Radius);

			PhysExplosionCallback cb = new PhysExplosionCallback();
			cb.Radius = Radius;
			cb.Force = Force;
			cb.pos = pos;

			m_world.QueryAABB(cb, Sector);
		}

		Fixture m_sensor;
		System.Collections.Generic.List<Body> m_bodies = new System.Collections.Generic.List<Body>();
		bool[] m_touching;
	};
}
