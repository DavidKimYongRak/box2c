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
    public partial class FixturePanel : UserControl
    {
		public FixturePanel()
        {
            InitializeComponent();
        }

		private void FixturePanel_Load(object sender, EventArgs e)
		{
		}

		public FixtureNode SelectedFixture
		{
			get { return Program.MainForm.SelectedNode.FixtureNode; }
		}

		public void Added()
		{
			LoadFixtureObjectSettings();
		}

		void LoadFixtureObjectSettings()
		{
			fixtureName.Text = SelectedFixture.Name;
			fixtureDensity.Value = Convert.ToDecimal(SelectedFixture.Fixture.Density);
			fixtureFriction.Value = Convert.ToDecimal(SelectedFixture.Fixture.Friction);
			fixtureIsSensor.Checked = SelectedFixture.Fixture.IsSensor;
			fixtureRestitution.Value = Convert.ToDecimal(SelectedFixture.Fixture.Restitution);

			fixtureCategoryBits.Value = SelectedFixture.Fixture.Filter.CategoryBits;
			fixtureGroupIndex.Value = SelectedFixture.Fixture.Filter.GroupIndex;
			fixtureMaskBits.Value = SelectedFixture.Fixture.Filter.MaskBits;
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			SelectedFixture.Name = fixtureName.Text;
		}

		private void fixtureIsSensor_SelectedIndexChanged(object sender, EventArgs e)
		{
			SelectedFixture.Fixture.IsSensor = fixtureIsSensor.Checked;
		}

		private void fixtureDensity_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedFixture.Fixture.Density = (float)e.NewValue;
		}

		private void fixtureFriction_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedFixture.Fixture.Friction = (float)e.NewValue;
		}

		private void fixtureRestitution_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedFixture.Fixture.Restitution = (float)e.NewValue;
		}

		private void fixtureCategoryBits_ValueChanged(object sender, EventArgs e)
		{
			SelectedFixture.Fixture.Filter.CategoryBits = (ushort)fixtureCategoryBits.Value;
		}

		private void fixtureMaskBits_ValueChanged(object sender, EventArgs e)
		{
			SelectedFixture.Fixture.Filter.MaskBits = (ushort)fixtureMaskBits.Value;
		}

		private void fixtureGroupIndex_ValueChanged(object sender, EventArgs e)
		{
			SelectedFixture.Fixture.Filter.CategoryBits = (ushort)fixtureCategoryBits.Value;
		}
    }
}
