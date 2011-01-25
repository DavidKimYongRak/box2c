using System;

using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;

namespace Box2DSharpRenderTest
{
	public enum BipedFixtureIndex
	{
		RFoot,
		RCalf,
		RThigh,
		Pelvis,
		Stomach,
		Chest,
		Neck,
		Head,
		RUpperArm,
		RForearm,
		RHand,

		LFoot,
		LCalf,
		LThigh,
		LUpperArm,
		LForearm,
		LHand,

		Max
	};

	public enum BipedJointIndex
	{
		RAnkle,
		RKnee,
		RHip,
		LowerAbs,
		UpperAbs,
		LowerNeck,
		UpperNeck,
		RShoulder,
		RElbow,
		RWrist,

		LAnkle,
		LKnee,
		LHip,
		LShoulder,
		LElbow,
		LWrist,

		Max
	}

	public class BipedDef
	{
		static short count = 0;
		float xScale, yScale;
		public BipedDef(World world, float xscale = 3.0f, float yscale = 3.0f)
		{
			xScale = xscale;
			yScale = yscale;

			for (int i = 0; i < (int)BipedFixtureIndex.Max; ++i)
			{
				Bodies[i] = new Body(world);
				Bodies[i].BodyType = BodyType.Dynamic;
			}

			for (int i = 0; i < (int)BipedJointIndex.Max; ++i)
				Joints[i] = new RevoluteJoint(Bodies[0], Bodies[1], Vector2.Zero, Vector2.Zero);

			DefaultVertices();
			DefaultPositions();
			DefaultJoints();

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
				f.Shape.Density = v;
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
				j.LimitEnabled = v;
		}
		public void SetMotor(bool v)
		{
			foreach (var j in Joints)
				j.MotorEnabled = v;
		}
		public void SetGroupIndex(short v)
		{
			foreach (var f in Fixtures)
				f.CollisionFilter.CollisionGroup = v;
		}

		public void SetPosition(Vector2 v)
		{
			foreach (var b in Bodies)
				b.Position = v;
		}

		public Body[] Bodies = new Body[(int)BipedFixtureIndex.Max];
		public Fixture[]	Fixtures = new Fixture[(int)BipedFixtureIndex.Max];
		public RevoluteJoint[] Joints = new RevoluteJoint[(int)BipedJointIndex.Max];

		public static PolygonShape ReverseShape(params Vector2[] verts)
		{
			Vertices ver = new Vertices(verts);
			ver.ForceCounterClockWise();

			return new PolygonShape(ver, 4.0f);
		}

		void DefaultVertices()
		{
			BipedModel model = new BipedModel(xScale, yScale);

			for (int i = 0; i < (int)BipedFixtureIndex.Max; ++i)
				Fixtures[i] = new Fixture(Bodies[i], model[(BipedFixtureIndex)i]);
		}

