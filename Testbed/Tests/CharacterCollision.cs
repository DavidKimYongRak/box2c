using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	public class CharacterCollision : Test
	{
		public CharacterCollision()
		{
			// Ground body
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-20.0f, 0.0f), new Vec2(20.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			// Collinear edges
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-8.0f, 1.0f), new Vec2(-6.0f, 1.0f));
				ground.CreateFixture(shape, 0.0f);
				shape.SetAsEdge(new Vec2(-6.0f, 1.0f), new Vec2(-4.0f, 1.0f));
				ground.CreateFixture(shape, 0.0f);
				shape.SetAsEdge(new Vec2(-4.0f, 1.0f), new Vec2(-2.0f, 1.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			// Square tiles
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(1.0f, 1.0f, new Vec2(4.0f, 3.0f), 0.0f);
				ground.CreateFixture(shape, 0.0f);
				shape.SetAsBox(1.0f, 1.0f, new Vec2(6.0f, 3.0f), 0.0f);
				ground.CreateFixture(shape, 0.0f);
				shape.SetAsBox(1.0f, 1.0f, new Vec2(8.0f, 3.0f), 0.0f);
				ground.CreateFixture(shape, 0.0f);
			}

			// Square made from edges notice how the edges are shrunk to account
			// for the polygon radius. This makes it so the square character does
			// not get snagged. However, ray casts can now go through the cracks.
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				float d = 2.0f * Box2DSettings.b2_polygonRadius;
				shape.SetAsEdge(new Vec2(-1.0f + d, 3.0f), new Vec2(1.0f - d, 3.0f));
				ground.CreateFixture(shape, 0.0f);
				shape.SetAsEdge(new Vec2(1.0f, 3.0f + d), new Vec2(1.0f, 5.0f - d));
				ground.CreateFixture(shape, 0.0f);
				shape.SetAsEdge(new Vec2(1.0f - d, 5.0f), new Vec2(-1.0f + d, 5.0f));
				ground.CreateFixture(shape, 0.0f);
				shape.SetAsEdge(new Vec2(-1.0f, 5.0f - d), new Vec2(-1.0f, 3.0f + d));
				ground.CreateFixture(shape, 0.0f);
			}

			// Square character
			{
				BodyDef bd = new BodyDef();
				bd.Position = new Vec2(-3.0f, 5.0f);
				bd.BodyType = BodyType.Dynamic;
				bd.FixedRotation = true;
				bd.AllowSleep = false;

				Body body = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.5f, 0.5f);

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 20.0f;
				body.CreateFixture(fd);
			}

			// Hexagon character
			{
				BodyDef bd = new BodyDef();
				bd.Position = new Vec2(-5.0f, 5.0f);
				bd.BodyType = BodyType.Dynamic;
				bd.FixedRotation = true;
				bd.AllowSleep = false;

				Body body = m_world.CreateBody(bd);

				float angle = 0.0f;
				float delta = (float)Math.PI / 3.0f;

				Vec2[] vertices = new Vec2[6];
				for (int i = 0; i < 6; ++i)
				{
					vertices[i] = new Vec2(0.5f * (float)Math.Cos(angle), 0.5f * (float)Math.Sin(angle));
					angle += delta;
				}

				PolygonShape shape = new PolygonShape();
				shape.Vertices = vertices;

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 20.0f;
				body.CreateFixture(fd);
			}

			// Circle character
			{
				BodyDef bd = new BodyDef();
				bd.Position = new Vec2(3.0f, 5.0f);
				bd.BodyType = BodyType.Dynamic;
				bd.FixedRotation = true;
				bd.AllowSleep = false;

				Body body = m_world.CreateBody(bd);

				CircleShape shape = new CircleShape();
				shape.Radius = 0.5f;

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 20.0f;
				body.CreateFixture(fd);
			}
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "This tests various character collision shapes");
			m_textLine += 15;
		}
	};
}
