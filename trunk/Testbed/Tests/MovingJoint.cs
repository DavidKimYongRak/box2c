using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	public class MovingJoint : Test
	{
		RevoluteJoint joint;
		Body staticBody;

		public MovingJoint()
		{
			var capeFixture = new FixtureDef(new PolygonShape(0.08f, 0.4f, (float)(Math.PI)), 0.2f);
			var capeBody = new BodyDef(BodyType.Dynamic, new Vec2(0, 10));

			Body testbody = m_world.CreateBody(capeBody);
			testbody.CreateFixture(capeFixture);

			staticBody = m_world.CreateBody(new BodyDef(BodyType.Static, testbody.WorldCenter + new Vec2(-0.08f / 25, 0.4f)));
			staticBody.CreateFixture(new CircleShape(0.15f), 0);

			{
				RevoluteJointDef rjd = new RevoluteJointDef();
				rjd.Initialize(testbody, staticBody, testbody.WorldCenter + new Vec2(0.0f, 0.4f));
				joint = (RevoluteJoint)m_world.CreateJoint(rjd);
			}

			// build cape
			Body lastBody = testbody;
			for (int i = 0; i < 8; ++i)
			{
				capeBody.Position = new Vec2(capeBody.Position.X, capeBody.Position.Y - 0.8f);

				var nextBody = m_world.CreateBody(capeBody);
				nextBody.CreateFixture(capeFixture);

				var joint = new RevoluteJointDef();
				joint.Initialize(lastBody, nextBody, nextBody.WorldCenter + new Vec2(0.0f, 0.4f));
				m_world.CreateJoint(joint);

				lastBody = nextBody;
			}
		}

		bool _moving;
		Vec2 newPos;

		public override void MouseDown(Vec2 p)
		{
			_moving = true;
			newPos = p;
			base.MouseDown(p);
		}

		public override void MouseMove(Vec2 p)
		{
			if (_moving)
			{
				newPos = p;
			}
			base.MouseMove(p);
		}

		public override void MouseUp(Vec2 p)
		{
			_moving = false;
			base.MouseUp(p);
		}

		public override void Step()
		{
			if (_moving)
				staticBody.Position = newPos;

			base.Step();
		}
	}
}
