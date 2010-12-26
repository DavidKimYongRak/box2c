﻿using System;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Box2CS
{
	[StructLayout(LayoutKind.Explicit)]
	public struct ContactID
	{
		[FieldOffset(0)]
		byte _referenceEdge;

		[FieldOffset(1)]
		byte _incidentEdge;

		[FieldOffset(2)]
		byte _incidentVertex;

		[FieldOffset(3)]
		byte _flip;

		[FieldOffset(0)]
		uint _key;

		public byte ReferenceEdge
		{
			get { return _referenceEdge; }
			set { _referenceEdge = value; }
		}

		public byte IncidentEdge
		{
			get { return _incidentEdge; }
			set { _incidentEdge = value; }
		}

		public byte IncidentVertex
		{
			get { return _incidentVertex; }
			set { _incidentVertex = value; }
		}

		public byte Flip
		{
			get { return _flip; }
			set { _flip = value; }
		}

		public uint Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public ContactID(uint key) :
			this()
		{
			ReferenceEdge = IncidentEdge = IncidentVertex = Flip = 0;
			Key = key;
		}

		public ContactID(byte referenceEdge, byte incidentEdge, byte incidentVertex, byte flip) :
			this()
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
		public Vec2 LocalPoint
		{
			get;
			set;
		}

		public float NormalImpulse
		{
			get;
			set;
		}

		public float TangentImpulse
		{
			get;
			set;
		}

		public ContactID ID
		{
			get;
			set;
		}
	}

	public enum ManifoldType
	{
		Circles,
		FaceA,
		FaceB
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct Manifold
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst=Box2DSettings.b2_maxManifoldPoints, ArraySubType=UnmanagedType.Struct)]
		ManifoldPoint[] _points;

		Vec2 _localNormal;
		Vec2 _localPoint;
		ManifoldType _manifoldType;
		int _pointCount;

		public ManifoldPoint[] Points
		{
			get { return _points; }
			set { _points = value; }
		}

		public Vec2 LocalNormal
		{
			get { return _localNormal; }
			set { _localNormal = value; }
		}

		public Vec2 LocalPoint
		{
			get { return _localPoint; }
			set { _localPoint = value; }
		}

		public ManifoldType ManifoldType
		{
			get { return _manifoldType; }
			set { _manifoldType = value; }
		}

		public int PointCount
		{
			get { return _pointCount; }
			set { _pointCount = value; }
		}
	
		public static void GetPointStates(ref PointState[] state1, ref PointState[] state2,
					  ref Manifold manifold1, ref Manifold manifold2)

		{
			for (int i = 0; i < Box2DSettings.b2_maxManifoldPoints; ++i)
			{
				state1[i] = PointState.Null;
				state2[i] = PointState.Null;
			}

			// Detect persists and removes.
			for (int i = 0; i < manifold1.PointCount; ++i)
			{
				ContactID id = manifold1.Points[i].ID;

				state1[i] = PointState.Remove;

				for (int j = 0; j < manifold2.PointCount; ++j)
				{
					if (manifold2.Points[j].ID.Key == id.Key)
					{
						state1[i] = PointState.Persist;
						break;
					}
				}
			}

			// Detect persists and adds.
			for (int i = 0; i < manifold2.PointCount; ++i)
			{
				ContactID id = manifold2.Points[i].ID;

				state2[i] = PointState.Add;

				for (int j = 0; j < manifold1.PointCount; ++j)
				{
					if (manifold1.Points[j].ID.Key == id.Key)
					{
						state2[i] = PointState.Persist;
						break;
					}
				}
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WorldManifold
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst=Box2DSettings.b2_maxManifoldPoints, ArraySubType=UnmanagedType.Struct)]
		Vec2[] _points;
		Vec2 _normal;

		public Vec2[] Points
		{
			get { return _points; }
			set { _points = value; }
		}

		public Vec2 Normal
		{
			get { return _normal; }
			set { _normal = value; }
		}
	}

	/// This is used for determining the state of contact points.
	public enum PointState
	{
		Null,		///< point does not exist
		Add,		///< point was added in the update
		Persist,	///< point persisted across the update
		Remove		///< point was removed in the update
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
			[return: MarshalAs(UnmanagedType.U1)]
			public static extern bool b2contact_istouching(IntPtr contact);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2contact_setenabled(IntPtr contact, [MarshalAs(UnmanagedType.U1)] bool flag);

			[DllImport(Box2DSettings.Box2CDLLName)]
			[return: MarshalAs(UnmanagedType.U1)]
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

		[EnvironmentPermissionAttribute(SecurityAction.Demand, Unrestricted=true)]
		internal Manifold ManifoldFromThing()
		{
			return (Manifold)Marshal.PtrToStructure(NativeMethods.b2contact_getmanifold(_contactPtr), typeof(Manifold));
		}

		public Manifold Manifold
		{
			get { return ManifoldFromThing(); }
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
