using System;
using Box2CS;

namespace Box2DSharpRenderTest
{
	public enum BipedFixtureIndex
	{
		LFoot,
		RFoot,
		LCalf,
		RCalf,
		LThigh,
		RThigh,
		Pelvis,
		Stomach,
		Chest,
		Neck,
		Head,
		LUpperArm,
		RUpperArm,
		LForearm,
		RForearm,
		LHand,
		RHand,
		Max
	};

	public enum BipedJointIndex
	{
		LAnkle,
		RAnkle,
		LKnee,
		RKnee,
		LHip,
		RHip,
		LowerAbs,
		UpperAbs,
		LowerNeck,
		UpperNeck,
		LShoulder,
		RShoulder,
		LElbow,
		RElbow,
		LWrist,
		RWrist,
		Max
	}

	public class BipedDef
	{
		static short count = 0;
		float xScale, yScale;
		public BipedDef(float xscale = 3.0f, float yscale = 3.0f)
		{
			for (int i = 0; i < (int)BipedFixtureIndex.Max; ++i)
			{
				Bodies[i] = new BodyDef(BodyType.Dynamic, Vec2.Empty);
				Fixtures[i] = new FixtureDef();
			}

			for (int i = 0; i < (int)BipedJointIndex.Max; ++i)
				Joints[i] = new RevoluteJointDef();

			xScale = xscale;
			yScale = yscale;
			SetMotorTorque(2.0f);
			SetMotorSpeed(0.0f);
			SetDensity(4.0f);
			SetRestitution(0.0f);
			SetLinearDamping(0.0f);
			SetAngularDamping(0.005f);
			SetGroupIndex(--count);
			SetMotor(false);
			SetLimit(true);

			DefaultVertices();
			DefaultPositions();
			DefaultJoints();

			Fixtures[(int)BipedFixtureIndex.LFoot].Friction = Fixtures[(int)BipedFixtureIndex.RFoot].Friction = 0.85f;
		}

		public void SetMotorTorque(float v)
		{
			foreach (var j in Joints)
				j.MaxMotorTorque = v;
		}
		public void SetMotorSpeed(float v)
		{
			foreach (var j in Joints)
				j.MotorSpeed = v;
		}
		public void SetDensity(float v)
		{
			foreach (var f in Fixtures)
				f.Density = v;
		}
		public void SetFriction(float v)
		{
			foreach (var f in Fixtures)
				f.Friction = v;
		}
		public void SetRestitution(float v)
		{
			foreach (var f in Fixtures)
				f.Restitution = v;
		}
		public void SetLinearDamping(float v)
		{
			foreach (var b in Bodies)
				b.LinearDamping = v;
		}
		public void SetAngularDamping(float v)
		{
			foreach (var b in Bodies)
				b.AngularDamping = v;
		}
		public void SetLimit(bool v)
		{
			foreach (var j in Joints)
				j.EnableLimit = v;
		}
		public void SetMotor(bool v)
		{
			foreach (var j in Joints)
				j.EnableMotor = v;
		}
		public void SetGroupIndex(short v)
		{
			foreach (var f in Fixtures)
				f.Filter.GroupIndex = v;
		}

		public void SetPosition(Vec2 v)
		{
			foreach (var b in Bodies)
				b.Position = v;
		}

		public BodyDef[] Bodies = new BodyDef[(int)BipedFixtureIndex.Max];
		public FixtureDef[]	Fixtures = new FixtureDef[(int)BipedFixtureIndex.Max];
		public RevoluteJointDef[] Joints = new RevoluteJointDef[(int)BipedJointIndex.Max];

