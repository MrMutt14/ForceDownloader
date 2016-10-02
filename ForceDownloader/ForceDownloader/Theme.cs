using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

/* AirFox Theme.
' Made by AeroRev9.
' Initial Release.
' Converted to C# by UniveX'
' */
public static class Helpers
{

    public enum MouseState : byte
    {
        None = 0,
        Over = 1,
        Down = 2
    }

    public enum RoundingStyle : byte
    {
        All = 0,
        Top = 1,
        Bottom = 2,
        Left = 3,
        Right = 4,
        TopRight = 5,
        BottomRight = 6
    }

    public static void CenterString(Graphics G, string T, Font F, Color C, Rectangle R)
    {
        SizeF TS = G.MeasureString(T, F);

        using (SolidBrush B = new SolidBrush(C))
        {
            G.DrawString(T, F, B, new Point((int)(R.X + R.Width / 2 - (TS.Width / 2)), (int)(R.Y + R.Height / 2 - (TS.Height / 2))));
        }
    }

    public static Color ColorFromHex(string Hex)
    {
        return Color.FromArgb((int)long.Parse(string.Format("FFFFFFFFFF{0}", Hex.Substring(1)), System.Globalization.NumberStyles.HexNumber));
    }

    public static Rectangle FullRectangle(Size S, bool Subtract)
    {

        if (Subtract)
        {
            return new Rectangle(0, 0, S.Width - 1, S.Height - 1);
        }
        else
        {
            return new Rectangle(0, 0, S.Width, S.Height);
        }

    }

