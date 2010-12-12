using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	public class MouseJointDef : JointDef, IDisposable
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2mousejointdef_constructor();

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2mousejointdef_destroy(IntPtr ptr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2mousejointdef_gettarget(IntPtr jointDef, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2mousejointdef_settarget(IntPtr jointDef, Vec2 vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2mousejointdef_getmaxforce(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2mousejointdef_setmaxforce(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2mousejointdef_getfrequency(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2mousejointdef_setfrequency(IntPtr jointDef, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2mousejointdef_getdampingratio(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2mousejointdef_setdampingratio(IntPtr jointDef, float vec);
		}

		public MouseJointDef()
		{
			JointDefPtr = NativeMethods.b2mousejointdef_constructor();
			shouldDispose = true;
		}

		public Vec2 Target
		{
			get { Vec2 temp; NativeMethods.b2mousejointdef_gettarget(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2mousejointdef_settarget(JointDefPtr, value); }
		}

		public float MaxForce
		{
			get { return NativeMethods.b2mousejointdef_getmaxforce(JointDefPtr); }
			set { NativeMethods.b2mousejointdef_setmaxforce(JointDefPtr, value); }
		}

		public float Frequency
		{
			get { return NativeMethods.b2mousejointdef_getfrequency(JointDefPtr); }
			set { NativeMethods.b2mousejointdef_setfrequency(JointDefPtr, value); }
		}

		public float DampingRatio
		{
			get { return NativeMethods.b2mousejointdef_getdampingratio(JointDefPtr); }
			set { NativeMethods.b2mousejointdef_setdampingratio(JointDefPtr, value); }
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
					NativeMethods.b2mousejointdef_destroy(JointDefPtr);
					//System.Console.WriteLine("b2mousejointdef_destroy");
				}

				disposed = true;
			}
		}

		~MouseJointDef()
		{
			Dispose(false);
		}
		#endregion
	}

	public class MouseJoint : Joint
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2mousejoint_gettarget(IntPtr joint, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2mousejoint_settarget(IntPtr joint, Vec2 val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2mousejoint_getmaxforce(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2mousejoint_setmaxforce(IntPtr joint, float val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2mousejoint_getfrequency(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2mousejoint_setfrequency(IntPtr joint, float val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2mousejoint_getdampingratio(IntPtr joint);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2mousejoint_setdampingratio(IntPtr joint, float val);
		}

		public MouseJoint(IntPtr ptr) :
			base(ptr)
		{
		}

		public Vec2 Target
		{
			get { Vec2 temp; NativeMethods.b2mousejoint_gettarget(JointPtr, out temp); return temp; }
			set { NativeMethods.b2mousejoint_settarget(JointPtr, value); }
		}

		public float MaxForce
		{
			get { return NativeMethods.b2mousejoint_getmaxforce(JointPtr); }
			set { NativeMethods.b2mousejoint_setmaxforce(JointPtr, value); }
		}

		public float Frequency
		{
			get { return NativeMethods.b2mousejoint_getfrequency(JointPtr); }
			set { NativeMethods.b2mousejoint_setfrequency(JointPtr, value); }
		}

		public float DampingRatio
		{
			get { return NativeMethods.b2mousejoint_getdampingratio(JointPtr); }
			set { NativeMethods.b2mousejoint_setdampingratio(JointPtr, value); }
		}
	}
}
