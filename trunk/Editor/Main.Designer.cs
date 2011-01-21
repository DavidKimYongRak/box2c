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
			this.mainMenu = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.decomposeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.panel2 = new System.Windows.Forms.Panel();
			this.baseTreeViewContextStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.newBodyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newFixtureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cloneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.shapeTreeViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.changeToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.circleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.polygonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.panel1 = new Editor.BlankControl();
			this.treeView1 = new Paril.Windows.Forms.TreeViewEx();
			this.mainMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.panel2.SuspendLayout();
			this.baseTreeViewContextStrip.SuspendLayout();
			this.shapeTreeViewContextMenu.SuspendLayout();
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
			// mainMenu
			// 
			this.mainMenu.AllowMerge = false;
			this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.decomposeToolStripMenuItem});
			this.mainMenu.Location = new System.Drawing.Point(0, 0);
			this.mainMenu.Name = "mainMenu";
			this.mainMenu.Size = new System.Drawing.Size(822, 24);
			this.mainMenu.TabIndex = 2;
			this.mainMenu.Text = "menuStrip1";
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
			// decomposeToolStripMenuItem
			// 
			this.decomposeToolStripMenuItem.Name = "decomposeToolStripMenuItem";
			this.decomposeToolStripMenuItem.Size = new System.Drawing.Size(82, 20);
			this.decomposeToolStripMenuItem.Text = "Decompose";
			this.decomposeToolStripMenuItem.Click += new System.EventHandler(this.decomposeToolStripMenuItem_Click);
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
			// baseTreeViewContextStrip
			// 
			this.baseTreeViewContextStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newBodyToolStripMenuItem,
            this.newFixtureToolStripMenuItem,
            this.toolStripMenuItem1,
            this.deleteToolStripMenuItem,
            this.cloneToolStripMenuItem});
			this.baseTreeViewContextStrip.Name = "contextMenuStrip1";
			this.baseTreeViewContextStrip.Size = new System.Drawing.Size(137, 98);
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
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(133, 6);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
			// 
			// cloneToolStripMenuItem
			// 
			this.cloneToolStripMenuItem.Name = "cloneToolStripMenuItem";
			this.cloneToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.cloneToolStripMenuItem.Text = "Clone";
			this.cloneToolStripMenuItem.Click += new System.EventHandler(this.cloneToolStripMenuItem_Click);
			// 
			// shapeTreeViewContextMenu
			// 
			this.shapeTreeViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeToToolStripMenuItem});
			this.shapeTreeViewContextMenu.Name = "contextMenuStrip1";
			this.shapeTreeViewContextMenu.Size = new System.Drawing.Size(142, 26);
			// 
			// changeToToolStripMenuItem
			// 
			this.changeToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.circleToolStripMenuItem,
            this.polygonToolStripMenuItem});
			this.changeToToolStripMenuItem.Name = "changeToToolStripMenuItem";
			this.changeToToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.changeToToolStripMenuItem.Text = "Change To...";
			// 
			// circleToolStripMenuItem
			// 
			this.circleToolStripMenuItem.Name = "circleToolStripMenuItem";
			this.circleToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this.circleToolStripMenuItem.Text = "Circle";
			this.circleToolStripMenuItem.Click += new System.EventHandler(this.circleToolStripMenuItem_Click);
			// 
			// polygonToolStripMenuItem
			// 
			this.polygonToolStripMenuItem.Name = "polygonToolStripMenuItem";
			this.polygonToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			this.polygonToolStripMenuItem.Text = "Polygon";
			this.polygonToolStripMenuItem.Click += new System.EventHandler(this.polygonToolStripMenuItem_Click);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
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
			this.treeView1.HideSelection = false;
			this.treeView1.HotTracking = false;
			this.treeView1.ImageIndex = -1;
			this.treeView1.ImageList = this.imageList1;
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
			this.Controls.Add(this.mainMenu);
			this.Name = "Main";
			this.Text = "Main";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
			this.Load += new System.EventHandler(this.Main_Load);
			this.mainMenu.ResumeLayout(false);
			this.mainMenu.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.baseTreeViewContextStrip.ResumeLayout(false);
			this.shapeTreeViewContextMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private BlankControl panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem oneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem twoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem threeToolStripMenuItem;
		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip baseTreeViewContextStrip;
		private System.Windows.Forms.ToolStripMenuItem newBodyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newFixtureToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private Paril.Windows.Forms.TreeViewEx treeView1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ContextMenuStrip shapeTreeViewContextMenu;
		private System.Windows.Forms.ToolStripMenuItem changeToToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem circleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem polygonToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem decomposeToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cloneToolStripMenuItem;
		private System.Windows.Forms.ImageList imageList1;

	}
}

