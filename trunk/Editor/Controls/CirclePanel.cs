using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Editor
{
    public partial class CirclePanel : UserControl
    {
        public CirclePanel()
        {
            InitializeComponent();
        }

        private void circleRadius_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            Main main = Program.MainForm;
            main.circleRadius_ValueChanged(sender, e);
        }

        private void circlePositionX_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            Main main = Program.MainForm;
            main.circlePositionX_ValueChanged(sender, e);
        }

        private void circlePositionY_ValueChanged(object sender, DecimalValueChangedEventArgs e)
        {
            Main main = Program.MainForm;
            main.circlePositionY_ValueChanged(sender, e);
        }
    }
}
