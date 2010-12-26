using System;
using Box2CS;

namespace Box2DSharpRenderTest
{
	public struct BipedBodyDescriptor
	{
		public BipedFixtureIndex FixtureIndex;
		public Biped Biped;

		public BipedBodyDescriptor(BipedFixtureIndex index, Biped biped)
		{
			FixtureIndex = index;
			Biped = biped;
		}
	}

	public class Biped
	{
		Form2.Player _ownedPlayer;

		public Form2.Player OwnedPlayer
		{
			get { return _ownedPlayer; }
		}

		static void BodyFromFixture (BipedFixtureIndex type,
			Biped biped, BipedDef def, Vec2 position, World world)
		{
			var bd = def.Bodies[(int)type];
			bd.Position += position;
			biped.Bodies[(int)type] = world.CreateBody(bd);
			biped.Bodies[(int)type].CreateFixture(def.Fixtures[(int)type]);
			biped.Bodies[(int)type].UserData = new BipedBodyDescriptor(type, biped);
		}

		public Biped(Form2.Player ownedPlayer, World world, Vec2 position, float xScale, float yScale)
		{
			_ownedPlayer = ownedPlayer;
			m_world = world;

			BipedDef def = new BipedDef(xScale, yScale);
			BodyDef bd = new BodyDef();

			// create body parts
			for (BipedFixtureIndex i = 0; i < BipedFixtureIndex.Max; ++i)
				BodyFromFixture(i, this, def, position, world);

			// link body parts
			def.Joints[(int)BipedJointIndex.LAnkle].BodyA = Bodies[(int)BipedFixtureIndex.LFoot];
			def.Joints[(int)BipedJointIndex.LAnkle].BodyB = Bodies[(int)BipedFixtureIndex.LCalf];
			def.Joints[(int)BipedJointIndex.RAnkle].BodyA = Bodies[(int)BipedFixtureIndex.RFoot];
			def.Joints[(int)BipedJointIndex.RAnkle].BodyB = Bodies[(int)BipedFixtureIndex.RCalf];
			def.Joints[(int)BipedJointIndex.LKnee].BodyA = Bodies[(int)BipedFixtureIndex.LCalf];
			def.Joints[(int)BipedJointIndex.LKnee].BodyB = Bodies[(int)BipedFixtureIndex.LThigh];
			def.Joints[(int)BipedJointIndex.RKnee].BodyA = Bodies[(int)BipedFixtureIndex.RCalf];
			def.Joints[(int)BipedJointIndex.RKnee].BodyB = Bodies[(int)BipedFixtureIndex.RThigh];
			def.Joints[(int)BipedJointIndex.LHip].BodyA = Bodies[(int)BipedFixtureIndex.LThigh];
			def.Joints[(int)BipedJointIndex.LHip].BodyB = Bodies[(int)BipedFixtureIndex.Pelvis];
			def.Joints[(int)BipedJointIndex.RHip].BodyA = Bodies[(int)BipedFixtureIndex.RThigh];
			def.Joints[(int)BipedJointIndex.RHip].BodyB = Bodies[(int)BipedFixtureIndex.Pelvis];
			def.Joints[(int)BipedJointIndex.LowerAbs].BodyA = Bodies[(int)BipedFixtureIndex.Pelvis];
			def.Joints[(int)BipedJointIndex.LowerAbs].BodyB = Bodies[(int)BipedFixtureIndex.Stomach];
			def.Joints[(int)BipedJointIndex.UpperAbs].BodyA = Bodies[(int)BipedFixtureIndex.Stomach];
			def.Joints[(int)BipedJointIndex.UpperAbs].BodyB = Bodies[(int)BipedFixtureIndex.Chest];
			def.Joints[(int)BipedJointIndex.LowerNeck].BodyA = Bodies[(int)BipedFixtureIndex.Chest];
			def.Joints[(int)BipedJointIndex.LowerNeck].BodyB = Bodies[(int)BipedFixtureIndex.Neck];
			def.Joints[(int)BipedJointIndex.UpperNeck].BodyA = Bodies[(int)BipedFixtureIndex.Chest];
			def.Joints[(int)BipedJointIndex.UpperNeck].BodyB = Bodies[(int)BipedFixtureIndex.Head];
			def.Joints[(int)BipedJointIndex.LShoulder].BodyA = Bodies[(int)BipedFixtureIndex.Chest];
			def.Joints[(int)BipedJointIndex.LShoulder].BodyB = Bodies[(int)BipedFixtureIndex.LUpperArm];
			def.Joints[(int)BipedJointIndex.RShoulder].BodyA = Bodies[(int)BipedFixtureIndex.Chest];
			def.Joints[(int)BipedJointIndex.RShoulder].BodyB = Bodies[(int)BipedFixtureIndex.RUpperArm];
			def.Joints[(int)BipedJointIndex.LElbow].BodyA = Bodies[(int)BipedFixtureIndex.LForearm];
			def.Joints[(int)BipedJointIndex.LElbow].BodyB = Bodies[(int)BipedFixtureIndex.LUpperArm];
			def.Joints[(int)BipedJointIndex.RElbow].BodyA = Bodies[(int)BipedFixtureIndex.RForearm];
			def.Joints[(int)BipedJointIndex.RElbow].BodyB = Bodies[(int)BipedFixtureIndex.RUpperArm];
			def.Joints[(int)BipedJointIndex.LWrist].BodyA = Bodies[(int)BipedFixtureIndex.LHand];
			def.Joints[(int)BipedJointIndex.LWrist].BodyB = Bodies[(int)BipedFixtureIndex.LForearm];
			def.Joints[(int)BipedJointIndex.RWrist].BodyA = Bodies[(int)BipedFixtureIndex.RHand];
			def.Joints[(int)BipedJointIndex.RWrist].BodyB = Bodies[(int)BipedFixtureIndex.RForearm];

			// create joints
			Joints[(int)BipedJointIndex.LAnkle] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.LAnkle]);
			Joints[(int)BipedJointIndex.RAnkle] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.RAnkle]);
			Joints[(int)BipedJointIndex.LKnee] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.LKnee]);
			Joints[(int)BipedJointIndex.RKnee] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.RKnee]);
			Joints[(int)BipedJointIndex.LHip] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.LHip]);
			Joints[(int)BipedJointIndex.RHip] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.RHip]);
			Joints[(int)BipedJointIndex.LowerAbs] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.LowerAbs]);
			Joints[(int)BipedJointIndex.UpperAbs] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.UpperAbs]);
			Joints[(int)BipedJointIndex.LowerNeck] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.LowerNeck]);
			Joints[(int)BipedJointIndex.UpperNeck] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.UpperNeck]);
			Joints[(int)BipedJointIndex.LShoulder] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.LShoulder]);
			Joints[(int)BipedJointIndex.RShoulder] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.RShoulder]);
			Joints[(int)BipedJointIndex.LElbow] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.LElbow]);
			Joints[(int)BipedJointIndex.RElbow] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.RElbow]);
			Joints[(int)BipedJointIndex.LWrist] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.LWrist]);
			Joints[(int)BipedJointIndex.RWrist] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)BipedJointIndex.RWrist]);
		}

		public void Disable()
		{
			foreach (var j in Joints)
			{
				j.UserData = new System.Drawing.PointF(j.LowerLimit, j.UpperLimit);
				j.LowerLimit = j.UpperLimit = j.BodyB.Angle - j.BodyA.Angle;
			}
		}

		public void Enable()
		{
			foreach (var j in Joints)
			{
				var pf = (System.Drawing.PointF)j.UserData;

				j.LowerLimit = pf.X;
				j.UpperLimit = pf.Y;
				j.UserData = null;
			}
		}

		World m_world;

		public Body[] Bodies = new Body[(int)BipedFixtureIndex.Max]; 
		public RevoluteJoint[] Joints = new RevoluteJoint[(int)BipedJointIndex.Max];
		bool stuck;

		public void StickBody()
		{
			stuck = !stuck;

			if (stuck)
				Disable();
			else
				Enable();
		}
	}
}
