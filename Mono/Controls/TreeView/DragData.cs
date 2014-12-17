/* 
 * 
 * FileName:   DragData.cs
 * Version:    1.0
 * Date:       2014.05.23
 * Author:     Ji
 * 
 *========================================
 * @namespace  Jisons 
 * @class      DragData
 * @extends    
 * 
 *             用以内部拖拽的数据判断
 *             一般情况下，内部拖拽需要自己处理节点的更改，如更改父子结构，需要自己删除后添加
 *              快捷操作的情况下，只需要删除父子结构和重新添加父子结构
 * 
 *========================================
 * 
 * Copyright © 迹I柳燕
 * 
 */

namespace CocoStudio.ToolKit
{
    /// <summary> 用以内部拖拽的数据判断 </summary>
    public class DragData
    {

        /// <summary> 拖拽数据 </summary>
        public object SelectionDatas;

        /// <summary> 一般情况下，内部拖拽需要自己处理节点的更改，如更改父子结构，需要自己删除后添加
        /// 快捷操作的情况下，只需要删除父子结构和重新添加父子结构</summary>
        public bool IsDo;

    }
}
