using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	[StructLayout(LayoutKind.Sequential)]
	public struct FilterData
	{
		public static FilterData Default = new FilterData(0x0001, 0xFFFF, 0);

		ushort _categoryBits, _maskBits;
		short _groupIndex;

		public ushort CategoryBits
		{
			get { return _categoryBits; }
			set { _categoryBits = value; }
		}

		public ushort MaskBits
		{
			get { return _maskBits; }
			set { _maskBits = value; }
		}

		public short GroupIndex
		{
			get { return _groupIndex; }
			set { _groupIndex = value; }
		}

		public FilterData(ushort categoryBits, ushort maskBits, short groupIndex)
		{
			_categoryBits = categoryBits;
			_maskBits = maskBits;
			_groupIndex = groupIndex;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MassData
	{
		/// The mass of the shape, usually in kilograms.
		float _mass;

		/// The position of the shape's centroid relative to the shape's origin.
		Vec2 _center;

		/// The rotational inertia of the shape about the local origin.
		float _I;

		public MassData(float mass, Vec2 center, float inertia)
		{
			_mass = mass;
			_center = center;
			_I = inertia;
		}

		public float Mass
		{
			get { return _mass; }
			set { _mass = value; }
		}

		public Vec2 Center
		{
			get { return _center; }
			set { _center = value; }
		}

		public float I
		{
			get { return _I; }
			set { _I = value; }
		}
	};

	internal struct FixtureDefInternal : IFixedSize
	{
		int IFixedSize.FixedSize()
		{
			return Marshal.SizeOf(typeof(FixtureDefInternal));
		}

		void IFixedSize.Lock()
		{
		}

		void IFixedSize.Unlock()
		{
		}

		public IntPtr _shape;
		public IntPtr _userData;
		public float _friction;
		public float _restitution;
		public float _density;
		[MarshalAs(UnmanagedType.U1)]
		public bool _isSensor;
		public FilterData _filter;
	}

	public sealed class FixtureDef
	{
		FixtureDefInternal _internalFixture = new FixtureDefInternal();
		// private, unrelated data
		Shape _realShape;

		internal FixtureDefInternal Internal
		{
			get { return _internalFixture; }
		}

		public FixtureDef() :
			this(null)
		{
		}

		public FixtureDef(Shape shape, float density, float restitution, float friction, FilterData filter, bool isSensor = false, object userData = null)
		{
			_realShape = shape;
			UserData = userData;
			_internalFixture._friction = friction;
			_internalFixture._restitution = restitution;
			_internalFixture._density = density;
			Filter = filter;
			_internalFixture._isSensor = isSensor;
		}

		public FixtureDef(Shape shape, float density = 0.0f, float restitution = 0.0f, float friction = 0.2f, bool isSensor = false, object userData = null) :
			this(shape, density, restitution, friction, FilterData.Default, isSensor, userData)
		{
		}

		public object UserData
		{
			get
			{
				return UserDataStorage.FixtureStorage.ObjectFromHandle(UserDataStorage.IntPtrToHandle(_internalFixture._userData));
			}

			set
			{
				var ptr = UserDataStorage.IntPtrToHandle(_internalFixture._userData);

				if (ptr != 0)
					UserDataStorage.FixtureStorage.UnpinObject(ptr);

				if (value != null)
				{
					var handle = UserDataStorage.FixtureStorage.PinDataToHandle(value);
					_internalFixture._userData = UserDataStorage.HandleToIntPtr(handle);
				}
				else
					_internalFixture._userData = IntPtr.Zero;
			}
		}

		public Shape Shape
		{
			get
			{
				return _realShape;
			}

			set
			{
				_realShape = value;
			}
		}

		public float Friction
		{
			get
			{
				return _internalFixture._friction;
			}

			set
			{
				_internalFixture._friction = value;
			}
		}

		public float Restitution
		{
			get
			{
				return _internalFixture._restitution;
			}

			set
			{
				_internalFixture._restitution = value;
			}
		}

		public float Density
		{
			get
			{
				return _internalFixture._density;
			}

			set
			{
				_internalFixture._density = value;
			}
		}

		public bool IsSensor
		{
			get
			{
				return _internalFixture._isSensor;
			}

			set
			{
				_internalFixture._isSensor = value;
			}
		}

		public FilterData Filter
		{
			get
			{
				return _internalFixture._filter;
			}

			set
			{
				_internalFixture._filter = value;
			}
		}

		internal void SetShape(IntPtr intPtr)
		{
			_internalFixture._shape = intPtr;
		}
	}

	public sealed class Fixture
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern int b2fixture_gettype(IntPtr fixture);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2fixture_getshape(IntPtr fixture, IntPtr shape);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2fixture_setsensor(IntPtr fixture, [MarshalAs(UnmanagedType.U1)] bool val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static extern bool b2fixture_getsensor(IntPtr fixture);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2fixture_setfilterdata(IntPtr fixture, FilterData val);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2fixture_getfilterdata(IntPtr fixture, out FilterData filterData);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2fixture_getbody(IntPtr fixture);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2fixture_getnext(IntPtr fixture);

			[DllImport(Box2DSettings.Box2CDLLName)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static extern bool b2fixture_testpoint(IntPtr fixture, Vec2 point);

			[DllImport(Box2DSettings.Box2CDLLName)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static extern bool b2fixture_raycast(IntPtr fixture, out RayCastOutput output, RayCastInput input);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2fixture_getmassdata(IntPtr fixture, out MassData data);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2fixture_getdensity(IntPtr fixture);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2fixture_setdensity(IntPtr fixture, float density);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2fixture_getfriction(IntPtr fixture);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2fixture_setfriction(IntPtr fixture, float density);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2fixture_getrestitution(IntPtr fixture);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2fixture_setrestitution(IntPtr fixture, float density);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2fixture_getaabb(IntPtr fixture, out AABB outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2fixture_setuserdata(IntPtr fixture, IntPtr data);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2fixture_getuserdata(IntPtr fixture);
		}

		IntPtr _fixturePtr;

		internal IntPtr FixturePtr
		{
			get { return _fixturePtr; }
		}

		internal Fixture(IntPtr fixture)
		{
			_fixturePtr = fixture;
		}

		internal static Fixture FromPtr(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
				return null;

			return new Fixture(ptr);
		}

		public EShapeType ShapeType
		{
			get { return (EShapeType)NativeMethods.b2fixture_gettype(_fixturePtr); }
		}

		public Shape Shape
		{
			get
			{
				switch (ShapeType)
				{
				case EShapeType.e_circle:
					{
						cb2circleshapeportable shape = new cb2circleshapeportable();

						using (var ptr = new StructToPtrMarshaller(shape))
						{
							NativeMethods.b2fixture_getshape(_fixturePtr, ptr.Pointer);
							shape = (cb2circleshapeportable)ptr.GetValue(typeof(cb2circleshapeportable));
						}

						return new CircleShape(shape);
					}
				case EShapeType.e_polygon:
					{
						cb2polygonshapeportable shape = new cb2polygonshapeportable();

						using (var ptr = new StructToPtrMarshaller(shape))
						{
							NativeMethods.b2fixture_getshape(_fixturePtr, ptr.Pointer);
							shape = (cb2polygonshapeportable)ptr.GetValue(typeof(cb2polygonshapeportable));
						}

						return new PolygonShape(shape);
					}
				}

				throw new InvalidOperationException();
			}
		}

		public object UserData
		{
			get
			{
				return UserDataStorage.FixtureStorage.ObjectFromHandle(UserDataStorage.IntPtrToHandle(NativeMethods.b2fixture_getuserdata(_fixturePtr)));
			}

			set
			{
				var ptr = UserDataStorage.IntPtrToHandle(NativeMethods.b2fixture_getuserdata(_fixturePtr));

				if (ptr != 0)
					UserDataStorage.FixtureStorage.UnpinObject(ptr);

				if (value != null)
				{
					var handle = UserDataStorage.FixtureStorage.PinDataToHandle(value);
					NativeMethods.b2fixture_setuserdata(_fixturePtr, UserDataStorage.HandleToIntPtr(handle));
				}
				else
					NativeMethods.b2fixture_setuserdata(_fixturePtr, IntPtr.Zero);
			}
		}

		public bool IsSensor
		{
			get { return NativeMethods.b2fixture_getsensor(_fixturePtr); }
			set { NativeMethods.b2fixture_setsensor(_fixturePtr, value); }
		}

		public FilterData FilterData
		{
			get { FilterData temp; NativeMethods.b2fixture_getfilterdata(_fixturePtr, out temp); return temp; }
			set { NativeMethods.b2fixture_setfilterdata(_fixturePtr, value); }
		}

		public Body Body
		{
			get { return Body.FromPtr(NativeMethods.b2fixture_getbody(_fixturePtr)); }
		}

		public Fixture Next
		{
			get { return Fixture.FromPtr(NativeMethods.b2fixture_getnext(_fixturePtr)); }
		}

		public bool TestPoint(Vec2 Point)
		{
			return NativeMethods.b2fixture_testpoint(_fixturePtr, Point);
		}

		public bool RayCast(out RayCastOutput Output, RayCastInput Input)
		{
			Output = new RayCastOutput();
			var returnVal = NativeMethods.b2fixture_raycast(_fixturePtr, out Output, Input);
			return returnVal;
		}

		public MassData MassData
		{
			get
			{
				MassData returnVal = new MassData();
				NativeMethods.b2fixture_getmassdata(_fixturePtr, out returnVal);

				return returnVal;
			}
		}

		public float Density
		{
			get { return NativeMethods.b2fixture_getdensity(_fixturePtr); }
			set { NativeMethods.b2fixture_setdensity(_fixturePtr, value); }
		}

		public float Friction
		{
			get { return NativeMethods.b2fixture_getfriction(_fixturePtr); }
			set { NativeMethods.b2fixture_setfriction(_fixturePtr, value); }
		}

		public float Restitution
		{
			get { return NativeMethods.b2fixture_getrestitution(_fixturePtr); }
			set { NativeMethods.b2fixture_setrestitution(_fixturePtr, value); }
		}

		public AABB AABB
		{
			get { AABB temp; NativeMethods.b2fixture_getaabb(_fixturePtr, out temp); return temp; }
		}

		public static bool operator ==(Fixture l, Fixture r)
		{
			if ((object)l == null && (object)r == null)
				return true;
			else if ((object)l == null && (object)r != null ||
				(object)l != null && (object)r == null)
				return false;

			return (l.FixturePtr == r.FixturePtr);
		}

		public static bool operator !=(Fixture l, Fixture r)
		{
			return !(l == r);
		}

		public override bool Equals(object obj)
		{
			if (obj is Fixture)
				return (obj as Fixture) == this;

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return FixturePtr.GetHashCode();
		}
	}

}
