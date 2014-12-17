/* 
 * 
 * FileName:   DragDropJudedArgs.cs
 * Version:    1.0
 * Date:       2014.05.23
 * Author:     Ji
 * 
 *========================================
 * @namespace  Jisons 
 * @class      DragDropJudedArgs
 * @extends    EventArgs
 * 
 *             发送给外部的用来处理拖拽过程的事件参数
 * 
 *========================================
 * 
 * Copyright © 迹I柳燕
 * 
 */

using System;
using Gdk;

namespace CocoStudio.ToolKit
{
    public class DragDropJudedArgs : EventArgs
    {

        private bool handle = false;
        /// <summary> 标识事件是否继续传递 </summary>
        public bool Handle
        {
            get { return handle; }
            set { handle = value; }
        }

        private bool allDrop = true;
        /// <summary> 设置当前拖拽是否继续 </summary>
        public bool AllDrop
        {
            get { return allDrop; }
            set
            {
                allDrop = value;
            }
        }

        public DragContext DragContext { get; set; }

    }
}
