using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Reflection;

namespace Box2CS
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class RecalculateMassAttribute : Attribute
	{
	}

	public class FilterDataTypeEditor : ExpandableObjectConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				try
				{
					return FilterData.Parse((string)value);
				}
				catch
				{
					throw new Exception("FilterData has wrong format");
				}
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				FilterData data = value as FilterData;
				return data.ToString();
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			return base.IsValid(context, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;

			return base.CanConvertTo(context, destinationType);
		}
	}

	public class UShortHexTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			else
			{
				return base.CanConvertFrom(context, sourceType);
			}
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return true;
			}
			else
			{
				return base.CanConvertTo(context, destinationType);
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value.GetType() == typeof(ushort))
			{
				return string.Format("0x{0:X4}", value);
			}
			else
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value.GetType() == typeof(string))
			{
				string input = (string)value;

				if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
				{
					input = input.Substring(2);
				}

				return ushort.Parse(input, System.Globalization.NumberStyles.HexNumber, culture);
			}
			else
			{
				return base.ConvertFrom(context, culture, value);
			}
		}
	}

	[TypeConverter(typeof(FilterDataTypeEditor))]
	[StructLayout(LayoutKind.Sequential)]
	public class FilterData
	{
		public const ushort DefaultCategoryBits = 0x0001;
		public const ushort DefaultMaskBits = 0xFFFF;

		ushort _categoryBits = DefaultCategoryBits, 
			_maskBits = DefaultMaskBits;
		short _groupIndex = 0;

		public static readonly FilterData Default = new FilterData();

		[Description("The collision category bits. Normally you would just set one bit.")]
		[TypeConverter(typeof(UShortHexTypeConverter))]
		public ushort CategoryBits
		{
			get { return _categoryBits; }
			set { _categoryBits = value; }
		}

		[Description("The collision mask bits. This states the categories that this shape would accept for collision.")]
		[TypeConverter(typeof(UShortHexTypeConverter))]
		public ushort MaskBits
		{
			get { return _maskBits; }
			set { _maskBits = value; }
		}

		[Description("Collision groups allow a certain group of objects to never collide (negative) or always collide (positive). Zero means no collision group. Non-zero group filtering always wins against the mask bits.")]
		public short GroupIndex
		{
			get { return _groupIndex; }
			set { _groupIndex = value; }
		}

		public FilterData()
		{
		}

		public FilterData(ushort categoryBits, ushort maskBits, short groupIndex)
		{
			CategoryBits = categoryBits;
			MaskBits = maskBits;
			GroupIndex = groupIndex;
		}

		public static bool operator ==(FilterData l, FilterData r)
		{
			if ((object)l == null && (object)r == null)
				return true;
			else if ((object)l == null && (object)r != null ||
				(object)l != null && (object)r == null)
				return false;

			return (l.CategoryBits == r.CategoryBits && l.MaskBits == r.MaskBits && l.GroupIndex == r.GroupIndex);
		}

		public static bool operator !=(FilterData l, FilterData r)
		{
			return !(l == r);
		}

		public override bool Equals(object obj)
		{
			if (obj is FilterData)
				return (obj as FilterData) == this;

			return base.Equals(obj);
		}

		public override string ToString()
		{
			return "CategoryBits="+CategoryBits.ToString()+" MaskBits="+MaskBits.ToString()+" GroupIndex="+GroupIndex.ToString();
		}

		public override int GetHashCode()
		{
			return CategoryBits.GetHashCode() + MaskBits.GetHashCode() + GroupIndex.GetHashCode();
		}

		public static FilterData Parse(string value)
		{
			SimpleParser parser = new SimpleParser(value, true);
			return new FilterData(ushort.Parse(parser.ValueFromKey("CategoryBits")), ushort.Parse(parser.ValueFromKey("MaskBits")), short.Parse(parser.ValueFromKey("GroupIndex")));
		}
	}

	public class MassDataTypeEditor : ExpandableObjectConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				try
				{
					return MassData.Parse((string)value);
				}
				catch
				{
					throw new Exception("Invalid format for MassData");
				}
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				MassData data = (MassData)value;
				return data.ToString();
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			return base.IsValid(context, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;

			return base.CanConvertTo(context, destinationType);
		}
	}

	[TypeConverter(typeof(MassDataTypeEditor))]
	[StructLayout(LayoutKind.Sequential)]
	public struct MassData
	{
		public readonly static MassData Empty = new MassData(0, Vec2.Empty, 0);
		public readonly static MassData Recalculate = new MassData(-1, new Vec2(-1, -1), -1);

		[Description("The mass of the body.")]
		public float Mass
		{
			get;
			set;
		}

		[Description("The center of gravity for the body")]
		public Vec2 Center
		{
			get;
			set;
		}

		[Description("Rotational inertia for the body")]
		public float Inertia
		{
			get;
			set;
		}

		public MassData(float mass, Vec2 center, float inertia) :
			this()
		{
			Mass = mass;
			Center = center;
			Inertia = inertia;
		}

		public static bool operator== (MassData left, MassData right)
		{
			return (left.Mass == right.Mass && left.Center == right.Center && left.Inertia == right.Inertia);
		}

		public static bool operator!=(MassData left, MassData right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if (obj is MassData)
				return this == (MassData)obj;

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Mass.GetHashCode() + Center.GetHashCode() + Inertia.GetHashCode();
		}

		public static MassData Parse(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Recalculate;

			SimpleParser parser = new SimpleParser(value, true);
			return new MassData(float.Parse(parser.ValueFromKey("Mass")), Vec2.Parse(parser.ValueFromKey("Center")), float.Parse(parser.ValueFromKey("Inertia")));
		}

		public override string ToString()
		{
			return "Mass="+Mass.ToString()+" Center="+Center.ToString()+" Inertia="+Inertia.ToString();
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

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public sealed class FixtureDef : ICompare<FixtureDef>, ICloneable
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
			this(shape, density, restitution, friction, new FilterData(), isSensor, userData)
		{
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public FixtureDef Clone()
		{
			return new FixtureDef(Shape, Density, Restitution, Friction, Filter, IsSensor, UserData);
		}

		[Category("Other")]
		[Description("User-specific and application-specific data.")]
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

		[RecalculateMassAttribute]
		[Category("Main")]
		[Description("The shape connected to this fixture. This property will cause the mass of the body to be recalculated, if required.")]
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

		[Category("Movement")]
		[Description("The friction of this fixture.")]
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

		[Category("Movement")]
		[Description("The restitution of this fixture.")]
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

		[RecalculateMassAttribute]
		[Category("Movement")]
		[Description("The density of this fixture. This property will cause the mass of the body to be recalculated, if required.")]
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

		[Category("Flags")]
		[Description("True if this fixture is a sensor shape (no contacts).")]
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

		[Category("Filtering")]
		[Description("The filtering data for this fixture.")]
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

		public bool CompareWith(FixtureDef fixture)
		{
			return (this.Density == fixture.Density &&
				this.Filter == fixture.Filter &&
				this.Friction == fixture.Friction &&
				this.IsSensor == fixture.IsSensor &&
				this.Restitution == fixture.Restitution && 
				this.Shape.CompareWith(fixture.Shape) &&
				this.UserData == fixture.UserData);
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
			public static extern void b2fixture_getfilterdata(IntPtr fixture, [Out] [In] FilterData filterData);

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

		[Browsable(false)]
		public ShapeType ShapeType
		{
			get { return (ShapeType)NativeMethods.b2fixture_gettype(_fixturePtr); }
		}

		public Shape Shape
		{
			get
			{
				switch (ShapeType)
				{
				case ShapeType.Circle:
					{
						cb2circleshapeportable shape = new cb2circleshapeportable();

						using (var ptr = new StructToPtrMarshaller(shape))
						{
							NativeMethods.b2fixture_getshape(_fixturePtr, ptr.Pointer);
							shape = (cb2circleshapeportable)ptr.GetValue(typeof(cb2circleshapeportable));
						}

						return new CircleShape(shape);
					}
				case ShapeType.Polygon:
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
			get { FilterData temp = new FilterData(); NativeMethods.b2fixture_getfilterdata(_fixturePtr, temp); return temp; }
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
