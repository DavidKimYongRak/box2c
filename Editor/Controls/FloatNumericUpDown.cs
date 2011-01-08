using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Editor
{
	/// <summary>
	/// A numeric up-down control that is a bit better
	/// </summary>
	[DefaultBindingProperty("Value")]
	[DefaultProperty("Value")]
	[DefaultEvent("ValueChanged")]
	public class FloatNumericUpDown : UserControl
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.floatNumericUpDownButtons1 = new Editor.FloatNumericUpDownButtons();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Location = new System.Drawing.Point(0, 0);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(101, 20);
			this.textBox1.TabIndex = 1;
			// 
			// floatNumericUpDownButtons1
			// 
			this.floatNumericUpDownButtons1.Dock = System.Windows.Forms.DockStyle.Right;
			this.floatNumericUpDownButtons1.Location = new System.Drawing.Point(101, 0);
			this.floatNumericUpDownButtons1.Name = "floatNumericUpDownButtons1";
			this.floatNumericUpDownButtons1.Size = new System.Drawing.Size(16, 20);
			this.floatNumericUpDownButtons1.TabIndex = 0;
			this.floatNumericUpDownButtons1.Text = "floatNumericUpDownButtons1";
			// 
			// FloatNumericUpDown
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.floatNumericUpDownButtons1);
			this.Name = "FloatNumericUpDown";
			this.Size = new System.Drawing.Size(117, 20);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		static FloatNumericUpDownAccelerator[] DefaultAccelerators = new FloatNumericUpDownAccelerator[]
		{
			new FloatNumericUpDownAccelerator(350, 1, 1),
			new FloatNumericUpDownAccelerator(200, 1, 1),
			new FloatNumericUpDownAccelerator(150, 1, 1),
			new FloatNumericUpDownAccelerator(90, 1, 1),
			new FloatNumericUpDownAccelerator(70, 1, 1),
			new FloatNumericUpDownAccelerator(50, 1, 1),
			new FloatNumericUpDownAccelerator(30, 1, 1),
			new FloatNumericUpDownAccelerator(1, -1, 1)
		};

		#region Private Members
		private FloatNumericUpDownButtons floatNumericUpDownButtons1;
		private TextBox textBox1;

		List<FloatNumericUpDownAccelerator> _accelerators = new List<FloatNumericUpDownAccelerator>();
		int _currentAccelerator = 0;
		int _tickNums = 0;
		decimal _value;
		#endregion

		#region Constructors
		public FloatNumericUpDown()
		{
			InitializeComponent();

			floatNumericUpDownButtons1.UpPressed += new EventHandler(floatNumericUpDownButtons1_UpPressed);
			floatNumericUpDownButtons1.DownPressed += new EventHandler(floatNumericUpDownButtons1_DownPressed);

			textBox1.Leave += new EventHandler(textBox1_Leave);
			textBox1.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);
			floatNumericUpDownButtons1.FillTimerData += new EventHandler<FloatNumericUpDownButtons.TimerEventArgs>(floatNumericUpDownButtons1_FillTimerData);
			floatNumericUpDownButtons1.ResetTimerData += new EventHandler(floatNumericUpDownButtons1_ResetTimerData);

			EnableUpperLimit = EnableLowerLimit = true;
			Maximum = 100;
			Minimum = 0;
			Interval = 1;
			Value = 0;
		}
		#endregion

		#region Internal Members
		void floatNumericUpDownButtons1_ResetTimerData(object sender, EventArgs e)
		{
			_currentAccelerator = _tickNums = 0;
		}

		void floatNumericUpDownButtons1_FillTimerData(object sender, FloatNumericUpDownButtons.TimerEventArgs e)
		{
			if (_accelerators.Count != 0)
			{
				FloatNumericUpDownAccelerator accel = _accelerators[_currentAccelerator];

				e.Timer.Interval = accel.Interval;
				
				if (_currentAccelerator == _accelerators.Count - 1)
					return;
				
				if (_tickNums == accel.TickCount)
					_tickNums++;
				else
					_currentAccelerator++;
			}
			else
			{
				FloatNumericUpDownAccelerator accel = DefaultAccelerators[_currentAccelerator];

				e.Timer.Interval = accel.Interval;
				
				if (_currentAccelerator == DefaultAccelerators.Length - 1)
					return;

				if (_tickNums != accel.TickCount)
					_tickNums++;
				else
				{
					_currentAccelerator++;
					_tickNums = 0;
				}
			}
		}

		void CommitTextbox()
		{
			if (string.IsNullOrEmpty(textBox1.Text))
			{
				textBox1.Text = Value.ToString();
				return;
			}

			// commit
			decimal p;
			bool succeed = decimal.TryParse(textBox1.Text, out p);

			if (!succeed)
			{
				MessageBox.Show("Value not valid");
				textBox1.Text = Value.ToString();
			}
			else
				Value = p;
		}

		decimal GetValueModifier()
		{
			if (_accelerators.Count == 0)
				return DefaultAccelerators[_currentAccelerator].ValueModifier;

			return _accelerators[_currentAccelerator].ValueModifier;
		}

		void ModifyValue(decimal newValue)
		{
			if (EnableUpperLimit && (newValue > Maximum))
				newValue = Maximum;
			else if (EnableLowerLimit && (newValue < Minimum))
				newValue = Minimum;

            DecimalValueChangedEventArgs eventArgs = new DecimalValueChangedEventArgs(_value, newValue);

			if (ValueChanged != null)
				ValueChanged(this, eventArgs);

			if (!eventArgs.Cancel)
			{
				_value = newValue;
				textBox1.Text = _value.ToString();
			}
		}
		#endregion

		#region Events/Overrides
		void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
			{
				e.Handled = true;
				CommitTextbox();
			}
			else
			{
				if (!char.IsDigit(e.KeyChar))
				{
					if (e.KeyChar != '.' &&
                        e.KeyChar != (char)Keys.Back 
                        && e.KeyChar != '-')
						e.Handled = true;
				}
			}
		}

		void textBox1_Leave(object sender, EventArgs e)
		{
			CommitTextbox();
		}

		void floatNumericUpDownButtons1_DownPressed(object sender, EventArgs e)
		{
			ModifyValue(Value - (Interval * GetValueModifier()));
		}

		void floatNumericUpDownButtons1_UpPressed(object sender, EventArgs e)
		{
			ModifyValue(Value + (Interval * GetValueModifier()));
		}
		#endregion

		#region Properties
		/// <summary>
		/// A list of accelerators, which define the intervals
		/// and timing for numeric updowns.
		/// </summary>
		[Browsable(false)]
		public IList<FloatNumericUpDownAccelerator> Accelerators
		{
			get { return _accelerators; }
		}

		/// <summary>
		/// Whether or not the Maximum value is enforced.
		/// </summary>
		[Description("Whether or not the Maximum value is enforced.")]
		[DefaultValue(true)]
		[Category("Data")]
		public bool EnableUpperLimit
		{
			get;
			set;
		}

		/// <summary>
		/// Whether or not the Minimum value is enforced.
		/// </summary>
		[Description("Whether or not the Minimum value is enforced.")]
		[DefaultValue(true)]
		[Category("Data")]
		public bool EnableLowerLimit
		{
			get;
			set;
		}

		/// <summary>
		/// The maximum value of this numeric updown when limits are enabled.
		/// </summary>
		[Description("The maximum value of this numeric updown when limits are enabled.")]
		[DefaultValue(typeof(Decimal), "100")]
		[Category("Data")]
		public decimal Maximum
		{
			get;
			set;
		}

		/// <summary>
		/// The minimum value of this numeric updown when limits are enabled.
		/// </summary>
		[Description("The minimum value of this numeric updown when limits are enabled.")]
		[DefaultValue(typeof(Decimal), "0")]
		[Category("Data")]
		public decimal Minimum
		{
			get;
			set;
		}

		/// <summary>
		/// The step that the numeric updown increases and decreases by.
		/// </summary>
		[Description("The step that the numeric updown increases and decreases by.")]
		[DefaultValue(typeof(Decimal), "1")]
		[Category("Data")]
		public decimal Interval
		{
			get;
			set;
		}

		/// <summary>
		/// The current value of this numeric updown.
		/// </summary>
		[Description("The current value of this numeric updown")]
		[DefaultValue(typeof(Decimal), "0")]
		[Category("Data")]
		public decimal Value
		{
			get { return _value; }	
			set { ModifyValue(value); }
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when the Value property changes.
		/// </summary>
		[Description("Raised when the Value property changes.")]
		[Category("Property Changed")]
        public event DecimalValueChangedEventHandler ValueChanged;
		#endregion
	}

	#region Buttons
	[DesignTimeVisible(false)]
	internal class FloatNumericUpDownButtons : Control
	{
		System.Timers.Timer _holdTimer = new System.Timers.Timer();

		public FloatNumericUpDownButtons()
		{
			DoubleBuffered = true;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.UserMouse, true);

			_holdTimer.SynchronizingObject = this;
			_holdTimer.Elapsed += new System.Timers.ElapsedEventHandler(_holdTimer_Elapsed);
		}

		static class Drawing
		{
			public static VisualStyleRenderer UpArrowNormal = new VisualStyleRenderer(VisualStyleElement.Spin.Up.Normal);
			public static VisualStyleRenderer UpArrowHot = new VisualStyleRenderer(VisualStyleElement.Spin.Up.Hot);
			public static VisualStyleRenderer UpArrowDisabled = new VisualStyleRenderer(VisualStyleElement.Spin.Up.Disabled);
			public static VisualStyleRenderer UpArrowPressed = new VisualStyleRenderer(VisualStyleElement.Spin.Up.Pressed);

			public static VisualStyleRenderer DownArrowNormal = new VisualStyleRenderer(VisualStyleElement.Spin.Down.Normal);
			public static VisualStyleRenderer DownArrowHot = new VisualStyleRenderer(VisualStyleElement.Spin.Down.Hot);
			public static VisualStyleRenderer DownArrowDisabled = new VisualStyleRenderer(VisualStyleElement.Spin.Down.Disabled);
			public static VisualStyleRenderer DownArrowPressed = new VisualStyleRenderer(VisualStyleElement.Spin.Down.Pressed);
		}

		protected override Size DefaultSize
		{
			get { return new Size(16, 20); }
		}

		bool _hovering = false, _down = false;
		WhichButton _button = WhichButton.Neither;

		enum WhichButton
		{
			Top,
			Bottom,
			Neither
		}

		VisualStyleRenderer GetStyleElement(WhichButton button)
		{
			switch (button)
			{
			case WhichButton.Bottom:
				if (!Enabled)
					return Drawing.DownArrowDisabled;
				else if (_button != WhichButton.Bottom)
					return Drawing.DownArrowNormal;
				else if (_down)
					return Drawing.DownArrowPressed;
				else if (_hovering)
					return Drawing.DownArrowHot;
				else
					return Drawing.DownArrowNormal;
			case WhichButton.Top:
				if (!Enabled)
					return Drawing.UpArrowDisabled;
				else if (_button != WhichButton.Top)
					return Drawing.UpArrowNormal;
				else if (_down)
					return Drawing.UpArrowPressed;
				else if (_hovering)
					return Drawing.UpArrowHot;
				else
					return Drawing.UpArrowNormal;
			}

			throw new Exception();
		}

		WhichButton WhichButtonIsBeingHovered(Point localPt)
		{
			if (localPt.X < 0 || localPt.X >= Size.Width ||
				localPt.Y < 0 || localPt.Y >= Size.Height)
				return WhichButton.Neither;
			else if (localPt.Y < 10)
				return WhichButton.Top;
			return WhichButton.Bottom;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			CheckStateChanges();

			base.OnMouseLeave(e);
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

			if (button != WhichButton.Neither)
			{
				_hovering = true;
				if (!alreadyInvalidated)
				{
					Invalidate();
					alreadyInvalidated = true;
				}
			}
			else
			{
				_hovering = false;
				if (!alreadyInvalidated)
				{
					Invalidate();
					alreadyInvalidated = true;
				}
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

		public event EventHandler UpPressed;
		public event EventHandler DownPressed;

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

		public class TimerEventArgs : EventArgs
		{
			public System.Timers.Timer Timer
			{
				get;
				set;
			}

			public TimerEventArgs(System.Timers.Timer timer)
			{
				Timer = timer;
			}
		}

		public event EventHandler<TimerEventArgs> FillTimerData;
		public event EventHandler ResetTimerData;

		void _holdTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if ((MouseButtons & System.Windows.Forms.MouseButtons.Left) == 0)
			{
				_holdTimer.Stop();

				if (ResetTimerData != null)
					ResetTimerData(this, EventArgs.Empty);

				return;
			}

			if (FillTimerData != null)
				FillTimerData(this, new TimerEventArgs(_holdTimer));

			var button = WhichButtonIsBeingHovered(PointToClient(Cursor.Position));

			switch (button)
			{
			case WhichButton.Top:
				if (UpPressed != null)
					UpPressed(this, EventArgs.Empty);
				break;
			case WhichButton.Bottom:
				if (DownPressed != null)
					DownPressed(this, EventArgs.Empty);
				break;
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			CheckStateChanges();

			var button = WhichButtonIsBeingHovered(e.Location);

			switch (button)
			{
			case WhichButton.Top:
				if (UpPressed != null)
					UpPressed(this, EventArgs.Empty);
				break;
			case WhichButton.Bottom:
				if (DownPressed != null)
					DownPressed(this, EventArgs.Empty);
				break;
			}

			if (button != WhichButton.Neither)
			{
				if (FillTimerData != null)
					FillTimerData(this, new TimerEventArgs(_holdTimer));
				_holdTimer.Start();
			}

			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			CheckStateChanges();
			base.OnMouseUp(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		
			GetStyleElement(WhichButton.Top).DrawBackground(e.Graphics, new Rectangle(0, 0, Size.Width, Size.Height / 2));
			GetStyleElement(WhichButton.Bottom).DrawBackground(e.Graphics, new Rectangle(0, Size.Height / 2, Size.Width, Size.Height / 2));
		}
	}
	#endregion

	/// <summary>
	/// A generic class for a property changing event args, with support
	/// for cancelling the change as well.
	/// </summary>
	/// <typeparam name="T">The type of value to be changed</typeparam>
	public class ValueChangedEventArgs<T> : EventArgs
	{
		public T OldValue
		{
			get;
			set;
		}

		public T NewValue
		{
			get;
			set;
		}

		public bool Cancel
		{
			get;
			set;
		}

		public ValueChangedEventArgs(T oldValue, T newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}

    public class DecimalValueChangedEventArgs : ValueChangedEventArgs<decimal>
    {
        public DecimalValueChangedEventArgs(decimal oldValue, decimal newValue) :
            base(oldValue, newValue)
        {
        }
    }

	public delegate void DecimalValueChangedEventHandler(object sender, DecimalValueChangedEventArgs e);

	public class FloatNumericUpDownAccelerator
	{
		public int Interval
		{
			get;
			set;
		}

		public decimal ValueModifier
		{
			get;
			set;
		}

		public int TickCount
		{
			get;
			set;
		}

		public FloatNumericUpDownAccelerator(int interval, int tickCount, decimal valueModifier)
		{
			Interval = interval;
			TickCount = tickCount;
			ValueModifier = valueModifier;
		}
	}
}
