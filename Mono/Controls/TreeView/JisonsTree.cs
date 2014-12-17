/* 
 * 
 * FileName:   JisonsTree.cs
 * Version:    1.0
 * Date:       2014.05.23
 * Author:     Ji
 * 
 *========================================
 * @namespace  Jisons 
 * @class      JisonsTree<R, T> : TreeView
                where R : class, ITreeViewData<T>
                where T : class
 * @extends    TreeView
 * 
 *             包装后的 TreeView 控件， 增加了内部拖拽 、 事件触发、界面刷新 等公共
 * 
 *========================================
 * 
 * Copyright © 迹I柳燕
 * 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Gdk;
using Gtk;
using Jisons;

namespace CocoStudio.ToolKit
{
    public class JisonsTree<R, T> : TreeView
        where R : class, ITreeViewData<T>
        where T : class
    {

        #region 平台配置

        enum TargetType
        {
            String,
            RootWindow
        };

        private static TargetEntry[] target_tableMac = new TargetEntry[] {
            DragTargetType.FileDropTarget,
            DragTargetType.CocoStudioTarget};

        private static TargetEntry[] target_tableWindows = new TargetEntry[] {
         new TargetEntry ("STRING", 0, (uint) TargetType.String ),
/*         DragTargetType.FileDropTarget,*/
        DragTargetType.CocoStudioTarget};

        #endregion

        #region 属性

        /// <summary> 拖拽图像 </summary>
        public Pixbuf dragPixbuf;

        /// <summary> 显示名称 </summary>
        public string DisplayName { get; set; }

        private bool IsSelfExpanded = false;

        /// <summary> 父级容器 </summary>
        public Gtk.ScrolledWindow ScrolledWindow { get; private set; }

        /// <summary> 子项模版 </summary>
        public TreeStore TreeStore
        {
            get { return this.Model as TreeStore; }
        }

        private R rootTreeIter;
        /// <summary> 用于数据操作的根节点数据 </summary>
        public R RootTreeIter
        {
            get { return rootTreeIter; }
            private set { rootTreeIter = value; }
        }

        /// <summary> 记录树中存在的子节点 </summary>
        private ObservableCollection<JisonsTreeIter<T>> JisonsTreeIters = new ObservableCollection<JisonsTreeIter<T>>();

        /// <summary> 获取父容器的子（待定，貌似获取的自己） </summary>
        protected Gtk.TreeView Widget
        {
            get { return (Gtk.TreeView)ScrolledWindow.Child; }
        }

        /// <summary> 设置 可做为拖拽发送者 </summary>
        public void AllDrag()
        {
            Gtk.Drag.SourceSet(this, Gdk.ModifierType.Button1Mask, target_tableWindows, Gdk.DragAction.Copy | Gdk.DragAction.Move | Gdk.DragAction.Link);
        }

        /// <summary> 设置 可做为拖拽接受者 </summary>
        public void AllDrop()
        {
            base.EnableModelDragDest(target_tableWindows, Gdk.DragAction.Copy | Gdk.DragAction.Move | Gdk.DragAction.Link);
        }

        /// <summary> 设置只可由外部拖拽，不支持内部拖拽 </summary>
        public void OnlyDragDropOut()
        {
            if (MonoDevelop.Core.Platform.IsMac)
            {
                Gtk.Drag.SourceSet(this, Gdk.ModifierType.Button1Mask, target_tableMac, Gdk.DragAction.Copy | Gdk.DragAction.Move | Gdk.DragAction.Link);
            }
            else
            {
                AllDrag();
                AllDrop();
            }
        }

        #endregion

        #region 事件声明

        /// <summary> 鼠标单击事件 </summary>
        public event EventHandler<WidgetEventArgs> OnMouseButtonDown;

        /// <summary> 鼠标双击事件 </summary>
        public event EventHandler<WidgetEventArgs> OnMouseDoubleClick;

        /// <summary> 选中更改事件 </summary>
        public event EventHandler SelectionChanged;

        /// <summary> 发送给外部的拖拽开始事件，此项具有控制后续执行的功能 </summary>
        public event EventHandler<DragDropJudedArgs> OnDragBeginJudged;

        /// <summary> 发送给外部的拖拽接收事件，在此内需要执行 GetData 用以支持外部拖拽
        /// Gtk.Drag.GetData(this.treeView, e.Context, e.Context.Targets.FirstOrDefault(), e.Time); </summary>
        public event EventHandler<DragDropArgs> OnDragDropOutJudged;

        /// <summary> 发送给外部的用以控制是否能执行拖拽接收的事件 </summary>
        public event EventHandler<DragDropJudedArgs> OnDragMotionJudged;

        /// <summary> 发送给外部的用以控制是否能执行拖拽接收的事件 </summary>
        public event EventHandler<DragDropJudedArgs> OnDragDropJudged;

        /// <summary> 发送给外部的拖拽真正能接收数据的事件 </summary>
        public event EventHandler<DragDropJudedArgs> OnDragDataReceivedJudged;

        #endregion

        #region 类声明

        /// <summary> 构造函数 </summary>
        /// <param name="scrolledwindow"></param>
        /// <param name="treestore"></param>
        public JisonsTree(ScrolledWindow scrolledwindow, R rootdata, TreeStore treestore)
            : base(treestore)
        {
            this.HeadersVisible = false;

            this.ScrolledWindow = scrolledwindow;
            this.WidgetEvent += JisonsTree_WidgetEvent;

            ReSetRootViewData(rootdata);

            this.Selection.Changed += (sender, e) => OnSelectionChangedHandle(sender, e);

            this.JisonsTreeIters.CollectionChanged += JisonsTreeIters_CollectionChanged;

            this.DragDrop += JisonsTree_DragDrop;

            this.RowExpanded += HandleRowExpanded;
            this.RowCollapsed += JisonsTree_RowCollapsed;
        }

        void JisonsTree_RowCollapsed(object o, RowCollapsedArgs args)
        {
            var jti = this.GetJisonsTreeIter(this.GetTreeIter(args.Path));
            if (jti != null)
            {
                this.IsSelfExpanded = true;
                this.RootTreeIter.SetExpanded(jti.Data, this.GetRowExpanded(args.Path));
                IsSelfExpanded = false;
            }
        }

        void HandleRowExpanded(object o, RowExpandedArgs args)
        {
            var jti = this.GetJisonsTreeIter(this.GetTreeIter(args.Path));
            if (jti != null)
            {
                this.IsSelfExpanded = true;
                this.RootTreeIter.SetExpanded(jti.Data, this.GetRowExpanded(args.Path));
                IsSelfExpanded = false;
            }
        }

        void JisonsTree_DragDrop(object o, DragDropArgs args)
        {
            OnDragDropJudgedHandle(o, args);
        }

        public void ReSetRootViewData(R rootdata)
        {
            this.JisonsTreeIters.DeleteEachItem();

            this.DeleteAllItems();
            if (rootdata != null)
            {
                this.rootTreeIter = rootdata;

                CreateRootViewTreeItem(this.rootTreeIter);
            }
        }

        #endregion

        #region 双向事件通知

        void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {

                case "IsSelected":
                    {
                        if (!IsSelfSelected)
                        {
                            var jti = this.GetJisonsTreeIter((T)sender);
                            if (jti != null)
                            {
                                IsSelfSelected = true;
                                if (this.RootTreeIter.GetIsSelected(jti.Data))
                                {
                                    this.SelectTreeIter(jti.TreeIter);
                                }
                                else
                                {
                                    this.UnSelectTreeIter(jti.TreeIter);
                                }
                                IsSelfSelected = false;
                            }
                        }
                        break;
                    }

                case "IsExpanded":
                    {
                        if (!IsSelfExpanded)
                        {
                            var jti = this.GetJisonsTreeIter((T)sender);
                            if (jti != null)
                            {
                                IsSelfExpanded = true;
                                var treepath = this.GetTreePath(jti.TreeIter);
                                var isexpanded = this.RootTreeIter.GetExpanded((T)sender);
                                if (isexpanded)
                                {
                                    this.ExpandToPath(treepath);
                                }
                                else
                                {
                                    this.CollapseRow(treepath);
                                }
                                IsSelfExpanded = false;
                            }
                        }
                        break;
                    }

                case "Name":
                    {
                        var jti = this.GetJisonsTreeIter((T)sender);
                        if (jti != null)
                        {
                            this.TreeStore.SetValues(jti.TreeIter, this.RootTreeIter.GetData(jti.Data).Values);
                        }

                        break;
                    }

                default: break;
            }
        }

        void JisonsTreeIters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewItems != null)
                        {
                            foreach (JisonsTreeIter<T> item in e.NewItems)
                            {
                                if (item != null)
                                {
                                    this.RootTreeIter.AddPropertyChangedEventHandler(item.Data, Data_PropertyChanged);

                                    var childcollection = this.RootTreeIter.GetTreeViewChildren(item.Data);
                                    if (childcollection != null)
                                    {
                                        childcollection.CollectionChanged += TreeViewChildren_CollectionChanged;
                                        //childcollection.AddCollectionEventHandler<ObservableCollection<T>>(TreeViewChildren_CollectionChanged);//.CollectionChanged += TreeViewChildren_CollectionChanged;
                                    }
                                }
                            }
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.OldItems != null)
                        {
                            foreach (JisonsTreeIter<T> item in e.OldItems)
                            {
                                if (item != null)
                                {
                                    this.RootTreeIter.RemovePropertyChangedEventHandler(item.Data, Data_PropertyChanged);

                                    this.DeleteEventHandle(item.Data);
                                }
                            }
                        }

                        break;
                    }

                default: break;
            }
        }

        void TreeViewChildren_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewItems != null)
                        {
                            foreach (T item in e.NewItems)
                            {
                                if (item != null)
                                {
                                    var parent = this.RootTreeIter.GetParentPart(item);
                                    if (parent != null)
                                    {
                                        var parentjti = this.GetJisonsTreeIter(parent);
                                        this.AddViewItem(parentjti.TreeIter, item, e.NewStartingIndex);
                                    }

                                    var children = this.RootTreeIter.GetTreeViewChildren(item);
                                    if (children != null && children.Count > 0)
                                    {
                                        foreach (var child in this.RootTreeIter.GetTreeViewChildren(item))
                                        {
                                            CreateViewTreeItem(this.GetJisonsTreeIter(item).TreeIter, child);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.OldItems != null)
                        {
                            foreach (T item in e.OldItems)
                            {
                                if (item != null)
                                {
                                    this.DeleteJisonsTreeIter(this.GetJisonsTreeIter(item));
                                }
                            }
                        }

                        break;
                    }

                default: break;
            }
        }

        #endregion

        #region 事件处理

        uint? firstClickTime = null;
        double x = -1;
        double y = -1;

        /// <summary> 事件转发处理 </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        void JisonsTree_WidgetEvent(object o, WidgetEventArgs args)
        {
            switch (args.Event.Type)
            {
                case EventType.ButtonPress:
                    {
                        bool isdoubleclick = false;
                        var eventbutton = ((EventButton)args.Event);
                        isdoubleclick = eventbutton.IsDoubleClick();
                        if (isdoubleclick)
                        {
                            OnMouseDoubleClickHandle(args);
                            MouseDoubleClick((EventButton)args.Event);
                        }
                        else
                        {
                            OnMouseButtonDownHandle(args);
                            MouseButtonDown((EventButton)args.Event);
                        }

                        break;
                    }

                default: break;
            }
        }

        #endregion

        #region 鼠标事件

        protected void OnMouseButtonDownHandle(WidgetEventArgs args)
        {
            if (this.OnMouseButtonDown != null)
            {
                this.OnMouseButtonDown(this, args);
            }
        }

        protected void OnMouseDoubleClickHandle(WidgetEventArgs args)
        {
            if (this.OnMouseDoubleClick != null)
            {
                this.OnMouseDoubleClick(this, args);
            }
        }

        public virtual void MouseButtonDown(EventButton eventButton)
        { }

        public virtual void MouseDoubleClick(EventButton eventButton)
        { }

        #endregion

        #region 拖拽事件

        protected void OnDragBeginJudgedHandle(DragDropJudedArgs args)
        {
            if (this.OnDragBeginJudged != null)
            {
                this.OnDragBeginJudged(this, args);
            }
        }

        protected void OnDragDropJudgedHandle(object o, DragDropArgs args)
        {
            if (this.OnDragDropOutJudged != null)
            {
                this.OnDragDropOutJudged(o, args);
            }
        }

        protected void OnOnDragDataReceivedJudgedHandle(DragDropJudedArgs args)
        {
            if (this.OnDragDataReceivedJudged != null)
            {
                this.OnDragDataReceivedJudged(this, args);
            }
        }

        protected void OnOnDragMotionJudgedHandle(DragDropJudedArgs args)
        {
            if (this.OnDragMotionJudged != null)
            {
                this.OnDragMotionJudged(this, args);
            }
        }

        protected void OnOnDragDropJudgedHandle(DragDropJudedArgs args)
        {
            if (this.OnDragDropJudged != null)
            {
                this.OnDragDropJudged(this, args);
            }
        }

        /// <summary> 拖拽开始执行的事件
        /// 做为数据发送者时触发 </summary>
        /// <param name="context"></param>
        protected override void OnDragBegin(DragContext context)
        {
            DragDropJudedArgs args = new DragDropJudedArgs() { DragContext = context };
            OnDragBeginJudgedHandle(args);
            if (!args.Handle)
            {
                var selectviewdatas = new List<T>();
                var selecttreeiters = this.GetAllSelectedTreeIters();
                foreach (var item in selecttreeiters)
                {
                    var isselectparent = selecttreeiters.FirstOrDefault(i => this.IsParent(item, i));
                    if (isselectparent.Equals(TreeIter.Zero))
                    {
                        selectviewdatas.Add(this.GetJisonsTreeIter(item).Data);
                    }
                }

                var dragData = new DragData()
                {
                    SelectionDatas = selectviewdatas,
                    IsDo = false
                };

                context.SetDragData(dragData);

                //Gtk.Drag.SetIconPixbuf(context, dragPixbuf, 0, 0);

                base.OnDragBegin(context);
            }
        }

        protected override bool OnDragMotion(DragContext context, int x, int y, uint time_)
        {
            DragDropJudedArgs args = new DragDropJudedArgs() { DragContext = context };
            OnOnDragMotionJudgedHandle(args);
            if (!args.Handle)
            {
                var dragwidget = context.GetSourceWidget();
                if (dragwidget != null)
                {
                    var dragData = context.GetDragData() as DragData;
                    //内部拖拽
                    if (dragData != null)
                    {
                        return base.OnDragMotion(context, x, y, time_);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //此时拖拽为外部拖拽
                    return true;
                }
            }
            else
            {
                if (args.AllDrop)
                {
                    return base.OnDragMotion(context, x, y, time_);
                }
                else
                {
                    return false;
                }
            }
        }

        protected override bool OnDragDrop(DragContext context, int x, int y, uint time_)
        {
            DragDropJudedArgs args = new DragDropJudedArgs() { DragContext = context };
            OnOnDragDropJudgedHandle(args);
            if (!args.Handle)
            {
                return base.OnDragDrop(context, x, y, time_);
            }
            else
            {
                return false;
            }
        }

        /// <summary> 接收具有拖拽数据的事件 
        /// 做为数据接收者时触发 </summary> 
        /// <param name="context"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="selection_data"></param>
        /// <param name="info"></param>
        /// <param name="time_"></param>
        protected override void OnDragDataReceived(DragContext context, int x, int y, SelectionData selection_data, uint info, uint time_)
        {
            DragDropJudedArgs args = new DragDropJudedArgs() { DragContext = context };
            OnOnDragDataReceivedJudgedHandle(args);
            if (!args.Handle)
            {
                base.OnDragDataReceived(context, x, y, selection_data, info, time_);
                var dragData = context.GetDragData() as DragData;
                //内部拖拽
                if (dragData != null)
                {
                    Gtk.TreePath treePath; Gtk.TreeViewDropPosition dropPostition;
                    var isDrop = GetDropTargetRow(x, y, out treePath, out dropPostition);

                    #region 结构树内数据项之间拖拽

                    if (isDrop)
                    {
                        dragData.IsDo = true;
                        var iters = dragData.SelectionDatas as List<T>;
                        var treeiterparent = this.GetTreeIter(treePath);
                        if (iters != null)
                        {
                            var parentjti = this.GetJisonsTreeIter(treeiterparent);
                            if (!iters.Contains(parentjti.Data))
                            {
                                switch (dropPostition)
                                {
                                    case TreeViewDropPosition.Before:
                                        {
                                            if (iters.All(i => this.RootTreeIter.CanDrop(parentjti.Data, i as object, DropPosition.InsertBefore, false)))
                                            {
                                                this.RootTreeIter.Drop(parentjti.Data, iters.Cast<object>().ToList(), DropPosition.InsertBefore, false);
                                            }
                                            break;
                                        }
                                    case TreeViewDropPosition.After:
                                        {
                                            if (iters.All(i => this.RootTreeIter.CanDrop(parentjti.Data, i as object, DropPosition.InsertAfter, false)))
                                            {
                                                this.RootTreeIter.Drop(parentjti.Data, iters.Cast<object>().ToList(), DropPosition.InsertAfter, false);
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            if (!iters.Contains(parentjti.Data) && iters.All(i => this.RootTreeIter.CanDrop(parentjti.Data, i as object, DropPosition.Add, false)))
                                            {
                                                this.RootTreeIter.Drop(parentjti.Data, iters.Cast<object>().ToList(), DropPosition.Add, false);
                                            }
                                            break;
                                        }
                                }
                            }
                        }
                    }

                    #endregion

                    #region 数据拖动到结构树数据项之外时触发

                    else if (treePath == null)
                    {

                        dragData.IsDo = true;
                        var iters = dragData.SelectionDatas as List<T>;
                        if (iters != null)
                        {
                            foreach (var item in iters)
                            {
                                var jti = this.GetJisonsTreeIter(item);
                                if (this.JisonsTreeIters.Contains(jti) &&
                                    this.GetParentTreeIter(jti.TreeIter).Equals(TreeIter.Zero))
                                {
                                    continue;
                                }

                                if (item != null)
                                {
                                    if (this.RootTreeIter != null && this.RootTreeIter.FirstDepthChildren.FirstOrDefault() != null)
                                    {
                                        this.RootTreeIter.Drop(this.RootTreeIter.FirstDepthChildren.FirstOrDefault(), iters.Cast<object>().ToList(), DropPosition.Add, false);
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                }
            }
        }

        #endregion

        #region 数据操作

        public JisonsTreeIter<T> AddViewItem(TreeIter parent, T data, int position = -1)
        {
            if (!this.RootTreeIter.CanAddViewItem(data))
            {
                return null;
            }

            var insertiter = parent.Equals(TreeIter.Zero) ? this.TreeStore.AppendNode() : this.TreeStore.InsertNode(parent, position);
            var jiter = new JisonsTreeIter<T>(data, insertiter);
            this.TreeStore.SetValues(insertiter, this.RootTreeIter.GetData(data).Values);

            JisonsTreeIters.Add(jiter);


            var expand = this.GetParentExpand(data);
            if (expand)
            {
                // 展开父级
                var jtiparent = this.RootTreeIter.GetParentPart(data);
                if (jtiparent != null)
                {
                    bool isexpanded = this.RootTreeIter.GetExpanded(jtiparent);
                    if (isexpanded)
                    {
                        this.ExpandToPath(this.GetTreePath(jiter.TreeIter));
                    }
                }
            }

            //展开子集
            //bool isexpanded = this.RootTreeIter.GetExpanded(jiter.Data);
            //if (isexpanded)
            //{
            //    this.ExpandToPath(this.GetTreePath(jiter.TreeIter));
            //}

            return jiter;

        }

        private bool GetParentExpand(T data)
        {
            var jtiparent = this.RootTreeIter.GetParentPart(data);
            if (jtiparent != null)
            {
                bool isexpanded = this.RootTreeIter.GetExpanded(jtiparent);
                if (isexpanded)
                {
                    var jtiparentTemp = this.RootTreeIter.GetParentPart(jtiparent);
                    if (jtiparentTemp != null)
                    {
                        return GetParentExpand(jtiparent);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void AddJisonsTreeIterToParent(JisonsTreeIter<T> jiter)
        {
            var parentjisonstree = GetParentJisonsTreeIter(jiter);
            if (parentjisonstree != null && RootTreeIter.GetTreeViewChildren(parentjisonstree.Data) != null &&
               this.RootTreeIter.GetTreeViewChildren(parentjisonstree.Data).FirstOrDefault(i => this.RootTreeIter.GetTreeViewChildren(i).Equals(this.RootTreeIter.GetTreeViewChildren(jiter.Data))) == null)
            {
                this.RootTreeIter.GetTreeViewChildren(parentjisonstree.Data).Add(jiter.Data);
            }
        }

        public JisonsTreeIter<T> InsertBeforeViewItem(TreeIter inserttreeiter, T data)
        {
            int position = inserttreeiter.Equals(TreeIter.Zero) ? -1 : this.TreeStore.GetPath(inserttreeiter).Indices.LastOrDefault();
            var parenttreeiter = this.GetParentTreeIter(inserttreeiter);
            return AddViewItem(parenttreeiter, data, position);
        }

        public JisonsTreeIter<T> InsertAfterViewItem(TreeIter inserttreeiter, T data)
        {
            int position = inserttreeiter.Equals(TreeIter.Zero) ? -1 : this.TreeStore.GetPath(inserttreeiter).Indices.LastOrDefault() + 1;
            var parenttreeiter = this.GetParentTreeIter(inserttreeiter);
            return AddViewItem(parenttreeiter, data, position);
        }

        public bool DeleteViewItem(TreeIter treeiter)
        {
            return this.DeleteJisonsTreeIter(this.GetJisonsTreeIter(treeiter));
        }

        public bool DeleteViewItem(TreePath treepath)
        {
            return this.DeleteViewItem(this.GetTreeIter(treepath));
        }

        public bool DeleteJisonsTreeIter(JisonsTreeIter<T> jiter)
        {
            if (JisonsTreeIters.Contains(jiter))
            {
                var items = GetAllChilerenTreeIters(jiter);
                foreach (var item in items)
                {
                    if (item != null)
                    {
                        JisonsTreeIters.Remove(item);
                    }
                }

                var deletetreeiter = jiter.TreeIter;
                return this.TreeStore.Remove(ref deletetreeiter);
            }
            return false;
        }

        public void DeleteAllItems()
        {
            this.JisonsTreeIters.DeleteEachItem();
            this.TreeStore.Clear();
        }

        public JisonsTreeIter<T> GetParentJisonsTreeIter(JisonsTreeIter<T> jti)
        {
            var parent = this.GetParentTreeIter(jti.TreeIter);
            return this.GetJisonsTreeIter(parent);
        }

        #endregion

        #region 数据显示

        /// <summary> 创建根数据 </summary>
        /// <param name="rootviewdata"></param>
        private void CreateRootViewTreeItem(R roottreeiter)
        {
            //根节点只作为数据处理
            foreach (var item in roottreeiter.FirstDepthChildren)
            {
                CreateViewTreeItem(TreeIter.Zero, item);
            }
        }

        /// <summary> 创建子项模版 </summary>
        /// <param name="parent"></param>
        /// <param name="viewdata"></param>
        /// <param name="iscreatself"></param>
        private void CreateViewTreeItem(TreeIter parent, T viewdata, JisonsTreeIter<T> jiterself = null)
        {
            if (jiterself == null)
            {
                jiterself = this.AddViewItem(parent, viewdata);
            }

            if (jiterself != null && this.RootTreeIter.GetTreeViewChildren(viewdata) != null)
            {
                foreach (var childitem in this.RootTreeIter.GetTreeViewChildren(viewdata))
                {
                    if (childitem != null)
                    {
                        CreateViewTreeItem(jiterself.TreeIter, childitem);
                    }
                }
            }
        }

        #endregion

        #region 事件移除处理

        /// <summary> 移出绑定的子项列表更改事件 </summary>
        /// <param name="parent"></param>
        /// <param name="viewdata"></param>
        /// <param name="iscreatself"></param>
        private void DeleteEventHandle(T viewdata)
        {
            if (viewdata != null)
            {
                var children = this.RootTreeIter.GetTreeViewChildren(viewdata);
                if (children != null)
                {
                    children.CollectionChanged -= TreeViewChildren_CollectionChanged;
                    foreach (var childitem in children)
                    {
                        if (childitem != null)
                        {
                            DeleteEventHandle(childitem);
                        }
                    }
                }
            }
        }

        #endregion

        #region 获取数据

        public bool GetDropTargetRow(double x, double y, out Gtk.TreePath treePath, out Gtk.TreeViewDropPosition dropPostition)
        {
            return Widget.GetDestRowAtPos((int)x, (int)y, out treePath, out dropPostition);
        }

        public List<T> GetSelectViewData()
        {
            List<T> selectviewdatas = new List<T>();
            GetSelectedTreeIters().ForEach(i => selectviewdatas.Add(this.GetJisonsTreeIter(i).Data));
            return selectviewdatas;
        }

        public List<T> GetAllSelectViewData()
        {
            List<T> selectviewdatas = new List<T>();
            GetAllSelectedTreeIters().ForEach(i =>
              {
                  var jti = this.GetJisonsTreeIter(i);
                  if (jti != null)
                  {
                      selectviewdatas.Add(jti.Data);
                  }
              });
            return selectviewdatas;
        }

        /// <summary> 获取所有选中的 TreePath </summary>
        public List<TreePath> GetAllSelectedTreePaths()
        {
            return this.Selection.GetSelectedRows().ToList();
        }

        /// <summary> 获取经过筛选后过滤子级的所有选中项 TreePath </summary>
        public List<TreePath> GetSelectedTreePaths()
        {
            List<TreePath> selecttreepathes = new List<TreePath>();
            GetSelectedTreeIters().ForEach(i => selecttreepathes.Add(this.GetTreePath(i)));
            return selecttreepathes;
        }

        /// <summary> 获取所有选中的 TreeIters </summary>
        public List<TreeIter> GetAllSelectedTreeIters()
        {
            var selecttreepaths = GetAllSelectedTreePaths();
            List<TreeIter> treeiters = new List<TreeIter>();
            TreeIter iter;
            foreach (var item in selecttreepaths)
            {
                if (this.Model.GetIter(out iter, item))
                {
                    treeiters.Add(iter);
                }
            }
            return treeiters;
        }

        /// <summary> 获取经过筛选后过滤子级的所有选中项 TreeIter </summary>
        public List<TreeIter> GetSelectedTreeIters()
        {
            var selectviewdatas = new List<TreeIter>();
            var selecttreeiters = this.GetAllSelectedTreeIters();
            foreach (var item in selecttreeiters)
            {
                var isselectparent = selecttreeiters.FirstOrDefault(i => this.IsParent(item, i));
                if (isselectparent.Equals(TreeIter.Zero))
                {
                    selectviewdatas.Add(item);
                }
            }
            return selectviewdatas;
        }

        public TreeIter GetTreeIter(TreePath path)
        {
            TreeIter iter = TreeIter.Zero;
            this.TreeStore.GetIter(out iter, path);
            return iter;
        }

        public TreePath GetTreePath(TreeIter iter)
        {
            return this.TreeStore.GetPath(iter);
        }

        public T GetViewData(TreeIter iter)
        {
            var jti = this.GetJisonsTreeIter(iter);
            return jti != null ? jti.Data : default(T);
        }

        public JisonsTreeIter<T> GetJisonsTreeIter(TreeIter iter)
        {
            return JisonsTreeIters.FirstOrDefault(i => i.TreeIter.Equals(iter));
        }

        public JisonsTreeIter<T> GetJisonsTreeIter(T data)
        {
            return JisonsTreeIters.FirstOrDefault(i => i.Data.Equals(data));
        }

        public TreeIter GetParentTreeIter(TreeIter iter)
        {
            TreeIter parent = TreeIter.Zero;
            this.TreeStore.IterParent(out parent, iter);
            return parent;
        }

        /// <summary> 获取指定节点所具有的所有子级数据 JisonsTreeIter<T> </summary>
        /// <param name="jiter"></param>
        /// <returns></returns>
        public IEnumerable<JisonsTreeIter<T>> GetAllChilerenTreeIters(JisonsTreeIter<T> jiter)
        {
            var children = this.RootTreeIter.GetTreeViewChildren(jiter.Data);
            if (children != null && children.Count != 0)
            {
                foreach (var item in this.RootTreeIter.GetTreeViewChildren(jiter.Data))
                {
                    foreach (var jti in GetAllChilerenTreeIters(this.GetJisonsTreeIter(item)))
                    {
                        yield return jti;
                    }
                }
            }

            yield return jiter;
        }

        /// <summary> 获取指定节点所具有的所有子级数据 T<T> </summary>
        public IEnumerable<T> GetAllChilerenViewData(T viewdata)
        {
            if (this.RootTreeIter.GetTreeViewChildren(viewdata).Count != 0)
            {
                foreach (var item in this.RootTreeIter.GetTreeViewChildren(viewdata))
                {
                    foreach (var child in GetAllChilerenViewData(item))
                    {
                        yield return child;
                    }
                }
            }

            yield return viewdata;
        }

        /// <summary> 判断是否是其某一级父节点 </summary>
        /// <param name="iter1"></param>
        /// <param name="iter2"></param>
        /// <returns></returns>
        public bool IsParent(TreeIter iter1, TreeIter iter2)
        {
            TreeIter parent = iter1;
            while (!parent.Equals(TreeIter.Zero))
            {
                parent = GetParentTreeIter(parent);
                if (!parent.Equals(TreeIter.Zero) && parent.Equals(iter2))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 选中事件

        private bool IsSelfSelected = false;

        /// <summary> 通知外部有选中项更改 </summary>
        protected void OnSelectionChangedHandle(object sender, EventArgs e)
        {
            if (this.SelectionChanged != null && !IsSelfSelected)
            {
                this.SelectionChanged(sender, e);
            }
        }

        /// <summary> 否决的 此项不一定完全可用 </summary>
        /// <param name="iter"></param>
        /// <returns></returns>
        public bool IsSelected(TreeIter iter)
        {
            TreeIter judgeiter;
            if (this.Selection.GetSelected(out judgeiter))
            {
                return judgeiter.Equals(iter);
            }
            return false;
        }

        public void SelectTreeIter(TreeIter iter)
        {
            this.Selection.SelectIter(iter);
        }

        public void SelectTreePath(TreePath path)
        {
            this.Selection.SelectPath(path);
        }

        public new void SelectAll()
        {
            this.Selection.SelectAll();
        }

        public void UnSelectTreeIter(TreeIter iter)
        {
            this.Selection.UnselectIter(iter);
        }

        public void UnSelectTreePath(TreePath path)
        {
            this.Selection.UnselectPath(path);
        }

        public new void UnselectAll()
        {
            this.Selection.UnselectAll();
        }

        #endregion

        #region 屏蔽事件

        [GLib.ConnectBefore]
        protected override bool OnKeyPressEvent(EventKey evnt)
        {

            //屏蔽 BackSpace 选中父级
            if (evnt.Key.Equals(Gdk.Key.BackSpace))
            {
                return false;
            }

            return base.OnKeyPressEvent(evnt);
        }

        [GLib.ConnectBefore]
        protected override bool OnButtonPressEvent(EventButton evnt)
        {

            // 屏蔽右键改变选中
            if (evnt.Button == 3)
            {
                return false;
            }

            return base.OnButtonPressEvent(evnt);
        }

        #endregion

    }
}
