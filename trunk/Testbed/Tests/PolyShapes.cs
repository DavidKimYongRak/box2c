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
		public const int e_maxCount = 4;

		public PolyShapesCallback()
		{
		}

		static Vec2[] drawingVertices = new Vec2[Box2DSettings.b2_maxPolygonVertices];
		public void DrawFixture(Fixture fixture)
		{
			ColorF color = new ColorF(0.95f, 0.95f, 0.6f);
			Transform xf = fixture.Body.Transform;

			switch (fixture.ShapeType)
			{
			case ShapeType.Circle:
				{
					CircleShape circle = (CircleShape)fixture.Shape;

					Vec2 center = xf * circle.Position;
					float radius = circle.Radius;

					m_debugDraw.DrawCircle(center, radius, color);
				}
				break;

			case ShapeType.Polygon:
				{
					PolygonShape poly = (PolygonShape)fixture.Shape;
					int vertexCount = poly.VertexCount;
					//Assert(vertexCount <= _maxPolygonVertices);

					for (int i = 0; i < vertexCount; ++i)
						drawingVertices[i] = xf * poly.Vertices[i];

					m_debugDraw.DrawPolygon(drawingVertices, vertexCount, color);
				}
				break;
			}
		}

		/// Called for each fixture found in the query AABB.
		/// @return false to terminate the query.
		public override bool ReportFixture(Fixture fixture)
		{
			if (aabbs.Count == e_maxCount)
				return false;

			Body body = fixture.Body;
			Shape shape = fixture.Shape;

			if (body.BodyType == BodyType.Static)
				return false;

			bool overlap = Box2D.TestOverlap(shape, m_circle, body.Transform, m_transform);

			if (overlap)
			{
				//DrawFixture(fixture);
				//m_debugDraw.DrawAABB(fixture.AABB, new ColorF(1, 0, 0));
				aabbs.Add(fixture.AABB);
			}

			return true;
		}

		public CircleShape m_circle;
		public Transform m_transform;
		public TestDebugDraw m_debugDraw;
		public List<AABB> aabbs = new List<AABB>();
	};

	public class PolyShapes : Test
	{
		public const int k_maxBodies = 256;
		
		public static string Name
		{
			get { return "PolyShapes"; }
		}

		public PolyShapes()
		{
			// Ground body
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			{
				Vec2[] vertices = new Vec2[3];
				vertices[0] = new Vec2(-0.5f, 0.0f);
				vertices[1] = new Vec2(0.5f, 0.0f);
				vertices[2] = new Vec2(0.0f, 1.5f);
				m_polygons[0] = new PolygonShape();
				m_polygons[0].Vertices = vertices;
			}
		
			{
				Vec2[] vertices = new Vec2[3];
				vertices[0] = new Vec2(-0.1f, 0.0f);
				vertices[1] = new Vec2(0.1f, 0.0f);
				vertices[2] = new Vec2(0.0f, 1.5f);
				m_polygons[1] = new PolygonShape();
				m_polygons[1].Vertices = vertices;
			}

			{
				float w = 1.0f;
				float b = w / (2.0f + (float)Math.Sqrt(2.0f));
				float s = (float)Math.Sqrt(2.0f) * b;

				Vec2[] vertices = new Vec2[8];
				vertices[0] = new Vec2(0.5f * s, 0.0f);
				vertices[1] = new Vec2(0.5f * w, b);
				vertices[2] = new Vec2(0.5f * w, b + s);
				vertices[3] = new Vec2(0.5f * s, w);
				vertices[4] = new Vec2(-0.5f * s, w);
				vertices[5] = new Vec2(-0.5f * w, b + s);
				vertices[6] = new Vec2(-0.5f * w, b);
				vertices[7] = new Vec2(-0.5f * s, 0.0f);

				m_polygons[2] = new PolygonShape();
				m_polygons[2].Vertices = vertices;
			}

			{
				m_polygons[3] = new PolygonShape();
				m_polygons[3].SetAsBox(0.5f, 0.5f);
			}

			{
				m_circle = new CircleShape();
				m_circle.Radius = 0.5f;
			}

			m_bodyIndex = 0;
		}

		void Create(int index)
		{
			if (m_bodies[m_bodyIndex] != null)
			{
				m_world.DestroyBody(m_bodies[m_bodyIndex]);
				m_bodies[m_bodyIndex] = null;
			}

			BodyDef bd = new BodyDef();
			bd.BodyType = BodyType.Dynamic;

			float x = Rand.RandomFloat(-2.0f, 2.0f);
			bd.Position = new Vec2(x, 10.0f);
			bd.Angle = Rand.RandomFloat((float)-Math.PI, (float)Math.PI);

			if (index == 4)
			{
				bd.AngularDamping = 0.02f;
			}

			m_bodies[m_bodyIndex] = m_world.CreateBody(bd);

			if (index < 4)
			{
				FixtureDef fd = new FixtureDef();
				fd.Shape = m_polygons[index];
				fd.Density = 1.0f;
				fd.Friction = 0.3f;
				m_bodies[m_bodyIndex].CreateFixture(fd);
			}
			else
			{
				FixtureDef fd = new FixtureDef();
				fd.Shape = m_circle;
				fd.Density = 1.0f;
				fd.Friction = 0.3f;

				m_bodies[m_bodyIndex].CreateFixture(fd);
			}

			m_bodyIndex = (m_bodyIndex + 1) % k_maxBodies;
		}

		void DestroyBody()
		{
			for (int i = 0; i < k_maxBodies; ++i)
			{
				if (m_bodies[i] != null)
				{
					m_world.DestroyBody(m_bodies[i]);
					m_bodies[i] = null;
					return;
				}
			}
		}

		public override void Keyboard(SFML.Window.KeyCode key)
		{
			switch ((char.ToLower((char)key)))
			{
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
				Create((int)(key - '1'));
				break;

			case 'a':
				for (int i = 0; i < k_maxBodies; i += 2)
				{
					if (m_bodies[i] != null)
					{
						bool active = m_bodies[i].IsActive;
						m_bodies[i].IsActive = !active;
					}
				}
				break;

			case 'd':
				DestroyBody();
				break;
			}
		}

		PolyShapesCallback callback = new PolyShapesCallback();
		public override void Step()
		{
			base.Step();

			callback.m_circle = new CircleShape();
			callback.m_circle.Radius = 2.0f;
			callback.m_circle.Position = new Vec2(0.0f, 2.1f);
			callback.m_transform = Transform.Identity;
			callback.m_debugDraw = m_debugDraw;
			callback.aabbs.Clear();

			AABB aabb;
			callback.m_circle.ComputeAABB(out aabb, callback.m_transform);
			
			m_world.QueryAABB(callback, aabb);
		}

		public override void Draw()
		{
			base.Draw();

			if (callback.m_circle == null)
				return;

			ColorF color = new ColorF(0.4f, 0.7f, 0.8f);
			m_debugDraw.DrawCircle(callback.m_circle.Position, callback.m_circle.Radius, color);

			AABB aabb;
			callback.m_circle.ComputeAABB(out aabb, callback.m_transform);

			m_debugDraw.DrawAABB(aabb, new ColorF(0.6f, 0.3f, 0.4f));

			foreach (var ab in callback.aabbs)
				m_debugDraw.DrawAABB(ab, new ColorF(1, 0, 0));

			m_debugDraw.DrawString(5, m_textLine, "Press 1-5 to drop stuff");
			m_textLine += 15;
			m_debugDraw.DrawString(5, m_textLine, "Press 'a' to (de)activate some bodies");
			m_textLine += 15;
			m_debugDraw.DrawString(5, m_textLine, "Press 'd' to destroy a body");
			m_textLine += 15;
		}

		int m_bodyIndex;
		Body[] m_bodies = new Body[k_maxBodies];
		PolygonShape[] m_polygons = new PolygonShape[4];
		CircleShape m_circle;
	};
}
