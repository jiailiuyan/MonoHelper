/* 
 * 
 * FileName:   JisonsTreeIter.cs
 * Version:    1.0
 * Date:       2014.05.23
 * Author:     Ji
 * 
 *========================================
 * @namespace  Jisons 
 * @class      JisonsTreeIter<T> where T : class
 * @extends    
 * 
 *             对 JisonsTree 进行了数据项包装，用以扩展 外部数据和 TreeIter 的交互
 * 
 *========================================
 * 
 * Copyright © 迹I柳燕
 * 
 */

using Gtk;

namespace CocoStudio.ToolKit
{
    public class JisonsTreeIter<T> where T : class
    {

        public TreeIter TreeIter { get; private set; }

        public T Data { get; set; }

        public JisonsTreeIter(T data, TreeIter treeiter)
        {
            this.TreeIter = treeiter;
            this.Data = data;
        }
    }

    public static class JisonsTreeIterHelper
    {
        public static JisonsTreeIter<R> CreatDefault<R, T>(this R itvd)
            where R : class, ITreeViewData<T>
            where T : class
        {
            return new JisonsTreeIter<R>(itvd, TreeIter.Zero);
        }
    }
}
