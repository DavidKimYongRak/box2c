using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void BeginEndContactDelegate(IntPtr contact);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void PreSolveDelegate(IntPtr contact, IntPtr oldManifold);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void PostSolveDelegate(IntPtr contact, IntPtr impulse);

	[StructLayout(LayoutKind.Sequential)]
	internal struct cb2contactlistener
	{
		public BeginEndContactDelegate BeginContact;
		public BeginEndContactDelegate EndContact;
		public PreSolveDelegate PreSolve;
		public PostSolveDelegate PostSolve;
	}

	/// Contact impulses for reporting. Impulses are used instead of forces because
	/// sub-step forces may approach infinity for rigid body collisions. These
	/// match up one-to-one with the contact points in Manifold.
	[StructLayout(LayoutKind.Sequential)]
	public struct ContactImpulse
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst=Box2DSettings.b2_maxManifoldPoints, ArraySubType=UnmanagedType.Struct)]
		public float[] normalImpulses;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst=Box2DSettings.b2_maxManifoldPoints, ArraySubType=UnmanagedType.Struct)]
		public float[] tangentImpulses;
	};

	public abstract class ContactListener : IDisposable
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr cb2contactlistener_create(cb2contactlistener functions);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void cb2contactlistener_destroy(IntPtr listener);
		}

		IntPtr _listener;
		cb2contactlistener functions;

		internal IntPtr Listener
		{
			get { return _listener; }
		}

		public ContactListener()
		{
			functions = new cb2contactlistener();
			functions.BeginContact += BeginContactInternal;
			functions.EndContact += EndContactInternal;
			functions.PreSolve += PreSolveInternal;
			functions.PostSolve += PostSolveInternal;

			_listener = NativeMethods.cb2contactlistener_create(functions);
		}

		void BeginContactInternal(IntPtr contact)
		{
			try
			{
				BeginContact(Contact.FromPtr(contact));
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		void EndContactInternal(IntPtr contact)
		{
			try
			{
				EndContact(Contact.FromPtr(contact));	
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		void PreSolveInternal(IntPtr contact, IntPtr oldManifold)
		{
			try
			{
				PreSolve(Contact.FromPtr(contact), (Manifold)Marshal.PtrToStructure(oldManifold, typeof(Manifold)));
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		void PostSolveInternal(IntPtr contact, IntPtr impulse)
		{
			try
			{
				PostSolve(Contact.FromPtr(contact), (ContactImpulse)Marshal.PtrToStructure(impulse, typeof(ContactImpulse)));
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public abstract void BeginContact(Contact contact);
		public abstract void EndContact(Contact contact);
		public abstract void PreSolve(Contact contact, Manifold oldManifold);
		public abstract void PostSolve(Contact contact, ContactImpulse impulse);

		#region IDisposable
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		bool disposed;

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
				}

				NativeMethods.cb2contactlistener_destroy(_listener);

				disposed = true;
			}
		}

		~ContactListener()
		{
			Dispose(false);
		}
		#endregion
	}
}
