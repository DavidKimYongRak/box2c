using System;

namespace Paril.Helpers
{
	public abstract class EasyDispose : IDisposable
	{
		public bool Disposed
		{
			get;
			private set;
		}

		protected abstract void ReleaseUnmanagedResources();
		protected abstract void ReleaseManagedResources();

		void Dispose(bool disposing)
		{
			if (!Disposed)
			{
				if (disposing)
					ReleaseManagedResources();

				ReleaseUnmanagedResources();
				Disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~EasyDispose()
		{
			Dispose(false);
		}
	}
}
