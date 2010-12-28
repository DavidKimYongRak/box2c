using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	public class BodyTypes : Test
	{
		public static string Name
		{
			get { return "BodyTypes"; }
		}

		public BodyTypes()
		{
			Body ground = null;
			{
				BodyDef bd = new BodyDef();
				ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-20.0f, 0.0f), new Vec2(20.0f, 0.0f));

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;

				ground.CreateFixture(fd);
			}

			// Define attachment
			{
				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(0.0f, 3.0f);
				m_attachment = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.5f, 2.0f);
				m_attachment.CreateFixture(shape, 2.0f);
			}

			// Define platform
			{
				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(-4.0f, 5.0f);
				m_platform = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.5f, 4.0f, new Vec2(4.0f, 0.0f), 0.5f * (float)Math.PI);

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Friction = 0.6f;
				fd.Density = 2.0f;
				m_platform.CreateFixture(fd);

				RevoluteJointDef rjd = new RevoluteJointDef();
				rjd.Initialize(m_attachment, m_platform, new Vec2(0.0f, 5.0f));
				rjd.MaxMotorTorque = 50.0f;
				rjd.EnableMotor = true;
				m_world.CreateJoint(rjd);

				PrismaticJointDef pjd = new PrismaticJointDef();
				pjd.Initialize(ground, m_platform, new Vec2(0.0f, 5.0f), new Vec2(1.0f, 0.0f));

				pjd.MaxMotorForce = 1000.0f;
				pjd.EnableMotor = true;
				pjd.LowerTranslation = -10.0f;
				pjd.UpperTranslation = 10.0f;
				pjd.EnableLimit = true;

				m_world.CreateJoint(pjd);

				m_speed = 3.0f;
			}

			// Create a payload
			{
				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.Position = new Vec2(0.0f, 8.0f);
				m_payload = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.75f, 0.75f);

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Friction = 0.6f;
				fd.Density = 2.0f;

				m_payload.CreateFixture(fd);
			}
		}

		public override void  Keyboard(SFML.Window.KeyCode key)
		{
			switch (char.ToLower((char)key))
			{
			case 'd':
				m_platform.BodyType = BodyType.Dynamic;
				break;

			case 's':
				m_platform.BodyType = BodyType.Static;
				break;

			case 'k':
				m_platform.BodyType = BodyType.Kinematic;
				m_platform.LinearVelocity = new Vec2(-m_speed, 0.0f);
				m_platform.AngularVelocity = 0.0f;
				break;

			case 'n':
				m_payload.BodyType = (m_payload.BodyType == BodyType.Dynamic) ? BodyType.Static : BodyType.Dynamic;
				m_platform.IsAwake = true;
				break;
			}
		}

		public override void Step()
		{
			// Drive the kinematic body.
			if (m_platform.BodyType == BodyType.Kinematic)
			{
				Vec2 p = m_platform.Transform.Position;
				Vec2 v = m_platform.LinearVelocity;

				if ((p.X < -10.0f && v.X < 0.0f) ||
					(p.X > 10.0f && v.X > 0.0f))
				{
					v.X = -v.X;
					m_platform.LinearVelocity = v;
				}
			}

			base.Step();
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "Keys: (d) dynamic, (s) static, (k) kinematic");
			m_textLine += 15;
		}

		Body m_attachment;
		Body m_platform, m_payload;
		float m_speed;
	}
}
