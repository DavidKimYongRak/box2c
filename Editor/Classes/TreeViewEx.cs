/*
 * TreeView.cs - Implementation of the
 *			"System.Windows.Forms.TreeView" class.
 *
 * Copyright (C) 2003 Neil Cawse
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace Paril.Windows.Forms
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.Drawing.Drawing2D;
	using Forms = System.Windows.Forms;
	using System.Windows.Forms.VisualStyles;

	public class TreeViewEx : Forms.Control
	{
		enum DragMoveDirection
		{
			Neither,
			Below,
			Above,
			On
		}

		internal bool checkBoxes;
		internal const int checkSize = 13;
		internal TreeNodeEx editNode;
		private bool fullRowSelect;
		private bool hideSelection;
		private bool hotTracking;
		private const int hScrollBarPixelsScrolled = 3;
		internal int imageIndex = 0;
		internal Forms.ImageList imageList;
		// No of pixels on the right of an image.
		internal const int imagePad = 5;
		internal int indent;
		private int itemHeight;
		private bool labelEdit;
		private Forms.Timer mouseClickTimer, mouseExpandTimer;
		private const int mouseEditTimeout = 350;
		internal TreeNodeExCollection nodes;
		private TreeNodeEx nodeToEdit = null;
		private string pathSeparator = @"\";
		internal TreeNodeEx root;
		private bool scrollable;
		internal int selectedImageIndex = 0;
		private TreeNodeEx selectedNode, nodeToBeDropped, selectedDropNode;
		DragMoveDirection dragMoveDirection;
		private bool showLines;
		private bool showPlusMinus;
		private bool showRootLines;
		private bool sorted;
		private Forms.TextBox textBox;
		// The node currently at the top of the control
		internal TreeNodeEx topNode;
		private int updating;
		private Forms.VScrollBar vScrollBar;
		private Forms.HScrollBar hScrollBar;
		// Offset of tree view by scrolling.
		internal int xOffset = 0;
		const int xPadding = 5;
		int xScrollValueBeforeEdit = 0;

		public event TreeViewExEventHandler AfterCheck;
		public event TreeViewExEventHandler AfterCollapse;
		public event TreeViewExEventHandler AfterExpand;
		public event NodeLabelExEditEventHandler AfterLabelEdit;
		public event TreeViewExEventHandler AfterSelect;

		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		public override Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
			set
			{
				base.BackgroundImage = value;
			}
		}

		public new event EventHandler BackgroundImageChanged
		{
			add
			{
				base.BackgroundImageChanged += value;
			}
			remove
			{
				base.BackgroundImageChanged -= value;
			}
		}

		public event TreeViewExCancelEventHandler BeforeCheck;
		public event TreeViewExCancelEventHandler BeforeCollapse;
		public event TreeViewExCancelEventHandler BeforeExpand;
		public event NodeLabelExEditEventHandler BeforeLabelEdit;
		public event TreeViewExCancelEventHandler BeforeSelect;

		internal void BeginEdit(TreeNodeEx node)
		{
			editNode = node;
			if (textBox == null)
			{
				textBox = new Forms.TextBox();
				textBox.BorderStyle = Forms.BorderStyle.FixedSingle;
				textBox.Visible = false;
				Controls.Add(textBox);
				textBox.Leave +=new EventHandler(textBox_Leave);
				textBox.KeyUp +=new Forms.KeyEventHandler(textBox_KeyUp);
			}
			textBox.Text = editNode.Text;
			RectangleF nodeBounds = node.Bounds;
			nodeBounds.Y -= 2;
			float y = nodeBounds.Y + (nodeBounds.Height - textBox.Height) /2;
			// Resize the text bounds to cover the area we want to clear.
			nodeBounds.X -= 2;
			nodeBounds.Height += 4;
			nodeBounds.Width += 4;

			if (hScrollBar != null)
			{
				xScrollValueBeforeEdit = hScrollBar.Value;
			}
			int x = (int)nodeBounds.X;
			int width = GetTextBoxWidth(ref x);

			textBox.AutoSize = false;
			textBox.SetBounds(x + 2, (int)y + 2, width, 17);
			textBox.Visible = true;
			textBox.Focus();
			textBox.SelectAll();
			// Redraw to hide the node we are editing.
			Draw(editNode);
		}

		public void BeginUpdate()
		{
			updating++;
		}

		public bool CheckBoxes
		{
			get
			{
				return checkBoxes;
			}
			set
			{
				if (checkBoxes != value)
				{
					checkBoxes = value;
					Invalidate();
				}
			}
		}

		public void CollapseAll()
		{
			root.Collapse();
		}

		protected override void CreateHandle()
		{
			base.CreateHandle();
		}

		protected override Forms.CreateParams CreateParams
		{
			get
			{
				return base.CreateParams;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(121, 97);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		public static Rectangle RectIFromRectF(RectangleF f)
		{
			return new Rectangle((int)f.X, (int)f.Y, (int)f.Width, (int)f.Height);
		}

		// Render the treeview starting from startingLine
		internal void Draw(Graphics g, TreeNodeEx startNode)
		{
			if (updating > 0)
			{
				return;
			}

			Rectangle clientRectangle = ClientRectangle;
			int drawableHeight = clientRectangle.Height;
			int drawableWidth = clientRectangle.Width - xOffset;

			// We count the visible rows to see if we need the v scrollbar but we wait before deciding if we need the h scroll bar.
			bool needsHScrollBar = false;
			bool needsVScrollBar = GetNeedVScrollBar() && scrollable;
			bool createNewVScrollBar = false;
			bool createNewHScrollBar = false;

			if (needsVScrollBar)
			{
				// Don't allow drawing on the area that is going to be the scroll bar.
				// Create the scroll bar so we can get its width.
				if (vScrollBar == null)
				{
					vScrollBar = new Forms.VScrollBar();
					createNewVScrollBar = true;
				}
				drawableWidth -= vScrollBar.Width;
				Rectangle rect = new Rectangle(drawableWidth + xOffset, 0, vScrollBar.Width, clientRectangle.Height);
				g.ExcludeClip(rect);
			}
			else
			{
				// Check to see if the top node is not the first node and we have room for the whole tree.
				// If so, abandon the draw and redraw the whole tree from the top.
				if (topNode != null && topNode != this.nodes[0])
				{
					topNode = null;
					Invalidate();
					return;
				}
				if (vScrollBar != null)
				{
					// We don't need the scroll bar anymore.
					Controls.Remove(vScrollBar);
					vScrollBar.Dispose();
					vScrollBar = null;
				}
			}
			// Is the node being processed on the screen.
			bool drawing = false;
			// Start counting from the top.
			int nodeFromTop = -1;
			// Number of nodes.
			int nodeCount = 0;
			int topNodePosition = 0;
			// The maximum width of a displayed node.
			float maxWidth = 0;
			//StringFormat format = new StringFormat(StringFormatFlags.NoWrap);
			if (topNode == null && this.nodes.Count > 0)
			{
				topNode = this.nodes[0];
			}
			RectangleF textBounds = Rectangle.Empty;

			NodeEnumeratorEx nodes = new NodeEnumeratorEx(this.nodes);
			using (Pen markerPen = new Pen(SystemColors.ControlDarkDark))
			{
				markerPen.DashStyle = DashStyle.Dot;
				while (nodes.MoveNext())
				{
					// If we havnt started drawing yet, then see if we need to and if so clear the background.
					if (!drawing)
					{
						if (nodes.currentNode  == topNode)
						{
							// We are at the top node.
							nodeFromTop = 0;
							topNodePosition = nodeCount;
						}

						// Check to see if we must start drawing. Clear the background.
						if (nodeFromTop >= 0 && (nodes.currentNode == startNode || startNode == root))
						{
							// Clear background.
							int y = ItemHeight * nodeFromTop;
							using (SolidBrush b = new SolidBrush(BackColor))
							{
								g.FillRectangle(b, 0, y, ClientSize.Width, ClientSize.Height - y);
							}
							drawing = true;
						}
					}

					// Even if we arnt drawing nodes yet, we need to measure if the nodes are visible, for hscrollbar purposes.
					if (nodeFromTop >= 0 && drawableHeight > 0)
					{
						textBounds = GetTextBounds(g, nodes.currentNode, nodeFromTop, nodes.level);
						// Is the text too wide to fit in - if so we need an h scroll bar.
						if (textBounds.Right > drawableWidth && !needsHScrollBar && scrollable)
						{
							needsHScrollBar = true;
							if (hScrollBar == null)
							{
								hScrollBar = new Forms.HScrollBar();
								createNewHScrollBar = true;
							}
							drawableHeight -= hScrollBar.Height;
							// Don't allow drawing on the area that is going to be the scroll bar.
							Rectangle rect = new Rectangle(0, clientRectangle.Height - hScrollBar.Height, clientRectangle.Width, hScrollBar.Height);
							g.ExcludeClip(rect);
						}
						if (textBounds.Right > maxWidth)
						{
							maxWidth = textBounds.Right;
						}

					}

					// Draw the node if we still have space.
					if (drawing && drawableHeight > 0)
					{
						RectangleF bounds;
						// Draw the lines and the expander.
						DrawExpanderMarker(g, markerPen, nodes.currentNode, nodeFromTop, nodes.level);

						// Draw checkboxes.
						if (checkBoxes)
						{
							bounds = GetCheckBounds(nodeFromTop, nodes.level);
							Forms.ButtonState state;
							if (nodes.currentNode.isChecked)
							{
								state = Forms.ButtonState.Checked;
							}
							else
							{
								state = Forms.ButtonState.Normal;
							}
							Forms.ControlPaint.DrawCheckBox(g, RectIFromRectF(bounds), state);
						}

						// Draw the node image.
						if (imageList != null)
						{
							bounds = GetImageBounds(nodeFromTop, nodes.level);
							int index = GetDisplayIndex(nodes.currentNode);

							if (index < imageList.Images.Count)
							{
								Image image = imageList.Images[index];
								g.DrawImage(image, bounds.X, bounds.Y);
							}
						}

						bounds = textBounds;
						// The height may be too small now.
						// If we are currently editing a node then dont draw it.
						if (drawableHeight > 0 && nodes.currentNode != editNode)
						{
							// Draw the node text.
							var bnds = RectIFromRectF(bounds);

							if (selectedDropNode == nodes.currentNode)
							{
								if (dragMoveDirection == DragMoveDirection.Above)
									g.DrawLine(Pens.Black, new PointF(bnds.X, bnds.Y), new PointF(bnds.X + bnds.Width, bnds.Y));
								else if (dragMoveDirection == DragMoveDirection.Below)
									g.DrawLine(Pens.Black, new PointF(bnds.X, bnds.Y + bnds.Height), new PointF(bnds.X + bnds.Width, bnds.Y + bnds.Height));
								else
									g.FillRectangle(SystemBrushes.Highlight, bnds);
							}

							if ((selectedDropNode == nodes.currentNode && dragMoveDirection == DragMoveDirection.On) ||
								(((nodeToBeDropped == null && nodes.currentNode == selectedNode)
								|| (nodes.currentNode == nodeToBeDropped))
								&& (Focused || !hideSelection)))
							{
								// TODO: FullRowSelect
								g.FillRectangle(SystemBrushes.Highlight, bnds);
								Forms.TextRenderer.DrawText(g, nodes.currentNode.Text, Font, bnds, SystemColors.HighlightText, Forms.TextFormatFlags.NoClipping);
								// Draw the focus rectangle.
								Rectangle r = new Rectangle((int)(bounds.X), (int)(bounds.Y), (int)(bounds.Width), (int)(bounds.Height));
								Forms.ControlPaint.DrawFocusRectangle(g, r);
							}
							else
							{
								Forms.TextRenderer.DrawText(g, nodes.currentNode.Text, Font, bnds, SystemColors.ControlText, Forms.TextFormatFlags.NoClipping);
							}
						}
						drawableHeight -= ItemHeight;
					}

					if (nodeFromTop >= 0)
					{
						nodeFromTop++;
					}
					nodeCount++;
				}
			}
			// If we need a v scroll bar, then set it up.
			if (needsVScrollBar)
			{
				SetupVScrollBar(nodeCount, needsHScrollBar, createNewVScrollBar, topNodePosition);
			}
			if (needsHScrollBar)
			{
				SetupHScrollBar(needsVScrollBar, (int)maxWidth, createNewHScrollBar, g);
			}
			else if (hScrollBar != null)
			{
				// We dont need the scroll bar.
				// If we have scrolled then we need to reset the position.
				if (xOffset != 0)
				{
					xOffset = 0;
					Invalidate();
				}
				Controls.Remove(hScrollBar);
				hScrollBar.Dispose();
				hScrollBar = null;
			}
		}

		// Draw from startNode downwards
		internal void Draw(TreeNodeEx startNode)
		{
			if (!Created || !Visible)
				return;
			using (Graphics g = CreateGraphics())
			{
				Draw(g, startNode);
			}
		}

		internal static VisualStyleRenderer _closedRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
		internal static VisualStyleRenderer _openedRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);

		private void DrawExpanderMarker(Graphics g, Pen markerPen, TreeNodeEx node, int nodeFromTop, int level)
		{
			Rectangle bounds = GetExpanderBounds(nodeFromTop, level);
			int midX = bounds.X + 4;
			int midY = bounds.Y + bounds.Height / 2;
			int lineRightStart = midX;
			int lineTopEnd = midY;
			if (node.Nodes.Count > 0 && showPlusMinus)
			{
				GraphicsPath path = new GraphicsPath();

				{
					if (!node.IsExpanded)
						_closedRenderer.DrawBackground(g, new Rectangle(midX - 4, midY - 4, 10, 10));
					else
						_openedRenderer.DrawBackground(g, new Rectangle(midX - 4, midY - 4, 10, 10));
				}
				lineRightStart += 6;
				lineTopEnd -= 6;
			}
			if (!showLines)
			{
				return;
			}
			// Draw the right lead line
			if (bounds.Right > lineRightStart)
				g.DrawLine(markerPen, lineRightStart, midY, bounds.Right - 6, midY);
			// Draw the top lead line
			TreeNodeEx lineNode = node.PrevNode;
			if (lineNode == null)
				lineNode = node.Parent;
			if (lineNode != null)
				g.DrawLine(markerPen, midX, lineNode.markerLineY + 2, midX, lineTopEnd);
			if (node.Nodes.Count > 0)
				node.markerLineY = midY + 6;
			else
				node.markerLineY = midY;
		}

		internal void EndEdit(bool cancel)
		{
			if (!cancel)
			{
				editNode.Text = textBox.Text;
				editNode.OnRenamed();
			}

			textBox.Visible = false;
			editNode = null;

			if (hScrollBar != null)
			{
				hScrollBar.Value = xScrollValueBeforeEdit;
			}
		}

		public void EndUpdate()
		{
			if (updating < 2)
			{
				updating = 0;
				Invalidate();
			}
			else
			{
				updating--;
			}
		}

		public void ExpandAll()
		{
			root.ExpandAll();
		}

		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}

		public bool FullRowSelect
		{
			get
			{
				return fullRowSelect;
			}
			set
			{
				if (fullRowSelect != value)
				{
					fullRowSelect = value;
					Invalidate();
				}
			}
		}
		protected Forms.OwnerDrawPropertyBag GetItemRenderStyles(TreeNodeEx node, int state)
		{
			// TODO: Property Bag
			return null;
		}

		// Get the node image index to display depending on what is set.
		private int GetDisplayIndex(TreeNodeEx node)
		{
			int index = 0;
			if (node == selectedNode)
			{
				if (node.SelectedImageIndex > -1)
					index = node.SelectedImageIndex;
				else if (selectedImageIndex > -1)
					index = selectedImageIndex;
				else if (this.imageIndex > -1)
					index = this.imageIndex;
			}
			else
			{
				if (node.ImageIndex > -1)
					index = node.ImageIndex;
				else if (this.imageIndex > -1)
					index = this.imageIndex;
			}
			return index;
		}

		// Returns true if we dont have vertical space to draw all the items.
		private bool GetNeedVScrollBar()
		{
			int fullNodes = VisibleCount;
			NodeEnumeratorEx nodes = new NodeEnumeratorEx(Nodes);
			while (nodes.MoveNext())
			{
				if (--fullNodes == 0)
				{
					return true;
				}
			}
			return false;
		}

		public TreeNodeEx GetNodeAt(int x, int y)
		{
			int height = ItemHeight;
			int nodeFromTop = -1;
			NodeEnumeratorEx nodes = new NodeEnumeratorEx(this.nodes);
			while (nodes.MoveNext())
			{
				if (nodes.currentNode == topNode)
				{
					// We are now at the top of the control.
					nodeFromTop = 1;
				}
				if (nodeFromTop > -1)
				{
					if (y < height * nodeFromTop)
					{
						return nodes.currentNode;
					}
					nodeFromTop++;
				}
			}
			return null;
		}

		public TreeNodeEx GetNodeAt(Point pt)
		{
			return GetNodeAt(pt.X, pt.Y);
		}

		// Return the bounds of a check given the node from the top and an x level.
		internal Rectangle GetCheckBounds(int nodeFromTop, int level)
		{
			if (!checkBoxes)
				return Rectangle.Empty;
			int height = ItemHeight;
			int y = height * nodeFromTop + (height - checkSize) / 2;
			int x = (level + 1) * indent - xOffset + xPadding;
			return new Rectangle(x, y, checkSize, checkSize);
		}

		// Return the bounds of an expander given the node from the top and an x level.
		internal Rectangle GetExpanderBounds(int nodeFromTop, int level)
		{
			int height = ItemHeight;
			int y = height * nodeFromTop;
			int x = level* indent - xOffset + xPadding;
			return new Rectangle(x, y, indent, height);
		}

		// Return the bounds of an image given the node from the top and an x level.
		internal Rectangle GetImageBounds(int nodeFromTop, int level)
		{
			int height = ItemHeight;
			int y = height * nodeFromTop + (height - imageList.ImageSize.Height) / 2;
			int x = (level + 1) * indent - xOffset + xPadding;
			// Add on the width of the checkBoxes if applicable.
			if (checkBoxes)
			{
				x += checkSize;
			}
			return new Rectangle(x, y, imageList.ImageSize.Width + imagePad, imageList.ImageSize.Height);
		}

		public RectangleF GetNodeBounds(TreeNodeEx node)
		{
			if (node.parent != null)
			{
				int nodeFromTop = -1;
				NodeEnumeratorEx nodes = new NodeEnumeratorEx(this.nodes);
				while (nodes.MoveNext())
				{
					if (nodes.currentNode == topNode)
					{
						// We are at the top of the control.
						nodeFromTop = 0;
					}
					if (nodes.currentNode == node)
					{
						using (Graphics g = CreateGraphics())
						{
							return GetTextBounds(g, node, nodeFromTop, nodes.level);
						}
					}
					if (nodeFromTop >= 0)
					{
						nodeFromTop++;
					}
				}
			}
			return Rectangle.Empty;
		}

		public int GetNodeCount(bool includeSubTrees)
		{
			return root.GetNodeCount(includeSubTrees);
		}

		// Get the bounds of a node. Supply a Graphics to measure the text, the node being measured, the number of the node being measured in the list of those being shown, the number of the node that is the first to be displayed (topNode) and the level of x indent.
		internal RectangleF GetTextBounds(Graphics g, TreeNodeEx node, int nodeFromTop, int level)
		{
			int height = ItemHeight;
			int y = height * nodeFromTop;
			// Calculate the basic offset from the level and the indent.
			int x = (level + 1) * indent - xOffset;
			// Add on the width of the image if applicable.
			if (imageList != null)
			{
				x += imageList.ImageSize.Width + imagePad;
			}
			// Add on the width of the checkBoxes if applicable.
			if (checkBoxes)
			{
				x += checkSize;
			}

			Font font;
			if (node.nodeFont == null)
			{
				font = Font;
			}
			else
			{
				font = node.nodeFont;
			}

			float width = g.MeasureString(node.text, font).Width;
			if (width < 5)
				width = 5;

			return new RectangleF(x, y, width, height);
		}

		// Returns the width the text box should be based on the width of the text.
		private int GetTextBoxWidth(ref int x)
		{
			const int minTextBoxWidth = 40;
			const int extraSpacing = 10;
			using (Graphics g = textBox.CreateGraphics())
			{
				SizeF size = g.MeasureString(textBox.Text, Font);
				int width = (int)size.Width + extraSpacing;
				if (width < minTextBoxWidth)
				{
					width = minTextBoxWidth;
				}
				int maxPossibleWidth = ClientRectangle.Width;
				if (vScrollBar != null)
				{
					maxPossibleWidth -= vScrollBar.Width;
				}
				int maxWidth = maxPossibleWidth - x;

				if (width > maxWidth)
				{
					width = maxWidth;
					// Try and move the control over to allow more space for the textbox.
					if (hScrollBar != null)
					{
						int offsetBefore = xOffset;
						hScrollBar.Value = hScrollBar.Maximum - hScrollBar.LargeChange + 1;
						int move = xOffset - offsetBefore;
						width += move;
						x -= move;
						if (x < 0)
						{
							x = 0;
						}
						if (width > maxPossibleWidth - x)
						{
							width = maxPossibleWidth - x;
						}

					}
				}
				return width;
			}
		}

		public bool HideSelection
		{
			get
			{
				return hideSelection;
			}
			set
			{
				if (hideSelection != value)
				{
					hideSelection = value;
					Invalidate();
				}
			}
		}

		public bool HotTracking
		{
			get
			{
				return hotTracking;
			}
			set
			{
				if (value != hotTracking)
				{
					hotTracking = value;
				}
			}
		}

		private void hScrollBar_ValueChanged(object sender, EventArgs e)
		{
			xOffset = hScrollBar.Value * hScrollBarPixelsScrolled;
			Invalidate();
		}

		public int ImageIndex
		{
			get
			{
				return imageIndex;
			}
			set
			{
				if (value != imageIndex)
				{
					imageIndex = value;
					Invalidate();
				}
			}
		}
		public Forms.ImageList ImageList
		{
			get
			{
				return imageList;
			}
			set
			{
				if (value != imageList)
				{
					imageList = value;
					Invalidate();
				}
			}
		}

		public int Indent
		{
			get
			{
				return indent;
			}
			set
			{
				if (value != indent)
				{
					indent = value;
					Invalidate();
				}
			}
		}

		// Invalidate from startNode down.
		internal void InvalidateDown(TreeNodeEx startNode)
		{
			if (updating > 0 || this.nodes == null)
			{
				return;
			}

			// Find the position of startNode relative to the top node.
			int nodeFromTop = -1;
			NodeEnumeratorEx nodes = new NodeEnumeratorEx(this.nodes);
			while (nodes.MoveNext())
			{
				if (nodes.currentNode == topNode)
				{
					// We are at the top of the control.
					nodeFromTop = 0;
				}
				if (nodes.currentNode == startNode)
				{
					break;
				}
				if (nodeFromTop >= 0)
				{
					nodeFromTop++;
				}
			}
			// Calculate the y position of startNode.
			int y = nodeFromTop * ItemHeight;
			// Invalidate from this position down.
			// Start one pixel higher to cover the focus rectangle.
			Invalidate(new Rectangle(0, y - 1, ClientRectangle.Width, ClientRectangle.Height - y + 1));
		}

		protected override bool IsInputKey(Forms.Keys keyData)
		{
			if (editNode != null && (keyData & Forms.Keys.Alt) == 0)
			{
				Forms.Keys key = keyData & Forms.Keys.KeyCode;
				if (key == Forms.Keys.Return || key == Forms.Keys.Escape || key == Forms.Keys.Prior || key == Forms.Keys.Next || key == Forms.Keys.Home || key == Forms.Keys.End)
					return true;
			}
			return base.IsInputKey(keyData);
		}

		public event Forms.ItemDragEventHandler ItemDrag;

		public int ItemHeight
		{
			get
			{
				if (itemHeight == -1)
					return FontHeight + 3;
				return itemHeight;
			}
			set
			{
				if (value != itemHeight)
				{
					itemHeight = value;
					Invalidate();
				}
			}
		}

		public bool LabelEdit
		{
			get
			{
				return labelEdit;
			}
			set
			{
				if (value != labelEdit)
				{
					labelEdit = value;
				}
			}
		}

		// This handles the timeout on the click timer and begins an edit.
		private void mouseClickTimer_Tick(object sender, EventArgs e)
		{
			mouseClickTimer.Stop();
			if (nodeToEdit != null)
			{
				nodeToEdit.BeginEdit();
				nodeToEdit = null;
			}
		}

		public TreeNodeExCollection Nodes
		{
			get
			{
				return nodes;
			}
		}

		protected internal virtual void OnAfterCheck(TreeViewExEventArgs e)
		{
			if (AfterCheck != null)
				AfterCheck(this, e);
		}

		protected internal virtual void OnAfterCollapse(TreeViewExEventArgs e)
		{
			if (AfterCollapse != null)
				AfterCollapse(this, e);
		}

		protected virtual void OnAfterExpand(TreeViewExEventArgs e)
		{
			if (AfterExpand != null)
				AfterExpand(this, e);
		}

		protected virtual void OnAfterLabelEdit(NodeLabelExEditEventArgs e)
		{
			if (AfterLabelEdit != null)
				AfterLabelEdit(this, e);
		}

		protected virtual void OnAfterSelect(TreeViewExEventArgs e)
		{
			if (AfterSelect != null)
				AfterSelect(this, e);
		}

		protected internal virtual void OnBeforeCheck(TreeViewExCancelEventArgs e)
		{
			if (BeforeCheck != null)
				BeforeCheck(this, e);
		}

		protected internal virtual void OnBeforeCollapse(TreeViewExCancelEventArgs e)
		{
			if (BeforeCollapse != null)
				BeforeCollapse(this, e);
		}

		protected virtual void OnBeforeExpand(TreeViewExCancelEventArgs e)
		{
			if (BeforeExpand != null)
				BeforeExpand(this, e);
		}

		protected virtual void OnBeforeLabelEdit(NodeLabelExEditEventArgs e)
		{
			if (BeforeLabelEdit != null)
				BeforeLabelEdit(this, e);
		}

		protected virtual void OnBeforeSelect(TreeViewExCancelEventArgs e)
		{
			if (BeforeSelect != null)
				BeforeSelect(this, e);
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			if (nodes.Count > 0)
			{
				SelectedNode.Invalidate();
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);
		}

		protected virtual void OnItemDrag(Forms.ItemDragEventArgs e)
		{
			if (ItemDrag != null)
				ItemDrag(this, e);
		}

		protected override void OnKeyDown(Forms.KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled && checkBoxes && selectedNode != null && (e.KeyData & Forms.Keys.KeyCode) == Forms.Keys.Space)
			{
				TreeViewExCancelEventArgs args = new TreeViewExCancelEventArgs(selectedNode, false, TreeViewExAction.ByKeyboard);
				this.OnBeforeCheck(args);
				if (!args.Cancel)
				{
					selectedNode.isChecked = !selectedNode.isChecked;
					selectedNode.Invalidate();
					this.OnAfterCheck(new TreeViewExEventArgs(selectedNode, TreeViewExAction.ByKeyboard));
				}
				e.Handled = true;
			}
		}

		protected override void OnKeyPress(Forms.KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			// Swallow the space
			if (e.KeyChar == ' ')
			{
				e.Handled = true;
			}
		}

		protected override void OnKeyUp(Forms.KeyEventArgs e)
		{
			base.OnKeyUp(e);
			// Swallow the space
			if ((e.KeyData & Forms.Keys.KeyCode) == Forms.Keys.Space)
			{
				e.Handled = true;
			}
		}

		// Non Microsoft member.
		protected override void OnMouseDown(Forms.MouseEventArgs e)
		{
			nodeToEdit = null;
			if (e.Button == Forms.MouseButtons.Left)
			{
				int nodeFromTop = -1;
				// Iterate through all the nodes, looking for the bounds that match.
				NodeEnumeratorEx nodes = new NodeEnumeratorEx(this.nodes);
				bool _clicked = false;

				while (nodes.MoveNext())
				{
					if (nodes.currentNode == topNode)
					{
						// We are now at the top of the control.
						nodeFromTop = 0;
					}
					if (nodeFromTop > -1)
					{
						if (GetExpanderBounds(nodeFromTop, nodes.level).Contains(e.X, e.Y))
						{
							nodes.currentNode.Toggle();
							_clicked = true;
							break;
						}
						else if (GetCheckBounds(nodeFromTop, nodes.level).Contains(e.X, e.Y))
						{
							TreeViewExCancelEventArgs args = new TreeViewExCancelEventArgs(nodes.currentNode, false, TreeViewExAction.ByMouse);
							OnBeforeCheck(args);
							if (!args.Cancel)
							{
								nodes.currentNode.isChecked = !nodes.currentNode.isChecked;
								OnAfterCheck(new TreeViewExEventArgs(nodes.currentNode, TreeViewExAction.ByMouse));
							}

							Invalidate(GetCheckBounds(nodeFromTop, nodes.level));
							_clicked = true;
							break;

						}
						nodeFromTop++;
					}
				}

				if (!_clicked)
				{
					var pt = new Point(e.X, e.Y);
					var node = GetNodeAt(e.X, e.Y);
					
					if (node != null && node.Bounds.Contains(pt))
					{
						Focus();
						nodeToBeDropped = node;

						if (selectedNode != null)
							selectedNode.Invalidate();
						nodeToBeDropped.Invalidate();
					}
				}
			}
			else
			{
				ProcessClick(e.X, e.Y, true);
			}
			base.OnMouseDown(e);
		}

		// Non Microsoft member.
		protected override void OnMouseMove(Forms.MouseEventArgs e)
		{
			var node = GetNodeAt(e.X, e.Y);

			if (nodeToBeDropped != null && nodeToBeDropped != node && node.CanDragDrop())
			{
				DoDragDrop(nodeToBeDropped, Forms.DragDropEffects.Move | Forms.DragDropEffects.Copy | Forms.DragDropEffects.Link);

				if (selectedNode != null)
					selectedNode.Invalidate();
			}

			//TODO: Hot tracking.
			base.OnMouseMove(e);
		}

		// Non Microsoft member.
		protected override void OnMouseLeave(EventArgs e)
		{
			//TODO: Hot tracking.
			base.OnMouseLeave(e);
		}

		protected override void OnDragDrop(Forms.DragEventArgs drgevent)
		{
			if (selectedDropNode != null && nodeToBeDropped != null)
			{
				TreeNodeExParent oldParent, newParent;

				if (nodeToBeDropped.Parent != null)
				{
					oldParent = new TreeNodeExParent(nodeToBeDropped.Parent);
					nodeToBeDropped.Parent.Nodes.Remove(nodeToBeDropped);
				}
				else
				{
					oldParent = new TreeNodeExParent(nodeToBeDropped.TreeView);
					nodeToBeDropped.TreeView.Nodes.Remove(nodeToBeDropped);
				}

				if (dragMoveDirection == DragMoveDirection.On)
				{
					selectedDropNode.Nodes.Add(nodeToBeDropped);
					newParent = new TreeNodeExParent(selectedDropNode);
				}
				else
				{
					int pos = (dragMoveDirection == DragMoveDirection.Above) ? selectedDropNode.Index : selectedDropNode.Index + 1;

					if (selectedDropNode.Parent == null)
					{
						Nodes.Insert(pos, nodeToBeDropped);
						newParent = new TreeNodeExParent(this);
					}
					else
					{
						selectedDropNode.Parent.Nodes.Insert(pos, nodeToBeDropped);
						newParent = new TreeNodeExParent(selectedDropNode.Parent);
					}
				}
				
				selectedDropNode.Invalidate();
				nodeToBeDropped.Invalidate();

				// tell the dropped node
				nodeToBeDropped.OnNodeMoved(new TreeNodeExMovedEventArgs(selectedDropNode, oldParent, newParent));

				// tell the receiver
				selectedDropNode.OnNodeDropped(new TreeNodeExMovedEventArgs(selectedDropNode, oldParent, newParent));

				selectedDropNode = nodeToBeDropped = null;
			}

			if (mouseExpandTimer != null)
			{
				mouseExpandTimer.Dispose();
				mouseExpandTimer = null;
			}

			Invalidate();

			base.OnDragDrop(drgevent);
		}

		protected override void OnDragEnter(Forms.DragEventArgs drgevent)
		{
			base.OnDragEnter(drgevent);
		}

		protected override void OnDragLeave(EventArgs e)
		{
			base.OnDragLeave(e);
		}

		bool HasParent(TreeNodeEx check, TreeNodeEx parent)
		{
			var node = check;

			while (node != null)
			{
				if (node == parent)
					return true;

				node = node.Parent;
			}

			return false;
		}

		protected override void OnDragOver(Forms.DragEventArgs drgevent)
		{
			var pt = PointToClient(new Point (drgevent.X, drgevent.Y));

			// Check if we can drag down
			if (vScrollBar != null)
			{
				if (pt.Y > (Height - 26) && (vScrollBar.Value < vScrollBar.Maximum - vScrollBar.LargeChange + 1))
					vScrollBar.Value++;
				else if (pt.Y < 26 && (vScrollBar.Value > vScrollBar.Minimum))
					vScrollBar.Value--;
			}

			var dragItem = GetNodeAt(pt);

			bool newItem = (dragItem != selectedDropNode);

			if (dragItem == nodeToBeDropped || HasParent(dragItem, nodeToBeDropped))
			{
				if (selectedDropNode != null)
					selectedDropNode.Invalidate();
				selectedDropNode = null;
				drgevent.Effect = Forms.DragDropEffects.None;
			}
			else
			{
				if (dragItem != selectedDropNode && selectedDropNode != null)
					selectedDropNode.Invalidate();

				if (dragItem != null)
				{
					var bnds = GetNodeBounds(dragItem);
					bool above = (pt.Y < (bnds.Y + (bnds.Height / 2)));
					bool on = (pt.Y > (bnds.Y + 4)) && (pt.Y < (bnds.Y + bnds.Height - 4));

					dragMoveDirection = DragMoveDirection.Neither;

					if (on && dragItem.CanDropOn(nodeToBeDropped))
						dragMoveDirection = DragMoveDirection.On;
					else if (above)
					{
						bool canUp = dragItem.CanDropAbove(nodeToBeDropped);
						bool canOn = dragItem.CanDropOn(nodeToBeDropped);

						if (!canUp && canOn)
							dragMoveDirection = DragMoveDirection.On;
						else if (canUp)
							dragMoveDirection = DragMoveDirection.Above;
					}
					else if (!above)
					{
						bool canBelow = dragItem.CanDropUnder(nodeToBeDropped);
						bool canOn = dragItem.CanDropOn(nodeToBeDropped);

						if (!canBelow && canOn)
							dragMoveDirection = DragMoveDirection.On;
						else if (canBelow)
							dragMoveDirection = DragMoveDirection.Below;
					}

					if (dragMoveDirection == DragMoveDirection.Neither)
					{
						drgevent.Effect = Forms.DragDropEffects.None;
						selectedDropNode = dragItem = null;
					}
				}

				if (dragItem != null)
				{
					selectedDropNode = dragItem;

					if (selectedDropNode != null)
						selectedDropNode.Invalidate();

					drgevent.Effect = Forms.DragDropEffects.Move;

					if (newItem && selectedDropNode.Nodes.Count != 0 && !selectedDropNode.IsExpanded)
					{
						if (mouseExpandTimer == null)
						{
							mouseExpandTimer = new Forms.Timer();
							mouseExpandTimer.Interval = 550;
							mouseExpandTimer.Tick += new EventHandler(mouseExpandTimer_Tick);
						}

						mouseExpandTimer.Stop();
						mouseExpandTimer.Start();
					}
				}
				else if (mouseExpandTimer != null)
					mouseExpandTimer.Stop();
			}

			base.OnDragOver(drgevent);
		}

		void mouseExpandTimer_Tick(object sender, EventArgs e)
		{
			if (selectedDropNode != null && !selectedDropNode.IsExpanded)
				selectedDropNode.Expand();
		}

		protected override void OnQueryContinueDrag(Forms.QueryContinueDragEventArgs qcdevent)
		{
			if (selectedDropNode == null && (Forms.Control.MouseButtons & Forms.MouseButtons.Left) != Forms.MouseButtons.Left)
			{
				nodeToBeDropped = null;
				qcdevent.Action = Forms.DragAction.Cancel;
			}

			base.OnQueryContinueDrag(qcdevent);
		}

		protected override void OnGiveFeedback(Forms.GiveFeedbackEventArgs gfbevent)
		{
			base.OnGiveFeedback(gfbevent);
		}

		// Non Microsoft member.
		protected override void OnMouseUp(Forms.MouseEventArgs e)
		{
			ProcessClick(e.X, e.Y, (e.Button == Forms.MouseButtons.Right));

			if (nodeToBeDropped != null)
			{
				nodeToBeDropped.Invalidate();
				nodeToBeDropped = null;
			}

			if (selectedNode != null)
				selectedNode.Invalidate();

			base.OnMouseUp(e);
		}

		void ProcessClick(int x, int y, bool rightMouse)
		{
			int nodeFromTop = -1;
			int height = ItemHeight;
			using (Graphics g = CreateGraphics())
			{
				// Iterate through all the nodes, looking for the bounds that match.
				NodeEnumeratorEx nodes = new NodeEnumeratorEx(this.nodes);
				while (nodes.MoveNext())
				{
					if (nodes.currentNode == topNode)
					{
						// We are now at the top of the control.
						nodeFromTop = 0;
					}
					if (nodeFromTop > -1)
					{
						// Check if the y matches this node.
						if (y < height * (nodeFromTop + 1))
						{
							bool allowEdit = false;
							bool allowSelect = true;
							// Clicking the image can be used to select.
							if (imageList == null || !GetImageBounds(nodeFromTop, nodes.level).Contains(x, y))
							{
								// Clicking the text can be used to edit and select.
								// if false then the hierarchy marker must have been clicked.
								if (GetTextBounds(g, nodes.currentNode, nodeFromTop, nodes.level).Contains(x, y))
								{
									allowEdit = true;
								}
								else
								{
									allowSelect = false;
								}
							}
							if (SelectedNode == nodes.Current && mouseClickTimer != null && mouseClickTimer.Enabled && (allowEdit || allowSelect))
							{
								mouseClickTimer.Stop();
								nodeToEdit = null;
								nodes.currentNode.Toggle();
								return;
							}
							if (allowSelect || rightMouse)
							{
								if (selectedNode == nodes.Current)
								{
									if (labelEdit && allowEdit && !rightMouse)
									{
										if (selectedNode.CanRename())
											nodeToEdit = nodes.currentNode;
									}
									Focus();
								}
								else
								{
									nodeToEdit = null;
									// Do the events.
									TreeViewExCancelEventArgs eventArgs = new TreeViewExCancelEventArgs(nodes.currentNode, false, TreeViewExAction.ByMouse);
									OnBeforeSelect(eventArgs);
									if (!eventArgs.Cancel)
									{
										SelectedNode = nodes.currentNode;
										OnAfterSelect(new TreeViewExEventArgs(nodes.currentNode));
										Focus();
									}
								}
								if (rightMouse)
								{
									return;
								}
								if (mouseClickTimer == null)
								{
									mouseClickTimer = new Forms.Timer();
									mouseClickTimer.Tick +=new EventHandler(mouseClickTimer_Tick);
									mouseClickTimer.Interval = mouseEditTimeout;
								}

								mouseClickTimer.Start();
								break;
							}
						}
						nodeFromTop++;
					}
				}
			}
		}

		protected override bool ProcessDialogKey(Forms.Keys keyData)
		{
			if ((keyData & Forms.Keys.Alt) == 0)
			{
				Forms.Keys key = keyData & Forms.Keys.KeyCode;
				bool shiftKey = (keyData & Forms.Keys.Shift) != 0;
				bool controlKey = (keyData & Forms.Keys.Control) != 0;
				TreeNodeEx selectedNode = SelectedNode;

				switch (key)
				{
				case Forms.Keys.Left:
					if (selectedNode != null)
					{
						if (selectedNode.IsExpanded)
						{
							selectedNode.Collapse();
						}
						else if (selectedNode.Parent != null)
						{
							SelectedNode = selectedNode.Parent;
						}
					}
					return true;
				case Forms.Keys.Right:
					if (selectedNode != null && selectedNode.Nodes.Count != 0)
					{
						if (selectedNode.IsExpanded)
						{
							SelectedNode = selectedNode.NextVisibleNode;
						}
						else
						{
							selectedNode.Expand();
						}
					}
					return true;
				case Forms.Keys.Up:
					if (selectedNode != null)
					{
						selectedNode = selectedNode.PrevVisibleNode;
						if (selectedNode != null)
						{
							SelectedNode = selectedNode;
						}
					}
					return true;
				case Forms.Keys.Down:
					if (selectedNode != null)
					{
						selectedNode = selectedNode.NextVisibleNode;
						if (selectedNode != null)
						{
							SelectedNode = selectedNode;
						}
					}
					return true;
				case Forms.Keys.Home:
					if (Nodes[0] != null)
					{
						SelectedNode = Nodes[0];
					}
					return true;
				case Forms.Keys.End:
					{
						NodeEnumeratorEx nodes = new NodeEnumeratorEx(this.nodes);
						while (nodes.MoveNext())
						{
						}
						SelectedNode = nodes.currentNode;
						return true;
					}
				case Forms.Keys.Prior:
					{
						int nodePosition = 0;
						// Get the position of the current selected node.
						NodeEnumeratorEx nodes = new NodeEnumeratorEx(this.nodes);
						while (nodes.MoveNext())
						{
							if (nodes.currentNode == selectedNode)
							{
								break;
							}
							nodePosition++;
						}

						nodePosition -= VisibleCountActual - 1;
						if (nodePosition < 0)
						{
							nodePosition = 0;
						}

						// Get the node that corresponds to the position.
						nodes.Reset();
						while (nodes.MoveNext())
						{
							if (nodePosition-- == 0)
							{
								break;
							}
						}

						// Set the selectedNode.
						SelectedNode = nodes.currentNode;

					}
					return true;
				case Forms.Keys.Next:
					{
						int rows = 0;
						int rowsPerPage = VisibleCountActual;
						NodeEnumeratorEx nodes = new NodeEnumeratorEx(this.nodes);
						while (nodes.MoveNext())
						{
							if (nodes.currentNode == selectedNode || rows > 0)
							{
								rows++;
								if (rows >= rowsPerPage)
								{
									break;
								}
							}
						}
						SelectedNode = nodes.currentNode;

						return true;
					}
				}

			}
			return base.ProcessDialogKey(keyData);
		}


		// Non Microsoft member.
		protected override void OnPaint(Forms.PaintEventArgs e)
		{
			Draw(e.Graphics, root);
		}

		// Non Microsoft member.
		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);
			if (selectedNode != null)
			{
				selectedNode.Invalidate();
			}
		}


		public new event Forms.PaintEventHandler Paint
		{
			add
			{
				base.Paint += value;
			}
			remove
			{
				base.Paint -= value;
			}
		}

		public string PathSeparator
		{
			get
			{
				return pathSeparator;
			}
			set
			{
				pathSeparator = value;
			}
		}
		// Emulate the Microsoft behaviour of resetting the control when they have to recreate the handle.
		internal void ResetView()
		{
			topNode = null;
			CollapseAll();
			Draw(root);
		}

		public bool Scrollable
		{
			get
			{
				return scrollable;
			}
			set
			{
				if (value != scrollable)
				{
					scrollable = value;
				}
			}
		}

		protected override void OnMouseWheel(Forms.MouseEventArgs e)
		{
			if (vScrollBar != null)
			{
				if (e.Delta > 0 && (vScrollBar.Value > vScrollBar.Minimum))
					vScrollBar.Value--;
				else if (e.Delta < 0 && (vScrollBar.Value < vScrollBar.Maximum - vScrollBar.LargeChange + 1))
					vScrollBar.Value++;
			}

			base.OnMouseWheel(e);
		}

		public int SelectedImageIndex
		{
			get
			{
				if (imageList ==null)
				{
					return -1;
				}
				if (selectedImageIndex >= imageList.Images.Count)
				{
					if (selectedImageIndex == -1)
					{
						return 0;
					}
					return imageList.Images.Count - 1;
				}
				return selectedImageIndex;
			}
			set
			{
				if (selectedImageIndex != value)
				{
					selectedImageIndex = value;
				}
			}
		}

		public TreeNodeEx SelectedNode
		{
			get
			{
				if (selectedNode == null && nodes.Count > 0)
				{
					return nodes[0];
				}
				return selectedNode;
			}
			set
			{
				if (value != selectedNode)
				{
					// Redraw the old item
					if (selectedNode != null)
					{
						TreeNodeEx oldNode = selectedNode;
						selectedNode = value;
						oldNode.Invalidate();
					}
					else
					{
						selectedNode = value;
					}

					if (selectedNode != null)
					{
						selectedNode.Invalidate();
						selectedNode.EnsureVisible();
					}
				}
			}
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, Forms.BoundsSpecified specified)
		{
			base.SetBoundsCore(x, y, width, height, specified);
			if ((specified & Forms.BoundsSpecified.Size) != 0)
			{
				Invalidate();
			}
		}

		private void SetupHScrollBar(bool needsVScrollBar, int maxWidth, bool createNew, Graphics g)
		{
			Rectangle clientRectangle = ClientRectangle;
			int width = clientRectangle.Width;
			if (needsVScrollBar)
			{
				width -= vScrollBar.Width;
			}

			// If a node remove operation has caused the right most node to be removed, then the xoffset needs to be moved back.
			// The "hScrollBarPixelsScrolled" is because xOffset can only occur in increments of hScrollBarPixelsScrolled.
			if (maxWidth < width - hScrollBarPixelsScrolled * 2)
			{
				xOffset -= width - maxWidth;
				Invalidate();
				return;
			}

			//hScrollBar.Value = xOffset/hScrollBarPixelsScrolled;
			hScrollBar.Maximum = (maxWidth + xOffset) / hScrollBarPixelsScrolled;
			hScrollBar.LargeChange = width / hScrollBarPixelsScrolled;
			// Set the position of the H scroll bar but leave a hole if they are both visible.
			hScrollBar.SetBounds(0, clientRectangle.Height - hScrollBar.Height, width, hScrollBar.Height);
			// Force a redraw because if none of the hScrollBar values above change, we still want to make sure it is redrawn.
			hScrollBar.Invalidate();

			if (createNew)
			{
				hScrollBar.ValueChanged +=new EventHandler(hScrollBar_ValueChanged);
				Controls.Add(hScrollBar);
			}

			// Draw the gap between the two if needed.
			if (needsVScrollBar)
			{
				Rectangle gap = new Rectangle(hScrollBar.Right, hScrollBar.Top, vScrollBar.Width, hScrollBar.Height);
				g.FillRectangle(SystemBrushes.Control, gap);
			}
		}

		private void SetupVScrollBar(int nodeCount, bool needsHScrollBar, bool createNew, int selectedNodePosition)
		{
			Rectangle clientRectangle = ClientRectangle;
			vScrollBar.Maximum = nodeCount;
			// Set the position of the V scroll bar but leave a hole if they are both visible.
			int height = clientRectangle.Height;
			if (needsHScrollBar)
			{
				height -= hScrollBar.Height;
			}
			vScrollBar.LargeChange = height / ItemHeight;
			vScrollBar.Value = selectedNodePosition;
			vScrollBar.SetBounds(clientRectangle.Width - vScrollBar.Width, 0, vScrollBar.Width, height);

			if (createNew)
			{
				Controls.Add(vScrollBar);
				vScrollBar.ValueChanged+=new EventHandler(vScrollBar_ValueChanged);
			}
		}

		public bool ShowLines
		{
			get
			{
				return showLines;
			}
			set
			{
				if (value != showLines)
				{
					showLines = value;
				}
			}
		}

		public bool ShowPlusMinus
		{
			get
			{
				return showPlusMinus;
			}
			set
			{
				if (value != showPlusMinus)
				{
					showPlusMinus = value;
				}
			}
		}

		public bool ShowRootLines
		{
			get
			{
				return showRootLines;
			}
			set
			{
				if (value != showRootLines)
				{
					showRootLines = value;
				}
			}
		}

		public bool Sorted
		{
			get
			{
				return sorted;
			}
			set
			{
				if (value != sorted)
				{
					sorted = value;
					//TODO: could be done better!
					TreeNodeEx[] nodes = new TreeNodeEx[Nodes.Count];
					Nodes.CopyTo(nodes, 0);
					Nodes.Clear();
					Nodes.AddRange(nodes);
				}
			}
		}

		public override String Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}


		private void textBox_KeyUp(Object sender, Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Forms.Keys.Enter)
			{
				textBox.Visible = false;
			}
			else
			{
				int x = textBox.Left;
				textBox.Width = GetTextBoxWidth(ref x);
				textBox.Left = x;
			}
		}

		private void textBox_Leave(object sender, EventArgs e)
		{
			EndEdit(false);
		}

		public new event EventHandler TextChanged
		{
			add
			{
				base.TextChanged += value;
			}
			remove
			{
				base.TextChanged -= value;
			}
		}

		public TreeViewEx()
			: base()
		{
			hideSelection = true;
			indent = 19;
			itemHeight = -1;
			scrollable = true;
			showLines = true;
			showPlusMinus = true;
			showRootLines = true;
			root = new TreeNodeEx(this);
			root.Expand();
			nodes = new TreeNodeExCollection(root);
			BackColor = SystemColors.Window;
			ForeColor = SystemColors.WindowText;
			SetStyle(Forms.ControlStyles.StandardClick, false);
			// Switch on double buffering.
			SetStyle(Forms.ControlStyles.DoubleBuffer | Forms.ControlStyles.AllPaintingInWmPaint | Forms.ControlStyles.UserPaint, true);
		}

		public TreeNodeEx TopNode
		{
			get
			{
				return topNode;
			}
		}

		public override string ToString()
		{
			string s = base.ToString();
			if (Nodes != null)
			{
				s = s + ", Count: " + Nodes.Count;
				if (Nodes.Count > 0)
				{
					s = s + ", [0]: " + Nodes[0].ToString();
				}
			}
			return s;

		}

		public int VisibleCount
		{
			get
			{
				return ClientRectangle.Height / ItemHeight;
			}
		}

		internal int VisibleCountActual
		{
			get
			{
				int height = ClientRectangle.Height;
				if (hScrollBar != null && hScrollBar.Visible)
				{
					height -= hScrollBar.Height;
				}
				return height / ItemHeight;
			}
		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			int nodeFromTop = 0;
			NodeEnumeratorEx nodes = new NodeEnumeratorEx(this.nodes);
			while (nodes.MoveNext())
			{
				if (nodeFromTop == vScrollBar.Value)
				{
					topNode = nodes.currentNode;
					Invalidate();
					return;
				}
				nodeFromTop++;
			}
		}

		// Private enumerator class for all returning all expanded nodes.
		internal class NodeEnumeratorEx : IEnumerator
		{
			// Internal state.
			private TreeNodeExCollection nodes;
			internal TreeNodeEx currentNode;
			private bool first;
			// level of node
			internal int level = 0;

			// Constructor.
			public NodeEnumeratorEx(TreeNodeExCollection nodes)
			{
				this.nodes = nodes;
				Reset();
			}

			// Move to the next element in the enumeration order.
			public bool MoveNext()
			{
				if (first)
				{
					if (nodes.Count == 0)
					{
						return false;
					}
					currentNode = nodes[0];
					first = false;
					return true;
				}
				if (currentNode == null)
				{
					return false;
				}
				if (currentNode.childCount > 0 && currentNode.expanded)
				{
					// If expanded climb into hierarchy.
					currentNode = currentNode.Nodes[0];
					level++;
				}
				else
				{
					TreeNodeEx nextNode = currentNode.NextNode;
					TreeNodeEx nextCurrentNode = currentNode;
					while (nextNode == null)
					{
						// We need to move back up.
						// Are we back at the top?
						if (nextCurrentNode.Parent == null)
						{
							// Leave the nextNode as the previous last node.
							nextNode = currentNode;
							return false;
						}
						else
						{
							nextCurrentNode = nextCurrentNode.Parent;
							if (nextCurrentNode.parent != null)
							{
								nextNode = nextCurrentNode.NextNode;
							}
							level--;
						}
					}
					currentNode = nextNode;
				}
				return true;

			}

			// Reset the enumeration.
			public void Reset()
			{
				first = true;
				currentNode = null;
				level = 0;
			}

			// Get the current value in the enumeration.
			public Object Current
			{
				get
				{
					if (currentNode == null)
					{
						throw new InvalidOperationException();
					}
					else
					{
						return currentNode;
					}
				}
			}
		}
	}

}