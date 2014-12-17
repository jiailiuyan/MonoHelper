using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;

namespace Gtk
{
    public class FilterTextBox : EventBox
    {
        public static string DefaultInfo = " Search ...";

        private Entry entry = new Entry();

        public event EventHandler TextChanged;

        public string Text
        {
            get { return entry.Text; }
            set { entry.Text = value; }
        }

        public FilterTextBox(Xwt.Drawing.Image background)
        {
            var hbox1 = new global::Gtk.HBox();
            hbox1.Spacing = 6;

            entry.CanFocus = true;
            entry.IsEditable = true;
            entry.InvisibleChar = '●';
            hbox1.Add(entry);
            Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(hbox1[entry]));
            w1.Position = 0;

            CSImageButton button1 = new CSImageButton();
            button1.Image = background;

            button1.CanFocus = true;
            button1.WidthRequest = 20;
            hbox1.Add(button1);
            Box.BoxChild w2 = ((Box.BoxChild)(hbox1[button1]));
            w2.Position = 1;
            w2.Expand = false;
            w2.Fill = false;

            EventBox space = new EventBox();
            space.WidthRequest = 20;
            hbox1.Add(space);
            Box.BoxChild w3 = ((Box.BoxChild)(hbox1[space]));
            w3.Position = 2;
            w3.Expand = false;
            w3.Fill = false;

            this.Add(hbox1);

            if (this.Child != null)
            {
                this.Child.ShowAll();
            }

            entry.TextInserted += (o, s) => OnTextChangedHandle();
            entry.TextDeleted += (o, s) => OnTextChangedHandle();

            button1.ButtonPressEvent += button1_ButtonPressEvent;

            entry.FocusInEvent += FilterTextBox_FocusInEvent;
            entry.FocusOutEvent += FilterTextBox_FocusOutEvent;

            SetShowInfo(DefaultInfo);
        }

        bool isSentChanged = true;

        [GLib.ConnectBefore]
        void FilterTextBox_FocusInEvent(object o, FocusInEventArgs args)
        {
            SetShowInfo(string.Empty);
        }

        [GLib.ConnectBefore]
        void FilterTextBox_FocusOutEvent(object o, FocusOutEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(entry.Text) || entry.Text == DefaultInfo)
            {
                SetShowInfo(DefaultInfo);
            }
        }

        private void SetShowInfo(string info)
        {
            isSentChanged = false;
            entry.Text = info;
            isSentChanged = true;
        }

        void button1_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            entry.Text = string.Empty;
            SetShowInfo(DefaultInfo);
        }

        protected void OnTextChangedHandle()
        {
            if (isSentChanged && this.TextChanged != null)
            {
                this.TextChanged(this, EventArgs.Empty);
            }
        }

    }
}
