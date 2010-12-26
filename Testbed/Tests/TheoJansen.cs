using System;
using Box2CS;

namespace Testbed.Tests
{
	public class TheoJansen : Test
	{
		void CreateLeg(float s, Vec2 wheelAnchor)
		{
			Vec2 p1 = new Vec2(5.4f * s, -6.1f);
			Vec2 p2 = new Vec2(7.2f * s, -1.2f);
			Vec2 p3 = new Vec2(4.3f * s, -1.9f);
			Vec2 p4 = new Vec2(3.1f * s, 0.8f);
			Vec2 p5 = new Vec2(6.0f * s, 1.5f);
			Vec2 p6 = new Vec2(2.5f * s, 3.7f);

			FixtureDef fd1 = new FixtureDef(), fd2 = new FixtureDef();
			fd1.Filter.GroupIndex = fd2.Filter.GroupIndex = -1;
			fd1.Density = 1.0f;
			fd2.Density = 1.0f;

			PolygonShape poly1 = new PolygonShape(), poly2 = new PolygonShape();

			if (s > 0.0f)
			{
				Vec2[] vertices = new Vec2[3];

				vertices[0] = p1;
				vertices[1] = p2;
				vertices[2] = p3;
				poly1.Vertices = vertices;

				vertices[0] = Vec2.Empty;
				vertices[1] = p5 - p4;
				vertices[2] = p6 - p4;
				poly2.Vertices = vertices;
			}
			else
			{
				Vec2[] vertices = new Vec2[3];

				vertices[0] = p1;
				vertices[1] = p3;
				vertices[2] = p2;
				poly1.Vertices = vertices;

				vertices[0] = Vec2.Empty;
				vertices[1] = p6 - p4;
				vertices[2] = p5 - p4;
				poly2.Vertices = vertices;
			}

			fd1.Shape = poly1;
			fd2.Shape = poly2;

			BodyDef bd1 = new BodyDef(), bd2 = new BodyDef();
			bd1.BodyType = BodyType.Dynamic;
			bd2.BodyType = BodyType.Dynamic;
			bd1.Position = m_offset;
			bd2.Position = p4 + m_offset;

			bd1.AngularDamping = 10.0f;
			bd2.AngularDamping = 10.0f;

			Body body1 = m_world.CreateBody(bd1);
			Body body2 = m_world.CreateBody(bd2);

			body1.CreateFixture(fd1);
			body2.CreateFixture(fd2);

			DistanceJointDef djd = new DistanceJointDef();

			// Using a soft distance constraint can reduce some jitter.
			// It also makes the structure seem a bit more fluid by
			// acting like a suspension system.
			djd.DampingRatio = 0.5f;
			djd.FrequencyHz = 10.0f;

			djd.Initialize(body1, body2, p2 + m_offset, p5 + m_offset);
			m_world.CreateJoint(djd);

			djd.Initialize(body1, body2, p3 + m_offset, p4 + m_offset);
			m_world.CreateJoint(djd);

			djd.Initialize(body1, m_wheel, p3 + m_offset, wheelAnchor + m_offset);
			m_world.CreateJoint(djd);

			djd.Initialize(body2, m_wheel, p6 + m_offset, wheelAnchor + m_offset);
			m_world.CreateJoint(djd);

			RevoluteJointDef rjd = new RevoluteJointDef();

			rjd.Initialize(body2, m_chassis, p4 + m_offset);
			m_world.CreateJoint(rjd);
		}

		public TheoJansen()
		{
			m_offset = new Vec2(0.0f, 8.0f);
			m_motorSpeed = 2.0f;
			m_motorOn = true;
			Vec2 pivot = new Vec2(0.0f, 0.8f);

			// Ground
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-50.0f, 0.0f), new Vec2(50.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);

				shape.SetAsEdge(new Vec2(-50.0f, 0.0f), new Vec2(-50.0f, 10.0f));
				ground.CreateFixture(shape, 0.0f);

				shape.SetAsEdge(new Vec2(50.0f, 0.0f), new Vec2(50.0f, 10.0f));
				ground.CreateFixture(shape, 0.0f);

