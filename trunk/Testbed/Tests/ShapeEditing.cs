using System;
using Box2CS;

namespace Testbed.Tests
{
	public class ShapeEditing : Test
	{
		public ShapeEditing()
		{
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			{
				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(0.0f, 10.0f);
				m_body = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(4.0f, 4.0f, new Vec2(0.0f, 0.0f), 0.0f);
				m_fixture1 = m_body.CreateFixture(shape, 10.0f);
				m_fixture2 = null;
			}
		}

		public override void  Keyboard(SFML.Window.KeyCode key)
		{
			switch (char.ToLower((char)key))
			{
			case 'c':
				if (m_fixture2 == null)
				{
					CircleShape shape = new CircleShape();
					shape.Radius = 3.0f;
					shape.Position = new Vec2(0.5f, -4.0f);
					m_fixture2 = m_body.CreateFixture(shape, 10.0f);
					m_body.IsAwake = true;
				}
				break;

			case 'd':
				if (m_fixture2 != null)
				{
					m_body.DestroyFixture(m_fixture2.Value);
					m_fixture2 = null;
					m_body.IsAwake = true;
				}
				break;
			}
		}


		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "Press: (c) create a shape, (d) destroy a shape.");
			m_textLine += 15;
		}

		Body m_body;
		Fixture? m_fixture1;
		Fixture? m_fixture2;
	};
}
