using System;
using System.Runtime.InteropServices;
namespace Box2CS
{
	public class LineJointDef : JointDef, IDisposable
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2linejointdef_constructor();

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_destroy(IntPtr ptr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_getlocalanchora(IntPtr jointDef, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_setlocalanchora(IntPtr jointDef, Vec2 vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_getlocalanchorb(IntPtr jointDef, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_setlocalanchorb(IntPtr jointDef, Vec2 vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_getlocalaxisa(IntPtr jointDef, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_setlocalaxisa(IntPtr jointDef, Vec2 vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2linejointdef_getenablelimit(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_setenablelimit(IntPtr jointDef, bool vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2linejointdef_getlowertranslation(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_setlowertranslation(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2linejointdef_getuppertranslation(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_setuppertranslation(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2linejointdef_getenablemotor(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_setenablemotor(IntPtr jointDef, bool vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2linejointdef_getmaxmotorforce(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_setmaxmotorforce(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2linejointdef_getmotorspeed(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejointdef_setmotorspeed(IntPtr jointDef, float vec);
		}

		public LineJointDef()
		{
			JointDefPtr = NativeMethods.b2linejointdef_constructor();
			shouldDispose = true;
		}

		public void Initialize(Body b1, Body b2, Vec2 anchor, Vec2 axis)
		{
			BodyA = b1;
			BodyB = b2;
			LocalAnchorA = BodyA.GetLocalPoint(anchor);
			LocalAnchorB = BodyB.GetLocalPoint(anchor);
			LocalAxisA = BodyA.GetLocalVector(axis);
		}

		public Vec2 LocalAnchorA
		{
			get { Vec2 temp; NativeMethods.b2linejointdef_getlocalanchora(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2linejointdef_setlocalanchora(JointDefPtr, value); }
		}

		public Vec2 LocalAnchorB
		{
			get { Vec2 temp; NativeMethods.b2linejointdef_getlocalanchorb(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2linejointdef_setlocalanchorb(JointDefPtr, value); }
		}

		public Vec2 LocalAxisA
		{
			get { Vec2 temp; NativeMethods.b2linejointdef_getlocalaxisa(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2linejointdef_setlocalaxisa(JointDefPtr, value); }
		}

		public bool EnableLimit
		{
			get { return NativeMethods.b2linejointdef_getenablelimit(JointDefPtr); }
			set { NativeMethods.b2linejointdef_setenablelimit(JointDefPtr, value); }
		}

		public float LowerTranslation
		{
			get { return NativeMethods.b2linejointdef_getlowertranslation(JointDefPtr); }
			set { NativeMethods.b2linejointdef_setlowertranslation(JointDefPtr, value); }
		}

		public float UpperTranslation
		{
			get { return NativeMethods.b2linejointdef_getuppertranslation(JointDefPtr); }
			set { NativeMethods.b2linejointdef_setuppertranslation(JointDefPtr, value); }
		}

		public bool EnableMotor
		{
			get { return NativeMethods.b2linejointdef_getenablemotor(JointDefPtr); }
			set { NativeMethods.b2linejointdef_setenablemotor(JointDefPtr, value); }
		}

		public float MaxMotorForce
		{
			get { return NativeMethods.b2linejointdef_getmaxmotorforce(JointDefPtr); }
			set { NativeMethods.b2linejointdef_setmaxmotorforce(JointDefPtr, value); }
		}

		public float MotorSpeed
		{
			get { return NativeMethods.b2linejointdef_getmotorspeed(JointDefPtr); }
			set { NativeMethods.b2linejointdef_setmotorspeed(JointDefPtr, value); }
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
					NativeMethods.b2linejointdef_destroy(JointDefPtr);
					//System.Console.WriteLine("b2linejointdef_destroy");
				}

				disposed = true;
			}
		}

		~LineJointDef()
		{
			Dispose(false);
		}
		#endregion
	}

	public class LineJoint : Joint
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2linejoint_getjointtranslation(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2linejoint_getjointspeed(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2linejoint_getenablelimit(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejoint_setenablelimit(IntPtr joint, bool val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2linejoint_getlowerlimit(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2linejoint_getupperlimit(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejoint_setlimits(IntPtr joint, float lower, float upper);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2linejoint_getenablemotor(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejoint_setenablemotor(IntPtr joint, bool val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2linejoint_getmotorspeed(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejoint_setmotorspeed(IntPtr joint, float val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2linejoint_setmaxmotorforce(IntPtr joint, float val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2linejoint_getmotorforce(IntPtr joint);
		}

		public LineJoint(IntPtr ptr) :
			base(ptr)
		{
		}

		public float JointTranslation
		{
			get { return NativeMethods.b2linejoint_getjointtranslation(JointPtr); }
		}

		public float JointSpeed
		{
			get { return NativeMethods.b2linejoint_getjointspeed(JointPtr); }
		}

		public bool IsLimitEnabled
		{
			get { return NativeMethods.b2linejoint_getenablelimit(JointPtr); }
			set { NativeMethods.b2linejoint_setenablelimit(JointPtr, value); }
		}

		public void SetLimits(float lower, float upper)
		{
			LowerLimit = lower;
			UpperLimit = upper;
		}

		public float LowerLimit
		{
			get { return NativeMethods.b2linejoint_getlowerlimit(JointPtr); }
			set { SetLimits(value, UpperLimit); }
		}

		public float UpperLimit
		{
			get { return NativeMethods.b2linejoint_getupperlimit(JointPtr); }
			set { SetLimits(LowerLimit, value); }
		}

		public bool IsMotorEnabled
		{
			get { return NativeMethods.b2linejoint_getenablemotor(JointPtr); }
			set { NativeMethods.b2linejoint_setenablemotor(JointPtr, value); }
		}

		public float MotorSpeed
		{
			get { return NativeMethods.b2linejoint_getmotorspeed(JointPtr); }
			set { NativeMethods.b2linejoint_setmotorspeed(JointPtr, value); }
		}

		public float MaxMotorForce
		{
			set { NativeMethods.b2linejoint_setmaxmotorforce(JointPtr, value); }
		}

		public float MotorForce
		{
			get { return NativeMethods.b2linejoint_getmotorforce(JointPtr); }
		}
	}
}
