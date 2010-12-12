using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate bool ShouldCollideDelegate(IntPtr fixtureA, IntPtr fixtureB);

	[StructLayout(LayoutKind.Sequential)]
	internal struct cb2contactfilter
	{
		public ShouldCollideDelegate ShouldCollide;
	}

	public abstract class ContactFilter : IDisposable
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr cb2contactfilter_create(cb2contactfilter functions);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void cb2contactfilter_destroy(IntPtr listener);
		}

		IntPtr _listener;
		cb2contactfilter functions;

		internal IntPtr Listener
		{
			get { return _listener; }
		}

		public ContactFilter()
		{
			functions = new cb2contactfilter();
			functions.ShouldCollide = ShouldCollideInternal;

			_listener = NativeMethods.cb2contactfilter_create(functions);
		}

		bool ShouldCollideInternal(IntPtr fixtureA, IntPtr fixtureB)
		{
			return ShouldCollide(Fixture.FromPtr(fixtureA), Fixture.FromPtr(fixtureB));
		}

		public abstract bool ShouldCollide(Fixture fixtureA, Fixture fixtureB);

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

				NativeMethods.cb2contactfilter_destroy(_listener);

				disposed = true;
			}
		}

		~ContactFilter()
		{
			Dispose(false);
		}
		#endregion
	}
}
