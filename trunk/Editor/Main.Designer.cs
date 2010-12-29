namespace Editor
{
	partial class Main
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.listBox2 = new System.Windows.Forms.ListBox();
			this.propertyGrid2 = new System.Windows.Forms.PropertyGrid();
			this.listBox3 = new System.Windows.Forms.ListBox();
			this.propertyGrid3 = new System.Windows.Forms.PropertyGrid();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.listBox4 = new System.Windows.Forms.ListBox();
			this.propertyGrid4 = new System.Windows.Forms.PropertyGrid();
			this.panel1 = new Editor.HolyCrapControl();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.panel1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
			this.splitContainer1.Size = new System.Drawing.Size(762, 542);
			this.splitContainer1.SplitterDistance = 518;
			this.splitContainer1.TabIndex = 1;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.propertyGrid1.Location = new System.Drawing.Point(3, 235);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(226, 278);
			this.propertyGrid1.TabIndex = 0;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage5);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(240, 542);
			this.tabControl1.TabIndex = 2;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.listBox1);
			this.tabPage1.Controls.Add(this.propertyGrid1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(232, 516);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Bodies";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.listBox2);
			this.tabPage2.Controls.Add(this.propertyGrid2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(232, 516);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Fixtures";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.listBox3);
			this.tabPage3.Controls.Add(this.propertyGrid3);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(232, 516);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Shapes";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// listBox1
			// 
			this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(3, 3);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(226, 232);
			this.listBox1.TabIndex = 1;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// listBox2
			// 
			this.listBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox2.FormattingEnabled = true;
			this.listBox2.Location = new System.Drawing.Point(3, 3);
			this.listBox2.Name = "listBox2";
			this.listBox2.Size = new System.Drawing.Size(226, 232);
			this.listBox2.TabIndex = 3;
			this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
			// 
			// propertyGrid2
			// 
			this.propertyGrid2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.propertyGrid2.Location = new System.Drawing.Point(3, 235);
			this.propertyGrid2.Name = "propertyGrid2";
			this.propertyGrid2.Size = new System.Drawing.Size(226, 278);
			this.propertyGrid2.TabIndex = 2;
			// 
			// listBox3
			// 
			this.listBox3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox3.FormattingEnabled = true;
			this.listBox3.Location = new System.Drawing.Point(3, 3);
			this.listBox3.Name = "listBox3";
			this.listBox3.Size = new System.Drawing.Size(226, 232);
			this.listBox3.TabIndex = 3;
			this.listBox3.SelectedIndexChanged += new System.EventHandler(this.listBox3_SelectedIndexChanged);
			// 
			// propertyGrid3
			// 
			this.propertyGrid3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.propertyGrid3.Location = new System.Drawing.Point(3, 235);
			this.propertyGrid3.Name = "propertyGrid3";
			this.propertyGrid3.Size = new System.Drawing.Size(226, 278);
			this.propertyGrid3.TabIndex = 2;
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.listBox4);
			this.tabPage5.Controls.Add(this.propertyGrid4);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage5.Size = new System.Drawing.Size(232, 516);
			this.tabPage5.TabIndex = 4;
			this.tabPage5.Text = "Joints";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// listBox4
			// 
			this.listBox4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox4.FormattingEnabled = true;
			this.listBox4.Location = new System.Drawing.Point(3, 3);
			this.listBox4.Name = "listBox4";
			this.listBox4.Size = new System.Drawing.Size(226, 232);
			this.listBox4.TabIndex = 5;
			this.listBox4.SelectedIndexChanged += new System.EventHandler(this.listBox4_SelectedIndexChanged);
			// 
			// propertyGrid4
			// 
			this.propertyGrid4.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.propertyGrid4.Location = new System.Drawing.Point(3, 235);
			this.propertyGrid4.Name = "propertyGrid4";
			this.propertyGrid4.Size = new System.Drawing.Size(226, 278);
			this.propertyGrid4.TabIndex = 4;
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(518, 542);
			this.panel1.TabIndex = 0;
			this.panel1.Text = "holyCrapControl1";
			this.panel1.Click += new System.EventHandler(this.panel1_Click);
			this.panel1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.panel1_KeyPress);
			this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(762, 542);
			this.Controls.Add(this.splitContainer1);
			this.Name = "Main";
			this.Text = "Main";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
			this.Load += new System.EventHandler(this.Main_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabPage5.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private HolyCrapControl panel1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.ListBox listBox2;
		private System.Windows.Forms.PropertyGrid propertyGrid2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.ListBox listBox3;
		private System.Windows.Forms.PropertyGrid propertyGrid3;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.ListBox listBox4;
		private System.Windows.Forms.PropertyGrid propertyGrid4;

	}
}

