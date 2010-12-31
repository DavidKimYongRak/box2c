using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	// This callback finds the closest hit. Polygon 0 is filtered.
	public class RaycastClosestCallback : RayCastCallback
	{
		public RaycastClosestCallback()
		{
			m_hit = false;
		}

		public override float ReportFixture(Fixture fixture, Vec2 point, Vec2 normal, float fraction)
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

	public class LengthRaycast
	{
		float _totalLength;
		float _length;
		int _hit;
		Vec2 _start, _normal;
		World _world;
		public List<Vec2> starts = new List<Vec2>(), ends = new List<Vec2>();
		RaycastClosestCallback _callback = new RaycastClosestCallback();

		public void Start(World world, float totalLength, Vec2 start, Vec2 normal)
		{
			Clear();

			_hit = 0;
			_totalLength = totalLength;
			_length = totalLength;
			_start = start;
			_normal = normal;
			_world = world;

			// Send a beam out
			_callback.m_hit = false;
			_world.RayCast(_callback, start, start + (normal * _length));

			if (_callback.m_hit)
				Report(_callback.m_point, _callback.m_normal);
			else
			{
				starts.Add(_start);
				ends.Add(_start  + (_normal * _length));
			}
		}

		public void Clear()
		{
			starts.Clear();
			ends.Clear();
		}

		Vec2 Reflect(Vec2 start, Vec2 end, Vec2 normal)
		{
			var sub = (start - end).Normalized();

			return sub - 2 * (sub.Dot(normal)) * normal;
		}

		void Report(Vec2 point, Vec2 normal)
		{
			_hit++;
			Vec2 sub = _start - point;
			float len = sub.Length();

			if (_hit > 100 || _start == point || len == 0)
				return;

			starts.Add(_start);
			ends.Add(point);

			var reflect = Reflect(_start, point, normal);

			_length -= len;
			_start = point;
			_normal = normal;

			if (_length > 0)
			{
				_world.RayCast(_callback, _start, _start - (reflect * _length));

				if (!_callback.m_hit || (_start == _callback.m_point))
				{
					starts.Add(_start);
					ends.Add(_start - (reflect * _length));
				}

				Report(_callback.m_point, _callback.m_normal);
			}
		}
	}

	public class Mirrors : Test
	{
		LengthRaycast _ray = new LengthRaycast();
		public Mirrors()
		{
			m_world.Gravity = new Vec2(0.0f, 0.0f);
			{
				Body body = m_world.CreateBody(new BodyDef());
			}

			{
				float startY = 0;

				FrictionJointDef jd = new FrictionJointDef();
				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				PolygonShape shape = new PolygonShape(1.0f, 0.25f);

				for (int i = 0; i < 20; ++i)
				{
					bd.Position = new Vec2(0, startY);
					startY += 1.5f;

					FixtureDef fix = new FixtureDef(shape, 1.0f);

					var body = m_world.CreateBody(bd);
					var ff = body.CreateFixture(fix);
					float I = body.Inertia;
					float mass = body.Mass; float radius = (float)Math.Sqrt(2.0f * I / mass);

					jd.LocalAnchorA = jd.LocalAnchorB = Vec2.Empty;
					jd.BodyA = m_groundBody;
					jd.BodyB = body;
					jd.CollideConnected = true;
					jd.MaxForce = body.Mass * 10.0f;
					jd.MaxTorque = body.Mass * radius * 10.0f;
					m_world.CreateJoint(jd);
				}
			}
		}

		public override void Step()
		{
			base.Step();

			_ray.Start(m_world, 250, new Vec2(5, -10), new Vec2(0.5f, 0.5f));
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawPoint(new Vec2(5, -10), 4, new ColorF(1, 0, 0));

			for (int i = 0; i < _ray.starts.Count; ++i)
			{
				m_debugDraw.DrawSegment(_ray.starts[i], _ray.ends[i], new ColorF(1, 0, 0));
			}
		}
	}
}
