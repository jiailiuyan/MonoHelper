using System;
using System.Collections.Generic;
using Gdk;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide;

namespace Gtk
{
    public class ShowImageControl : EventBox
    {
        private VPaned vpanedMain = new Gtk.VPaned() { CanFocus = true };

        private Expander expanderMain = new Gtk.Expander(null) { CanFocus = true, Expanded = false };

        private Label imageInfoLabel = new Gtk.Label() { UseUnderline = true };

        private Image showImage = new Gtk.Image() { };

        private Widget CustomWidget;

        private int mainWidgetHeight = 0;
        private int expanderHeight = 24;

        private int expanderMainHeight = 150;

        public int ImageBorder = 3;

        Paned.PanedChild ShowPaned;

        public string ImageLabel
        {
            get { return imageInfoLabel.LabelProp; }
            set { imageInfoLabel.LabelProp = value; }
        }

        Xwt.Drawing.Image XwtImage;

        public string ImageFile
        {
            get
            {
                return showImage.File;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    XwtImage = null;
                    showImage.Visible = false;
                    ImageLabel = string.Empty;
                    showImage.Pixbuf = null;
                }
                else
                {
                    XwtImage = Gtk.ImageIcon.GetIconFromFile(value);
                    if (XwtImage != null)
                    {
                        showImage.Pixbuf = XwtImage.GetPixbuf();
                        if (showImage.Pixbuf != null)
                        {
                            ImageLabel = " " + showImage.Pixbuf.Width + " * " + showImage.Pixbuf.Height;
                        }

                        showImage.Visible = true;

                        JudgePostion();
                    }
                }
            }
        }

        public ShowImageControl(Widget widget)
        {
            this.CustomWidget = widget;
            Initialize();
            InitEvent();
        }

        private void Initialize()
        {
            this.vpanedMain.Add(this.CustomWidget);
            var w2 = ((global::Gtk.Paned.PanedChild)(vpanedMain[this.CustomWidget]));
            w2.Resize = true;

            EventBox ieb = new EventBox();
            ieb.Child = showImage;
            ieb.BorderWidth = (uint)ImageBorder;
            this.expanderMain.Add(ieb);

            this.expanderMain.LabelWidget = imageInfoLabel;
            this.vpanedMain.Add(expanderMain);
            this.Add(vpanedMain);

            ShowPaned = ((Paned.PanedChild)(this.vpanedMain[this.expanderMain]));
            ShowPaned.Resize = false;
            ShowPaned.Child.SizeAllocated += Child_SizeAllocated;

            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
        }

        Gdk.Rectangle alloction;
        void Child_SizeAllocated(object o, SizeAllocatedArgs args)
        {
            if (!alloction.Equals(args.Allocation))
            {
                alloction = args.Allocation;
                JudgePostion();
            }
        }

        private void InitEvent()
        {
            this.expanderMain.Activated += expanderMain_Activated;
            this.vpanedMain.SizeRequested += vpanedMain_SizeRequested;
            this.expanderMain.Expanded = false;

            JudgePostion();
        }

        private void vpanedMain_SizeRequested(object o, SizeRequestedArgs args)
        {
            mainWidgetHeight = (((Gtk.Paned)(o)).Child2).Allocation.Bottom;
        }

        private void expanderMain_Activated(object sender, EventArgs e)
        {
            if (expanderMain.Expanded)
            {
                this.vpanedMain.Position = mainWidgetHeight - expanderHeight - expanderMainHeight - 30;
            }
            else
            {
                this.vpanedMain.Position = mainWidgetHeight - (((Gtk.Paned)(this.vpanedMain)).Child1).Allocation.Location.Y - expanderHeight;
            }
        }

        private void JudgePostion()
        {
            if (XwtImage != null)
            {
                var pixbuf = XwtImage.GetPixbuf();
                var w = ShowPaned.Child.Allocation.Right - 2 * ImageBorder;
                var h = ShowPaned.Child.Allocation.Height - expanderHeight - 2 * ImageBorder;
                double scalespanh = (double)h / (double)pixbuf.Height;
                double scalespanw = (double)w / (double)pixbuf.Width;
                double scalespan = scalespanh < scalespanw ? scalespanh : scalespanw;

                var width = (int)(pixbuf.Width * scalespan);
                width = width > (int)(pixbuf.Width * scalespan) ? width : (int)(pixbuf.Width * scalespan);
                var heigth = (int)(pixbuf.Height * scalespan);
                heigth = heigth > (int)(pixbuf.Height * scalespan) ? heigth : (int)(pixbuf.Height * scalespan);

                this.showImage.Pixbuf = pixbuf.ScaleSimple(width, heigth, Gdk.InterpType.Bilinear);

                this.showImage.GrabFocus();
                this.showImage.ShowNow();
                this.showImage.ShowAll();
            }

        }
    }

}



