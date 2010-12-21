using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	public static class b2Math
	{
		public static bool IsValid(float x)
		{
			if (float.IsNaN(x) || float.IsInfinity(x))
				return false;

			return true;
		}

		public static float b2Clamp(float a, float low, float high)
		{
			return Math.Max(low, Math.Min(a, high));
		}

		public static int b2Clamp(int a, int low, int high)
		{
			return Math.Max(low, Math.Min(a, high));
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vec2
	{
		public static Vec2 Min(Vec2 a, Vec2 b)
		{
			return new Vec2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
		}

		public static Vec2 Max(Vec2 a, Vec2 b)
		{
			return new Vec2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
		}

		public static Vec2 Empty = new Vec2(0, 0);

		public float x;
		public float y;

		public Vec2(float _x, float _y)
		{
			x = _x;
			y = _y;
		}

		public static Vec2 operator-(Vec2 l, Vec2 r)
		{
			return new Vec2(l.x - r.x, l.y - r.y);
		}

		public static Vec2 operator+(Vec2 l, Vec2 r)
		{
			return new Vec2(l.x + r.x, l.y + r.y);
		}

		public static Vec2 operator *(Vec2 l, float v)
		{
			return new Vec2(l.x * v, l.y * v);
		}

		public static Vec2 operator *(Vec2 l, Vec2 v)
		{
			return new Vec2(l.x * v.x, l.y * v.y);
		}

		public static Vec2 operator*(float v, Vec2 l)
		{
			return l * v;
		}

		public static Vec2 operator-(Vec2 l)
		{
			return new Vec2(-l.x, -l.y);
		}

		public static bool operator==(Vec2 l, Vec2 r)
		{
			return (l.x == r.x && l.y == r.y);
		}

		public override bool Equals(object obj)
		{
			if (obj is Vec2)
				return ((Vec2)obj) == this;

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator !=(Vec2 l, Vec2 r)
		{
			return !(l == r);
		}

		/// Perform the dot product on two vectors.
		public float Dot(Vec2 b)
		{
			return x * b.x + y * b.y;
		}

		/// Perform the cross product on two vectors. In 2D this produces a scalar.
		public float Cross(Vec2 b)
		{
			return x * b.y - y * b.x;
		}

		/// Perform the cross product on a vector and a scalar. In 2D this produces
		/// a vector.
		public Vec2 Cross(float s)
		{
			return new Vec2(s * y, -s * x);
		}

		/// Perform the cross product on a scalar and a vector. In 2D this produces
		/// a vector.
		public static Vec2 b2Cross(float s, Vec2 a)
		{
			return new Vec2(-s * a.y, s * a.x);
		}

		public bool IsValid()
		{
			return b2Math.IsValid(x) && b2Math.IsValid(y);
		}

		public float Length()
		{
			return (float)Math.Sqrt(x * x + y * y);
		}

		public float Normalize()
		{
			float length = Length();

			if (length < float.Epsilon)
				return 0.0f;

			float invLength = 1.0f / length;
			x *= invLength;
			y *= invLength;

			return length;
		}

		public Vec2 Normalized()
		{
			var newVec = this;
			newVec.Normalize();
			return newVec;
		}

		public float LengthSquared()
		{
			return x * x + y * y;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Mat22
	{
		public Vec2 col1, col2;

		public void Set(float angle)
		{
			float c = (float)Math.Cos(angle), s = (float)Math.Sin(angle);
			col1.x = c; col2.x = -s;
			col1.y = s; col2.y = c;
		}

		public static Vec2 operator*(Mat22 A, Vec2 v)
		{
			return new Vec2(A.col1.x * v.x + A.col2.x * v.y, A.col1.y * v.x + A.col2.y * v.y);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Transform
	{
		public Vec2 position;
		public Mat22 R;

		public static Vec2 operator*(Transform T, Vec2 v)
		{
			float x = T.position.x + T.R.col1.x * v.x + T.R.col2.x * v.y;
			float y = T.position.y + T.R.col1.y * v.x + T.R.col2.y * v.y;

			return new Vec2(x, y);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ColorF
	{
		float _r, _g, _b, _a;

		public float R
		{
			get { return _r; }
			set { _r = value; }
		}

		public float G
		{
			get { return _g; }
			set { _g = value; }
		}

		public float B
		{
			get { return _b; }
			set { _b = value; }
		}

		public float A
		{
			get { return _a; }
			set { _a = value; }
		}

		public ColorF(float r, float g, float b, float a)
		{
			_r = r;
			_g = g;
			_b = b;
			_a = a;
		}

		public ColorF(float r, float g, float b) :
			this(r, g, b, 1)
		{
		}

		public ColorF(System.Drawing.Color color)
		{
			_r = I2F(color.R, 255);
			_g = I2F(color.G, 255);
			_b = I2F(color.B, 255);
			_a = I2F(color.A, 255);
		}

		static int F2I(float f, int multiplier)
		{
			return (int)(f * multiplier);
		}

		static float I2F(int i, int multiplier)
		{
			return (float)i / (float)multiplier;
		}

		public System.Drawing.Color ToGDIColor()
		{
			return System.Drawing.Color.FromArgb(
				F2I(_r, 255), 
				F2I(_g, 255),
				F2I(_b, 255),
				F2I(_a, 255));
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AABB
	{
		Vec2 _lowerBound;	///< the lower vertex
		Vec2 _upperBound;	///< the upper vertex

		public AABB(Vec2 lowerBound, Vec2 upperBound)
		{
			_lowerBound = lowerBound;
			_upperBound = upperBound;
		}

		public Vec2 LowerBound
		{
			get { return _lowerBound; }
			set { _lowerBound = value; }
		}

		public Vec2 UpperBound
		{
			get { return _upperBound; }
			set { _upperBound = value; }
		}

		/// Verify that the bounds are sorted.
		public bool IsValid()
		{
			Vec2 d = _upperBound - _lowerBound;
			bool valid = d.x >= 0.0f && d.y >= 0.0f;
			valid = valid && _lowerBound.IsValid() && _upperBound.IsValid();
			return valid;
		}

		/// Get the center of the AABB.
		public Vec2 GetCenter()
		{
			return 0.5f * (_lowerBound + _upperBound);
		}

		/// Get the extents of the AABB (half-widths).
		public Vec2 GetExtents()
		{
			return 0.5f * (_upperBound - _lowerBound);
		}

		/// Combine two AABBs into this one.
		public void Combine(AABB aabb1, AABB aabb2)
		{
			_lowerBound = Vec2.Min(aabb1._lowerBound, aabb2._lowerBound);
			_upperBound = Vec2.Min(aabb1._upperBound, aabb2._upperBound);
		}

		/// Does this aabb contain the provided AABB.
		public bool Contains(AABB aabb)
		{
			bool result = true;
			result = result && _lowerBound.x <= aabb._lowerBound.x;
			result = result && _lowerBound.y <= aabb._lowerBound.y;
			result = result && aabb._upperBound.x <= _upperBound.x;
			result = result && aabb._upperBound.y <= _upperBound.y;
			return result;
		}

		public static bool TestOverlap(AABB a, AABB b)
		{
			var d1 = b.LowerBound - a.UpperBound;
			var d2 = a.LowerBound - b.UpperBound;

			if (d1.x > 0.0f || d1.y > 0.0f)
				return false;

			if (d2.x > 0.0f || d2.y > 0.0f)
				return false;

			return true;
		}

		public bool Overlaps(AABB b)
		{
			return TestOverlap(this, b);
		}
	};
}
