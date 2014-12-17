using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using MonoDevelop.Components;

namespace Gtk
{
    public class CellRendererRenameText : CellRendererText
    {

        bool isEditableMode = false;
        [GLib.Property("iseditablemode")]
        public bool IsEditableMode
        {
            get { return isEditableMode; }
            set
            {
                isEditableMode = value;
            }
        }

        public CellRendererRenameText()
        {
            this.Mode |= Gtk.CellRendererMode.Editable;
        }
    }

    public class RenameTextArgs : EventArgs
    {
        public string OldName { get; set; }

        public string NewName { get; set; }
    }

}