		void DefaultVertices()
		{
			{	// feet
				Fixtures[(int)BipedFixtureIndex.LFoot].Shape =
					Fixtures[(int)BipedFixtureIndex.RFoot].Shape =
					new PolygonShape(true,
						new Vec2(xScale, yScale) * new Vec2(.033f, .143f),
						new Vec2(xScale, yScale) * new Vec2(.023f, .033f),
						new Vec2(xScale, yScale) * new Vec2(.267f, .035f),
						new Vec2(xScale, yScale) * new Vec2(.265f, .065f),
						new Vec2(xScale, yScale) * new Vec2(.117f, .143f));
			}
			{	// calves
				Fixtures[(int)BipedFixtureIndex.LCalf].Shape =
					Fixtures[(int)BipedFixtureIndex.RCalf].Shape =
					new PolygonShape(true,
						new Vec2(xScale, yScale) * new Vec2(.089f, .016f),
						new Vec2(xScale, yScale) * new Vec2(.178f, .016f),
						new Vec2(xScale, yScale) * new Vec2(.205f, .417f),
						new Vec2(xScale, yScale) * new Vec2(.095f, .417f));
			}
			{	// thighs
				Fixtures[(int)BipedFixtureIndex.LThigh].Shape =
					Fixtures[(int)BipedFixtureIndex.RThigh].Shape =
					new PolygonShape(true,
						new Vec2(xScale, yScale) * new Vec2(.137f, .032f),
						new Vec2(xScale, yScale) * new Vec2(.243f, .032f),
						new Vec2(xScale, yScale) * new Vec2(.318f, .343f),
						new Vec2(xScale, yScale) * new Vec2(.142f, .343f));
			}
			{	// pelvis
				Fixtures[(int)BipedFixtureIndex.Pelvis].Shape = new PolygonShape(true,
					new Vec2(xScale, yScale) * new Vec2(.105f, .051f),
					new Vec2(xScale, yScale) * new Vec2(.277f, .053f),
					new Vec2(xScale, yScale) * new Vec2(.320f, .233f),
					new Vec2(xScale, yScale) * new Vec2(.112f, .233f),
					new Vec2(xScale, yScale) * new Vec2(.067f, .152f));
			}
			{	// stomach
				Fixtures[(int)BipedFixtureIndex.Stomach].Shape = new PolygonShape(true,
					new Vec2(xScale, yScale) * new Vec2(.088f, .043f),
					new Vec2(xScale, yScale) * new Vec2(.284f, .043f),
					new Vec2(xScale, yScale) * new Vec2(.295f, .231f),
					new Vec2(xScale, yScale) * new Vec2(.100f, .231f));
			}
			{	// chest
				Fixtures[(int)BipedFixtureIndex.Chest].Shape = new PolygonShape(true,
					new Vec2(xScale, yScale) * new Vec2(.091f, .042f),
					new Vec2(xScale, yScale) * new Vec2(.283f, .042f),
					new Vec2(xScale, yScale) * new Vec2(.177f, .289f),
					new Vec2(xScale, yScale) * new Vec2(.065f, .289f));
			}
			{	// head
				Fixtures[(int)BipedFixtureIndex.Head].Shape = new CircleShape(yScale * .115f);
			}
			{	// neck
				Fixtures[(int)BipedFixtureIndex.Neck].Shape = new PolygonShape(true,
					new Vec2(xScale, yScale) * new Vec2(.038f, .054f),
					new Vec2(xScale, yScale) * new Vec2(.149f, .054f),
					new Vec2(xScale, yScale) * new Vec2(.154f, .102f),
					new Vec2(xScale, yScale) * new Vec2(.054f, .113f));
			}
			{	// upper arms
				Fixtures[(int)BipedFixtureIndex.LUpperArm].Shape =
					Fixtures[(int)BipedFixtureIndex.RUpperArm].Shape =
					new PolygonShape(true,
						new Vec2(xScale, yScale) * new Vec2(.092f, .059f),
						new Vec2(xScale, yScale) * new Vec2(.159f, .059f),
						new Vec2(xScale, yScale) * new Vec2(.169f, .335f),
						new Vec2(xScale, yScale) * new Vec2(.078f, .335f),
						new Vec2(xScale, yScale) * new Vec2(.064f, .248f));
			}
			{	// forearms
				Fixtures[(int)BipedFixtureIndex.LForearm].Shape =
					Fixtures[(int)BipedFixtureIndex.RForearm].Shape =
					new PolygonShape(true,
						new Vec2(xScale, yScale) * new Vec2(.082f, .054f),
						new Vec2(xScale, yScale) * new Vec2(.138f, .054f),
						new Vec2(xScale, yScale) * new Vec2(.149f, .296f),
						new Vec2(xScale, yScale) * new Vec2(.088f, .296f));
			}
			{	// hands
				Fixtures[(int)BipedFixtureIndex.LHand].Shape =
					Fixtures[(int)BipedFixtureIndex.RHand].Shape =
					new PolygonShape(true,
						new Vec2(xScale, yScale) * new Vec2(.066f, .031f),
						new Vec2(xScale, yScale) * new Vec2(.123f, .020f),
						new Vec2(xScale, yScale) * new Vec2(.160f, .127f),
						new Vec2(xScale, yScale) * new Vec2(.127f, .178f),
						new Vec2(xScale, yScale) * new Vec2(.074f, .178f));
			}
		}

