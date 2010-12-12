using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	public class ContactEdge
	{
		static class NativeMethods
		{
			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2contactedge_getother(IntPtr contactEdge);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2contactedge_getcontact(IntPtr contactEdge);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2contactedge_getprev(IntPtr contactEdge);

			[DllImport(Box2DSettings.Box2CDLLName)]
			public static extern IntPtr b2contactedge_getnext(IntPtr contactEdge);
		}

		public IntPtr _contactEdgePtr;

		internal ContactEdge(IntPtr ptr)
		{
			_contactEdgePtr = ptr;
		}

		internal static ContactEdge FromPtr(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
				return null;

			return new ContactEdge(ptr);
		}

		public Body Body
		{
			get { return Body.FromPtr(NativeMethods.b2contactedge_getother(_contactEdgePtr)); }
		}

		public Contact Contact
		{
			get { return Contact.FromPtr(NativeMethods.b2contactedge_getcontact(_contactEdgePtr)); }
		}

		public ContactEdge Next
		{
			get { return ContactEdge.FromPtr(NativeMethods.b2contactedge_getnext(_contactEdgePtr)); }
		}

		public ContactEdge Prev
		{
			get { return ContactEdge.FromPtr(NativeMethods.b2contactedge_getprev(_contactEdgePtr)); }
		}
	}
}
