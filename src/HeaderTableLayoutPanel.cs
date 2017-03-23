using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D; 
using System.Runtime.InteropServices;

namespace CBComponents
{
  public class HeaderTableLayoutPanel : System.Windows.Forms.TableLayoutPanel
  {
    /// <summary>
    /// Header text
    /// </summary>
    [Browsable(true), DefaultValue(null), Category("Header")]
    public string CaptionText
    {
      get { return this.captionText; }
      set
      {
        if (this.captionText != value)
        {
          this.captionText = value;
          this.CalculateCaptionParams();
          Invalidate();
        }
      }
    }
    private string captionText = null;

    /// <summary>
    /// Drawing styles for Header
    /// </summary>
    public enum HighlightCaptionStyle
    {
      ForeColor, HighlightColor, HighlightStyle, NavisionAxaptaStyle
    }

    /// <summary>
    /// Drawing style of Header
    /// </summary>
    [Browsable(true), DefaultValue(HighlightCaptionStyle.ForeColor), Category("Header")]
    public HighlightCaptionStyle CaptionStyle
    {
      get { return this.captionStyle; }
      set
      {
        if (this.captionStyle != value)
        {
          this.captionStyle = value;
          this.CalculateCaptionParams();
          Invalidate();
        }
      }
    }
    private HighlightCaptionStyle captionStyle = HighlightCaptionStyle.ForeColor;

    [Browsable(true), DefaultValue((byte)2), Category("Header")]
    public byte CaptionLineWidth
    {
      get { return this.captionLineWidth; }
      set
      {
        if (this.captionLineWidth != value)
        {
          this.captionLineWidth = value > (byte)22 ? (byte)22 : value;
          this.CalculateCaptionParams();
          Invalidate();
        }
      }
    }
    private byte captionLineWidth = 2;
        
    protected override void OnForeColorChanged(EventArgs e)
    {
      base.OnForeColorChanged(e);
      this.CalculateCaptionParams();
      Invalidate();
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
      base.OnBackColorChanged(e);
      this.CalculateCaptionParams();
      Invalidate();
    }

    protected override void OnFontChanged(EventArgs e)
    {
      base.OnFontChanged(e);
      this.CalculateCaptionParams();
      Invalidate();
    }

    // calculating and storing params 
    private int captionTextWidth;
    private int captionTextHeight;
    private Color captionTextColor;
    private Color captionLineBeginColor;
    private Color captionLineEndColor;
    private void CalculateCaptionParams()
    {
      if (!string.IsNullOrEmpty(this.captionText))
        using (var g = this.CreateGraphics())
        {
          var _size = g.MeasureString(this.captionText + "*", this.Font).ToSize();
          this.captionTextWidth = _size.Width;
          this.captionTextHeight = _size.Height;
        }
      else
      {
        this.captionTextWidth = 0;
        this.captionTextHeight = 0;
      }
      if (this.captionStyle == HighlightCaptionStyle.ForeColor)
      {
        this.captionTextColor = this.ForeColor;
        this.captionLineBeginColor = this.ForeColor;
        this.captionLineEndColor = this.BackColor;
      }
      else
      {
        this.captionTextColor = this.captionStyle == HighlightCaptionStyle.HighlightStyle ? SystemColors.HighlightText : SystemColors.Highlight;
        this.captionLineBeginColor = SystemColors.MenuHighlight;
        this.captionLineEndColor = this.BackColor; 
      }
    }

    public override Rectangle DisplayRectangle
    {
      get
      {
        var result = base.DisplayRectangle;
        int resize = 0;
        if (this.captionTextHeight > 0) resize = this.captionTextHeight + 2;
        if (this.captionStyle == HighlightCaptionStyle.HighlightStyle) resize += this.captionLineWidth * 2;
        else if (this.captionStyle != HighlightCaptionStyle.NavisionAxaptaStyle) resize += this.captionLineWidth;
        result.Height -= resize;
        result.Offset(0, resize);
        return result;
      }
    }

    protected override Size SizeFromClientSize(Size clientSize)
    {
      var result = base.SizeFromClientSize(clientSize);
      int resize = 0;
      if (this.captionTextHeight > 0) resize = this.captionTextHeight + 2;
      if (this.captionStyle == HighlightCaptionStyle.HighlightStyle) resize += this.captionLineWidth * 2;
      else if (this.captionStyle != HighlightCaptionStyle.NavisionAxaptaStyle) resize += this.captionLineWidth;
      result.Height += resize;
      return result;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      // draw gradient
      if (this.captionStyle == HighlightCaptionStyle.HighlightStyle)
      { // HighlightCaptionStyle.HighlightStyle allways draw
        using (Brush _gradientBrush = new LinearGradientBrush(new Point(0, 0), new Point(this.Width, 0), this.captionLineBeginColor, this.captionLineEndColor))
        using (Pen _gradientPen = new Pen(_gradientBrush, this.captionLineWidth * 2 + this.captionTextHeight))
          e.Graphics.DrawLine(_gradientPen, 0, _gradientPen.Width / 2, this.Width, _gradientPen.Width / 2);
      }
      else if (this.captionLineWidth > 0)
        if (this.captionStyle != HighlightCaptionStyle.NavisionAxaptaStyle)
        { // HighlightCaptionMode.ForeColor | HighlightCaptionMode.SystemColorsHighlight
          using (Brush _gradientBrush = new LinearGradientBrush(new Point(0, 0), new Point(this.Width, 0), this.captionLineBeginColor, this.captionLineEndColor))
          using (Pen _gradientPen = new Pen(_gradientBrush, this.captionLineWidth))
            e.Graphics.DrawLine(_gradientPen, 0, this.captionTextHeight + this.captionLineWidth / 2, this.Width, this.captionTextHeight + this.captionLineWidth / 2);
        }
        else if (this.captionTextWidth + 1 < this.Width)
        { // HighlightCaptionMode.NavisionAxapta
          using (Brush _gradientBrush = new LinearGradientBrush(new Point(this.captionTextWidth, 0), new Point(this.Width, 0), this.captionLineBeginColor, this.captionLineEndColor))
          using (Pen _gradientPen = new Pen(_gradientBrush, this.captionLineWidth > this.captionTextHeight ? this.captionTextHeight : this.captionLineWidth))
            e.Graphics.DrawLine(_gradientPen, this.captionTextWidth, this.captionTextHeight / 2 + 1, this.Width, this.captionTextHeight / 2 + 1);
          //using (Pen _roundPen = new Pen(this.captionGBColor, (this.captionLineWidth > this.captionTextHeight ? this.captionTextHeight : this.captionLineWidth) - 2))
            //e.Graphics.DrawLine(_roundPen, this.captionTextWidth - 1, this.captionTextHeight / 2 + 1, this.captionTextWidth, this.captionTextHeight / 2 + 1);
        }
      // draw Text
      if (this.captionTextHeight > 0)
        using (Brush _textBrush = new SolidBrush(this.captionTextColor))
          e.Graphics.DrawString(this.captionText, this.Font, _textBrush, 0, this.captionStyle == HighlightCaptionStyle.HighlightStyle ? this.CaptionLineWidth : 0);
    }

  }
}
