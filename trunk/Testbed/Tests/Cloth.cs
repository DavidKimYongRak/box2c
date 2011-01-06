using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	public class Cloth : Test
	{
		const float ClothBodySize = 0.22f;
		const int ClothSegmentsWidth = 18;
		const int ClothSegmentsHeight = 25;
		const float ClothBodySpacingWidth = ClothBodySize * 2;
		const float ClothTotalWidth = (ClothBodySpacingWidth * ClothSegmentsWidth);
		Body[,] cloth = new Body[ClothSegmentsWidth, ClothSegmentsHeight];

		public Cloth()
		{
			FixtureDef boxFix = new FixtureDef(new CircleShape(ClothBodySize), 0.2f);
			BodyDef boxBod = new BodyDef(BodyType.Dynamic, Vec2.Empty);

			boxFix.Filter.GroupIndex = -1;
			boxBod.Position = new Vec2(-ClothTotalWidth / 2, 30);

			Body bar;
			{
				bar = m_world.CreateBody(new BodyDef(BodyType.Static, new Vec2(-ClothBodySpacingWidth / 2, 30)));

				var fd = new FixtureDef(new PolygonShape((ClothTotalWidth / 2) + ClothBodySpacingWidth, 0.25f));
				fd.Filter.GroupIndex = -1;
				bar.CreateFixture(fd);
			}

			for (int y = 0; y < ClothSegmentsHeight; ++y)
			{
				for (int x = 0; x < ClothSegmentsWidth; ++x)
				{
					Body body = m_world.CreateBody(boxBod);
					boxBod.Position += new Vec2(ClothBodySpacingWidth, 0);

					body.CreateFixture(boxFix);

					if (y == 0)
					{
						WeldJointDef wjd = new WeldJointDef();
						wjd.Initialize(body, bar, body.WorldCenter);
						m_world.CreateJoint(wjd);
					}

					cloth[x, y] = body;
				}

				boxBod.Position = new Vec2(-ClothTotalWidth / 2, boxBod.Position.Y - ClothBodySpacingWidth);
			}

			for (int y = 0; y < ClothSegmentsHeight; ++y)
			{
				for (int x = 0; x < ClothSegmentsWidth; ++x)
				{
					Body leftBody, rightBody;

					DistanceJointDef djd = new DistanceJointDef();
					djd.FrequencyHz = 15 + Rand.RandomFloat(0, 6);
					djd.DampingRatio = 0.11f + Rand.RandomFloat(0.01f, 0.15f);

					// connect to right
					if (x != ClothSegmentsWidth - 1)
					{
						leftBody = cloth[x, y];
						rightBody = cloth[x + 1, y];

						djd.Initialize(leftBody, rightBody, leftBody.WorldCenter, rightBody.WorldCenter);
						m_world.CreateJoint(djd);
					}

					// connect to up
					if (y != 0)
					{
						leftBody = cloth[x, y];
						rightBody = cloth[x, y - 1];

						djd.Initialize(leftBody, rightBody, leftBody.WorldCenter, rightBody.WorldCenter);
						m_world.CreateJoint(djd);
					}
				}
			}
		}

		bool _wind = false;

		public override void Step()
		{
			base.Step();

			if (_wind)
			for (int x = 0; x < ClothSegmentsWidth; ++x)
			{
				cloth[x, ClothSegmentsHeight - 1].LinearVelocity += new Vec2(Rand.RandomFloat(1, 1), Rand.RandomFloat(1, 1));
			}

			for (int y = 0; y < ClothSegmentsHeight; ++y)
			{
				for (int x = 0; x < ClothSegmentsWidth; ++x)
				{
					if (y != 0)
					{
						var l = cloth[x, y];
						var u = cloth[x, y - 1];

						if ((l.WorldCenter - u.WorldCenter).Length() > ClothBodySpacingWidth * 1.45f)
						{
							// find the up joint connecting these two
							Joint joint = null;

							foreach (var j in l.Joints)
							{
								if ((j.Joint.BodyA == l && j.Joint.BodyB == u) || (j.Joint.BodyA == u && j.Joint.BodyB == l))
								{
									joint = j.Joint;
									break;
								}
							}

							if (joint != null && (joint is DistanceJoint))
								m_world.DestroyJoint(joint);
						}
					}

					if (x != ClothSegmentsWidth - 1)
					{
						var l = cloth[x, y];
						var u = cloth[x + 1, y];

						if ((l.WorldCenter - u.WorldCenter).Length() > ClothBodySpacingWidth * 1.45f)
						{
							// find the up joint connecting these two
							Joint joint = null;

							foreach (var j in l.Joints)
							{
								if ((j.Joint.BodyA == l && j.Joint.BodyB == u) || (j.Joint.BodyA == u && j.Joint.BodyB == l))
								{
									joint = j.Joint;
									break;
								}
							}

							if (joint != null && (joint is DistanceJoint))
								m_world.DestroyJoint(joint);
						}
					}
				}
			}
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "w = wind");
			m_textLine += 15;
		}

		public override void Keyboard(SFML.Window.KeyCode key)
		{
			switch (char.ToLower((char)key))
			{
			case 'w':
				_wind = !_wind;
				break;
			}
		}
	}
}
