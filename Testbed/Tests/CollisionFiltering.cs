using System;
using Box2CS;

namespace Testbed.Tests
{
	public class CollisionFiltering : Test
	{
		// This is a test of collision filtering.
		// There is a triangle, a box, and a circle.
		// There are 6 shapes. 3 large and 3 small.
		// The 3 small ones always collide.
		// The 3 large ones never collide.
		// The boxes don't collide with triangles (except if both are small).
		const int	k_smallGroup = 1;
		const int k_largeGroup = -1;

		const ushort k_defaultCategory = 0x0001;
		const ushort k_triangleCategory = 0x0002;
		const ushort k_boxCategory = 0x0004;
		const ushort k_circleCategory = 0x0008;

		const ushort k_triangleMask = 0xFFFF;
		const ushort k_boxMask = 0xFFFF ^ k_triangleCategory;
		const ushort k_circleMask = 0xFFFF;

		public CollisionFiltering()
		{
			// Ground body
			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));

				FixtureDef sd = new FixtureDef();
				sd.Shape = shape;
				sd.Friction = 0.3f;

				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);
				ground.CreateFixture(sd);
			}

			// Small triangle
			Vec2[] vertices = new Vec2[3];
			vertices[0] = new Vec2(-1.0f, 0.0f);
			vertices[1] = new Vec2(1.0f, 0.0f);
			vertices[2] = new Vec2(0.0f, 2.0f);

			PolygonShape polygon = new PolygonShape(vertices);

			FixtureDef triangleShapeDef = new FixtureDef();
			triangleShapeDef.Shape = polygon;
			triangleShapeDef.Density = 1.0f;

			triangleShapeDef.Filter = new FilterData(k_triangleCategory, k_triangleMask, k_smallGroup);

			BodyDef triangleBodyDef = new BodyDef();
			triangleBodyDef.BodyType = BodyType.Dynamic;
			triangleBodyDef.Position = new Vec2(-5.0f, 2.0f);

			Body body1 = m_world.CreateBody(triangleBodyDef);
			body1.CreateFixture(triangleShapeDef);

			// Large triangle (recycle definitions)
			vertices[0] *= 2.0f;
			vertices[1] *= 2.0f;
			vertices[2] *= 2.0f;
			polygon.Vertices = vertices;
			triangleShapeDef.Filter = new FilterData(triangleShapeDef.Filter.CategoryBits, triangleShapeDef.Filter.MaskBits, k_largeGroup);
			triangleBodyDef.Position = new Vec2(-5.0f, 6.0f);
			triangleBodyDef.FixedRotation = true; // look at me!

			Body body2 = m_world.CreateBody(triangleBodyDef);
			body2.CreateFixture(triangleShapeDef);

			{
				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(-5.0f, 10.0f);
				Body body = m_world.CreateBody(bd);

				PolygonShape p = new PolygonShape();
				p.SetAsBox(0.5f, 1.0f);
				body.CreateFixture(p, 1.0f);

				PrismaticJointDef jd = new PrismaticJointDef();
				jd.BodyA = body2;
				jd.BodyB = body;
				jd.EnableLimit = true;
				jd.LocalAnchorA = new Vec2(0.0f, 4.0f);
				jd.LocalAnchorB = Vec2.Empty;
				jd.LocalAxis = new Vec2(0.0f, 1.0f);
				jd.LowerTranslation = -1.0f;
				jd.UpperTranslation = 1.0f;

				m_world.CreateJoint(jd);
			}

			// Small box
			polygon.SetAsBox(1.0f, 0.5f);
			FixtureDef boxShapeDef = new FixtureDef();
			boxShapeDef.Shape = polygon;
			boxShapeDef.Density = 1.0f;
			boxShapeDef.Restitution = 0.1f;

			boxShapeDef.Filter = new FilterData(k_boxCategory, k_boxMask, k_smallGroup);

			BodyDef boxBodyDef = new BodyDef();
			boxBodyDef.BodyType = BodyType.Dynamic;
			boxBodyDef.Position = new Vec2(0.0f, 2.0f);

			Body body3 = m_world.CreateBody(boxBodyDef);
			body3.CreateFixture(boxShapeDef);

			// Large box (recycle definitions)
			polygon.SetAsBox(2.0f, 1.0f);
			boxShapeDef.Filter = new FilterData(k_boxCategory, k_boxMask, k_largeGroup);
			boxBodyDef.Position = new Vec2(0.0f, 6.0f);

			Body body4 = m_world.CreateBody(boxBodyDef);
			body4.CreateFixture(boxShapeDef);

			// Small circle
			CircleShape circle = new CircleShape();
			circle.Radius = 1.0f;

			FixtureDef circleShapeDef = new FixtureDef();
			circleShapeDef.Shape = circle;
			circleShapeDef.Density = 1.0f;

			circleShapeDef.Filter = new FilterData(k_circleCategory, k_circleMask, k_smallGroup);

			BodyDef circleBodyDef = new BodyDef();
			circleBodyDef.BodyType = BodyType.Dynamic;
			circleBodyDef.Position = new Vec2(5.0f, 2.0f);
		
			Body body5 = m_world.CreateBody(circleBodyDef);
			body5.CreateFixture(circleShapeDef);

			// Large circle
			circle.Radius *= 2.0f;
			circleShapeDef.Filter = new FilterData(k_circleCategory, k_circleMask, k_largeGroup);
			circleBodyDef.Position = new Vec2(5.0f, 6.0f);

			Body body6 = m_world.CreateBody(circleBodyDef);
			body6.CreateFixture(circleShapeDef);
		}
	};
}
