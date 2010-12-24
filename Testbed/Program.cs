using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SFML.Window;
using SFML.Graphics;

namespace Testbed
{
	static class Program
	{
		public static Main MainForm
		{
			get;
			private set;
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(MainForm = new Main());
		}
	}
}
