using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Editor
{
	static class Program
	{
        public static Main MainForm
        {
            get;
            set;
        }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new Main();
			Application.Run(MainForm);
		}
	}
}
