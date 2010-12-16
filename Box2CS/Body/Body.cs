using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Box2CS
{
	public enum EBodyType
	{
		b2_staticBody = 0,
		b2_kinematicBody,
		b2_dynamicBody,
	};

	[StructLayout(LayoutKind.Sequential)]
	public class BodyDef : IFixedSize
	{
		int IFixedSize.FixedSize()
		{
			return Marshal.SizeOf(typeof(BodyDef));
		}

		void IFixedSize.Lock()
		{
		}

		void IFixedSize.Unlock()
		{
		}

		[MarshalAs(UnmanagedType.I4)]
		EBodyType _type;

		Vec2 _position;
		float _angle;
		Vec2 _linearVelocity;
		float _angularVelocity;
		float _linearDamping;
		float _angularDamping;

		[MarshalAs(UnmanagedType.U1)]
		bool _allowSleep;

		[MarshalAs(UnmanagedType.U1)]
		bool _awake;

		[MarshalAs(UnmanagedType.U1)]
		bool _fixedRotation;

		[MarshalAs(UnmanagedType.U1)]
		bool _bullet;

		[MarshalAs(UnmanagedType.U1)]
		bool _active;

		IntPtr _userData;
		float _inertiaScale;

		public BodyDef(
			EBodyType bodyType,
			Vec2 position,
			float angle,
			Vec2 linearVelocity,
			float angularVelocity = 0.0f,
			float linearDamping = 0.0f,
			float angularDamping = 0.0f,
			bool bullet = false,
			bool active = true,
			bool fixedRotation = false,
			bool allowSleep = true,
			bool awake = true,
			float inertiaScale = 1.0f,
			object userData = null)
		{
			UserData = userData;
			_position = position;
			_angle = angle;
			_linearVelocity = linearVelocity;
			_angularVelocity = angularVelocity;
			_linearDamping = linearDamping;
			_angularDamping = angularDamping;
			_allowSleep = allowSleep;
			_awake = awake;
			_fixedRotation = fixedRotation;
			_bullet = bullet;
			_type = bodyType;
			_active = active;
			_inertiaScale = inertiaScale;
		}

		public BodyDef(
			EBodyType bodyType,
			Vec2 position,
			float angle = 0,
			bool bullet = false,
			float angularVelocity = 0.0f,
			float linearDamping = 0.0f,
			float angularDamping = 0.0f,
			bool active = true,
			bool fixedRotation = false,
			bool allowSleep = true,
			bool awake = true,
			float inertiaScale = 1.0f,
			object userData = null) :
			this(bodyType, position, angle, Vec2.Empty, angularVelocity, linearDamping, angularDamping, bullet, active, fixedRotation, allowSleep,
			awake, inertiaScale, userData)
		{
		}

		public BodyDef(
			EBodyType bodyType,
			Vec2 position,
			float angle,
			Vec2 linearVelocity,
			bool bullet = false,
			bool active = true,
			bool fixedRotation = false,
			bool allowSleep = true,
			bool awake = true,
			float inertiaScale = 1.0f,
			object userData = null) :
			this(bodyType, position, angle, linearVelocity, 0.0f, 0.0f, 0.0f, bullet, active, fixedRotation, allowSleep,
			awake, inertiaScale, userData)
		{
		}

		public BodyDef() :
			this(EBodyType.b2_staticBody, Vec2.Empty, 0.0f, Vec2.Empty, 0.0f, 0.0f, 0.0f, false, true, false, true, true, 1.0f, null)
		{
		}

		public object UserData
		{
			get { return UserDataStorage.BodyStorage.ObjectFromHandle(UserDataStorage.IntPtrToHandle(_userData)); }

			set
			{
				var ptr = UserDataStorage.IntPtrToHandle(_userData);

				if (ptr != 0)
					UserDataStorage.BodyStorage.UnpinObject(ptr);

				if (value != null)
				{
					var handle = UserDataStorage.BodyStorage.PinDataToHandle(value);
					_userData = UserDataStorage.HandleToIntPtr(handle);
				}
				else
					_userData = IntPtr.Zero;
			}
		}

		public Vec2 Position
		{
			get { return _position; }
			set { _position = value; }
		}

		public float Angle
		{
			get { return _angle; }
			set { _angle = value; }
		}

		public Vec2 LinearVelocity
		{
			get { return _linearVelocity; }
			set { _linearVelocity = value; }
		}

		public float LinearDamping
		{
			get { return _linearDamping; }
			set { _linearDamping = value; }
		}

		public float AngularDamping
		{
			get { return _angularDamping; }
			set { _angularDamping = value; }
		}

		public bool AllowSleep
		{
			get { return _allowSleep; }
			set { _allowSleep = value; }
		}

		public bool Awake
		{
			get { return _awake; }
			set { _awake = value; }
		}

		public bool FixedRotation
		{
			get { return _fixedRotation; }
			set { _fixedRotation = value; }
		}

		public bool Bullet
		{
			get { return _bullet; }
			set { _bullet = value; }
		}

		public EBodyType BodyType
		{
			get { return _type; }
			set { _type = value; }
		}

		public bool Active
		{
			get { return _active; }  
			set { _active = value; }
		}

		public float InertiaScale
		{
			get { return _inertiaScale; }
			set { _inertiaScale = value; }
		}
	}

	public class Body
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2body_createfixture(IntPtr body, IntPtr fixtureDef);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2body_createfixturefromshape(IntPtr body, IntPtr shape, float density);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_destroyfixture(IntPtr body, IntPtr fixture);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_settransform(IntPtr body, Vec2 position, float angle);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_getposition(IntPtr body, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2body_getangle(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_getworldcenter(IntPtr body, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_getlocalcenter(IntPtr body, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_setlinearvelocity(IntPtr body, Vec2 vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_getlinearvelocity(IntPtr body, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_setangularvelocity(IntPtr body, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2body_getangularvelocity(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_applyforce(IntPtr body, Vec2 force, Vec2 point);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_applytorque(IntPtr body, float torque);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_applylinearimpulse(IntPtr body, Vec2 force, Vec2 point);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_applyangularimpulse(IntPtr body, float torque);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2body_getmass(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2body_getinertia(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_getmassdata(IntPtr body, out MassData data);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_setmassdata(IntPtr body, out MassData data);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_resetmassdata(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_getworldpoint(IntPtr body, Vec2 localPoint, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_getworldvector(IntPtr body, Vec2 localPoint, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_getlocalpoint(IntPtr body, Vec2 localPoint, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_getlocalvector(IntPtr body, Vec2 localPoint, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_getlinearvelocityfromworldvector(IntPtr body, Vec2 localPoint, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_getlinearvelocityfromlocalvector(IntPtr body, Vec2 localPoint, out Vec2 outPtr);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_setlineardamping(IntPtr body, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2body_getlineardamping(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_setangulardamping(IntPtr body, float vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern float b2body_getangulardamping(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_settype(IntPtr body, int vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern int b2body_gettype(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_setbullet(IntPtr body, bool vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2body_getbullet(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_setissleepingallowed(IntPtr body, bool vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2body_getissleepingallowed(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_setawake(IntPtr body, bool vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2body_getawake(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_setactive(IntPtr body, bool vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2body_getactive(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2body_setfixedrotation(IntPtr body, bool vec);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern bool b2body_getfixedrotation(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2body_getfixturelist(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2body_getjointlist(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2body_getcontactlist(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2body_getnext(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2body_getuserdata(IntPtr body);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2body_setuserdata(IntPtr body, IntPtr data);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2body_getworld(IntPtr body);
		}

		IntPtr _bodyPtr;

		internal Body(IntPtr ptr)
		{
			_bodyPtr = ptr;
		}

		internal IntPtr BodyPtr
		{
			get { return _bodyPtr; }
		}

		internal static Body FromPtr(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
				return null;

			return new Body(ptr);
		}

		public Fixture CreateFixture(FixtureDef def)
		{
			def.SetShape(def.Shape.Lock());
			Fixture fixture;
			using (StructToPtrMarshaller<FixtureDefInternal> structPtr = new StructToPtrMarshaller<FixtureDefInternal>(def.Internal))
				fixture = Fixture.FromPtr(NativeMethods.b2body_createfixture(_bodyPtr, structPtr.Pointer));
			def.Shape.Unlock();
			return fixture;
		}

		public Fixture CreateFixture(Shape shape, float density)
		{
			var ptr = shape.Lock();
			var fix = Fixture.FromPtr(NativeMethods.b2body_createfixturefromshape(_bodyPtr, ptr, density));
			shape.Unlock();
			return fix;
		}

		public void DestroyFixture(Fixture fixture)
		{
			NativeMethods.b2body_destroyfixture(_bodyPtr, fixture.FixturePtr);
		}

		public void SetTransform(Vec2 position, float angle)
		{
			NativeMethods.b2body_settransform(_bodyPtr, position, angle);
		}

		public Vec2 Position
		{
			get { Vec2 temp; NativeMethods.b2body_getposition(_bodyPtr, out temp); return temp; }
			set { SetTransform(value, Angle); }
		}

		public float Angle
		{
			get { return NativeMethods.b2body_getangle(_bodyPtr); }
			set { SetTransform(Position, value); }
		}

		public Vec2 WorldCenter
		{
			get { Vec2 temp; NativeMethods.b2body_getworldcenter(_bodyPtr, out temp); return temp; }
		}

		public Vec2 LocalCenter
		{
			get { Vec2 temp; NativeMethods.b2body_getlocalcenter(_bodyPtr, out temp); return temp; }
		}

		public Vec2 LinearVelocity
		{
			get { Vec2 temp; NativeMethods.b2body_getlinearvelocity(_bodyPtr, out temp); return temp; }
			set { NativeMethods.b2body_setlinearvelocity(_bodyPtr, value); }
		}

		public float AngularVelocity
		{
			get { return NativeMethods.b2body_getangularvelocity(_bodyPtr); }
			set { NativeMethods.b2body_setangularvelocity(_bodyPtr, value); }
		}

		public Body Next
		{
			get { return Body.FromPtr(NativeMethods.b2body_getnext(_bodyPtr)); }
		}

		public void ApplyForce(Vec2 force, Vec2 point)
		{
			NativeMethods.b2body_applyforce(_bodyPtr, force, point);
		}

		public void ApplyTorque(float torque)
		{
			NativeMethods.b2body_applytorque(_bodyPtr, torque);
		}

		public void ApplyLinearImpulse(Vec2 force, Vec2 point)
		{
			NativeMethods.b2body_applylinearimpulse(_bodyPtr, force, point);
		}

		public void ApplyAngularImpulse(float impulse)
		{
			NativeMethods.b2body_applyangularimpulse(_bodyPtr, impulse);
		}

		public float Mass
		{
			get { return NativeMethods.b2body_getmass(_bodyPtr); }
			set
			{
				var x = MassData;
				MassData = new MassData(value, x.Value.Center, x.Value.I);
			}
		}

		public float Inertia
		{
			get { return NativeMethods.b2body_getinertia(_bodyPtr); }
		}

		public float InvMass
		{
			get { return 1.0f / Mass; }
		}

		public float InvInertia
		{
			get { return 1.0f / Inertia; }
		}

		public MassData? MassData
		{
			get
			{
				MassData returnVal = new MassData();

				NativeMethods.b2body_getmassdata(_bodyPtr, out returnVal);

				return returnVal;
			}

			set
			{
				if (value == null)
				{
					NativeMethods.b2body_resetmassdata(_bodyPtr);
					return;
				}

				MassData mb = value.Value;
				NativeMethods.b2body_setmassdata(_bodyPtr, out mb);
			}
		}

		public Vec2 GetWorldPoint(Vec2 localPoint)
		{
			Vec2 temp;
			NativeMethods.b2body_getworldpoint(_bodyPtr, localPoint, out temp);
			return temp;
		}

		public Vec2 GetWorldVector(Vec2 localPoint)
		{
			Vec2 temp;
			NativeMethods.b2body_getworldvector(_bodyPtr, localPoint, out temp);
			return temp;
		}

		public Vec2 GetLocalPoint(Vec2 worldPoint)
		{
			Vec2 temp;
			NativeMethods.b2body_getlocalpoint(_bodyPtr, worldPoint, out temp);
			return temp;
		}

		public Vec2 GetLocalVector(Vec2 worldPoint)
		{
			Vec2 temp;
			NativeMethods.b2body_getlocalvector(_bodyPtr, worldPoint, out temp);
			return temp;
		}

		public Vec2 GetLinearVelocityFromWorldVector(Vec2 localPoint)
		{
			Vec2 temp;
			NativeMethods.b2body_getlinearvelocityfromworldvector(_bodyPtr, localPoint, out temp);
			return temp;
		}

		public Vec2 GetLinearVelocityFromLocalVector(Vec2 localPoint)
		{
			Vec2 temp;
			NativeMethods.b2body_getlinearvelocityfromlocalvector(_bodyPtr, localPoint, out temp);
			return temp;
		}

		public float LinearDamping
		{
			get { return NativeMethods.b2body_getlineardamping(_bodyPtr); }
			set { NativeMethods.b2body_setlineardamping(_bodyPtr, value); }
		}

		public float AngularDamping
		{
			get { return NativeMethods.b2body_getangulardamping(_bodyPtr); }
			set { NativeMethods.b2body_setangulardamping(_bodyPtr, value); }
		}

		public EBodyType BodyType
		{
			get { return (EBodyType)NativeMethods.b2body_gettype(_bodyPtr); }
			set { NativeMethods.b2body_settype(_bodyPtr, (int)value); }
		}

		public bool IsBullet
		{
			get { return NativeMethods.b2body_getbullet(_bodyPtr); }
			set { NativeMethods.b2body_setbullet(_bodyPtr, value); }
		}

		public bool IsSleepingAllowed
		{
			get { return NativeMethods.b2body_getissleepingallowed(_bodyPtr); }
			set { NativeMethods.b2body_setissleepingallowed(_bodyPtr, value); }
		}

		public bool IsAwake
		{
			get { return NativeMethods.b2body_getawake(_bodyPtr); }
			set { NativeMethods.b2body_setawake(_bodyPtr, value); }
		}

		public bool IsActive
		{
			get { return NativeMethods.b2body_getactive(_bodyPtr); }
			set { NativeMethods.b2body_setactive(_bodyPtr, value); }
		}

		public bool IsFixedRotation
		{
			get { return NativeMethods.b2body_getfixedrotation(_bodyPtr); }
			set { NativeMethods.b2body_setfixedrotation(_bodyPtr, value); }
		}

		public Fixture FixtureList
		{
			get { return Fixture.FromPtr(NativeMethods.b2body_getfixturelist(_bodyPtr)); }
		}

		public IEnumerable<Fixture> Fixtures
		{
			get
			{
				for (var fixture = FixtureList; fixture != null; fixture = fixture.Next)
					yield return fixture;
			}
		}

		public JointEdge JointList
		{
			get { return JointEdge.FromPtr(NativeMethods.b2body_getjointlist(_bodyPtr)); }
		}

		public IEnumerable<JointEdge> Joints
		{
			get
			{
				for (var jointEdge = JointList; jointEdge != null; jointEdge = jointEdge.Next)
					yield return jointEdge;
			}
		}

		public ContactEdge ContactList
		{
			get { return ContactEdge.FromPtr(NativeMethods.b2body_getcontactlist(_bodyPtr)); }
		}

		public IEnumerable<ContactEdge> Contacts
		{
			get
			{
				for (var contactEdge = ContactList; contactEdge != null; contactEdge = contactEdge.Next)
					yield return contactEdge;
			}
		}

		public object UserData
		{
			get
			{
				return UserDataStorage.BodyStorage.ObjectFromHandle(UserDataStorage.IntPtrToHandle(NativeMethods.b2body_getuserdata(_bodyPtr)));
			}

			set
			{
				var ptr = UserDataStorage.IntPtrToHandle(NativeMethods.b2body_getuserdata(_bodyPtr));

				if (ptr != 0)
					UserDataStorage.BodyStorage.UnpinObject(ptr);

				if (value != null)
				{
					var handle = UserDataStorage.BodyStorage.PinDataToHandle(value);
					NativeMethods.b2body_setuserdata(_bodyPtr, UserDataStorage.HandleToIntPtr(handle));
				}
				else
					NativeMethods.b2body_setuserdata(_bodyPtr, IntPtr.Zero);
			}
		}

		public World World
		{
			get { return World.FromPtr(NativeMethods.b2body_getworld(_bodyPtr)); }
		}

		public static bool operator ==(Body l, Body r)
		{
			return l.BodyPtr == r.BodyPtr;
		}

		public static bool operator !=(Body l, Body r)
		{
			if ((object)l == null && (object)r == null)
				return true;
			else if ((object)l == null && (object)r != null ||
				(object)l != null && (object)r == null)
				return false;
			
			return !(l == r);
		}

		public override bool Equals(object obj)
		{
			if (obj is Body)
				return (obj as Body) == this;

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return BodyPtr.GetHashCode();
		}
	}
}
