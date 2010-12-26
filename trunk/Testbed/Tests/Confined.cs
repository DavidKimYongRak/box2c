using System;
using Box2CS;

namespace Testbed.Tests
{
	public class Confined : Test
	{
		public Confined()
		{
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();

				// Floor
				shape.SetAsEdge(new Vec2(-10.0f, 0.0f), new Vec2(10.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);

				// Left wall
				shape.SetAsEdge(new Vec2(-10.0f, 0.0f), new Vec2(-10.0f, 20.0f));
				ground.CreateFixture(shape, 0.0f);

				// Right wall
				shape.SetAsEdge(new Vec2(10.0f, 0.0f), new Vec2(10.0f, 20.0f));
				ground.CreateFixture(shape, 0.0f);

				// Roof
				shape.SetAsEdge(new Vec2(-10.0f, 20.0f), new Vec2(10.0f, 20.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			m_world.Gravity = new Vec2(0.0f, 0.0f);
		}

		void CreateCircle()
		{
			float radius = 2.0f;
			CircleShape shape = new CircleShape();
			shape.Position = Vec2.Empty;
			shape.Radius = radius;

			FixtureDef fd = new FixtureDef();
			fd.Shape = shape;
			fd.Density = 1.0f;
			fd.Friction = 0.0f;

			Vec2 p = new Vec2(Rand.RandomFloat(), 3.0f + Rand.RandomFloat());
			BodyDef bd = new BodyDef();
			bd.BodyType = BodyType.Dynamic;
			bd.Position = p;
			//bd.allowSleep = false;
			Body body = m_world.CreateBody(bd);

			body.CreateFixture(fd);
		}

		public override void Keyboard(SFML.Window.KeyCode key)
		{
			switch (char.ToLower((char)key))
			{
			case 'c':
				CreateCircle();
				break;
			}
		}

		public override void Step()
		{
			base.Step();
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "Press 'c' to create a circle.");
			m_textLine += 15;
		}
	};
}
