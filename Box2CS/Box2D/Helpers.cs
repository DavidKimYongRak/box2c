using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Box2CS
{
	internal interface IFixedSize
	{
		int FixedSize();
		void Lock();
		void Unlock();
	}

	internal class StructToPtrMarshaller : IDisposable
	{
		IntPtr _ptr;
		IFixedSize _storedVal;

		public IntPtr Pointer
		{
			get { return _ptr; }
		}

		[EnvironmentPermissionAttribute(SecurityAction.Demand, Unrestricted=true)]
		public StructToPtrMarshaller(IFixedSize val)
		{
			_storedVal = val;
			_storedVal.Lock();
			_ptr = Marshal.AllocHGlobal(val.FixedSize());
			Marshal.StructureToPtr(val, _ptr, false);	
		}

		[EnvironmentPermissionAttribute(SecurityAction.Demand, Unrestricted=true)]
		public object GetValue(Type type)
		{
			return Marshal.PtrToStructure(_ptr, type);
		}

		[EnvironmentPermissionAttribute(SecurityAction.Demand, Unrestricted=true)]
		public void Free()
		{
			disposed = true;
			Marshal.FreeHGlobal(_ptr);
			_storedVal.Unlock();
		}

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

				Free();

				disposed = true;
			}
		}

		~StructToPtrMarshaller()
		{
			Dispose(false);
		}
		#endregion
	}
}
