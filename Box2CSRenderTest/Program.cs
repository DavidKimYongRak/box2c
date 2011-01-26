using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Box2DSharpRenderTest
{
	static class Program
	{
		public static Form3 MainForm
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
			Application.Run(MainForm = new Form3());
		}
	}
}
