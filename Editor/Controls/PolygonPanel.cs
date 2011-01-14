using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Box2CS;

namespace Editor
{
    public partial class PolygonPanel : UserControl
    {
		public PolygonPanel()
        {
            InitializeComponent();
        }

		public PolygonShape SelectedShape
		{
			get { return (PolygonShape)Program.MainForm.SelectedNode.ShapeNode.Shape; }
		}

		public void Added()
		{
			LoadPolygonShapeData();
		}

		public void LoadPolygonShapeData()
		{
		}
    }
}
