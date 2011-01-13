namespace Editor
{
	partial class FixturePanel
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
			this.fixtureIsSensor = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.fixtureGroupIndex = new System.Windows.Forms.NumericUpDown();
			this.fixtureMaskBits = new System.Windows.Forms.NumericUpDown();
			this.fixtureCategoryBits = new System.Windows.Forms.NumericUpDown();
			this.label26 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.fixtureName = new System.Windows.Forms.TextBox();
			this.label28 = new System.Windows.Forms.Label();
			this.label41 = new System.Windows.Forms.Label();
			this.label40 = new System.Windows.Forms.Label();
			this.label43 = new System.Windows.Forms.Label();
			this.label42 = new System.Windows.Forms.Label();
			this.fixtureRestitution = new Editor.FloatNumericUpDown();
			this.fixtureDensity = new Editor.FloatNumericUpDown();
			this.fixtureFriction = new Editor.FloatNumericUpDown();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.fixtureGroupIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fixtureMaskBits)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fixtureCategoryBits)).BeginInit();
			this.SuspendLayout();
			// 
			// fixtureIsSensor
			// 
			this.fixtureIsSensor.AutoSize = true;
			this.fixtureIsSensor.Location = new System.Drawing.Point(92, 59);
			this.fixtureIsSensor.Name = "fixtureIsSensor";
			this.fixtureIsSensor.Size = new System.Drawing.Size(15, 14);
			this.fixtureIsSensor.TabIndex = 53;
			this.fixtureIsSensor.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.fixtureGroupIndex);
			this.groupBox1.Controls.Add(this.fixtureMaskBits);
			this.groupBox1.Controls.Add(this.fixtureCategoryBits);
			this.groupBox1.Controls.Add(this.label26);
			this.groupBox1.Controls.Add(this.label25);
			this.groupBox1.Controls.Add(this.label24);
			this.groupBox1.Location = new System.Drawing.Point(20, 82);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(199, 77);
			this.groupBox1.TabIndex = 52;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Filter";
			// 
			// fixtureGroupIndex
			// 
			this.fixtureGroupIndex.Location = new System.Drawing.Point(109, 31);
			this.fixtureGroupIndex.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
			this.fixtureGroupIndex.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
			this.fixtureGroupIndex.Name = "fixtureGroupIndex";
			this.fixtureGroupIndex.Size = new System.Drawing.Size(84, 20);
			this.fixtureGroupIndex.TabIndex = 18;
			// 
			// fixtureMaskBits
			// 
			this.fixtureMaskBits.Hexadecimal = true;
			this.fixtureMaskBits.Location = new System.Drawing.Point(109, 52);
			this.fixtureMaskBits.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.fixtureMaskBits.Name = "fixtureMaskBits";
			this.fixtureMaskBits.Size = new System.Drawing.Size(84, 20);
			this.fixtureMaskBits.TabIndex = 17;
			// 
			// fixtureCategoryBits
			// 
			this.fixtureCategoryBits.Hexadecimal = true;
			this.fixtureCategoryBits.Location = new System.Drawing.Point(109, 10);
			this.fixtureCategoryBits.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.fixtureCategoryBits.Name = "fixtureCategoryBits";
			this.fixtureCategoryBits.Size = new System.Drawing.Size(84, 20);
			this.fixtureCategoryBits.TabIndex = 16;
			// 
			// label26
			// 
			this.label26.AutoSize = true;
			this.label26.Location = new System.Drawing.Point(50, 54);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(53, 13);
			this.label26.TabIndex = 15;
			this.label26.Text = "MaskBits:";
			// 
			// label25
			// 
			this.label25.AutoSize = true;
			this.label25.Location = new System.Drawing.Point(36, 34);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(68, 13);
			this.label25.TabIndex = 13;
			this.label25.Text = "Group Index:";
			// 
			// label24
			// 
			this.label24.AutoSize = true;
			this.label24.Location = new System.Drawing.Point(31, 13);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(72, 13);
			this.label24.TabIndex = 11;
			this.label24.Text = "Category Bits:";
			// 
			// fixtureName
			// 
			this.fixtureName.Location = new System.Drawing.Point(80, 3);
			this.fixtureName.Name = "fixtureName";
			this.fixtureName.Size = new System.Drawing.Size(130, 20);
			this.fixtureName.TabIndex = 51;
			this.fixtureName.Text = "Test Fixture";
			// 
			// label28
			// 
			this.label28.AutoSize = true;
			this.label28.Location = new System.Drawing.Point(38, 6);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(38, 13);
			this.label28.TabIndex = 50;
			this.label28.Text = "Name:";
			// 
			// label41
			// 
			this.label41.AutoSize = true;
			this.label41.Location = new System.Drawing.Point(120, 59);
			this.label41.Name = "label41";
			this.label41.Size = new System.Drawing.Size(60, 13);
			this.label41.TabIndex = 48;
			this.label41.Text = "Restitution:";
			// 
			// label40
			// 
			this.label40.AutoSize = true;
			this.label40.Location = new System.Drawing.Point(32, 59);
			this.label40.Name = "label40";
			this.label40.Size = new System.Drawing.Size(54, 13);
			this.label40.TabIndex = 49;
			this.label40.Text = "Is Sensor:";
			// 
			// label43
			// 
			this.label43.AutoSize = true;
			this.label43.Location = new System.Drawing.Point(112, 33);
			this.label43.Name = "label43";
			this.label43.Size = new System.Drawing.Size(45, 13);
			this.label43.TabIndex = 44;
			this.label43.Text = "Density:";
			// 
			// label42
			// 
			this.label42.AutoSize = true;
			this.label42.Location = new System.Drawing.Point(1, 33);
			this.label42.Name = "label42";
			this.label42.Size = new System.Drawing.Size(44, 13);
			this.label42.TabIndex = 46;
			this.label42.Text = "Friction:";
			// 
			// fixtureRestitution
			// 
			this.fixtureRestitution.EnableUpperLimit = false;
			this.fixtureRestitution.Location = new System.Drawing.Point(182, 56);
			this.fixtureRestitution.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.fixtureRestitution.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.fixtureRestitution.Name = "fixtureRestitution";
			this.fixtureRestitution.Size = new System.Drawing.Size(44, 20);
			this.fixtureRestitution.TabIndex = 47;
			// 
			// fixtureDensity
			// 
			this.fixtureDensity.EnableUpperLimit = false;
			this.fixtureDensity.Location = new System.Drawing.Point(162, 29);
			this.fixtureDensity.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.fixtureDensity.Name = "fixtureDensity";
			this.fixtureDensity.Size = new System.Drawing.Size(65, 20);
			this.fixtureDensity.TabIndex = 43;
			// 
			// fixtureFriction
			// 
			this.fixtureFriction.EnableUpperLimit = false;
			this.fixtureFriction.Location = new System.Drawing.Point(52, 29);
			this.fixtureFriction.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.fixtureFriction.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.fixtureFriction.Name = "fixtureFriction";
			this.fixtureFriction.Size = new System.Drawing.Size(54, 20);
			this.fixtureFriction.TabIndex = 45;
			// 
			// FixturePanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.fixtureIsSensor);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.fixtureName);
			this.Controls.Add(this.label28);
			this.Controls.Add(this.label41);
			this.Controls.Add(this.label40);
			this.Controls.Add(this.label43);
			this.Controls.Add(this.label42);
			this.Controls.Add(this.fixtureRestitution);
			this.Controls.Add(this.fixtureDensity);
			this.Controls.Add(this.fixtureFriction);
			this.Name = "FixturePanel";
			this.Size = new System.Drawing.Size(229, 170);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.fixtureGroupIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fixtureMaskBits)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fixtureCategoryBits)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.CheckBox fixtureIsSensor;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown fixtureGroupIndex;
		private System.Windows.Forms.NumericUpDown fixtureMaskBits;
		private System.Windows.Forms.NumericUpDown fixtureCategoryBits;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.TextBox fixtureName;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Label label41;
		private System.Windows.Forms.Label label40;
		private System.Windows.Forms.Label label43;
		private System.Windows.Forms.Label label42;
		private FloatNumericUpDown fixtureRestitution;
		private FloatNumericUpDown fixtureDensity;
		private FloatNumericUpDown fixtureFriction;


	}
}
