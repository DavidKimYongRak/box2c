/*
 * TreeNode.cs - Implementation of the
 *			"System.Windows.Forms.TreeNode" class.
 *
 * Copyright (C) 2003  Neil Cawse.
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
	using System.Text;
	using System.Globalization;
	using System.Runtime.Serialization;

	public class TreeNodeEx : /*MarshalByRefObject,*/ ICloneable
	, ISerializable
	{
		internal Color backColor;
		internal int childCount = 0;
		internal TreeNodeEx[] children;
		internal bool expanded;
		internal Color foreColor;
		private int imageIndex;
		internal int index;
		internal bool isChecked;
		internal Font nodeFont;
		private TreeNodeExCollection nodes;
		internal TreeNodeEx parent;
		private int selectedImageIndex;
		private object tag;
		internal string text;
		internal TreeViewEx treeView;

		// Must GO:
		internal int markerLineY;

		internal int AddSorted(TreeNodeEx node)
		{
			int pos = 0;
			string text = node.Text;
			if (childCount > 0)
			{
				System.Globalization.CompareInfo compare = System.Windows.Forms.Application.CurrentCulture.CompareInfo;
				// Simple optimization if added in sort order
				if (compare.Compare(children[(childCount - 1)].Text, text) <= 0)
					pos = childCount;
				else
				{
					// Binary Search
					int i = 0;
					int j = childCount;
					while (i < j)
					{
						int mid = (i + j) / 2;
						if (compare.Compare(children[mid].Text, text) <= 0)
							i = mid + 1;
						else
							i = mid;
					}
					pos = i;
				}
			}

			node.SortChildren();
			InsertNodeAt(pos, node);
			if (treeView != null && node == treeView.SelectedNode)
			{
				treeView.SelectedNode = node;
			}
			return pos;
		}

		public Color BackColor
		{
			get
			{
				// TODO:Property Bag
				return backColor;
			}
			set
			{
				// TODO:Property Bag
				if (value == backColor)
				{
					return;
				}
				backColor = value;
				Invalidate();
			}
		}

		public void BeginEdit()
		{
			if (/*treeView.toolkitWindow != null*/true)
			{
				if (!treeView.LabelEdit)
				{
					throw new Exception("SWF_TreeNodeLabelEditFalse");
				}
				if (!treeView.Focused)
				{
					treeView.Focus();
				}
				treeView.BeginEdit(this);
			}
		}

		public RectangleF Bounds
		{
			get
			{
				return treeView.GetNodeBounds(this);
			}
		}

		public bool Checked
		{
			get
			{
				return isChecked;
			}

			set
			{
				if (treeView != null)
				{
					TreeViewExCancelEventArgs e = new TreeViewExCancelEventArgs(this, false, TreeViewExAction.Unknown);
					treeView.OnBeforeCheck(e);
					if (e.Cancel)
					{
						return;
					}
					isChecked = value;
					Invalidate();
					treeView.OnAfterCheck(new TreeViewExEventArgs(this, TreeViewExAction.Unknown));
				}
				else
				{
					isChecked = value;
				}
			}
		}

		internal void Clear()
		{
			children = null;
			childCount = 0;
		}

		public virtual object Clone()
		{
			TreeNodeEx node = new TreeNodeEx(text, imageIndex, selectedImageIndex);
			if (childCount > 0)
			{
				node.children = new TreeNodeEx[childCount];
				for (int i = 0; i < childCount; i++)
					node.Nodes.Add(children[i].Clone() as TreeNodeEx);
			}
			node.Checked = Checked;
			node.Tag = Tag;
			return node;
		}

		public void Collapse()
		{
			CollapseInternal();
			treeView.InvalidateDown(this);
		}

		// Collapse the children recursively but don't redraw.
		private void CollapseInternal()
		{
			bool selected = false;
			// Recursively collapse, if a child was selected, mark to select the parent.
			if (childCount > 0)
			{
				for (int i = 0; i < childCount; i++)
				{
					if (treeView.SelectedNode == children[i])
						selected = true;
					children[i].CollapseInternal();
				}
			}
			// Do the events.
			TreeViewExCancelEventArgs eventArgs = new TreeViewExCancelEventArgs(this, false, TreeViewExAction.Collapse);
			treeView.OnBeforeCollapse(eventArgs);
			if (!eventArgs.Cancel)
			{
				treeView.OnAfterCollapse(new TreeViewExEventArgs(this));
				// The node is now collapsed.
				expanded = false;
			}
			if (selected)
			{
				treeView.SelectedNode = this;
			}
		}

		public void EndEdit(bool cancel)
		{
			if (treeView != null)
			{
				treeView.EndEdit(cancel);
			}
		}

		public void EnsureVisible()
		{
			TreeViewEx.NodeEnumeratorEx nodes;
			int nodeFromTop;
			int nodeNo;
			while (true)
			{
				// Find "this" node number and position from the top control.
				nodeFromTop = -1;
				nodeNo = 0;
				bool nodeFound = false;
				nodes = new TreeViewEx.NodeEnumeratorEx(treeView.nodes);
				while (nodes.MoveNext())
				{
					if (nodes.currentNode == treeView.topNode)
					{
						// We are at the top of the control.
						nodeFromTop = 0;
					}
					if (nodes.currentNode == this)
					{
						if (nodeFromTop < 0)
						{
							treeView.topNode = this;
							treeView.Invalidate();
							return;
						}
						nodeFound = true;
						break;
					}
					if (nodeFromTop >= 0)
					{
						nodeFromTop++;
					}
					nodeNo++;
				}


				if (nodeFound)
				{
					break;
				}
				else
				{
					// Make sure all parents are expanded and see if its now visible.
					TreeNodeEx node = this;
					TreeNodeEx highestNode = node;
					for (; node != null; node = node.Parent)
					{
						node.expanded = true;
						highestNode = node;
					}
					treeView.InvalidateDown(highestNode);
				}
			}

			int visibleNodes = treeView.VisibleCountActual;
			// See if its already visible.
			if (nodeFromTop < visibleNodes)
			{
				return;
			}

			// Set the top node no we want to make this node 1 up from the bottom.
			nodeFromTop = nodeNo - visibleNodes + 1;
			if (nodeFromTop < 0)
			{
				nodeFromTop = 0;
			}

			// Find the node corresponding to this node no.
			nodes.Reset();
			nodeNo = 0;
			while (nodes.MoveNext())
			{
				if (nodeFromTop == nodeNo)
				{
					treeView.topNode = nodes.currentNode;
					treeView.Invalidate();
					break;
				}
				nodeNo++;
			}
		}

		public void Expand()
		{
			if (expanded)
			{
				return;
			}
			TreeNodeEx node = this;
			node.expanded = true;
			if (treeView == null)
			{
				return;
			}

			TreeNodeEx highestNode = node;
			for (; node != null; node = node.Parent)
			{
				node.expanded = true;
				highestNode = node;
			}
			treeView.InvalidateDown(highestNode);
		}

		public void ExpandAll()
		{
			Expand();
			for (int i = 0; i < childCount; i++)
			{
				children[i].ExpandAll();
			}
		}

		public TreeNodeEx FirstNode
		{
			get
			{
				if (childCount == 0)
				{
					return null;
				}
				else
				{
					return children[0];
				}
			}
		}

		public Color ForeColor
		{
			get
			{
				// TODO:Property Bag
				return foreColor;
			}
			set
			{
				// TODO:Property Bag
				if (value == foreColor)
				{
					return;
				}
				foreColor = value;
				Invalidate();
			}
		}

		// Not used in this implementation
		public static TreeNodeEx FromHandle(TreeViewEx tree, IntPtr handle)
		{
			return null;
		}

		public string FullPath
		{
			get
			{
				StringBuilder s = new StringBuilder();
				GetFullPath(s, TreeView.PathSeparator);
				return s.ToString();
			}
		}

		private void GetFullPath(StringBuilder path, string pathSeparator)
		{
			if (parent == null)
				return;
			parent.GetFullPath(path, pathSeparator);
			if (parent.parent != null)
			{
				path.Append(pathSeparator);
			}
			path.Append(text);
		}

		public int GetNodeCount(bool includeSubTrees)
		{
			int count = childCount;
			if (includeSubTrees)
			{
				for (int i = 0; i < childCount; i++)
				{
					count += children[i].GetNodeCount(true);
				}
			}
			return count;
		}

		void System.Runtime.Serialization.ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			si.AddValue("Text", text);
			si.AddValue("IsChecked", isChecked);
			si.AddValue("ImageIndex", imageIndex);
			si.AddValue("SelectedImageIndex", selectedImageIndex);
			si.AddValue("ChildCount", childCount);
			if (childCount > 0)
			{
				for (int i = 0; i < childCount; i++)
				{
					si.AddValue("children"+ i, children[i], typeof(TreeNodeEx));
				}
			}
			if (tag != null && tag.GetType().IsSerializable)
			{
				si.AddValue("UserData", tag, tag.GetType());
			}
		}

		// This is not used in this implementation.
		public IntPtr Handle
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		public int ImageIndex
		{
			get
			{
				return imageIndex;
			}
			set
			{
				if (imageIndex == value)
					return;
				imageIndex = value;
				Invalidate();
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
		}

		internal void InsertNodeAt(int index, TreeNodeEx node)
		{
			SizeChildrenArray();
			node.parent = this;
			node.index = index;
			node.treeView = treeView;
			for (int i = childCount; i > index; i--)
			{
				TreeNodeEx node1 = children[i - 1];
				node1.index = i;
				children[i] = node1;
			}
			childCount++;
			children[index] = node;
			if (treeView != null)
			{
				if (childCount == 1 && IsVisible)
				{
					Invalidate();
				}
				else if (index != 0 && expanded && children[index - 1].IsVisible)
				{
					treeView.InvalidateDown(node);
				}
			}
		}

		internal void Invalidate()
		{
			if (treeView == null || !treeView.IsHandleCreated)
			{
				return;
			}
			RectangleF bounds = Bounds;
			if (bounds != Rectangle.Empty)
			{
				// Include the focus rectangle.
				bounds = new Rectangle(0, (int)bounds.Y - 1, (int)bounds.Right + 2, (int)bounds.Height + 2);
				treeView.Invalidate(TreeViewEx.RectIFromRectF(bounds));
			}
		}

		public bool IsEditing
		{
			get
			{
				if (treeView == null)
				{
					return false;
				}
				else
				{
					return (treeView.editNode == this);
				}
			}
		}

		public bool IsExpanded
		{
			get
			{
				return expanded;
			}
		}

		public bool IsSelected
		{
			get
			{
				if (treeView == null)
					return false;
				return treeView.SelectedNode == this;
			}
		}

		public bool IsVisible
		{
			get
			{
				if (treeView == null || !treeView.Visible)
					return false;
				RectangleF bounds = Bounds;
				if (bounds == Rectangle.Empty)
					return false;
				return (treeView.ClientRectangle.IntersectsWith(TreeViewEx.RectIFromRectF(bounds)));
			}
		}

		public TreeNodeEx LastNode
		{
			get
			{
				if (childCount == 0)
					return null;
				else
					return children[childCount - 1];
			}
		}

		public TreeNodeEx NextNode
		{
			get
			{
				if (index < parent.Nodes.Count - 1)
					return parent.Nodes[index + 1];
				else
					return null;
			}
		}

		public TreeNodeEx NextVisibleNode
		{
			get
			{
				bool pastThis = false;
				TreeViewEx.NodeEnumeratorEx nodes = new TreeViewEx.NodeEnumeratorEx(treeView.nodes);
				while (nodes.MoveNext())
				{
					if (pastThis)
					{
						if (nodes.currentNode.parent.expanded)
						{
							return nodes.currentNode;
						}
					}
					else if (nodes.currentNode == this)
					{
						pastThis = true;
					}
				}
				return null;
			}
		}

		public Font NodeFont
		{
			get
			{
				// TODO:Property Bag
				return nodeFont;
			}
			set
			{
				// TODO:Property Bag
				if (value == nodeFont)
				{
					return;
				}
				nodeFont = value;
				Invalidate();
			}
		}

		public TreeNodeExCollection Nodes
		{
			get
			{
				if (nodes == null)
				{
					nodes = new TreeNodeExCollection(this);
				}
				return nodes;
			}
		}

		public TreeNodeEx Parent
		{
			get
			{
				if (treeView != null && parent == treeView.root)
				{
					return null;
				}
				return parent;
			}
		}

		public TreeNodeEx PrevNode
		{
			get
			{
				if (index > 0 && index <= parent.Nodes.Count)
				{
					return parent.Nodes[index - 1];
				}
				else
				{
					return null;
				}
			}
		}

		public TreeNodeEx PrevVisibleNode
		{
			get
			{
				TreeNodeEx visibleNode = null;
				TreeViewEx.NodeEnumeratorEx nodes = new TreeViewEx.NodeEnumeratorEx(treeView.nodes);
				while (nodes.MoveNext())
				{
					if (nodes.currentNode == this)
					{
						break;
					}
					else if (nodes.currentNode.parent.expanded)
					{
						visibleNode = nodes.currentNode;
					}
				}
				return visibleNode;
			}
		}

		public void Remove()
		{
			// If we need to, redraw the parent.
			if (treeView != null)
			{
				treeView.InvalidateDown(this);
			}

			// When removing a node, we need to see if topNode is it or its children.
			// If so we find a new topNode.
			TreeNodeEx node = treeView.topNode;
			while (node != null)
			{
				if (node == this)
				{
					treeView.topNode = PrevVisibleNode;
					break;
				}
				node = node.parent;
			}

			RemoveRecurse();

		}

		private void RemoveRecurse()
		{
			// Remove children.
			// FIXME: why?
			/*for (int i = 0; i < childCount; i++)
			{
				children[i].RemoveRecurse();
			}*/
			// Remove out of parent's children.
			for (int i = index; i < parent.childCount - 1; i++)
			{
				TreeNodeEx node = parent.children[i + 1];
				node.index = i;
				parent.children[i] = node;
			}
			parent.childCount--;
			parent = null;
			treeView = null;
		}

		public int SelectedImageIndex
		{
			get
			{
				return selectedImageIndex;
			}
			set
			{
				selectedImageIndex = value;
				Invalidate();
			}
		}

		internal void SizeChildrenArray()
		{
			if (children == null)
			{
				children = new TreeNodeEx[10];
			}
			else if (childCount == children.Length)
			{
				TreeNodeEx[] copy = new TreeNodeEx[childCount * 2];
				Array.Copy(children, 0, copy, 0, childCount);
				children = copy;
			}
		}

		private void SortChildren()
		{
			if (childCount > 0)
			{
				TreeNodeEx[] sort = new TreeNodeEx[childCount];
				CompareInfo compare = System.Windows.Forms.Application.CurrentCulture.CompareInfo;
				for (int i = 0; i < childCount; i++)
				{

					int pos = -1;
					for (int j = 0; j < childCount; j++)
					{
						if (children[j] != null)
						{
							if (pos == -1 || compare.Compare(children[j].Text, children[pos].Text) < 0)
							{
								pos = j;
							}
						}
					}
					sort[i] = children[pos];
					children[pos] = null;
					sort[i].index = i;
					sort[i].SortChildren();
				}
				children = sort;
			}
		}

		public object Tag
		{
			get
			{
				return tag;
			}
			set
			{
				tag = value;
			}
		}

		public string Text
		{
			get
			{
				if (text == null)
					return String.Empty;
				else
					return text;
			}
			set
			{
				text = value;
				Invalidate();
			}
		}

		public void Toggle()
		{
			if (expanded)
			{
				Collapse();
			}
			else
			{
				Expand();
			}
		}

		public override string ToString()
		{
			String s = base.ToString();
			if (nodes != null)
			{
				s += ", Nodes.Count: " + childCount;
				if (childCount > 0)
				{
					s += ", Nodes[0]: " + nodes[0];
				}
			}
			return s;
		}

		public TreeNodeEx()
		{
			imageIndex = -1;
			selectedImageIndex = -1;
			backColor = Color.Empty;
		}

		internal TreeNodeEx(TreeViewEx treeView)
			: this()
		{
			this.treeView = treeView;
		}

		public TreeNodeEx(string text)
			: this()
		{
			this.text = text;
		}

		public TreeNodeEx(string text, TreeNodeEx[] children)
			: this()
		{
			this.text = text;
			Nodes.AddRange(children);
		}

		public TreeNodeEx(string text, int imageIndex, int selectedImageIndex)
			: this()
		{
			this.text = text;
			this.imageIndex = imageIndex;
			this.selectedImageIndex = selectedImageIndex;
		}

		public TreeNodeEx(string text, int imageIndex, int selectedImageIndex, TreeNodeEx[] children)
			: this()
		{
			this.text = text;
			this.imageIndex = imageIndex;
			this.selectedImageIndex = selectedImageIndex;
			Nodes.AddRange(children);
		}

		public TreeViewEx TreeView
		{
			get
			{
				return treeView;
			}
		}

		public virtual bool CanDragDrop()
		{
			return true;
		}

		public virtual bool CanDropOn(TreeNodeEx nodeToDrop)
		{
			return true;
		}

		public virtual bool CanDropUnder(TreeNodeEx nodeToDrop)
		{
			int count = (Parent != null) ? Parent.Nodes.Count : TreeView.Nodes.Count;

			if (count > 0 && IsExpanded)
				return true;

			return Index == count - 1;
		}

		public virtual bool CanDropAbove(TreeNodeEx nodeToDrop)
		{
			return true;
		}

		public virtual bool CanRename()
		{
			return true;
		}

		public event EventHandler Renamed;

		public virtual void OnRenamed()
		{
			if (Renamed != null)
				Renamed(this, EventArgs.Empty);
		}

		public event TreeNodeExMovedEventHandler NodeMoved;
		public event TreeNodeExMovedEventHandler NodeDropped;

		public virtual void OnNodeMoved(TreeNodeExMovedEventArgs args)
		{
			if (NodeMoved != null)
				NodeMoved(this, args);
		}

		public virtual void OnNodeDropped(TreeNodeExMovedEventArgs args)
		{
			if (NodeDropped != null)
				NodeDropped(this, args);
		}
	}; // class TreeNode

	public struct TreeNodeExParent
	{
		public TreeNodeEx Node
		{
			get;
			set;
		}

		public TreeViewEx TreeView
		{
			get;
			set;
		}

		public bool IsNode
		{
			get;
			set;
		}

		public TreeNodeExParent(TreeNodeEx node) :
			this()
		{
			IsNode = true;
			Node = node;
			TreeView = null;
		}

		public TreeNodeExParent(TreeViewEx view) :
			this()
		{
			IsNode = false;
			Node = null;
			TreeView = view;
		}
	}

	public class TreeNodeExMovedEventArgs : EventArgs
	{
		public TreeNodeExParent OldParent
		{
			get;
			set;
		}

		public TreeNodeExParent NewParent
		{
			get;
			set;
		}

		public TreeNodeEx Node
		{
			get;
			set;
		}

		public TreeNodeExMovedEventArgs(TreeNodeEx node, TreeNodeExParent oldParent, TreeNodeExParent newParent)
		{
			Node = node;
			OldParent = oldParent;
			NewParent = newParent;
		}
	}

	public delegate void TreeNodeExMovedEventHandler (object sender, TreeNodeExMovedEventArgs e);

}; // namespace System.Windows.Forms