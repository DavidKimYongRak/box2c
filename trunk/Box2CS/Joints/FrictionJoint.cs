using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
#if !NEW_JOINTS
	[StructLayout(LayoutKind.Sequential)]
	public class FrictionJointDef : JointDef, IFixedSize
	{
		Vec2 _localAnchorA;
		Vec2 _localAnchorB;
		float _maxForce;
		float _maxTorque;

		int IFixedSize.FixedSize()
		{
			return Marshal.SizeOf(typeof(FrictionJointDef));
		}

		void IFixedSize.Lock()
		{
		}

		void IFixedSize.Unlock()
		{
		}

		public FrictionJointDef()
		{
			JointType = EJointType.e_frictionJoint;
			_localAnchorA = Vec2.Empty;
			_localAnchorB = Vec2.Empty;
			_maxForce = 0.0f;
			_maxTorque = 0.0f;
		}

		public void Initialize(Body bodyA, Body bodyB, Vec2 anchor)
		{
			BodyA = bodyA;
			BodyB = bodyB;
			_localAnchorA = bodyA.GetLocalPoint(anchor);
			_localAnchorB = bodyB.GetLocalPoint(anchor);
		}

		public Vec2 LocalAnchorA
		{
			get { return _localAnchorA; }
			set { _localAnchorA = value; }
		}

		public Vec2 LocalAnchorB
		{
			get { return _localAnchorB; }
			set { _localAnchorB = value; }
		}

		public float MaxForce
		{
			get { return _maxForce; }
			set { _maxForce = value; }
		}

		public float MaxTorque
		{
			get { return _maxTorque; }
			set { _maxTorque = value; }
		}
	}
#else
	public class FrictionJointDef : JointDef, IDisposable
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2frictionjointdef_constructor();

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2frictionjointdef_destroy(IntPtr ptr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2frictionjointdef_getlocalanchora(IntPtr jointDef, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2frictionjointdef_setlocalanchora(IntPtr jointDef, Vec2 vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2frictionjointdef_getlocalanchorb(IntPtr jointDef, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2frictionjointdef_setlocalanchorb(IntPtr jointDef, Vec2 vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2frictionjointdef_getmaxforce(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2frictionjointdef_setmaxforce(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2frictionjointdef_getmaxtorque(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2frictionjointdef_setmaxtorque(IntPtr jointDef, float vec);
		}

		public FrictionJointDef()
		{
			JointDefPtr = NativeMethods.b2frictionjointdef_constructor();
			shouldDispose = true;
		}

		public void Initialize(Body b1, Body b2,
									Vec2 anchor)
		{
			BodyA = b1;
			BodyB = b2;
			LocalAnchorA = BodyA.GetLocalPoint(anchor);
			LocalAnchorB = BodyB.GetLocalPoint(anchor);
		}

		public Vec2 LocalAnchorA
		{
			get { Vec2 temp; NativeMethods.b2frictionjointdef_getlocalanchora(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2frictionjointdef_setlocalanchora(JointDefPtr, value); }
		}

		public Vec2 LocalAnchorB
		{
			get { Vec2 temp; NativeMethods.b2frictionjointdef_getlocalanchorb(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2frictionjointdef_setlocalanchorb(JointDefPtr, value); }
		}

		public float MaxForce
		{
			get { return NativeMethods.b2frictionjointdef_getmaxforce(JointDefPtr); }
			set { NativeMethods.b2frictionjointdef_setmaxforce(JointDefPtr, value); }
		}

		public float MaxTorque
		{
			get { return NativeMethods.b2frictionjointdef_getmaxtorque(JointDefPtr); }
			set { NativeMethods.b2frictionjointdef_setmaxtorque(JointDefPtr, value); }
		}

		#region IDisposable
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		bool disposed;
		bool shouldDispose;

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
				}

				if (shouldDispose)
				{
					NativeMethods.b2frictionjointdef_destroy(JointDefPtr);
					//System.Console.WriteLine("b2frictionjointdef_destroy");
				}

				disposed = true;
			}
		}

		~FrictionJointDef()
		{
			Dispose(false);
		}
		#endregion
	}
#endif

	public class FrictionJoint : Joint
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2frictionjoint_getmaxforce(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2frictionjoint_setmaxforce(IntPtr joint, float data);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2frictionjoint_getmaxtorque(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2frictionjoint_setmaxtorque(IntPtr joint, float data);
		}

		public FrictionJoint(IntPtr ptr) :
			base(ptr)
		{
		}

		public float MaxForce
		{
			get { return NativeMethods.b2frictionjoint_getmaxforce(JointPtr); }
			set { NativeMethods.b2frictionjoint_setmaxforce(JointPtr, value); }
		}

		public float MaxTorque
		{
			get { return NativeMethods.b2frictionjoint_getmaxtorque(JointPtr); }
			set { NativeMethods.b2frictionjoint_setmaxtorque(JointPtr, value); }
		}
	}
}
