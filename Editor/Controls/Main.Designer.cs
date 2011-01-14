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
			this.components = new System.ComponentModel.Container();
			this.oneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.twoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.threeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.newBodyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newFixtureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new Editor.BlankControl();
			this.treeView1 = new Paril.Windows.Forms.TreeViewEx();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.panel2.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// oneToolStripMenuItem
			// 
			this.oneToolStripMenuItem.Name = "oneToolStripMenuItem";
			this.oneToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
			this.oneToolStripMenuItem.Text = "One";
			// 
			// twoToolStripMenuItem
			// 
			this.twoToolStripMenuItem.Name = "twoToolStripMenuItem";
			this.twoToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
			this.twoToolStripMenuItem.Text = "Two";
			// 
			// threeToolStripMenuItem
			// 
			this.threeToolStripMenuItem.Name = "threeToolStripMenuItem";
			this.threeToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
			this.threeToolStripMenuItem.Text = "Three";
			// 
			// menuStrip1
			// 
			this.menuStrip1.AllowMerge = false;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(822, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// loadToolStripMenuItem
			// 
			this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			this.loadToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
			this.loadToolStripMenuItem.Text = "Load...";
			this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
			this.saveToolStripMenuItem.Text = "Save...";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.panel1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(822, 507);
			this.splitContainer1.SplitterDistance = 575;
			this.splitContainer1.TabIndex = 1;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.panel2);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.label1);
			this.splitContainer2.Size = new System.Drawing.Size(243, 507);
			this.splitContainer2.SplitterDistance = 195;
			this.splitContainer2.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.treeView1);
			this.panel2.Cursor = System.Windows.Forms.Cursors.Default;
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Padding = new System.Windows.Forms.Padding(1);
			this.panel2.Size = new System.Drawing.Size(243, 195);
			this.panel2.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 37);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "label1";
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newBodyToolStripMenuItem,
            this.newFixtureToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(137, 48);
			// 
			// newBodyToolStripMenuItem
			// 
			this.newBodyToolStripMenuItem.Name = "newBodyToolStripMenuItem";
			this.newBodyToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.newBodyToolStripMenuItem.Text = "New Body";
			this.newBodyToolStripMenuItem.Click += new System.EventHandler(this.newBodyToolStripMenuItem_Click);
			// 
			// newFixtureToolStripMenuItem
			// 
			this.newFixtureToolStripMenuItem.Name = "newFixtureToolStripMenuItem";
			this.newFixtureToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.newFixtureToolStripMenuItem.Text = "New Fixture";
			this.newFixtureToolStripMenuItem.Click += new System.EventHandler(this.newFixtureToolStripMenuItem_Click);
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(575, 507);
			this.panel1.TabIndex = 0;
			this.panel1.Click += new System.EventHandler(this.panel1_Click);
			// 
			// treeView1
			// 
			this.treeView1.AllowDrop = true;
			this.treeView1.BackColor = System.Drawing.SystemColors.Window;
			this.treeView1.CheckBoxes = false;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.ForeColor = System.Drawing.SystemColors.WindowText;
			this.treeView1.FullRowSelect = false;
			this.treeView1.HideSelection = true;
			this.treeView1.HotTracking = false;
			this.treeView1.ImageIndex = 0;
			this.treeView1.ImageList = null;
			this.treeView1.Indent = 19;
			this.treeView1.ItemHeight = 16;
			this.treeView1.LabelEdit = true;
			this.treeView1.Location = new System.Drawing.Point(1, 1);
			this.treeView1.Margin = new System.Windows.Forms.Padding(1);
			this.treeView1.Name = "treeView1";
			this.treeView1.PathSeparator = "\\";
			this.treeView1.Scrollable = true;
			this.treeView1.SelectedImageIndex = -1;
			this.treeView1.SelectedNode = null;
			this.treeView1.ShowLines = true;
			this.treeView1.ShowPlusMinus = true;
			this.treeView1.ShowRootLines = true;
			this.treeView1.Size = new System.Drawing.Size(239, 191);
			this.treeView1.Sorted = false;
			this.treeView1.TabIndex = 2;
			this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseUp);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(822, 531);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.Name = "Main";
			this.Text = "Main";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
			this.Load += new System.EventHandler(this.Main_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private BlankControl panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem oneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem twoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem threeToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem newBodyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newFixtureToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private Paril.Windows.Forms.TreeViewEx treeView1;
		private System.Windows.Forms.Panel panel2;
		public System.Windows.Forms.Label label1;

	}
}

