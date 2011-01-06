using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Editor
{
	/// <summary>
	/// A button which has two components: the button itself
	/// and a drop-down button.
	/// </summary>
	public class SplitButton : Control
	{
		#region Component Designer generated code
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

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		public SplitButton()
		{
			DoubleBuffered = true;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.UserMouse, true);
			InitializeComponent();
		}

		#region Static/Private Members
		static class Drawing
		{
			public static VisualStyleRenderer SplitButtonLeftNormal = new VisualStyleRenderer(VisualStyleElement.ToolBar.SplitButton.Hot);
			public static VisualStyleRenderer SplitButtonRightNormal = new VisualStyleRenderer(VisualStyleElement.ToolBar.SplitButtonDropDown.Hot);

			public static VisualStyleRenderer SplitButtonLeftPressed = new VisualStyleRenderer(VisualStyleElement.ToolBar.SplitButton.Pressed);
			public static VisualStyleRenderer SplitButtonRightPressed = new VisualStyleRenderer(VisualStyleElement.ToolBar.SplitButtonDropDown.Pressed);

			public static VisualStyleRenderer SplitButtonLeftDisabled = new VisualStyleRenderer(VisualStyleElement.ToolBar.SplitButton.Disabled);
			public static VisualStyleRenderer SplitButtonRightDisabled = new VisualStyleRenderer(VisualStyleElement.ToolBar.SplitButtonDropDown.Disabled);

			public static StringFormat CenterStringFormat = CreateCenterStringFormat();

			static StringFormat CreateCenterStringFormat()
			{
				StringFormat strf = new StringFormat();
				strf.LineAlignment = StringAlignment.Center;
				return strf;
			}
		}

		bool _down = false;
		WhichButton _button = WhichButton.Neither;

		enum WhichButton
		{
			Left,
			Right,
			Neither
		}

		VisualStyleRenderer GetStyleElement(WhichButton button)
		{
			switch (button)
			{
			case WhichButton.Left:
				if (!Enabled)
					return Drawing.SplitButtonLeftDisabled;
				else if (_button != WhichButton.Left)
					return Drawing.SplitButtonLeftNormal;
				else if (_down)
					return Drawing.SplitButtonLeftPressed;
				else
					return Drawing.SplitButtonLeftNormal;
			case WhichButton.Right:
				if (!Enabled)
					return Drawing.SplitButtonRightDisabled;
				else if (_button != WhichButton.Right)
					return Drawing.SplitButtonRightNormal;
				else if (_down)
					return Drawing.SplitButtonRightPressed;
				else
					return Drawing.SplitButtonRightNormal;
			}

			throw new Exception();
		}

		WhichButton WhichButtonIsBeingHovered(Point localPt)
		{
			if (localPt.X < 0 || localPt.X >= Size.Width ||
				localPt.Y < 0 || localPt.Y >= Size.Height)
				return WhichButton.Neither;
			else if (localPt.X >= (Size.Width - 20))
				return WhichButton.Right;
			return WhichButton.Left;
		}

		void CheckStateChanges()
		{
			bool alreadyInvalidated = false;
			WhichButton button = WhichButtonIsBeingHovered(PointToClient(Cursor.Position));

			if (button != _button)
			{
				_button = button;
				Invalidate();
				alreadyInvalidated = true;
			}

			if ((MouseButtons & MouseButtons.Left) != 0)
			{
				_down = true;
				if (!alreadyInvalidated)
				{
					Invalidate();
					alreadyInvalidated = true;
				}
			}
			else if (_down)
			{
				_down = false;
				if (!alreadyInvalidated)
				{
					Invalidate();
					alreadyInvalidated = true;
				}
			}
		}
		#endregion

		#region Event/Overrides
		protected override void OnMouseHover(EventArgs e)
		{
			CheckStateChanges();
			base.OnMouseHover(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			CheckStateChanges();
			base.OnMouseMove(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			CheckStateChanges();
			base.OnMouseDown(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			CheckStateChanges();

			base.OnMouseLeave(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			CheckStateChanges();

			switch (WhichButtonIsBeingHovered(e.Location))
			{
			case WhichButton.Left:
				if (ButtonClick != null)
					ButtonClick(this, e);
				break;
			case WhichButton.Right:
				if (Strip != null)
					Strip.Show(Parent.PointToScreen(new Point(Left + Width - Strip.Width, Bottom)), ToolStripDropDownDirection.BelowRight);
				if (DropDownClick != null)
					DropDownClick(this, e);
				break;
			}

			base.OnMouseUp(e);
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);

			GetStyleElement(WhichButton.Left).DrawBackground(pe.Graphics, new Rectangle(0, 0, Size.Width - 20, Size.Height));
			GetStyleElement(WhichButton.Right).DrawBackground(pe.Graphics, new Rectangle(Size.Width - 20, 0, 20, Size.Height));
			TextRenderer.DrawText(pe.Graphics, Text, Font, new Rectangle(4, 0, Size.Width - 20 - 4, Size.Height), ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
		}

		protected override Size DefaultSize
		{
			get { return new Size(90, 23); }
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when the button portion is clicked.
		/// </summary>
		[Description("Occurs when the button portion is clicked.")]
		[Category("Action")]
		public event MouseEventHandler ButtonClick;

		/// <summary>
		/// Occurs when the dropdown portion is clicked.
		/// </summary>
		[Description("Occurs when the dropdown portion is clicked.")]
		[Category("Action")]
		public event MouseEventHandler DropDownClick;
		#endregion

		#region Public Properties
		/// <summary>
		/// The ContextMenuStrip that opens when the drop-down button is clicked.
		/// </summary>
		[Description("The ContextMenuStrip that opens when the drop-down button is clicked.")]
		[Category("Behavior")]
		public ContextMenuStrip Strip
		{
			get;
			set;
		}
		#endregion
	}
}
