using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
#if !NEW_JOINTS
	[StructLayout(LayoutKind.Sequential)]
	public class WeldJointDef : JointDef, IFixedSize
	{
		Vec2 _localAnchorA;
		Vec2 _localAnchorB;
		float _referenceAngle;

		int IFixedSize.FixedSize()
		{
			return Marshal.SizeOf(typeof(WeldJointDef));
		}

		void IFixedSize.Lock()
		{
		}

		void IFixedSize.Unlock()
		{
		}

		public WeldJointDef()
		{
			JointType = EJointType.e_weldJoint;
			_localAnchorA = Vec2.Empty;
			_localAnchorB = Vec2.Empty;
			_referenceAngle = 0.0f;
		}

		/// Initialize the bodies, anchors, and reference angle using a world
		/// anchor point.
		public void Initialize(Body body1, Body body2, Vec2 anchor)
		{
			BodyA = body1;
			BodyB = body2;
			_localAnchorA = BodyA.GetLocalPoint(anchor);
			_localAnchorB = BodyB.GetLocalPoint(anchor);
			_referenceAngle = BodyB.Angle - BodyA.Angle;
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
	};
#else
	public class WeldJointDef : JointDef, IDisposable
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2weldjointdef_constructor();

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2weldjointdef_destroy(IntPtr ptr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2weldjointdef_getlocalanchora(IntPtr ptr, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2weldjointdef_setlocalanchora(IntPtr ptr, Vec2 val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2weldjointdef_getlocalanchorb(IntPtr ptr, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2weldjointdef_setlocalanchorb(IntPtr ptr, Vec2 val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2weldjointdef_getreferenceangle(IntPtr ptr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2weldjointdef_setreferenceangle(IntPtr ptr, float val);
		}

		public const float MinPulleyLength = 2.0f;

		public WeldJointDef()
		{
			JointDefPtr = NativeMethods.b2weldjointdef_constructor();
			shouldDispose = true;
		}

		public void Initialize(Body bA, Body bB, Vec2 anchor)
		{
			BodyA = bA;
			BodyB = bB;
			LocalAnchorA = BodyA.GetLocalPoint(anchor);
			LocalAnchorB = BodyB.GetLocalPoint(anchor);
			ReferenceAngle = BodyB.Angle - BodyA.Angle;
		}

		public Vec2 LocalAnchorA
		{
			get { Vec2 temp; NativeMethods.b2weldjointdef_getlocalanchora(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2weldjointdef_setlocalanchora(JointDefPtr, value); }
		}

		public Vec2 LocalAnchorB
		{
			get { Vec2 temp; NativeMethods.b2weldjointdef_getlocalanchorb(JointDefPtr, out temp); return temp; }
			set { NativeMethods.b2weldjointdef_setlocalanchorb(JointDefPtr, value); }
		}

		public float ReferenceAngle
		{
			get { return NativeMethods.b2weldjointdef_getreferenceangle(JointDefPtr); }
			set { NativeMethods.b2weldjointdef_setreferenceangle(JointDefPtr, value); }
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
					NativeMethods.b2weldjointdef_destroy(JointDefPtr);
					//System.Console.WriteLine("b2weldjointdef_destroy");
				}

				disposed = true;
			}
		}

		~WeldJointDef()
		{
			Dispose(false);
		}
		#endregion
	}
#endif

	public class WeldJoint : Joint
	{
		public WeldJoint(IntPtr ptr) :
			base(ptr)
		{
		}
	}
}
