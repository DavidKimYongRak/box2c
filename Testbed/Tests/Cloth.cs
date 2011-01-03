using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	public class Cloth : Test
	{
		const float ClothBodySize = 0.35f;
		const int ClothSegmentsWidth = 16;
		const int ClothSegmentsHeight = 20;
		const float ClothBodySpacingWidth = 0.35f * 2;
		const float ClothTotalWidth = (ClothBodySpacingWidth * ClothSegmentsWidth);
		Body[,] cloth = new Body[ClothSegmentsWidth, ClothSegmentsHeight];

		public Cloth()
		{
			FixtureDef boxFix = new FixtureDef(new PolygonShape(ClothBodySize, ClothBodySize), 0.2f);
			BodyDef boxBod = new BodyDef(BodyType.Dynamic, Vec2.Empty);

			//boxFix.Filter.GroupIndex = -1;
			boxBod.Position = new Vec2(-ClothTotalWidth / 2, 30);

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
						wjd.Initialize(body, m_groundBody, body.WorldCenter);
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

					// connect to right
					if (x != ClothSegmentsWidth - 1)
					{
						leftBody = cloth[x, y];
						rightBody = cloth[x + 1, y];

						DistanceJointDef djd = new DistanceJointDef();
						djd.Initialize(leftBody, rightBody, leftBody.WorldCenter, rightBody.WorldCenter);
						djd.FrequencyHz = 6;
						djd.DampingRatio = 0.11f;
						m_world.CreateJoint(djd);
					}

					// connect to up
					if (y != 0)
					{
						leftBody = cloth[x, y];
						rightBody = cloth[x, y - 1];

						DistanceJointDef djd = new DistanceJointDef();
						djd.Initialize(leftBody, rightBody, leftBody.WorldCenter, rightBody.WorldCenter);
						djd.FrequencyHz = 6;
						djd.DampingRatio = 0.11f;
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
		}

		public override void Draw()
		{
			base.Draw();

			m_debugDraw.DrawString(5, m_textLine, "w = wind");
			m_textLine += 15;

			for (int x = 0; x < ClothSegmentsWidth - 1; ++x)
			{
				m_debugDraw.DrawSegment(cloth[x, 0].WorldCenter, cloth[x + 1, 0].WorldCenter, new ColorF(1, 0, 0));
				m_debugDraw.DrawSegment(cloth[x, ClothSegmentsHeight - 1].WorldCenter, cloth[x + 1, ClothSegmentsHeight - 1].WorldCenter, new ColorF(1, 0, 0));
			}

			for (int y = 0; y < ClothSegmentsHeight - 1; ++y)
			{
				m_debugDraw.DrawSegment(cloth[0, y].WorldCenter, cloth[0, y + 1].WorldCenter, new ColorF(1, 0, 0));
				m_debugDraw.DrawSegment(cloth[ClothSegmentsWidth - 1, y].WorldCenter, cloth[ClothSegmentsWidth - 1, y + 1].WorldCenter, new ColorF(1, 0, 0));
			}
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
