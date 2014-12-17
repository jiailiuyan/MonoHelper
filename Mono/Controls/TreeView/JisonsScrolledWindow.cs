/* 
 * 
 * FileName:   JisonsScrolledWindow.cs
 * Version:    1.0
 * Date:       2014.05.23
 * Author:     Ji
 * 
 *========================================
 * @namespace  Jisons 
 * @class      JisonsScrolledWindow
 * @extends    Gtk.ScrolledWindow
 * 
 *             处理 TreeView 包装的 JisonsScrolledWindow 在其中处理了鼠标事件
 *             在此 KeyDown 事件由其内部的顶级控件进行捕获和传出，在此不做处理
 *========================================
 * 
 * Copyright © 迹I柳燕
 * 
 */

using System;
using Gdk;
using Gtk;

namespace CocoStudio.ToolKit
{
    public class JisonsScrolledWindow : Gtk.ScrolledWindow
    {

        public event EventHandler<WidgetEventArgs> KeyUp;

        public JisonsScrolledWindow()
        {
            this.WidgetEvent += JisonsScrolledWindow_WidgetEvent;
        }

        void JisonsScrolledWindow_WidgetEvent(object o, Gtk.WidgetEventArgs args)
        {
            switch (args.Event.Type)
            {
                case EventType.KeyRelease:
                    {
                        KeyUpHandle(o, args);
                        break;
                    }

                default: break;
            }
        }

        public void KeyUpHandle(object o, WidgetEventArgs args)
        {
            bool issent = true;
            OnKeyUp((EventKey)args.Event, ref issent);
            if (issent)
            {
                OnKeyUpHandle(o, args);
            }
        }

        public virtual void OnKeyUp(EventKey args, ref bool isSent)
        { }

        protected void OnKeyUpHandle(object o, WidgetEventArgs args)
        {
            if (this.KeyUp != null)
            {
                this.KeyUp(o, args);
            }
        }

    }
}