		void DefaultPositions()
		{
			Bodies[(int)BipedFixtureIndex.LFoot].Position = Bodies[(int)BipedFixtureIndex.RFoot].Position = new Vec2(xScale, yScale) * new Vec2(-.122f, -.901f);
			Bodies[(int)BipedFixtureIndex.LCalf].Position = Bodies[(int)BipedFixtureIndex.RCalf].Position = new Vec2(xScale, yScale) * new Vec2(-.177f, -.771f);
			Bodies[(int)BipedFixtureIndex.LThigh].Position = Bodies[(int)BipedFixtureIndex.RThigh].Position = new Vec2(xScale, yScale) * new Vec2(-.217f, -.391f);
			Bodies[(int)BipedFixtureIndex.LUpperArm].Position = Bodies[(int)BipedFixtureIndex.RUpperArm].Position = new Vec2(xScale, yScale) * new Vec2(-.127f, .228f);
			Bodies[(int)BipedFixtureIndex.LForearm].Position = Bodies[(int)BipedFixtureIndex.RForearm].Position = new Vec2(xScale, yScale) * new Vec2(-.117f, -.011f);
			Bodies[(int)BipedFixtureIndex.LHand].Position = Bodies[(int)BipedFixtureIndex.RHand].Position = new Vec2(xScale, yScale) * new Vec2(-.112f, -.136f);
			Bodies[(int)BipedFixtureIndex.Pelvis].Position = new Vec2(xScale, yScale) * new Vec2(-.177f, -.101f);
			Bodies[(int)BipedFixtureIndex.Stomach].Position = new Vec2(xScale, yScale) * new Vec2(-.142f, .088f);
			Bodies[(int)BipedFixtureIndex.Chest].Position = new Vec2(xScale, yScale) * new Vec2(-.132f, .282f);
			Bodies[(int)BipedFixtureIndex.Neck].Position = new Vec2(xScale, yScale) * new Vec2(-.102f, .518f);
			Bodies[(int)BipedFixtureIndex.Head].Position = new Vec2(xScale, yScale) * new Vec2(.022f, .738f);
		}

		float ReverseAngle (float x)
		{
			if (xScale < 0)
				return -x;
			return x;
		}
		
