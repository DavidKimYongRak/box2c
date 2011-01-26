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

	public class Player
	{
	}

	public class Biped
	{
		Player _ownedPlayer;

		public Player OwnedPlayer
		{
			get { return _ownedPlayer; }
		}

		public Body[] Bodies;

		public Biped(Player ownedPlayer, World world, Vector2 position, float xScale, float yScale)
		{
			_ownedPlayer = ownedPlayer;
			m_world = world;

			BipedDef def = new BipedDef(world, xScale, yScale);

			for (int i = 0; i < def.Bodies.Length; ++i)
			{
				def.Bodies[i].Position += position;
				def.Bodies[i].UserData = new BipedBodyDescriptor((BipedFixtureIndex)i, this);
			}

			Bodies = def.Bodies;
			Joints = def.Joints;

			// link body parts
			def.Joints[(int)BipedJointIndex.LAnkle].BodyA = def.Bodies[(int)BipedFixtureIndex.LFoot];
			def.Joints[(int)BipedJointIndex.LAnkle].BodyB = def.Bodies[(int)BipedFixtureIndex.LCalf];
			def.Joints[(int)BipedJointIndex.RAnkle].BodyA = def.Bodies[(int)BipedFixtureIndex.RFoot];
			def.Joints[(int)BipedJointIndex.RAnkle].BodyB = def.Bodies[(int)BipedFixtureIndex.RCalf];
			def.Joints[(int)BipedJointIndex.LKnee].BodyA = def.Bodies[(int)BipedFixtureIndex.LCalf];
			def.Joints[(int)BipedJointIndex.LKnee].BodyB = def.Bodies[(int)BipedFixtureIndex.LThigh];
			def.Joints[(int)BipedJointIndex.RKnee].BodyA = def.Bodies[(int)BipedFixtureIndex.RCalf];
			def.Joints[(int)BipedJointIndex.RKnee].BodyB = def.Bodies[(int)BipedFixtureIndex.RThigh];
			def.Joints[(int)BipedJointIndex.LHip].BodyA = def.Bodies[(int)BipedFixtureIndex.LThigh];
			def.Joints[(int)BipedJointIndex.LHip].BodyB = def.Bodies[(int)BipedFixtureIndex.Pelvis];
			def.Joints[(int)BipedJointIndex.RHip].BodyA = def.Bodies[(int)BipedFixtureIndex.RThigh];
			def.Joints[(int)BipedJointIndex.RHip].BodyB = def.Bodies[(int)BipedFixtureIndex.Pelvis];
			def.Joints[(int)BipedJointIndex.LowerAbs].BodyA = def.Bodies[(int)BipedFixtureIndex.Pelvis];
			def.Joints[(int)BipedJointIndex.LowerAbs].BodyB = def.Bodies[(int)BipedFixtureIndex.Stomach];
			def.Joints[(int)BipedJointIndex.UpperAbs].BodyA = def.Bodies[(int)BipedFixtureIndex.Stomach];
			def.Joints[(int)BipedJointIndex.UpperAbs].BodyB = def.Bodies[(int)BipedFixtureIndex.Chest];
			def.Joints[(int)BipedJointIndex.LowerNeck].BodyA = def.Bodies[(int)BipedFixtureIndex.Chest];
			def.Joints[(int)BipedJointIndex.LowerNeck].BodyB = def.Bodies[(int)BipedFixtureIndex.Neck];
			def.Joints[(int)BipedJointIndex.UpperNeck].BodyA = def.Bodies[(int)BipedFixtureIndex.Chest];
			def.Joints[(int)BipedJointIndex.UpperNeck].BodyB = def.Bodies[(int)BipedFixtureIndex.Head];
			def.Joints[(int)BipedJointIndex.LShoulder].BodyA = def.Bodies[(int)BipedFixtureIndex.Chest];
			def.Joints[(int)BipedJointIndex.LShoulder].BodyB = def.Bodies[(int)BipedFixtureIndex.LUpperArm];
			def.Joints[(int)BipedJointIndex.RShoulder].BodyA = def.Bodies[(int)BipedFixtureIndex.Chest];
			def.Joints[(int)BipedJointIndex.RShoulder].BodyB = def.Bodies[(int)BipedFixtureIndex.RUpperArm];
			def.Joints[(int)BipedJointIndex.LElbow].BodyA = def.Bodies[(int)BipedFixtureIndex.LForearm];
			def.Joints[(int)BipedJointIndex.LElbow].BodyB = def.Bodies[(int)BipedFixtureIndex.LUpperArm];
			def.Joints[(int)BipedJointIndex.RElbow].BodyA = def.Bodies[(int)BipedFixtureIndex.RForearm];
			def.Joints[(int)BipedJointIndex.RElbow].BodyB = def.Bodies[(int)BipedFixtureIndex.RUpperArm];
			def.Joints[(int)BipedJointIndex.LWrist].BodyA = def.Bodies[(int)BipedFixtureIndex.LHand];
			def.Joints[(int)BipedJointIndex.LWrist].BodyB = def.Bodies[(int)BipedFixtureIndex.LForearm];
			def.Joints[(int)BipedJointIndex.RWrist].BodyA = def.Bodies[(int)BipedFixtureIndex.RHand];
			def.Joints[(int)BipedJointIndex.RWrist].BodyB = def.Bodies[(int)BipedFixtureIndex.RForearm];

			// create joints
			world.AddJoint(def.Joints[(int)BipedJointIndex.LAnkle]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.RAnkle]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.LKnee]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.RKnee]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.LHip]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.RHip]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.LowerAbs]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.UpperAbs]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.LowerNeck]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.UpperNeck]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.LShoulder]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.RShoulder]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.LElbow]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.RElbow]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.LWrist]);
			world.AddJoint(def.Joints[(int)BipedJointIndex.RWrist]);
		}

		public void Disable()
		{
			foreach (var j in Joints)
			{
				j.UserData = new System.Drawing.PointF(j.LowerLimit, j.UpperLimit);
				j.LowerLimit = j.UpperLimit = j.BodyB.Rotation - j.BodyA.Rotation;
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
