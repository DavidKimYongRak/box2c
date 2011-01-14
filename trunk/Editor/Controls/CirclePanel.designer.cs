namespace Editor
{
    partial class CirclePanel
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
			this.label42 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.circlePositionY = new Editor.FloatNumericUpDown();
			this.circlePositionX = new Editor.FloatNumericUpDown();
			this.circleRadius = new Editor.FloatNumericUpDown();
			this.SuspendLayout();
			// 
			// label42
			// 
			this.label42.AutoSize = true;
			this.label42.Location = new System.Drawing.Point(67, 8);
			this.label42.Name = "label42";
			this.label42.Size = new System.Drawing.Size(43, 13);
			this.label42.TabIndex = 11;
			this.label42.Text = "Radius:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 34);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57, 13);
			this.label1.TabIndex = 13;
			this.label1.Text = "Position X:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(129, 34);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(17, 13);
			this.label2.TabIndex = 15;
			this.label2.Text = "Y:";
			// 
			// circlePositionY
			// 
			this.circlePositionY.EnableLowerLimit = false;
			this.circlePositionY.EnableUpperLimit = false;
			this.circlePositionY.Location = new System.Drawing.Point(152, 30);
			this.circlePositionY.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.circlePositionY.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.circlePositionY.Name = "circlePositionY";
			this.circlePositionY.Size = new System.Drawing.Size(50, 20);
			this.circlePositionY.TabIndex = 14;
			this.circlePositionY.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.circlePositionY_ValueChanged);
			// 
			// circlePositionX
			// 
			this.circlePositionX.EnableLowerLimit = false;
			this.circlePositionX.EnableUpperLimit = false;
			this.circlePositionX.Location = new System.Drawing.Point(70, 30);
			this.circlePositionX.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
			this.circlePositionX.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
			this.circlePositionX.Name = "circlePositionX";
			this.circlePositionX.Size = new System.Drawing.Size(50, 20);
			this.circlePositionX.TabIndex = 12;
			this.circlePositionX.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.circlePositionX_ValueChanged);
			// 
			// circleRadius
			// 
			this.circleRadius.EnableUpperLimit = false;
			this.circleRadius.Interval = new decimal(new int[] {
            5,
            0,
            0,
            131072});
			this.circleRadius.Location = new System.Drawing.Point(118, 4);
			this.circleRadius.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.circleRadius.Name = "circleRadius";
			this.circleRadius.Size = new System.Drawing.Size(84, 20);
			this.circleRadius.TabIndex = 10;
			this.circleRadius.ValueChanged += new Editor.DecimalValueChangedEventHandler(this.circleRadius_ValueChanged);
			// 
			// CirclePanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.circlePositionY);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.circlePositionX);
			this.Controls.Add(this.label42);
			this.Controls.Add(this.circleRadius);
			this.Name = "CirclePanel";
			this.Size = new System.Drawing.Size(229, 62);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public FloatNumericUpDown circleRadius;
        public FloatNumericUpDown circlePositionX;
        public FloatNumericUpDown circlePositionY;
    }
}
