using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	/// This tests stacking. It also shows how to use World::Query
	/// and TestOverlap.

	/// This callback is called by World::QueryAABB. We find all the fixtures
	/// that overlap an AABB. Of those, we use TestOverlap to determine which fixtures
	/// overlap a circle. Up to 4 overlapped fixtures will be highlighted with a yellow border.
	public class PolyShapesCallback : QueryCallback
	{	
		public const int e_maxCount = 4

		PolyShapesCallback()
		{
			m_count = 0;
		}

		void DrawFixture(Fixture fixture)
		{
			ColorF color = new ColorF(0.95f, 0.95f, 0.6f);
			Transform xf = fixture.Body.Transform;

			switch (fixture.ShapeType)
			{
			case EShapeType.e_circle:
				{
					CircleShape circle = (CircleShape)fixture.Shape;

					Vec2 center = xf * circle.Position;
					float radius = circle.Radius;

					m_debugDraw.DrawCircle(center, radius, color);
				}
				break;

			case EShapeType.e_polygon:
				{
					PolygonShape poly = (PolygonShape)fixture.Shape;
					int vertexCount = poly.VertexCount;
					//Assert(vertexCount <= _maxPolygonVertices);
					Vec2[] vertices = new Vec2[Box2DSettings.b2_maxPolygonVertices];

					for (int i = 0; i < vertexCount; ++i)
						vertices[i] = xf * poly.Vertices[i];

					m_debugDraw.DrawPolygon(vertices, vertexCount, color);
				}
				break;
			}
		}

		/// Called for each fixture found in the query AABB.
		/// @return false to terminate the query.
		bool ReportFixture(Fixture fixture)
		{
			if (m_count == e_maxCount)
				return false;

			Body body = fixture.Body;
			Shape shape = fixture.Shape;

			bool overlap = TestOverlap(shape, &m_circle, body->GetTransform(), m_transform);

			if (overlap)
			{
				DrawFixture(fixture);
				++m_count;
			}

			return true;
		}

		CircleShape m_circle;
		Transform m_transform;
		DebugDraw m_debugDraw;
		int m_count;
	};

	public class PolyShapes : Test
	{
		public const int k_maxBodies = 256;

		PolyShapes()
		{
			// Ground body
			{
				BodyDef bd;
				Body* ground = m_world->CreateBody(&bd);

				PolygonShape shape;
				shape.SetAsEdge(Vec2(-40.0f, 0.0f), Vec2(40.0f, 0.0f));
				ground->CreateFixture(&shape, 0.0f);
			}

			{
				Vec2 vertices[3];
				vertices[0].Set(-0.5f, 0.0f);
				vertices[1].Set(0.5f, 0.0f);
				vertices[2].Set(0.0f, 1.5f);
				m_polygons[0].Set(vertices, 3);
			}
		
			{
				Vec2 vertices[3];
				vertices[0].Set(-0.1f, 0.0f);
				vertices[1].Set(0.1f, 0.0f);
				vertices[2].Set(0.0f, 1.5f);
				m_polygons[1].Set(vertices, 3);
			}

			{
				float32 w = 1.0f;
				float32 b = w / (2.0f + sqrtf(2.0f));
				float32 s = sqrtf(2.0f) * b;

				Vec2 vertices[8];
				vertices[0].Set(0.5f * s, 0.0f);
				vertices[1].Set(0.5f * w, b);
				vertices[2].Set(0.5f * w, b + s);
				vertices[3].Set(0.5f * s, w);
				vertices[4].Set(-0.5f * s, w);
				vertices[5].Set(-0.5f * w, b + s);
				vertices[6].Set(-0.5f * w, b);
				vertices[7].Set(-0.5f * s, 0.0f);

				m_polygons[2].Set(vertices, 8);
			}

			{
				m_polygons[3].SetAsBox(0.5f, 0.5f);
			}

			{
				m_circle.m_radius = 0.5f;
			}

			m_bodyIndex = 0;
			memset(m_bodies, 0, sizeof(m_bodies));
		}

		void Create(int index)
		{
			if (m_bodies[m_bodyIndex] != NULL)
			{
				m_world->DestroyBody(m_bodies[m_bodyIndex]);
				m_bodies[m_bodyIndex] = NULL;
			}

			BodyDef bd;
			bd.type = _dynamicBody;

			float32 x = RandomFloat(-2.0f, 2.0f);
			bd.position.Set(x, 10.0f);
			bd.angle = RandomFloat(-_pi, _pi);

			if (index == 4)
			{
				bd.angularDamping = 0.02f;
			}

			m_bodies[m_bodyIndex] = m_world->CreateBody(&bd);

			if (index < 4)
			{
				FixtureDef fd;
				fd.shape = m_polygons + index;
				fd.density = 1.0f;
				fd.friction = 0.3f;
				m_bodies[m_bodyIndex]->CreateFixture(&fd);
			}
			else
			{
				FixtureDef fd;
				fd.shape = &m_circle;
				fd.density = 1.0f;
				fd.friction = 0.3f;

				m_bodies[m_bodyIndex]->CreateFixture(&fd);
			}

			m_bodyIndex = (m_bodyIndex + 1) % k_maxBodies;
		}

		void DestroyBody()
		{
			for (int i = 0; i < k_maxBodies; ++i)
			{
				if (m_bodies[i] != NULL)
				{
					m_world->DestroyBody(m_bodies[i]);
					m_bodies[i] = NULL;
					return;
				}
			}
		}

		void Keyboard(unsigned char key)
		{
			switch (key)
			{
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
				Create(key - '1');
				break;

			case 'a':
				for (int i = 0; i < k_maxBodies; i += 2)
				{
					if (m_bodies[i])
					{
						bool active = m_bodies[i]->IsActive();
						m_bodies[i]->SetActive(!active);
					}
				}
				break;

			case 'd':
				DestroyBody();
				break;
			}
		}

		void Step(Settings* settings)
		{
			Test::Step(settings);

			PolyShapesCallback callback;
			callback.m_circle.m_radius = 2.0f;
			callback.m_circle.m_p.Set(0.0f, 2.1f);
			callback.m_transform.SetIdentity();
			callback.m_debugDraw = &m_debugDraw;

			AABB aabb;
			callback.m_circle.ComputeAABB(&aabb, callback.m_transform);

			m_world->QueryAABB(&callback, aabb);

			Color color(0.4f, 0.7f, 0.8f);
			m_debugDraw.DrawCircle(callback.m_circle.m_p, callback.m_circle.m_radius, color);

			m_debugDraw.DrawString(5, m_textLine, "Press 1-5 to drop stuff");
			m_textLine += 15;
			m_debugDraw.DrawString(5, m_textLine, "Press 'a' to (de)activate some bodies");
			m_textLine += 15;
			m_debugDraw.DrawString(5, m_textLine, "Press 'd' to destroy a body");
			m_textLine += 15;
		}

		static Test* Create()
		{
			return new PolyShapes;
		}

		int m_bodyIndex;
		Body* m_bodies[k_maxBodies];
		PolygonShape m_polygons[4];
		CircleShape m_circle;
	};
}
