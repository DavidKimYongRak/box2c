using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	internal interface IFixedSize
	{
		int FixedSize();
		void Lock();
		void Unlock();
	}

	internal class StructToPtrMarshaller<T> : IDisposable
		where T : IFixedSize
	{
		IntPtr _ptr;
		T _storedVal;
		bool _retrieve;

		public IntPtr Pointer
		{
			get { return _ptr; }
		}

		public StructToPtrMarshaller(T val, bool retrieve = false)
		{
			_retrieve = retrieve;
			_storedVal = val;
			_storedVal.Lock();
			_ptr = Marshal.AllocHGlobal(val.FixedSize());
			Marshal.StructureToPtr(val, _ptr, false);	
		}

		public T StoredValue
		{
			get
			{
				if (_retrieve)
					_storedVal = (T)Marshal.PtrToStructure(_ptr, typeof(T));

				return _storedVal;
			}
		}

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
