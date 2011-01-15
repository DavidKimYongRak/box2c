namespace Editor
{
	partial class PolygonPanel
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.circlePositionY = new Editor.FloatNumericUpDown();
			this.circlePositionX = new Editor.FloatNumericUpDown();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 13);
			this.label1.TabIndex = 13;
			this.label1.Text = "Centroid X:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(140, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(17, 13);
			this.label2.TabIndex = 15;
			this.label2.Text = "Y:";
			// 
			// circlePositionY
			// 
			this.circlePositionY.EnableLowerLimit = false;
			this.circlePositionY.EnableUpperLimit = false;
			this.circlePositionY.Location = new System.Drawing.Point(163, 3);
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
			// 
			// circlePositionX
			// 
			this.circlePositionX.EnableLowerLimit = false;
			this.circlePositionX.EnableUpperLimit = false;
			this.circlePositionX.Location = new System.Drawing.Point(81, 3);
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
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToResizeColumns = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
			this.dataGridView1.Location = new System.Drawing.Point(17, 29);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(194, 211);
			this.dataGridView1.TabIndex = 16;
			// 
			// Column1
			// 
			this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Column1.HeaderText = "X";
			this.Column1.Name = "Column1";
			this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Column2
			// 
			this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Column2.HeaderText = "Y";
			this.Column2.Name = "Column2";
			this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// PolygonPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.circlePositionY);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.circlePositionX);
			this.Name = "PolygonPanel";
			this.Size = new System.Drawing.Size(229, 243);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
        public FloatNumericUpDown circlePositionX;
        public FloatNumericUpDown circlePositionY;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
    }
}
