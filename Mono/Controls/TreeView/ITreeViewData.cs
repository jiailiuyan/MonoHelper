/* 
 * 
 * FileName:   ITreeViewData.cs
 * Version:    1.0
 * Date:       2014.05.23
 * Author:     Ji
 * 
 *========================================
 * @namespace  Jisons 
 * @class      ITreeViewData<T> where T : class
 * @extends    EventArgs
 * @type       interface
 * 
 *             声明可用树控件的根数据
 * 
 *========================================
 * 
 * Copyright © 迹I柳燕
 * 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Gtk;

namespace CocoStudio.ToolKit
{
    public interface ITreeViewData<T> where T : class
    {
        string GetName(T data);
        void SetName(T data, string name);

        bool GetIsSelected(T data);
        void SetIsSelected(T data, bool value);

        void SetValue(T data, int colum, object value);

        T GetParentPart(T data);

        ObservableCollection<T> FirstDepthChildren { get; }

        ObservableCollection<T> GetTreeViewChildren(T data = null);

        ITreeViewItem GetData(T data = null);

        void AddPropertyChangedEventHandler(T data, PropertyChangedEventHandler handle);

        void RemovePropertyChangedEventHandler(T data, PropertyChangedEventHandler handle);

        List<CellRenderer> AddColumns(TreeView treeview);

        bool CanDrop(T data, object node, DropPosition mode, bool copy);

        void Drop(T data, List<object> node, DropPosition mode, bool copy);

        bool CanAddViewItem(T data);

        bool GetExpanded(T data);

        void SetExpanded(T data, bool value);

    }

    public class ITreeViewItem
    {
        public Type[] Types { get; set; }

        public object[] Values { get; set; }
    }
}
