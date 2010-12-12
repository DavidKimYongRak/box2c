using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	public struct Box2DVersion
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern void b2version_get(out Box2DVersion version);
		}

		static Box2DVersion GetBox2DVersion()
		{
			Box2DVersion ver;
			NativeMethods.b2version_get(out ver);
			return ver;
		}

		public static Box2DVersion Version = GetBox2DVersion();

		public int Major;
		public int Minor;
		public int Revision;

		public override string ToString()
		{
			return Major.ToString() + " " + Minor.ToString() + " " + Revision.ToString();
		}
	}
}