				const float groundHeight = 0.19f;
				const float groundHeight2 = groundHeight * 2;

				shape.SetAsBox(10, groundHeight, new Vec2(-14, (groundHeight / 2)), 0);
				ground.CreateFixture(shape, 0.0f);

				shape.SetAsBox(8, groundHeight, new Vec2(-14, (groundHeight / 2) + groundHeight2), 0);
				ground.CreateFixture(shape, 0.0f);

				shape.SetAsBox(6, groundHeight, new Vec2(-14, (groundHeight / 2) + groundHeight2 + groundHeight2), 0);
				ground.CreateFixture(shape, 0.0f);

				shape.SetAsBox(4, groundHeight, new Vec2(-14, (groundHeight / 2) + groundHeight2 + groundHeight2 + groundHeight2), 0);
				ground.CreateFixture(shape, 0.0f);

				shape.SetAsBox(2, groundHeight, new Vec2(-14, (groundHeight / 2) + groundHeight2 + groundHeight2 + groundHeight2 + groundHeight2), 0);
				ground.CreateFixture(shape, 0.0f);
			}

			// Balls
			for (int i = 0; i < 25; ++i)
			{
				CircleShape shape = new CircleShape();
				shape.Radius = 0.25f;

				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(-40.0f + 2.0f * i, 3f);

				Body body = m_world.CreateBody(bd);
				body.CreateFixture(shape, 1.0f);
			}

			// Chassis
			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(2.5f, 1.0f);

				FixtureDef sd = new FixtureDef();
				sd.Density = 1.0f;
				sd.Shape = shape;
				sd.Filter.GroupIndex = -1;
				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = pivot + m_offset;
				m_chassis = m_world.CreateBody(bd);
				m_chassis.CreateFixture(sd);
			}

			{
				CircleShape shape = new CircleShape();
				shape.Radius = 1.6f;

				FixtureDef sd = new FixtureDef();
				sd.Density = 1.0f;
				sd.Shape = shape;
				sd.Filter.GroupIndex = -1;
				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = pivot + m_offset;
				m_wheel = m_world.CreateBody(bd);
				m_wheel.CreateFixture(sd);
			}

			{
				RevoluteJointDef jd = new RevoluteJointDef();
				jd.Initialize(m_wheel, m_chassis, pivot + m_offset);
				jd.CollideConnected = false;
				jd.MotorSpeed = m_motorSpeed;
				jd.MaxMotorTorque = 400.0f;
				jd.EnableMotor = m_motorOn;
				m_motorJoint = (RevoluteJoint)m_world.CreateJoint(jd);
			}

			Vec2 wheelAnchor = pivot + new Vec2(0.0f, -0.8f);

			CreateLeg(-1.0f, wheelAnchor);
			CreateLeg(1.0f, wheelAnchor);

			m_wheel.SetTransform(m_wheel.Position, 120.0f * (float)Math.PI / 180.0f);
			CreateLeg(-1.0f, wheelAnchor);
			CreateLeg(1.0f, wheelAnchor);

			m_wheel.SetTransform(m_wheel.Position, -120.0f * (float)Math.PI / 180.0f);
			CreateLeg(-1.0f, wheelAnchor);
			CreateLeg(1.0f, wheelAnchor);
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "Keys: left = a, brake = s, right = d, toggle motor = m");
			m_textLine += 15;
		}

		public override void  Keyboard(SFML.Window.KeyCode key)
		{
			switch (char.ToLower((char)key))
			{
			case 'a':
				m_motorJoint.MotorSpeed = -m_motorSpeed;
				break;

			case 's':
				m_motorJoint.MotorSpeed = 0.0f;
				break;

			case 'd':
				m_motorJoint.MotorSpeed = m_motorSpeed;
				break;

			case 'm':
				m_motorJoint.IsMotorEnabled = !m_motorJoint.IsMotorEnabled;
				break;
			}
		}

		Vec2 m_offset;
		Body m_chassis;
		Body m_wheel;
		RevoluteJoint m_motorJoint;
		bool m_motorOn;
		float m_motorSpeed;
	};
}
