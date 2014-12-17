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
 *             用以支持 TreeView 的数据操作
 * 
 *========================================
 * 
 * Copyright © 迹I柳燕
 * 
 */

using Gtk;
using System;
using System.Collections.Generic;

namespace CocoStudio.ToolKit
{

    public partial class JisionsTreeView<R, T>
    {

        /// <summary> 在此屏蔽内部的添加删除触发，只处理对外的数据更改事件 </summary>
        private bool IsDoSelf = false;

        private void InitJisionsTreeViewOfData()
        {
            this.TreeView.SelectionChanged += TreeView_SelectionChanged;
        }

        #region 界面数据操作

        public JisonsTreeIter<T> AddTreeViewItem(T parent, T data)
        {
            return this.AddTreeViewItem(this.TreeView.GetJisonsTreeIter(parent), data);
        }

        public JisonsTreeIter<T> AddTreeViewItem(JisonsTreeIter<T> parent, T data)
        {
            this.IsDoSelf = true;
            var jti = this.TreeView.AddViewItem(parent.TreeIter, data);
            this.IsDoSelf = false;
            return jti;
        }

        public void DeleteTreeViewItem(T data)
        {
            this.IsDoSelf = true;
            this.TreeView.DeleteJisonsTreeIter(this.TreeView.GetJisonsTreeIter(data));
            this.IsDoSelf = false;
        }

        public void DeleteTreeViewItem(JisonsTreeIter<T> jti)
        {
            this.IsDoSelf = true;
            this.TreeView.DeleteViewItem(jti.TreeIter);
            this.IsDoSelf = false;
        }

        public void DeleteAllItems()
        {
            this.IsDoSelf = true;
            this.treeView.DeleteAllItems();
            this.IsDoSelf = false;
        }

        public void RefreshTreeView(R rootview = null)
        {
            if (rootview != null)
            {
                this.RootViewData = rootview;
            }

            this.treeView.ReSetRootViewData(this.RootViewData);
        }

        #endregion

        #region 选中事件处理

        /// <summary> 屏蔽内部自己的选中事件标识 </summary>
        private bool IsSelfSelcted = false;

        /// <summary> 通知外部有选中项更改 </summary>
        protected void OnSelectionChangedHandle()
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, EventArgs.Empty);
            }
        }

        /// <summary> 在此屏蔽内部自己的选中事件 </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TreeView_SelectionChanged(object sender, EventArgs e)
        {
            if (!IsSelfSelcted)
            {
                OnSelectionChangedHandle();
            }
        }

        /// <summary> 控制选中项在视图中可见 </summary>
        /// <param name="path"></param>
        /// <param name="column"></param>
        /// <param name="use_align"></param>
        /// <param name="row_align"></param>
        /// <param name="col_align"></param>
        public void BringToView(TreeIter iter, TreeViewColumn column, bool use_align = true, float row_align = 0, float col_align = 0)
        {
            var treepath = this.treeView.GetTreePath(iter);
            if (treepath != null)
            {
                this.treeView.ScrollToCell(treepath, column, use_align, row_align, col_align);
            }
        }

        public void SelectTreeViewItem(T data)
        {
            this.IsSelfSelcted = true;

            var jti = this.TreeView.GetJisonsTreeIter(data);
            if (jti != null)
            {
                this.TreeView.SelectTreeIter(jti.TreeIter);
            }

            this.IsSelfSelcted = false;
        }

        public void SelectAll()
        {
            this.IsSelfSelcted = true;
            this.TreeView.SelectAll();
            this.IsSelfSelcted = false;
        }

        public void UnSelectTreeViewItem(T data)
        {
            this.IsSelfSelcted = true;
            this.TreeView.UnSelectTreeIter(this.TreeView.GetJisonsTreeIter(data).TreeIter);
            this.IsSelfSelcted = false;
        }

        public void UnselectAll()
        {
            this.IsSelfSelcted = true;
            this.TreeView.UnselectAll();
            this.IsSelfSelcted = false;
        }

        public List<T> GetSelectViewData()
        {
            return this.treeView.GetSelectViewData();
        }

        public List<T> GetAllSelectViewData()
        {
            return this.treeView.GetAllSelectViewData();
        }

        #endregion

        #region 数据获取

        public JisonsTreeIter<T> GetJisonsTreeIter(T data)
        {
            return this.TreeView.GetJisonsTreeIter(data);
        }

        #endregion

    }
}
