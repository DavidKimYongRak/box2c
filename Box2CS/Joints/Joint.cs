using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	public enum EJointType
	{
		e_unknownJoint,
		e_revoluteJoint,
		e_prismaticJoint,
		e_distanceJoint,
		e_pulleyJoint,
		e_mouseJoint,
		e_gearJoint,
		e_lineJoint,
		e_weldJoint,
		e_frictionJoint,
	};

#if !NEW_JOINTS
	[StructLayout(LayoutKind.Sequential)]
	public abstract class JointDef : IFixedSize
	{
		EJointType _type;
		IntPtr _userData;
		IntPtr _bodyA;
		IntPtr _bodyB;
		bool _collideConnected;

		int IFixedSize.FixedSize()
		{
			throw new Exception();
		}
		
		void IFixedSize.Lock()
		{
		}

		void IFixedSize.Unlock()
		{
		}

		public JointDef()
		{
			_type = EJointType.e_unknownJoint;
			_userData = IntPtr.Zero;
			_bodyA = IntPtr.Zero;
			_bodyB = IntPtr.Zero;
			_collideConnected = false;
		}

		public EJointType JointType
		{
			get { return _type; }
			protected set { _type = value; }
		}

		public object UserData
		{
			get
			{
				return UserDataStorage.JointStorage.ObjectFromHandle(UserDataStorage.IntPtrToHandle(_userData));
			}

			set
			{
				var ptr = UserDataStorage.IntPtrToHandle(_userData);

				if (ptr != 0)
					UserDataStorage.JointStorage.UnpinObject(ptr);

				if (value != null)
				{
					var handle = UserDataStorage.JointStorage.PinDataToHandle(value);
					_userData = UserDataStorage.HandleToIntPtr(handle);
				}
				else
					_userData = IntPtr.Zero;
			}
		}

		public Body BodyA
		{
			get { return Body.FromPtr(_bodyA); }
			set { _bodyA = value.BodyPtr; }
		}

		public Body BodyB
		{
			get { return Body.FromPtr(_bodyB); }
			set { _bodyB = value.BodyPtr; }
		}

		public bool CollideConnected
		{
			get { return _collideConnected; }
			set { _collideConnected = value; }
		}
	}
