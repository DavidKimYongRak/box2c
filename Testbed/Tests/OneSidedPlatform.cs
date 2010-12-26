using System;
using Box2CS;

namespace Testbed.Tests
{
	public class OneSidedPlatform : Test
	{
		enum State
		{
			e_unknown,
			e_above,
			e_below,
		};

		public OneSidedPlatform()
		{
			// Ground
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-20.0f, 0.0f), new Vec2(20.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			// Platform
			{
				BodyDef bd = new BodyDef();
				bd.Position = new Vec2(0.0f, 10.0f);
				Body body = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(3.0f, 0.5f);
				m_platform = body.CreateFixture(shape, 0.0f);

				m_top = 10.0f + 0.5f;
			}

			// Actor
			{
				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(0.0f, 12.0f);
				Body body = m_world.CreateBody(bd);

				m_radius = 0.5f;
				CircleShape shape = new CircleShape();
				shape.Radius = m_radius;
				m_character = body.CreateFixture(shape, 20.0f);

				body.LinearVelocity = new Vec2(0.0f, -50.0f);
			}
		}

		public override void PreSolve(Contact contact, Manifold oldManifold)
		{
			base.PreSolve(contact, oldManifold);

			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;

			if (fixtureA != m_platform && fixtureA != m_character)
				return;

			if (fixtureB != m_character && fixtureB != m_character)
				return;

			Vec2 position = m_character.Body.Position;

			if (position.Y < m_top + m_radius - 3.0f * Box2DSettings.b2_linearSlop)
				contact.Enabled = false;
		}

		float m_radius, m_top;
		Fixture m_platform;
		Fixture m_character;
	};
}
