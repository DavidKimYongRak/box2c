using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	public class PrismaticJointDef : JointDef, IDisposable
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_getlocalanchora(IntPtr jointDef, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_setlocalanchora(IntPtr jointDef, Vec2 vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_getlocalanchorb(IntPtr jointDef, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_setlocalanchorb(IntPtr jointDef, Vec2 vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_getlocalaxis1(IntPtr jointDef, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_setlocalaxis1(IntPtr jointDef, Vec2 vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2prismaticjointdef_getreferenceangle(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_setreferenceangle(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2prismaticjointdef_getenablelimit(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_setenablelimit(IntPtr jointDef, bool vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2prismaticjointdef_getlowertranslation(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_setlowertranslation(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2prismaticjointdef_getuppertranslation(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_setuppertranslation(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2prismaticjointdef_getenablemotor(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_setenablemotor(IntPtr jointDef, bool vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2prismaticjointdef_getmaxmotorforce(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_setmaxmotorforce(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2prismaticjointdef_getmotorspeed(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_setmotorspeed(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2prismaticjointdef_constructor();

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjointdef_destroy(IntPtr ptr);
		}

		public PrismaticJointDef()
		{
			JointDefPtr = NativeMethods.b2prismaticjointdef_constructor();
			shouldDispose = true;
		}

		public void Initialize(Body b1, Body b2, Vec2 anchor, Vec2 axis)
		{
			BodyA = b1;
			BodyB = b2;
			LocalAnchorA = BodyA.GetLocalPoint(anchor);
			LocalAnchorB = BodyB.GetLocalPoint(anchor);
			LocalAxisA = BodyA.GetLocalVector(axis);
			ReferenceAngle = BodyB.Angle - BodyA.Angle;
		}

		public Vec2 LocalAnchorA
		{
			get { Vec2 temp; NativeMethods.b2prismaticjointdef_getlocalanchora(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2prismaticjointdef_setlocalanchora(JointDefPtr, value); }
		}

		public Vec2 LocalAnchorB
		{
			get { Vec2 temp; NativeMethods.b2prismaticjointdef_getlocalanchorb(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2prismaticjointdef_setlocalanchorb(JointDefPtr, value); }
		}

		public Vec2 LocalAxisA
		{
			get { Vec2 temp; NativeMethods.b2prismaticjointdef_getlocalaxis1(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2prismaticjointdef_setlocalaxis1(JointDefPtr, value); }
		}

		public float ReferenceAngle
		{
			get { return NativeMethods.b2prismaticjointdef_getreferenceangle(JointDefPtr); }
			set { NativeMethods.b2prismaticjointdef_setreferenceangle(JointDefPtr, value); }
		}

		public bool EnableLimit
		{
			get { return NativeMethods.b2prismaticjointdef_getenablelimit(JointDefPtr); }
			set { NativeMethods.b2prismaticjointdef_setenablelimit(JointDefPtr, value); }
		}

		public float LowerTranslation
		{
			get { return NativeMethods.b2prismaticjointdef_getlowertranslation(JointDefPtr); }
			set { NativeMethods.b2prismaticjointdef_setlowertranslation(JointDefPtr, value); }
		}

		public float UpperTranslation
		{
			get { return NativeMethods.b2prismaticjointdef_getuppertranslation(JointDefPtr); }
			set { NativeMethods.b2prismaticjointdef_setuppertranslation(JointDefPtr, value); }
		}

		public bool EnableMotor
		{
			get { return NativeMethods.b2prismaticjointdef_getenablemotor(JointDefPtr); }
			set { NativeMethods.b2prismaticjointdef_setenablemotor(JointDefPtr, value); }
		}

		public float MotorSpeed
		{
			get { return NativeMethods.b2prismaticjointdef_getmotorspeed(JointDefPtr); }
			set { NativeMethods.b2prismaticjointdef_setmotorspeed(JointDefPtr, value); }
		}

		public float MaxMotorForce
		{
			get { return NativeMethods.b2prismaticjointdef_getmaxmotorforce(JointDefPtr); }
			set { NativeMethods.b2prismaticjointdef_setmaxmotorforce(JointDefPtr, value); }
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
					NativeMethods.b2prismaticjointdef_destroy(JointDefPtr);
					//System.Console.WriteLine("b2prismaticjointdef_destroy");
				}

				disposed = true;
			}
		}

		~PrismaticJointDef()
		{
			Dispose(false);
		}
		#endregion
	}

	public class PrismaticJoint : Joint
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2prismaticjoint_getjointtranslation(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2prismaticjoint_getjointspeed(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2prismaticjoint_getenablelimit(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjoint_setenablelimit(IntPtr joint, bool val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2prismaticjoint_getlowerlimit(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2prismaticjoint_getupperlimit(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjoint_setlimits(IntPtr joint, float lower, float upper);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2prismaticjoint_getenablemotor(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjoint_setenablemotor(IntPtr joint, bool val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2prismaticjoint_getmotorspeed(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjoint_setmotorspeed(IntPtr joint, float val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2prismaticjoint_setmaxmotorforce(IntPtr joint, float val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2prismaticjoint_getmotorforce(IntPtr joint);
		}

		public PrismaticJoint(IntPtr ptr) :
			base(ptr)
		{
		}

		public float JointTranslation
		{
			get { return NativeMethods.b2prismaticjoint_getjointtranslation(JointPtr); }
		}

		public float JointSpeed
		{
			get { return NativeMethods.b2prismaticjoint_getjointspeed(JointPtr); }
		}

		public bool IsLimitEnabled
		{
			get { return NativeMethods.b2prismaticjoint_getenablelimit(JointPtr); }
			set { NativeMethods.b2prismaticjoint_setenablelimit(JointPtr, value); }
		}

		public void SetLimits(float lower, float upper)
		{
			LowerLimit = lower;
			UpperLimit = upper;
		}

		public float LowerLimit
		{
			get { return NativeMethods.b2prismaticjoint_getlowerlimit(JointPtr); }
			set { SetLimits(value, UpperLimit); }
		}

		public float UpperLimit
		{
			get { return NativeMethods.b2prismaticjoint_getupperlimit(JointPtr); }
			set { SetLimits(LowerLimit, value); }
		}

		public bool IsMotorEnabled
		{
			get { return NativeMethods.b2prismaticjoint_getenablemotor(JointPtr); }
			set { NativeMethods.b2prismaticjoint_setenablemotor(JointPtr, value); }
		}

		public float MotorSpeed
		{
			get { return NativeMethods.b2prismaticjoint_getmotorspeed(JointPtr); }
			set { NativeMethods.b2prismaticjoint_setmotorspeed(JointPtr, value); }
		}

		public float MaxMotorForce
		{
			set { NativeMethods.b2prismaticjoint_setmaxmotorforce(JointPtr, value); }
		}

		public float MotorForce
		{
			get { return NativeMethods.b2prismaticjoint_getmotorforce(JointPtr); }
		}
	}
}