		void DefaultJoints()
		{
			//b.LAnkleDef.body1		= LFoot;
			//b.LAnkleDef.body2		= LCalf;
			//b.RAnkleDef.body1		= RFoot;
			//b.RAnkleDef.body2		= RCalf;
			{	// ankles
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.045f, -.75f);
				Joints[(int)BipedJointIndex.LAnkle].LocalAnchorA = Joints[(int)BipedJointIndex.RAnkle].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.LFoot].Position;
				Joints[(int)BipedJointIndex.LAnkle].LocalAnchorB = Joints[(int)BipedJointIndex.RAnkle].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.LCalf].Position;
				Joints[(int)BipedJointIndex.LAnkle].ReferenceAngle = Joints[(int)BipedJointIndex.RAnkle].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LAnkle].LowerAngle = Joints[(int)BipedJointIndex.RAnkle].LowerAngle = ReverseAngle(-0.523598776f);
				Joints[(int)BipedJointIndex.LAnkle].UpperAngle = Joints[(int)BipedJointIndex.RAnkle].UpperAngle = ReverseAngle(0.523598776f);
			}

			//b.LKneeDef.body1		= LCalf;
			//b.LKneeDef.body2		= LThigh;
			//b.RKneeDef.body1		= RCalf;
			//b.RKneeDef.body2		= RThigh;
			{	// knees
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.030f, -.355f);
				Joints[(int)BipedJointIndex.LKnee].LocalAnchorA = Joints[(int)BipedJointIndex.RKnee].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.LCalf].Position;
				Joints[(int)BipedJointIndex.LKnee].LocalAnchorB = Joints[(int)BipedJointIndex.RKnee].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.LThigh].Position;
				Joints[(int)BipedJointIndex.LKnee].ReferenceAngle = Joints[(int)BipedJointIndex.RKnee].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LKnee].LowerAngle = Joints[(int)BipedJointIndex.RKnee].LowerAngle = ReverseAngle(0);
				Joints[(int)BipedJointIndex.LKnee].UpperAngle = Joints[(int)BipedJointIndex.RKnee].UpperAngle = ReverseAngle(2.61799388f);
			}

			//b.LHipDef.body1			= LThigh;
			//b.LHipDef.body2			= Pelvis;
			//b.RHipDef.body1			= RThigh;
			//b.RHipDef.body2			= Pelvis;
			{	// hips
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(.005f, -.045f);
				Joints[(int)BipedJointIndex.LHip].LocalAnchorA = Joints[(int)BipedJointIndex.RHip].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.LThigh].Position;
				Joints[(int)BipedJointIndex.LHip].LocalAnchorB = Joints[(int)BipedJointIndex.RHip].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.Pelvis].Position;
				Joints[(int)BipedJointIndex.LHip].ReferenceAngle = Joints[(int)BipedJointIndex.RHip].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LHip].LowerAngle = Joints[(int)BipedJointIndex.RHip].LowerAngle = ReverseAngle(-0.76892803f);
				Joints[(int)BipedJointIndex.LHip].UpperAngle = Joints[(int)BipedJointIndex.RHip].UpperAngle = 0;
			}

			//b.LowerAbsDef.body1		= Pelvis;
			//b.LowerAbsDef.body2		= Stomach;
			{	// lower abs
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(.035f, .135f);
				Joints[(int)BipedJointIndex.LowerAbs].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.Pelvis].Position;
				Joints[(int)BipedJointIndex.LowerAbs].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.Stomach].Position;
				Joints[(int)BipedJointIndex.LowerAbs].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LowerAbs].LowerAngle = ReverseAngle(-0.523598776f);
				Joints[(int)BipedJointIndex.LowerAbs].UpperAngle = ReverseAngle(0.523598776f);
			}

			//b.UpperAbsDef.body1		= Stomach;
			//b.UpperAbsDef.body2		= Chest;
			{	// upper abs
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(.045f, .320f);
				Joints[(int)BipedJointIndex.UpperAbs].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.Stomach].Position;
				Joints[(int)BipedJointIndex.UpperAbs].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.Chest].Position;
				Joints[(int)BipedJointIndex.UpperAbs].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.UpperAbs].LowerAngle = ReverseAngle(-0.523598776f);
				Joints[(int)BipedJointIndex.UpperAbs].UpperAngle = ReverseAngle(0.174532925f);
			}

			//b.LowerNeckDef.body1	= Chest;
			//b.LowerNeckDef.body2	= Neck;
			{	// lower neck
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.015f, .575f);
				Joints[(int)BipedJointIndex.LowerNeck].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.Chest].Position;
				Joints[(int)BipedJointIndex.LowerNeck].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.Neck].Position;
				Joints[(int)BipedJointIndex.LowerNeck].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LowerNeck].LowerAngle = ReverseAngle(-0.174532925f);
				Joints[(int)BipedJointIndex.LowerNeck].UpperAngle = ReverseAngle(0.174532925f);
			}

			//b.UpperNeckDef.body1	= Chest;
			//b.UpperNeckDef.body2	= Head;
			{	// upper neck
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.005f, .630f);
				Joints[(int)BipedJointIndex.UpperNeck].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.Chest].Position;
				Joints[(int)BipedJointIndex.UpperNeck].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.Head].Position;
				Joints[(int)BipedJointIndex.UpperNeck].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.UpperNeck].LowerAngle = ReverseAngle(-0.610865238f);
				Joints[(int)BipedJointIndex.UpperNeck].UpperAngle = ReverseAngle(0.785398163f);
			}

			//b.LShoulderDef.body1	= Chest;
			//b.LShoulderDef.body2	= LUpperArm;
			//b.RShoulderDef.body1	= Chest;
			//b.RShoulderDef.body2	= RUpperArm;
			{	// shoulders
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.015f, .545f);
				Joints[(int)BipedJointIndex.LShoulder].LocalAnchorA = Joints[(int)BipedJointIndex.RShoulder].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.Chest].Position;
				Joints[(int)BipedJointIndex.LShoulder].LocalAnchorB = Joints[(int)BipedJointIndex.RShoulder].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.LUpperArm].Position;
				Joints[(int)BipedJointIndex.LShoulder].ReferenceAngle = Joints[(int)BipedJointIndex.RShoulder].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LShoulder].LowerAngle = Joints[(int)BipedJointIndex.RShoulder].LowerAngle = ReverseAngle(-1.04719755f);
				Joints[(int)BipedJointIndex.LShoulder].UpperAngle = Joints[(int)BipedJointIndex.RShoulder].UpperAngle = ReverseAngle(3.14159265f);
			}

			//b.LElbowDef.body1		= LForearm;
			//b.LElbowDef.body2		= LUpperArm;
			//b.RElbowDef.body1		= RForearm;
			//b.RElbowDef.body2		= RUpperArm;
			{	// elbows
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.005f, .290f);
				Joints[(int)BipedJointIndex.LElbow].LocalAnchorA = Joints[(int)BipedJointIndex.RElbow].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.LForearm].Position;
				Joints[(int)BipedJointIndex.LElbow].LocalAnchorB = Joints[(int)BipedJointIndex.RElbow].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.LUpperArm].Position;
				Joints[(int)BipedJointIndex.LElbow].ReferenceAngle = Joints[(int)BipedJointIndex.RElbow].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LElbow].LowerAngle = Joints[(int)BipedJointIndex.RElbow].LowerAngle = ReverseAngle(-2.7925268f);
				Joints[(int)BipedJointIndex.LElbow].UpperAngle = Joints[(int)BipedJointIndex.RElbow].UpperAngle = 0;
			}

			//b.LWristDef.body1		= LHand;
			//b.LWristDef.body2		= LForearm;
			//b.RWristDef.body1		= RHand;
			//b.RWristDef.body2		= RForearm;
			{	// wrists
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.010f, .045f);
				Joints[(int)BipedJointIndex.LWrist].LocalAnchorA = Joints[(int)BipedJointIndex.RWrist].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.LHand].Position;
				Joints[(int)BipedJointIndex.LWrist].LocalAnchorB = Joints[(int)BipedJointIndex.RWrist].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.LForearm].Position;
				Joints[(int)BipedJointIndex.LWrist].ReferenceAngle = Joints[(int)BipedJointIndex.RWrist].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LWrist].LowerAngle = Joints[(int)BipedJointIndex.RWrist].LowerAngle = ReverseAngle(-0.174532925f);
				Joints[(int)BipedJointIndex.LWrist].UpperAngle = Joints[(int)BipedJointIndex.RWrist].UpperAngle = ReverseAngle(0.174532925f);
			}

			if (xScale < 0)
				foreach (RevoluteJointDef j in Joints)
				{
					var old = j.UpperAngle;
					j.UpperAngle = j.LowerAngle;
					j.LowerAngle = old;
				}
		}
	};
}