    public static GraphicsPath RoundRect(Rectangle Rect, int Rounding, RoundingStyle Style = RoundingStyle.All)
    {

        GraphicsPath GP = new GraphicsPath();
        int AW = Rounding * 2;

        GP.StartFigure();

        if (Rounding == 0)
        {
            GP.AddRectangle(Rect);
            GP.CloseAllFigures();
            return GP;
        }

        switch (Style)
        {
            case RoundingStyle.All:
                GP.AddArc(new Rectangle(Rect.X, Rect.Y, AW, AW), -180, 90);
                GP.AddArc(new Rectangle(Rect.Width - AW + Rect.X, Rect.Y, AW, AW), -90, 90);
                GP.AddArc(new Rectangle(Rect.Width - AW + Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 0, 90);
                GP.AddArc(new Rectangle(Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 90, 90);
                break;
            case RoundingStyle.Top:
                GP.AddArc(new Rectangle(Rect.X, Rect.Y, AW, AW), -180, 90);
                GP.AddArc(new Rectangle(Rect.Width - AW + Rect.X, Rect.Y, AW, AW), -90, 90);
                GP.AddLine(new Point(Rect.X + Rect.Width, Rect.Y + Rect.Height), new Point(Rect.X, Rect.Y + Rect.Height));
                break;
            case RoundingStyle.Bottom:
                GP.AddLine(new Point(Rect.X, Rect.Y), new Point(Rect.X + Rect.Width, Rect.Y));
                GP.AddArc(new Rectangle(Rect.Width - AW + Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 0, 90);
                GP.AddArc(new Rectangle(Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 90, 90);
                break;
            case RoundingStyle.Left:
                GP.AddArc(new Rectangle(Rect.X, Rect.Y, AW, AW), -180, 90);
                GP.AddLine(new Point(Rect.X + Rect.Width, Rect.Y), new Point(Rect.X + Rect.Width, Rect.Y + Rect.Height));
                GP.AddArc(new Rectangle(Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 90, 90);
                break;
            case RoundingStyle.Right:
                GP.AddLine(new Point(Rect.X, Rect.Y + Rect.Height), new Point(Rect.X, Rect.Y));
                GP.AddArc(new Rectangle(Rect.Width - AW + Rect.X, Rect.Y, AW, AW), -90, 90);
                GP.AddArc(new Rectangle(Rect.Width - AW + Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 0, 90);
                break;
            case RoundingStyle.TopRight:
                GP.AddLine(new Point(Rect.X, Rect.Y + 1), new Point(Rect.X, Rect.Y));
                GP.AddArc(new Rectangle(Rect.Width - AW + Rect.X, Rect.Y, AW, AW), -90, 90);
                GP.AddLine(new Point(Rect.X + Rect.Width, Rect.Y + Rect.Height - 1), new Point(Rect.X + Rect.Width, Rect.Y + Rect.Height));
                GP.AddLine(new Point(Rect.X + 1, Rect.Y + Rect.Height), new Point(Rect.X, Rect.Y + Rect.Height));
                break;
            case RoundingStyle.BottomRight:
                GP.AddLine(new Point(Rect.X, Rect.Y + 1), new Point(Rect.X, Rect.Y));
                GP.AddLine(new Point(Rect.X + Rect.Width - 1, Rect.Y), new Point(Rect.X + Rect.Width, Rect.Y));
                GP.AddArc(new Rectangle(Rect.Width - AW + Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 0, 90);
                GP.AddLine(new Point(Rect.X + 1, Rect.Y + Rect.Height), new Point(Rect.X, Rect.Y + Rect.Height));
                break;
        }

        GP.CloseAllFigures();

        return GP;

    }

}

namespace Base
{
    public class AirFoxNumericUpDown : Control
    {

        private Graphics G;

        private bool IsEnabled;
        private int _Value;
        public int Value
        {
            get { return _Value; }
            set
            {
                if (value > _Max)
                {
                    _Value = _Max;
                    return;
                }
                if (value < Min)
                {
                    _Value = _Min;
                    return;
                }
                _Value = value;
                Invalidate();
            }
        }

        private int _Max = 100;
        public int Max
        {
            get { return _Max; }
            set
            {
                if (value == _Min || value < _Min)
                {
                    _Max = _Min + 1;
                    Invalidate();
                }
                else
                {
                    _Max = value;
                }
            }
        }

        private int _Min = 0;
        public int Min
        {
            get { return _Min; }
            set
            {
                if (value > _Max || value == _Max)
                {
                    _Min = _Max - 1;
                    Invalidate();
                }
                else
                {
                    _Min = value;
                }
            }
        }

        public new bool Enabled
        {
            get { return EnabledCalc; }
            set
            {
                IsEnabled = value;
                Invalidate();
            }
        }

        [DisplayName("Enabled")]
        public bool EnabledCalc
        {
            get { return IsEnabled; }
            set
            {
                Enabled = value;
                Invalidate();
            }
        }

        public AirFoxNumericUpDown()
        {
            Enabled = true;
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;


            if (Enabled)
            {
                using (Pen Border = new Pen(Helpers.ColorFromHex("#C8C8C8")))
                {
                    G.DrawPath(Border, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));
                    G.DrawPath(Border, Helpers.RoundRect(new Rectangle(Width - 20, 4, 15, 18), 2));
                }

                using (SolidBrush TextColor = new SolidBrush(Helpers.ColorFromHex("#424E5A")))
                {
                    using (Font TextFont = new Font("Segoe UI", 10))
                    {
                        Helpers.CenterString(G, Value.ToString(), TextFont, TextColor.Color, new Rectangle(-10, 0, Width, Height));
                    }
                }

                using (SolidBrush SignColor = new SolidBrush(Helpers.ColorFromHex("#56626E")))
                {
                    using (Font SignFont = new Font("Marlett", 10))
                    {
                        G.DrawString("t", SignFont, SignColor, new Point(Width - 20, 4));
                        G.DrawString("u", SignFont, SignColor, new Point(Width - 20, 10));
                    }
                }


            }
            else
            {
                using (Pen Border = new Pen(Helpers.ColorFromHex("#E6E6E6")))
                {
                    G.DrawPath(Border, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));
                    G.DrawPath(Border, Helpers.RoundRect(new Rectangle(Width - 20, 4, 15, 18), 2));
                }

                using (SolidBrush TextColor = new SolidBrush(Helpers.ColorFromHex("#A6B2BE")))
                {
                    using (Font TextFont = new Font("Segoe UI", 10))
                    {
                        Helpers.CenterString(G, Value.ToString(), TextFont, TextColor.Color, new Rectangle(-10, 0, Width, Height));
                    }
                }

                using (SolidBrush SignColor = new SolidBrush(Helpers.ColorFromHex("#BAC6D2")))
                {
                    using (Font SignFont = new Font("Marlett", 10))
                    {
                        G.DrawString("t", SignFont, SignColor, new Point(Width - 20, 4));
                        G.DrawString("u", SignFont, SignColor, new Point(Width - 20, 10));
                    }
                }

            }

            base.OnPaint(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);


            if (Enabled)
            {
                if (e.X > Width - 20 & e.Y < 10)
                {
                    Value += 1;
                }
                else if (e.X > Width - 20 & e.Y > 10)
                {
                    Value -= 1;
                }

            }

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);


            if (Enabled)
            {
                if (e.X > Width - 20 & e.Y < 10)
                {
                    Cursor = Cursors.Hand;
                }
                else if (e.X > Width - 20 & e.Y > 10)
                {
                    Cursor = Cursors.Hand;
                }
                else
                {
                    Cursor = Cursors.Default;
                }

            }

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(Width, 27);
        }

    }
    public abstract class LeftTabControl : TabControl
    {

        public Rectangle BaseRect;
        public Rectangle OverRect;

        public int ItemWidth = 180;

        private int _OverIndex = -1;
        public bool Hovering
        {
            get { return !(OverIndex == -1); }
        }

        public int OverIndex
        {
            get { return _OverIndex; }
            set
            {
                _OverIndex = value;

                if (!(_OverIndex == -1))
                {
                    OverRect = new Rectangle(GetTabRect(OverIndex).X, GetTabRect(OverIndex).Y, GetTabRect(OverIndex).Width, GetTabRect(OverIndex).Height);
                }

                Invalidate();
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            e.Control.BackColor = Color.White;
            e.Control.ForeColor = Helpers.ColorFromHex("#424E5A");
            e.Control.Font = new Font("Segoe UI", 10);
        }

        public LeftTabControl()
        {
            DoubleBuffered = true;
            Alignment = TabAlignment.Left;
            ItemSize = new Size(40, ItemWidth);
            SizeMode = TabSizeMode.Fixed;
            Font = new Font("Segoe UI", 10);

        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            for (int I = 0; I <= TabPages.Count - 1; I++)
            {
                if (GetTabRect(I).Contains(e.Location) & !(SelectedIndex == I))
                {
                    OverIndex = I;
                    break; // TODO: might not be correct. Was : Exit For
                }
                else
                {
                    OverIndex = -1;
                }
            }

            base.OnMouseMove(e);

        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            OverIndex = -1;
        }

    }

    public abstract class TopTabControl : TabControl
    {

        public Rectangle BaseRect;
        public Rectangle OverRect;

        public int ItemWidth = 180;

        private int _OverIndex = -1;
        public bool Hovering
        {
            get { return !(OverIndex == -1); }
        }

        public int OverIndex
        {
            get { return _OverIndex; }
            set
            {
                _OverIndex = value;

                if (!(_OverIndex == -1))
                {
                    OverRect = new Rectangle(GetTabRect(OverIndex).X, GetTabRect(OverIndex).Y, GetTabRect(OverIndex).Width, GetTabRect(OverIndex).Height);
                }

                Invalidate();
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            e.Control.BackColor = Color.White;
            e.Control.ForeColor = Helpers.ColorFromHex("#424E5A");
            e.Control.Font = new Font("Segoe UI", 10);
        }

        public TopTabControl()
        {
            DoubleBuffered = true;
            Alignment = TabAlignment.Top;
            ItemSize = new Size(ItemWidth, 40);
            SizeMode = TabSizeMode.Fixed;
            Font = new Font("Segoe UI", 10);
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            for (int I = 0; I <= TabPages.Count - 1; I++)
            {
                if (GetTabRect(I).Contains(e.Location) & !(SelectedIndex == I))
                {
                    OverIndex = I;
                    break; // TODO: might not be correct. Was : Exit For
                }
                else
                {
                    OverIndex = -1;
                }
            }

            base.OnMouseMove(e);

        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            OverIndex = -1;
        }

    }

    public abstract class CheckControl : Control
    {

        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender, EventArgs e);

        public Helpers.MouseState State;
        private bool IsEnabled;

        private bool IsChecked;
        public new bool Enabled
        {
            get { return EnabledCalc; }
            set
            {
                IsEnabled = value;

                if (Enabled)
                {
                    Cursor = Cursors.Hand;
                }
                else
                {
                    Cursor = Cursors.Default;
                }

                Invalidate();
            }
        }

        [DisplayName("Enabled")]
        public bool EnabledCalc
        {
            get { return IsEnabled; }
            set
            {
                Enabled = value;
                Invalidate();
            }
        }

        public bool Checked
        {
            get { return IsChecked; }
            set
            {
                IsChecked = value;
                Invalidate();
            }
        }

        public CheckControl()
        {
            Enabled = true;
            DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            State = Helpers.MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            State = Helpers.MouseState.None;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            State = Helpers.MouseState.Over;
            Invalidate();

            if (Enabled)
            {
                Checked = !Checked;
                if (CheckedChanged != null)
                {
                    CheckedChanged(this, e);
                }
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            State = Helpers.MouseState.Down;
            Invalidate();
        }

    }

    public abstract class BaseRadioButton : Control
    {

        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender, EventArgs e);

        public Helpers.MouseState State;
        private bool IsEnabled;

        private bool IsChecked;
        public new bool Enabled
        {
            get { return EnabledCalc; }
            set
            {
                IsEnabled = value;

                if (Enabled)
                {
                    Cursor = Cursors.Hand;
                }
                else
                {
                    Cursor = Cursors.Default;
                }

                Invalidate();
            }
        }

        [DisplayName("Enabled")]
        public bool EnabledCalc
        {
            get { return IsEnabled; }
            set
            {
                Enabled = value;
                Invalidate();
            }
        }

        public bool Checked
        {
            get { return IsChecked; }
            set
            {
                IsChecked = value;
                Invalidate();
            }
        }

        public BaseRadioButton()
        {
            Enabled = true;
            DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            State = Helpers.MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            State = Helpers.MouseState.None;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            State = Helpers.MouseState.Over;
            Invalidate();


            if (Enabled)
            {

                if (!Checked)
                {
                    foreach (Control C in Parent.Controls)
                    {
                        if (C is BaseRadioButton)
                        {
                            ((BaseRadioButton)C).Checked = false;
                        }
                    }

                }

                Checked = true;
                if (CheckedChanged != null)
                {
                    CheckedChanged(this, e);
                }
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            State = Helpers.MouseState.Down;
            Invalidate();
        }

    }

    public abstract class ButtonBase : Control
    {

        public new event ClickEventHandler Click;
        public new delegate void ClickEventHandler(object sender, EventArgs e);

        public Helpers.MouseState State;

        private bool IsEnabled;
        public new bool Enabled
        {
            get { return EnabledCalc; }
            set
            {
                IsEnabled = value;

                if (Enabled)
                {
                    Cursor = Cursors.Hand;
                }
                else
                {
                    Cursor = Cursors.Default;
                }

                Invalidate();
            }
        }

        [DisplayName("Enabled")]
        public bool EnabledCalc
        {
            get { return IsEnabled; }
            set
            {
                Enabled = value;
                Invalidate();
            }
        }

        public ButtonBase()
        {
            DoubleBuffered = true;
            Enabled = true;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            State = Helpers.MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            State = Helpers.MouseState.None;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            State = Helpers.MouseState.Over;
            Invalidate();

            if (Enabled)
            {
                if (Click != null)
                {
                    Click(this, e);
                }
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            State = Helpers.MouseState.Down;
            Invalidate();
        }

    }

}

namespace Theme
{

    public class AirFoxMainTabControl : Base.LeftTabControl
    {


        private Graphics G;
        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            G.Clear(Helpers.ColorFromHex("#424F5A"));


            for (int I = 0; I <= TabPages.Count - 1; I++)
            {
                BaseRect = GetTabRect(I);


                if (SelectedIndex == I)
                {
                    using (SolidBrush SelectionBrush = new SolidBrush(Helpers.ColorFromHex("#343F48")))
                    {
                        using (SolidBrush TextBrush = new SolidBrush(Helpers.ColorFromHex("#F2F2F2")))
                        {
                            using (Font TextFont = new Font("Segoe UI", 10))
                            {
                                G.FillRectangle(SelectionBrush, new Rectangle(BaseRect.X - 6, BaseRect.Y + 1, BaseRect.Width + 9, BaseRect.Height - 1));
                                G.DrawString(TabPages[I].Text, TextFont, TextBrush, new Point(BaseRect.X + 55, BaseRect.Y + 11));
                            }
                        }
                    }

                    G.SmoothingMode = SmoothingMode.Default;

                    using (SolidBrush LeftBarBrush = new SolidBrush(Helpers.ColorFromHex("#FF9500")))
                    {
                        G.FillRectangle(LeftBarBrush, new Rectangle(BaseRect.X - 2, BaseRect.Y + 2, 4, BaseRect.Height - 2));
                    }


                }
                else
                {
                    using (SolidBrush TextBrush = new SolidBrush(Helpers.ColorFromHex("#C1C1C1")))
                    {
                        using (Font TextFont = new Font("Segoe UI", 10))
                        {
                            G.DrawString(TabPages[I].Text, TextFont, TextBrush, new Point(BaseRect.X + 55, BaseRect.Y + 11));
                        }
                    }

                }

                if ((ImageList != null))
                {
                    if (!(TabPages[I].ImageIndex < 0))
                    {
                        G.DrawImage(ImageList.Images[TabPages[I].ImageIndex], new Rectangle(BaseRect.X + 23, BaseRect.Y + ((BaseRect.Height / 2) - 8), 16, 16));
                    }
                }


                if (Hovering)
                {
                    using (SolidBrush HoverBrush = new SolidBrush(Helpers.ColorFromHex("#5E6972")))
                    {
                        using (SolidBrush TextBrush = new SolidBrush(Helpers.ColorFromHex("#C1C1C1")))
                        {
                            using (Font TextFont = new Font("Segoe UI", 10))
                            {
                                G.FillRectangle(HoverBrush, new Rectangle(OverRect.X - 6, OverRect.Y + 1, OverRect.Width + 9, OverRect.Height - 1));
                                G.DrawString(TabPages[OverIndex].Text, TextFont, TextBrush, new Point(OverRect.X + 55, OverRect.Y + 11));
                            }
                        }
                    }

                    if ((ImageList != null))
                    {
                        if (!(TabPages[OverIndex].ImageIndex < 0))
                        {
                            G.DrawImage(ImageList.Images[TabPages[OverIndex].ImageIndex], new Rectangle(OverRect.X + 23, OverRect.Y + ((OverRect.Height / 2) - 8), 16, 16));
                        }
                    }

                }

            }

            base.OnPaint(e);

        }

    }

    public class AirFoxSubTabControl : Base.TopTabControl
    {


        private Graphics G;
        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            G.Clear(Parent.BackColor);


            for (int I = 0; I <= TabPages.Count - 1; I++)
            {
                BaseRect = GetTabRect(I);


                if (SelectedIndex == I)
                {
                    using (SolidBrush SelectionBrush = new SolidBrush(Helpers.ColorFromHex("#F0F0F0")))
                    {
                        using (SolidBrush TextBrush = new SolidBrush(Helpers.ColorFromHex("#424F5A")))
                        {
                            using (Font TextFont = new Font("Segoe UI", 10))
                            {
                                G.FillRectangle(SelectionBrush, new Rectangle(BaseRect.X + 2, BaseRect.Y - 2, BaseRect.Width, BaseRect.Height + 1));
                                Helpers.CenterString(G, TabPages[I].Text, TextFont, TextBrush.Color, new Rectangle(BaseRect.X + 4, BaseRect.Y, BaseRect.Width, BaseRect.Height));
                            }
                        }
                    }

                    G.SmoothingMode = SmoothingMode.Default;

                    using (SolidBrush BottomBarBrush = new SolidBrush(Helpers.ColorFromHex("#FF9500")))
                    {
                        G.FillRectangle(BottomBarBrush, new Rectangle(BaseRect.X + 3, BaseRect.Y + 36, BaseRect.Width - 1, 4));
                    }


                }
                else
                {
                    using (SolidBrush TextBrush = new SolidBrush(Helpers.ColorFromHex("#424F5A")))
                    {
                        using (Font TextFont = new Font("Segoe UI", 10))
                        {
                            Helpers.CenterString(G, TabPages[I].Text, TextFont, TextBrush.Color, new Rectangle(BaseRect.X + 4, BaseRect.Y, BaseRect.Width, BaseRect.Height));
                        }
                    }

                }


                if (Hovering)
                {
                    using (SolidBrush HoverBrush = new SolidBrush(Helpers.ColorFromHex("#F0F0F0")))
                    {
                        using (SolidBrush TextBrush = new SolidBrush(Helpers.ColorFromHex("#C1C1C1")))
                        {
                            using (Font TextFont = new Font("Segoe UI", 10))
                            {
                                G.FillRectangle(HoverBrush, new Rectangle(OverRect.X + 2, OverRect.Y - 2, OverRect.Width, OverRect.Height + 1));
                                Helpers.CenterString(G, TabPages[OverIndex].Text, TextFont, TextBrush.Color, new Rectangle(OverRect.X + 4, OverRect.Y, OverRect.Width, OverRect.Height));
                            }
                        }
                    }

                }

            }

            using (Pen BottomLine = new Pen(Helpers.ColorFromHex("#C8C8C8"), 1))
            {
                G.DrawLine(BottomLine, 5, 42, Width - 6, 42);
            }

            base.OnPaint(e);

        }

    }

    [DefaultEvent("CheckedChanged")]
    public class AirFoxCheckBox : Base.CheckControl
    {

        private Graphics G;
        private string B64E = "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAMAAABhq6zVAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAABfVBMVEUAAAArntgsm9srm9sgn98gnuE1m9gsndoqnNkvmdYpnNkqm9gvl9YtnNksntosnNorm9o2mtoumtg/mb8rmtotm9snn9s/kMIqnNosm9ksoNssndksndklndwyl9gqn9krm9ornNksm9ksm9srm9srm9ssnNornNkgnd4xm9ksnNosnNorm9orm9ssnNosnNoalf8rnNkrm9ksnNosm9opnNkpnNkrnNkqm9ksnNosnNktmdcqm9ssnNosnNornNssnNksnNornNornNosm9srnNodnNYsmtosnNornNovmtkqnNornNosm9osnNoqmtorm9opntYim9Msm9osnNorm9osnNorm9oum9oAr9U1mNYqndosnNosnNosnNoxmNEont4qndoqnNosnNosnNosm9krm9kpnNksndornNkrnNksntsulc8pnt0snNosnNosnNosnNosnNosnNosnNosnNosnNosnNosnNosnNosnNosnNosnNornNn///9BLcc1AAAAbnRSTlMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAWz8agABKOiVAAOt7jYBAgEABFr/fAABAhjRyg0VZhQDi/tMApWzBUD6kwJtgMTcHgIABKTkuvtkBAAAGtb7vwEBAkPz9zABAAJwdAEAANmSOx8AAAABYktHRH4/uEFzAAAAB3RJTUUH3wscBzYVoy2+dwAAAJxJREFUCNdjYAABWTl5RgVFJTBbmUmFWVUtTx3EZmHV0GTTytfWYdDV02c3MOQwMi4w4WTg4jI1M+e2sCy0suZhsLG147XncyhydOJ3ZnApdnUTcPco8RT0EmLwLi3zEfYt9/MXCQhkCAquCAkNqwwXjYiMYhCLjqmKrY4Tj09gYWFIlEhKrklJlUzjSmdgkMqQzqzNksnOyWVgAABtEB7gG6KeHgAAACV0RVh0ZGF0ZTpjcmVhdGUAMjAxNS0xMS0yOFQwNzo1NDoyMS0wNTowMCRACR8AAAAldEVYdGRhdGU6bW9kaWZ5ADIwMTUtMTEtMjhUMDc6NTQ6MjEtMDU6MDBVHbGjAAAAAElFTkSuQmCC";

        private string B64D = "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAMAAABhq6zVAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAABfVBMVEUAAAC2tbW3s7O2tLS/r6/Brq6vtra1s7O2tLS1srK2s7O2tLSxsLC1s7O3t7e2s7O2tLS1ubm0s7OroqK2tLS3s7O2tbWxqKi2s7O1s7O2t7e2tbW2s7O6sbGxtra4tLS2tLS2tLS1tLS3s7O2tLS2tLS2tLS2s7O9r6+ytbW1s7O2tLS2s7O2tLS2tLS1s7P1ioq1tLS1tLS2tLS2tLS2s7O2s7O2tLS2s7O2tLS1s7OzsbG2tLS2tLS1s7O2tbW2s7O2tLS2s7O2s7O2tLS2tLS6srK2tLS2tLS2s7O1tbW2s7O1s7O2tLS2tLS2s7O2tLSys7OssLC2tLS2tLS1tLS2tLS2s7O2tLTGoKCut7e0tbW2tLS2tLS2tLS1rq63tra3s7O3s7O2tLS2tLS1s7O1s7O2s7O2tbW1tLS1tLS3tLSwsbG3tra2tLS2tLS2tLS2tLS2tLS2tLS2tLS2tLS2tLS2tLS2tLS2tLS2tLS2tLS2tLS1tLT///9wiMU7AAAAbnRSTlMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAWz8agABKOiVAAOt7jYBAgEABFr/fAABAhjRyg0VZhQDi/tMApWzBUD6kwJtgMTcHgIABKTkuvtkBAAAGtb7vwEBAkPz9zABAAJwdAEAANmSOx8AAAABYktHRH4/uEFzAAAAB3RJTUUH3wscBzg0cceDpwAAAJxJREFUCNdjYAABWTl5RgVFJTBbmUmFWVUtTx3EZmHV0GTTytfWYdDV02c3MOQwMi4w4WTg4jI1M+e2sCy0suZhsLG147XncyhydOJ3ZnApdnUTcPco8RT0EmLwLi3zEfYt9/MXCQhkCAquCAkNqwwXjYiMYhCLjqmKrY4Tj09gYWFIlEhKrklJlUzjSmdgkMqQzqzNksnOyWVgAABtEB7gG6KeHgAAACV0RVh0ZGF0ZTpjcmVhdGUAMjAxNS0xMS0yOFQwNzo1Njo1Mi0wNTowMBuYyqYAAAAldEVYdGRhdGU6bW9kaWZ5ADIwMTUtMTEtMjhUMDc6NTY6NTItMDU6MDBqxXIaAAAAAElFTkSuQmCC";
        public bool Bold { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            G.Clear(Parent.BackColor);


            if (Enabled)
            {
                switch (State)
                {

                    case Helpers.MouseState.None:

                        using (Pen Border = new Pen(Helpers.ColorFromHex("#C8C8C8")))
                        {
                            G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, 20, 20), 2));
                        }


                        break;

                    default:

                        using (Pen Border = new Pen(Helpers.ColorFromHex("#2C9CDA")))
                        {
                            G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, 20, 20), 2));
                        }


                        break;
                }

                using (SolidBrush TextColor = new SolidBrush(Helpers.ColorFromHex("#424E5A")))
                {


                    if (Bold)
                    {
                        using (Font BFont = new Font("Segoe UI", 10, FontStyle.Bold))
                        {
                            G.DrawString(Text, BFont, TextColor, new Point(27, 1));
                        }

                    }
                    else
                    {
                        using (Font DFont = new Font("Segoe UI", 10))
                        {
                            G.DrawString(Text, DFont, TextColor, new Point(27, 1));
                        }

                    }

                }


            }
            else
            {
                using (Pen Border = new Pen(Helpers.ColorFromHex("#E6E6E6")))
                {
                    G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, 20, 20), 2));
                }

                using (SolidBrush TextColor = new SolidBrush(Helpers.ColorFromHex("#A6B2BE")))
                {


                    if (Bold)
                    {
                        using (Font BFont = new Font("Segoe UI", 10, FontStyle.Bold))
                        {
                            G.DrawString(Text, BFont, TextColor, new Point(27, 1));
                        }

                    }
                    else
                    {
                        using (Font DFont = new Font("Segoe UI", 10))
                        {
                            G.DrawString(Text, DFont, TextColor, new Point(27, 1));
                        }

                    }

                }

            }


            if (Checked)
            {

                if (Enabled)
                {
                    using (Image I = Image.FromStream(new MemoryStream(Convert.FromBase64String(B64E))))
                    {
                        G.DrawImage(I, new Rectangle(5, 4, 12, 12));
                    }


                }
                else
                {
                    using (Image I = Image.FromStream(new MemoryStream(Convert.FromBase64String(B64D))))
                    {
                        G.DrawImage(I, new Rectangle(5, 4, 12, 12));
                    }

                }

            }

            base.OnPaint(e);
        }

    }

    [DefaultEvent("CheckedChanged")]
    public class AirFoxRadioButton : Base.BaseRadioButton
    {

        private Graphics G;
        public bool Bold { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            G.Clear(Parent.BackColor);


            if (Enabled)
            {
                switch (State)
                {

                    case Helpers.MouseState.None:

                        using (Pen Border = new Pen(Helpers.ColorFromHex("#C8C8C8")))
                        {
                            G.DrawEllipse(Border, new Rectangle(0, 0, 20, 20));
                        }


                        break;
                    default:

                        using (Pen Border = new Pen(Helpers.ColorFromHex("#2C9CDA")))
                        {
                            G.DrawEllipse(Border, new Rectangle(0, 0, 20, 20));
                        }


                        break;
                }

                using (SolidBrush TextColor = new SolidBrush(Helpers.ColorFromHex("#424E5A")))
                {


                    if (Bold)
                    {
                        using (Font BFont = new Font("Segoe UI", 10, FontStyle.Bold))
                        {
                            G.DrawString(Text, BFont, TextColor, new Point(27, 1));
                        }

                    }
                    else
                    {
                        using (Font DFont = new Font("Segoe UI", 10))
                        {
                            G.DrawString(Text, DFont, TextColor, new Point(27, 1));
                        }

                    }

                }


            }
            else
            {
                using (Pen Border = new Pen(Helpers.ColorFromHex("#E6E6E6")))
                {
                    G.DrawEllipse(Border, new Rectangle(0, 0, 20, 20));
                }

                using (SolidBrush TextColor = new SolidBrush(Helpers.ColorFromHex("#A6B2BE")))
                {


                    if (Bold)
                    {
                        using (Font BFont = new Font("Segoe UI", 10, FontStyle.Bold))
                        {
                            G.DrawString(Text, BFont, TextColor, new Point(27, 1));
                        }

                    }
                    else
                    {
                        using (Font DFont = new Font("Segoe UI", 10))
                        {
                            G.DrawString(Text, DFont, TextColor, new Point(27, 1));
                        }

                    }

                }

            }


            if (Checked)
            {

                if (Enabled)
                {
                    using (SolidBrush FillColor = new SolidBrush(Helpers.ColorFromHex("#2C9CDA")))
                    {
                        G.FillEllipse(FillColor, new Rectangle(4, 4, 12, 12));
                    }


                }
                else
                {
                    using (SolidBrush FillColor = new SolidBrush(Helpers.ColorFromHex("#B6B4B4")))
                    {
                        G.FillEllipse(FillColor, new Rectangle(4, 4, 12, 12));
                    }

                }

            }

            base.OnPaint(e);
        }

    }

    public class AirFoxHeader1 : Control
    {


        private Graphics G;
        public AirFoxHeader1()
        {
            Font = new Font("Segoe UI Semibold", 20);
            ForeColor = Helpers.ColorFromHex("#4C5864");
            DoubleBuffered = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(Width, 51);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            G.Clear(Parent.BackColor);

            using (Font HFont = new Font("Segoe UI Semibold", 20))
            {
                using (SolidBrush HColor = new SolidBrush(Helpers.ColorFromHex("#4C5864")))
                {
                    G.DrawString(Text, HFont, HColor, new Point(0, 0));
                }
            }

            using (Pen BottomLine = new Pen(Helpers.ColorFromHex("#C8C8C8")))
            {
                G.DrawLine(BottomLine, new Point(0, 50), new Point(Width, 50));
            }

            base.OnPaint(e);

        }

    }

    public class AirFoxHeader2 : Control
    {


        private Graphics G;
        public AirFoxHeader2()
        {
            Font = new Font("Segoe UI", 10, FontStyle.Bold);
            ForeColor = Helpers.ColorFromHex("#4C5864");
            DoubleBuffered = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(Width, 19);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            G.Clear(Parent.BackColor);

            using (Font HFont = new Font("Segoe UI", 10, FontStyle.Bold))
            {
                using (SolidBrush HColor = new SolidBrush(Helpers.ColorFromHex("#4C5864")))
                {
                    G.DrawString(Text, HFont, HColor, new Point(0, 0));
                }
            }

            base.OnPaint(e);

        }

    }

    [DefaultEvent("CheckedChanged")]
    public class AirFoxOnOffBox : Base.CheckControl
    {

        private Graphics G;
        private string B64C = "iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAQAAAAnOwc2AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QA/4ePzL8AAAAHdElNRQffCxwIKTQpQueKAAAAn0lEQVQI1yXKMU4CQRxG8TczW5nYWRCX+4it1/AUVvacArkGBQkBLmKUkBB3ne/b+VNs9ZKXXwKAOicT8cR3mVejUbo0scpf/NKSypRE7Sr1VReFdgx55D+rE3Wlq0J798SD3qeFqC+6KHR2b9BGoa3e9KPQwUvjgtYKNY0KnfxsVCr84Q+FQsdZGcOQB/ypgxezqhgi3VIr02PDyRgDd6AdcPpYOg4ZAAAAJXRFWHRkYXRlOmNyZWF0ZQAyMDE1LTExLTI4VDA4OjQxOjUyLTA1OjAwH7rbKgAAACV0RVh0ZGF0ZTptb2RpZnkAMjAxNS0xMS0yOFQwODo0MTo1Mi0wNTowMG7nY5YAAAAASUVORK5CYII=";

        private string B64U = "iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAQAAAAnOwc2AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QA/4ePzL8AAAAHdElNRQffCxwIKir4YIkqAAAAgUlEQVQI122OMQrCQAAENxoMxz3Aj8Y3WAv6jtzVAYPYKah/8AtC5AZdm1TqFss0y6xGseXoxb26yA172iKx5o1JDg4kzMhK9JgnJpMn6uVIwoCn7hx1lmsSplAwyfVJs2Wlr8wlR7qfOYc/Ina8MNnBgTxdeogNg5ubrnLDQFv0AXVYjzifEiowAAAAJXRFWHRkYXRlOmNyZWF0ZQAyMDE1LTExLTI4VDA4OjQyOjQyLTA1OjAwOCdgtwAAACV0RVh0ZGF0ZTptb2RpZnkAMjAxNS0xMS0yOFQwODo0Mjo0Mi0wNTowMEl62AsAAAAASUVORK5CYII=";
        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;


            if (Enabled)
            {

                if (Checked)
                {
                    using (Pen Border = new Pen(Helpers.ColorFromHex("#C8C8C8")))
                    {
                        using (SolidBrush Background = new SolidBrush(Helpers.ColorFromHex("#2C9CDA")))
                        {
                            using (Pen BackBorder = new Pen(Helpers.ColorFromHex("#2A8AC1")))
                            {
                                using (Image I = Image.FromStream(new MemoryStream(Convert.FromBase64String(B64C))))
                                {
                                    G.DrawPath(Border, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));

                                    G.FillPath(Background, Helpers.RoundRect(new Rectangle(5, 5, 17, 17), 2));
                                    G.DrawPath(BackBorder, Helpers.RoundRect(new Rectangle(5, 5, 17, 17), 2));

                                    G.DrawImage(I, new Point(9, 9));
                                }
                            }
                        }
                    }


                }
                else
                {
                    using (Pen Border = new Pen(Helpers.ColorFromHex("#C8C8C8")))
                    {
                        using (SolidBrush Background = new SolidBrush(Helpers.ColorFromHex("#FF9500")))
                        {
                            using (Pen BackBorder = new Pen(Helpers.ColorFromHex("#DC8400")))
                            {
                                using (Image I = Image.FromStream(new MemoryStream(Convert.FromBase64String(B64U))))
                                {
                                    G.DrawPath(Border, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));

                                    G.FillPath(Background, Helpers.RoundRect(new Rectangle(Width - 23, 5, 17, 17), 2));
                                    G.DrawPath(BackBorder, Helpers.RoundRect(new Rectangle(Width - 23, 5, 17, 17), 2));

                                    G.DrawImage(I, new Point(Width - 19, 9));
                                }
                            }
                        }
                    }

                }


            }
            else
            {

                if (Checked)
                {
                    using (Pen Border = new Pen(Helpers.ColorFromHex("#E6E6E6")))
                    {
                        using (SolidBrush Background = new SolidBrush(Helpers.ColorFromHex("#7DB7D8")))
                        {
                            using (Pen BackBorder = new Pen(Helpers.ColorFromHex("#7CA6BF")))
                            {
                                using (Image I = Image.FromStream(new MemoryStream(Convert.FromBase64String(B64C))))
                                {
                                    G.DrawPath(Border, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));

                                    G.FillPath(Background, Helpers.RoundRect(new Rectangle(5, 5, 17, 17), 2));
                                    G.DrawPath(BackBorder, Helpers.RoundRect(new Rectangle(5, 5, 17, 17), 2));

                                    G.DrawImage(I, new Point(9, 9));
                                }
                            }
                        }
                    }


                }
                else
                {
                    using (Pen Border = new Pen(Helpers.ColorFromHex("#E6E6E6")))
                    {
                        using (SolidBrush Background = new SolidBrush(Helpers.ColorFromHex("#FFCB7C")))
                        {
                            using (Pen BackBorder = new Pen(Helpers.ColorFromHex("#E2BD85")))
                            {
                                using (Image I = Image.FromStream(new MemoryStream(Convert.FromBase64String(B64U))))
                                {
                                    G.DrawPath(Border, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));

                                    G.FillPath(Background, Helpers.RoundRect(new Rectangle(Width - 23, 5, 17, 17), 2));
                                    G.DrawPath(BackBorder, Helpers.RoundRect(new Rectangle(Width - 23, 5, 17, 17), 2));

                                    G.DrawImage(I, new Point(Width - 19, 9));
                                }
                            }
                        }
                    }

                }


            }


            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(55, 28);
        }

    }

    public class AirFoxButton : Base.ButtonBase
    {


        private Graphics G;
        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            G.Clear(Parent.BackColor);


            if (Enabled)
            {
                switch (State)
                {

                    case Helpers.MouseState.None:

                        using (SolidBrush Background = new SolidBrush(Helpers.ColorFromHex("#F9F9F9")))
                        {
                            G.FillPath(Background, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));
                        }


                        break;
                    case Helpers.MouseState.Over:

                        using (SolidBrush Background = new SolidBrush(Helpers.ColorFromHex("#F2F2F2")))
                        {
                            G.FillPath(Background, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));
                        }


                        break;
                    case Helpers.MouseState.Down:

                        using (SolidBrush Background = new SolidBrush(Helpers.ColorFromHex("#E8E8E8")))
                        {
                            G.FillPath(Background, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));
                        }


                        break;
                }

                using (Pen Border = new Pen(Helpers.ColorFromHex("#C1C1C1")))
                {
                    G.DrawPath(Border, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));
                }

                using (Font TextFont = new Font("Segoe UI", 10))
                {
                    using (SolidBrush TextColor = new SolidBrush(Helpers.ColorFromHex("#424E5A")))
                    {
                        Helpers.CenterString(G, Text, TextFont, TextColor.Color, new Rectangle(3, 0, Width, Height));
                    }
                }


            }
            else
            {
                using (SolidBrush Background = new SolidBrush(Helpers.ColorFromHex("#F9F9F9")))
                {
                    G.FillPath(Background, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));
                }

                using (Font TextFont = new Font("Segoe UI", 10))
                {
                    using (SolidBrush TextColor = new SolidBrush(Helpers.ColorFromHex("#A6B2BE")))
                    {
                        Helpers.CenterString(G, Text, TextFont, TextColor.Color, new Rectangle(3, 0, Width, Height));
                    }
                }

                using (Pen Border = new Pen(Helpers.ColorFromHex("#D1D1D1")))
                {
                    G.DrawPath(Border, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));
                }

            }

            base.OnPaint(e);
        }

    }

    [DefaultEvent("TextChanged")]
    public class AirFoxTextbox : Control
    {

        private TextBox withEventsField_TB = new TextBox();
        private TextBox TB
        {
            get { return withEventsField_TB; }
            set
            {
                if (withEventsField_TB != null)
                {
                    withEventsField_TB.TextChanged -= TextChangeTb;
                }
                withEventsField_TB = value;
                if (withEventsField_TB != null)
                {
                    withEventsField_TB.TextChanged += TextChangeTb;
                }
            }
        }
        private Graphics G;
        private Helpers.MouseState State;
        private bool IsDown;
        private bool _allowpassword = false;
        private int _maxChars = 32767;
        private HorizontalAlignment _textAlignment;
        private bool _multiLine = false;

        private bool _readOnly = false;

        private bool IsEnabled;
        public new bool Enabled
        {
            get { return EnabledCalc; }
            set
            {
                IsEnabled = value;

                if (Enabled)
                {
                    Cursor = Cursors.Hand;
                }
                else
                {
                    Cursor = Cursors.Default;
                }

                Invalidate();
            }
        }

        [DisplayName("Enabled")]
        public bool EnabledCalc
        {
            get { return IsEnabled; }
            set
            {
                Enabled = value;
                Invalidate();
            }
        }

        public new bool UseSystemPasswordChar
        {
            get { return _allowpassword; }
            set
            {
                TB.UseSystemPasswordChar = UseSystemPasswordChar;
                _allowpassword = value;
                Invalidate();
            }
        }

        public new int MaxLength
        {
            get { return _maxChars; }
            set
            {
                _maxChars = value;
                TB.MaxLength = MaxLength;
                Invalidate();
            }
        }

        public new HorizontalAlignment TextAlign
        {
            get { return _textAlignment; }
            set
            {
                _textAlignment = value;
                Invalidate();
            }
        }

        public new bool MultiLine
        {
            get { return _multiLine; }
            set
            {
                _multiLine = value;
                TB.Multiline = value;
                OnResize(EventArgs.Empty);
                Invalidate();
            }
        }

        public new bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                if (TB != null)
                {
                    TB.ReadOnly = value;
                }
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            Invalidate();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            TB.ForeColor = ForeColor;
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            TB.Font = Font;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            TB.Focus();
        }

        private void TextChangeTb(object sender, EventArgs e)
        {
            Text = TB.Text;
        }

        private void TextChng()
        {
            TB.Text = Text;
        }

        public void NewTextBox()
        {
            var _with1 = TB;
            _with1.Text = string.Empty;
            _with1.BackColor = Color.White;
            _with1.ForeColor = Color.FromArgb(66, 78, 90);
            _with1.TextAlign = HorizontalAlignment.Left;
            _with1.BorderStyle = BorderStyle.None;
            _with1.Location = new Point(3, 3);
            _with1.Font = new Font("Segoe UI", 10);
            _with1.Size = new Size(Width - 3, Height - 3);
            _with1.UseSystemPasswordChar = UseSystemPasswordChar;
        }

        public AirFoxTextbox() : base()
        {
            TextChanged += AirFoxTextbox_TextChanged;
            NewTextBox();
            Controls.Add(TB);
            SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            TextAlign = HorizontalAlignment.Left;
            ForeColor = Color.FromArgb(66, 78, 90);
            Font = new Font("Segoe UI", 10);
            Size = new Size(130, 29);
            Enabled = true;
        }

        void AirFoxTextbox_TextChanged(object sender, EventArgs e)
        {
            TB.Text = Text;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            G.Clear(Parent.BackColor);


            if (Enabled)
            {

                if (State == Helpers.MouseState.Down)
                {
                    using (Pen Border = new Pen(Helpers.ColorFromHex("#2C9CDA")))
                    {
                        G.DrawPath(Border, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));
                    }


                }
                else
                {
                    using (Pen Border = new Pen(Helpers.ColorFromHex("#C8C8C8")))
                    {
                        G.DrawPath(Border, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));
                    }

                }


            }
            else
            {
                using (Pen Border = new Pen(Helpers.ColorFromHex("#E6E6E6")))
                {
                    G.DrawPath(Border, Helpers.RoundRect(Helpers.FullRectangle(Size, true), 2));
                }

            }

            TB.TextAlign = TextAlign;
            TB.UseSystemPasswordChar = UseSystemPasswordChar;

            base.OnPaint(e);

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!MultiLine)
            {
                TB.Location = new Point(10, (Height / 2) - (TB.Height / 2) - 0);
                TB.Size = new Size(Width - 20, TB.Height);
            }
            else
            {
                TB.Location = new Point(10, 10);
                TB.Size = new Size(Width - 20, Height - 20);
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            State = Helpers.MouseState.Down;
            Invalidate();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            State = Helpers.MouseState.None;
            Invalidate();
        }

    }
    public class AirFoxRedirect : Base.ButtonBase
    {


        private Graphics G;

        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            switch (State)
            {

                case Helpers.MouseState.Over:

                    using (SolidBrush TextColor = new SolidBrush(Helpers.ColorFromHex("#178CE5")))
                    {
                        using (Font TextFont = new Font("Segoe UI", 10, FontStyle.Underline))
                        {
                            G.DrawString(Text, TextFont, TextColor, new Point(0, 0));
                        }
                    }


                    break;
                case Helpers.MouseState.Down:

                    using (SolidBrush TextColor = new SolidBrush(Helpers.ColorFromHex("#FF9500")))
                    {
                        using (Font TextFont = new Font("Segoe UI", 10))
                        {
                            G.DrawString(Text, TextFont, TextColor, new Point(0, 0));
                        }
                    }


                    break;
                default:

                    using (SolidBrush TextColor = new SolidBrush(Helpers.ColorFromHex("#0095DD")))
                    {
                        using (Font TextFont = new Font("Segoe UI", 10))
                        {
                            G.DrawString(Text, TextFont, TextColor, new Point(0, 0));
                        }
                    }


                    break;
            }

            base.OnPaint(e);

        }

    }

    public class AirFoxAlertBox : Base.ButtonBase
    {

        public Styles Style { get; set; }

        private Graphics G;
        private Color Background;
        private Color TextColor;

        private Color LeftBar;
        public enum Styles : byte
        {
            Green = 0,
            Blue = 1,
            Yellow = 2,
            Red = 3
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            switch (Style)
            {

                case Styles.Green:
                    Background = Helpers.ColorFromHex("#DFF0D6");
                    TextColor = Helpers.ColorFromHex("#4E8C45");
                    LeftBar = Helpers.ColorFromHex("#CEE5B6");

                    break;
                case Styles.Blue:
                    Background = Helpers.ColorFromHex("#D9EDF8");
                    TextColor = Helpers.ColorFromHex("#498FB8");
                    LeftBar = Helpers.ColorFromHex("#AFD9F0");

                    break;
                case Styles.Yellow:
                    Background = Helpers.ColorFromHex("#FCF8E1");
                    TextColor = Helpers.ColorFromHex("#908358");
                    LeftBar = Helpers.ColorFromHex("#FAEBC8");

                    break;
                case Styles.Red:
                    Background = Helpers.ColorFromHex("#F2DEDE");
                    TextColor = Helpers.ColorFromHex("#C2635E");
                    LeftBar = Helpers.ColorFromHex("#EBCCD1");

                    break;
            }

            using (Font TextFont = new Font("Segoe UI", 10))
            {
                using (SolidBrush Back = new SolidBrush(Background))
                {
                    using (SolidBrush TC = new SolidBrush(TextColor))
                    {
                        using (SolidBrush LB = new SolidBrush(LeftBar))
                        {
                            G.FillRectangle(Back, Helpers.FullRectangle(Size, true));
                            G.SmoothingMode = SmoothingMode.None;
                            G.FillRectangle(LB, new Rectangle(0, 1, 6, Height - 2));
                            G.SmoothingMode = SmoothingMode.HighQuality;
                            G.DrawString(Text, TextFont, TC, new Point(20, 11));
                        }
                    }
                }
            }

            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(Width, 40);
        }

    }

}