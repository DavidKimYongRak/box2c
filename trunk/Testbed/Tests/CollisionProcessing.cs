using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	// This test shows collision processing and tests
	// deferred body destruction.
	public class CollisionProcessing : Test
	{
		public CollisionProcessing()
		{
			// Ground body
			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-50.0f, 0.0f), new Vec2(50.0f, 0.0f));

				FixtureDef sd = new FixtureDef();
				sd.Shape = shape;

				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);
				ground.CreateFixture(sd);
			}

			float xLo = -5.0f, xHi = 5.0f;
			float yLo = 2.0f, yHi = 35.0f;

			// Small triangle
			Vec2[] vertices = new Vec2[3];
			vertices[0] = new Vec2(-1.0f, 0.0f);
			vertices[1] = new Vec2(1.0f, 0.0f);
			vertices[2] = new Vec2(0.0f, 2.0f);

			PolygonShape polygon = new PolygonShape(vertices);

			FixtureDef triangleShapeDef = new FixtureDef();
			triangleShapeDef.Shape = polygon;
			triangleShapeDef.Density = 1.0f;

			BodyDef triangleBodyDef = new BodyDef();
			triangleBodyDef.BodyType = BodyType.Dynamic;
			triangleBodyDef.Position = new Vec2(Rand.RandomFloat(xLo, xHi), Rand.RandomFloat(yLo, yHi));

			Body body1 = m_world.CreateBody(triangleBodyDef);
			body1.CreateFixture(triangleShapeDef);

			// Large triangle (recycle definitions)
			vertices[0] *= 2.0f;
			vertices[1] *= 2.0f;
			vertices[2] *= 2.0f;
			polygon.Vertices = vertices;

			triangleBodyDef.Position = new Vec2(Rand.RandomFloat(xLo, xHi), Rand.RandomFloat(yLo, yHi));

			Body body2 = m_world.CreateBody(triangleBodyDef);
			body2.CreateFixture(triangleShapeDef);
		
			// Small box
			polygon.SetAsBox(1.0f, 0.5f);

			FixtureDef boxShapeDef = new FixtureDef();
			boxShapeDef.Shape = polygon;
			boxShapeDef.Density = 1.0f;

			BodyDef boxBodyDef = new BodyDef();
			boxBodyDef.BodyType = BodyType.Dynamic;
			boxBodyDef.Position = new Vec2(Rand.RandomFloat(xLo, xHi), Rand.RandomFloat(yLo, yHi));

			Body body3 = m_world.CreateBody(boxBodyDef);
			body3.CreateFixture(boxShapeDef);

			// Large box (recycle definitions)
			polygon.SetAsBox(2.0f, 1.0f);
			boxBodyDef.Position = new Vec2(Rand.RandomFloat(xLo, xHi), Rand.RandomFloat(yLo, yHi));
		
			Body body4 = m_world.CreateBody(boxBodyDef);
			body4.CreateFixture(boxShapeDef);

			// Small circle
			CircleShape circle = new CircleShape();
			circle.Radius = 1.0f;

			FixtureDef circleShapeDef = new FixtureDef();
			circleShapeDef.Shape = circle;
			circleShapeDef.Density = 1.0f;

			BodyDef circleBodyDef = new BodyDef();
			circleBodyDef.BodyType = BodyType.Dynamic;
			circleBodyDef.Position = new Vec2(Rand.RandomFloat(xLo, xHi), Rand.RandomFloat(yLo, yHi));

			Body body5 = m_world.CreateBody(circleBodyDef);
			body5.CreateFixture(circleShapeDef);

			// Large circle
			circle.Radius *= 2.0f;
			circleBodyDef.Position = new Vec2(Rand.RandomFloat(xLo, xHi), Rand.RandomFloat(yLo, yHi));

			Body body6 = m_world.CreateBody(circleBodyDef);
			body6.CreateFixture(circleShapeDef);
		}

		public override void  Step()
		{
			base.Step();

			// We are going to destroy some bodies according to contact
			// points. We must buffer the bodies that should be destroyed
			// because they may belong to multiple contact points.
			List<Body> nuke = new List<Body>();

			// Traverse the contact results. Destroy bodies that
			// are touching heavier bodies.
			for (int i = 0; i < m_pointCount; ++i)
			{
				ContactPoint point = m_points[i];

				Body body1 = point.fixtureA.Body;
				Body body2 = point.fixtureB.Body;
				float mass1 = body1.Mass;
				float mass2 = body2.Mass;

				if (mass1 > 0.0f && mass2 > 0.0f)
				{
					if (mass2 > mass1)
						nuke.Add(body1);
					else
						nuke.Add(body2);
				}
			}

			// Sort the nuke array to group duplicates.
			var nukeB = nuke.Distinct<Body>();

			// Destroy the bodies, skipping duplicates.
			foreach (var b in nukeB)
			{
				if (b != m_bomb)
					m_world.DestroyBody(b);
			}
		}
	};
}
