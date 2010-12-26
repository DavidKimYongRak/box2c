using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	public class SphereStack : Test
	{
		public static string Name
		{
			get { return "SphereStack"; }
		}

		public const int e_count = 10;

		public SphereStack()
		{
			{
				BodyDef bd = new BodyDef();
				Body ground = m_world.CreateBody(bd);

				PolygonShape shape = new PolygonShape();
				shape.SetAsEdge(new Vec2(-40.0f, 0.0f), new Vec2(40.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			{
				CircleShape shape = new CircleShape();

				for (int i = 0; i < e_count; ++i)
				{
					shape.Radius = Rand.RandomFloat(0.5f, 0.5f);
					BodyDef bd = new BodyDef();
					bd.BodyType = EBodyType.b2_dynamicBody;
					bd.Position = new Vec2(0.0f, 4.0f + 3.0f * i);

					m_bodies[i] = m_world.CreateBody(bd);

					m_bodies[i].CreateFixture(shape, 1.0f);
				}
			}
		}

		public override void  Step()
		{
			base.Step();

			//for (int32 i = 0; i < e_count; ++i)
			//{
			//	printf("%g ", m_bodies[i].GetWorldCenter().y);
			//}

			//for (int32 i = 0; i < e_count; ++i)
			//{
			//	printf("%g ", m_bodies[i].GetLinearVelocity().y);
			//}

			//printf("\n");
		}

		Body[] m_bodies = new Body[e_count];
	};

}
