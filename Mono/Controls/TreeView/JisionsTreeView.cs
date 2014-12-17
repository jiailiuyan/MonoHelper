/* 
 * 
 * FileName:   JisionsTreeView.cs
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
 *             在 Gtk 中，TreeView 果然是万能的 。。。
 *             需要扩展 Drag ， Selections 事件即可 。。。
 *             Ps 。 此项默认支持多选 设置 SelectionMode 。。。
 *             
 * 
 *========================================
 * 
 * Copyright © 迹I柳燕
 * 
 */
using Gtk;

namespace CocoStudio.ToolKit
{

    public partial class JisionsTreeView<R, T> : JisonsScrolledWindow
    {
        private JisonsTree<R, T> treeView;
        public JisonsTree<R, T> TreeView
        {
            get { return treeView; }
            private set { treeView = value; }
        }

        public string DisplayName
        {
            get
            {
                if (this.treeView != null)
                {
                    return this.treeView.DisplayName;
                }
                return null;
            }
            set
            {
                if (this.treeView != null)
                {
                    this.treeView.DisplayName = value;
                }
            }
        }

        protected JisionsTreeView()
        { }

        /// <summary> 构造函数，默认为多选树 </summary>
        /// <param name="selectionMode"></param>
        public JisionsTreeView(R rootdata)
        {
            InitJisionsTreeViewOfTree(rootdata, Gtk.SelectionMode.Multiple);
            InitVisualObjectTreeViewOfExpanad();
            InitJisionsTreeViewOfData();
            InitJisionsTreeViewOfMouseAction();
            InitJisionsTreeViewOfDragDrop();

            this.treeView.Show();

            this.Add(this.TreeView);

            this.ShowAll();
        }

        void HandleDragDrop(object o, Gtk.DragDropArgs args)
        {

        }

        private bool IsClickedNodeSelected(int x, int y)
        {
            if (treeView != null)
            {
                Gtk.TreePath path;
                if (treeView.GetPathAtPos(x, y, out path))
                {
                    return treeView.Selection.PathIsSelected(path);
                }
            }
            return false;
        }

        public bool MultipleNodesSelected()
        {
            return treeView != null && treeView.Selection.GetSelectedRows().Length > 1;
        }

    }
}
