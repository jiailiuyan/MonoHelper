using Gdk;
using MonoDevelop.Components;

namespace Gtk
{
    public class CellRendererFileSystemInfoImage : CellRendererText
    {

        public static Xwt.Drawing.Image DefaultImage { get; set; }

        private Label lable = new Gtk.Label();

        private string fielName;
        [GLib.Property("filename")]
        public string FileName
        {
            get { return fielName; }
            set
            {
                lable.Text = value;
                Text = fielName = value;
            }
        }

        [GLib.Property("image")]
        public Xwt.Drawing.Image Image { get; set; }

        public CellRendererFileSystemInfoImage()
        {
            this.Mode |= Gtk.CellRendererMode.Editable;
        }

        protected override void Render(Drawable window, Widget widget, Rectangle background_area, Rectangle cell_area, Rectangle expose_area, CellRendererState flags)
        {
            var gc = widget.Style.ForegroundGC(StateType.Normal);
            if (Image != null)
            {
                var ys = cell_area.Height / 2 - (int)Image.Height / 2;
                using (var ctx = Gdk.CairoHelper.Create(window))
                {
                    var x = cell_area.X;
                    var y = cell_area.Y + ys;
                    ctx.DrawImage(widget, Image, x, y);
                }
                window.DrawLayout(gc, cell_area.X + 3 + (int)Image.Width, cell_area.Y + ys, lable.Layout);
            }
            else
            {
                int ys = cell_area.Height / 2 - (int)(CellRendererFileSystemInfoImage.DefaultImage.Height / 2);

                using (var ctx = Gdk.CairoHelper.Create(window))
                {
                    var x = cell_area.X;
                    var y = cell_area.Y + ys;
                    ctx.DrawImage(widget, CellRendererFileSystemInfoImage.DefaultImage, x, y);
                }
                window.DrawLayout(gc, cell_area.X + 3 + (int)CellRendererFileSystemInfoImage.DefaultImage.Width, cell_area.Y + ys, lable.Layout);
            }
        }

        protected void GetImageInfo(Gdk.Rectangle cell_area, out Xwt.Drawing.Image img, out int x, out int y)
        {
            img = GetImage();
            if (img == null)
            {
                x = (int)(cell_area.X + cell_area.Width / 2);
                y = (int)(cell_area.Y + cell_area.Height / 2);
            }
            else
            {
                x = (int)(cell_area.X + cell_area.Width / 2 - (int)(img.Width / 2));
                y = (int)(cell_area.Y + cell_area.Height / 2 - (int)(img.Height / 2));
            }
        }

        public override void GetSize(Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
        {
            var img = GetImage();
            if (img != null)
            {
                width = (int)img.Width;
                height = (int)img.Height;
            }
            else
                width = height = 0;

            var filelenght = this.FileName != null ? this.FileName.Length : 0;
            width += (int)Xpad * 2 + filelenght * 7;
            height += (int)Ypad * 2;

            x_offset = y_offset = 0;
        }

        Xwt.Drawing.Image GetImage()
        {
            Xwt.Drawing.Image img = this.Image;
            return img != CellRendererImage.NullImage ? img : null;
        }
    }
}
