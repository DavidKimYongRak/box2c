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
    public partial class CirclePanel : UserControl
    {
        public CirclePanel()
        {
            InitializeComponent();
        }

		public CircleShape SelectedShape
		{
			get { return (CircleShape)Program.MainForm.SelectedNode.ShapeNode.Shape; }
		}

		public void Added()
		{
			LoadCircleShapeData();
		}

		public void LoadCircleShapeData()
		{
			circleRadius.Value = (decimal)SelectedShape.Radius;
			circlePositionX.Value = (decimal)SelectedShape.Position.X;
			circlePositionY.Value = (decimal)SelectedShape.Position.Y;
		}

		public void circleRadius_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedShape.Radius = (float)e.NewValue;

			if ((Program.MainForm.SelectedNode.Node.Parent as FixtureNode).OwnedBody != null &&
				(Program.MainForm.SelectedNode.Node.Parent as FixtureNode).OwnedBody.AutoMassRecalculate)
				(Program.MainForm.SelectedNode.Node.Parent as FixtureNode).OwnedBody.RecalculateMass();
		}

		public void circlePositionX_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedShape.Position = new Vec2((float)e.NewValue, SelectedShape.Position.Y);
		}

		public void circlePositionY_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedShape.Position = new Vec2(SelectedShape.Position.X, (float)e.NewValue);
		}

		private void CirclePanel_Load(object sender, EventArgs e)
		{

		}
    }
}
