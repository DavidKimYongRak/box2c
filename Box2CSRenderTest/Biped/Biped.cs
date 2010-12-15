using System;
using Box2CS;

namespace Box2DSharpRenderTest
{
	public class Biped
	{
		static void BodyFromFixture (EBipedFixtureIndex type,
			Biped biped, BipedDef def, Vec2 position, World world)
		{
			var bd = def.Bodies[(int)type];
			bd.Position += position;
			biped.Bodies[(int)type] = world.CreateBody(bd);
			biped.Bodies[(int)type].CreateFixture(def.Fixtures[(int)type]);
		}

		public Biped(World world, Vec2 position, float xScale, float yScale)
		{
			m_world = world;

			BipedDef def = new BipedDef(xScale, yScale);
			BodyDef bd = new BodyDef();

			// create body parts
			for (EBipedFixtureIndex i = 0; i < EBipedFixtureIndex.Max; ++i)
				BodyFromFixture(i, this, def, position, world);

			// link body parts
			def.Joints[(int)EBipedJointIndex.LAnkle].BodyA = Bodies[(int)EBipedFixtureIndex.LFoot];
			def.Joints[(int)EBipedJointIndex.LAnkle].BodyB = Bodies[(int)EBipedFixtureIndex.LCalf];
			def.Joints[(int)EBipedJointIndex.RAnkle].BodyA = Bodies[(int)EBipedFixtureIndex.RFoot];
			def.Joints[(int)EBipedJointIndex.RAnkle].BodyB = Bodies[(int)EBipedFixtureIndex.RCalf];
			def.Joints[(int)EBipedJointIndex.LKnee].BodyA = Bodies[(int)EBipedFixtureIndex.LCalf];
			def.Joints[(int)EBipedJointIndex.LKnee].BodyB = Bodies[(int)EBipedFixtureIndex.LThigh];
			def.Joints[(int)EBipedJointIndex.RKnee].BodyA = Bodies[(int)EBipedFixtureIndex.RCalf];
			def.Joints[(int)EBipedJointIndex.RKnee].BodyB = Bodies[(int)EBipedFixtureIndex.RThigh];
			def.Joints[(int)EBipedJointIndex.LHip].BodyA = Bodies[(int)EBipedFixtureIndex.LThigh];
			def.Joints[(int)EBipedJointIndex.LHip].BodyB = Bodies[(int)EBipedFixtureIndex.Pelvis];
			def.Joints[(int)EBipedJointIndex.RHip].BodyA = Bodies[(int)EBipedFixtureIndex.RThigh];
			def.Joints[(int)EBipedJointIndex.RHip].BodyB = Bodies[(int)EBipedFixtureIndex.Pelvis];
			def.Joints[(int)EBipedJointIndex.LowerAbs].BodyA = Bodies[(int)EBipedFixtureIndex.Pelvis];
			def.Joints[(int)EBipedJointIndex.LowerAbs].BodyB = Bodies[(int)EBipedFixtureIndex.Stomach];
			def.Joints[(int)EBipedJointIndex.UpperAbs].BodyA = Bodies[(int)EBipedFixtureIndex.Stomach];
			def.Joints[(int)EBipedJointIndex.UpperAbs].BodyB = Bodies[(int)EBipedFixtureIndex.Chest];
			def.Joints[(int)EBipedJointIndex.LowerNeck].BodyA = Bodies[(int)EBipedFixtureIndex.Chest];
			def.Joints[(int)EBipedJointIndex.LowerNeck].BodyB = Bodies[(int)EBipedFixtureIndex.Neck];
			def.Joints[(int)EBipedJointIndex.UpperNeck].BodyA = Bodies[(int)EBipedFixtureIndex.Chest];
			def.Joints[(int)EBipedJointIndex.UpperNeck].BodyB = Bodies[(int)EBipedFixtureIndex.Head];
			def.Joints[(int)EBipedJointIndex.LShoulder].BodyA = Bodies[(int)EBipedFixtureIndex.Chest];
			def.Joints[(int)EBipedJointIndex.LShoulder].BodyB = Bodies[(int)EBipedFixtureIndex.LUpperArm];
			def.Joints[(int)EBipedJointIndex.RShoulder].BodyA = Bodies[(int)EBipedFixtureIndex.Chest];
			def.Joints[(int)EBipedJointIndex.RShoulder].BodyB = Bodies[(int)EBipedFixtureIndex.RUpperArm];
			def.Joints[(int)EBipedJointIndex.LElbow].BodyA = Bodies[(int)EBipedFixtureIndex.LForearm];
			def.Joints[(int)EBipedJointIndex.LElbow].BodyB = Bodies[(int)EBipedFixtureIndex.LUpperArm];
			def.Joints[(int)EBipedJointIndex.RElbow].BodyA = Bodies[(int)EBipedFixtureIndex.RForearm];
			def.Joints[(int)EBipedJointIndex.RElbow].BodyB = Bodies[(int)EBipedFixtureIndex.RUpperArm];
			def.Joints[(int)EBipedJointIndex.LWrist].BodyA = Bodies[(int)EBipedFixtureIndex.LHand];
			def.Joints[(int)EBipedJointIndex.LWrist].BodyB = Bodies[(int)EBipedFixtureIndex.LForearm];
			def.Joints[(int)EBipedJointIndex.RWrist].BodyA = Bodies[(int)EBipedFixtureIndex.RHand];
			def.Joints[(int)EBipedJointIndex.RWrist].BodyB = Bodies[(int)EBipedFixtureIndex.RForearm];

			// create joints
			Joints[(int)EBipedJointIndex.LAnkle] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.LAnkle]);
			Joints[(int)EBipedJointIndex.RAnkle] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.RAnkle]);
			Joints[(int)EBipedJointIndex.LKnee] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.LKnee]);
			Joints[(int)EBipedJointIndex.RKnee] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.RKnee]);
			Joints[(int)EBipedJointIndex.LHip] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.LHip]);
			Joints[(int)EBipedJointIndex.RHip] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.RHip]);
			Joints[(int)EBipedJointIndex.LowerAbs] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.LowerAbs]);
			Joints[(int)EBipedJointIndex.UpperAbs] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.UpperAbs]);
			Joints[(int)EBipedJointIndex.LowerNeck] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.LowerNeck]);
			Joints[(int)EBipedJointIndex.UpperNeck] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.UpperNeck]);
			Joints[(int)EBipedJointIndex.LShoulder] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.LShoulder]);
			Joints[(int)EBipedJointIndex.RShoulder] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.RShoulder]);
			Joints[(int)EBipedJointIndex.LElbow] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.LElbow]);
			Joints[(int)EBipedJointIndex.RElbow] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.RElbow]);
			Joints[(int)EBipedJointIndex.LWrist] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.LWrist]);
			Joints[(int)EBipedJointIndex.RWrist] = (RevoluteJoint)world.CreateJoint(def.Joints[(int)EBipedJointIndex.RWrist]);
		}

		World m_world;

		public Body[]	Bodies = new Body[(int)EBipedFixtureIndex.Max]; 
		public RevoluteJoint[]	Joints = new RevoluteJoint[(int)EBipedJointIndex.Max]; 
	}
}
