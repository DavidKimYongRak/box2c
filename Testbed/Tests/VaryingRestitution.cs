using System;
using Box2CS;

namespace Testbed.Tests
{
	public class VaryingRestitution : Test
	{
		public VaryingRestitution()
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
				shape.Radius = 1.0f;

				FixtureDef fd = new FixtureDef();
				fd.Shape = shape;
				fd.Density = 1.0f;

				float[] restitution = {0.0f, 0.1f, 0.3f, 0.5f, 0.75f, 0.9f, 1.0f};

				for (int i = 0; i < 7; ++i)
				{
					BodyDef bd = new BodyDef();
					bd.BodyType = BodyType.Dynamic; 
					bd.Position = new Vec2(-10.0f + 3.0f * i, 20.0f);

					Body body = m_world.CreateBody(bd);

					fd.Restitution = restitution[i];
					fd.UserData = i;
					body.CreateFixture(fd);
				}
			}		
		}
	};
}
