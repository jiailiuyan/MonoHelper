/* 
 * 
 * FileName:   JisionsTreeViewOfExpanad.cs
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
 *             用以支持 TreeView 的展开操作
 * 
 *========================================
 * 
 * Copyright © 迹I柳燕
 * 
 */

using Gtk;

namespace CocoStudio.ToolKit
{

    public partial class JisionsTreeView<R, T>
    {

        /// <summary> 加载展开控制 </summary>
        public void InitVisualObjectTreeViewOfExpanad()
        { }

        /// <summary> 展开所有项 </summary>
        public void ExpanadAllViewItem()
        {
            this.treeView.ExpandAll();
        }

        /// <summary> 只展开当前项 </summary>
        /// <param name="treepath"></param>
        public void ExpandSingleViewItem(TreePath treepath)
        {
            this.treeView.ExpandToPath(treepath);
        }

        public void ExpandSingleViewItem(T itemdata)
        {
            DoActionOfJisonsTreeIter(itemdata, (jti) => this.treeView.ExpandToPath(this.treeView.GetTreePath(jti.TreeIter)));
        }

        /// <summary> 展开当前和其所有的子项 </summary>
        /// <param name="treepath"></param>
        /// <returns></returns>
        public bool ExpandSingleAndChildrenViewItem(TreePath treepath)
        {
            return this.treeView.ExpandRow(treepath, true);
        }

        /// <summary> 关闭所有的展开项 </summary>
        public void CollapseAllViewItem()
        {
            this.treeView.CollapseAll();
        }

        /// <summary> 否决的，暂时未正真实现，其操作现在同于 CollapseSingleAndChildrenViewItem </summary>
        /// <param name="treepath"></param>
        public void CollapseSingleViewItem(TreePath treepath)
        {
            CollapseSingleAndChildrenViewItem(treepath);
        }

        /// <summary> 关闭当前和其所有的子项的展开 </summary>
        /// <param name="treepath"></param>
        public void CollapseSingleAndChildrenViewItem(TreePath treepath)
        {
            this.treeView.CollapseRow(treepath);
        }

        /// <summary> 关闭当前和其所有的子项的展开 </summary>
        /// <param name="treepath"></param>
        public void CollapseSingleAndChildrenViewItem(T itemdata)
        {
            DoActionOfJisonsTreeIter(itemdata, (jti) => this.treeView.CollapseRow(this.treeView.GetTreePath(jti.TreeIter)));
        }

        private void DoActionOfJisonsTreeIter(T itemdata, System.Action<JisonsTreeIter<T>> action)
        {
            var jti = this.treeView.GetJisonsTreeIter(itemdata);
            if (jti != null)
            {
                action.Invoke(jti);
            }
        }

        /// <summary> 获取指定项是否为展开状态 </summary>
        /// <param name="treepath"></param>
        /// <returns></returns>
        public bool GetViewItemExpanded(TreePath treepath)
        {
            return this.treeView.GetRowExpanded(treepath);
        }

    }
}
