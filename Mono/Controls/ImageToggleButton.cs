/* 
 * 
 * FileName:   ToggleButton.cs
 * Version:    1.0
 * Date:       2014.05.21
 * Author:     Ji
 * 
 *========================================
 * @namespace  Gtk 
 * @class      ToggleButton
 * @extends    Gtk.ToggleButton
 * 
 *             实现渲染图片的 ToggleButton 
 * 
 *========================================
 * 
 * Copyright © 迹I柳燕
 * 
 */

using Gdk;

namespace Gtk
{
    public class ImageToggleButton : Gtk.ToggleButton
    {

        public Gdk.Pixbuf CheckPixbuf { get; set; }
        public Gdk.Pixbuf UnCheckPixbuf { get; set; }

        public string CheckString { get; set; }
        public string UnCheckString { get; set; }

        private Label lable = new Gtk.Label();

        public ImageToggleButton()
        { }

        public ImageToggleButton(Gdk.Pixbuf checkimage, Gdk.Pixbuf uncheckimage)
            : base()
        {
            this.CheckPixbuf = checkimage;
            this.UnCheckPixbuf = uncheckimage;
        }

        protected override bool OnExposeEvent(Gdk.EventExpose evnt)
        {
            bool r = base.OnExposeEvent(evnt);

            if (this.CheckPixbuf != null && this.UnCheckPixbuf != null)
            {
                int ix = 0;
                if (this.Active)
                {
                    if (this.CheckPixbuf.Width <= this.Allocation.Width && this.CheckPixbuf.Height <= this.Allocation.Height)
                    {
                        ix = this.Allocation.X;
                        var iy = (this.Allocation.Height - this.CheckPixbuf.Height) / 2 + this.Allocation.Y;
                        this.GdkWindow.DrawPixbuf(this.Style.BackgroundGC(StateType.Normal), this.CheckPixbuf, 0, 0, ix, iy, this.CheckPixbuf.Width, this.CheckPixbuf.Height, RgbDither.Normal, 0, 0);

                        lable.Text = CheckString;
                    }
                }
                else
                {
                    if (this.UnCheckPixbuf.Width <= this.Allocation.Width && this.UnCheckPixbuf.Height <= this.Allocation.Height)
                    {
                        ix = this.Allocation.X;
                        var iy = (this.Allocation.Height - this.UnCheckPixbuf.Height) / 2 + this.Allocation.Y;
                        this.GdkWindow.DrawPixbuf(this.Style.BackgroundGC(StateType.Normal), this.UnCheckPixbuf, 0, 0, ix, iy, this.UnCheckPixbuf.Width, this.UnCheckPixbuf.Height, RgbDither.Normal, 0, 0);

                        lable.Text = UnCheckString;
                    }
                }

                int wi, he;
                lable.Layout.GetPixelSize(out wi, out he);
                var sx = ix + this.UnCheckPixbuf.Width + 2;
                var sy = this.Allocation.Height / 2 - he / 2 + this.Allocation.Y;
                this.GdkWindow.DrawLayout(this.Style.ForegroundGC(StateType.Normal), sx, sy, lable.Layout);
            }

            return r;
        }
    }
}
