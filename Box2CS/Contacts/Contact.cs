using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	[StructLayout(LayoutKind.Explicit)]
	public struct ContactID
	{
		[FieldOffset(0)]
		public byte ReferenceEdge;	///< The edge that defines the outward contact normal.
		[FieldOffset(1)]
		public byte IncidentEdge;		///< The edge most anti-parallel to the reference edge.
		[FieldOffset(2)]
		public byte IncidentVertex;	///< The vertex (0 or 1) on the incident edge that was clipped.
		[FieldOffset(3)]
		public byte Flip;				///< A value of 1 indicates that the reference edge is on shape2.

		[FieldOffset(0)]
		public uint Key;

		public ContactID(uint key)
		{
			ReferenceEdge = IncidentEdge = IncidentVertex = Flip = 0;
			Key = key;
		}

		public ContactID(byte referenceEdge, byte incidentEdge, byte incidentVertex, byte flip)
		{
			Key = 0;
			ReferenceEdge = referenceEdge;
			IncidentEdge = incidentEdge;
			IncidentVertex = incidentVertex;
			Flip = flip;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ManifoldPoint
	{
		public Vec2 LocalPoint;		///< usage depends on manifold type
		public float NormalImpulse;	///< the non-penetration impulse
		public float TangentImpulse;	///< the friction impulse
		public ContactID ID;			///< uniquely identifies a contact point between two shapes
	}

	public enum EManifoldType
	{
		e_circles,
		e_faceA,
		e_faceB
	};

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Manifold
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst=Box2DSettings.b2_maxManifoldPoints, ArraySubType=UnmanagedType.Struct)]
		public ManifoldPoint[] Points;	///< the points of contact
		public Vec2 LocalNormal;								///< not use for Type::e_points
		public Vec2 LocalPoint;								///< usage depends on manifold type
		public EManifoldType ManifoldType;
		public int PointCount;								///< the number of manifold points
	
		public static void GetPointStates(ref EPointState[] state1, ref EPointState[] state2,
					  ref Manifold manifold1, ref Manifold manifold2)

		{
			for (int i = 0; i < Box2DSettings.b2_maxManifoldPoints; ++i)
			{
				state1[i] = EPointState.b2_nullState;
				state2[i] = EPointState.b2_nullState;
			}

			// Detect persists and removes.
			for (int i = 0; i < manifold1.PointCount; ++i)
			{
				ContactID id = manifold1.Points[i].ID;

				state1[i] = EPointState.b2_removeState;

				for (int j = 0; j < manifold2.PointCount; ++j)
				{
					if (manifold2.Points[j].ID.Key == id.Key)
					{
						state1[i] = EPointState.b2_persistState;
						break;
					}
				}
			}

			// Detect persists and adds.
			for (int i = 0; i < manifold2.PointCount; ++i)
			{
				ContactID id = manifold2.Points[i].ID;

				state2[i] = EPointState.b2_addState;

				for (int j = 0; j < manifold1.PointCount; ++j)
				{
					if (manifold1.Points[j].ID.Key == id.Key)
					{
						state2[i] = EPointState.b2_persistState;
						break;
					}
				}
			}
		}
	}

	public unsafe struct WorldManifold
	{
		public Vec2 Normal;						///< world vector pointing from A to B

		[MarshalAs(UnmanagedType.ByValArray, SizeConst=Box2DSettings.b2_maxManifoldPoints, ArraySubType=UnmanagedType.Struct)]
		public Vec2[] Points;	///< world contact point (point of intersection)
	}

	/// This is used for determining the state of contact points.
	public enum EPointState
	{
		b2_nullState,		///< point does not exist
		b2_addState,		///< point was added in the update
		b2_persistState,	///< point persisted across the update
		b2_removeState		///< point was removed in the update
	};

	public sealed class Contact
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2contact_getmanifold(IntPtr contact);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2contact_getworldmanifold(IntPtr contact, out WorldManifold worldManifold);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2contact_istouching(IntPtr contact);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2contact_setenabled(IntPtr contact, bool flag);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2contact_getenabled(IntPtr contact);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2contact_getnext(IntPtr contact);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2contact_getfixturea(IntPtr contact);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2contact_getfixtureb(IntPtr contact);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2contact_evaluate(IntPtr contact, out Manifold outManifold, Transform xfA, Transform xfB);
		}

		IntPtr _contactPtr;

		internal Contact(IntPtr ptr)
		{
			_contactPtr = ptr;
		}

		internal static Contact FromPtr(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
				return null;

			return new Contact(ptr);
		}

		public Manifold Manifold
		{
			get
			{
				return (Manifold)Marshal.PtrToStructure(NativeMethods.b2contact_getmanifold(_contactPtr), typeof(Manifold));
			}
		}

		public WorldManifold WorldManifold
		{
			get
			{
				var manifold = new WorldManifold();
				NativeMethods.b2contact_getworldmanifold(_contactPtr, out manifold);

				return manifold;
			}
		}

		public bool IsTouching
		{
			get { return NativeMethods.b2contact_istouching(_contactPtr); }
		}

		public bool Enabled
		{
			get { return NativeMethods.b2contact_getenabled(_contactPtr); }
			set { NativeMethods.b2contact_setenabled(_contactPtr, value); }
		}

		public Contact Next
		{
			get { return Contact.FromPtr(NativeMethods.b2contact_getnext(_contactPtr)); }
		}

		public Fixture FixtureA
		{
			get { return Fixture.FromPtr(NativeMethods.b2contact_getfixturea(_contactPtr)); }
		}

		public Fixture FixtureB
		{
			get { return Fixture.FromPtr(NativeMethods.b2contact_getfixtureb(_contactPtr)); }
		}

		public void Evaluate(out Manifold manifold, Transform xfA, Transform xfB)
		{
			manifold = new Manifold();

			NativeMethods.b2contact_evaluate(_contactPtr, out manifold, xfA, xfB);
		}

		public static bool operator ==(Contact l, Contact r)
		{
			if ((object)l == null && (object)r == null)
				return true;
			else if ((object)l == null && (object)r != null ||
				(object)l != null && (object)r == null)
				return false;

			return l._contactPtr == r._contactPtr;
		}

		public static bool operator !=(Contact l, Contact r)
		{
			return !(l == r);
		}

		public override bool Equals(object obj)
		{
			if (obj is Contact)
				return (obj as Contact) == this;

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return _contactPtr.GetHashCode();
		}
	}
}
