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
    public partial class BodyPanel : UserControl
    {
		public BodyPanel()
        {
            InitializeComponent();
        }

		public BodyNode SelectedBody
		{
			get { return Program.MainForm.SelectedNode.BodyNode; }
		}

		public void Added()
		{
			LoadBodyObjectSettings();
		}

		public void LoadBodyObjectSettings()
		{
			bodyActive.Checked = SelectedBody.Body.Active;
			bodyAllowSleep.Checked = SelectedBody.Body.AllowSleep;
			bodyAngle.Value = Convert.ToDecimal(SelectedBody.Body.Angle);
			bodyAngularDamping.Value = Convert.ToDecimal(SelectedBody.Body.AngularDamping);
			bodyAngularVelocity.Value = Convert.ToDecimal(SelectedBody.Body.AngularVelocity);
			bodyAwake.Checked = SelectedBody.Body.Awake;
			bodyType.SelectedIndex = (int)SelectedBody.Body.BodyType;
			bodyBullet.Checked = SelectedBody.Body.Bullet;
			bodyFixedRotation.Checked = SelectedBody.Body.FixedRotation;
			bodyInertiaScale.Value = Convert.ToDecimal(SelectedBody.Body.InertiaScale);
			bodyLinearDamping.Value = Convert.ToDecimal(SelectedBody.Body.LinearDamping);
			bodyLinearVelX.Value = Convert.ToDecimal(SelectedBody.Body.LinearVelocity.X);
			bodyLinearVelY.Value = Convert.ToDecimal(SelectedBody.Body.LinearVelocity.Y);
			bodyPositionX.Value = Convert.ToDecimal(SelectedBody.Body.Position.X);
			bodyPositionY.Value = Convert.ToDecimal(SelectedBody.Body.Position.Y);

			bodyName.Text = SelectedBody.Name;

			bodyAutoMassRecalculate.Checked = SelectedBody.AutoMassRecalculate;

			bodyCenterX.Value = Convert.ToDecimal(SelectedBody.Mass.Center.X);
			bodyCenterY.Value = Convert.ToDecimal(SelectedBody.Mass.Center.Y);
			bodyMass.Value = Convert.ToDecimal(SelectedBody.Mass.Mass);
			bodyInertia.Value = Convert.ToDecimal(SelectedBody.Mass.Inertia);
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			SelectedBody.Name = bodyName.Text;
		}

		private void bodyAutoMassRecalculate_SelectedIndexChanged(object sender, EventArgs e)
		{
			SelectedBody.AutoMassRecalculate = bodyAutoMassRecalculate.Checked;
		}

		private void bodyActive_SelectedIndexChanged(object sender, EventArgs e)
		{
			SelectedBody.Body.Active = bodyActive.Checked;
		}

		private void bodyAllowSleep_SelectedIndexChanged(object sender, EventArgs e)
		{
			SelectedBody.Body.AllowSleep = bodyAllowSleep.Checked;
		}

		private void bodyAngle_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedBody.Body.Angle = (float)e.NewValue;
		}

		private void bodyAngularDamping_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedBody.Body.AngularDamping = (float)e.NewValue;
		}

		private void bodyAngularVelocity_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedBody.Body.AngularVelocity = (float)e.NewValue;
		}

		private void bodyAwake_SelectedIndexChanged(object sender, EventArgs e)
		{
			SelectedBody.Body.Awake = bodyAwake.Checked;
		}

		private void bodyType_SelectedIndexChanged(object sender, EventArgs e)
		{
			SelectedBody.Body.BodyType = (BodyType)bodyType.SelectedIndex;
		}

		private void bodyBullet_SelectedIndexChanged(object sender, EventArgs e)
		{
			SelectedBody.Body.Bullet = bodyBullet.Checked;
		}

		private void bodyFixedRotation_SelectedIndexChanged(object sender, EventArgs e)
		{
			SelectedBody.Body.FixedRotation = bodyFixedRotation.Checked;
		}

		private void bodyInertiaScale_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedBody.Body.InertiaScale = (float)e.NewValue;
		}

		private void bodyLinearDamping_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedBody.Body.LinearDamping = (float)e.NewValue;
		}

		private void bodyLinearVelX_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedBody.Body.LinearVelocity = new Vec2((float)e.NewValue, SelectedBody.Body.LinearVelocity.Y);
		}

		private void bodyLinearVelY_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedBody.Body.LinearVelocity = new Vec2(SelectedBody.Body.LinearVelocity.X, (float)e.NewValue);
		}

		private void bodyPositionX_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedBody.Body.Position = new Vec2((float)e.NewValue, SelectedBody.Body.Position.Y);
		}

		private void bodyPositionY_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			SelectedBody.Body.Position = new Vec2(SelectedBody.Body.Position.X, (float)e.NewValue);
		}

		private void bodyMass_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			MassData Mass = SelectedBody.Mass;
			Mass.Mass = (float)e.NewValue;
			SelectedBody.Mass = Mass;
		}

		private void bodyInertia_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			MassData Mass = SelectedBody.Mass;
			Mass.Inertia = (float)e.NewValue;
			SelectedBody.Mass = Mass;
		}

		private void bodyCenterX_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			MassData Mass = SelectedBody.Mass;
			Mass.Center = new Vec2((float)e.NewValue, Mass.Center.Y);
			SelectedBody.Mass = Mass;
		}

		private void bodyCenterY_ValueChanged(object sender, DecimalValueChangedEventArgs e)
		{
			MassData Mass = SelectedBody.Mass;
			Mass.Center = new Vec2(Mass.Center.X, (float)e.NewValue);
			SelectedBody.Mass = Mass;
		}
    }
}
