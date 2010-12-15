using System;
using Box2CS;

namespace Box2DSharpRenderTest
{
	public enum EBipedFixtureIndex
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

	public enum EBipedJointIndex
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
			for (int i = 0; i < (int)EBipedFixtureIndex.Max; ++i)
			{
				Bodies[i] = new BodyDef(EBodyType.b2_dynamicBody, Vec2.Empty);
				Fixtures[i] = new FixtureDef();
			}

			for (int i = 0; i < (int)EBipedJointIndex.Max; ++i)
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

			Fixtures[(int)EBipedFixtureIndex.LFoot].Friction = Fixtures[(int)EBipedFixtureIndex.RFoot].Friction = 0.85f;
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
			var filter = new FilterData(FilterData.Default.CategoryBits, FilterData.Default.MaskBits, v);
			foreach (var f in Fixtures)
				f.Filter = filter;
		}

		public void SetPosition(Vec2 v)
		{
			foreach (var b in Bodies)
				b.Position = v;
		}

		public BodyDef[] Bodies = new BodyDef[(int)EBipedFixtureIndex.Max];
		public FixtureDef[]	Fixtures = new FixtureDef[(int)EBipedFixtureIndex.Max];
		public RevoluteJointDef[] Joints = new RevoluteJointDef[(int)EBipedJointIndex.Max];

		void DefaultVertices()
		{
			{	// feet
				Fixtures[(int)EBipedFixtureIndex.LFoot].Shape =
					Fixtures[(int)EBipedFixtureIndex.RFoot].Shape =
					new PolygonShape(true,
						new Vec2(xScale, yScale) * new Vec2(.033f, .143f),
						new Vec2(xScale, yScale) * new Vec2(.023f, .033f),
						new Vec2(xScale, yScale) * new Vec2(.267f, .035f),
						new Vec2(xScale, yScale) * new Vec2(.265f, .065f),
						new Vec2(xScale, yScale) * new Vec2(.117f, .143f));
			}
			{	// calves
				Fixtures[(int)EBipedFixtureIndex.LCalf].Shape =
					Fixtures[(int)EBipedFixtureIndex.RCalf].Shape =
					new PolygonShape(true,
						new Vec2(xScale, yScale) * new Vec2(.089f, .016f),
						new Vec2(xScale, yScale) * new Vec2(.178f, .016f),
						new Vec2(xScale, yScale) * new Vec2(.205f, .417f),
						new Vec2(xScale, yScale) * new Vec2(.095f, .417f));
			}
			{	// thighs
				Fixtures[(int)EBipedFixtureIndex.LThigh].Shape =
					Fixtures[(int)EBipedFixtureIndex.RThigh].Shape =
					new PolygonShape(true,
						new Vec2(xScale, yScale) * new Vec2(.137f, .032f),
						new Vec2(xScale, yScale) * new Vec2(.243f, .032f),
						new Vec2(xScale, yScale) * new Vec2(.318f, .343f),
						new Vec2(xScale, yScale) * new Vec2(.142f, .343f));
			}
			{	// pelvis
				Fixtures[(int)EBipedFixtureIndex.Pelvis].Shape = new PolygonShape(true,
					new Vec2(xScale, yScale) * new Vec2(.105f, .051f),
					new Vec2(xScale, yScale) * new Vec2(.277f, .053f),
					new Vec2(xScale, yScale) * new Vec2(.320f, .233f),
					new Vec2(xScale, yScale) * new Vec2(.112f, .233f),
					new Vec2(xScale, yScale) * new Vec2(.067f, .152f));
			}
			{	// stomach
				Fixtures[(int)EBipedFixtureIndex.Stomach].Shape = new PolygonShape(true,
					new Vec2(xScale, yScale) * new Vec2(.088f, .043f),
					new Vec2(xScale, yScale) * new Vec2(.284f, .043f),
					new Vec2(xScale, yScale) * new Vec2(.295f, .231f),
					new Vec2(xScale, yScale) * new Vec2(.100f, .231f));
			}
			{	// chest
				Fixtures[(int)EBipedFixtureIndex.Chest].Shape = new PolygonShape(true,
					new Vec2(xScale, yScale) * new Vec2(.091f, .042f),
					new Vec2(xScale, yScale) * new Vec2(.283f, .042f),
					new Vec2(xScale, yScale) * new Vec2(.177f, .289f),
					new Vec2(xScale, yScale) * new Vec2(.065f, .289f));
			}
			{	// head
				Fixtures[(int)EBipedFixtureIndex.Head].Shape = new CircleShape(yScale * .115f);
			}
			{	// neck
				Fixtures[(int)EBipedFixtureIndex.Neck].Shape = new PolygonShape(true,
					new Vec2(xScale, yScale) * new Vec2(.038f, .054f),
					new Vec2(xScale, yScale) * new Vec2(.149f, .054f),
					new Vec2(xScale, yScale) * new Vec2(.154f, .102f),
					new Vec2(xScale, yScale) * new Vec2(.054f, .113f));
			}
			{	// upper arms
				Fixtures[(int)EBipedFixtureIndex.LUpperArm].Shape =
					Fixtures[(int)EBipedFixtureIndex.RUpperArm].Shape =
					new PolygonShape(true,
						new Vec2(xScale, yScale) * new Vec2(.092f, .059f),
						new Vec2(xScale, yScale) * new Vec2(.159f, .059f),
						new Vec2(xScale, yScale) * new Vec2(.169f, .335f),
						new Vec2(xScale, yScale) * new Vec2(.078f, .335f),
						new Vec2(xScale, yScale) * new Vec2(.064f, .248f));
			}
			{	// forearms
				Fixtures[(int)EBipedFixtureIndex.LForearm].Shape =
					Fixtures[(int)EBipedFixtureIndex.RForearm].Shape =
					new PolygonShape(true,
						new Vec2(xScale, yScale) * new Vec2(.082f, .054f),
						new Vec2(xScale, yScale) * new Vec2(.138f, .054f),
						new Vec2(xScale, yScale) * new Vec2(.149f, .296f),
						new Vec2(xScale, yScale) * new Vec2(.088f, .296f));
			}
			{	// hands
				Fixtures[(int)EBipedFixtureIndex.LHand].Shape =
					Fixtures[(int)EBipedFixtureIndex.RHand].Shape =
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
			Bodies[(int)EBipedFixtureIndex.LFoot].Position = Bodies[(int)EBipedFixtureIndex.RFoot].Position = new Vec2(xScale, yScale) * new Vec2(-.122f, -.901f);
			Bodies[(int)EBipedFixtureIndex.LCalf].Position = Bodies[(int)EBipedFixtureIndex.RCalf].Position = new Vec2(xScale, yScale) * new Vec2(-.177f, -.771f);
			Bodies[(int)EBipedFixtureIndex.LThigh].Position = Bodies[(int)EBipedFixtureIndex.RThigh].Position = new Vec2(xScale, yScale) * new Vec2(-.217f, -.391f);
			Bodies[(int)EBipedFixtureIndex.LUpperArm].Position = Bodies[(int)EBipedFixtureIndex.RUpperArm].Position = new Vec2(xScale, yScale) * new Vec2(-.127f, .228f);
			Bodies[(int)EBipedFixtureIndex.LForearm].Position = Bodies[(int)EBipedFixtureIndex.RForearm].Position = new Vec2(xScale, yScale) * new Vec2(-.117f, -.011f);
			Bodies[(int)EBipedFixtureIndex.LHand].Position = Bodies[(int)EBipedFixtureIndex.RHand].Position = new Vec2(xScale, yScale) * new Vec2(-.112f, -.136f);
			Bodies[(int)EBipedFixtureIndex.Pelvis].Position = new Vec2(xScale, yScale) * new Vec2(-.177f, -.101f);
			Bodies[(int)EBipedFixtureIndex.Stomach].Position = new Vec2(xScale, yScale) * new Vec2(-.142f, .088f);
			Bodies[(int)EBipedFixtureIndex.Chest].Position = new Vec2(xScale, yScale) * new Vec2(-.132f, .282f);
			Bodies[(int)EBipedFixtureIndex.Neck].Position = new Vec2(xScale, yScale) * new Vec2(-.102f, .518f);
			Bodies[(int)EBipedFixtureIndex.Head].Position = new Vec2(xScale, yScale) * new Vec2(.022f, .738f);
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
				Joints[(int)EBipedJointIndex.LAnkle].LocalAnchorA = Joints[(int)EBipedJointIndex.RAnkle].LocalAnchorA = anchor - Bodies[(int)EBipedFixtureIndex.LFoot].Position;
				Joints[(int)EBipedJointIndex.LAnkle].LocalAnchorB = Joints[(int)EBipedJointIndex.RAnkle].LocalAnchorB = anchor - Bodies[(int)EBipedFixtureIndex.LCalf].Position;
				Joints[(int)EBipedJointIndex.LAnkle].ReferenceAngle = Joints[(int)EBipedJointIndex.RAnkle].ReferenceAngle = 0.0f;
				Joints[(int)EBipedJointIndex.LAnkle].LowerAngle = Joints[(int)EBipedJointIndex.RAnkle].LowerAngle = ReverseAngle(-0.523598776f);
				Joints[(int)EBipedJointIndex.LAnkle].UpperAngle = Joints[(int)EBipedJointIndex.RAnkle].UpperAngle = ReverseAngle(0.523598776f);
			}

			//b.LKneeDef.body1		= LCalf;
			//b.LKneeDef.body2		= LThigh;
			//b.RKneeDef.body1		= RCalf;
			//b.RKneeDef.body2		= RThigh;
			{	// knees
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.030f, -.355f);
				Joints[(int)EBipedJointIndex.LKnee].LocalAnchorA = Joints[(int)EBipedJointIndex.RKnee].LocalAnchorA = anchor - Bodies[(int)EBipedFixtureIndex.LCalf].Position;
				Joints[(int)EBipedJointIndex.LKnee].LocalAnchorB = Joints[(int)EBipedJointIndex.RKnee].LocalAnchorB = anchor - Bodies[(int)EBipedFixtureIndex.LThigh].Position;
				Joints[(int)EBipedJointIndex.LKnee].ReferenceAngle = Joints[(int)EBipedJointIndex.RKnee].ReferenceAngle = 0.0f;
				Joints[(int)EBipedJointIndex.LKnee].LowerAngle = Joints[(int)EBipedJointIndex.RKnee].LowerAngle = ReverseAngle(0);
				Joints[(int)EBipedJointIndex.LKnee].UpperAngle = Joints[(int)EBipedJointIndex.RKnee].UpperAngle = ReverseAngle(2.61799388f);
			}

			//b.LHipDef.body1			= LThigh;
			//b.LHipDef.body2			= Pelvis;
			//b.RHipDef.body1			= RThigh;
			//b.RHipDef.body2			= Pelvis;
			{	// hips
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(.005f, -.045f);
				Joints[(int)EBipedJointIndex.LHip].LocalAnchorA = Joints[(int)EBipedJointIndex.RHip].LocalAnchorA = anchor - Bodies[(int)EBipedFixtureIndex.LThigh].Position;
				Joints[(int)EBipedJointIndex.LHip].LocalAnchorB = Joints[(int)EBipedJointIndex.RHip].LocalAnchorB = anchor - Bodies[(int)EBipedFixtureIndex.Pelvis].Position;
				Joints[(int)EBipedJointIndex.LHip].ReferenceAngle = Joints[(int)EBipedJointIndex.RHip].ReferenceAngle = 0.0f;
				Joints[(int)EBipedJointIndex.LHip].LowerAngle = Joints[(int)EBipedJointIndex.RHip].LowerAngle = ReverseAngle(-2.26892803f);
				Joints[(int)EBipedJointIndex.LHip].UpperAngle = Joints[(int)EBipedJointIndex.RHip].UpperAngle = 0;
			}

			//b.LowerAbsDef.body1		= Pelvis;
			//b.LowerAbsDef.body2		= Stomach;
			{	// lower abs
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(.035f, .135f);
				Joints[(int)EBipedJointIndex.LowerAbs].LocalAnchorA = anchor - Bodies[(int)EBipedFixtureIndex.Pelvis].Position;
				Joints[(int)EBipedJointIndex.LowerAbs].LocalAnchorB = anchor - Bodies[(int)EBipedFixtureIndex.Stomach].Position;
				Joints[(int)EBipedJointIndex.LowerAbs].ReferenceAngle = 0.0f;
				Joints[(int)EBipedJointIndex.LowerAbs].LowerAngle = ReverseAngle(-0.523598776f);
				Joints[(int)EBipedJointIndex.LowerAbs].UpperAngle = ReverseAngle(0.523598776f);
			}

			//b.UpperAbsDef.body1		= Stomach;
			//b.UpperAbsDef.body2		= Chest;
			{	// upper abs
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(.045f, .320f);
				Joints[(int)EBipedJointIndex.UpperAbs].LocalAnchorA = anchor - Bodies[(int)EBipedFixtureIndex.Stomach].Position;
				Joints[(int)EBipedJointIndex.UpperAbs].LocalAnchorB = anchor - Bodies[(int)EBipedFixtureIndex.Chest].Position;
				Joints[(int)EBipedJointIndex.UpperAbs].ReferenceAngle = 0.0f;
				Joints[(int)EBipedJointIndex.UpperAbs].LowerAngle = ReverseAngle(-0.523598776f);
				Joints[(int)EBipedJointIndex.UpperAbs].UpperAngle = ReverseAngle(0.174532925f);
			}

			//b.LowerNeckDef.body1	= Chest;
			//b.LowerNeckDef.body2	= Neck;
			{	// lower neck
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.015f, .575f);
				Joints[(int)EBipedJointIndex.LowerNeck].LocalAnchorA = anchor - Bodies[(int)EBipedFixtureIndex.Chest].Position;
				Joints[(int)EBipedJointIndex.LowerNeck].LocalAnchorB = anchor - Bodies[(int)EBipedFixtureIndex.Neck].Position;
				Joints[(int)EBipedJointIndex.LowerNeck].ReferenceAngle = 0.0f;
				Joints[(int)EBipedJointIndex.LowerNeck].LowerAngle = ReverseAngle(-0.174532925f);
				Joints[(int)EBipedJointIndex.LowerNeck].UpperAngle = ReverseAngle(0.174532925f);
			}

			//b.UpperNeckDef.body1	= Chest;
			//b.UpperNeckDef.body2	= Head;
			{	// upper neck
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.005f, .630f);
				Joints[(int)EBipedJointIndex.UpperNeck].LocalAnchorA = anchor - Bodies[(int)EBipedFixtureIndex.Chest].Position;
				Joints[(int)EBipedJointIndex.UpperNeck].LocalAnchorB = anchor - Bodies[(int)EBipedFixtureIndex.Head].Position;
				Joints[(int)EBipedJointIndex.UpperNeck].ReferenceAngle = 0.0f;
				Joints[(int)EBipedJointIndex.UpperNeck].LowerAngle = ReverseAngle(-0.610865238f);
				Joints[(int)EBipedJointIndex.UpperNeck].UpperAngle = ReverseAngle(0.785398163f);
			}

			//b.LShoulderDef.body1	= Chest;
			//b.LShoulderDef.body2	= LUpperArm;
			//b.RShoulderDef.body1	= Chest;
			//b.RShoulderDef.body2	= RUpperArm;
			{	// shoulders
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.015f, .545f);
				Joints[(int)EBipedJointIndex.LShoulder].LocalAnchorA = Joints[(int)EBipedJointIndex.RShoulder].LocalAnchorA = anchor - Bodies[(int)EBipedFixtureIndex.Chest].Position;
				Joints[(int)EBipedJointIndex.LShoulder].LocalAnchorB = Joints[(int)EBipedJointIndex.RShoulder].LocalAnchorB = anchor - Bodies[(int)EBipedFixtureIndex.LUpperArm].Position;
				Joints[(int)EBipedJointIndex.LShoulder].ReferenceAngle = Joints[(int)EBipedJointIndex.RShoulder].ReferenceAngle = 0.0f;
				Joints[(int)EBipedJointIndex.LShoulder].LowerAngle = Joints[(int)EBipedJointIndex.RShoulder].LowerAngle = ReverseAngle(-1.04719755f);
				Joints[(int)EBipedJointIndex.LShoulder].UpperAngle = Joints[(int)EBipedJointIndex.RShoulder].UpperAngle = ReverseAngle(3.14159265f);
			}

			//b.LElbowDef.body1		= LForearm;
			//b.LElbowDef.body2		= LUpperArm;
			//b.RElbowDef.body1		= RForearm;
			//b.RElbowDef.body2		= RUpperArm;
			{	// elbows
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.005f, .290f);
				Joints[(int)EBipedJointIndex.LElbow].LocalAnchorA = Joints[(int)EBipedJointIndex.RElbow].LocalAnchorA = anchor - Bodies[(int)EBipedFixtureIndex.LForearm].Position;
				Joints[(int)EBipedJointIndex.LElbow].LocalAnchorB = Joints[(int)EBipedJointIndex.RElbow].LocalAnchorB = anchor - Bodies[(int)EBipedFixtureIndex.LUpperArm].Position;
				Joints[(int)EBipedJointIndex.LElbow].ReferenceAngle = Joints[(int)EBipedJointIndex.RElbow].ReferenceAngle = 0.0f;
				Joints[(int)EBipedJointIndex.LElbow].LowerAngle = Joints[(int)EBipedJointIndex.RElbow].LowerAngle = ReverseAngle(-2.7925268f);
				Joints[(int)EBipedJointIndex.LElbow].UpperAngle = Joints[(int)EBipedJointIndex.RElbow].UpperAngle = 0;
			}

			//b.LWristDef.body1		= LHand;
			//b.LWristDef.body2		= LForearm;
			//b.RWristDef.body1		= RHand;
			//b.RWristDef.body2		= RForearm;
			{	// wrists
				Vec2 anchor = new Vec2(xScale, yScale) * new Vec2(-.010f, .045f);
				Joints[(int)EBipedJointIndex.LWrist].LocalAnchorA = Joints[(int)EBipedJointIndex.RWrist].LocalAnchorA = anchor - Bodies[(int)EBipedFixtureIndex.LHand].Position;
				Joints[(int)EBipedJointIndex.LWrist].LocalAnchorB = Joints[(int)EBipedJointIndex.RWrist].LocalAnchorB = anchor - Bodies[(int)EBipedFixtureIndex.LForearm].Position;
				Joints[(int)EBipedJointIndex.LWrist].ReferenceAngle = Joints[(int)EBipedJointIndex.RWrist].ReferenceAngle = 0.0f;
				Joints[(int)EBipedJointIndex.LWrist].LowerAngle = Joints[(int)EBipedJointIndex.RWrist].LowerAngle = ReverseAngle(-0.174532925f);
				Joints[(int)EBipedJointIndex.LWrist].UpperAngle = Joints[(int)EBipedJointIndex.RWrist].UpperAngle = ReverseAngle(0.174532925f);
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
