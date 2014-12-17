/* 
 * 
 * FileName:   JisionsTreeViewOfMouseAction.cs
 * Version:    1.0
 * Date:       2014.05.08
 * Author:     Ji
 * 
 *========================================
 * @namespace  Jisons 
 * @class      JisionsTreeView<R, T>
                where R : class, ITreeViewData<T>
                where T : class
 * @extends    JisonsScrolledWindow
 * 
 *             在此实现 UI 编辑器中的结构树，继承 JisonsScrolledWindow 内部包装了一个 TreeView 。。。
 *             用以支持 TreeView 的鼠标操作
 * 
 *========================================
 * 
 * Copyright © 迹I柳燕
 * 
 */

using Gdk;
using Gtk;
using System;

namespace CocoStudio.ToolKit
{
    public partial class JisionsTreeView<R, T>
    {

        public event EventHandler<KeyPressEventArgs> KeyDown;

        /// <summary>   </summary>
        public event EventHandler<ActionArgs> OnMouseDoubleClick;

        public void InitJisionsTreeViewOfMouseAction()
        {
            this.treeView.OnMouseDoubleClick += treeView_OnMouseDoubleClick;

            this.TreeView.KeyPressEvent += TreeView_KeyPressEvent;
        }

        [GLib.ConnectBefore]
        void TreeView_KeyPressEvent(object o, KeyPressEventArgs args)
        {
            if (this.KeyDown != null)
            {
                this.KeyDown(o, args);
            }
        }

        protected void OnMouseDoubleClickHandler(ActionArgs args)
        {
            if (this.OnMouseDoubleClick != null)
            {
                this.OnMouseDoubleClick(this, args);
            }
        }

        void treeView_OnMouseDoubleClick(object sender, WidgetEventArgs e)
        {
            var args = new ActionArgs();
            this.OnMouseDoubleClickHandler(args);
            if (!args.Handle)
            {
                var selectednodes = this.treeView.GetAllSelectedTreePaths();
                if (selectednodes != null)
                {
                    foreach (var node in selectednodes)
                    {
                        var isexpanded = this.GetViewItemExpanded(node);
                        if (isexpanded)
                        {
                            this.CollapseSingleViewItem(node);
                        }
                        else
                        {
                            this.ExpandSingleViewItem(node);
                        }
                    }
                }
            }
        }
    }
}
