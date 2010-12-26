using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	public class ApplyForce : Test
	{
		public static string Name
		{
			get { return "ApplyForce"; }
		}

		public ApplyForce()
		{
			m_world.Gravity = new Vec2(0.0f, 0.0f);

			const float k_restitution = 0.4f;

			Body ground;
			{
				BodyDef bd = new BodyDef();
				bd.Position = new Vec2(0.0f, 20.0f);
				ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();

				FixtureDef sd = new FixtureDef();
				sd.Shape = shape;
				sd.Density = 0.0f;
				sd.Restitution = k_restitution;

				// Left vertical
				shape.SetAsEdge(new Vec2(-20.0f, -20.0f), new Vec2(-20.0f, 20.0f));
				ground.CreateFixture(sd);

				// Right vertical
				shape.SetAsEdge(new Vec2(20.0f, -20.0f), new Vec2(20.0f, 20.0f));
				ground.CreateFixture(sd);

				// Top horizontal
				shape.SetAsEdge(new Vec2(-20.0f, 20.0f), new Vec2(20.0f, 20.0f));
				ground.CreateFixture(sd);

				// Bottom horizontal
				shape.SetAsEdge(new Vec2(-20.0f, -20.0f), new Vec2(20.0f, -20.0f));
				ground.CreateFixture(sd);
			}

			{
				Transform xf1 = new Transform();
				xf1.R = new Mat22(0.3524f * (float)Math.PI);
				xf1.Position = (xf1.R * new Vec2(1.0f, 0.0f));

				Vec2[] vertices = new Vec2[3]
				{
					(xf1 * new Vec2(-1.0f, 0.0f)),
					(xf1 * new Vec2(1.0f, 0.0f)),
					(xf1 * new Vec2(0.0f, 0.5f))
				};
			
				PolygonShape poly1 = new PolygonShape(vertices);

				FixtureDef sd1 = new FixtureDef();
				sd1.Shape = poly1;
				sd1.Density = 4.0f;

				Transform xf2 = new Transform();
				xf2.R = new Mat22(-0.3524f * (float)Math.PI);
				xf2.Position = (xf2.R * new Vec2(-1.0f, 0.0f));

				vertices = new Vec2[] 
				{
					(xf2 * new Vec2(-1.0f, 0.0f)),
					(xf2 * new Vec2(1.0f, 0.0f)),
					(xf2 * new Vec2(0.0f, 0.5f))
				};

				PolygonShape poly2 = new PolygonShape(vertices);

				FixtureDef sd2 = new FixtureDef();
				sd2.Shape = poly2;
				sd2.Density = 2.0f;

				BodyDef bd = new BodyDef();
				bd.BodyType = BodyType.Dynamic;
				bd.AngularDamping = 5.0f;
				bd.LinearDamping = 0.1f;

				bd.Position = new Vec2(0.0f, 2.0f);
				bd.Angle = (float)Math.PI;
				bd.AllowSleep = false;
				m_body = m_world.CreateBody(bd);
				m_body.CreateFixture(sd1);
				m_body.CreateFixture(sd2);
			}

			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.5f, 0.5f);

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 1.0f;
				fd.Friction = 0.3f;

				for (int i = 0; i < 10; ++i)
				{
					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic;

					bd.Position = new Vec2(0.0f, 5.0f + 1.54f * i);
					Body body = m_world.CreateBody(bd);

					body.CreateFixture(fd);

					float gravity = 10.0f;
					float I = body.Inertia;
					float mass = body.Mass;

					// For a circle: I = 0.5 * m * r * r ==> r = sqrt(2 * I / m)
					float radius = (float)Math.Sqrt(2.0f * I / mass);

					FrictionJointDef jd = new FrictionJointDef();
					jd.LocalAnchorA = jd.LocalAnchorB = Vec2.Empty;
					jd.BodyA = ground;
					jd.BodyB = body;
					jd.CollideConnected = true;
					jd.MaxForce = mass * gravity;
					jd.MaxTorque = mass * radius * gravity;

					m_world.CreateJoint(jd);
				}
			}
		}

		public override void  Keyboard(SFML.Window.KeyCode key)
		{
			switch (char.ToLower((char)key))
			{
			case 'w':
				{
					Vec2 f = m_body.GetWorldVector(new Vec2(0.0f, -200.0f));
					Vec2 p = m_body.WorldCenter;
					m_body.ApplyForce(f, p);
				}
				break;

			case 'a':
				{
					m_body.ApplyTorque(50.0f);
				}
				break;

			case 'd':
				{
					m_body.ApplyTorque(-50.0f);
				}
				break;
			}
		}

		Body m_body;
	};
}
