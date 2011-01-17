using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	class EdgeShapesCallback : RayCastCallback
	{
		public EdgeShapesCallback()
		{
			m_fixture = null;
		}

		public override float ReportFixture(Fixture fixture,  Vec2 point,
			 Vec2 normal, float fraction)
		{
			m_fixture = fixture;
			m_point = point;
			m_normal = normal;

			return fraction;
		}

		public Fixture? m_fixture;
		public Vec2 m_point;
		public Vec2 m_normal;
	};

	public class EdgeShapes : Test
	{
		public static string Name
		{
			get { return "Edge Shapes"; }
		}

		public const int e_maxBodies = 256;

		public EdgeShapes()
		{
			// Ground body
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				float x1 = -20.0f;
				float y1 = 2.0f * (float)Math.Cos(x1 / 10.0f * Math.PI);
				for (int i = 0; i < 80; ++i)
				{
					float x2 = x1 + 0.5f;
					float y2 = 2.0f * (float)Math.Cos(x2 / 10.0f * Math.PI);

					PolygonShape shape = new PolygonShape();
					shape.SetAsEdge(new Vec2(x1, y1), new Vec2(x2, y2));
					ground.CreateFixture(shape, 0.0f);

					x1 = x2;
					y1 = y2;
				}
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
			m_angle = 0.0f;
		}

		void Create(int index)
		{
			if (m_bodies[m_bodyIndex] != null)
			{
				m_world.DestroyBody(m_bodies[m_bodyIndex]);
				m_bodies[m_bodyIndex] = null;
			}

			BodyDef bd = new BodyDef();

			float x = Rand.RandomFloat(-10.0f, 10.0f);
			float y = Rand.RandomFloat(10.0f, 20.0f);
			bd.Position = new Vec2(x, y);
			bd.Angle = Rand.RandomFloat((float)-Math.PI, (float)Math.PI);
			bd.BodyType = BodyType.Dynamic;

			if (index == 4)
				bd.AngularDamping = 0.02f;

			m_bodies[m_bodyIndex] = m_world.CreateBody(bd);

			if (index < 4)
			{
				FixtureDef fd = new FixtureDef();
				fd.Shape = m_polygons[index];
				fd.Friction = 0.3f;
				fd.Density = 20.0f;
				m_bodies[m_bodyIndex].CreateFixture(fd);
			}
			else
			{
				FixtureDef fd = new FixtureDef();
				fd.Shape = m_circle;
				fd.Friction = 0.3f;
				fd.Density = 20.0f;
				m_bodies[m_bodyIndex].CreateFixture(fd);
			}

			m_bodyIndex = (m_bodyIndex + 1) % e_maxBodies;
		}

		void DestroyBody()
		{
			for (int i = 0; i < e_maxBodies; ++i)
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
			switch (char.ToLower((char)key))
			{
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
				Create((int)(key - '1'));
				break;

			case 'd':
				DestroyBody();
				break;
			}
		}

		EdgeShapesCallback callback = new EdgeShapesCallback();

		public override void  Step()
		{
			bool advanceRay = TestSettings.pause == false || TestSettings.singleStep;

			base.Step();

			float L = 25.0f;
			Vec2 point1 = new Vec2(0.0f, 10.0f);
			Vec2 d = new Vec2((float)(L * Math.Cos(m_angle)), (float)(-L * Math.Abs(Math.Sin(m_angle))));
			Vec2 point2 = point1 + d;

			m_world.RayCast(callback, point1, point2);

			if (advanceRay)
			{
				m_angle += (float)(0.25f * Math.PI / 180.0f);
			}
		}

		public override void Draw()
		{
			base.Draw();

			float L = 25.0f;
			Vec2 point1 = new Vec2(0.0f, 10.0f);
			Vec2 d = new Vec2((float)(L * Math.Cos(m_angle)), (float)(-L * Math.Abs(Math.Sin(m_angle))));
			Vec2 point2 = point1 + d;

			m_debugDraw.DrawString(5, m_textLine, "Press 1-5 to drop stuff");
			m_textLine += 15;

			if (callback.m_fixture != null)
			{
				m_debugDraw.DrawPoint(callback.m_point, 5.0f, new ColorF(0.4f, 0.9f, 0.4f));

				m_debugDraw.DrawSegment(point1, callback.m_point, new ColorF(0.8f, 0.8f, 0.8f));

				Vec2 head = callback.m_point + 0.5f * callback.m_normal;
				m_debugDraw.DrawSegment(callback.m_point, head, new ColorF(0.9f, 0.9f, 0.4f));
			}
			else
			{
				m_debugDraw.DrawSegment(point1, point2, new ColorF(0.8f, 0.8f, 0.8f));
			}
		}

		int m_bodyIndex;
		Body[] m_bodies = new Body[e_maxBodies];
		PolygonShape[] m_polygons = new PolygonShape[4];
		CircleShape m_circle;

		float m_angle;
	};
}
