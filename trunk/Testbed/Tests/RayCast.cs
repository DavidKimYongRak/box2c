using System;
using System.Collections.Generic;
using Box2CS;

namespace Testbed.Tests
{
	// This test demonstrates how to use the world ray-cast feature.
	// NOTE: we are intentionally filtering one of the polygons, therefore
	// the ray will always miss one type of polygon.

	// This callback finds the closest hit. Polygon 0 is filtered.
	public class RayCastClosestCallback : RayCastCallback
	{
		public RayCastClosestCallback()
		{
			m_hit = false;
		}

		public override float  ReportFixture(Fixture fixture, Vec2 point, Vec2 normal, float fraction)
		{
			Body body = fixture.Body;
			object userData = body.UserData;

			if (userData != null && userData is int)
			{
				int index = (int)userData;
				
				if (index == 0)
				{
					// filter
					return -1.0f;
				}
			}

			m_hit = true;
			m_point = point;
			m_normal = normal;
			return fraction;
		}
	
		public bool m_hit;
		public Vec2 m_point;
		public Vec2 m_normal;
	};

	// This callback finds any hit. Polygon 0 is filtered.
	public class RayCastAnyCallback : RayCastCallback
	{
		public RayCastAnyCallback()
		{
			m_hit = false;
		}

		public override float  ReportFixture(Fixture fixture, Vec2 point, Vec2 normal, float fraction)
		{
			Body body = fixture.Body;
			object userData = body.UserData;

			if (userData != null && userData is int)
			{
				int index = (int)userData;
				if (index == 0)
				{
					// filter
					return -1.0f;
				}
			}

			m_hit = true;
			m_point = point;
			m_normal = normal;
			return 0.0f;
		}

		public bool m_hit;
		public Vec2 m_point;
		public Vec2 m_normal;
	};

	// This ray cast collects multiple hits along the ray. Polygon 0 is filtered.
	public class RayCastMultipleCallback : RayCastCallback
	{
		public RayCastMultipleCallback()
		{
			m_count = 0;
		}

		public override float  ReportFixture(Fixture fixture, Vec2 point, Vec2 normal, float fraction)
		{
			Body body = fixture.Body;
			object userData = body.UserData;
			if (userData != null && userData is int)
			{
				int index = (int)userData;
				if (index == 0)
				{
					// filter
					return -1.0f;
				}
			}

			//Assert(m_count < e_maxCount);

			m_points.Add(point);
			m_normals.Add(normal);
			++m_count;

			// (m_count == e_maxCount)
			//{
			//	return 0.0f;
			//}

			return 1.0f;
		}

		public List<Vec2> m_points = new List<Vec2>();
		public List<Vec2> m_normals = new List<Vec2>();
		public int m_count;
	};


	public class RayCast : Test
	{
		public static string Name
		{
			get { return "RayCast"; }
		}

		public const int e_maxBodies = 256;

		public enum Mode
		{
			e_closest,
			e_any,
			e_multiple
		};

		public RayCast()
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
			m_angle = 0.0f;

			m_mode = Mode.e_closest;
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
			float y = Rand.RandomFloat(0.0f, 20.0f);
			bd.Position = new Vec2(x, y);
			bd.Angle = Rand.RandomFloat((float)-Math.PI, (float)Math.PI);

			bd.UserData = index;

			if (index == 4)
				bd.AngularDamping = 0.02f;

			m_bodies[m_bodyIndex] = m_world.CreateBody(bd);

			if (index < 4)
			{
				FixtureDef fd = new FixtureDef();
				fd.Shape = m_polygons[index];
				fd.Friction = 0.3f;
				m_bodies[m_bodyIndex].CreateFixture(fd);
			}
			else
			{
				FixtureDef fd = new FixtureDef();
				fd.Shape = m_circle;
				fd.Friction = 0.3f;

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

			case 'm':
				if (m_mode == Mode.e_closest)
					m_mode = Mode.e_any;
				else if (m_mode == Mode.e_any)
					m_mode = Mode.e_multiple;
				else if (m_mode == Mode.e_multiple)
					m_mode = Mode.e_closest;
				break;
			}
		}

		RayCastCallback _callback;
		public override void  Step()
		{
			bool advanceRay = TestSettings.pause == false || TestSettings.singleStep;

			base.Step();

			float L = 11.0f;
			Vec2 point1 = new Vec2(0.0f, 10.0f);
			Vec2 d = new Vec2(L * (float)Math.Cos(m_angle), L * (float)Math.Sin(m_angle));
			Vec2 point2 = point1 + d;

			if (m_mode == Mode.e_closest)
			{
				_callback = new RayCastClosestCallback();
				m_world.RayCast(_callback, point1, point2);
			}
			else if (m_mode == Mode.e_any)
			{
				_callback = new RayCastAnyCallback();
				m_world.RayCast(_callback, point1, point2);
			}
			else if (m_mode == Mode.e_multiple)
			{
				_callback = new RayCastMultipleCallback();
				m_world.RayCast(_callback, point1, point2);
			}

			if (advanceRay)
			{
				m_angle += (float)(0.25 * Math.PI / 180.0);
			}
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "Press 1-5 to place stuff, m to change the mode");
			m_textLine += 15;
			m_debugDraw.DrawString(5, m_textLine, "Mode = "+m_mode.ToString());
			m_textLine += 15;

			if (_callback == null)
				return;

			float L = 11.0f;
			Vec2 point1 = new Vec2(0.0f, 10.0f);
			Vec2 d = new Vec2(L * (float)Math.Cos(m_angle), L * (float)Math.Sin(m_angle));
			Vec2 point2 = point1 + d;

			if (_callback is RayCastClosestCallback)
			{
				RayCastClosestCallback callback = (RayCastClosestCallback)_callback;
				m_world.RayCast(_callback, point1, point2);

				if (callback.m_hit)
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
			else if (_callback is RayCastAnyCallback)
			{
				RayCastAnyCallback callback = (RayCastAnyCallback)_callback;
				_callback = new RayCastAnyCallback();

				if (callback.m_hit)
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
			else if (_callback is RayCastMultipleCallback)
			{
				RayCastMultipleCallback callback = (RayCastMultipleCallback)_callback;
				m_debugDraw.DrawSegment(point1, point2, new ColorF(0.8f, 0.8f, 0.8f));

				for (int i = 0; i < callback.m_count; ++i)
				{
					Vec2 p = callback.m_points[i];
					Vec2 n = callback.m_normals[i];
					m_debugDraw.DrawPoint(p, 5.0f, new ColorF(0.4f, 0.9f, 0.4f));
					m_debugDraw.DrawSegment(point1, p, new ColorF(0.8f, 0.8f, 0.8f));
					Vec2 head = p + 0.5f * n;
					m_debugDraw.DrawSegment(p, head, new ColorF(0.9f, 0.9f, 0.4f));
				}
			}
		}

		int m_bodyIndex;
		Body[] m_bodies = new Body[e_maxBodies];
		int[] m_userData = new int[e_maxBodies];
		PolygonShape[] m_polygons = new PolygonShape[4];
		CircleShape m_circle;

		float m_angle;

		Mode m_mode;
	};
}
