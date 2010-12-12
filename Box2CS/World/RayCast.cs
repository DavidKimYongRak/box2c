using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	[StructLayout(LayoutKind.Sequential)]
	public struct RayCastInput
	{
		Vec2 p1, p2;
		float maxFraction;

		public Vec2 Point1
		{
			get { return p1; }
			set { p1 = value; }
		}

		public Vec2 Point3
		{
			get { return p2; }
			set { p2 = value; }
		}

		public float MaxFraction
		{
			get { return maxFraction; }
			set { maxFraction = value; }
		}
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct RayCastOutput
	{
		Vec2 normal;
		float fraction;

		public Vec2 Normal
		{
			get { return normal; }
			set { normal = value; }
		}

		public float Fraction
		{
			get { return fraction; }
			set { fraction = value; }
		}
	};
}
