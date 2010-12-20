using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
#if !NEW_JOINTS
	[StructLayout(LayoutKind.Sequential)]
	public class RevoluteJointDef : JointDef, IFixedSize
	{
		Vec2 _localAnchorA;
		Vec2 _localAnchorB;
		float _referenceAngle;
		bool _enableLimit;
		float _lowerAngle;
		float _upperAngle;
		bool _enableMotor;
		float _motorSpeed;
		float _maxMotorTorque;

		int IFixedSize.FixedSize()
		{
			return Marshal.SizeOf(typeof(RevoluteJointDef));
		}

		void IFixedSize.Lock()
		{
		}

		void IFixedSize.Unlock()
		{
		}

		public RevoluteJointDef()
		{
			JointType = EJointType.e_revoluteJoint;
			_localAnchorA = Vec2.Empty;
			_localAnchorB = Vec2.Empty;
			_referenceAngle = 0.0f;
			_lowerAngle = 0.0f;
			_upperAngle = 0.0f;
			_maxMotorTorque = 0.0f;
			_motorSpeed = 0.0f;
			_enableLimit = false;
			_enableMotor = false;
		}

		public void Initialize(Body bodyA, Body bodyB, Vec2 anchor)
		{
			BodyA = bodyA;
			BodyB = bodyB;
			_localAnchorA = bodyA.GetLocalPoint(anchor);
			_localAnchorB = bodyB.GetLocalPoint(anchor);
			_referenceAngle = bodyB.Angle - bodyA.Angle;
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

		public float ReferenceAngle
		{
			get { return _referenceAngle; }
			set { _referenceAngle = value; }
		}

		public bool EnableLimit
		{
			get { return _enableLimit; }
			set { _enableLimit = value; }
		}

		public float LowerAngle
		{
			get { return _lowerAngle; }
			set { _lowerAngle = value; }
		}

		public float UpperAngle
		{
			get { return _upperAngle; }
			set { _upperAngle = value; }
		}

		public bool EnableMotor
		{
			get { return _enableMotor; }
			set { _enableMotor = value; }
		}

		public float MotorSpeed
		{
			get { return _motorSpeed; }
			set { _motorSpeed = value; }
		}

		public float MaxMotorTorque
		{
			get { return _maxMotorTorque; }
			set { _maxMotorTorque = value; }
		}
	}
#else
	public class RevoluteJointDef : JointDef, IDisposable
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejointdef_getlocalanchora(IntPtr jointDef, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejointdef_setlocalanchora(IntPtr jointDef, Vec2 vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejointdef_getlocalanchorb(IntPtr jointDef, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejointdef_setlocalanchorb(IntPtr jointDef, Vec2 vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2revolutejointdef_getreferenceangle(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejointdef_setreferenceangle(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2revolutejointdef_getenablelimit(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejointdef_setenablelimit(IntPtr jointDef, bool vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2revolutejointdef_getlowerangle(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejointdef_setlowerangle(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2revolutejointdef_getupperangle(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejointdef_setupperangle(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2revolutejointdef_getenablemotor(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejointdef_setenablemotor(IntPtr jointDef, bool vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2revolutejointdef_getmotorspeed(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejointdef_setmotorspeed(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2revolutejointdef_getmaxmotortorque(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejointdef_setmaxmotortorque(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2revolutejointdef_constructor();

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejointdef_destroy(IntPtr ptr);
		}

		public RevoluteJointDef()
		{
			JointDefPtr = NativeMethods.b2revolutejointdef_constructor();
			shouldDispose = true;
		}

		public void Initialize(Body b1, Body b2, Vec2 anchor)
		{
			BodyA = b1;
			BodyB = b2;
			LocalAnchorA = b1.GetLocalPoint(anchor);
			LocalAnchorB = b2.GetLocalPoint(anchor);
			ReferenceAngle = b2.Angle - b1.Angle;
		}

		public Vec2 LocalAnchorA
		{
			get { Vec2 temp; NativeMethods.b2revolutejointdef_getlocalanchora(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2revolutejointdef_setlocalanchora(JointDefPtr, value); }
		}

		public Vec2 LocalAnchorB
		{
			get { Vec2 temp; NativeMethods.b2revolutejointdef_getlocalanchorb(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2revolutejointdef_setlocalanchorb(JointDefPtr, value); }
		}

		public float ReferenceAngle
		{
			get { return NativeMethods.b2revolutejointdef_getreferenceangle(JointDefPtr); }
			set { NativeMethods.b2revolutejointdef_setreferenceangle(JointDefPtr, value); }
		}

		public bool EnableLimit
		{
			get { return NativeMethods.b2revolutejointdef_getenablelimit(JointDefPtr); }
			set { NativeMethods.b2revolutejointdef_setenablelimit(JointDefPtr, value); }
		}

		public float LowerAngle
		{
			get { return NativeMethods.b2revolutejointdef_getlowerangle(JointDefPtr); }
			set { NativeMethods.b2revolutejointdef_setlowerangle(JointDefPtr, value); }
		}

		public float UpperAngle
		{
			get { return NativeMethods.b2revolutejointdef_getupperangle(JointDefPtr); }
			set { NativeMethods.b2revolutejointdef_setupperangle(JointDefPtr, value); }
		}

		public bool EnableMotor
		{
			get { return NativeMethods.b2revolutejointdef_getenablemotor(JointDefPtr); }
			set { NativeMethods.b2revolutejointdef_setenablemotor(JointDefPtr, value); }
		}

		public float MotorSpeed
		{
			get { return NativeMethods.b2revolutejointdef_getmotorspeed(JointDefPtr); }
			set { NativeMethods.b2revolutejointdef_setmotorspeed(JointDefPtr, value); }
		}

		public float MaxMotorTorque
		{
			get { return NativeMethods.b2revolutejointdef_getmaxmotortorque(JointDefPtr); }
			set { NativeMethods.b2revolutejointdef_setmaxmotortorque(JointDefPtr, value); }
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
					NativeMethods.b2revolutejointdef_destroy(JointDefPtr);
					//System.Console.WriteLine("b2revolutejointdef_destroy");
				}

				disposed = true;
			}
		}

		~RevoluteJointDef()
		{
			Dispose(false);
		}
		#endregion
	}
#endif

	public class RevoluteJoint : Joint
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2revolutejoint_getjointangle(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2revolutejoint_getjointspeed(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2revolutejoint_getenablelimit(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejoint_setenablelimit(IntPtr joint, bool val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2revolutejoint_getlowerlimit(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2revolutejoint_getupperlimit(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejoint_setlimits(IntPtr joint, float lower, float upper);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2revolutejoint_getenablemotor(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejoint_setenablemotor(IntPtr joint, bool val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2revolutejoint_getmotorspeed(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejoint_setmotorspeed(IntPtr joint, float val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2revolutejoint_setmaxmotortorque(IntPtr joint, float val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2revolutejoint_getmotortorque(IntPtr joint);
		}

		public RevoluteJoint(IntPtr ptr) :
			base(ptr)
		{
		}

		public float JointTranslation
		{
			get { return NativeMethods.b2revolutejoint_getjointangle(JointPtr); }
		}

		public float JointSpeed
		{
			get { return NativeMethods.b2revolutejoint_getjointspeed(JointPtr); }
		}

		public bool IsLimitEnabled
		{
			get { return NativeMethods.b2revolutejoint_getenablelimit(JointPtr); }
			set { NativeMethods.b2revolutejoint_setenablelimit(JointPtr, value); }
		}

		public void SetLimits(float lower, float upper)
		{
			LowerLimit = lower;
			UpperLimit = upper;
		}

		public float LowerLimit
		{
			get { return NativeMethods.b2revolutejoint_getlowerlimit(JointPtr); }
			set { SetLimits(value, UpperLimit); }
		}

		public float UpperLimit
		{
			get { return NativeMethods.b2revolutejoint_getupperlimit(JointPtr); }
			set { SetLimits(LowerLimit, value); }
		}

		public bool IsMotorEnabled
		{
			get { return NativeMethods.b2revolutejoint_getenablemotor(JointPtr); }
			set { NativeMethods.b2revolutejoint_setenablemotor(JointPtr, value); }
		}

		public float MotorSpeed
		{
			get { return NativeMethods.b2revolutejoint_getmotorspeed(JointPtr); }
			set { NativeMethods.b2revolutejoint_setmotorspeed(JointPtr, value); }
		}

		public float MaxMotorTorque
		{
			set { NativeMethods.b2revolutejoint_setmaxmotortorque(JointPtr, value); }
		}

		public float MotorTorque
		{
			get { return NativeMethods.b2revolutejoint_getmotortorque(JointPtr); }
		}
	}
}
