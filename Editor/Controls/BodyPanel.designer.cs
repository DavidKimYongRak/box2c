namespace Editor
{
	partial class BodyPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.bodyActive = new System.Windows.Forms.CheckBox();
			this.bodyAutoMassRecalculate = new System.Windows.Forms.CheckBox();
			this.bodyAwake = new System.Windows.Forms.CheckBox();
			this.bodyAllowSleep = new System.Windows.Forms.CheckBox();
			this.bodyBullet = new System.Windows.Forms.CheckBox();
			this.bodyFixedRotation = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.bodyCenterY = new Editor.FloatNumericUpDown();
			this.label23 = new System.Windows.Forms.Label();
			this.bodyInertiaScale = new Editor.FloatNumericUpDown();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.bodyType = new System.Windows.Forms.ComboBox();
			this.bodyCenterX = new Editor.FloatNumericUpDown();
			this.bodyName = new System.Windows.Forms.TextBox();
			this.label21 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.bodyInertia = new Editor.FloatNumericUpDown();
			this.bodyLinearVelY = new Editor.FloatNumericUpDown();
			this.bodyAngle = new Editor.FloatNumericUpDown();
			this.label20 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.bodyAngularVelocity = new Editor.FloatNumericUpDown();
			this.bodyAngularDamping = new Editor.FloatNumericUpDown();
			this.bodyMass = new Editor.FloatNumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.bodyPositionX = new Editor.FloatNumericUpDown();
			this.label16 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.bodyPositionY = new Editor.FloatNumericUpDown();
			this.label15 = new System.Windows.Forms.Label();
			this.bodyLinearDamping = new Editor.FloatNumericUpDown();
			this.bodyLinearVelX = new Editor.FloatNumericUpDown();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// bodyActive
			// 
			this.bodyActive.AutoSize = true;
			this.bodyActive.Location = new System.Drawing.Point(170, 6);
			this.bodyActive.Name = "bodyActive";
			this.bodyActive.Size = new System.Drawing.Size(15, 14);
			this.bodyActive.TabIndex = 96;
			this.bodyActive.UseVisualStyleBackColor = true;
			this.bodyActive.CheckedChanged += new System.EventHandler(this.bodyActive_SelectedIndexChanged);
			// 
			// bodyAutoMassRecalculate
			// 
			this.bodyAutoMassRecalculate.AutoSize = true;
			this.bodyAutoMassRecalculate.Location = new System.Drawing.Point(212, 235);
			this.bodyAutoMassRecalculate.Name = "bodyAutoMassRecalculate";
			this.bodyAutoMassRecalculate.Size = new System.Drawing.Size(15, 14);
			this.bodyAutoMassRecalculate.TabIndex = 95;
			this.bodyAutoMassRecalculate.UseVisualStyleBackColor = true;
			this.bodyAutoMassRecalculate.CheckedChanged += new System.EventHandler(this.bodyAutoMassRecalculate_SelectedIndexChanged);
			// 
			// bodyAwake
			// 
			this.bodyAwake.AutoSize = true;
			this.bodyAwake.Location = new System.Drawing.Point(212, 215);
			this.bodyAwake.Name = "bodyAwake";
			this.bodyAwake.Size = new System.Drawing.Size(15, 14);
			this.bodyAwake.TabIndex = 94;
			this.bodyAwake.UseVisualStyleBackColor = true;
			this.bodyAwake.CheckedChanged += new System.EventHandler(this.bodyAwake_SelectedIndexChanged);
			// 
			// bodyAllowSleep
			// 
			this.bodyAllowSleep.AutoSize = true;
			this.bodyAllowSleep.Location = new System.Drawing.Point(71, 235);
			this.bodyAllowSleep.Name = "bodyAllowSleep";
			this.bodyAllowSleep.Size = new System.Drawing.Size(15, 14);
			this.bodyAllowSleep.TabIndex = 93;
			this.bodyAllowSleep.UseVisualStyleBackColor = true;
			this.bodyAllowSleep.CheckedChanged += new System.EventHandler(this.bodyAllowSleep_SelectedIndexChanged);
			// 
			// bodyBullet
			// 
			this.bodyBullet.AutoSize = true;
			this.bodyBullet.Location = new System.Drawing.Point(135, 215);
			this.bodyBullet.Name = "bodyBullet";
			this.bodyBullet.Size = new System.Drawing.Size(15, 14);
			this.bodyBullet.TabIndex = 92;
			this.bodyBullet.UseVisualStyleBackColor = true;
			this.bodyBullet.CheckedChanged += new System.EventHandler(this.bodyBullet_SelectedIndexChanged);
			// 
			// bodyFixedRotation
			// 
			this.bodyFixedRotation.AutoSize = true;
			this.bodyFixedRotation.Location = new System.Drawing.Point(71, 215);
			this.bodyFixedRotation.Name = "bodyFixedRotation";
			this.bodyFixedRotation.Size = new System.Drawing.Size(15, 14);
			this.bodyFixedRotation.TabIndex = 91;
			this.bodyFixedRotation.UseVisualStyleBackColor = true;
			this.bodyFixedRotation.CheckedChanged += new System.EventHandler(this.bodyFixedRotation_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(92, 235);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(114, 13);
			this.label1.TabIndex = 55;
			this.label1.Text = "AutoMassRecalculate:";
			// 
			// label22
			// 
			this.label22.AutoSize = true;
			this.label22.Location = new System.Drawing.Point(153, 141);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(17, 13);
			this.label22.TabIndex = 90;
			this.label22.Text = "Y:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(163, 215);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(43, 13);
			this.label7.TabIndex = 64;
			this.label7.Text = "Awake:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(0, 235);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 13);
			this.label3.TabIndex = 57;
			this.label3.Text = "Allow Sleep:";
			// 
			// bodyCenterY
			// 
			this.bodyCenterY.EnableLowerLimit = false;
			this.bodyCenterY.EnableUpperLimit = false;
			this.bodyCenterY.Location = new System.Drawing.Point(177, 137);
			this.bodyCenterY.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.bodyCenterY.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.bodyCenterY.Name = "bodyCenterY";
			this.bodyCenterY.Size = new System.Drawing.Size(50, 20);
			this.bodyCenterY.TabIndex = 89;
			this.bodyCenterY.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyCenterY_ValueChanged);
			// 
			// label23
			// 
			this.label23.AutoSize = true;
			this.label23.Location = new System.Drawing.Point(1, 141);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(77, 13);
			this.label23.TabIndex = 88;
			this.label23.Text = "Center Grav X:";
			// 
			// bodyInertiaScale
			// 
			this.bodyInertiaScale.EnableUpperLimit = false;
			this.bodyInertiaScale.Location = new System.Drawing.Point(82, 163);
			this.bodyInertiaScale.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.bodyInertiaScale.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.bodyInertiaScale.Name = "bodyInertiaScale";
			this.bodyInertiaScale.Size = new System.Drawing.Size(47, 20);
			this.bodyInertiaScale.TabIndex = 69;
			this.bodyInertiaScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.bodyInertiaScale.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyInertiaScale_ValueChanged);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(11, 166);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(69, 13);
			this.label11.TabIndex = 70;
			this.label11.Text = "Inertia Scale:";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(10, 215);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(55, 13);
			this.label10.TabIndex = 68;
			this.label10.Text = "Fixed Rot:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(93, 215);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(36, 13);
			this.label9.TabIndex = 67;
			this.label9.Text = "Bullet:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(5, 191);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(61, 13);
			this.label8.TabIndex = 65;
			this.label8.Text = "Body Type:";
			// 
			// bodyType
			// 
			this.bodyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.bodyType.FormattingEnabled = true;
			this.bodyType.Items.AddRange(new object[] {
            "Static",
            "Kinematic",
            "Dynamic"});
			this.bodyType.Location = new System.Drawing.Point(67, 188);
			this.bodyType.Name = "bodyType";
			this.bodyType.Size = new System.Drawing.Size(62, 21);
			this.bodyType.TabIndex = 66;
			this.bodyType.SelectedIndexChanged += new System.EventHandler(this.bodyType_SelectedIndexChanged);
			// 
			// bodyCenterX
			// 
			this.bodyCenterX.EnableLowerLimit = false;
			this.bodyCenterX.EnableUpperLimit = false;
			this.bodyCenterX.Location = new System.Drawing.Point(80, 137);
			this.bodyCenterX.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.bodyCenterX.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.bodyCenterX.Name = "bodyCenterX";
			this.bodyCenterX.Size = new System.Drawing.Size(49, 20);
			this.bodyCenterX.TabIndex = 87;
			this.bodyCenterX.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyCenterX_ValueChanged);
			// 
			// bodyName
			// 
			this.bodyName.Location = new System.Drawing.Point(47, 3);
			this.bodyName.Name = "bodyName";
			this.bodyName.Size = new System.Drawing.Size(70, 20);
			this.bodyName.TabIndex = 82;
			this.bodyName.Text = "Test Body";
			this.bodyName.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label21
			// 
			this.label21.AutoSize = true;
			this.label21.Location = new System.Drawing.Point(135, 167);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(39, 13);
			this.label21.TabIndex = 86;
			this.label21.Text = "Inertia:";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(5, 6);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(38, 13);
			this.label19.TabIndex = 81;
			this.label19.Text = "Name:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(124, 6);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 13);
			this.label2.TabIndex = 56;
			this.label2.Text = "Active:";
			// 
			// bodyInertia
			// 
			this.bodyInertia.EnableUpperLimit = false;
			this.bodyInertia.Location = new System.Drawing.Point(177, 163);
			this.bodyInertia.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.bodyInertia.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.bodyInertia.Name = "bodyInertia";
			this.bodyInertia.Size = new System.Drawing.Size(51, 20);
			this.bodyInertia.TabIndex = 85;
			this.bodyInertia.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyInertia_ValueChanged);
			// 
			// bodyLinearVelY
			// 
			this.bodyLinearVelY.EnableLowerLimit = false;
			this.bodyLinearVelY.EnableUpperLimit = false;
			this.bodyLinearVelY.Location = new System.Drawing.Point(177, 85);
			this.bodyLinearVelY.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.bodyLinearVelY.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.bodyLinearVelY.Name = "bodyLinearVelY";
			this.bodyLinearVelY.Size = new System.Drawing.Size(51, 20);
			this.bodyLinearVelY.TabIndex = 75;
			this.bodyLinearVelY.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyLinearVelY_ValueChanged);
			// 
			// bodyAngle
			// 
			this.bodyAngle.EnableLowerLimit = false;
			this.bodyAngle.EnableUpperLimit = false;
			this.bodyAngle.Location = new System.Drawing.Point(178, 33);
			this.bodyAngle.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.bodyAngle.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.bodyAngle.Name = "bodyAngle";
			this.bodyAngle.Size = new System.Drawing.Size(50, 20);
			this.bodyAngle.TabIndex = 58;
			this.bodyAngle.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyAngle_ValueChanged);
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(140, 193);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(35, 13);
			this.label20.TabIndex = 84;
			this.label20.Text = "Mass:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(135, 37);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(37, 13);
			this.label4.TabIndex = 59;
			this.label4.Text = "Angle:";
			// 
			// bodyAngularVelocity
			// 
			this.bodyAngularVelocity.EnableLowerLimit = false;
			this.bodyAngularVelocity.EnableUpperLimit = false;
			this.bodyAngularVelocity.Location = new System.Drawing.Point(74, 33);
			this.bodyAngularVelocity.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.bodyAngularVelocity.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.bodyAngularVelocity.Name = "bodyAngularVelocity";
			this.bodyAngularVelocity.Size = new System.Drawing.Size(55, 20);
			this.bodyAngularVelocity.TabIndex = 62;
			this.bodyAngularVelocity.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyAngularVelocity_ValueChanged);
			// 
			// bodyAngularDamping
			// 
			this.bodyAngularDamping.EnableLowerLimit = false;
			this.bodyAngularDamping.EnableUpperLimit = false;
			this.bodyAngularDamping.Location = new System.Drawing.Point(177, 111);
			this.bodyAngularDamping.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.bodyAngularDamping.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.bodyAngularDamping.Name = "bodyAngularDamping";
			this.bodyAngularDamping.Size = new System.Drawing.Size(51, 20);
			this.bodyAngularDamping.TabIndex = 60;
			this.bodyAngularDamping.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyAngularDamping_ValueChanged);
			// 
			// bodyMass
			// 
			this.bodyMass.EnableUpperLimit = false;
			this.bodyMass.Location = new System.Drawing.Point(176, 189);
			this.bodyMass.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.bodyMass.Name = "bodyMass";
			this.bodyMass.Size = new System.Drawing.Size(52, 20);
			this.bodyMass.TabIndex = 83;
			this.bodyMass.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyMass_ValueChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(145, 115);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(29, 13);
			this.label5.TabIndex = 61;
			this.label5.Text = "Ang:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(4, 37);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(64, 13);
			this.label6.TabIndex = 63;
			this.label6.Text = "Angular Vel:";
			// 
			// bodyPositionX
			// 
			this.bodyPositionX.EnableLowerLimit = false;
			this.bodyPositionX.EnableUpperLimit = false;
			this.bodyPositionX.Location = new System.Drawing.Point(80, 59);
			this.bodyPositionX.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.bodyPositionX.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.bodyPositionX.Name = "bodyPositionX";
			this.bodyPositionX.Size = new System.Drawing.Size(49, 20);
			this.bodyPositionX.TabIndex = 77;
			this.bodyPositionX.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyPositionX_ValueChanged);
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(17, 63);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(57, 13);
			this.label16.TabIndex = 78;
			this.label16.Text = "Position X:";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(13, 115);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(69, 13);
			this.label12.TabIndex = 72;
			this.label12.Text = "Damping Lin:";
			// 
			// bodyPositionY
			// 
			this.bodyPositionY.EnableLowerLimit = false;
			this.bodyPositionY.EnableUpperLimit = false;
			this.bodyPositionY.Location = new System.Drawing.Point(177, 59);
			this.bodyPositionY.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.bodyPositionY.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.bodyPositionY.Name = "bodyPositionY";
			this.bodyPositionY.Size = new System.Drawing.Size(51, 20);
			this.bodyPositionY.TabIndex = 79;
			this.bodyPositionY.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyPositionY_ValueChanged);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(152, 63);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(17, 13);
			this.label15.TabIndex = 80;
			this.label15.Text = "Y:";
			// 
			// bodyLinearDamping
			// 
			this.bodyLinearDamping.EnableLowerLimit = false;
			this.bodyLinearDamping.EnableUpperLimit = false;
			this.bodyLinearDamping.Location = new System.Drawing.Point(84, 111);
			this.bodyLinearDamping.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.bodyLinearDamping.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.bodyLinearDamping.Name = "bodyLinearDamping";
			this.bodyLinearDamping.Size = new System.Drawing.Size(45, 20);
			this.bodyLinearDamping.TabIndex = 71;
			this.bodyLinearDamping.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyLinearDamping_ValueChanged);
			// 
			// bodyLinearVelX
			// 
			this.bodyLinearVelX.EnableLowerLimit = false;
			this.bodyLinearVelX.EnableUpperLimit = false;
			this.bodyLinearVelX.Location = new System.Drawing.Point(79, 85);
			this.bodyLinearVelX.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.bodyLinearVelX.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.bodyLinearVelX.Name = "bodyLinearVelX";
			this.bodyLinearVelX.Size = new System.Drawing.Size(49, 20);
			this.bodyLinearVelX.TabIndex = 73;
			this.bodyLinearVelX.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.bodyLinearVelX_ValueChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(8, 87);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(67, 13);
			this.label13.TabIndex = 74;
			this.label13.Text = "Linear Vel X:";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(152, 89);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(17, 13);
			this.label14.TabIndex = 76;
			this.label14.Text = "Y:";
			// 
			// BodyPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.bodyActive);
			this.Controls.Add(this.bodyAutoMassRecalculate);
			this.Controls.Add(this.bodyAwake);
			this.Controls.Add(this.bodyAllowSleep);
			this.Controls.Add(this.bodyBullet);
			this.Controls.Add(this.bodyFixedRotation);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label22);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.bodyCenterY);
			this.Controls.Add(this.label23);
			this.Controls.Add(this.bodyInertiaScale);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.bodyType);
			this.Controls.Add(this.bodyCenterX);
			this.Controls.Add(this.bodyName);
			this.Controls.Add(this.label21);
			this.Controls.Add(this.label19);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.bodyInertia);
			this.Controls.Add(this.bodyLinearVelY);
			this.Controls.Add(this.bodyAngle);
			this.Controls.Add(this.label20);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.bodyAngularVelocity);
			this.Controls.Add(this.bodyAngularDamping);
			this.Controls.Add(this.bodyMass);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.bodyPositionX);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.bodyPositionY);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.bodyLinearDamping);
			this.Controls.Add(this.bodyLinearVelX);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label14);
			this.Name = "BodyPanel";
			this.Size = new System.Drawing.Size(229, 258);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.CheckBox bodyActive;
		private System.Windows.Forms.CheckBox bodyAutoMassRecalculate;
		private System.Windows.Forms.CheckBox bodyAwake;
		private System.Windows.Forms.CheckBox bodyAllowSleep;
		private System.Windows.Forms.CheckBox bodyBullet;
		private System.Windows.Forms.CheckBox bodyFixedRotation;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label3;
		private FloatNumericUpDown bodyCenterY;
		private System.Windows.Forms.Label label23;
		private FloatNumericUpDown bodyInertiaScale;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox bodyType;
		private FloatNumericUpDown bodyCenterX;
		private System.Windows.Forms.TextBox bodyName;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label2;
		private FloatNumericUpDown bodyInertia;
		private FloatNumericUpDown bodyLinearVelY;
		private FloatNumericUpDown bodyAngle;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label4;
		private FloatNumericUpDown bodyAngularVelocity;
		private FloatNumericUpDown bodyAngularDamping;
		private FloatNumericUpDown bodyMass;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private FloatNumericUpDown bodyPositionX;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label12;
		private FloatNumericUpDown bodyPositionY;
		private System.Windows.Forms.Label label15;
		private FloatNumericUpDown bodyLinearDamping;
		private FloatNumericUpDown bodyLinearVelX;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;

	}
}
