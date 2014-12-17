/* 
 * 
 * FileName:   JisionsTreeViewOfDragDrop.cs
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
 *             用以支持 TreeView 的拖拽操作
 * 
 *========================================
 * 
 * Copyright © 迹I柳燕
 * 
 */

using System;
using Gtk;
using Gdk;

namespace CocoStudio.ToolKit
{

    public partial class JisionsTreeView<R, T>
    {
		private static TargetEntry[] target_tableMac = new TargetEntry[] {

			new TargetEntry ("text/uri-list", 0, 0),
			new TargetEntry ("application/x-rootwindow-drop", 0, 0)};


        /// <summary>   </summary>
        public event EventHandler<DragDropJudedArgs> OnDragBeginJudged;

        /// <summary>   </summary>
        public event EventHandler<DragDropArgs> OnDragDropOutJudged;

        /// <summary> 发送给外部的用以控制是否能执行拖拽接收的事件 </summary>
        public event EventHandler<DragDropJudedArgs> OnDragDropJudged;

        /// <summary>   </summary>
        public event EventHandler<DragDropJudedArgs> OnDragMotionJudged;

        /// <summary>   </summary>
        public event EventHandler<DragDropJudedArgs> OnDragDataReceivedJudged;

        /// <summary>   </summary>
        public event EventHandler<DragDataReceivedArgs> OnDragDataReceivedOutJudged;

        public void OnlyDragDropOut()
        {
			if (MonoDevelop.Core.Platform.IsMac)
			{
				Gtk.Drag.DestSet (this, DestDefaults.All, target_tableMac, DragAction.Copy | DragAction.Move| Gdk.DragAction.Link);
				//base.EnableModelDragDest(target_tableMac, Gdk.DragAction.Copy | Gdk.DragAction.Move | Gdk.DragAction.Link);
				//Gtk.Drag.DestSet (this, Gdk.ModifierType.Button1Mask | Gdk.ModifierType.Button3Mask, target_tableMac, Gdk.DragAction.Copy | Gdk.DragAction.Move | Gdk.DragAction.Link);

			}

            this.TreeView.OnlyDragDropOut();
        }

        public void InitJisionsTreeViewOfDragDrop()
        {
            this.TreeView.OnDragBeginJudged += TreeView_OnDragBeginJudged;
            this.TreeView.OnDragDropOutJudged += TreeView_OnDragDropOutJudged;

            this.TreeView.OnDragDataReceivedJudged += TreeView_OnDragDataReceivedJudged;
            this.TreeView.OnDragMotionJudged += TreeView_OnDragMotionJudged;

            this.TreeView.OnDragDropJudged += TreeView_OnDragDropJudged;

            this.DragDataReceived += JisionsTreeView_DragDataReceived;

			this.DragDrop += HandleDragDrop1;
        }

		void HandleDragDrop1 (object o, DragDropArgs args)
        {
        	
        }


		protected override bool OnDragDrop (Gdk.DragContext context, int x, int y, uint time_)
		{
			return base.OnDragDrop (context, x, y, time_);
		}

        protected void TreeView_OnDragBeginJudged(object sender, DragDropJudedArgs e)
        {
            if (this.OnDragBeginJudged != null)
            {
                this.OnDragBeginJudged(sender, e);
            }
        }

        protected void TreeView_OnDragDropJudged(object sender, DragDropJudedArgs e)
        {
            if (this.OnDragDropJudged != null)
            {
                this.OnDragDropJudged(sender, e);
            }
        }

        void TreeView_OnDragDropOutJudged(object sender, DragDropArgs e)
        {
            if (this.OnDragDropOutJudged != null)
            {
                this.OnDragDropOutJudged(this, e);
            }
        }

        protected void TreeView_OnDragDataReceivedJudged(object sender, DragDropJudedArgs e)
        {
            if (this.OnDragDataReceivedJudged != null)
            {
                this.OnDragDataReceivedJudged(sender, e);
            }
        }
         
        void TreeView_OnDragMotionJudged(object sender, DragDropJudedArgs e)
        {
            if (this.OnDragMotionJudged != null)
            {
                this.OnDragMotionJudged(this, e);
            }
        }

        void JisionsTreeView_DragDataReceived(object o, DragDataReceivedArgs args)
        {
            if (this.OnDragDataReceivedOutJudged != null)
            {
                this.OnDragDataReceivedOutJudged(o, args);
            }
        }


    }
}