#else
	public abstract class JointDef
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern int b2jointdef_gettype(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2jointdef_getuserdata(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2jointdef_setuserdata(IntPtr jointDef, IntPtr data);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2jointdef_getbodya(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2jointdef_setbodya(IntPtr jointDef, IntPtr data);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2jointdef_getbodyb(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2jointdef_setbodyb(IntPtr jointDef, IntPtr data);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2jointdef_getcollideconnected(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2jointdef_setcollideconnected(IntPtr jointDef, bool data);
		}

		IntPtr _jointDefPtr;

		internal IntPtr JointDefPtr
		{
			get { return _jointDefPtr; }
			set { _jointDefPtr = value; }
		}

		public EJointType EJointType
		{
			get { return (EJointType)NativeMethods.b2jointdef_gettype(_jointDefPtr); }
		}

		public object UserData
		{
			get
			{
				return UserDataStorage.JointStorage.ObjectFromHandle(UserDataStorage.IntPtrToHandle(NativeMethods.b2jointdef_getuserdata(_jointDefPtr)));
			}

			set
			{
				var ptr = UserDataStorage.IntPtrToHandle(NativeMethods.b2jointdef_getuserdata(_jointDefPtr));

				if (ptr != 0)
					UserDataStorage.JointStorage.UnpinObject(ptr);

				if (value != null)
				{
					var handle = UserDataStorage.JointStorage.PinDataToHandle(value);
					NativeMethods.b2jointdef_setuserdata(_jointDefPtr, UserDataStorage.HandleToIntPtr(handle));
				}
				else
					NativeMethods.b2jointdef_setuserdata(_jointDefPtr, IntPtr.Zero);
			}
		}

		public Body BodyA
		{
			get { return Body.FromPtr(NativeMethods.b2jointdef_getbodya(_jointDefPtr)); }
			set { NativeMethods.b2jointdef_setbodya(_jointDefPtr, value.BodyPtr); }
		}

		public Body BodyB
		{
			get { return Body.FromPtr(NativeMethods.b2jointdef_getbodyb(_jointDefPtr)); }
			set { NativeMethods.b2jointdef_setbodyb(_jointDefPtr, value.BodyPtr); }
		}

		public bool CollideConnected
		{
			get { return NativeMethods.b2jointdef_getcollideconnected(_jointDefPtr); }
			set { NativeMethods.b2jointdef_setcollideconnected(_jointDefPtr, value); }
		}

		public static bool operator ==(JointDef l, JointDef r)
		{
			if ((object)l == null && (object)r == null)
				return true;
			else if ((object)l == null && (object)r != null ||
				(object)l != null && (object)r == null)
				return false;

			return l._jointDefPtr == r._jointDefPtr;
		}

		public static bool operator !=(JointDef l, JointDef r)
		{
			return !(l == r);
		}

		public override bool Equals(object obj)
		{
			if (obj is JointDef)
				return (obj as JointDef) == this;

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return _jointDefPtr.GetHashCode();
		}
	}
#endif

	public abstract class Joint
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern int b2joint_gettype(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2joint_getbodya(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2joint_getbodyb(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2joint_getanchora(IntPtr joint, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2joint_getanchorb(IntPtr joint, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2joint_getreactionforce(IntPtr joint, float inv_dt, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2joint_getreactiontorque(IntPtr joint, float inv_dt);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2joint_getnext(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2joint_getuserdata(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2joint_setuserdata(IntPtr joint, IntPtr data);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2joint_getisactive(IntPtr joint);
		}

		IntPtr _jointPtr;

		internal IntPtr JointPtr
		{
			get { return _jointPtr; }
		}

		internal Joint(IntPtr ptr)
		{
			_jointPtr = ptr;
		}

		internal static Joint FromPtr(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
				throw new Exception("Invalid joint ptr (locked world?)");

			switch ((EJointType)NativeMethods.b2joint_gettype(ptr))
			{
			case EJointType.e_distanceJoint:
				return new DistanceJoint(ptr);
			case EJointType.e_frictionJoint:
				return new FrictionJoint(ptr);
			case EJointType.e_gearJoint:
				return new GearJoint(ptr);
			case EJointType.e_lineJoint:
				return new LineJoint(ptr);
			case EJointType.e_mouseJoint:
				return new MouseJoint(ptr);
			case EJointType.e_prismaticJoint:
				return new PrismaticJoint(ptr);
			case EJointType.e_pulleyJoint:
				return new PulleyJoint(ptr);
			case EJointType.e_revoluteJoint:
				return new RevoluteJoint(ptr);
			case EJointType.e_weldJoint:
				return new WeldJoint(ptr);
			}

			return null;
		}

		public Vec2 GetReactionForce(float invDt)
		{
			Vec2 temp;
			NativeMethods.b2joint_getreactionforce(_jointPtr, invDt, out temp);
			return temp;
		}

		public float GetReactionTorque(float invDt)
		{
			return NativeMethods.b2joint_getreactiontorque(_jointPtr, invDt);
		}

		public EJointType JointType
		{
			get { return (EJointType)NativeMethods.b2joint_gettype(_jointPtr); }
		}

		public Body BodyA
		{
			get { return Body.FromPtr(NativeMethods.b2joint_getbodya(_jointPtr)); }
		}

		public Body BodyB
		{
			get { return Body.FromPtr(NativeMethods.b2joint_getbodyb(_jointPtr)); }
		}

		public Vec2 AnchorA
		{
			get { Vec2 temp; NativeMethods.b2joint_getanchora(_jointPtr, out temp); return temp; }
		}

		public Vec2 AnchorB
		{
			get { Vec2 temp; NativeMethods.b2joint_getanchorb(_jointPtr, out temp); return temp; }
		}

		public Joint Next
		{
			get { return Joint.FromPtr(NativeMethods.b2joint_getnext(_jointPtr)); }
		}

		public bool IsActive
		{
			get { return NativeMethods.b2joint_getisactive(_jointPtr); }
		}

		public object UserData
		{
			get
			{
				return UserDataStorage.JointStorage.ObjectFromHandle(UserDataStorage.IntPtrToHandle(NativeMethods.b2joint_getuserdata(_jointPtr)));
			}

			set
			{
				var ptr = UserDataStorage.IntPtrToHandle(NativeMethods.b2joint_getuserdata(_jointPtr));

				if (ptr != 0)
					UserDataStorage.JointStorage.UnpinObject(ptr);

				if (value != null)
				{
					var handle = UserDataStorage.JointStorage.PinDataToHandle(value);
					NativeMethods.b2joint_setuserdata(_jointPtr, UserDataStorage.HandleToIntPtr(handle));
				}
				else
					NativeMethods.b2joint_setuserdata(_jointPtr, IntPtr.Zero);
			}
		}

		public static bool operator ==(Joint l, Joint r)
		{
			if ((object)l == null && (object)r == null)
				return true;
			else if ((object)l == null && (object)r != null ||
				(object)l != null && (object)r == null)
				return false;

			return l._jointPtr == r._jointPtr;
		}

		public static bool operator !=(Joint l, Joint r)
		{
			return !(l == r);
		}

		public override bool Equals(object obj)
		{
			if (obj is Joint)
				return (obj as Joint) == this;

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return _jointPtr.GetHashCode();
		}
	}
}
