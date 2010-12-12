using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	public class PulleyJointDef : JointDef, IDisposable
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2pulleyjointdef_constructor();

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_destroy(IntPtr ptr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_getgroundanchora(IntPtr ptr, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_setgroundanchora(IntPtr ptr, Vec2 val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_getgroundanchorb(IntPtr ptr, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_setgroundanchorb(IntPtr ptr, Vec2 val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_getlocalanchora(IntPtr ptr, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_setlocalanchora(IntPtr ptr, Vec2 val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_getlocalanchorb(IntPtr ptr, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_setlocalanchorb(IntPtr ptr, Vec2 val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2pulleyjointdef_getlengtha(IntPtr ptr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_setlengtha(IntPtr ptr, float val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2pulleyjointdef_getmaxlengtha(IntPtr ptr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_setmaxlengtha(IntPtr ptr, float val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2pulleyjointdef_getlengthb(IntPtr ptr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_setlengthb(IntPtr ptr, float val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2pulleyjointdef_getmaxlengthb(IntPtr ptr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_setmaxlengthb(IntPtr ptr, float val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2pulleyjointdef_getratio(IntPtr ptr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2pulleyjointdef_setratio(IntPtr ptr, float val);
		}

		public const float MinPulleyLength = 2.0f;

		public PulleyJointDef()
		{
			JointDefPtr = NativeMethods.b2pulleyjointdef_constructor();
			shouldDispose = true;
		}

		public void Initialize(Body b1, Body b2,
				Vec2 ga1, Vec2 ga2,
				Vec2 anchor1, Vec2 anchor2,
				float r)
		{
			BodyA = b1;
			BodyB = b2;
			GroundAnchorA = ga1;
			GroundAnchorB = ga2;
			LocalAnchorA = BodyA.GetLocalPoint(anchor1);
			LocalAnchorB = BodyB.GetLocalPoint(anchor2);
			Vec2 d1 = anchor1 - ga1;
			LengthA = d1.Length();
			Vec2 d2 = anchor2 - ga2;
			LengthB = d2.Length();
			Ratio = r;
			//Settings.b2Assert(ratio > b2_epsilon);

			if (!(Ratio > float.Epsilon))
				throw new Exception();

			float C = LengthA + Ratio * LengthB;
			MaxLengthA = C - Ratio * MinPulleyLength;
			MaxLengthB = (C - MinPulleyLength) / Ratio;
		}

		public Vec2 GroundAnchorA
		{
			get { Vec2 temp; NativeMethods.b2pulleyjointdef_getgroundanchora(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2pulleyjointdef_setgroundanchora(JointDefPtr, value); }
		}

		public Vec2 GroundAnchorB
		{
			get { Vec2 temp; NativeMethods.b2pulleyjointdef_getgroundanchorb(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2pulleyjointdef_setgroundanchorb(JointDefPtr, value); }
		}

		public Vec2 LocalAnchorA
		{
			get { Vec2 temp; NativeMethods.b2pulleyjointdef_getlocalanchora(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2pulleyjointdef_setlocalanchora(JointDefPtr, value); }
		}

		public Vec2 LocalAnchorB
		{
			get { Vec2 temp; NativeMethods.b2pulleyjointdef_getlocalanchorb(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2pulleyjointdef_setlocalanchorb(JointDefPtr, value); }
		}

		public float LengthA
		{
			get { return NativeMethods.b2pulleyjointdef_getlengtha(JointDefPtr); }
			set { NativeMethods.b2pulleyjointdef_setlengtha(JointDefPtr, value); }
		}

		public float MaxLengthA
		{
			get { return NativeMethods.b2pulleyjointdef_getmaxlengtha(JointDefPtr); }
			set { NativeMethods.b2pulleyjointdef_setmaxlengtha(JointDefPtr, value); }
		}
		public float LengthB
		{
			get { return NativeMethods.b2pulleyjointdef_getlengthb(JointDefPtr); }
			set { NativeMethods.b2pulleyjointdef_setlengthb(JointDefPtr, value); }
		}

		public float MaxLengthB
		{
			get { return NativeMethods.b2pulleyjointdef_getmaxlengthb(JointDefPtr); }
			set { NativeMethods.b2pulleyjointdef_setmaxlengthb(JointDefPtr, value); }
		}

		public float Ratio
		{
			get { return NativeMethods.b2pulleyjointdef_getratio(JointDefPtr); }
			set { NativeMethods.b2pulleyjointdef_setratio(JointDefPtr, value); }
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
					NativeMethods.b2pulleyjointdef_destroy(JointDefPtr);
					//System.Console.WriteLine("b2pulleyjointdef_destroy");
				}

				disposed = true;
			}
		}

		~PulleyJointDef()
		{
			Dispose(false);
		}
		#endregion
	}

	public class PulleyJoint : Joint
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2pulleyjoint_getlength1(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2pulleyjoint_getlength2(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2pulleyjoint_getratio(IntPtr joint);
		}

		public PulleyJoint(IntPtr ptr) :
			base(ptr)
		{
		}

		public float LengthA
		{
			get { return NativeMethods.b2pulleyjoint_getlength1(JointPtr); }
		}

		public float LengthB
		{
			get { return NativeMethods.b2pulleyjoint_getlength2(JointPtr); }
		}

		public float Ratio
		{
			get { return NativeMethods.b2pulleyjoint_getratio(JointPtr); }
		}
	}
}
