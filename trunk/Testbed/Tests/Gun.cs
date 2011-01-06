using System;
using System.Collections.Generic;
using Box2CS;

namespace Testbed.Tests
{
	public class Gun : Test
	{
		MeshShape bulletShape = new MeshShape("bullet.bmesh", 8, true);

		void AddBullet(Vec2 pos, float angle)
		{
			Body bullet = m_world.CreateBody(new BodyDef(BodyType.Dynamic, pos, angle));
			bulletShape.AddToBody(bullet, 0.2f);
		}

		public Gun()
		{
			{
				Body ground = m_world.CreateBody(new BodyDef());
				ground.CreateFixture(new PolygonShape(new Vec2(-80, 0), new Vec2(80, 0)), 0);

				ground.CreateFixture(new PolygonShape(new Vec2(0, 15), new Vec2(-1.5f, 4)), 0);
				ground.CreateFixture(new PolygonShape(new Vec2(4.5f, 15), new Vec2(3.0f, 4)), 0);
				ground.CreateFixture(new PolygonShape(new Vec2(-1.5f, 4), new Vec2(3.0f, 4)), 0);

				{
					Body feeder = m_world.CreateBody(new BodyDef(BodyType.Dynamic, new Vec2(-0.05f, 3)));
					feeder.CreateFixture(new PolygonShape(new Vec2(-1.39f + 0.04f, 1.2f), new Vec2(-1.39f, 1), new Vec2(2.99f, 1), new Vec2(2.99f + 0.04f, 1.2f)), 0.5f);

					PrismaticJointDef pjd = new PrismaticJointDef();
					pjd.Initialize(feeder, ground, feeder.WorldCenter, (feeder.WorldCenter - new Vec2(2.25f, 15)).Normalized());
					pjd.CollideConnected = false;
					pjd.EnableLimit = true;
					pjd.LowerTranslation = 0;
					pjd.UpperTranslation = 15.0f - (.1f);
					m_world.CreateJoint(pjd);
				}
			}

			{
				const float addX = 0.16f;
				const float addY = 1.42f;
				Vec2 pos = new Vec2(6.90f, 8.45f);
				for (int i = 0; i < 9; ++i)
				{
					AddBullet(pos, (float)((Math.PI) + (Math.PI / 2)));
					pos += new Vec2(addX, addY);
				}
			}
		}
	}
}
