using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	public class GearJointDef : JointDef, IDisposable
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2gearjointdef_constructor();

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2gearjointdef_destroy(IntPtr ptr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2gearjointdef_getjoint1(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2gearjointdef_setjoint1(IntPtr jointDef, IntPtr vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2gearjointdef_getjoint2(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2gearjointdef_setjoint2(IntPtr jointDef, IntPtr vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2gearjointdef_getratio(IntPtr jointDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2gearjointdef_setratio(IntPtr jointDef, float vec);
		}

		public GearJointDef()
		{
			JointDefPtr = NativeMethods.b2gearjointdef_constructor();
			shouldDispose = true;
		}

		public Joint JointA
		{
			get { return Joint.FromPtr(NativeMethods.b2gearjointdef_getjoint1(JointDefPtr)); }
			set { NativeMethods.b2gearjointdef_setjoint1(JointDefPtr, value.JointPtr); }
		}

		public Joint JointB
		{
			get { return Joint.FromPtr(NativeMethods.b2gearjointdef_getjoint2(JointDefPtr)); }
			set { NativeMethods.b2gearjointdef_setjoint2(JointDefPtr, value.JointPtr); }
		}

		public float Ratio
		{
			get { return NativeMethods.b2gearjointdef_getratio(JointDefPtr); }
			set { NativeMethods.b2gearjointdef_setratio(JointDefPtr, value); }
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
					NativeMethods.b2gearjointdef_destroy(JointDefPtr);
					//System.Console.WriteLine("b2gearjointdef_destroy");
				}

				disposed = true;
			}
		}

		~GearJointDef()
		{
			Dispose(false);
		}
		#endregion
	}

	public class GearJoint : Joint
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2gearjoint_setratio(IntPtr joint, float data);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2gearjoint_getratio(IntPtr joint);
		}

		public GearJoint(IntPtr ptr) :
			base(ptr)
		{
		}

		public float Ratio
		{
			get { return NativeMethods.b2gearjoint_getratio(JointPtr); }
			set { NativeMethods.b2gearjoint_setratio(JointPtr, value); }
		}
	}
}