		void DefaultPositions()
		{
			Bodies[(int)BipedFixtureIndex.LFoot].Position = Bodies[(int)BipedFixtureIndex.RFoot].Position = new Vector2(xScale, yScale) * new Vector2(-.122f, -.901f);
			Bodies[(int)BipedFixtureIndex.LCalf].Position = Bodies[(int)BipedFixtureIndex.RCalf].Position = new Vector2(xScale, yScale) * new Vector2(-.177f, -.771f);
			Bodies[(int)BipedFixtureIndex.LThigh].Position = Bodies[(int)BipedFixtureIndex.RThigh].Position = new Vector2(xScale, yScale) * new Vector2(-.217f, -.391f);
			Bodies[(int)BipedFixtureIndex.LUpperArm].Position = Bodies[(int)BipedFixtureIndex.RUpperArm].Position = new Vector2(xScale, yScale) * new Vector2(-.127f, .228f);
			Bodies[(int)BipedFixtureIndex.LForearm].Position = Bodies[(int)BipedFixtureIndex.RForearm].Position = new Vector2(xScale, yScale) * new Vector2(-.117f, -.011f);
			Bodies[(int)BipedFixtureIndex.LHand].Position = Bodies[(int)BipedFixtureIndex.RHand].Position = new Vector2(xScale, yScale) * new Vector2(-.112f, -.136f);
			Bodies[(int)BipedFixtureIndex.Pelvis].Position = new Vector2(xScale, yScale) * new Vector2(-.177f, -.101f);
			Bodies[(int)BipedFixtureIndex.Stomach].Position = new Vector2(xScale, yScale) * new Vector2(-.142f, .088f);
			Bodies[(int)BipedFixtureIndex.Chest].Position = new Vector2(xScale, yScale) * new Vector2(-.132f, .282f);
			Bodies[(int)BipedFixtureIndex.Neck].Position = new Vector2(xScale, yScale) * new Vector2(-.102f, .518f);
			Bodies[(int)BipedFixtureIndex.Head].Position = new Vector2(xScale, yScale) * new Vector2(.022f, .738f);
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
				Vector2 anchor = new Vector2(xScale, yScale) * new Vector2(-.045f, -.75f);
				Joints[(int)BipedJointIndex.LAnkle].LocalAnchorA = Joints[(int)BipedJointIndex.RAnkle].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.LFoot].Position;
				Joints[(int)BipedJointIndex.LAnkle].LocalAnchorB = Joints[(int)BipedJointIndex.RAnkle].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.LCalf].Position;
				Joints[(int)BipedJointIndex.LAnkle].ReferenceAngle = Joints[(int)BipedJointIndex.RAnkle].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LAnkle].LowerLimit = Joints[(int)BipedJointIndex.RAnkle].LowerLimit = ReverseAngle(-0.523598776f);
				Joints[(int)BipedJointIndex.LAnkle].UpperLimit = Joints[(int)BipedJointIndex.RAnkle].UpperLimit = ReverseAngle(0.523598776f);
			}

			//b.LKneeDef.body1		= LCalf;
			//b.LKneeDef.body2		= LThigh;
			//b.RKneeDef.body1		= RCalf;
			//b.RKneeDef.body2		= RThigh;
			{	// knees
				Vector2 anchor = new Vector2(xScale, yScale) * new Vector2(-.030f, -.355f);
				Joints[(int)BipedJointIndex.LKnee].LocalAnchorA = Joints[(int)BipedJointIndex.RKnee].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.LCalf].Position;
				Joints[(int)BipedJointIndex.LKnee].LocalAnchorB = Joints[(int)BipedJointIndex.RKnee].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.LThigh].Position;
				Joints[(int)BipedJointIndex.LKnee].ReferenceAngle = Joints[(int)BipedJointIndex.RKnee].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LKnee].LowerLimit = Joints[(int)BipedJointIndex.RKnee].LowerLimit = ReverseAngle(0);
				Joints[(int)BipedJointIndex.LKnee].UpperLimit = Joints[(int)BipedJointIndex.RKnee].UpperLimit = ReverseAngle(2.61799388f);
			}

			//b.LHipDef.body1			= LThigh;
			//b.LHipDef.body2			= Pelvis;
			//b.RHipDef.body1			= RThigh;
			//b.RHipDef.body2			= Pelvis;
			{	// hips
				Vector2 anchor = new Vector2(xScale, yScale) * new Vector2(.005f, -.045f);
				Joints[(int)BipedJointIndex.LHip].LocalAnchorA = Joints[(int)BipedJointIndex.RHip].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.LThigh].Position;
				Joints[(int)BipedJointIndex.LHip].LocalAnchorB = Joints[(int)BipedJointIndex.RHip].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.Pelvis].Position;
				Joints[(int)BipedJointIndex.LHip].ReferenceAngle = Joints[(int)BipedJointIndex.RHip].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LHip].LowerLimit = Joints[(int)BipedJointIndex.RHip].LowerLimit = ReverseAngle(-0.76892803f);
				Joints[(int)BipedJointIndex.LHip].UpperLimit = Joints[(int)BipedJointIndex.RHip].UpperLimit = 0;
			}

			//b.LowerAbsDef.body1		= Pelvis;
			//b.LowerAbsDef.body2		= Stomach;
			{	// lower abs
				Vector2 anchor = new Vector2(xScale, yScale) * new Vector2(.035f, .135f);
				Joints[(int)BipedJointIndex.LowerAbs].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.Pelvis].Position;
				Joints[(int)BipedJointIndex.LowerAbs].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.Stomach].Position;
				Joints[(int)BipedJointIndex.LowerAbs].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LowerAbs].LowerLimit = ReverseAngle(-0.523598776f);
				Joints[(int)BipedJointIndex.LowerAbs].UpperLimit = ReverseAngle(0.523598776f);
			}

			//b.UpperAbsDef.body1		= Stomach;
			//b.UpperAbsDef.body2		= Chest;
			{	// upper abs
				Vector2 anchor = new Vector2(xScale, yScale) * new Vector2(.045f, .320f);
				Joints[(int)BipedJointIndex.UpperAbs].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.Stomach].Position;
				Joints[(int)BipedJointIndex.UpperAbs].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.Chest].Position;
				Joints[(int)BipedJointIndex.UpperAbs].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.UpperAbs].LowerLimit = ReverseAngle(-0.523598776f);
				Joints[(int)BipedJointIndex.UpperAbs].UpperLimit = ReverseAngle(0.174532925f);
			}

			//b.LowerNeckDef.body1	= Chest;
			//b.LowerNeckDef.body2	= Neck;
			{	// lower neck
				Vector2 anchor = new Vector2(xScale, yScale) * new Vector2(-.015f, .575f);
				Joints[(int)BipedJointIndex.LowerNeck].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.Chest].Position;
				Joints[(int)BipedJointIndex.LowerNeck].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.Neck].Position;
				Joints[(int)BipedJointIndex.LowerNeck].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LowerNeck].LowerLimit = ReverseAngle(-0.174532925f);
				Joints[(int)BipedJointIndex.LowerNeck].UpperLimit = ReverseAngle(0.174532925f);
			}

			//b.UpperNeckDef.body1	= Chest;
			//b.UpperNeckDef.body2	= Head;
			{	// upper neck
				Vector2 anchor = new Vector2(xScale, yScale) * new Vector2(-.005f, .630f);
				Joints[(int)BipedJointIndex.UpperNeck].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.Chest].Position;
				Joints[(int)BipedJointIndex.UpperNeck].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.Head].Position;
				Joints[(int)BipedJointIndex.UpperNeck].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.UpperNeck].LowerLimit = ReverseAngle(-0.610865238f);
				Joints[(int)BipedJointIndex.UpperNeck].UpperLimit = ReverseAngle(0.785398163f);
			}

			//b.LShoulderDef.body1	= Chest;
			//b.LShoulderDef.body2	= LUpperArm;
			//b.RShoulderDef.body1	= Chest;
			//b.RShoulderDef.body2	= RUpperArm;
			{	// shoulders
				Vector2 anchor = new Vector2(xScale, yScale) * new Vector2(-.015f, .545f);
				Joints[(int)BipedJointIndex.LShoulder].LocalAnchorA = Joints[(int)BipedJointIndex.RShoulder].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.Chest].Position;
				Joints[(int)BipedJointIndex.LShoulder].LocalAnchorB = Joints[(int)BipedJointIndex.RShoulder].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.LUpperArm].Position;
				Joints[(int)BipedJointIndex.LShoulder].ReferenceAngle = Joints[(int)BipedJointIndex.RShoulder].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LShoulder].LowerLimit = Joints[(int)BipedJointIndex.RShoulder].LowerLimit = ReverseAngle(-1.04719755f);
				Joints[(int)BipedJointIndex.LShoulder].UpperLimit = Joints[(int)BipedJointIndex.RShoulder].UpperLimit = ReverseAngle(3.14159265f);
			}

			//b.LElbowDef.body1		= LForearm;
			//b.LElbowDef.body2		= LUpperArm;
			//b.RElbowDef.body1		= RForearm;
			//b.RElbowDef.body2		= RUpperArm;
			{	// elbows
				Vector2 anchor = new Vector2(xScale, yScale) * new Vector2(-.005f, .290f);
				Joints[(int)BipedJointIndex.LElbow].LocalAnchorA = Joints[(int)BipedJointIndex.RElbow].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.LForearm].Position;
				Joints[(int)BipedJointIndex.LElbow].LocalAnchorB = Joints[(int)BipedJointIndex.RElbow].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.LUpperArm].Position;
				Joints[(int)BipedJointIndex.LElbow].ReferenceAngle = Joints[(int)BipedJointIndex.RElbow].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LElbow].LowerLimit = Joints[(int)BipedJointIndex.RElbow].LowerLimit = ReverseAngle(-2.7925268f);
				Joints[(int)BipedJointIndex.LElbow].UpperLimit = Joints[(int)BipedJointIndex.RElbow].UpperLimit = 0;
			}

			//b.LWristDef.body1		= LHand;
			//b.LWristDef.body2		= LForearm;
			//b.RWristDef.body1		= RHand;
			//b.RWristDef.body2		= RForearm;
			{	// wrists
				Vector2 anchor = new Vector2(xScale, yScale) * new Vector2(-.010f, .045f);
				Joints[(int)BipedJointIndex.LWrist].LocalAnchorA = Joints[(int)BipedJointIndex.RWrist].LocalAnchorA = anchor - Bodies[(int)BipedFixtureIndex.LHand].Position;
				Joints[(int)BipedJointIndex.LWrist].LocalAnchorB = Joints[(int)BipedJointIndex.RWrist].LocalAnchorB = anchor - Bodies[(int)BipedFixtureIndex.LForearm].Position;
				Joints[(int)BipedJointIndex.LWrist].ReferenceAngle = Joints[(int)BipedJointIndex.RWrist].ReferenceAngle = 0.0f;
				Joints[(int)BipedJointIndex.LWrist].LowerLimit = Joints[(int)BipedJointIndex.RWrist].LowerLimit = ReverseAngle(-0.174532925f);
				Joints[(int)BipedJointIndex.LWrist].UpperLimit = Joints[(int)BipedJointIndex.RWrist].UpperLimit = ReverseAngle(0.174532925f);
			}

			{
				RopeJoint rj = new RopeJoint(Bodies[(int)BipedFixtureIndex.RUpperArm], Bodies[(int)BipedFixtureIndex.RHand], Vector2.Zero, Vector2.Zero);
				rj = new RopeJoint(Bodies[(int)BipedFixtureIndex.LUpperArm], Bodies[(int)BipedFixtureIndex.LHand], Vector2.Zero, Vector2.Zero);
			}

			if (xScale < 0)
				foreach (var j in Joints)
				{
					var old = j.UpperLimit;
					j.UpperLimit = j.LowerLimit;
					j.LowerLimit = old;
				}
		}
	};

	public struct BipedModel
	{
		Shape[] _shapes;

		public Shape this[BipedFixtureIndex index]
		{
			get
			{
				switch (index)
				{
				case BipedFixtureIndex.LCalf:
					return _shapes[(int)BipedFixtureIndex.RCalf];
				case BipedFixtureIndex.LFoot:
					return _shapes[(int)BipedFixtureIndex.RFoot];
				case BipedFixtureIndex.LForearm:
					return _shapes[(int)BipedFixtureIndex.RForearm];
				case BipedFixtureIndex.LHand:
					return _shapes[(int)BipedFixtureIndex.RHand];
				case BipedFixtureIndex.LThigh:
					return _shapes[(int)BipedFixtureIndex.RThigh];
				case BipedFixtureIndex.LUpperArm:
					return _shapes[(int)BipedFixtureIndex.RUpperArm];
				}
				return _shapes[(int)index];
			}
		}

		public BipedModel(float xScale, float yScale)
		{
			_shapes = new Shape[(int)BipedFixtureIndex.RHand + 1];

			_shapes[(int)BipedFixtureIndex.RFoot] = BipedDef.ReverseShape
				(
					new Vector2(xScale, yScale) * new Vector2(.033f, .143f),
					new Vector2(xScale, yScale) * new Vector2(.023f, .033f),
					new Vector2(xScale, yScale) * new Vector2(.267f, .035f),
					new Vector2(xScale, yScale) * new Vector2(.265f, .065f),
					new Vector2(xScale, yScale) * new Vector2(.117f, .143f)
				);

			_shapes[(int)BipedFixtureIndex.RCalf] = BipedDef.ReverseShape
				(
					new Vector2(xScale, yScale) * new Vector2(.089f, .016f),
					new Vector2(xScale, yScale) * new Vector2(.178f, .016f),
					new Vector2(xScale, yScale) * new Vector2(.205f, .417f),
					new Vector2(xScale, yScale) * new Vector2(.095f, .417f)
				);

			_shapes[(int)BipedFixtureIndex.RThigh] = BipedDef.ReverseShape
				(
					new Vector2(xScale, yScale) * new Vector2(.137f, .032f),
					new Vector2(xScale, yScale) * new Vector2(.243f, .032f),
					new Vector2(xScale, yScale) * new Vector2(.318f, .343f),
					new Vector2(xScale, yScale) * new Vector2(.142f, .343f)
				);

			_shapes[(int)BipedFixtureIndex.Pelvis] = BipedDef.ReverseShape
				(
					new Vector2(xScale, yScale) * new Vector2(.105f, .051f),
					new Vector2(xScale, yScale) * new Vector2(.277f, .053f),
					new Vector2(xScale, yScale) * new Vector2(.320f, .233f),
					new Vector2(xScale, yScale) * new Vector2(.112f, .233f),
					new Vector2(xScale, yScale) * new Vector2(.067f, .152f)
				);

			_shapes[(int)BipedFixtureIndex.Stomach] = BipedDef.ReverseShape
				(
					new Vector2(xScale, yScale) * new Vector2(.088f, .043f),
					new Vector2(xScale, yScale) * new Vector2(.284f, .043f),
					new Vector2(xScale, yScale) * new Vector2(.295f, .231f),
					new Vector2(xScale, yScale) * new Vector2(.100f, .231f)
				);

			_shapes[(int)BipedFixtureIndex.Chest] = BipedDef.ReverseShape
				(
					new Vector2(xScale, yScale) * new Vector2(.091f, .042f),
					new Vector2(xScale, yScale) * new Vector2(.283f, .042f),
					new Vector2(xScale, yScale) * new Vector2(.177f, .289f),
					new Vector2(xScale, yScale) * new Vector2(.065f, .289f)
				);

			_shapes[(int)BipedFixtureIndex.Neck] = BipedDef.ReverseShape
				(
					new Vector2(xScale, yScale) * new Vector2(.038f, .054f),
					new Vector2(xScale, yScale) * new Vector2(.149f, .054f),
					new Vector2(xScale, yScale) * new Vector2(.154f, .102f),
					new Vector2(xScale, yScale) * new Vector2(.054f, .113f)
				);

			_shapes[(int)BipedFixtureIndex.Head] = new CircleShape(yScale * .115f, 4.0f);
			
			_shapes[(int)BipedFixtureIndex.RUpperArm] = BipedDef.ReverseShape
				(
					new Vector2(xScale, yScale) * new Vector2(.092f, .059f),
					new Vector2(xScale, yScale) * new Vector2(.159f, .059f),
					new Vector2(xScale, yScale) * new Vector2(.169f, .335f),
					new Vector2(xScale, yScale) * new Vector2(.078f, .335f),
					new Vector2(xScale, yScale) * new Vector2(.064f, .248f)
				);

			_shapes[(int)BipedFixtureIndex.RForearm] = BipedDef.ReverseShape
				(
					new Vector2(xScale, yScale) * new Vector2(.082f, .054f),
					new Vector2(xScale, yScale) * new Vector2(.138f, .054f),
					new Vector2(xScale, yScale) * new Vector2(.149f, .296f),
					new Vector2(xScale, yScale) * new Vector2(.088f, .296f)
				);

			_shapes[(int)BipedFixtureIndex.RHand] = BipedDef.ReverseShape
				(
					new Vector2(xScale, yScale) * new Vector2(.066f, .031f),
					new Vector2(xScale, yScale) * new Vector2(.123f, .020f),
					new Vector2(xScale, yScale) * new Vector2(.160f, .127f),
					new Vector2(xScale, yScale) * new Vector2(.127f, .178f),
					new Vector2(xScale, yScale) * new Vector2(.074f, .178f)
				);
		}
	}
}
